using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Buffs
{
    public class NeuroCompanionAttackBuff : ModBuff
    {
        public override string Texture => $"Terraria/Images/Buff_{BuffID.MagicPower}";

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
    }
}