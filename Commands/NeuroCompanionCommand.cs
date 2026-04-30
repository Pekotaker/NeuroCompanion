using NeuroCompanion.Neuro;
using NeuroCompanion.Players;
using Terraria.ModLoader;

namespace NeuroCompanion.Commands
{
    public class NeuroCompanionCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "neuro";

        public override string Usage =>
            "/neuro <recall|status|mode follow|mode attack|mode stayclose>";

        public override string Description =>
            "Controls the Neuro Companion test state.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (caller.Player == null)
            {
                caller.Reply("This command must be used by a player.");
                return;
            }

            NeuroCompanionPlayer neuroPlayer =
                caller.Player.GetModPlayer<NeuroCompanionPlayer>();

            if (args.Length == 0)
            {
                ReplyWithHelp(caller);
                return;
            }

            string subcommand = args[0].ToLowerInvariant();

            switch (subcommand)
            {
                case "recall":
                    neuroPlayer.RequestRecall();
                    caller.Reply("Neuro companion recall requested.");
                    break;

                case "status":
                    caller.Reply(
                        $"Neuro companion mode: {neuroPlayer.CompanionMode.ToCommandName()}"
                    );
                    break;

                case "mode":
                    HandleModeCommand(caller, neuroPlayer, args);
                    break;

                default:
                    caller.Reply($"Unknown Neuro command: {subcommand}");
                    ReplyWithHelp(caller);
                    break;
            }
        }

        private static void HandleModeCommand(
            CommandCaller caller,
            NeuroCompanionPlayer neuroPlayer,
            string[] args
        )
        {
            if (args.Length < 2)
            {
                caller.Reply("Missing mode.");
                caller.Reply("Usage: /neuro mode <follow|attack|stayclose>");
                return;
            }

            if (!TryParseMode(args[1], out NeuroCompanionMode mode))
            {
                caller.Reply($"Invalid mode: {args[1]}");
                caller.Reply("Valid modes: follow, attack, stayclose");
                return;
            }

            neuroPlayer.SetCompanionMode(mode);
            caller.Reply($"Neuro companion mode set to: {mode.ToCommandName()}");
        }

        private static bool TryParseMode(string value, out NeuroCompanionMode mode)
        {
            switch (value.ToLowerInvariant())
            {
                case "follow":
                case "followonly":
                    mode = NeuroCompanionMode.FollowOnly;
                    return true;

                case "attack":
                case "attacknearest":
                    mode = NeuroCompanionMode.AttackNearest;
                    return true;

                case "stay":
                case "stayclose":
                case "close":
                    mode = NeuroCompanionMode.StayClose;
                    return true;

                default:
                    mode = NeuroCompanionMode.AttackNearest;
                    return false;
            }
        }

        private static void ReplyWithHelp(CommandCaller caller)
        {
            caller.Reply("Neuro Companion commands:");
            caller.Reply("/neuro recall");
            caller.Reply("/neuro status");
            caller.Reply("/neuro mode follow");
            caller.Reply("/neuro mode attack");
            caller.Reply("/neuro mode stayclose");
        }
    }
}