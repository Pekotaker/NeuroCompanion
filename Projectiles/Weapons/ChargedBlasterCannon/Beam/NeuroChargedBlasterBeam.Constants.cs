namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Beam
{
    public partial class NeuroChargedBlasterBeam
    {
        private const float MaxBeamLength = 150f * 16f; // 150 tiles
        private const float BeamTileCollisionWidth = 1f;
        private const float BeamHitboxCollisionWidth = 14f;
        private const float BeamLengthChangeFactor = 0.75f;
        private const float BeamStartDistanceFromCannon = 18f;
        private const float BeamDrawStartOffset = 0f;

        private const float BeamDrawScale = 1f;
        private const float BeamLightBrightness = 5.5f;

        private const byte BeamColorAlpha = 25;

        private const int BeamDustChanceDenominator = 3;
        private const float BeamDustScale = 0.4f;

        private const float BeamDustMinDistance = 24f;
        private const int OwnerHitCooldownTicks = 30;

        private const int BeamLocalNpcHitCooldownTicks = 10; // 6 hits per second
        private const float BeamDamageMultiplier = 1.5f;
    }
}