using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Projectiles.Companion
{
    public partial class NeuroCompanionProjectile
    {
        private static readonly int[] Mk2TeleportDustTypes =
        {
            DustID.RedTorch,
            DustID.WhiteTorch,
            DustID.YellowTorch,
            DustID.OrangeTorch
        };

        private static readonly int[] Mk3TeleportDustTypes =
        {
            DustID.WhiteTorch,
            DustID.Enchanted_Gold,
            DustID.GemRuby
        };

        private void CreateTeleportEffects(
            Vector2 departurePosition,
            Vector2 arrivalPosition
        )
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            Vector2 teleportPath =
                arrivalPosition - departurePosition;

            float teleportDistance =
                teleportPath.Length();

            if (
                teleportDistance <
                MinimumTeleportEffectDistance ||
                teleportPath.HasNaNs()
            )
            {
                return;
            }

            Vector2 pathDirection =
                teleportPath / teleportDistance;

            Vector2 perpendicularDirection =
                new Vector2(
                    -pathDirection.Y,
                    pathDirection.X
                );

            CreateTeleportTrail(
                departurePosition,
                teleportPath,
                pathDirection,
                perpendicularDirection,
                teleportDistance
            );

            CreateTeleportEndpointBurst(
                departurePosition
            );

            CreateTeleportEndpointBurst(
                arrivalPosition
            );
        }

        private void CreateTeleportTrail(
            Vector2 departurePosition,
            Vector2 teleportPath,
            Vector2 pathDirection,
            Vector2 perpendicularDirection,
            float teleportDistance
        )
        {
            int particleCount =
                GetTeleportTrailParticleCount(
                    teleportDistance
                );

            for (
                int particleIndex = 1;
                particleIndex < particleCount;
                particleIndex++
            )
            {
                float progress =
                    particleIndex /
                    (float)particleCount;

                Vector2 spawnPosition =
                    departurePosition +
                    teleportPath * progress;

                spawnPosition +=
                    perpendicularDirection *
                    Main.rand.NextFloat(
                        -TeleportTrailHalfWidth,
                        TeleportTrailHalfWidth
                    );

                spawnPosition +=
                    pathDirection *
                    Main.rand.NextFloat(
                        -TeleportTrailLongitudinalJitter,
                        TeleportTrailLongitudinalJitter
                    );

                Vector2 velocity =
                    Main.rand.NextVector2Circular(
                        TeleportTrailVelocity,
                        TeleportTrailVelocity
                    );

                SpawnTeleportDust(
                    spawnPosition,
                    velocity
                );
            }
        }

        private void CreateTeleportEndpointBurst(
            Vector2 position
        )
        {
            for (
                int particleIndex = 0;
                particleIndex <
                TeleportEndpointParticleCount;
                particleIndex++
            )
            {
                Vector2 velocity =
                    Main.rand.NextVector2CircularEdge(
                        TeleportEndpointVelocity,
                        TeleportEndpointVelocity
                    );

                velocity *=
                    Main.rand.NextFloat(
                        0.6f,
                        1f
                    );

                SpawnTeleportDust(
                    position,
                    velocity
                );
            }
        }

        private void SpawnTeleportDust(
            Vector2 position,
            Vector2 velocity
        )
        {
            Dust dust =
                Dust.NewDustPerfect(
                    position,
                    RollTeleportDustType(),
                    velocity,
                    20,
                    Color.White,
                    Main.rand.NextFloat(
                        MinimumTeleportDustScale,
                        MaximumTeleportDustScale
                    )
                );

            dust.noGravity = true;
            dust.fadeIn = 1f;
        }

        private int RollTeleportDustType()
        {
            if (IsEvilVisualActive())
            {
                return DustID.LifeDrain;
            }

            switch (GetOwnerStaffVisualTier())
            {
                case 2:
                    return RollDustType(
                        Mk2TeleportDustTypes
                    );

                case 3:
                    return RollDustType(
                        Mk3TeleportDustTypes
                    );

                case 4:
                    return DustID.Electric;

                default:
                    return DustID.OrangeTorch;
            }
        }

        private static int RollDustType(
            int[] dustTypes
        )
        {
            if (
                dustTypes == null ||
                dustTypes.Length == 0
            )
            {
                return DustID.OrangeTorch;
            }

            return dustTypes[
                Main.rand.Next(
                    dustTypes.Length
                )
            ];
        }

        private static int GetTeleportTrailParticleCount(
            float teleportDistance
        )
        {
            int particleCount =
                (int)Math.Ceiling(
                    teleportDistance /
                    TeleportParticleSpacing
                );

            return Math.Clamp(
                particleCount,
                MinimumTeleportTrailParticles,
                MaximumTeleportTrailParticles
            );
        }
    }
}