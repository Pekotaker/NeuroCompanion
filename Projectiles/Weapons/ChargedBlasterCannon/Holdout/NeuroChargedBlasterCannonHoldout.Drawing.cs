using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Holdout
{
    public partial class NeuroChargedBlasterCannonHoldout
    {
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects =
                Projectile.spriteDirection == -1
                    ? SpriteEffects.FlipVertically
                    : SpriteEffects.None;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            int frameCount = Main.projFrames[Type];

            if (frameCount <= 0)
            {
                frameCount = 1;
            }

            int frameHeight =
                texture.Height / frameCount;

            Rectangle sourceRectangle =
                new Rectangle(
                    0,
                    frameHeight * Projectile.frame,
                    texture.Width,
                    frameHeight
                );

            Vector2 origin =
                sourceRectangle.Size() * 0.5f;

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRectangle,
                lightColor,
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