using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles.Weapons.MedusaHead.Ray
{
    public partial class NeuroMedusaHeadRay : ModProjectile
    {
        public override string Texture =>
            "Terraria/Images/Projectile_" +
            ProjectileID.MedusaHeadRay;

        private bool initialized;
        private bool ownerDamageApplied;

        private int age;
        private float rayLength;

        private bool OwnerDamageEnabled =>
            Projectile.ai[0] >= 0.5f;

        private bool IgnoreBlocks =>
            Projectile.ai[1] >= 0.5f;

        private Vector2 RayDirection =>
            SafeNormalize(
                Projectile.velocity,
                Vector2.UnitX
            );

        private Vector2 RayEnd =>
            Projectile.Center +
            RayDirection * rayLength;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.timeLeft = LifetimeTicks;
            Projectile.aiStyle = 0;
            Projectile.netImportant = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? CanDamage()
        {
            if (OwnerDamageEnabled)
            {
                return false;
            }

            return age <= DamageWindowTicks;
        }

        public override void AI()
        {
            InitializeIfNeeded();

            age++;

            if (
                OwnerDamageEnabled &&
                !ownerDamageApplied
            )
            {
                TryDamageOwner();
            }

            Projectile.rotation =
                RayDirection.ToRotation();
        }

        private void InitializeIfNeeded()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;

            Vector2 direction = RayDirection;

            float requestedLength =
                Projectile.velocity.Length();

            if (requestedLength <= 0.01f)
            {
                Projectile.Kill();
                return;
            }

            rayLength =
                GetAvailableRayLength(
                    direction,
                    requestedLength
                );

            Projectile.velocity =
                direction * rayLength;

            Projectile.rotation =
                direction.ToRotation();
        }

        private float GetAvailableRayLength(
            Vector2 direction,
            float requestedLength
        )
        {
            if (IgnoreBlocks)
            {
                return requestedLength;
            }

            float[] samples =
                new float[LaserScanSampleCount];

            Collision.LaserScan(
                Projectile.Center,
                direction,
                RayCollisionWidth,
                requestedLength,
                samples
            );

            float totalDistance = 0f;

            for (int i = 0; i < samples.Length; i++)
            {
                totalDistance += samples[i];
            }

            float availableLength =
                totalDistance / samples.Length;

            return MathHelper.Clamp(
                availableLength,
                0f,
                requestedLength
            );
        }

        private void TryDamageOwner()
        {
            Player owner = GetOwner();

            if (owner == null)
            {
                return;
            }

            float collisionPoint = 0f;

            bool intersectsOwner =
                Collision.CheckAABBvLineCollision(
                    owner.position,
                    new Vector2(
                        owner.width,
                        owner.height
                    ),
                    Projectile.Center,
                    RayEnd,
                    RayCollisionWidth,
                    ref collisionPoint
                );

            if (!intersectsOwner)
            {
                return;
            }

            ownerDamageApplied = true;

            int hitDirection =
                owner.Center.X < Projectile.Center.X
                    ? -1
                    : 1;

            PlayerDeathReason deathReason =
                PlayerDeathReason.ByCustomReason(
                    $"{owner.name} was petrified by Evil Neuro."
                );

            owner.Hurt(
                deathReason,
                Projectile.damage,
                hitDirection,
                pvp: true
            );
        }

        private Player GetOwner()
        {
            if (
                Projectile.owner < 0 ||
                Projectile.owner >= Main.maxPlayers
            )
            {
                return null;
            }

            Player owner = Main.player[Projectile.owner];

            if (
                owner == null ||
                !owner.active ||
                owner.dead
            )
            {
                return null;
            }

            return owner;
        }

        private static Vector2 SafeNormalize(
            Vector2 value,
            Vector2 fallback
        )
        {
            if (
                value.LengthSquared() <= 0.01f ||
                value.HasNaNs()
            )
            {
                return fallback;
            }

            value.Normalize();
            return value;
        }
    }
}