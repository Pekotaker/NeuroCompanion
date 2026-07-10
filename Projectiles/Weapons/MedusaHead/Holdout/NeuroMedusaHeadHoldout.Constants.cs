namespace NeuroCompanion.Projectiles.Weapons.MedusaHead.Holdout
{
    public partial class NeuroMedusaHeadHoldout
    {
        private const int RefreshGraceTicks = 12;
        private const int SingleShotLifetimeTicks = 30;

        private const int AnimationFrameDurationTicks = 5;

        private const float FallbackAimSpeed = 30f;
        private const float AimResponsiveness = 0.2f;

        // Position relative to Neuro's center.
        //
        // Positive X moves the head farther toward the direction Neuro is facing.
        // Negative X moves it behind Neuro.
        //
        // Negative Y moves it upward.
        // Positive Y moves it downward.

        // Normal Neuro sprite position.
        private const float HoldoutFaceOffsetX = 4f;
        private const float HoldoutFaceOffsetY = -5f;

        // Larger Mk4 Neuro sprite position.
        private const float Mk4HoldoutFaceOffsetX = 6f;
        private const float Mk4HoldoutFaceOffsetY = -10f;
    }
}