using NeuroCompanion.Players;
using NeuroCompanion.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Neuro
{
    public static class NeuroActionExecutor
    {
        private const int TicksPerSecond = 60;

        private const int DefaultAttackDurationSeconds = 10;
        private const int MinimumAttackDurationSeconds = 1;
        private const int MaximumAttackDurationSeconds = 180;

        private const float DebuffEnemySearchRange = 700f;

        private static bool HasCompanionSummoned(Player player)
        {
            return player.ownedProjectileCounts[
                ModContent.ProjectileType<NeuroCompanionProjectile>()
            ] > 0;
        }

        public static NeuroActionResult Execute(
            Player player,
            NeuroCommand command
        )
        {
            if (player == null || !player.active)
            {
                return NeuroActionResult.Fail("No active player was available.");
            }

            NeuroCompanionPlayer neuroPlayer =
                player.GetModPlayer<NeuroCompanionPlayer>();

            bool commandRequiresCompanion =
                command.Type == NeuroCommandType.Recall ||
                command.Type == NeuroCommandType.Follow ||
                command.Type == NeuroCommandType.AttackOnce ||
                command.Type == NeuroCommandType.StartTimedAttack;

            if (commandRequiresCompanion && !HasCompanionSummoned(player))
            {
                return NeuroActionResult.Ok(
                    "Neuro companion is not summoned, so the action was skipped. Use the Neuro Companion Staff first."
                );
            }


            switch (command.Type)
            {
                case NeuroCommandType.Recall:
                    neuroPlayer.RequestRecall();
                    return NeuroActionResult.Ok("Neuro companion recall requested.");

                case NeuroCommandType.Follow:
                    neuroPlayer.StopTimedAttack();
                    return NeuroActionResult.Ok("Neuro companion returned to follow mode.");

                case NeuroCommandType.AttackOnce:
                    neuroPlayer.RequestSingleAttack();
                    return NeuroActionResult.Ok("Neuro companion single attack requested.");

                case NeuroCommandType.StartTimedAttack:
                    return StartTimedAttack(neuroPlayer, command.DurationSeconds);

                case NeuroCommandType.BuffPlayer:
                    NeuroPotionEffects.ApplyRandomRedPotionBuffs(player);
                    return NeuroActionResult.Ok(
                        "Neuro applied 3 random Red Potion-style buffs to the player."
                    );

                case NeuroCommandType.DebuffPlayer:
                    NeuroPotionEffects.ApplyRedPotionDebuffs(player);
                    return NeuroActionResult.Ok(
                        "Neuro applied Red Potion-style debuffs to the player."
                    );

                case NeuroCommandType.DebuffNearestEnemy:
                    return DebuffNearestEnemy(player);

                default:
                    return NeuroActionResult.Fail($"Unknown Neuro command: {command.Type}");
            }
        }

        private static NeuroActionResult StartTimedAttack(
            NeuroCompanionPlayer neuroPlayer,
            int requestedSeconds
        )
        {
            int seconds = requestedSeconds;

            if (seconds <= 0)
            {
                seconds = DefaultAttackDurationSeconds;
            }

            seconds = Clamp(
                seconds,
                MinimumAttackDurationSeconds,
                MaximumAttackDurationSeconds
            );

            neuroPlayer.StartTimedAttack(seconds * TicksPerSecond);

            return NeuroActionResult.Ok(
                $"Neuro companion will attack for {seconds} seconds."
            );
        }

        private static NeuroActionResult DebuffNearestEnemy(Player player)
        {
            NPC target = FindNearestDebuffTarget(player);

            if (target == null)
            {
                return NeuroActionResult.Fail("No valid enemy found nearby.");
            }

            int appliedCount = NeuroPotionEffects.ApplyRedPotionDebuffs(target);

            return NeuroActionResult.Ok(
                $"Neuro applied {appliedCount} Red Potion-style debuffs to {target.FullName}."
            );
        }

        private static NPC FindNearestDebuffTarget(Player player)
        {
            NPC bestTarget = null;
            float bestDistance = DebuffEnemySearchRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || !npc.CanBeChasedBy())
                {
                    continue;
                }

                float distance = player.Distance(npc.Center);

                if (distance >= bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                bestTarget = npc;
            }

            return bestTarget;
        }

        private static int Clamp(int value, int minimum, int maximum)
        {
            if (value < minimum)
            {
                return minimum;
            }

            if (value > maximum)
            {
                return maximum;
            }

            return value;
        }
    }
}