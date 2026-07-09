using Microsoft.Xna.Framework;

using Terraria;

using NeuroCompanion.Neuro.Weapons.Firing;
using NeuroCompanion.Projectiles.Globals;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Holdout
{
    public partial class NeuroChargedBlasterCannonHoldout
    {
        public void RefreshFromContext(
            Vector2 targetPosition,
            NeuroWeaponProjectileSpawnContext context
        )
        {
            RefreshTarget(targetPosition);

            if (context != null)
            {
                ApplySpawnContext(context);
            }

            Projectile.netUpdate = true;
        }

        private void RefreshTarget(Vector2 targetPosition)
        {
            Projectile.ai[0] = targetPosition.X;
            Projectile.ai[1] = targetPosition.Y;

            if (RemainingLifeTicks < RefreshGraceTicks)
            {
                RemainingLifeTicks = RefreshGraceTicks;
            }
        }

        private void ApplySpawnContext(
            NeuroWeaponProjectileSpawnContext context
        )
        {
            Projectile.damage = context.Damage;
            Projectile.originalDamage = context.Damage;
            Projectile.CritChance = context.CritChance;

            OwnerDamageEnabled = context.IsEvil;

            Projectile.friendly = !OwnerDamageEnabled;
            Projectile.hostile = false;

            Projectile
                .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                .IgnoreTilesForNeuroMk4 =
                    context.NeuroPlayer != null &&
                    context.NeuroPlayer.NeuroStaffCanDetectThroughBlocks;

            PreventGenericOwnerDamageOnHoldout();
        }

        private void PreventGenericOwnerDamageOnHoldout()
        {
            EvilNeuroPlayerAttackGlobal evilGlobal =
                Projectile.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            evilGlobal.CanDamageOwner = false;
            evilGlobal.KillOnOwnerHit = false;
        }

        private void ResetChargeCycle()
        {
            ChargeTicks = 0f;

            rapidOrbTimer = RapidOrbCooldownTicks - 1;
            heavyOrbTimer = HeavyOrbCooldownTicks - 1;
            heavyHumTimer = 0;
            cannonDustTimer = 0;

            smallOrbsFired = 0;
            heavyOrbsFired = 0;

            beamSpawned = false;
            initialPewSoundPlayed = false;

            Projectile.netUpdate = true;
        }

        private void ApplyChildProjectileState(
            int projectileIndex,
            bool useGenericOwnerDamage
        )
        {
            if (
                projectileIndex < 0 ||
                projectileIndex >= Main.maxProjectiles
            )
            {
                return;
            }

            Projectile child = Main.projectile[projectileIndex];

            if (child == null || !child.active)
            {
                return;
            }

            child.CritChance = Projectile.CritChance;
            child.originalDamage = child.damage;

            child
                .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                .IgnoreTilesForNeuroMk4 =
                    Projectile
                        .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                        .IgnoreTilesForNeuroMk4;

            EvilNeuroPlayerAttackGlobal evilGlobal =
                child.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            if (OwnerDamageEnabled)
            {
                child.friendly = false;
                child.hostile = false;

                evilGlobal.CanDamageOwner = useGenericOwnerDamage;
                evilGlobal.KillOnOwnerHit = false;
            }
            else
            {
                child.friendly = true;
                child.hostile = false;

                evilGlobal.CanDamageOwner = false;
                evilGlobal.KillOnOwnerHit = false;
            }
        }
    }
}