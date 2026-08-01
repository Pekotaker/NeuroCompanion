using Microsoft.Xna.Framework;

using Terraria;

namespace NeuroCompanion.Projectiles.Companion
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
                FarInertia,
                TeleportDistance
            );

            RotateForMovement(IdleRotationFactor);
        }

        private Vector2 GetIdlePosition(Player owner)
        {
            return owner.Center +
                   new Vector2(
                       -IdleOffsetX * owner.direction,
                       IdleOffsetY
                   );
        }

        private void PositionForImmediateEnemyAttack(
            Player owner,
            NPC target
        )
        {
            Vector2 destination =
                GetCombatPosition(owner, target);

            // A one-shot command is consumed immediately.
            // Teleport when Neuro is not already close enough so the
            // attack is not wasted while she is still approaching.
            if (!IsNearPosition(
                    destination,
                    CombatReadyDistance
                ))
            {
                TeleportTo(destination);
            }
            else
            {
                MoveTowardCombatPosition(destination);
            }

            FaceTarget(target);
            RotateForMovement(AttackRotationFactor);
        }

        private bool MoveIntoSustainedEnemyAttackPosition(
            Player owner,
            NPC target
        )
        {
            Vector2 destination =
                GetCombatPosition(owner, target);

            MoveTowardCombatPosition(destination);

            FaceTarget(target);
            RotateForMovement(AttackRotationFactor);

            return IsNearPosition(
                destination,
                CombatReadyDistance
            );
        }

        private void MoveTowardCombatPosition(
            Vector2 destination
        )
        {
            MoveToward(
                destination,
                IdleMoveSpeed,
                IdleInertia,
                FarMoveSpeed,
                FarInertia,
                CombatTeleportDistance
            );
        }

        private void PositionForEvilAttack(Player owner)
        {
            Vector2 destination =
                GetEvilAttackPosition(owner);

            MoveToward(
                destination,
                IdleMoveSpeed,
                IdleInertia,
                FarMoveSpeed,
                FarInertia,
                EvilAttackTeleportDistance
            );

            FaceOwner(owner);
            RotateForMovement(AttackRotationFactor);
        }

        private Vector2 GetCombatPosition(
    Player owner,
    NPC target
)
        {
            Vector2 leftPosition =
                GetLeftCombatPosition(target);

            Vector2 rightPosition =
                GetRightCombatPosition(target);

            bool leftIsCloser =
                Vector2.DistanceSquared(
                    Projectile.Center,
                    leftPosition
                ) <=
                Vector2.DistanceSquared(
                    Projectile.Center,
                    rightPosition
                );

            Vector2 closestSidePosition =
                leftIsCloser
                    ? leftPosition
                    : rightPosition;

            Vector2 alternateSidePosition =
                leftIsCloser
                    ? rightPosition
                    : leftPosition;

            bool requiresClearShot =
                !CanDetectTargetsThroughBlocks(owner);

            if (
                IsCombatPositionUsable(
                    closestSidePosition,
                    target,
                    requiresClearShot
                )
            )
            {
                return closestSidePosition;
            }

            if (
                IsCombatPositionUsable(
                    alternateSidePosition,
                    target,
                    requiresClearShot
                )
            )
            {
                return alternateSidePosition;
            }

            Vector2 directOverheadPosition =
                GetDirectOverheadCombatPosition(target);

            if (
                IsCombatPositionUsable(
                    directOverheadPosition,
                    target,
                    requiresClearShot
                )
            )
            {
                return directOverheadPosition;
            }

            // When every tested position is obstructed, use the closer
            // side instead of defaulting to a potentially distant side.
            return closestSidePosition;
        }

        private static Vector2 GetLeftCombatPosition(
    NPC target
)
        {
            return new Vector2(
                target.position.X -
                CombatHorizontalGap,

                target.position.Y -
                CombatVerticalGap
            );
        }

        private static Vector2 GetRightCombatPosition(
            NPC target
        )
        {
            return new Vector2(
                target.position.X +
                target.width +
                CombatHorizontalGap,

                target.position.Y -
                CombatVerticalGap
            );
        }

        private static Vector2 GetDirectOverheadCombatPosition(
            NPC target
        )
        {
            return new Vector2(
                target.Center.X,

                target.position.Y -
                CombatVerticalGap -
                DirectOverheadExtraHeight
            );
        }

        private Vector2 GetEvilAttackPosition(Player owner)
        {
            return owner.Center +
                   new Vector2(
                       0f,
                       -EvilAttackHoverHeight
                   );
        }

        private bool IsCombatPositionUsable(
            Vector2 centerPosition,
            NPC target,
            bool requiresClearShot
        )
        {
            Vector2 topLeft =
                GetProjectileTopLeftAt(centerPosition);

            if (Collision.SolidCollision(
                    topLeft,
                    Projectile.width,
                    Projectile.height
                ))
            {
                return false;
            }

            if (!requiresClearShot)
            {
                return true;
            }

            return Collision.CanHitLine(
                topLeft,
                Projectile.width,
                Projectile.height,
                target.position,
                target.width,
                target.height
            );
        }

        private Vector2 GetProjectileTopLeftAt(
            Vector2 centerPosition
        )
        {
            return centerPosition -
                   new Vector2(
                       Projectile.width * 0.5f,
                       Projectile.height * 0.5f
                   );
        }

        private bool IsNearPosition(
            Vector2 destination,
            float maximumDistance
        )
        {
            return Vector2.DistanceSquared(
                       Projectile.Center,
                       destination
                   ) <=
                   maximumDistance * maximumDistance;
        }

        private void MoveToward(
            Vector2 destination,
            float normalSpeed,
            float normalInertia,
            float farSpeed,
            float farInertia,
            float teleportDistance
        )
        {
            float distance =
                Vector2.Distance(
                    Projectile.Center,
                    destination
                );

            if (distance > teleportDistance)
            {
                TeleportTo(destination);
                return;
            }

            float speed =
                distance > FarDistance
                    ? farSpeed
                    : normalSpeed;

            float inertia =
                distance > FarDistance
                    ? farInertia
                    : normalInertia;

            if (distance > ArriveDistance)
            {
                MoveWithInertia(
                    destination,
                    speed,
                    inertia
                );
            }
            else
            {
                Projectile.velocity *=
                    SlowdownWhenArrived;
            }

            FaceMovementDirection();
        }

        private void MoveWithInertia(
            Vector2 destination,
            float speed,
            float inertia
        )
        {
            Vector2 direction =
                destination - Projectile.Center;

            direction =
                direction.SafeNormalize(Vector2.Zero);

            Vector2 desiredVelocity =
                direction * speed;

            Projectile.velocity =
                (
                    Projectile.velocity *
                    (inertia - 1f) +
                    desiredVelocity
                ) /
                inertia;
        }

        private void TeleportTo(Vector2 destination)
        {
            Vector2 departurePosition =
                Projectile.Center;

            Projectile.Center = destination;
            Projectile.velocity = Vector2.Zero;
            Projectile.netUpdate = true;

            CreateTeleportEffects(
                departurePosition,
                destination
            );
        }

        private void FaceMovementDirection()
        {
            if (
                Projectile.velocity.X >
                MinimumFacingVelocity
            )
            {
                Projectile.spriteDirection = 1;
            }
            else if (
                Projectile.velocity.X <
                -MinimumFacingVelocity
            )
            {
                Projectile.spriteDirection = -1;
            }
        }

        private void FaceOwner(Player owner)
        {
            Projectile.spriteDirection =
                owner.Center.X >= Projectile.Center.X
                    ? 1
                    : -1;
        }

        private void FaceTarget(NPC target)
        {
            Projectile.spriteDirection =
                target.Center.X >= Projectile.Center.X
                    ? 1
                    : -1;
        }

        private void RotateForMovement(
            float rotationFactor
        )
        {
            Projectile.rotation =
                Projectile.velocity.X *
                rotationFactor;
        }
    }
}