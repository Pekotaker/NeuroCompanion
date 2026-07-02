using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Neuro.Weapons.Firing
{
    public static class NeuroWeaponShotProfile
    {
        private const float MinimumVelocityLengthSquared = 0.01f;

        public static int GetProjectileType(Item weapon)
        {
            if (weapon == null || weapon.IsAir)
            {
                return ProjectileID.None;
            }

            switch (weapon.type)
            {
                case ItemID.LaserMachinegun:
                    return ProjectileID.LaserMachinegunLaser;

                default:
                    return weapon.shoot;
            }
        }

        public static Vector2[] CreateShotVelocities(
            Item weapon,
            Vector2 baseVelocity
        )
        {
            ShotPattern pattern = GetShotPattern(weapon);

            if (
                pattern.ProjectileCount <= 1 ||
                baseVelocity.LengthSquared() <= MinimumVelocityLengthSquared
            )
            {
                return new[] { baseVelocity };
            }

            return CreateSpreadVelocities(
                baseVelocity,
                pattern.ProjectileCount,
                MathHelper.ToRadians(pattern.TotalSpreadDegrees)
            );
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

        private static Vector2[] CreateSpreadVelocities(
            Vector2 baseVelocity,
            int projectileCount,
            float totalSpreadRadians
        )
        {
            Vector2[] velocities = new Vector2[projectileCount];

            if (projectileCount <= 1)
            {
                velocities[0] = baseVelocity;
                return velocities;
            }

            float firstAngle = -totalSpreadRadians * 0.5f;
            float angleStep = totalSpreadRadians / (projectileCount - 1);

            for (int i = 0; i < projectileCount; i++)
            {
                float angle = firstAngle + angleStep * i;
                velocities[i] = Rotate(baseVelocity, angle);
            }

            return velocities;
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