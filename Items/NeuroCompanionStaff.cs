using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Items
{
    public class NeuroCompanionStaff : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.SlimeStaff}"; // TODO: Replace with custom texture

        public override void SetDefaults()
        {
            // The size of the item icon/hitbox in inventory/world.
            Item.width = 32;
            Item.height = 32;

            // This is a single staff, not a stackable material.
            Item.maxStack = 1;

            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            // Simple beginner recipe: 10 wood at a workbench.
            recipe.AddIngredient(ItemID.Wood, 10);
            recipe.AddTile(TileID.WorkBenches);

            recipe.Register();
        }
    }
}