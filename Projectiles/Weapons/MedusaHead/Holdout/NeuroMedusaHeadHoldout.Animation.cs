using Terraria;

namespace NeuroCompanion.Projectiles.Weapons.MedusaHead.Holdout
{
    public partial class NeuroMedusaHeadHoldout
    {
        private void UpdateAnimation()
        {
            int frameCount = Main.projFrames[Type];

            if (frameCount <= 1)
            {
                Projectile.frame = 0;
                return;
            }

            Projectile.frameCounter++;

            if (
                Projectile.frameCounter <
                AnimationFrameDurationTicks
            )
            {
                return;
            }

            Projectile.frameCounter = 0;
            Projectile.frame++;

            if (Projectile.frame >= frameCount)
            {
                Projectile.frame = 0;
            }
        }
    }
}