using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Projectiles
{
    public partial class NeuroCompanionProjectile
    {
        private void Animate()
        {
            Projectile.frameCounter++;

            if (Projectile.frameCounter < AnimationFrameDurationTicks)
            {
                return;
            }

            Projectile.frameCounter = 0;
            Projectile.frame++;

            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
        }

        private void CreateVisualEffects()
        {
            int dustChance = State == CompanionState.Attacking
                ? AttackDustChance
                : IdleDustChance;

            if (!Main.rand.NextBool(dustChance))
            {
                return;
            }

            Dust.NewDust(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                DustID.GemSapphire
            );
        }
    }
}