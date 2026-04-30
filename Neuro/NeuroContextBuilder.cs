using System.Text;
using NeuroCompanion.Buffs;
using NeuroCompanion.Players;
using NeuroCompanion.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Neuro
{
    public static class NeuroContextBuilder
    {
        private const float NearbyEnemyRange = 900f;
        private const int MaxEnemiesToDescribe = 5;

        public static string Build(Player player)
        {
            if (player == null || !player.active)
            {
                return "Terraria context: no active player.";
            }

            NeuroCompanionPlayer neuroPlayer =
                player.GetModPlayer<NeuroCompanionPlayer>();

            bool companionSummoned = IsCompanionSummoned(player);
            int autoAttackSecondsRemaining =
                neuroPlayer.GetTimedAttackSecondsRemaining();

            NPC nearestEnemy = FindNearestEnemy(player);
            bool validEnemyAvailable = nearestEnemy != null;

            StringBuilder builder = new();

            builder.AppendLine("Terraria context:");
            builder.AppendLine($"- Player life: {player.statLife}/{player.statLifeMax2}");
            builder.AppendLine($"- Player mana: {player.statMana}/{player.statManaMax2}");
            builder.AppendLine($"- Player position: X={player.Center.X:0}, Y={player.Center.Y:0}");
            builder.AppendLine($"- Companion summoned: {companionSummoned}");
            builder.AppendLine($"- Companion mode: {neuroPlayer.CompanionMode.ToCommandName()}");
            builder.AppendLine($"- Autoattack remaining: {autoAttackSecondsRemaining} seconds");
            builder.AppendLine($"- {NeuroActionCooldowns.GetCooldownStatusText()}");
            builder.AppendLine($"- Valid enemy target available: {validEnemyAvailable}");

            AppendNearestEnemy(builder, player, nearestEnemy);
            AppendNearbyEnemies(builder, player);
            AppendAvailableActions(builder, companionSummoned, validEnemyAvailable);

            return builder.ToString();
        }

        private static bool IsCompanionSummoned(Player player)
        {
            return player.ownedProjectileCounts[
                ModContent.ProjectileType<NeuroCompanionProjectile>()
            ] > 0;
        }

        private static NPC FindNearestEnemy(Player player)
        {
            NPC bestTarget = null;
            float bestDistance = NearbyEnemyRange;

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

        private static void AppendNearestEnemy(
            StringBuilder builder,
            Player player,
            NPC nearestEnemy
        )
        {
            if (nearestEnemy == null)
            {
                builder.AppendLine("- Nearest enemy: none");
                return;
            }

            float distance = player.Distance(nearestEnemy.Center);

            builder.AppendLine(
                $"- Nearest enemy: {nearestEnemy.FullName}, life {nearestEnemy.life}/{nearestEnemy.lifeMax}, distance {distance:0}"
            );
        }

        private static void AppendNearbyEnemies(
            StringBuilder builder,
            Player player
        )
        {
            int describedCount = 0;
            int totalNearbyEnemies = 0;
            bool bossNearby = false;

            builder.AppendLine("- Nearby enemies:");

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || !npc.CanBeChasedBy())
                {
                    continue;
                }

                float distance = player.Distance(npc.Center);

                if (distance > NearbyEnemyRange)
                {
                    continue;
                }

                totalNearbyEnemies++;

                if (npc.boss)
                {
                    bossNearby = true;
                }

                if (describedCount >= MaxEnemiesToDescribe)
                {
                    continue;
                }

                describedCount++;

                builder.AppendLine(
                    $"  {describedCount}. {npc.FullName}, life {npc.life}/{npc.lifeMax}, distance {distance:0}"
                );
            }

            if (totalNearbyEnemies == 0)
            {
                builder.AppendLine("  None.");
            }

            builder.AppendLine($"- Total nearby enemies: {totalNearbyEnemies}");
            builder.AppendLine($"- Boss nearby: {bossNearby}");
        }

        private static void AppendAvailableActions(
            StringBuilder builder,
            bool companionSummoned,
            bool validEnemyAvailable
        )
        {
            builder.AppendLine("- Available Neuro actions:");

            builder.AppendLine("  buff_player: applies 3 random Red Potion-style positive buffs to the player.");
            builder.AppendLine("  debuff_player: applies Red Potion-style debuffs to the player.");
            builder.AppendLine("  debuff_enemy: applies Red Potion-style debuffs to the nearest valid enemy.");

            if (!companionSummoned)
            {
                builder.AppendLine("  recall_companion: skipped because the companion is not summoned.");
                builder.AppendLine("  follow: skipped because the companion is not summoned.");
                builder.AppendLine("  attack_once: skipped because the companion is not summoned.");
                builder.AppendLine("  autoattack: skipped because the companion is not summoned.");
                builder.AppendLine("  The player must summon Neuro Companion with the staff before companion actions can run.");
                return;
            }

            builder.AppendLine("  recall_companion: teleports the companion back near the player.");
            builder.AppendLine("  follow: deactivates autoattack mode and makes the companion follow the player.");
            builder.AppendLine("  attack_once: makes the companion fire one Razorblade Typhoon attack.");

            if (validEnemyAvailable)
            {
                builder.AppendLine("  attack_once target behavior: fires at the nearest valid enemy.");
                builder.AppendLine("  autoattack: makes the companion automatically attack nearby enemies for a limited duration.");
            }
            else
            {
                builder.AppendLine("  attack_once target behavior: fires toward the player cursor because no valid enemy is nearby.");
                builder.AppendLine("  autoattack: activates timed attack mode; the companion will attack enemies when valid targets are nearby.");
            }
        }
    }
}