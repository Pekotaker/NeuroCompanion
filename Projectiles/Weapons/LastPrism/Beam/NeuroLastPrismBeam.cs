using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Weapons.LastPrism.Holdout;

namespace NeuroCompanion.Projectiles.Weapons.LastPrism.Beam
{
    public partial class NeuroLastPrismBeam : ModProjectile
    {
        public override string Texture =>
            "Terraria/Images/Projectile_" + ProjectileID.LastPrismLaser;

        private int ownerHitCooldown;

        private int BeamID => (int)Projectile.ai[0];

        private int HostPrismIndex => (int)Projectile.ai[1];

        private float BeamLength
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.penetrate = -1;
            Projectile.alpha = 255;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.timeLeft = 2;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            Projectile hostPrism = GetHostPrism();

            if (hostPrism == null)
            {
                Projectile.Kill();
                return;
            }

            UpdateOwnerHitCooldown();

            float chargeRatio =
                GetChargeRatio(hostPrism);

            UpdateDamageFromHost(
                hostPrism,
                chargeRatio
            );

            ApplyDamageBehaviorFromHost(
                hostPrism,
                chargeRatio
            );

            Vector2 hostDirection =
                SafeNormalize(hostPrism.velocity, -Vector2.UnitY);

            BeamFocusState focusState =
                UpdateFocusState(chargeRatio);

            PositionBeamAroundHost(
                hostPrism,
                hostDirection,
                focusState
            );

            bool ignoreTiles =
                CopyMk4BehaviorFromHost(hostPrism);

            UpdateBeamLength(ignoreTiles);

            ProduceVisualEffects(chargeRatio);

            TryDamageOwnerWithBeam();

            Projectile.timeLeft = 2;
        }


        private Projectile GetHostPrism()
        {
            int hostIndex = HostPrismIndex;

            if (hostIndex < 0 || hostIndex >= Main.maxProjectiles)
            {
                return null;
            }

            Projectile hostPrism = Main.projectile[hostIndex];

            if (
                hostPrism == null ||
                !hostPrism.active ||
                hostPrism.type != ModContent.ProjectileType<NeuroLastPrismHoldout>()
            )
            {
                return null;
            }

            return hostPrism;
        }

        private static Vector2 SafeNormalize(
            Vector2 value,
            Vector2 fallback
        )
        {
            if (value.LengthSquared() <= 0.01f || value.HasNaNs())
            {
                return fallback;
            }

            value.Normalize();
            return value;
        }
    }
}