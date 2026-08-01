using System;

using Microsoft.Xna.Framework;

using Terraria;

using NeuroCompanion.Players;

namespace NeuroCompanion.Projectiles.Companion
{
    public partial class NeuroCompanionProjectile
    {
        private NPC FindTarget(Player owner)
        {
            NPC manualTarget =
                FindManualTarget(owner);

            if (manualTarget != null)
            {
                return manualTarget;
            }

            return FindClosestVisibleTarget(owner);
        }

        private NPC FindManualTarget(Player owner)
        {
            if (!owner.HasMinionAttackTargetNPC)
            {
                return null;
            }

            int targetIndex =
                owner.MinionAttackTargetNPC;

            if (
                targetIndex < 0 ||
                targetIndex >= Main.maxNPCs
            )
            {
                return null;
            }

            NPC selectedTarget =
                Main.npc[targetIndex];

            if (
                IsValidTargetWithinRange(
                    selectedTarget,
                    owner,
                    ManualTargetSearchRangeFromPlayer
                )
            )
            {
                return selectedTarget;
            }

            return null;
        }

        private NPC FindClosestVisibleTarget(
            Player owner
        )
        {
            Rectangle visibleWorldBounds =
                GetVisibleWorldBounds();

            NPC closestTarget = null;

            float closestDistanceSquared =
                float.MaxValue;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (
                    !IsValidVisibleTarget(
                        npc,
                        owner,
                        visibleWorldBounds
                    )
                )
                {
                    continue;
                }

                float distanceSquared =
                    Vector2.DistanceSquared(
                        owner.Center,
                        npc.Center
                    );

                if (
                    distanceSquared >=
                    closestDistanceSquared
                )
                {
                    continue;
                }

                closestDistanceSquared =
                    distanceSquared;

                closestTarget = npc;
            }

            return closestTarget;
        }

        private bool IsValidVisibleTarget(
            NPC npc,
            Player owner,
            Rectangle visibleWorldBounds
        )
        {
            if (!IsChaseableTarget(npc))
            {
                return false;
            }

            if (
                !visibleWorldBounds.Intersects(
                    npc.Hitbox
                )
            )
            {
                return false;
            }

            if (CanDetectTargetsThroughBlocks(owner))
            {
                return true;
            }

            return CanShootTarget(npc, owner);
        }

        private bool IsValidTargetWithinRange(
            NPC npc,
            Player owner,
            float maximumDistance
        )
        {
            if (!IsChaseableTarget(npc))
            {
                return false;
            }

            float distanceSquared =
                Vector2.DistanceSquared(
                    owner.Center,
                    npc.Center
                );

            if (
                distanceSquared >
                maximumDistance * maximumDistance
            )
            {
                return false;
            }

            if (CanDetectTargetsThroughBlocks(owner))
            {
                return true;
            }

            return CanShootTarget(npc, owner);
        }

        private bool IsChaseableTarget(NPC npc)
        {
            return npc != null &&
                   npc.active &&
                   npc.CanBeChasedBy(this);
        }

        private static Rectangle GetVisibleWorldBounds()
        {
            Vector2 zoom =
                Main.GameViewMatrix.Zoom;

            float horizontalZoom =
                Math.Max(
                    zoom.X,
                    MinimumViewportZoom
                );

            float verticalZoom =
                Math.Max(
                    zoom.Y,
                    MinimumViewportZoom
                );

            Vector2 unscaledScreenSize =
                new Vector2(
                    Main.screenWidth,
                    Main.screenHeight
                );

            Vector2 visibleWorldSize =
                new Vector2(
                    unscaledScreenSize.X /
                    horizontalZoom,

                    unscaledScreenSize.Y /
                    verticalZoom
                );

            Vector2 screenCenter =
                Main.screenPosition +
                unscaledScreenSize * 0.5f;

            Vector2 visibleWorldTopLeft =
                screenCenter -
                visibleWorldSize * 0.5f;

            return new Rectangle(
                (int)Math.Floor(
                    visibleWorldTopLeft.X
                ),
                (int)Math.Floor(
                    visibleWorldTopLeft.Y
                ),
                (int)Math.Ceiling(
                    visibleWorldSize.X
                ),
                (int)Math.Ceiling(
                    visibleWorldSize.Y
                )
            );
        }

        private bool CanShootTarget(
            NPC npc,
            Player owner
        )
        {
            bool companionCanSeeTarget =
                Collision.CanHitLine(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    npc.position,
                    npc.width,
                    npc.height
                );

            if (companionCanSeeTarget)
            {
                return true;
            }

            return Collision.CanHitLine(
                owner.position,
                owner.width,
                owner.height,
                npc.position,
                npc.width,
                npc.height
            );
        }

        private static bool CanDetectTargetsThroughBlocks(
            Player owner
        )
        {
            if (
                owner == null ||
                !owner.active
            )
            {
                return false;
            }

            return owner
                .GetModPlayer<NeuroCompanionPlayer>()
                .NeuroStaffCanDetectThroughBlocks;
        }
    }
}