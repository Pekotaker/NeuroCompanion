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

            bool companionSummoned =
                player.ownedProjectileCounts[
                    ModContent.ProjectileType<NeuroCompanionProjectile>()
                ] > 0;

            bool attackBuffActive =
                player.HasBuff(ModContent.BuffType<NeuroCompanionAttackBuff>());

            StringBuilder builder = new();

            builder.AppendLine("Terraria context:");
            builder.AppendLine($"- Player life: {player.statLife}/{player.statLifeMax2}");
            builder.AppendLine($"- Player mana: {player.statMana}/{player.statManaMax2}");
            builder.AppendLine($"- Player position: X={player.Center.X:0}, Y={player.Center.Y:0}");
            builder.AppendLine($"- Companion summoned: {companionSummoned}");
            builder.AppendLine($"- Companion mode: {neuroPlayer.CompanionMode.ToCommandName()}");
            builder.AppendLine($"- Timed attack buff active: {attackBuffActive}");

            AppendNearbyEnemies(builder, player);

            return builder.ToString();
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
    }
}