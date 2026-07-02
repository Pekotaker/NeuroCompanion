using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class LaserMachinegunProfile
    {
        public const int SpinUpTicks = 120;

        public const int InitialCooldownTicks = 30;
        public const int FullSpeedCooldownTicks = 6;

        public const int ProjectileCount = 2;
        public const float MaxInaccuracyDegrees = 5f;

        public static int ProjectileType => ProjectileID.LaserMachinegunLaser;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.LaserMachinegun;
        }

        public static int GetCooldownTicks(int channelTicks)
        {
            float spinUpProgress = Clamp01(
                channelTicks / (float)SpinUpTicks
            );

            int cooldownTicks = (int)Math.Round(
                Lerp(
                    InitialCooldownTicks,
                    FullSpeedCooldownTicks,
                    spinUpProgress
                )
            );

            if (cooldownTicks < FullSpeedCooldownTicks)
            {
                return FullSpeedCooldownTicks;
            }

            return cooldownTicks;
        }

        public static Vector2[] CreateShotVelocities(Vector2 baseVelocity)
        {
            Vector2[] velocities = new Vector2[ProjectileCount];

            if (baseVelocity.LengthSquared() <= 0.01f)
            {
                for (int i = 0; i < velocities.Length; i++)
                {
                    velocities[i] = baseVelocity;
                }

                return velocities;
            }

            float maxSpreadRadians = MathHelper.ToRadians(MaxInaccuracyDegrees);

            for (int i = 0; i < velocities.Length; i++)
            {
                float angle = Main.rand.NextFloat(
                    -maxSpreadRadians,
                    maxSpreadRadians
                );

                velocities[i] = Rotate(baseVelocity, angle);
            }

            return velocities;
        }

        private static float Clamp01(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }

            if (value > 1f)
            {
                return 1f;
            }

            return value;
        }

        private static float Lerp(
            float from,
            float to,
            float amount
        )
        {
            return from + (to - from) * amount;
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
    }
}