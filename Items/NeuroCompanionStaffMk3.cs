using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Buffs.Companion;

namespace NeuroCompanion.Items
{
    public class NeuroCompanionStaffMk3 : NeuroCompanionStaff
    {
        public override string Texture => "NeuroCompanion/Items/NeuroCompanionStaffMk3";

        protected override int StaffShootCooldownTicks => 1;

        protected override int StaffVisualTier => 3;

        protected override int StaffBuffType =>
            ModContent.BuffType<HallowedNeuroBuff>();

        protected override int StaffRarity => ItemRarityID.Pink;

        protected override int StaffValue =>
            Item.buyPrice(gold: 5);

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ModContent.ItemType<NeuroCompanionStaffMk2>());
            recipe.AddIngredient(ItemID.HallowedBar, 10);
            recipe.AddTile(TileID.MythrilAnvil);

            recipe.Register();
        }
    }
}