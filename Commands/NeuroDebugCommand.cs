using NeuroCompanion.Neuro;
using NeuroCompanion.Players;
using NeuroCompanion.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Commands
{
    public class NeuroDebugCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "neurodebug";

        public override string Usage => "/neurodebug";

        public override string Description =>
            "Shows detailed Neuro Companion debug information.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Player player = caller.Player ?? Main.LocalPlayer;

            NeuroCompanionPlayer neuroPlayer =
                player.GetModPlayer<NeuroCompanionPlayer>();

            bool companionSummoned =
                player.ownedProjectileCounts[
                    ModContent.ProjectileType<NeuroCompanionProjectile>()
                ] > 0;

            caller.Reply("Neuro Companion debug:");
            caller.Reply($"- Websocket connected: {NeuroClient.Instance.IsConnected}");
            caller.Reply($"- Websocket status: {NeuroClient.Instance.LastStatus}");
            caller.Reply($"- Companion summoned: {companionSummoned}");
            caller.Reply($"- Companion mode: {neuroPlayer.CompanionMode.ToCommandName()}");
            caller.Reply(
                $"- Autoattack remaining: {neuroPlayer.GetTimedAttackSecondsRemaining()} seconds"
            );
            caller.Reply($"- Cooldowns: {NeuroActionCooldowns.GetCooldownStatusText()}");

            string[] runtimeLines =
                NeuroRuntimeStatus.BuildDebugText().Split('\n');

            foreach (string line in runtimeLines)
            {
                caller.Reply(line);
            }
        }
    }
}