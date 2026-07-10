namespace NeuroCompanion.Projectiles.Weapons.LifeDrain
{
    public partial class NeuroLifeDrainField
    {
        // Spawn one traveling particle per destination at this interval.
        // Increase this to reduce particle density.
        private const int StreamSpawnIntervalTicks = 5;

        // Each particle receives one random travel speed when spawned.
        private const float MinimumStreamSpeedPixelsPerTick = 4f;
        private const float MaximumStreamSpeedPixelsPerTick = 8f;

        private const int StreamParticleMaximumLifetimeTicks = 180;

        // Random starting displacement perpendicular to the stream.
        // These are the requested tunable 2–4 pixel values.
        private const float MinimumPerpendicularSpawnOffset = 2f;
        private const float MaximumPerpendicularSpawnOffset = 4f;

        // Do not create two nearly overlapping streams when
        // Neuro and the player are in almost the same position.
        private const float DuplicateDestinationDistance = 24f;


        private const float StreamDustScaleMinimum = 0.65f;
        private const float StreamDustScaleMaximum = 0.9f;

        // Terraria blocks are 16 pixels wide.
        // 96 pixels gives a six-block drain radius.
        private const int DrainDiameter = 192;
        private const float DrainRadius = DrainDiameter * 0.5f;

        private const int LifetimeTicks = 12;

        // Refreshed every tick while at least one enemy is in the field.
        private const int HealingBuffDurationTicks = 3;
    }
}