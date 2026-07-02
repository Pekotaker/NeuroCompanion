using NeuroCompanion.Neuro;
using NeuroCompanion.Players;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Commands
{
    public static class NeuroStatusCommandHandler
    {
        public static void ReplyWithStatus(
            CommandCaller caller,
            Player player
        )
        {
            NeuroCompanionPlayer neuroPlayer =
                player.GetModPlayer<NeuroCompanionPlayer>();

            caller.Reply(
                $"Neuro companion mode: {neuroPlayer.CompanionMode.ToCommandName()}"
            );

            caller.Reply(
                $"Autoattack remaining: {neuroPlayer.GetTimedAttackSecondsRemaining()} seconds"
            );

            caller.Reply(NeuroActionCooldowns.GetCooldownStatusText());
        }
    }
}