using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Globals;
using NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Holdout;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Beam
{
    public partial class NeuroChargedBlasterBeam : ModProjectile
    {
        public override string Texture =>
            "Terraria/Images/Projectile_" + ProjectileID.ChargedBlasterLaser;

        private int ownerHitCooldown;

        private int HostCannonIndex =>
            (int)Projectile.ai[0];

        private float BeamLength
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = BeamLocalNpcHitCooldownTicks;

            Projectile.alpha = 255;
            Projectile.timeLeft = 2;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            Projectile hostProjectile = GetHostCannon();

            if (
                hostProjectile == null ||
                hostProjectile.ModProjectile
                    is not NeuroChargedBlasterCannonHoldout hostCannon
            )
            {
                Projectile.Kill();
                return;
            }

            if (!hostCannon.BeamPhaseActive)
            {
                Projectile.Kill();
                return;
            }

            if (ownerHitCooldown > 0)
            {
                ownerHitCooldown--;
            }

            Vector2 direction =
                SafeNormalize(hostProjectile.velocity, Vector2.UnitX);

            Projectile.Center =
                hostProjectile.Center + direction * BeamStartDistanceFromCannon;

            Projectile.velocity = direction;
            Projectile.rotation = direction.ToRotation();

            ApplyStateFromHost(
                hostProjectile,
                hostCannon
            );

            bool ignoreTiles =
                hostProjectile
                    .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                    .IgnoreTilesForNeuroMk4;

            float targetBeamLength =
                PerformBeamHitscan(ignoreTiles);

            BeamLength =
                MathHelper.Lerp(
                    BeamLength,
                    targetBeamLength,
                    BeamLengthChangeFactor
                );

            ProduceVisualEffects();

            TryDamageOwnerWithBeam(hostCannon);

            Projectile.timeLeft = 2;
        }

        private Projectile GetHostCannon()
        {
            if (
                HostCannonIndex < 0 ||
                HostCannonIndex >= Main.maxProjectiles
            )
            {
                return null;
            }

            Projectile hostProjectile =
                Main.projectile[HostCannonIndex];

            if (
                hostProjectile == null ||
                !hostProjectile.active ||
                hostProjectile.type !=
                ModContent.ProjectileType<NeuroChargedBlasterCannonHoldout>()
            )
            {
                return null;
            }

            return hostProjectile;
        }

        private void ApplyStateFromHost(
            Projectile hostProjectile,
            NeuroChargedBlasterCannonHoldout hostCannon
        )
        {
            int beamDamage =
                (int)(hostProjectile.damage * BeamDamageMultiplier);

            if (beamDamage < 1)
            {
                beamDamage = 1;
            }

            Projectile.damage = beamDamage;
            Projectile.originalDamage = beamDamage;
            Projectile.CritChance = hostProjectile.CritChance;

            Projectile.friendly = !hostCannon.OwnerDamageEnabled;
            Projectile.hostile = false;

            Projectile
                .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                .IgnoreTilesForNeuroMk4 =
                    hostProjectile
                        .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                        .IgnoreTilesForNeuroMk4;

            EvilNeuroPlayerAttackGlobal evilGlobal =
                Projectile.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            // Owner damage for this beam is handled by TryDamageOwnerWithBeam.
            // Do not let the generic Evil Neuro hitbox logic hit the player.
            evilGlobal.CanDamageOwner = false;
            evilGlobal.KillOnOwnerHit = false;
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