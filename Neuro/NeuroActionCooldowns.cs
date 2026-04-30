using System;
using System.Collections.Generic;

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
            return commandType switch
            {
                NeuroCommandType.Recall => TimeSpan.FromSeconds(2),
                NeuroCommandType.Follow => TimeSpan.FromSeconds(1),
                NeuroCommandType.AttackOnce => TimeSpan.FromSeconds(3),
                NeuroCommandType.StartTimedAttack => TimeSpan.FromSeconds(5),
                NeuroCommandType.BuffPlayer => TimeSpan.FromSeconds(60),
                NeuroCommandType.DebuffPlayer => TimeSpan.FromSeconds(60),
                NeuroCommandType.DebuffNearestEnemy => TimeSpan.FromSeconds(30),
                _ => TimeSpan.Zero
            };
        }
    }
}