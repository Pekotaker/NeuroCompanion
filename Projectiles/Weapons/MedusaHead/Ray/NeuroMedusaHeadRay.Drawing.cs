using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace NeuroCompanion.Projectiles.Weapons.MedusaHead.Ray
{
    public partial class NeuroMedusaHeadRay
    {
        public override bool PreDraw(ref Color lightColor)
        {
            if (
                !initialized ||
                rayLength <= 0f
            )
            {
                return false;
            }

            Texture2D texture =
                TextureAssets
                    .Projectile[ProjectileID.MedusaHeadRay]
                    .Value;

            if (
                texture == null ||
                texture.Width <= 0 ||
                texture.Height <= 0
            )
            {
                return false;
            }

            float fadeIn =
                MathHelper.Clamp(
                    age / 2f,
                    0f,
                    1f
                );

            float fadeOut =
                MathHelper.Clamp(
                    Projectile.timeLeft /
                    (float)FadeOutDurationTicks,
                    0f,
                    1f
                );

            float opacity =
                fadeIn * fadeOut;

            Vector2 drawPosition =
                Projectile.Center -
                Main.screenPosition;

            // The vanilla texture is vertical.
            // Its bottom-center is placed on the Medusa head,
            // and its height is stretched along the ray.
            Vector2 origin =
                new Vector2(
                    texture.Width * 0.5f,
                    texture.Height
                );

            Vector2 scale =
                new Vector2(
                    Projectile.scale,
                    rayLength / texture.Height
                );

            float rotation =
                RayDirection.ToRotation() +
                MathHelper.PiOver2;

            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                Color.White * opacity,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}