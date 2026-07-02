using NeuroCompanion.Neuro;

namespace NeuroCompanion.Commands.Neuro
{
    public static class NeuroChatCommandParser
    {
        public static bool TryCreateCommand(
            string[] args,
            out NeuroCommand command
        )
        {
            command = null;

            if (args == null || args.Length == 0)
            {
                return false;
            }

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
                    command = CreateAttackCommand(args);
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

        private static NeuroCommand CreateAttackCommand(string[] args)
        {
            if (args.Length >= 2)
            {
                string target = args[1].ToLowerInvariant();

                if (target == "player" || target == "self")
                {
                    return new NeuroCommand(NeuroCommandType.AttackPlayer);
                }
            }

            return new NeuroCommand(NeuroCommandType.AttackOnce);
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
    }
}