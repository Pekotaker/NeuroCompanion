using NeuroCompanion.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Buffs
{
    public class NeuroCompanionBuff : ModBuff
    {
        // Temporary texture: use the vanilla Baby Slime buff icon.
        // Without this, tModLoader expects us to provide our own image at:
        // Buffs/NeuroCompanionBuff.png
        public override string Texture => $"Terraria/Images/Buff_{BuffID.BabySlime}";

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