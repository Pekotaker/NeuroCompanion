using Microsoft.Xna.Framework;
using Terraria;

namespace NeuroCompanion.Projectiles
{
    public partial class NeuroCompanionProjectile
    {
        private void FollowOwner(Player owner)
        {
            MoveToward(
                GetIdlePosition(owner),
                IdleMoveSpeed,
                IdleInertia,
                FarMoveSpeed,
                FarInertia
            );

            RotateForMovement(IdleRotationFactor);
        }

        private Vector2 GetIdlePosition(Player owner)
        {
            return owner.Center + new Vector2(
                -IdleOffsetX * owner.direction,
                IdleOffsetY
            );
        }

        private void HoverNearOwnerForCombat(Player owner, NPC target)
        {
            MoveToward(
                GetCombatPosition(owner, target),
                IdleMoveSpeed,
                IdleInertia,
                FarMoveSpeed,
                FarInertia
            );

            FaceTarget(target);
            RotateForMovement(AttackRotationFactor);
        }

        private Vector2 GetCombatPosition(Player owner, NPC target)
        {
            float sideFacingTarget = target.Center.X >= owner.Center.X ? 1f : -1f;

            return owner.Center + new Vector2(
                CombatOffsetX * sideFacingTarget,
                CombatOffsetY
            );
        }

        private void MoveToward(
            Vector2 destination,
            float normalSpeed,
            float normalInertia,
            float farSpeed,
            float farInertia
        )
        {
            float distance = Vector2.Distance(Projectile.Center, destination);

            if (distance > TeleportDistance)
            {
                TeleportTo(destination);
                return;
            }

            float speed = distance > FarDistance ? farSpeed : normalSpeed;
            float inertia = distance > FarDistance ? farInertia : normalInertia;

            if (distance > ArriveDistance)
            {
                MoveWithInertia(destination, speed, inertia);
            }
            else
            {
                Projectile.velocity *= SlowdownWhenArrived;
            }

            FaceMovementDirection();
        }

        private void MoveWithInertia(
            Vector2 destination,
            float speed,
            float inertia
        )
        {
            Vector2 direction = destination - Projectile.Center;
            direction = direction.SafeNormalize(Vector2.Zero);

            Vector2 desiredVelocity = direction * speed;

            Projectile.velocity =
                (Projectile.velocity * (inertia - 1f) + desiredVelocity) / inertia;
        }

        private void TeleportTo(Vector2 destination)
        {
            Projectile.Center = destination;
            Projectile.velocity = Vector2.Zero;
            Projectile.netUpdate = true;
        }

        private void FaceMovementDirection()
        {
            if (Projectile.velocity.X > MinimumFacingVelocity)
            {
                Projectile.spriteDirection = 1;
            }
            else if (Projectile.velocity.X < -MinimumFacingVelocity)
            {
                Projectile.spriteDirection = -1;
            }
        }

        private void FaceTarget(NPC target)
        {
            Projectile.spriteDirection = target.Center.X >= Projectile.Center.X ? 1 : -1;
        }

        private void RotateForMovement(float rotationFactor)
        {
            Projectile.rotation = Projectile.velocity.X * rotationFactor;
        }
    }
}