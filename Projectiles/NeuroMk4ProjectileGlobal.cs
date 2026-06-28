using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles
{
    public class NeuroMk4ProjectileGlobal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IgnoreTilesForNeuroMk4 { get; set; }

        public override bool PreAI(Projectile projectile)
        {
            if (IgnoreTilesForNeuroMk4)
            {
                projectile.tileCollide = false;
            }

            return true;
        }

        public override void PostAI(Projectile projectile)
        {
            if (IgnoreTilesForNeuroMk4)
            {
                projectile.tileCollide = false;
            }
        }
    }
}