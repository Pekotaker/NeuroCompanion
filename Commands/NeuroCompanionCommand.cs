
using NeuroCompanion.Commands.Neuro;
using NeuroCompanion.Commands.Neuro.Weapon;
using Terraria;
using Terraria.ModLoader;
using NeuroCompanion.Neuro.Runtime;
using NeuroCompanion.Neuro.Actions;

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
                NeuroCommandHelp.ReplyWithHelp(caller);
                return;
            }

            Player player = caller.Player;
            string subcommand = args[0].ToLowerInvariant();

            if (subcommand == "help")
            {
                NeuroCommandHelp.ReplyWithHelp(caller);
                return;
            }

            if (subcommand == "status")
            {
                NeuroStatusCommandHandler.ReplyWithStatus(caller, player);
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
                NeuroCommandHelp.ReplyWithHelp(caller);
                return;
            }

            NeuroActionResult result =
                NeuroActionExecutor.Execute(player, command);

            caller.Reply(result.Message);
        }
    }
}