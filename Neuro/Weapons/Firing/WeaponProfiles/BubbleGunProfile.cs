using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

using NeuroCompanion.Neuro.Weapons.Firing;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class BubbleGunProfile
    {
        private const int ProjectileCount = 3;

        private const float MaxAngleOffsetDegrees = 15f;

        private const float MinScale = 0.5f;
        private const float MaxScale = 1.2f;

        private const float MinSpeedMultiplier = 0.7f;
        private const float MaxSpeedMultiplier = 1.3f;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.BubbleGun;
        }

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity
        )
        {
            NeuroWeaponShot[] shots =
                new NeuroWeaponShot[ProjectileCount];

            int projectileType = weapon.shoot;

            float maxAngleOffsetRadians =
                MathHelper.ToRadians(MaxAngleOffsetDegrees);

            for (int i = 0; i < shots.Length; i++)
            {
                float angleOffset = Main.rand.NextFloat(
                    -maxAngleOffsetRadians,
                    maxAngleOffsetRadians
                );

                float speedMultiplier = Main.rand.NextFloat(
                    MinSpeedMultiplier,
                    MaxSpeedMultiplier
                );

                float scale = Main.rand.NextFloat(
                    MinScale,
                    MaxScale
                );

                Vector2 velocity =
                    Rotate(baseVelocity, angleOffset) * speedMultiplier;

                shots[i] = new NeuroWeaponShot(
                    projectileType,
                    basePosition,
                    velocity,
                    scale: scale
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
    }
}