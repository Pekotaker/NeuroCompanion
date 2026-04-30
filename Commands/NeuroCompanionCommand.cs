using NeuroCompanion.Neuro;
using NeuroCompanion.Players;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Commands
{
    public class NeuroCompanionCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "neuro";
        public override string Usage =>
            "/neuro <help|recall|status|follow|attack|autoattack [seconds]|buff|debuff player|debuff enemy>";

        public override string Description =>
            "Controls the Neuro Companion test state.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (caller.Player == null)
            {
                caller.Reply("This command must be used by a player.");
                return;
            }

            if (args.Length == 0)
            {
                ReplyWithHelp(caller);
                return;
            }

            Player player = caller.Player;
            string subcommand = args[0].ToLowerInvariant();

            if (subcommand == "help")
            {
                ReplyWithHelp(caller);
                return;
            }

            if (subcommand == "status")
            {
                ReplyWithStatus(caller, player);
                return;
            }

            NeuroCommand command;

            if (!TryCreateCommand(args, out command))
            {
                caller.Reply($"Unknown or invalid Neuro command: {input}");
                ReplyWithHelp(caller);
                return;
            }

            NeuroActionResult result =
                NeuroActionExecutor.Execute(player, command);

            caller.Reply(result.Message);
        }

        private static void ReplyWithStatus(CommandCaller caller, Player player)
        {
            NeuroCompanionPlayer neuroPlayer =
                player.GetModPlayer<NeuroCompanionPlayer>();

            caller.Reply(
                $"Neuro companion mode: {neuroPlayer.CompanionMode.ToCommandName()}"
            );
        }

        private static bool TryCreateCommand(
            string[] args,
            out NeuroCommand command
        )
        {
            command = null;

            string subcommand = args[0].ToLowerInvariant();

            switch (subcommand)
            {
                case "recall":
                    command = new NeuroCommand(NeuroCommandType.Recall);
                    return true;

                case "follow":
                    command = new NeuroCommand(NeuroCommandType.Follow);
                    return true;

                case "attack":
                    command = new NeuroCommand(NeuroCommandType.AttackOnce);
                    return true;

                case "autoattack":
                case "attackmode":
                    command = CreateTimedAttackCommand(args);
                    return true;

                case "buff":
                    command = new NeuroCommand(NeuroCommandType.BuffPlayer);
                    return true;

                case "debuff":
                    return TryCreateDebuffCommand(args, out command);

                default:
                    return false;
            }
        }

        private static NeuroCommand CreateTimedAttackCommand(string[] args)
        {
            const int defaultSeconds = 10;

            if (args.Length < 2)
            {
                return new NeuroCommand(
                    NeuroCommandType.StartTimedAttack,
                    defaultSeconds
                );
            }

            if (!int.TryParse(args[1], out int seconds))
            {
                seconds = defaultSeconds;
            }

            return new NeuroCommand(
                NeuroCommandType.StartTimedAttack,
                seconds
            );
        }

        private static bool TryCreateDebuffCommand(
            string[] args,
            out NeuroCommand command
        )
        {
            command = null;

            if (args.Length < 2)
            {
                return false;
            }

            string target = args[1].ToLowerInvariant();

            switch (target)
            {
                case "player":
                case "self":
                    command = new NeuroCommand(NeuroCommandType.DebuffPlayer);
                    return true;

                case "enemy":
                    command = new NeuroCommand(NeuroCommandType.DebuffNearestEnemy);
                    return true;

                default:
                    return false;
            }
        }

        private static void ReplyWithHelp(CommandCaller caller)
        {
            caller.Reply("Neuro Companion commands:");
            caller.Reply("/neuro help");
            caller.Reply("/neuro recall");
            caller.Reply("/neuro status");
            caller.Reply("/neuro follow");
            caller.Reply("/neuro attack");
            caller.Reply("/neuro autoattack [seconds]");
            caller.Reply("/neuro buff");
            caller.Reply("/neuro debuff player");
            caller.Reply("/neuro debuff enemy");
        }
    }
}