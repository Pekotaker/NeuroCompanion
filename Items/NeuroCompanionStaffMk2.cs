using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Items
{
    public class NeuroCompanionStaffMk2 : NeuroCompanionStaff
    {
        protected override int StaffShootCooldownTicks => 30;

        protected override int StaffRarity => ItemRarityID.Orange;

        protected override int StaffValue =>
            Item.buyPrice(gold: 1);

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ModContent.ItemType<NeuroCompanionStaff>());
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddTile(TileID.Anvils);

            recipe.Register();
        }
    }
}