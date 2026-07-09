namespace NeuroCompanion.Projectiles.Weapons.LastPrism.Beam
{
    public partial class NeuroLastPrismBeam
    {
        private const float MaxDamageMultiplier = 1.5f;
        private const float DamageBalanceMultiplier = 0.525f;
        private const float MaxBeamScale = 1.6f;
        private const float MaxBeamSpread = 0.95f;
        private const float MaxBeamLength = 2400f;

        private const float BeamTileCollisionWidth = 1f;
        private const float BeamHitboxCollisionWidth = 22f;
        private const int NumSamplePoints = 3;

        private const float BeamLengthChangeFactor = 0.75f;
        private const float VisualEffectThreshold = 0.1f;

        private const float OuterBeamOpacityMultiplier = 0.75f;
        private const float InnerBeamOpacityMultiplier = 0.1f;
        private const float InnerBeamScaleMultiplier = 0.5f;

        private const float BeamLightBrightness = 0.75f;

        private const float BeamColorSaturation = 1f;
        private const float BeamColorLightness = 0.5f;

        private const int OwnerHitCooldownTicks = 30;
    }
}