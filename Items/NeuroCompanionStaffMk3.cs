using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Items
{
    public class NeuroCompanionStaffMk3 : NeuroCompanionStaff
    {
        protected override int StaffShootCooldownTicks => 12;

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