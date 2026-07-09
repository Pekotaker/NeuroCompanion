using Microsoft.Xna.Framework;

using Terraria;

namespace NeuroCompanion.Projectiles.Weapons.SpiritFlame
{
    public partial class NeuroSpiritFlameProjectile
    {
        private void HoverAroundAnchor()
        {
            float horizontalOffset =
                (float)System.Math.Sin(
                    age * HoverHorizontalSpeed + Projectile.whoAmI
                ) * HoverHorizontalDistance;

            float verticalOffset =
                (float)System.Math.Cos(
                    age * HoverVerticalSpeed + Projectile.whoAmI
                ) * HoverVerticalDistance;

            Projectile.Center =
                AnchorPosition +
                new Vector2(
                    horizontalOffset,
                    verticalOffset
                );

            Projectile.velocity = Vector2.Zero;
        }

        private void HomeTowardTarget(NPC target)
        {
            Vector2 toTarget =
                target.Center - Projectile.Center;

            if (toTarget.LengthSquared() <= 0.01f)
            {
                return;
            }

            Vector2 desiredVelocity =
                toTarget.SafeNormalize(Vector2.UnitY) * EnemyHomingSpeed;

            Projectile.velocity =
                Vector2.Lerp(
                    Projectile.velocity,
                    desiredVelocity,
                    EnemyHomingTurnStrength
                );

            if (Projectile.velocity.LengthSquared() <= 0.01f)
            {
                Projectile.velocity = desiredVelocity;
            }

            Projectile.Center += Projectile.velocity;
        }

        private void HomeTowardOwner()
        {
            Player owner = GetOwner();

            if (owner == null)
            {
                Projectile.Kill();
                return;
            }

            if (ProjectileHitsOwner(owner))
            {
                DamageOwnerAndExplode(owner);
                return;
            }

            Vector2 toOwner =
                owner.Center - Projectile.Center;

            if (toOwner.LengthSquared() <= 0.01f)
            {
                DamageOwnerAndExplode(owner);
                return;
            }

            Vector2 desiredVelocity =
                toOwner.SafeNormalize(Vector2.UnitY) * OwnerHomingSpeed;

            Projectile.velocity =
                Vector2.Lerp(
                    Projectile.velocity,
                    desiredVelocity,
                    OwnerHomingTurnStrength
                );

            if (Projectile.velocity.LengthSquared() <= 0.01f)
            {
                Projectile.velocity = desiredVelocity;
            }

            Projectile.Center += Projectile.velocity;
        }
    }
}