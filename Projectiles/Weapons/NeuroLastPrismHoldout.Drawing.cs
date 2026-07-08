using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;

namespace NeuroCompanion.Projectiles.Weapons
{
    public partial class NeuroLastPrismHoldout
    {
        private void UpdateAnimation()
        {
            Projectile.frameCounter++;

            int framesPerAnimationUpdate =
                FrameCounter >= MaxCharge ? 2 :
                FrameCounter >= MaxCharge * 0.66f ? 3 :
                4;

            if (Projectile.frameCounter < framesPerAnimationUpdate)
            {
                return;
            }

            Projectile.frameCounter = 0;

            Projectile.frame++;

            if (Projectile.frame >= NumAnimationFrames)
            {
                Projectile.frame = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects =
                Projectile.spriteDirection == -1
                    ? SpriteEffects.FlipHorizontally
                    : SpriteEffects.None;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            int frameHeight =
                texture.Height / Main.projFrames[Type];

            Rectangle sourceRectangle =
                new Rectangle(
                    0,
                    frameHeight * Projectile.frame,
                    texture.Width,
                    frameHeight
                );

            Vector2 origin =
                sourceRectangle.Size() * 0.5f;

            Color drawColor =
                new Color(255, 220, 255);

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRectangle,
                drawColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                effects,
                0
            );

            return false;
        }
    }
}