using Terraria.Audio;
using Terraria.ID;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Holdout
{
    public partial class NeuroChargedBlasterCannonHoldout
    {
        private const int SmallOrbCountBeforeHeavyPhase = 6;
        private const int HeavyOrbCountBeforeBeamPhase = 3;

        private const int PhaseTwoStartTicks = 90;         // 1.5 seconds
        private const int BeamStartTicks = 180;            // 3.0 seconds

        private const int BeamDurationTicks = 300;         // 5 seconds
        private const int CycleResetTicks = BeamStartTicks + BeamDurationTicks;

        public const int DurationTicks = CycleResetTicks;
        public const int RefreshGraceTicks = 12;

        private const int RapidOrbCooldownTicks = 10;      // 0.17 seconds
        private const int HeavyOrbCooldownTicks = 30;      // 0.5 seconds

        private const float HoldoutDistanceFromNeuro = 34f;
        private const float AimResponsiveness = 0.18f;
        private const float MuzzleDistance = 34f;

        private const float SmallOrbDamageMultiplier = 1f;
        private const float HeavyOrbDamageMultiplier = 2.5f;

        private const int HeavyOrbHumIntervalTicks = 6;

        private const float InitialPewSoundVolume = 1f;
        private const float InitialPewSoundPitch = 0f;

        private const float SmallOrbHumSoundVolume = 0.25f;
        private const float SmallOrbHumSoundPitch = -0.35f;

        private const float HeavyOrbHumSoundVolume = 0.45f;
        private const float HeavyOrbHumSoundPitch = -0.2f;

        private const float BeamStartHumSoundVolume = 1.05f;
        private const float BeamStartHumSoundPitch = -0.05f;

        private const int SmallOrbPhaseCannonDustIntervalTicks = 6;
        private const int HeavyOrbPhaseCannonDustIntervalTicks = 4;
        private const int BeamPhaseCannonDustIntervalTicks = 2;

        private const int SmallOrbPhaseCannonDustCount = 8;
        private const int HeavyOrbPhaseCannonDustCount = 12;
        private const int BeamPhaseCannonDustCount = 3;

        private const float SmallOrbPhaseCannonDustScale = 0.45f;
        private const float HeavyOrbPhaseCannonDustScale = 0.55f;
        private const float BeamPhaseCannonDustScale = 0.8f;

        private const int CannonDustType = DustID.Electric;

        private const float CannonDustOriginOffset = 8f;

        private const float SmallOrbPhaseSuctionRadiusMin = 4f;
        private const float SmallOrbPhaseSuctionRadiusMax = 18f;

        private const float HeavyOrbPhaseSuctionRadiusMin = 4f;
        private const float HeavyOrbPhaseSuctionRadiusMax = 18f;

        private const float SmallOrbPhaseSuctionSpeed = 2f;
        private const float HeavyOrbPhaseSuctionSpeed = 2f;

        private const float SuctionDustRandomVelocity = 0.45f;

        private const float BeamPhaseCannonDustSpeed = 3f;
        private const float BeamPhaseCannonDustConeRadians = 0.64f;
        private const float BeamPhaseDustSpawnBackOffset = -16f;
        private const float BeamPhaseDustSpawnSideSpread = 8f;

        private const float BeamPhaseCannonDustCenterSpeedMultiplier = 5f;
        private const float BeamPhaseCannonDustEdgeSpeedMultiplier = 1f;
        private const float BeamPhaseCannonDustFalloffPower = 0.2f;
        private const float BeamPhaseCannonDustRandomSpeedMultiplier = 0.15f;

        private const float SmallOrbSpeed = 18f;
        private const float HeavyOrbSpeed = 18f;

        private static SoundStyle InitialPewSound =>
            SoundID.Item75 with
            {
                Volume = InitialPewSoundVolume,
                Pitch = InitialPewSoundPitch
            };

        private static SoundStyle SmallOrbHumSound =>
            SoundID.Item15 with
            {
                Volume = SmallOrbHumSoundVolume,
                Pitch = SmallOrbHumSoundPitch
            };

        private static SoundStyle HeavyOrbHumSound =>
            SoundID.Item15 with
            {
                Volume = HeavyOrbHumSoundVolume,
                Pitch = HeavyOrbHumSoundPitch
            };

        private static SoundStyle BeamStartHumSound =>
            SoundID.Item15 with
            {
                Volume = BeamStartHumSoundVolume,
                Pitch = BeamStartHumSoundPitch
            };
    }
}