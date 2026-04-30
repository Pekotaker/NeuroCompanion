using Microsoft.Xna.Framework;
using Terraria;

namespace NeuroCompanion.Projectiles
{
    public partial class NeuroCompanionProjectile
    {
        private NPC FindTarget(Player owner)
        {
            NPC manualTarget = FindManualTarget(owner);

            if (manualTarget != null)
            {
                return manualTarget;
            }

            return FindClosestTargetToPlayer(owner);
        }

        private NPC FindManualTarget(Player owner)
        {
            if (!owner.HasMinionAttackTargetNPC)
            {
                return null;
            }

            NPC selectedTarget = Main.npc[owner.MinionAttackTargetNPC];

            if (IsValidTarget(selectedTarget, owner, ManualTargetSearchRangeFromPlayer))
            {
                return selectedTarget;
            }

            return null;
        }

        private NPC FindClosestTargetToPlayer(Player owner)
        {
            NPC bestTarget = null;
            float bestDistanceFromPlayer = TargetSearchRangeFromPlayer;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!IsValidTarget(npc, owner, TargetSearchRangeFromPlayer))
                {
                    continue;
                }

                float distanceFromPlayer = Vector2.Distance(owner.Center, npc.Center);

                if (distanceFromPlayer < bestDistanceFromPlayer)
                {
                    bestDistanceFromPlayer = distanceFromPlayer;
                    bestTarget = npc;
                }
            }

            return bestTarget;
        }

        private bool IsValidTarget(
            NPC npc,
            Player owner,
            float maxDistanceFromOwner
        )
        {
            if (!npc.active)
            {
                return false;
            }

            if (!npc.CanBeChasedBy(this))
            {
                return false;
            }

            float distanceFromOwner = Vector2.Distance(owner.Center, npc.Center);

            if (distanceFromOwner > maxDistanceFromOwner)
            {
                return false;
            }

            return CanShootTarget(npc, owner);
        }

        private bool CanShootTarget(NPC npc, Player owner)
        {
            bool companionCanSeeTarget = Collision.CanHitLine(
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

            bool ownerCanSeeTarget = Collision.CanHitLine(
                owner.position,
                owner.width,
                owner.height,
                npc.position,
                npc.width,
                npc.height
            );

            return ownerCanSeeTarget;
        }
    }
}