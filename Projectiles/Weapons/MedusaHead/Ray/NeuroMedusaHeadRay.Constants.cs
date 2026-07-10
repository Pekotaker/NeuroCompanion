using Microsoft.Xna.Framework;


namespace NeuroCompanion.Projectiles.Weapons.MedusaHead.Ray
{
    public partial class NeuroMedusaHeadRay
    {
        private const int LifetimeTicks = 12;
        private const int DamageWindowTicks = 3;
        private const int FadeOutDurationTicks = 10;

        private const float RayCollisionWidth = 8f;
        private const int LaserScanSampleCount = 3;
    }
}