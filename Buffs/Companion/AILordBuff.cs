using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Buffs.Companion
{
    public class AILordBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            NeuroCompanionBuffBehavior.SetCompanionBuffDefaults(Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            NeuroCompanionBuffBehavior.UpdateCompanionBuff(
                player,
                ref buffIndex
            );
        }
    }
}