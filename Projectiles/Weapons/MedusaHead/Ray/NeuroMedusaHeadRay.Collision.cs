using Microsoft.Xna.Framework;

using Terraria;

namespace NeuroCompanion.Projectiles.Weapons.MedusaHead.Ray
{
    public partial class NeuroMedusaHeadRay
    {
        public override bool? Colliding(
            Rectangle projectileHitbox,
            Rectangle targetHitbox
        )
        {
            float collisionPoint = 0f;

            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center,
                RayEnd,
                RayCollisionWidth,
                ref collisionPoint
            );
        }
    }
}