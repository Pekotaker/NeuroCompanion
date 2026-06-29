using NeuroCompanion.Neuro;
using NeuroCompanion.Players;
using Terraria;

namespace NeuroCompanion.Projectiles
{
    public partial class NeuroCompanionProjectile
    {
        private void ApplyRecallCommand(
            Player owner,
            NeuroCompanionPlayer neuroPlayer
        )
        {
            if (!neuroPlayer.ConsumeRecallRequest())
            {
                return;
            }

            State = CompanionState.Idle;
            ShootTimer = 0f;

            TeleportTo(GetIdlePosition(owner));
        }

        private void RunCommandedBehavior(
            Player owner,
            NeuroCompanionPlayer neuroPlayer
        )
        {
            if (neuroPlayer.ConsumeAttackPlayerRequest())
            {
                RunAttackPlayerCommand(owner, neuroPlayer);
                return;
            }

            if (neuroPlayer.ConsumeSingleAttackRequest())
            {
                RunSingleAttackCommand(owner);
                return;
            }

            if (neuroPlayer.CompanionMode == NeuroCompanionMode.TimedAttack)
            {
                RunTimedAttackMode(owner);
                return;
            }

            RunFollowMode(owner);
        }

        private void RunFollowMode(Player owner)
        {
            State = CompanionState.Idle;
            ShootTimer = 0f;

            FollowOwner(owner);
        }

        private void RunSingleAttackCommand(Player owner)
        {
            NPC target = FindTarget(owner);

            State = CompanionState.Attacking;

            if (target == null)
            {
                FollowOwner(owner);
                ShootWeaponTowardCursor(owner);
                return;
            }

            HoverNearOwnerForCombat(owner, target);
            ShootWeaponAtTarget(owner, target);
        }

        private void RunAttackPlayerCommand(
            Player owner,
            NeuroCompanionPlayer neuroPlayer
        )
        {
            State = CompanionState.Attacking;

            neuroPlayer.TriggerEvilVisual();

            HoverNearOwnerForEvilAttack(owner);
            ShootEvilProjectileAtOwner(owner);
        }

        private void RunTimedAttackMode(Player owner)
        {
            NPC target = FindTarget(owner);

            if (target == null)
            {
                State = CompanionState.Idle;
                FollowOwner(owner);
                return;
            }

            State = CompanionState.Attacking;

            HoverNearOwnerForCombat(owner, target);
            ShootWeaponAtTargetWhenReady(owner, target);
        }
    }
}