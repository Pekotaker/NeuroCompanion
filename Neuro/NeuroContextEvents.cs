using NeuroCompanion.Buffs;
using NeuroCompanion.Players;
using NeuroCompanion.Projectiles;
using NeuroCompanion.Configs;
using Terraria;
using Terraria.ModLoader;


namespace NeuroCompanion.Neuro
{
    public class NeuroContextEvents
    {
        private const float NearbyEnemyRange = 900f;
        private const float LowHealthRatio = 0.35f;

        private bool previousCompanionSummoned;
        private bool previousLowHealth;
        private bool previousAutoattackActive;
        private bool previousBossNearby;


        public void Reset()
        {
            previousCompanionSummoned = false;
            previousLowHealth = false;
            previousAutoattackActive = false;
            previousBossNearby = false;
        }

        public void CheckAndSendEvents(Player player)
        {
            NeuroCompanionConfig config =
                ModContent.GetInstance<NeuroCompanionConfig>();

            if (!config.EnableEventContextMessages)
            {
                return;
            }

            if (!NeuroClient.Instance.IsConnected)
            {
                return;
            }

            if (player == null || !player.active)
            {
                return;
            }

            CheckCompanionSummonedEvent(player);
            CheckLowHealthEvent(player);
            CheckAutoattackEvent(player);
            CheckEnemyEvents(player);
        }

        private void CheckCompanionSummonedEvent(Player player)
        {
            bool companionSummoned = IsCompanionSummoned(player);

            if (companionSummoned == previousCompanionSummoned)
            {
                return;
            }

            previousCompanionSummoned = companionSummoned;

            string message = companionSummoned
                ? "Terraria event: Neuro Companion has been summoned."
                : "Terraria event: Neuro Companion is no longer summoned.";

            NeuroClient.Instance.SendContext(message, silent: false);
        }

        private void CheckLowHealthEvent(Player player)
        {
            int lowHealthPercent =
                ModContent.GetInstance<NeuroCompanionConfig>().LowHealthPercent;

            if (lowHealthPercent < 1)
            {
                lowHealthPercent = 1;
            }

            if (lowHealthPercent > 100)
            {
                lowHealthPercent = 100;
            }

            bool lowHealth =
                player.statLifeMax2 > 0 &&
                player.statLife * 100 <= player.statLifeMax2 * lowHealthPercent;

            if (lowHealth == previousLowHealth)
            {
                return;
            }

            previousLowHealth = lowHealth;

            if (lowHealth)
            {
                NeuroClient.Instance.SendContext(
                    $"Terraria event: Player health is low: {player.statLife}/{player.statLifeMax2}.",
                    silent: false
                );
            }
            else
            {
                NeuroClient.Instance.SendContext(
                    $"Terraria event: Player health recovered: {player.statLife}/{player.statLifeMax2}.",
                    silent: true
                );
            }
        }

        private void CheckAutoattackEvent(Player player)
        {
            bool autoattackActive =
                player.HasBuff(ModContent.BuffType<NeuroCompanionAttackBuff>());

            if (autoattackActive == previousAutoattackActive)
            {
                return;
            }

            previousAutoattackActive = autoattackActive;

            NeuroCompanionPlayer neuroPlayer =
                player.GetModPlayer<NeuroCompanionPlayer>();

            string message = autoattackActive
                ? $"Terraria event: Autoattack mode started. Remaining time: {neuroPlayer.GetTimedAttackSecondsRemaining()} seconds."
                : "Terraria event: Autoattack mode ended. Neuro Companion returned to follow mode.";

            NeuroClient.Instance.SendContext(message, silent: false);
        }

        private void CheckEnemyEvents(Player player)
        {
            NPC boss = null;

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

                if (npc.boss)
                {
                    boss = npc;
                    break;
                }
            }

            bool bossNearby = boss != null;

            if (bossNearby == previousBossNearby)
            {
                return;
            }

            previousBossNearby = bossNearby;

            string message = bossNearby
                ? $"Terraria event: Boss nearby: {boss.FullName}."
                : "Terraria event: No boss nearby.";

            NeuroClient.Instance.SendContext(message, silent: false);
        }

        private static bool IsCompanionSummoned(Player player)
        {
            return player.ownedProjectileCounts[
                ModContent.ProjectileType<NeuroCompanionProjectile>()
            ] > 0;
        }
    }
}