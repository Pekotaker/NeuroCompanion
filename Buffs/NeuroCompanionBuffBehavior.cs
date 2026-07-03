using Terraria;
using Terraria.ModLoader;

using NeuroCompanion.Buffs.Companion;
using NeuroCompanion.Projectiles.Companion;

namespace NeuroCompanion.Buffs
{
    public static class NeuroCompanionBuffBehavior
    {
        public static void SetCompanionBuffDefaults(int buffType)
        {
            // The buff should not show a countdown timer.
            Main.buffNoTimeDisplay[buffType] = true;

            // The buff should not persist after leaving/reloading the world.
            Main.buffNoSave[buffType] = true;
        }

        public static void UpdateCompanionBuff(
            Player player,
            ref int buffIndex
        )
        {
            bool minionIsAlive =
                player.ownedProjectileCounts[
                    ModContent.ProjectileType<NeuroCompanionProjectile>()
                ] > 0;

            if (minionIsAlive)
            {
                // Keep the buff alive while the minion exists.
                player.buffTime[buffIndex] = 18000;
                return;
            }

            // Remove the buff if the minion is gone.
            player.DelBuff(buffIndex);
            buffIndex--;
        }

        public static bool PlayerHasAnyCompanionBuff(Player player)
        {
            return player.HasBuff(ModContent.BuffType<NeuroCompanionBuff>()) ||
                   player.HasBuff(ModContent.BuffType<HellfireNeuroBuff>()) ||
                   player.HasBuff(ModContent.BuffType<HallowedNeuroBuff>()) ||
                   player.HasBuff(ModContent.BuffType<AILordBuff>());
        }

        public static void ClearCompanionBuffs(Player player)
        {
            player.ClearBuff(ModContent.BuffType<NeuroCompanionBuff>());
            player.ClearBuff(ModContent.BuffType<HellfireNeuroBuff>());
            player.ClearBuff(ModContent.BuffType<HallowedNeuroBuff>());
            player.ClearBuff(ModContent.BuffType<AILordBuff>());
        }
    }
}