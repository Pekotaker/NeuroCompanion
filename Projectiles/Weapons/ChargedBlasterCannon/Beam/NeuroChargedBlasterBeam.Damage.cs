using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;

using NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Holdout;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Beam
{
    public partial class NeuroChargedBlasterBeam
    {
        private void TryDamageOwnerWithBeam(
                    NeuroChargedBlasterCannonHoldout hostCannon
                )
        {
            if (!hostCannon.OwnerDamageEnabled)
            {
                return;
            }

            if (ownerHitCooldown > 0)
            {
                return;
            }

            if (Projectile.owner < 0 || Projectile.owner >= Main.maxPlayers)
            {
                return;
            }

            Player owner = Main.player[Projectile.owner];

            if (!owner.active || owner.dead)
            {
                return;
            }

            float collisionPoint = 0f;

            bool hit =
                Collision.CheckAABBvLineCollision(
                    owner.position,
                    new Vector2(owner.width, owner.height),
                    Projectile.Center,
                    Projectile.Center + Projectile.velocity * BeamLength,
                    BeamHitboxCollisionWidth * Projectile.scale,
                    ref collisionPoint
                );

            if (!hit)
            {
                return;
            }

            int hitDirection =
                owner.Center.X < Projectile.Center.X ? -1 : 1;

            PlayerDeathReason deathReason =
                PlayerDeathReason.ByCustomReason(
                    $"{owner.name} was blasted by Evil Neuro."
                );

            owner.Hurt(
                deathReason,
                Projectile.damage,
                hitDirection,
                pvp: true
            );

            ownerHitCooldown = OwnerHitCooldownTicks;
        }
    }
}


