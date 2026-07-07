using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class FangStaffProfile
    {
        private const float MaxSpreadDegrees = 15f;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   (
                       weapon.type == ItemID.PoisonStaff ||
                       weapon.type == ItemID.VenomStaff
                   );
        }

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity
        )
        {
            int projectileCount = RollProjectileCount(weapon);

            NeuroWeaponShot[] shots =
                new NeuroWeaponShot[projectileCount];

            float[] angleOffsets =
                CreateAngleOffsets(projectileCount);

            for (int i = 0; i < shots.Length; i++)
            {
                shots[i] = new NeuroWeaponShot(
                    weapon.shoot,
                    basePosition,
                    Rotate(baseVelocity, angleOffsets[i])
                );
            }

            return shots;
        }

        private static int RollProjectileCount(Item weapon)
        {
            if (weapon.type == ItemID.PoisonStaff)
            {
                return Main.rand.NextBool(3)
                    ? 4
                    : 3;
            }

            if (weapon.type == ItemID.VenomStaff)
            {
                int roll = Main.rand.Next(60);

                if (roll < 24)
                {
                    return 4;
                }

                if (roll < 50)
                {
                    return 5;
                }

                if (roll < 59)
                {
                    return 6;
                }

                return 7;
            }

            return 1;
        }

        private static float[] CreateAngleOffsets(int projectileCount)
        {
            float[] angleOffsets = new float[projectileCount];

            if (projectileCount <= 0)
            {
                return angleOffsets;
            }

            // One fang tracks the target exactly.
            angleOffsets[0] = 0f;

            if (projectileCount == 1)
            {
                return angleOffsets;
            }

            int remainingProjectiles = projectileCount - 1;

            int lowerCount = remainingProjectiles / 2;
            int upperCount = remainingProjectiles - lowerCount;

            // For even total projectile counts, randomly choose which side
            // gets the extra fang so the spread does not always lean one way.
            if (upperCount != lowerCount && Main.rand.NextBool())
            {
                int temporary = upperCount;
                upperCount = lowerCount;
                lowerCount = temporary;
            }

            int index = 1;

            for (int i = 0; i < lowerCount; i++)
            {
                angleOffsets[index] = -RollRandomSpreadRadians();
                index++;
            }

            for (int i = 0; i < upperCount; i++)
            {
                angleOffsets[index] = RollRandomSpreadRadians();
                index++;
            }

            return angleOffsets;
        }

        private static float RollRandomSpreadRadians()
        {
            float spreadDegrees = Main.rand.NextFloat(
                0f,
                MaxSpreadDegrees
            );

            return MathHelper.ToRadians(spreadDegrees);
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