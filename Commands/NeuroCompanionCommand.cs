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
            "/neuro <help|recall|status|follow|attack|autoattack [seconds]|buff [buff name/id]|debuff player/enemy [debuff name/id]>";

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
                HandleWeaponCommand(caller, player, args);
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

            caller.Reply(
                $"Autoattack remaining: {neuroPlayer.GetTimedAttackSecondsRemaining()} seconds"
            );

            caller.Reply(NeuroActionCooldowns.GetCooldownStatusText());
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
                    return TryCreateBuffCommand(args, out command);

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


        private static bool TryCreateBuffCommand(
            string[] args,
            out NeuroCommand command
        )
        {
            command = null;

            if (args.Length < 2)
            {
                command = new NeuroCommand(NeuroCommandType.BuffPlayer);
                return true;
            }

            string buffInput = JoinArgs(args, 1);

            if (!NeuroPotionEffects.TryFindPositiveBuff(
                    buffInput,
                    out int buffId,
                    out _
                ))
            {
                return false;
            }

            command = new NeuroCommand(
                NeuroCommandType.BuffPlayer,
                effectBuffId: buffId
            );

            return true;
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
            NeuroCommandType commandType;

            switch (target)
            {
                case "player":
                case "self":
                    commandType = NeuroCommandType.DebuffPlayer;
                    break;

                case "enemy":
                    commandType = NeuroCommandType.DebuffNearestEnemy;
                    break;

                default:
                    return false;
            }

            if (args.Length < 3)
            {
                command = new NeuroCommand(commandType);
                return true;
            }

            string debuffInput = JoinArgs(args, 2);

            if (!NeuroPotionEffects.TryFindDebuff(
                    debuffInput,
                    out int debuffId,
                    out _
                ))
            {
                return false;
            }

            command = new NeuroCommand(
                commandType,
                effectBuffId: debuffId
            );

            return true;
        }

        private static string JoinArgs(string[] args, int startIndex)
        {
            string result = string.Empty;

            for (int i = startIndex; i < args.Length; i++)
            {
                if (i > startIndex)
                {
                    result += " ";
                }

                result += args[i];
            }

            return result;
        }

        private static void HandleWeaponCommand(
            CommandCaller caller,
            Player player,
            string[] args
        )
        {
            if (args.Length < 2)
            {
                ReplyWithWeaponHelp(caller);
                return;
            }

            string weaponCommand = args[1].ToLowerInvariant();

            NeuroActionResult result;

            switch (weaponCommand)
            {
                case "status":
                    caller.Reply(NeuroWeaponService.GetStatusText(player));
                    return;

                case "set":
                    result = NeuroWeaponService.SetFromSelectedItem(player);
                    caller.Reply(result.Message);
                    return;

                case "take":
                    result = NeuroWeaponService.TakeStrongestFromInventory(player);
                    caller.Reply(result.Message);
                    return;

                case "return":
                    result = NeuroWeaponService.ReturnWeaponToInventory(player);
                    caller.Reply(result.Message);
                    return;

                case "inspect":
                    caller.Reply(NeuroWeaponService.InspectSelectedItem(player));
                    return;

                default:
                    caller.Reply($"Unknown weapon command: {weaponCommand}");
                    ReplyWithWeaponHelp(caller);
                    return;
            }
        }

        private static void ReplyWithWeaponHelp(CommandCaller caller)
        {
            caller.Reply("Neuro weapon commands:");
            caller.Reply("/neuro weapon status");
            caller.Reply("/neuro weapon set");
            caller.Reply("/neuro weapon take");
            caller.Reply("/neuro weapon return");
            caller.Reply("/neuro weapon inspect");
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