using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Terraria;

namespace NeuroCompanion.Projectiles.Weapons.MedusaHead.Holdout
{
    public partial class NeuroMedusaHeadHoldout
    {
        private NPC[] FindTargets()
        {
            List<NPC> candidates = new List<NPC>();

            float maximumDistanceSquared =
                TargetSearchRange * TargetSearchRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!IsValidTarget(npc, maximumDistanceSquared))
                {
                    continue;
                }

                candidates.Add(npc);
            }

            candidates.Sort(
                (first, second) =>
                {
                    float firstDistanceSquared =
                        Vector2.DistanceSquared(
                            Projectile.Center,
                            first.Center
                        );

                    float secondDistanceSquared =
                        Vector2.DistanceSquared(
                            Projectile.Center,
                            second.Center
                        );

                    return firstDistanceSquared.CompareTo(
                        secondDistanceSquared
                    );
                }
            );

            int targetCount =
                Math.Min(
                    candidates.Count,
                    MaxTargetCount
                );

            NPC[] targets = new NPC[targetCount];

            for (int i = 0; i < targetCount; i++)
            {
                targets[i] = candidates[i];
            }

            return targets;
        }

        private bool IsValidTarget(
            NPC npc,
            float maximumDistanceSquared
        )
        {
            if (
                npc == null ||
                !npc.active ||
                !npc.CanBeChasedBy(Projectile)
            )
            {
                return false;
            }

            float distanceSquared =
                Vector2.DistanceSquared(
                    Projectile.Center,
                    npc.Center
                );

            if (distanceSquared > maximumDistanceSquared)
            {
                return false;
            }

            if (IgnoreBlocks)
            {
                return true;
            }

            return Collision.CanHitLine(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                npc.position,
                npc.width,
                npc.height
            );
        }

        private Player GetOwnerTarget()
        {
            if (
                Projectile.owner < 0 ||
                Projectile.owner >= Main.maxPlayers
            )
            {
                return null;
            }

            Player owner = Main.player[Projectile.owner];

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
    }
}