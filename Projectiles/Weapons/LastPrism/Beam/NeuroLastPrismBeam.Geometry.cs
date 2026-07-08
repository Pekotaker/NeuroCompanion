using Microsoft.Xna.Framework;

using Terraria;

using NeuroCompanion.Projectiles.Globals;

using NeuroCompanion.Projectiles.Weapons.LastPrism.Holdout;

namespace NeuroCompanion.Projectiles.Weapons.LastPrism.Beam
{
    public partial class NeuroLastPrismBeam
    {
        private readonly struct BeamFocusState
        {
            public float BeamSpread { get; }
            public float SpinRate { get; }
            public float BeamStartSidewaysOffset { get; }
            public float BeamStartForwardsOffset { get; }

            public BeamFocusState(
                float beamSpread,
                float spinRate,
                float beamStartSidewaysOffset,
                float beamStartForwardsOffset
            )
            {
                BeamSpread = beamSpread;
                SpinRate = spinRate;
                BeamStartSidewaysOffset = beamStartSidewaysOffset;
                BeamStartForwardsOffset = beamStartForwardsOffset;
            }
        }

        private float GetChargeRatio(Projectile hostPrism)
        {
            return MathHelper.Clamp(
                hostPrism.localAI[0] / NeuroLastPrismHoldout.MaxCharge,
                0f,
                1f
            );
        }

        private BeamFocusState UpdateFocusState(float chargeRatio)
        {
            if (chargeRatio >= 1f)
            {
                Projectile.scale = MaxBeamScale;
                Projectile.Opacity = 1f;

                return new BeamFocusState(
                    beamSpread: 0f,
                    spinRate: 1.55f,
                    beamStartSidewaysOffset: 3.25f,
                    beamStartForwardsOffset: -17f
                );
            }

            Projectile.scale =
                MathHelper.Lerp(0f, MaxBeamScale, chargeRatio);

            float beamSpread =
                MathHelper.Lerp(MaxBeamSpread, 0f, chargeRatio);

            float beamStartSidewaysOffset =
                MathHelper.Lerp(16f, 3.25f, chargeRatio);

            float beamStartForwardsOffset =
                MathHelper.Lerp(-21f, -17f, chargeRatio);

            float spinRate;

            if (chargeRatio <= 0.66f)
            {
                float phaseRatio = chargeRatio * 1.5f;

                Projectile.Opacity =
                    MathHelper.Lerp(0f, 0.4f, phaseRatio);

                spinRate =
                    MathHelper.Lerp(10f, 8f, phaseRatio);
            }
            else
            {
                float phaseRatio =
                    (chargeRatio - 0.66f) * 3f;

                Projectile.Opacity =
                    MathHelper.Lerp(0.4f, 1f, phaseRatio);

                spinRate =
                    MathHelper.Lerp(8f, 1.55f, phaseRatio);
            }

            return new BeamFocusState(
                beamSpread,
                spinRate,
                beamStartSidewaysOffset,
                beamStartForwardsOffset
            );
        }

        private void PositionBeamAroundHost(
            Projectile hostPrism,
            Vector2 hostDirection,
            BeamFocusState focusState
        )
        {
            float beamIdOffset =
                BeamID - NeuroLastPrismHoldout.NumBeams / 2f + 0.5f;

            float deviationAngle =
                (
                    hostPrism.localAI[0] +
                    beamIdOffset * focusState.SpinRate
                ) /
                (
                    focusState.SpinRate *
                    NeuroLastPrismHoldout.NumBeams
                ) *
                MathHelper.TwoPi;

            Vector2 unitRot =
                Vector2.UnitY.RotatedBy(deviationAngle);

            Vector2 yVector =
                new Vector2(2.5f, focusState.BeamStartSidewaysOffset);

            float hostAngle =
                hostPrism.velocity.ToRotation();

            Vector2 beamSpanVector =
                (unitRot * yVector).RotatedBy(hostAngle);

            float sinusoidYOffset =
                unitRot.Y *
                MathHelper.Pi /
                NeuroLastPrismHoldout.NumBeams *
                focusState.BeamSpread;

            Projectile.Center = hostPrism.Center;

            Projectile.position += hostDirection * 16f;
            Projectile.position += hostDirection * focusState.BeamStartForwardsOffset;
            Projectile.position += beamSpanVector;

            Projectile.velocity =
                hostDirection.RotatedBy(sinusoidYOffset);

            Projectile.velocity =
                SafeNormalize(Projectile.velocity, hostDirection);

            Projectile.rotation =
                Projectile.velocity.ToRotation();
        }

        private bool CopyMk4BehaviorFromHost(Projectile hostPrism)
        {
            bool ignoreTiles =
                hostPrism
                    .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                    .IgnoreTilesForNeuroMk4;

            Projectile
                .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                .IgnoreTilesForNeuroMk4 = ignoreTiles;

            return ignoreTiles;
        }

        private void UpdateBeamLength(bool ignoreTiles)
        {
            float hitscanBeamLength =
                PerformBeamHitscan(ignoreTiles);

            BeamLength =
                MathHelper.Lerp(
                    BeamLength,
                    hitscanBeamLength,
                    BeamLengthChangeFactor
                );
        }

        private float PerformBeamHitscan(bool ignoreTiles)
        {
            if (ignoreTiles)
            {
                return MaxBeamLength;
            }

            float[] samples = new float[NumSamplePoints];

            Collision.LaserScan(
                Projectile.Center,
                Projectile.velocity,
                BeamTileCollisionWidth * Projectile.scale,
                MaxBeamLength,
                samples
            );

            float averageLength = 0f;

            for (int i = 0; i < samples.Length; i++)
            {
                averageLength += samples[i];
            }

            averageLength /= samples.Length;

            return averageLength;
        }

        public override bool? Colliding(
            Rectangle projHitbox,
            Rectangle targetHitbox
        )
        {
            float collisionPoint = 0f;

            Vector2 beamEnd =
                Projectile.Center + Projectile.velocity * BeamLength;

            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center,
                beamEnd,
                BeamHitboxCollisionWidth * Projectile.scale,
                ref collisionPoint
            );
        }
    }
}