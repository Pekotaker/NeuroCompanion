using Microsoft.Xna.Framework;

namespace NeuroCompanion.Projectiles.Weapons.SpiritFlame
{
    public partial class NeuroSpiritFlameProjectile
    {
        private const int ProjectileWidth = 18;
        private const int ProjectileHeight = 18;

        private const int LifetimeTicks = 180;

        private const int AnimationFrameDurationTicks = 5;

        private const float TargetSearchRange = 25f * 16f;

        private const float EnemyHomingSpeed = 9f;
        private const float EnemyHomingTurnStrength = 0.12f;

        private const float OwnerHomingSpeed = 10f;
        private const float OwnerHomingTurnStrength = 0.16f;
        private const float OwnerHitboxPadding = 8f;

        private const float HoverHorizontalDistance = 6f;
        private const float HoverVerticalDistance = 4f;
        private const float HoverHorizontalSpeed = 0.05f;
        private const float HoverVerticalSpeed = 0.07f;

        private const int AmbientDustChanceDenominator = 3;
        private const float AmbientDustScale = 1.1f;

        private const int ImpactDustCount = 34;
        private const int ImpactSmokeDustCount = 10;

        private const float ImpactDustSpeedMin = 1.5f;
        private const float ImpactDustSpeedMax = 7f;

        private const float ImpactDustScaleMin = 0.8f;
        private const float ImpactDustScaleMax = 1.9f;

        private static readonly Color[] ImpactDustColors =
        {
            new Color(85, 35, 135),
            new Color(125, 45, 190),
            new Color(170, 85, 240),
            new Color(205, 145, 255),
            new Color(110, 100, 125),
            new Color(145, 145, 155)
        };
    }
}