using System;
using System.Collections.Generic;
using NeuroCompanion.Configs;
using Terraria.ModLoader;

namespace NeuroCompanion.Neuro
{
    public static class NeuroActionCooldowns
    {
        private static readonly Dictionary<NeuroCommandType, DateTime> CooldownEndTimes = new();

        public static bool IsOnCooldown(
            NeuroCommandType commandType,
            out TimeSpan remaining
        )
        {
            remaining = TimeSpan.Zero;

            if (!CooldownEndTimes.TryGetValue(commandType, out DateTime cooldownEndTime))
            {
                return false;
            }

            DateTime now = DateTime.UtcNow;

            if (now >= cooldownEndTime)
            {
                CooldownEndTimes.Remove(commandType);
                return false;
            }

            remaining = cooldownEndTime - now;
            return true;
        }

        public static void StartCooldown(NeuroCommandType commandType)
        {
            TimeSpan cooldown = GetCooldown(commandType);

            if (cooldown <= TimeSpan.Zero)
            {
                return;
            }

            CooldownEndTimes[commandType] = DateTime.UtcNow + cooldown;
        }

        public static string GetCooldownStatusText()
        {
            List<string> activeCooldowns = new();

            foreach (NeuroCommandType commandType in Enum.GetValues<NeuroCommandType>())
            {
                if (!IsOnCooldown(commandType, out TimeSpan remaining))
                {
                    continue;
                }

                activeCooldowns.Add(
                    $"{commandType}: {Math.Ceiling(remaining.TotalSeconds)}s"
                );
            }

            if (activeCooldowns.Count == 0)
            {
                return "No Neuro actions are on cooldown.";
            }

            return "Neuro action cooldowns: " + string.Join(", ", activeCooldowns);
        }

        private static TimeSpan GetCooldown(NeuroCommandType commandType)
        {
            NeuroCompanionConfig config =
                ModContent.GetInstance<NeuroCompanionConfig>();

            int seconds = commandType switch
            {
                NeuroCommandType.Recall => config.RecallCooldownSeconds,
                NeuroCommandType.Follow => config.FollowCooldownSeconds,
                NeuroCommandType.AttackOnce => config.AttackOnceCooldownSeconds,
                NeuroCommandType.StartTimedAttack => config.AutoAttackCooldownSeconds,
                NeuroCommandType.BuffPlayer => config.BuffPlayerCooldownSeconds,
                NeuroCommandType.DebuffPlayer => config.DebuffPlayerCooldownSeconds,
                NeuroCommandType.DebuffNearestEnemy => config.DebuffEnemyCooldownSeconds,
                _ => 0
            };

            if (seconds < 0)
            {
                seconds = 0;
            }

            return TimeSpan.FromSeconds(seconds);
        }
    }
}