using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Buffs
{
    public class NeuroCompanionAttackBuff : ModBuff
    {

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
    }
}