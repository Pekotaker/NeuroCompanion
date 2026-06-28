using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Items
{
    public class NeuroCompanionStaffMk4 : NeuroCompanionStaff
    {
        protected override int StaffShootCooldownTicks => 1;

        protected override int StaffRarity => ItemRarityID.Red;

        protected override int StaffValue =>
            Item.buyPrice(gold: 20);

        protected override bool StaffCanDetectThroughBlocks => true;

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ModContent.ItemType<NeuroCompanionStaffMk3>());
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);

            recipe.Register();
        }
    }
}