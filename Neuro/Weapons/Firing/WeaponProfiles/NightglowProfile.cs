using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class NightglowProfile
    {
        private const int ProjectileCount = 4;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.FairyQueenMagicItem;
        }

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
            NeuroWeaponShot[] shots = new NeuroWeaponShot[ProjectileCount];

            float baseHue = Main.rand.NextFloat();

            for (int i = 0; i < shots.Length; i++)
            {
                Vector2 velocity = RollNightglowBloomVelocity();

                shots[i] = new NeuroWeaponShot(
                    weapon.shoot,
                    basePosition,
                    velocity,
                    ai0: -1f,
                    ai1: GetAdjacentHue(baseHue, i)
                );
            }

            return shots;
        }

        public static int GetCooldownTicks(Item weapon)
        {
            return SkySpawnProfileHelper.GetFullUseCooldownTicks(weapon);
        }

        private static Vector2 RollNightglowBloomVelocity()
        {
            Vector2 velocity =
                Main.rand.NextVector2Circular(1f, 1f) +
                Main.rand.NextVector2CircularEdge(3f, 3f);

            // Terraria Y+ is downward.
            // Vanilla Nightglow flips downward bloom into upward bloom.
            if (velocity.Y > 0f)
            {
                velocity.Y *= -1f;
            }

            return velocity;
        }

        private static float GetAdjacentHue(float baseHue, int shotIndex)
        {
            // Keep one volley in a nearby color family instead of randomizing
            // every projectile across the whole rainbow.
            float hueOffset =
                (shotIndex - (ProjectileCount - 1) * 0.5f) * 0.035f;

            float hue = baseHue + hueOffset;

            if (hue < 0f)
            {
                hue += 1f;
            }

            if (hue >= 1f)
            {
                hue -= 1f;
            }

            return hue;
        }
    }
}