using Microsoft.Xna.Framework;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon
{
    public static class ChargedBlasterCannonEffectColor
    {
        public const byte Red = 222;
        public const byte Green = 255;
        public const byte Blue = 255;

        public static Color GetColor(byte alpha = 255)
        {
            return new Color(
                Red,
                Green,
                Blue,
                alpha
            );
        }
    }
}