using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Holdout;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class ChargedBlasterCannonProfile
    {
        private const float FallbackAimSpeed = 30f;
        private const int RefreshCooldownTicks = 10;

        public static int ProjectileType =>
            ModContent.ProjectileType<NeuroChargedBlasterCannonHoldout>();

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.ChargedBlasterCannon;
        }

        public static NeuroWeaponShot[] CreateShots(
            Player owner,
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
            if (TryRefreshExistingCannon(owner, targetPosition))
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

            return new[]
            {
                new NeuroWeaponShot(
                    ProjectileType,
                    basePosition,
                    direction * FallbackAimSpeed,
                    ai0: targetPosition.X,
                    ai1: targetPosition.Y,
                    forceVisible: true,
                    useSkyDrawLayer: true
                )
            };
        }

        public static int GetCooldownTicks(int channelTicks)
        {
            return RefreshCooldownTicks;
        }

        private static bool TryRefreshExistingCannon(
            Player owner,
            Vector2 targetPosition
        )
        {
            if (owner == null)
            {
                return false;
            }

            int cannonType =
                ModContent.ProjectileType<NeuroChargedBlasterCannonHoldout>();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];

                if (
                    projectile != null &&
                    projectile.active &&
                    projectile.owner == owner.whoAmI &&
                    projectile.type == cannonType
                )
                {
                    if (
                        projectile.ModProjectile
                        is NeuroChargedBlasterCannonHoldout cannon
                    )
                    {
                        cannon.RefreshFromContext(
                            targetPosition,
                            NeuroWeaponProjectileSpawnContext.Current
                        );
                    }

                    return true;
                }
            }

            return false;
        }
    }
}