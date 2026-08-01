using Microsoft.Xna.Framework;

using Terraria;

namespace NeuroCompanion.Projectiles.Companion
{
    public partial class NeuroCompanionProjectile
    {
        private void UpdateCompanionLighting()
        {
            Color glowColor = GetCompanionGlowColor();

            Lighting.AddLight(
                Projectile.Center,
                glowColor.ToVector3() * NeuroLightIntensity
            );
        }

        private Color GetCompanionGlowColor()
        {
            if (IsEvilVisualActive())
            {
                return new Color(255, 70, 70);
            }

            switch (GetOwnerStaffVisualTier())
            {
                case 2:
                    return new Color(255, 220, 140);

                case 3:
                    return new Color(255, 235, 200);

                case 4:
                    return new Color(120, 255, 245);

                default:
                    return new Color(255, 170, 90);
            }
        }
    }
}