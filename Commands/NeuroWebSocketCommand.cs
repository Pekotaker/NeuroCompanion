using NeuroCompanion.Neuro;
using Terraria.ModLoader;

namespace NeuroCompanion.Commands
{
    public class NeuroWebSocketCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "neurows";

        public override string Usage => "/neurows <connect|disconnect|status>";

        public override string Description =>
            "Controls the Neuro Companion Randy websocket connection.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length == 0 || args[0].ToLowerInvariant() == "help")
            {
                ReplyWithHelp(caller);
                return;
            }

            string subcommand = args[0].ToLowerInvariant();

            switch (subcommand)
            {
                case "connect":
                    NeuroClient.Instance.Start();
                    caller.Reply("Starting Neuro websocket client.");
                    caller.Reply("Use /neurows status to check connection state.");
                    break;

                case "disconnect":
                    NeuroClient.Instance.Stop();
                    caller.Reply("Stopped Neuro websocket client.");
                    break;

                case "status":
                    caller.Reply(
                        $"Connected: {NeuroClient.Instance.IsConnected}"
                    );
                    caller.Reply(NeuroClient.Instance.LastStatus);
                    break;

                default:
                    caller.Reply($"Unknown websocket command: {subcommand}");
                    ReplyWithHelp(caller);
                    break;
            }
        }

        private static void ReplyWithHelp(CommandCaller caller)
        {
            caller.Reply("Neuro websocket commands:");
            caller.Reply("/neurows connect");
            caller.Reply("/neurows disconnect");
            caller.Reply("/neurows status");
        }
    }
}