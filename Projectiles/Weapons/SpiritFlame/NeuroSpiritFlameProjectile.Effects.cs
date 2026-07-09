using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Projectiles.Weapons.SpiritFlame
{
    public partial class NeuroSpiritFlameProjectile
    {
        private void EmitAmbientEffects()
        {
            Lighting.AddLight(
                Projectile.Center,
                0.45f,
                0.1f,
                0.65f
            );

            if (!Main.rand.NextBool(AmbientDustChanceDenominator))
            {
                return;
            }

            Dust dust =
                Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Shadowflame,
                    0f,
                    0f,
                    100,
                    default,
                    AmbientDustScale
                );

            dust.noGravity = true;
            dust.velocity *= 0.25f;
        }

        private void UpdateAnimation()
        {
            Projectile.frameCounter++;

            if (Projectile.frameCounter < AnimationFrameDurationTicks)
            {
                return;
            }

            Projectile.frameCounter = 0;

            int frameCount = Main.projFrames[Type];

            if (frameCount <= 0)
            {
                frameCount = 1;
            }

            Projectile.frame++;

            if (Projectile.frame >= frameCount)
            {
                Projectile.frame = 0;
            }
        }
    }
}