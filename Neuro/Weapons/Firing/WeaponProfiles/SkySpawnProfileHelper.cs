using Microsoft.Xna.Framework;

using Terraria;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class SkySpawnProfileHelper
    {
        private const float MinimumVelocityLengthSquared = 0.01f;
        private const float FallbackShotSpeed = 11f;

        public static float GetShotSpeed(Item weapon, Vector2 baseVelocity)
        {
            if (baseVelocity.LengthSquared() > MinimumVelocityLengthSquared)
            {
                return baseVelocity.Length();
            }

            if (weapon != null && weapon.shootSpeed > 0f)
            {
                return weapon.shootSpeed;
            }

            return FallbackShotSpeed;
        }

        public static Vector2 AimAt(
            Vector2 from,
            Vector2 to,
            float speed
        )
        {
            Vector2 direction = to - from;

            if (direction.LengthSquared() <= MinimumVelocityLengthSquared)
            {
                return Vector2.UnitY * speed;
            }

            direction.Normalize();
            return direction * speed;
        }

        public static float GetNeuroSkyY(Vector2 neuroPosition)
        {
            return neuroPosition.Y - 520f;
        }
    }
}