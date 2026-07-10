using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Weapons.MedusaHead.Holdout;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class MedusaHeadProfile
    {
        private const float FallbackAimSpeed = 30f;
        private const int RefreshCooldownTicks = 5;

        public static int ProjectileType =>
            ModContent.ProjectileType<NeuroMedusaHeadHoldout>();

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.MedusaHead;
        }

        public static NeuroWeaponShot[] CreateShots(
            Player owner,
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
            if (TryRefreshExistingHead(owner, targetPosition))
            {
                return Array.Empty<NeuroWeaponShot>();
            }

            Vector2 direction =
                targetPosition - basePosition;

            if (direction.LengthSquared() <= 0.01f)
            {
                direction = baseVelocity;
            }

            if (direction.LengthSquared() <= 0.01f)
            {
                direction = Vector2.UnitX;
            }

            direction.Normalize();

            float aimSpeed =
                weapon != null && weapon.shootSpeed > 0f
                    ? weapon.shootSpeed
                    : FallbackAimSpeed;

            bool singleShotOnly =
                NeuroWeaponProjectileSpawnContext.Current != null &&
                NeuroWeaponProjectileSpawnContext.Current.SingleShotOnly;

            return new[]
            {
                new NeuroWeaponShot(
                    ProjectileType,
                    basePosition,
                    direction * aimSpeed,
                    ai0: targetPosition.X,
                    ai1: targetPosition.Y,
                    ai2: singleShotOnly ? 1f : 0f,
                    forceVisible: true
                )
            };
        }

        public static int GetCooldownTicks(int channelTicks)
        {
            return RefreshCooldownTicks;
        }

        private static bool TryRefreshExistingHead(
            Player owner,
            Vector2 targetPosition
        )
        {
            if (owner == null)
            {
                return false;
            }

            int holdoutType =
                ModContent.ProjectileType<NeuroMedusaHeadHoldout>();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];

                if (
                    projectile == null ||
                    !projectile.active ||
                    projectile.owner != owner.whoAmI ||
                    projectile.type != holdoutType
                )
                {
                    continue;
                }

                if (
                    projectile.ModProjectile
                    is NeuroMedusaHeadHoldout holdout
                )
                {
                    holdout.RefreshFromContext(
                        targetPosition,
                        NeuroWeaponProjectileSpawnContext.Current
                    );
                }

                return true;
            }

            return false;
        }
    }
}