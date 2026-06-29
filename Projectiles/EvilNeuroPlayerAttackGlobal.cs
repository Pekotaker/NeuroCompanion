using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles
{
    public class EvilNeuroPlayerAttackGlobal : GlobalProjectile
    {
        private const int OwnerHitCooldownTicks = 30;

        public override bool InstancePerEntity => true;

        public bool CanDamageOwner { get; set; }

        public bool KillOnOwnerHit { get; set; }

        private int ownerHitCooldown;

        public override void PostAI(Projectile projectile)
        {
            if (ownerHitCooldown > 0)
            {
                ownerHitCooldown--;
            }

            if (!CanDamageOwner)
            {
                return;
            }

            if (ownerHitCooldown > 0)
            {
                return;
            }

            TryDamageOwner(projectile);
        }

        private void TryDamageOwner(Projectile projectile)
        {
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
            {
                return;
            }

            Player owner = Main.player[projectile.owner];

            if (!owner.active || owner.dead)
            {
                return;
            }

            if (!ProjectileHitsOwner(projectile, owner))
            {
                return;
            }

            int hitDirection = owner.Center.X < projectile.Center.X ? -1 : 1;

            PlayerDeathReason deathReason =
                PlayerDeathReason.ByCustomReason(
                    $"{owner.name} was harpooned by Evil Neuro."
                );

            owner.Hurt(
                deathReason,
                projectile.damage,
                hitDirection,
                pvp: true
            );

            ownerHitCooldown = OwnerHitCooldownTicks;

            if (KillOnOwnerHit)
            {
                projectile.Kill();
            }
        }

        private static bool ProjectileHitsOwner(
            Projectile projectile,
            Player owner
        )
        {
            Rectangle projectileHitbox = projectile.Hitbox;
            projectileHitbox.Inflate(8, 8);

            if (projectileHitbox.Intersects(owner.Hitbox))
            {
                return true;
            }

            Vector2 oldCenter =
                projectile.oldPosition
                + new Vector2(projectile.width, projectile.height) * 0.5f;

            Vector2 currentCenter = projectile.Center;

            float collisionPoint = 0f;

            return Collision.CheckAABBvLineCollision(
                owner.position,
                new Vector2(owner.width, owner.height),
                oldCenter,
                currentCenter,
                projectile.width + 8f,
                ref collisionPoint
            );
        }
    }
}