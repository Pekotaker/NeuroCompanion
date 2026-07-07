using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Helpers;

namespace NeuroCompanion.Systems
{
    public class NeuroDelayedProjectileSystem : ModSystem
    {
        public override void PostUpdateProjectiles()
        {
            NeuroProjectileSpawner.UpdateDelayedWeaponShots();
        }
    }
}