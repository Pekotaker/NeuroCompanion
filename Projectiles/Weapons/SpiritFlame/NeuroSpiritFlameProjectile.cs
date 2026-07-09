using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles.Weapons.SpiritFlame
{
    public partial class NeuroSpiritFlameProjectile : ModProjectile
    {
        public override string Texture =>
            "Terraria/Images/Projectile_" + ProjectileID.SpiritFlame;

        private Vector2 AnchorPosition
        {
            get => new Vector2(Projectile.ai[0], Projectile.ai[1]);
            set
            {
                Projectile.ai[0] = value.X;
                Projectile.ai[1] = value.Y;
            }
        }

        private bool OwnerAttackMode =>
            Projectile.ai[2] >= 0.5f;

        private bool hasStartedHoming;
        private bool hasImpacted;
        private float age;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] =
                Main.projFrames[ProjectileID.SpiritFlame];
        }

        public override void SetDefaults()
        {
            Projectile.width = ProjectileWidth;
            Projectile.height = ProjectileHeight;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = LifetimeTicks;
            Projectile.aiStyle = 0;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            age++;

            if (AnchorPosition == Vector2.Zero)
            {
                AnchorPosition = Projectile.Center;
            }

            if (OwnerAttackMode)
            {
                Projectile.friendly = false;
                Projectile.hostile = false;

                HomeTowardOwner();
                UpdateAnimation();
                EmitAmbientEffects();

                return;
            }

            Projectile.friendly = true;
            Projectile.hostile = false;

            NPC target = FindTarget();

            if (target != null)
            {
                hasStartedHoming = true;
                HomeTowardTarget(target);
            }
            else
            {
                if (hasStartedHoming)
                {
                    hasStartedHoming = false;
                    AnchorPosition = Projectile.Center;
                    Projectile.velocity = Vector2.Zero;
                }

                HoverAroundAnchor();
            }

            UpdateAnimation();
            EmitAmbientEffects();
        }
    }
}