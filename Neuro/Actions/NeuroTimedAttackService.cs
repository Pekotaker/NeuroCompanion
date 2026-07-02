using NeuroCompanion.Configs;
using NeuroCompanion.Players;
using Terraria.ModLoader;

namespace NeuroCompanion.Neuro.Actions
{
    public static class NeuroTimedAttackService
    {
        private const int TicksPerSecond = 60;

        private const int DefaultAttackDurationSeconds = 10;
        private const int MinimumAttackDurationSeconds = 1;
        private const int HardMaximumAttackDurationSeconds = 600;

        public static NeuroActionResult StartTimedAttack(
            NeuroCompanionPlayer neuroPlayer,
            int requestedSeconds
        )
        {
            int seconds = requestedSeconds;

            if (seconds <= 0)
            {
                seconds = DefaultAttackDurationSeconds;
            }

            int maximumAttackDurationSeconds =
                GetMaximumAttackDurationSeconds();

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

        private static int GetMaximumAttackDurationSeconds()
        {
            NeuroCompanionConfig config =
                ModContent.GetInstance<NeuroCompanionConfig>();

            int maximumSeconds = config.MaxAutoAttackDurationSeconds;

            return Clamp(
                maximumSeconds,
                MinimumAttackDurationSeconds,
                HardMaximumAttackDurationSeconds
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
    }
}