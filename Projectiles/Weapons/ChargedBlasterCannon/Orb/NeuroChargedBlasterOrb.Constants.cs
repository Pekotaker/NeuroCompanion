using Terraria.ID;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Orb
{
    public partial class NeuroChargedBlasterOrb
    {
        private const int SmallOrbWidth = 14;
        private const int SmallOrbHeight = 14;
        private const float SmallOrbScale = 0.65f;

        private const int HeavyOrbWidth = 40;
        private const int HeavyOrbHeight = 40;
        private const float HeavyOrbScale = 1f;

        private const int SmallOrbPenetration = 1;
        private const int HeavyOrbPenetration = -1;

        private const int SmallOrbLifetimeTicks = 180;
        private const int HeavyOrbLifetimeTicks = 240;

        private const int HeavyOrbLocalNpcHitCooldownTicks = 12;

        private const int SmallOrbTrailDustCountPerTick = 1;
        private const int HeavyOrbTrailDustCountPerTick = 1;

        private const float SmallOrbTrailDustScale = 0.35f;
        private const float HeavyOrbTrailDustScale = 0.4f;

        private const float SmallOrbTrailLength = 72f;
        private const float HeavyOrbTrailLength = 96f;

        private const float SmallOrbTrailSideSpread = 6f;
        private const float HeavyOrbTrailSideSpread = 9f;

        private const float SmallOrbTrailPullVelocity = 5.8f;
        private const float HeavyOrbTrailPullVelocity = 7.2f;

        private const float TrailDustRandomVelocity = 0.75f;

        private const int OrbTrailDustType = DustID.Electric;
        private const int OrbDeathDustType = DustID.Electric;

        private const int SmallOrbDeathDustCount = 8;
        private const int HeavyOrbDeathDustCount = 18;

        private const float SmallOrbDeathDustScale = 0.95f;
        private const float HeavyOrbDeathDustScale = 1.55f;

        private const float DeathDustMaxVelocity = 2f;

        private const float OrbLightRed = 0.2f;
        private const float OrbLightGreen = 0.45f;
        private const float OrbLightBlue = 1f;
    }
}
