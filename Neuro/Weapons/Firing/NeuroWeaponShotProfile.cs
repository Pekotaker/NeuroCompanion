using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

using NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles;

namespace NeuroCompanion.Neuro.Weapons.Firing
{
    public static class NeuroWeaponShotProfile
    {
        public static NeuroWeaponShot[] CreateShots(
            Player owner,
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
            if (weapon == null || weapon.IsAir)
            {
                return Array.Empty<NeuroWeaponShot>();
            }

            NeuroWeaponShot[] profiledShots =
                NeuroWeaponFiringProfileRegistry.CreateShots(
                    owner,
                    weapon,
                    basePosition,
                    baseVelocity,
                    targetPosition
                );

            if (profiledShots != null)
            {
                return profiledShots;
            }

            int projectileType = GetProjectileType(weapon);

            if (projectileType <= ProjectileID.None)
            {
                return Array.Empty<NeuroWeaponShot>();
            }

            return new[]
            {
                new NeuroWeaponShot(
                    projectileType,
                    basePosition,
                    baseVelocity
                )
            };
        }

        public static int GetProjectileType(Item weapon)
        {
            if (weapon == null || weapon.IsAir)
            {
                return ProjectileID.None;
            }

            int profiledProjectileType =
                NeuroWeaponFiringProfileRegistry.GetProjectileType(weapon);

            if (profiledProjectileType > ProjectileID.None)
            {
                return profiledProjectileType;
            }

            return weapon.shoot;
        }
    }
}