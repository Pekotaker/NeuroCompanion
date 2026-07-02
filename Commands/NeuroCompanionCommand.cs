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
            "/neuro <help|status|follow|attack [player]|autoattack [seconds]|buff [buff name/id]|debuff player/enemy [debuff name/id]>";

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

            if (subcommand == "weapon")
            {
                NeuroWeaponCommandHandler.Handle(caller, player, args);
                return;
            }

            if (!NeuroChatCommandParser.TryCreateCommand(
                    args,
                    out NeuroCommand command
                ))
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

            caller.Reply(
                $"Autoattack remaining: {neuroPlayer.GetTimedAttackSecondsRemaining()} seconds"
            );

            caller.Reply(NeuroActionCooldowns.GetCooldownStatusText());
        }

        private static void ReplyWithHelp(CommandCaller caller)
        {
            caller.Reply("Neuro Companion commands:");
            caller.Reply("/neuro help");
            caller.Reply("/neuro status");
            caller.Reply("/neuro follow");
            caller.Reply("/neuro attack");
            caller.Reply("/neuro attack player");
            caller.Reply("/neuro autoattack [seconds]");
            caller.Reply("/neuro buff [buff name/id]");
            caller.Reply("/neuro debuff player [debuff name/id]");
            caller.Reply("/neuro debuff enemy [debuff name/id]");
            caller.Reply("/neuro weapon status");
            caller.Reply("/neuro weapon set");
            caller.Reply("/neuro weapon take");
            caller.Reply("/neuro weapon return");
        }
    }
}