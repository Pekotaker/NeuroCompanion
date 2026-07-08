using Microsoft.Xna.Framework;

using NeuroCompanion.Neuro.Weapons.Firing;
using NeuroCompanion.Projectiles.Globals;

namespace NeuroCompanion.Projectiles.Weapons.LastPrism.Holdout
{
    public partial class NeuroLastPrismHoldout
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

            ApplyOwnerDamageMode(
                context.IsEvil,
                context.KillOnOwnerHit
            );

            ApplyMk4ModeFromContext(context);
        }

        private void ApplyOwnerDamageMode(
            bool ownerDamageEnabled,
            bool killOnOwnerHit
        )
        {
            // The holdout itself cannot damage, but child beams read this state.
            Projectile.friendly = !ownerDamageEnabled;
            Projectile.hostile = false;

            EvilNeuroPlayerAttackGlobal evilGlobal =
                Projectile.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            evilGlobal.CanDamageOwner = ownerDamageEnabled;
            evilGlobal.KillOnOwnerHit = ownerDamageEnabled && killOnOwnerHit;
        }

        private void ApplyMk4ModeFromContext(
            NeuroWeaponProjectileSpawnContext context
        )
        {
            bool ignoreTiles =
                context.NeuroPlayer != null &&
                context.NeuroPlayer.NeuroStaffCanDetectThroughBlocks;

            Projectile
                .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                .IgnoreTilesForNeuroMk4 = ignoreTiles;

            // The prism body itself should never move/collide like a normal projectile.
            Projectile.tileCollide = false;
        }
    }
}