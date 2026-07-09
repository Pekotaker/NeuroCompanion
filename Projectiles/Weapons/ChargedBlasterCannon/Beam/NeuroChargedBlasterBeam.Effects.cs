using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Beam
{
    public partial class NeuroChargedBlasterBeam
    {
        private void ProduceVisualEffects()
        {
            Color beamColor = GetBeamColor();

            DelegateMethods.v3_1 =
                beamColor.ToVector3() *
                BeamLightBrightness;

            Utils.PlotTileLine(
                Projectile.Center,
                Projectile.Center + Projectile.velocity * BeamLength,
                Projectile.width * Projectile.scale,
                DelegateMethods.CastLight
            );

            if (Main.rand.NextBool(BeamDustChanceDenominator))
            {
                Vector2 dustPosition =
                    Projectile.Center +
                    Projectile.velocity *
                    Main.rand.NextFloat(BeamDustMinDistance, BeamLength);

                Dust dust =
                    Dust.NewDustDirect(
                        dustPosition,
                        0,
                        0,
                        DustID.BlueTorch,
                        0f,
                        0f,
                        100,
                        beamColor,
                        BeamDustScale
                    );

                dust.noGravity = true;
            }
        }

        private static Color GetBeamColor()
        {
            return ChargedBlasterCannonEffectColor.GetColor(BeamColorAlpha);
        }
    }
}