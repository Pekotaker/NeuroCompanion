using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

using NeuroCompanion.Neuro.Weapons.Firing;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class BatScepterProfile
    {
        private const int MinBatCount = 2;
        private const int MaxBatCount = 3;

        private const float MaxAngleOffsetDegrees = 10f;
        private const float MinSpeedMultiplier = 0.85f;
        private const float MaxSpeedMultiplier = 1.15f;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.BatScepter;
        }

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity
        )
        {
            int batCount = Main.rand.Next(
                MinBatCount,
                MaxBatCount + 1
            );

            NeuroWeaponShot[] shots = new NeuroWeaponShot[batCount];

            for (int i = 0; i < shots.Length; i++)
            {
                shots[i] = new NeuroWeaponShot(
                    weapon.shoot,
                    basePosition,
                    CreateRandomizedVelocity(baseVelocity)
                );
            }

            return shots;
        }

        private static Vector2 CreateRandomizedVelocity(Vector2 baseVelocity)
        {
            float maxAngleOffsetRadians =
                MathHelper.ToRadians(MaxAngleOffsetDegrees);

            float angleOffset = Main.rand.NextFloat(
                -maxAngleOffsetRadians,
                maxAngleOffsetRadians
            );

            float speedMultiplier = Main.rand.NextFloat(
                MinSpeedMultiplier,
                MaxSpeedMultiplier
            );

            return Rotate(baseVelocity, angleOffset) * speedMultiplier;
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