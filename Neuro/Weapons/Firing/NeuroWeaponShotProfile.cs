using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

using NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles;

namespace NeuroCompanion.Neuro.Weapons.Firing
{
    public static class NeuroWeaponShotProfile
    {
        private const float MinimumVelocityLengthSquared = 0.01f;

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity
        )
        {
            if (weapon == null || weapon.IsAir)
            {
                return Array.Empty<NeuroWeaponShot>();
            }

            NeuroWeaponShot[] profiledShots =
                NeuroWeaponFiringProfileRegistry.CreateShots(
                    weapon,
                    basePosition,
                    baseVelocity
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

            ShotPattern pattern = GetShotPattern(weapon);

            if (
                pattern.ProjectileCount <= 1 ||
                baseVelocity.LengthSquared() <= MinimumVelocityLengthSquared
            )
            {
                return new[]
                {
                    new NeuroWeaponShot(
                        projectileType,
                        basePosition,
                        baseVelocity
                    )
                };
            }

            return CreateSpreadShots(
                projectileType,
                basePosition,
                baseVelocity,
                pattern.ProjectileCount,
                MathHelper.ToRadians(pattern.TotalSpreadDegrees)
            );
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

        private static ShotPattern GetShotPattern(Item weapon)
        {
            if (weapon == null || weapon.IsAir)
            {
                return ShotPattern.Single;
            }

            switch (weapon.type)
            {
                case ItemID.SkyFracture:
                    return new ShotPattern(
                        projectileCount: 3,
                        totalSpreadDegrees: 10f
                    );

                case ItemID.BeeGun:
                    return new ShotPattern(
                        projectileCount: 3,
                        totalSpreadDegrees: 12f
                    );

                case ItemID.BatScepter:
                    return new ShotPattern(
                        projectileCount: 3,
                        totalSpreadDegrees: 14f
                    );

                case ItemID.BubbleGun:
                    return new ShotPattern(
                        projectileCount: 3,
                        totalSpreadDegrees: 16f
                    );

                case ItemID.LunarFlareBook:
                    return new ShotPattern(
                        projectileCount: 3,
                        totalSpreadDegrees: 12f
                    );

                default:
                    return ShotPattern.Single;
            }
        }

        private static NeuroWeaponShot[] CreateSpreadShots(
            int projectileType,
            Vector2 basePosition,
            Vector2 baseVelocity,
            int projectileCount,
            float totalSpreadRadians
        )
        {
            NeuroWeaponShot[] shots = new NeuroWeaponShot[projectileCount];

            if (projectileCount <= 1)
            {
                shots[0] = new NeuroWeaponShot(
                    projectileType,
                    basePosition,
                    baseVelocity
                );

                return shots;
            }

            float firstAngle = -totalSpreadRadians * 0.5f;
            float angleStep = totalSpreadRadians / (projectileCount - 1);

            for (int i = 0; i < projectileCount; i++)
            {
                float angle = firstAngle + angleStep * i;

                shots[i] = new NeuroWeaponShot(
                    projectileType,
                    basePosition,
                    Rotate(baseVelocity, angle)
                );
            }

            return shots;
        }

        private static Vector2 Rotate(Vector2 vector, float radians)
        {
            float sin = (float)Math.Sin(radians);
            float cos = (float)Math.Cos(radians);

            return new Vector2(
                vector.X * cos - vector.Y * sin,
                vector.X * sin + vector.Y * cos
            );
        }

        private readonly struct ShotPattern
        {
            public static ShotPattern Single => new ShotPattern(
                projectileCount: 1,
                totalSpreadDegrees: 0f
            );

            public int ProjectileCount { get; }
            public float TotalSpreadDegrees { get; }

            public ShotPattern(
                int projectileCount,
                float totalSpreadDegrees
            )
            {
                ProjectileCount = projectileCount;
                TotalSpreadDegrees = totalSpreadDegrees;
            }
        }
    }
}