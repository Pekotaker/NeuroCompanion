using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class SkyFractureProfile
    {
        private const int ProjectileCount = 3;
        private const int DelayBetweenSwordsTicks = 3;
        private const int CooldownTicks = 30;

        private const float MinSpawnDistanceFromNeuro = 24f;
        private const float MaxSpawnDistanceFromNeuro = 60f;
        private const float ForwardBiasDistance = 12f;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.SkyFracture;
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

            Vector2 aimDirection =
                GetAimDirection(
                    basePosition,
                    baseVelocity,
                    targetPosition
                );

            for (int i = 0; i < shots.Length; i++)
            {
                Vector2 spawnPosition =
                    basePosition +
                    aimDirection * ForwardBiasDistance +
                    RollRandomOffsetAroundNeuro();

                Vector2 velocity =
                    SkySpawnProfileHelper.AimAt(
                        spawnPosition,
                        targetPosition,
                        shotSpeed
                    );

                shots[i] = new NeuroWeaponShot(
                    weapon.shoot,
                    spawnPosition,
                    velocity,
                    delayTicks: i * DelayBetweenSwordsTicks
                );
            }

            return shots;
        }

        public static int GetCooldownTicks()
        {
            return CooldownTicks;
        }

        private static Vector2 RollRandomOffsetAroundNeuro()
        {
            float angle = Main.rand.NextFloat(MathHelper.TwoPi);

            float distance = Main.rand.NextFloat(
                MinSpawnDistanceFromNeuro,
                MaxSpawnDistanceFromNeuro
            );

            return new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle)
            ) * distance;
        }

        private static Vector2 GetAimDirection(
            Vector2 basePosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
            if (baseVelocity.LengthSquared() > 0.01f)
            {
                Vector2 direction = baseVelocity;
                direction.Normalize();
                return direction;
            }

            Vector2 fallbackDirection = targetPosition - basePosition;

            if (fallbackDirection.LengthSquared() <= 0.01f)
            {
                return Vector2.UnitX;
            }

            fallbackDirection.Normalize();
            return fallbackDirection;
        }
    }
}