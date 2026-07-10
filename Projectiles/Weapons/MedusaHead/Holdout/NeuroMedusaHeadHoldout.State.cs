using Microsoft.Xna.Framework;

using Terraria;

using NeuroCompanion.Neuro.Weapons.Firing;
using NeuroCompanion.Projectiles.Globals;

namespace NeuroCompanion.Projectiles.Weapons.MedusaHead.Holdout
{
    public partial class NeuroMedusaHeadHoldout
    {
        private Vector2 TargetPosition =>
            new Vector2(
                Projectile.ai[0],
                Projectile.ai[1]
            );

        private bool SingleShotOnly =>
            Projectile.ai[2] >= 0.5f;

        public bool OwnerDamageEnabled =>
            ownerDamageEnabled;

        public bool IgnoreBlocks =>
            Projectile
                .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                .IgnoreTilesForNeuroMk4;

        public void RefreshFromContext(
            Vector2 targetPosition,
            NeuroWeaponProjectileSpawnContext context
        )
        {
            if (context != null)
            {
                ApplySpawnContext(context);
            }

            RefreshTarget(targetPosition);

            Projectile.netUpdate = true;
        }

        private void InitializeIfNeeded()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;

            fallbackAnchorPosition = Projectile.Center;

            remainingLifeTicks =
                SingleShotOnly
                    ? SingleShotLifetimeTicks
                    : RefreshGraceTicks;

            ownerDamageEnabled =
                Projectile
                    .GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>()
                    .CanDamageOwner;
        }

        private void RefreshTarget(Vector2 targetPosition)
        {
            Projectile.ai[0] = targetPosition.X;
            Projectile.ai[1] = targetPosition.Y;

            int minimumLifeTicks =
                SingleShotOnly
                    ? SingleShotLifetimeTicks
                    : RefreshGraceTicks;

            if (remainingLifeTicks < minimumLifeTicks)
            {
                remainingLifeTicks = minimumLifeTicks;
            }
        }

        private void ApplySpawnContext(
            NeuroWeaponProjectileSpawnContext context
        )
        {
            Projectile.damage = context.Damage;
            Projectile.originalDamage = context.Damage;
            Projectile.CritChance = context.CritChance;

            Projectile.ai[2] =
                context.SingleShotOnly ? 1f : 0f;

            ownerDamageEnabled = context.IsEvil;

            Projectile.friendly = false;
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
    }
}