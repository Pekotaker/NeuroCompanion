using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Visuals;
using NeuroCompanion.Projectiles.Companion;

namespace NeuroCompanion.Projectiles.Weapons.LifeDrain
{
    public partial class NeuroLifeDrainField
    {
        private void UpdateDrainEffects()
        {
            List<NPC> targets = FindDrainTargets();

            if (targets.Count <= 0)
            {
                streamSpawnTimer =
                    StreamSpawnIntervalTicks - 1;

                return;
            }

            Player owner = GetOwner();

            if (owner != null)
            {
                owner.AddBuff(
                    BuffID.SoulDrain,
                    HealingBuffDurationTicks
                );
            }

            // Gameplay still runs on the server,
            // but dust is a client-side visual.
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            streamSpawnTimer++;

            if (
                streamSpawnTimer <
                StreamSpawnIntervalTicks
            )
            {
                return;
            }

            streamSpawnTimer = 0;

            Projectile companion =
                FindNeuroCompanionProjectile();

            for (int i = 0; i < targets.Count; i++)
            {
                NPC target = targets[i];

                if (
                    target == null ||
                    !target.active
                )
                {
                    continue;
                }

                if (companion != null)
                {
                    SpawnTravelingDrainParticle(
                        target.Center,
                        companion
                    );
                }

                if (
                    owner != null &&
                    (
                        companion == null ||
                        Vector2.DistanceSquared(
                            companion.Center,
                            owner.Center
                        ) >
                        DuplicateDestinationDistance *
                        DuplicateDestinationDistance
                    )
                )
                {
                    SpawnTravelingDrainParticle(
                        target.Center,
                        owner
                    );
                }
            }
        }

        private void SpawnTravelingDrainParticle(
            Vector2 sourcePosition,
            Entity destination
        )
        {
            if (destination == null)
            {
                return;
            }

            Vector2 path =
                destination.Center -
                sourcePosition;

            float distance = path.Length();

            if (
                distance <= 0.01f ||
                path.HasNaNs()
            )
            {
                return;
            }

            Vector2 direction =
                path / distance;

            Vector2 perpendicular =
                new Vector2(
                    -direction.Y,
                    direction.X
                );

            float perpendicularOffset =
                Main.rand.NextFloat(
                    MinimumPerpendicularSpawnOffset,
                    MaximumPerpendicularSpawnOffset
                );

            if (Main.rand.NextBool())
            {
                perpendicularOffset =
                    -perpendicularOffset;
            }

            Vector2 spawnPosition =
                sourcePosition +
                perpendicular * perpendicularOffset;

            float speedPixelsPerTick =
                Main.rand.NextFloat(
                    MinimumStreamSpeedPixelsPerTick,
                    MaximumStreamSpeedPixelsPerTick
                );

            Dust dust =
                Dust.NewDustPerfect(
                    spawnPosition,
                    ModContent.DustType<
                        NeuroLifeDrainStreamDust
                    >(),
                    Vector2.Zero,
                    0,
                    Color.White,
                    1f
                );

            dust.customData =
                new NeuroLifeDrainStreamState(
                    destination,
                    speedPixelsPerTick,
                    StreamParticleMaximumLifetimeTicks
                );
        }

        private Player GetOwner()
        {
            if (
                Projectile.owner < 0 ||
                Projectile.owner >= Main.maxPlayers
            )
            {
                return null;
            }

            Player owner =
                Main.player[Projectile.owner];

            if (
                owner == null ||
                !owner.active ||
                owner.dead
            )
            {
                return null;
            }

            return owner;
        }

        private Projectile FindNeuroCompanionProjectile()
        {
            int companionType =
                ModContent.ProjectileType<
                    NeuroCompanionProjectile
                >();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile =
                    Main.projectile[i];

                if (
                    projectile != null &&
                    projectile.active &&
                    projectile.owner == Projectile.owner &&
                    projectile.type == companionType
                )
                {
                    return projectile;
                }
            }

            return null;
        }
    }
}