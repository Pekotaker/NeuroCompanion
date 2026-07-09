using Microsoft.Xna.Framework;
using Terraria;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Orb
{
    public partial class NeuroChargedBlasterOrb
    {
        private void EmitDraggedTrailDust()
        {
            Vector2 direction =
                Projectile.velocity.SafeNormalize(Vector2.UnitX);

            Vector2 perpendicular =
                direction.RotatedBy(MathHelper.PiOver2);

            int dustCount =
                IsHeavyOrb
                    ? HeavyOrbTrailDustCountPerTick
                    : SmallOrbTrailDustCountPerTick;

            float dustScale =
                IsHeavyOrb
                    ? HeavyOrbTrailDustScale
                    : SmallOrbTrailDustScale;

            float trailLength =
                IsHeavyOrb
                    ? HeavyOrbTrailLength
                    : SmallOrbTrailLength;

            float sideSpread =
                IsHeavyOrb
                    ? HeavyOrbTrailSideSpread
                    : SmallOrbTrailSideSpread;

            float pullVelocity =
                IsHeavyOrb
                    ? HeavyOrbTrailPullVelocity
                    : SmallOrbTrailPullVelocity;

            for (int i = 0; i < dustCount; i++)
            {
                float distanceBehindOrb =
                    Main.rand.NextFloat(0f, trailLength);

                Vector2 dustPosition =
                    Projectile.Center -
                    direction * distanceBehindOrb +
                    perpendicular * Main.rand.NextFloat(-sideSpread, sideSpread);

                Vector2 dustVelocity =
                    direction * Main.rand.NextFloat(
                        pullVelocity * 0.65f,
                        pullVelocity
                    ) +
                    Main.rand.NextVector2Circular(
                        TrailDustRandomVelocity,
                        TrailDustRandomVelocity
                    );

                Dust dust =
                    Dust.NewDustPerfect(
                        dustPosition,
                        OrbTrailDustType,
                        dustVelocity,
                        100,
                        GetOrbEffectColor(),
                        dustScale
                    );

                dust.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            int dustCount =
                IsHeavyOrb
                    ? HeavyOrbDeathDustCount
                    : SmallOrbDeathDustCount;

            float dustScale =
                IsHeavyOrb
                    ? HeavyOrbDeathDustScale
                    : SmallOrbDeathDustScale;

            for (int i = 0; i < dustCount; i++)
            {
                Dust dust =
                    Dust.NewDustDirect(
                        Projectile.position,
                        Projectile.width,
                        Projectile.height,
                        OrbDeathDustType,
                        Main.rand.NextFloat(-DeathDustMaxVelocity, DeathDustMaxVelocity),
                        Main.rand.NextFloat(-DeathDustMaxVelocity, DeathDustMaxVelocity),
                        100,
                        GetOrbEffectColor(),
                        dustScale
                    );

                dust.noGravity = true;
            }
        }
    }
}
