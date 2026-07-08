using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class ShadowflameHexDollProfile
    {
        private const int ProjectileCount = 3;
        private const int DelayBetweenTentaclesTicks = 4;

        private const float SpawnRadiusAroundNeuro = 18f;

        private const float AimNoiseHalfWidth = 56f;
        private const float AimNoiseHalfHeight = 36f;

        private const float AngleNoiseDegrees = 8f;

        private const float MinimumSpeedMultiplier = 0.9f;
        private const float MaximumSpeedMultiplier = 1.1f;

        private const float MinimumBendAcceleration = 0.025f;
        private const float MaximumBendAcceleration = 0.05f;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.ShadowFlameHexDoll;
        }

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
            NeuroWeaponShot[] shots = new NeuroWeaponShot[ProjectileCount];

            float shotSpeed =
                SkySpawnProfileHelper.GetShotSpeed(
                    weapon,
                    baseVelocity
                );

            for (int i = 0; i < shots.Length; i++)
            {
                Vector2 spawnPosition =
                    basePosition + RollSpawnOffset();

                Vector2 aimPoint =
                    targetPosition + RollAimNoise();

                Vector2 velocity =
                    SkySpawnProfileHelper.AimAt(
                        spawnPosition,
                        aimPoint,
                        shotSpeed
                    );

                velocity =
                    Rotate(
                        velocity,
                        Main.rand.NextFloat(
                            -MathHelper.ToRadians(AngleNoiseDegrees),
                            MathHelper.ToRadians(AngleNoiseDegrees)
                        )
                    );

                velocity *= Main.rand.NextFloat(
                    MinimumSpeedMultiplier,
                    MaximumSpeedMultiplier
                );

                Vector2 bendAcceleration =
                    CreateBendAcceleration(
                        velocity,
                        i
                    );

                shots[i] = new NeuroWeaponShot(
                    weapon.shoot,
                    spawnPosition,
                    velocity,
                    ai0: bendAcceleration.X,
                    ai1: bendAcceleration.Y,
                    delayTicks: i * DelayBetweenTentaclesTicks
                );
            }

            return shots;
        }

        public static int GetCooldownTicks(Item weapon)
        {
            return SkySpawnProfileHelper.GetFullUseCooldownTicks(weapon);
        }

        private static Vector2 CreateBendAcceleration(
            Vector2 velocity,
            int shotIndex
        )
        {
            if (velocity.LengthSquared() <= 0.01f)
            {
                return Vector2.Zero;
            }

            Vector2 direction = velocity;
            direction.Normalize();

            Vector2 perpendicular = new Vector2(
                -direction.Y,
                direction.X
            );

            float bendStrength = Main.rand.NextFloat(
                MinimumBendAcceleration,
                MaximumBendAcceleration
            );

            // Alternate left/right so the volley does not all bend the same way.
            if (shotIndex % 2 == 1)
            {
                bendStrength *= -1f;
            }

            // Occasionally flip anyway so it does not become too predictable.
            if (Main.rand.NextBool(3))
            {
                bendStrength *= -1f;
            }

            return perpendicular * bendStrength;
        }

        private static Vector2 RollSpawnOffset()
        {
            float angle = Main.rand.NextFloat(MathHelper.TwoPi);
            float distance = Main.rand.NextFloat(0f, SpawnRadiusAroundNeuro);

            return new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle)
            ) * distance;
        }

        private static Vector2 RollAimNoise()
        {
            return new Vector2(
                Main.rand.NextFloat(-AimNoiseHalfWidth, AimNoiseHalfWidth),
                Main.rand.NextFloat(-AimNoiseHalfHeight, AimNoiseHalfHeight)
            );
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