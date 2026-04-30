using NeuroCompanion.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Items
{
    public class NeuroCompanionStaff : ModItem
    {
        // Temporary texture: use vanilla Slime Staff sprite.
        public override string Texture => $"Terraria/Images/Item_{ItemID.SlimeStaff}";

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 1;

            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;

            // Basic use behavior.
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = false;

            // Important: this staff itself should not hit enemies like a sword.
            Item.noMelee = true;

            // Mana cost, because this is becoming summon/magic-like.
            Item.mana = 5;

            // Damage info.
            Item.damage = 8;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Summon;

            // This connects the item to our custom projectile.
            Item.shoot = ModContent.ProjectileType<NeuroCompanionProjectile>();
            Item.shootSpeed = 8f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ItemID.Wood, 10);
            recipe.AddTile(TileID.WorkBenches);

            recipe.Register();
        }
    }
}