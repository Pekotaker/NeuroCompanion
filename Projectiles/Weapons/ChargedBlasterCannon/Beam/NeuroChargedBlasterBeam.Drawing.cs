using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Beam
{
    public partial class NeuroChargedBlasterBeam
    {

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero || BeamLength <= 0f)
            {
                return false;
            }

            Texture2D texture =
                TextureAssets.Projectile[Type].Value;

            Vector2 startPosition =
                Projectile.Center.Floor()
                + Projectile.velocity * BeamDrawStartOffset
                - Main.screenPosition;

            Vector2 endPosition =
                startPosition + Projectile.velocity * BeamLength;

            Utils.LaserLineFraming lineFraming =
                new Utils.LaserLineFraming(
                    DelegateMethods.RainbowLaserDraw
                );

            DelegateMethods.c_1 =
                GetBeamColor();

            Utils.DrawLaser(
                Main.spriteBatch,
                texture,
                startPosition,
                endPosition,
                new Vector2(BeamDrawScale),
                lineFraming
            );

            return false;
        }
    }
}