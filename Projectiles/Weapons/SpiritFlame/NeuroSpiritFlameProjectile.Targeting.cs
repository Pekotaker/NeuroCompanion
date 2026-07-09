using Microsoft.Xna.Framework;

using Terraria;

namespace NeuroCompanion.Projectiles.Weapons.SpiritFlame
{
    public partial class NeuroSpiritFlameProjectile
    {
        private NPC FindTarget()
        {
            NPC bestTarget = null;
            float bestDistanceSquared =
                TargetSearchRange * TargetSearchRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (
                    npc == null ||
                    !npc.CanBeChasedBy(Projectile)
                )
                {
                    continue;
                }

                float distanceSquared =
                    Vector2.DistanceSquared(
                        Projectile.Center,
                        npc.Center
                    );

                if (distanceSquared >= bestDistanceSquared)
                {
                    continue;
                }

                if (
                    !Collision.CanHitLine(
                        Projectile.position,
                        Projectile.width,
                        Projectile.height,
                        npc.position,
                        npc.width,
                        npc.height
                    )
                )
                {
                    continue;
                }

                bestTarget = npc;
                bestDistanceSquared = distanceSquared;
            }

            return bestTarget;
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