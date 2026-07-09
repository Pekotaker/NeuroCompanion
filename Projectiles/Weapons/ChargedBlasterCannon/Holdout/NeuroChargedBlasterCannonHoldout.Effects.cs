using System;

using Microsoft.Xna.Framework;

using Terraria;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Holdout
{
    public partial class NeuroChargedBlasterCannonHoldout
    {
        private static Color GetCannonDustColor()
        {
            return ChargedBlasterCannonEffectColor.GetColor();
        }

        private void EmitCannonPhaseDust()
        {
            if (SingleShotOnly)
            {
                return;
            }

            int intervalTicks;
            int dustCount;
            float dustScale;

            bool beamPhase =
                ChargeTicks >= BeamStartTicks;

            if (beamPhase)
            {
                intervalTicks = BeamPhaseCannonDustIntervalTicks;
                dustCount = BeamPhaseCannonDustCount;
                dustScale = BeamPhaseCannonDustScale;
            }
            else if (ChargeTicks >= PhaseTwoStartTicks)
            {
                intervalTicks = HeavyOrbPhaseCannonDustIntervalTicks;
                dustCount = HeavyOrbPhaseCannonDustCount;
                dustScale = HeavyOrbPhaseCannonDustScale;
            }
            else
            {
                intervalTicks = SmallOrbPhaseCannonDustIntervalTicks;
                dustCount = SmallOrbPhaseCannonDustCount;
                dustScale = SmallOrbPhaseCannonDustScale;
            }

            cannonDustTimer++;

            if (cannonDustTimer < intervalTicks)
            {
                return;
            }

            cannonDustTimer = 0;

            Vector2 direction =
                SafeNormalize(Projectile.velocity, Vector2.UnitX);

            Vector2 perpendicular =
                direction.RotatedBy(MathHelper.PiOver2);

            Vector2 cannonPosition =
                Projectile.Center + direction * CannonDustOriginOffset;

            for (int i = 0; i < dustCount; i++)
            {
                if (beamPhase)
                {
                    EmitBeamPhaseCannonDust(
                        cannonPosition,
                        direction,
                        perpendicular,
                        dustScale
                    );
                }
                else
                {
                    EmitOrbPhaseSuctionDust(
                        cannonPosition,
                        dustScale
                    );
                }
            }
        }

        private void EmitBeamPhaseCannonDust(
            Vector2 cannonPosition,
            Vector2 beamDirection,
            Vector2 perpendicular,
            float dustScale
        )
        {
            Vector2 dustPosition =
                cannonPosition -
                beamDirection * BeamPhaseDustSpawnBackOffset +
                perpendicular *
                Main.rand.NextFloat(
                    -BeamPhaseDustSpawnSideSpread,
                    BeamPhaseDustSpawnSideSpread
                );

            float angle =
                Main.rand.NextFloat(
                    -BeamPhaseCannonDustConeRadians,
                    BeamPhaseCannonDustConeRadians
                );

            float normalizedDivergence =
                MathHelper.Clamp(
                    Math.Abs(angle) / BeamPhaseCannonDustConeRadians,
                    0f,
                    1f
                );

            float angleSpeedMultiplier =
                MathHelper.Lerp(
                    BeamPhaseCannonDustCenterSpeedMultiplier,
                    BeamPhaseCannonDustEdgeSpeedMultiplier,
                    MathF.Pow(
                        normalizedDivergence,
                        BeamPhaseCannonDustFalloffPower
                    )
                );

            float randomSpeedMultiplier =
                Main.rand.NextFloat(
                    1f - BeamPhaseCannonDustRandomSpeedMultiplier,
                    1f
                );

            Vector2 dustVelocity =
                beamDirection.RotatedBy(angle) *
                BeamPhaseCannonDustSpeed *
                angleSpeedMultiplier *
                randomSpeedMultiplier;

            Dust dust =
                Dust.NewDustPerfect(
                    dustPosition,
                    CannonDustType,
                    dustVelocity,
                    100,
                    GetCannonDustColor(),
                    dustScale
                );

            dust.noGravity = true;
        }

        private void EmitOrbPhaseSuctionDust(
            Vector2 cannonPosition,
            float dustScale
        )
        {
            bool heavyOrbPhase =
                ChargeTicks >= PhaseTwoStartTicks;

            float radiusMin =
                heavyOrbPhase
                    ? HeavyOrbPhaseSuctionRadiusMin
                    : SmallOrbPhaseSuctionRadiusMin;

            float radiusMax =
                heavyOrbPhase
                    ? HeavyOrbPhaseSuctionRadiusMax
                    : SmallOrbPhaseSuctionRadiusMax;

            float suctionSpeed =
                heavyOrbPhase
                    ? HeavyOrbPhaseSuctionSpeed
                    : SmallOrbPhaseSuctionSpeed;

            Vector2 fromDirection =
                Main.rand.NextVector2CircularEdge(1f, 1f);

            Vector2 dustPosition =
                cannonPosition +
                fromDirection *
                Main.rand.NextFloat(radiusMin, radiusMax);

            Vector2 toCannon =
                (cannonPosition - dustPosition)
                .SafeNormalize(Vector2.UnitX);

            Vector2 dustVelocity =
                toCannon *
                Main.rand.NextFloat(
                    suctionSpeed * 0.65f,
                    suctionSpeed
                ) +
                Main.rand.NextVector2Circular(
                    SuctionDustRandomVelocity,
                    SuctionDustRandomVelocity
                );

            Dust dust =
                Dust.NewDustPerfect(
                    dustPosition,
                    CannonDustType,
                    dustVelocity,
                    100,
                    GetCannonDustColor(),
                    dustScale
                );

            dust.noGravity = true;
        }
    }
}