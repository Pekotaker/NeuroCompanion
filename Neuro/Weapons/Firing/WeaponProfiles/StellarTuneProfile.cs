using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class StellarTuneProfile
    {
        private const int ProjectileCount = 2;
        private const int DelayBetweenStarsTicks = 5;

        private const float AimNoiseHalfWidth = 48f;
        private const float AimNoiseHalfHeight = 36f;

        private const float MinimumShotSpeed = 7f;
        private const float MaximumShotSpeed = 10f;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.SparkleGuitar;
        }

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
            NeuroWeaponShot[] shots = new NeuroWeaponShot[ProjectileCount];

            for (int i = 0; i < shots.Length; i++)
            {
                Vector2 aimPoint =
                    targetPosition + RollAimNoise();

                Vector2 velocity =
                    SkySpawnProfileHelper.AimAt(
                        basePosition,
                        aimPoint,
                        Main.rand.NextFloat(
                            MinimumShotSpeed,
                            MaximumShotSpeed
                        )
                    );

                velocity =
                    Rotate(
                        velocity,
                        Main.rand.NextFloat(
                            -0.18f,
                            0.18f
                        )
                    );

                shots[i] = new NeuroWeaponShot(
                    ProjectileID.SparkleGuitar,
                    basePosition,
                    velocity,
                    ai0: aimPoint.X,
                    ai1: aimPoint.Y,
                    delayTicks: i * DelayBetweenStarsTicks
                );
            }

            return shots;
        }

        public static int GetCooldownTicks(Item weapon)
        {
            return SkySpawnProfileHelper.GetFullUseCooldownTicks(weapon);
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
            float sin = (float)System.Math.Sin(radians);
            float cos = (float)System.Math.Cos(radians);

            return new Vector2(
                vector.X * cos - vector.Y * sin,
                vector.X * sin + vector.Y * cos
            );
        }
    }
}