using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Terraria;

namespace NeuroCompanion.Projectiles.Weapons.LifeDrain
{
    public partial class NeuroLifeDrainField
    {
        public override bool? CanHitNPC(NPC target)
        {
            if (!IsValidDrainTarget(target))
            {
                return false;
            }

            return IsInsideDrainArea(target.Hitbox);
        }

        public override bool? Colliding(
            Rectangle projectileHitbox,
            Rectangle targetHitbox
        )
        {
            return IsInsideDrainArea(targetHitbox);
        }

        private List<NPC> FindDrainTargets()
        {
            List<NPC> targets = new List<NPC>();

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!IsValidDrainTarget(npc))
                {
                    continue;
                }

                if (!IsInsideDrainArea(npc.Hitbox))
                {
                    continue;
                }

                targets.Add(npc);
            }

            return targets;
        }

        private bool IsValidDrainTarget(NPC npc)
        {
            return npc != null &&
                   npc.active &&
                   npc.CanBeChasedBy(Projectile);
        }

        private bool IsInsideDrainArea(Rectangle hitbox)
        {
            float closestX =
                MathHelper.Clamp(
                    DrainCenter.X,
                    hitbox.Left,
                    hitbox.Right
                );

            float closestY =
                MathHelper.Clamp(
                    DrainCenter.Y,
                    hitbox.Top,
                    hitbox.Bottom
                );

            Vector2 closestPoint =
                new Vector2(
                    closestX,
                    closestY
                );

            float distanceSquared =
                Vector2.DistanceSquared(
                    DrainCenter,
                    closestPoint
                );

            return distanceSquared <=
                   DrainRadius * DrainRadius;
        }
    }
}