using Microsoft.Xna.Framework;
using Terraria;

namespace NeuroCompanion.Projectiles.Helpers
{
    public static class NeuroShotDirectionHelper
    {
        public static Vector2 NormalizeForOwnerFacing(
            Vector2 rawDirection,
            Player owner,
            float minimumLengthSquared
        )
        {
            Vector2 fallbackDirection = Vector2.UnitX * owner.direction;

            return NormalizeWithFallback(
                rawDirection,
                fallbackDirection,
                minimumLengthSquared
            );
        }

        public static Vector2 NormalizeFromTo(
            Vector2 from,
            Vector2 to,
            Vector2 fallbackDirection,
            float minimumLengthSquared
        )
        {
            Vector2 rawDirection = to - from;

            return NormalizeWithFallback(
                rawDirection,
                fallbackDirection,
                minimumLengthSquared
            );
        }

        public static Vector2 NormalizeWithFallback(
            Vector2 rawDirection,
            Vector2 fallbackDirection,
            float minimumLengthSquared
        )
        {
            if (rawDirection.LengthSquared() < minimumLengthSquared)
            {
                rawDirection = fallbackDirection;
            }

            return rawDirection.SafeNormalize(fallbackDirection);
        }
    }
}