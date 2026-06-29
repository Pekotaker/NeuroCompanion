using NeuroCompanion.Configs;
using NeuroCompanion.Players;
using NeuroCompanion.Projectiles;
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

            if (NeuroActionCooldowns.IsOnCooldown(command.Type, out TimeSpan remaining))
            {
                return NeuroActionResult.Ok(
                    $"Action skipped: {command.Type} is on cooldown for {Math.Ceiling(remaining.TotalSeconds)} more seconds."
                );
            }

            NeuroCompanionPlayer neuroPlayer =
                player.GetModPlayer<NeuroCompanionPlayer>();

            bool commandRequiresCompanion =
                command.Type == NeuroCommandType.Recall ||
                command.Type == NeuroCommandType.Follow ||
                command.Type == NeuroCommandType.AttackOnce ||
                command.Type == NeuroCommandType.AttackPlayer ||
                command.Type == NeuroCommandType.StartTimedAttack;

            if (commandRequiresCompanion && !HasCompanionSummoned(player))
            {
                return NeuroActionResult.Ok(
                    "Neuro companion is not summoned, so the action was skipped. Use the Neuro Companion Staff first."
                );
            }
            bool commandRequiresWeapon =
                command.Type == NeuroCommandType.AttackOnce ||
                command.Type == NeuroCommandType.StartTimedAttack;

            if (commandRequiresWeapon && !HasUsableNeuroWeapon(player, out string weaponReason))
            {
                return NeuroActionResult.Ok(
                    $"Action skipped: {weaponReason}"
                );
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
                        BuffPlayer(player, command.EffectBuffId)
                    );

                case NeuroCommandType.DebuffPlayer:
                    {
                        NeuroActionResult debuffResult =
                            DebuffPlayer(player, command.EffectBuffId);

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
                        DebuffNearestEnemy(player, command.EffectBuffId)
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

        private static NeuroActionResult BuffPlayer(
            Player player,
            int specificBuffId
        )
        {
            if (specificBuffId >= 0)
            {
                if (NeuroPotionEffects.TryApplySpecificRedPotionBuff(
                        player,
                        specificBuffId,
                        out string specificMessage
                    ))
                {
                    return NeuroActionResult.Ok(specificMessage);
                }

                return NeuroActionResult.Fail(specificMessage);
            }

            return NeuroActionResult.Ok(
                NeuroPotionEffects.ApplyPrioritizedRedPotionBuffs(player)
            );
        }

        private static NeuroActionResult DebuffPlayer(
            Player player,
            int specificDebuffId
        )
        {
            if (specificDebuffId >= 0)
            {
                if (NeuroPotionEffects.TryApplySpecificRedPotionDebuff(
                        player,
                        specificDebuffId,
                        out string specificMessage
                    ))
                {
                    return NeuroActionResult.Ok(specificMessage);
                }

                return NeuroActionResult.Fail(specificMessage);
            }

            int appliedCount = NeuroPotionEffects.ApplyRedPotionDebuffs(player);

            return NeuroActionResult.Ok(
                $"Neuro applied {appliedCount} Red Potion-style debuffs to the player."
            );
        }

        private static NeuroActionResult DebuffNearestEnemy(
            Player player,
            int specificDebuffId
        )
        {
            NPC target = FindNearestDebuffTarget(player);

            if (target == null)
            {
                return NeuroActionResult.Fail("No valid enemy found nearby.");
            }

            if (specificDebuffId >= 0)
            {
                if (NeuroPotionEffects.TryApplySpecificRedPotionDebuff(
                        target,
                        specificDebuffId,
                        out string specificMessage
                    ))
                {
                    return NeuroActionResult.Ok(specificMessage);
                }

                return NeuroActionResult.Fail(specificMessage);
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

        private static bool HasUsableNeuroWeapon(Player player, out string reason)
        {
            NeuroCompanionPlayer neuroPlayer =
                player.GetModPlayer<NeuroCompanionPlayer>();

            if (!neuroPlayer.HasNeuroWeapon())
            {
                reason = "Neuro has no magic weapon equipped. Use /neuro weapon set or /neuro weapon take first.";
                return false;
            }

            if (!NeuroWeaponValidator.IsValidNeuroWeapon(neuroPlayer.NeuroWeapon, out reason))
            {
                reason = $"Neuro's equipped weapon is no longer valid: {reason}";
                return false;
            }

            reason = string.Empty;
            return true;
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