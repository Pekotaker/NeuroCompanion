using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ID;

using NeuroCompanion.Projectiles.Weapons.LastPrism.Holdout;

namespace NeuroCompanion.Projectiles.Weapons.LastPrism.Beam
{
    public partial class NeuroLastPrismBeam
    {
        private void ProduceVisualEffects(float chargeRatio)
        {
            Color outerBeamColor =
                GetOuterBeamColor();

            if (chargeRatio >= VisualEffectThreshold)
            {
                ProduceBeamDust(outerBeamColor);
            }

            DelegateMethods.v3_1 =
                outerBeamColor.ToVector3() *
                BeamLightBrightness *
                chargeRatio;

            Utils.PlotTileLine(
                Projectile.Center,
                Projectile.Center + Projectile.velocity * BeamLength,
                Projectile.width * Projectile.scale,
                DelegateMethods.CastLight
            );
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero || BeamLength <= 0f)
            {
                return false;
            }

            Texture2D texture =
                TextureAssets.Projectile[Type].Value;

            Vector2 drawScale =
                new Vector2(Projectile.scale);

            float visualBeamLength =
                BeamLength - 14.5f * Projectile.scale * Projectile.scale;

            if (visualBeamLength <= 0f)
            {
                return false;
            }

            Vector2 startPosition =
                Projectile.Center.Floor()
                + Projectile.velocity * Projectile.scale * 10.5f
                - Main.screenPosition;

            Vector2 endPosition =
                startPosition + Projectile.velocity * visualBeamLength;

            DrawBeam(
                Main.spriteBatch,
                texture,
                startPosition,
                endPosition,
                drawScale,
                GetOuterBeamColor() *
                OuterBeamOpacityMultiplier *
                Projectile.Opacity
            );

            DrawBeam(
                Main.spriteBatch,
                texture,
                startPosition,
                endPosition,
                drawScale * InnerBeamScaleMultiplier,
                GetInnerBeamColor() *
                InnerBeamOpacityMultiplier *
                Projectile.Opacity
            );

            return false;
        }

        private void DrawBeam(
            SpriteBatch spriteBatch,
            Texture2D texture,
            Vector2 startPosition,
            Vector2 endPosition,
            Vector2 drawScale,
            Color beamColor
        )
        {
            Utils.LaserLineFraming lineFraming =
                new Utils.LaserLineFraming(
                    DelegateMethods.RainbowLaserDraw
                );

            DelegateMethods.c_1 = beamColor;

            Utils.DrawLaser(
                spriteBatch,
                texture,
                startPosition,
                endPosition,
                drawScale,
                lineFraming
            );
        }

        private Color GetInnerBeamColor()
        {
            return Color.White;
        }

        private Color GetOuterBeamColor()
        {
            float hue =
                BeamID / (float)NeuroLastPrismHoldout.NumBeams;

            Color color =
                Main.hslToRgb(
                    hue,
                    BeamColorSaturation,
                    BeamColorLightness
                );

            color.A = 64;

            return color;
        }

        private void ProduceBeamDust(Color beamColor)
        {
            Vector2 endPosition =
                Projectile.Center +
                Projectile.velocity *
                (BeamLength - 14.5f * Projectile.scale);

            float angle =
                Projectile.rotation +
                (Main.rand.NextBool() ? 1f : -1f) *
                MathHelper.PiOver2;

            Vector2 dustVelocity =
                angle.ToRotationVector2() *
                Main.rand.NextFloat(1f, 1.8f);

            Dust dust =
                Dust.NewDustDirect(
                    endPosition,
                    0,
                    0,
                    DustID.FireworkFountain_Pink,
                    dustVelocity.X,
                    dustVelocity.Y,
                    0,
                    beamColor,
                    Main.rand.NextFloat(0.7f, 1.1f)
                );

            dust.noGravity = true;
            dust.color = beamColor;
        }
    }
}