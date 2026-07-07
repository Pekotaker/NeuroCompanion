using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

using NeuroCompanion.Neuro.Weapons.Firing;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class BeeGunProfile
    {
        private const float MaxAngleOffsetDegrees = 10f;
        private const float MinSpeedMultiplier = 0.85f;
        private const float MaxSpeedMultiplier = 1.15f;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.BeeGun;
        }

        public static NeuroWeaponShot[] CreateShots(
            Player owner,
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity
        )
        {
            int beeCount = RollBeeCount(owner);

            NeuroWeaponShot[] shots = new NeuroWeaponShot[beeCount];

            for (int i = 0; i < shots.Length; i++)
            {
                int projectileType = owner != null
                    ? owner.beeType()
                    : weapon.shoot;

                shots[i] = new NeuroWeaponShot(
                    projectileType,
                    basePosition,
                    CreateRandomizedVelocity(baseVelocity)
                );
            }

            return shots;
        }

        private static int RollBeeCount(Player owner)
        {
            int beeCount = Main.rand.Next(1, 4);

            if (Main.rand.NextBool(6))
            {
                beeCount++;
            }

            if (Main.rand.NextBool(6))
            {
                beeCount++;
            }

            if (owner != null && owner.strongBees && Main.rand.NextBool(3))
            {
                beeCount++;
            }

            return beeCount;
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