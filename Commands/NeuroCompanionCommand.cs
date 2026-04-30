using NeuroCompanion.Neuro;
using NeuroCompanion.Players;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Commands
{
    public class NeuroCompanionCommand : ModCommand
    {
        private const int TicksPerSecond = 60;
        private const int DefaultAttackDurationSeconds = 10;
        private const float DebuffEnemySearchRange = 700f;

        public override CommandType Type => CommandType.Chat;

        public override string Command => "neuro";

        public override string Usage =>
            "/neuro <recall|status|follow|attack|autoattack [seconds]|buff|debuff player|debuff enemy>";

        public override string Description =>
            "Controls the Neuro Companion test state.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (caller.Player == null)
            {
                caller.Reply("This command must be used by a player.");
                return;
            }

            Player player = caller.Player;
            NeuroCompanionPlayer neuroPlayer =
                player.GetModPlayer<NeuroCompanionPlayer>();

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

                case "follow":
                    neuroPlayer.StopTimedAttack();
                    caller.Reply("Neuro companion mode set to: follow");
                    break;

                case "attack":
                    neuroPlayer.RequestSingleAttack();
                    caller.Reply("Neuro companion single attack requested.");
                    break;

                case "autoattack":
                case "attackmode":
                    StartTimedAttack(caller, neuroPlayer, args);
                    break;

                case "buff":
                    NeuroPotionEffects.ApplyRandomRedPotionBuffs(player);
                    caller.Reply("Neuro applied 3 random Red Potion-style buffs to you.");
                    break;

                case "debuff":
                    HandleDebuffCommand(caller, player, args);
                    break;

                default:
                    caller.Reply($"Unknown Neuro command: {subcommand}");
                    ReplyWithHelp(caller);
                    break;
            }
        }

        private static void StartTimedAttack(
            CommandCaller caller,
            NeuroCompanionPlayer neuroPlayer,
            string[] args
        )
        {
            int seconds = DefaultAttackDurationSeconds;

            if (args.Length >= 2 && !int.TryParse(args[1], out seconds))
            {
                caller.Reply($"Invalid duration: {args[1]}");
                caller.Reply("Usage: /neuro autoattack [seconds]");
                return;
            }

            if (seconds < 1)
            {
                seconds = 1;
            }

            int durationTicks = seconds * TicksPerSecond;

            neuroPlayer.StartTimedAttack(durationTicks);

            caller.Reply($"Neuro companion will attack for {seconds} seconds.");
        }

        private static void HandleDebuffCommand(
            CommandCaller caller,
            Player player,
            string[] args
        )
        {
            if (args.Length < 2)
            {
                caller.Reply("Usage: /neuro debuff <player|enemy>");
                return;
            }

            string target = args[1].ToLowerInvariant();

            switch (target)
            {
                case "player":
                case "self":
                    NeuroPotionEffects.ApplyRedPotionDebuffs(player);
                    caller.Reply("Neuro applied Red Potion-style debuffs to you.");
                    break;

                case "enemy":
                    NPC npc = FindNearestDebuffTarget(player);

                    if (npc == null)
                    {
                        caller.Reply("No valid enemy found nearby.");
                        return;
                    }

                    int appliedCount = NeuroPotionEffects.ApplyRedPotionDebuffs(npc);
                    caller.Reply(
                        $"Neuro applied {appliedCount} Red Potion-style debuffs to {npc.FullName}."
                    );
                    break;

                default:
                    caller.Reply($"Invalid debuff target: {target}");
                    caller.Reply("Valid targets: player, enemy");
                    break;
            }
        }

        private static NPC FindNearestDebuffTarget(Player player)
        {
            NPC bestTarget = null;
            float bestDistance = DebuffEnemySearchRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || !npc.CanBeChasedBy())
                {
                    continue;
                }

                float distance = player.Distance(npc.Center);

                if (distance >= bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                bestTarget = npc;
            }

            return bestTarget;
        }

        private static void ReplyWithHelp(CommandCaller caller)
        {
            caller.Reply("Neuro Companion commands:");
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