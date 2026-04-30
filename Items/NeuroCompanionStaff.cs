using Microsoft.Xna.Framework;
using NeuroCompanion.Buffs;
using NeuroCompanion.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Items
{
    public class NeuroCompanionStaff : ModItem
    {
        // Temporary texture: vanilla Slime Staff.
        public override string Texture => $"Terraria/Images/Item_{ItemID.SlimeStaff}";

        public override void SetStaticDefaults()
        {
            // This item summons one minion that uses one minion slot.
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 1;

            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = false;

            Item.noMelee = true;

            Item.mana = 10;

            Item.damage = 8;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Summon;

            Item.shoot = ModContent.ProjectileType<NeuroCompanionProjectile>();
            Item.shootSpeed = 1f;

            Item.buffType = ModContent.BuffType<NeuroCompanionBuff>();
        }

        public override bool Shoot(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback
        )
        {
            // Remove any existing Neuro Companion owned by this player.
            // This keeps the design rule: Neuro has one body, not many clones.
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];

                if (
                    projectile.active &&
                    projectile.owner == player.whoAmI &&
                    projectile.type == ModContent.ProjectileType<NeuroCompanionProjectile>()
                )
                {
                    projectile.Kill();
                }
            }

            // Add the buff.
            player.AddBuff(Item.buffType, 2);

            // Spawn the companion at the mouse cursor.
            Projectile.NewProjectile(
                source,
                Main.MouseWorld,
                Vector2.Zero,
                type,
                damage,
                knockback,
                player.whoAmI
            );

            // Return false because we manually spawned the projectile above.
            return false;
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