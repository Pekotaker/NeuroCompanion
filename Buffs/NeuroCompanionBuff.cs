using NeuroCompanion.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Buffs
{
    public class NeuroCompanionBuff : ModBuff
    {

        public override void SetStaticDefaults()
        {
            // The buff should not show a countdown timer.
            Main.buffNoTimeDisplay[Type] = true;

            // The buff should not persist after leaving/reloading the world.
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            bool minionIsAlive =
                player.ownedProjectileCounts[ModContent.ProjectileType<NeuroCompanionProjectile>()] > 0;

            if (minionIsAlive)
            {
                // Keep the buff alive while the minion exists.
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                // Remove the buff if the minion is gone.
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}