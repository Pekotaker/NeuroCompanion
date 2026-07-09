namespace NeuroCompanion.Projectiles.Weapons.LastPrism.Holdout
{
    public partial class NeuroLastPrismHoldout
    {
        public const int NumBeams = 6;
        public const int DurationTicks = 260;
        public const int RefreshGraceTicks = 12;

        public const float MaxCharge = 200f;
        public const float DamageStart = 12f;

        private const int NumAnimationFrames = 5;
        private const float PrismDistanceFromNeuro = 28f;
        private const float AimResponsiveness = 0.18f;
        private const int SoundInterval = 20;
    }
}