using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace NeuroCompanion.Projectiles.Weapons.SpiritFlame
{
    public partial class NeuroSpiritFlameProjectile
    {
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture =
                TextureAssets.Projectile[ProjectileID.SpiritFlame].Value;

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
                0f,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}