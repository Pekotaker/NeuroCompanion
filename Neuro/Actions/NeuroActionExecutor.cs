using NeuroCompanion.Configs;
using NeuroCompanion.Players;
using System;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Neuro
{
    public static class NeuroActionExecutor
    {
        private const int TicksPerSecond = 60;

        private const int DefaultAttackDurationSeconds = 10;
        private const int MinimumAttackDurationSeconds = 1;

        private const float DebuffEnemySearchRange = 700f;

        public static NeuroActionResult Execute(
            Player player,
            NeuroCommand command
        )
        {
            if (player == null || !player.active)
            {
                return NeuroActionResult.Fail("No active player was available.");
            }

            if (NeuroActionCooldowns.IsOnCooldown(command.Type, out TimeSpan remaining))
            {
                return NeuroActionResult.Ok(
                    $"Action skipped: {command.Type} is on cooldown for {Math.Ceiling(remaining.TotalSeconds)} more seconds."
                );
            }

            NeuroCompanionPlayer neuroPlayer =
                player.GetModPlayer<NeuroCompanionPlayer>();

            if (!NeuroCommandRequirements.TryValidate(
                    player,
                    command,
                    out NeuroActionResult validationResult
                ))
            {
                return validationResult;
            }


            switch (command.Type)
            {
                case NeuroCommandType.Recall:
                    neuroPlayer.RequestRecall();
                    return StartCooldownAndReturn(
                        command.Type,
                        NeuroActionResult.Ok("Neuro companion recall requested.")
                    );

                case NeuroCommandType.Follow:
                    neuroPlayer.StopTimedAttack();
                    return StartCooldownAndReturn(
                        command.Type,
                        NeuroActionResult.Ok("Neuro companion returned to follow mode.")
                    );

                case NeuroCommandType.AttackOnce:
                    neuroPlayer.RequestSingleAttack();
                    return StartCooldownAndReturn(
                        command.Type,
                        NeuroActionResult.Ok("Neuro companion single attack requested.")
                    );

                case NeuroCommandType.StartTimedAttack:
                    return StartCooldownAndReturn(
                        command.Type,
                        StartTimedAttack(neuroPlayer, command.DurationSeconds)
                    );

                case NeuroCommandType.AttackPlayer:
                    neuroPlayer.RequestAttackPlayer();
                    neuroPlayer.TriggerEvilVisual();

                    return StartCooldownAndReturn(
                        command.Type,
                        NeuroActionResult.Ok("Evil Neuro attack requested.")
                    );

                case NeuroCommandType.BuffPlayer:
                    return StartCooldownAndReturn(
                        command.Type,
                        NeuroEffectActionService.BuffPlayer(
                            player,
                            command.EffectBuffId
                        )
                    );

                case NeuroCommandType.DebuffPlayer:
                    {
                        NeuroActionResult debuffResult =
                            NeuroEffectActionService.DebuffPlayer(
                                player,
                                command.EffectBuffId
                            );

                        if (debuffResult.Success)
                        {
                            neuroPlayer.TriggerEvilVisual();
                        }

                        return StartCooldownAndReturn(
                            command.Type,
                            debuffResult
                        );
                    }

                case NeuroCommandType.DebuffNearestEnemy:
                    return StartCooldownAndReturn(
                        command.Type,
                        NeuroEffectActionService.DebuffNearestEnemy(
                            player,
                            command.EffectBuffId
                        )
                    );

                case NeuroCommandType.WeaponStatus:
                    return NeuroActionResult.Ok(
                        NeuroWeaponService.GetStatusText(player)
                    );

                case NeuroCommandType.EquipWeaponFromInventory:
                    return StartCooldownAndReturn(
                        command.Type,
                        NeuroWeaponService.TakeStrongestFromInventory(player)
                    );

                case NeuroCommandType.ReturnWeaponToPlayer:
                    return StartCooldownAndReturn(
                        command.Type,
                        NeuroWeaponService.ReturnWeaponToInventory(player)
                    );

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

            int maximumAttackDurationSeconds = GetMaximumAttackDurationSeconds();

            seconds = Clamp(
                seconds,
                MinimumAttackDurationSeconds,
                maximumAttackDurationSeconds
            );

            neuroPlayer.StartTimedAttack(seconds * TicksPerSecond);

            return NeuroActionResult.Ok(
                $"Neuro companion will attack for {seconds} seconds. Current maximum is {maximumAttackDurationSeconds} seconds."
            );
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

        private static NeuroActionResult StartCooldownAndReturn(
            NeuroCommandType commandType,
            NeuroActionResult result
        )
        {
            if (result.Success)
            {
                NeuroActionCooldowns.StartCooldown(commandType);
            }

            return result;
        }

        private static int GetMaximumAttackDurationSeconds()
        {
            NeuroCompanionConfig config =
                ModContent.GetInstance<NeuroCompanionConfig>();

            int maximumSeconds = config.MaxAutoAttackDurationSeconds;

            if (maximumSeconds < MinimumAttackDurationSeconds)
            {
                return MinimumAttackDurationSeconds;
            }

            if (maximumSeconds > 600)
            {
                return 600;
            }

            return maximumSeconds;
        }
    }
}