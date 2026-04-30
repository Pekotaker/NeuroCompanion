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
        private const int ItemWidth = 32;
        private const int ItemHeight = 32;
        private const int MaxStack = 1;

        private const int UseTimeTicks = 36;
        private const int UseAnimationTicks = 36;

        private const int ManaCost = 10;
        private const int BaseDamage = 8;
        private const float KnockBack = 2f;

        private const int BuffDurationTicks = 2;

        private const float ShootSpeed = 1f;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = ItemWidth;
            Item.height = ItemHeight;
            Item.maxStack = MaxStack;

            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = UseTimeTicks;
            Item.useAnimation = UseAnimationTicks;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = false;

            Item.noMelee = true;

            Item.mana = ManaCost;

            Item.damage = BaseDamage;
            Item.knockBack = KnockBack;
            Item.DamageType = DamageClass.Summon;

            Item.shoot = ModContent.ProjectileType<NeuroCompanionProjectile>();
            Item.shootSpeed = ShootSpeed;

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
            RemoveExistingCompanions(player);

            player.AddBuff(Item.buffType, BuffDurationTicks);

            SpawnCompanionAtMouseCursor(player, source, type, damage, knockback);

            return false;
        }

        private static void RemoveExistingCompanions(Player player)
        {
            int companionProjectileType = ModContent.ProjectileType<NeuroCompanionProjectile>();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];

                if (
                    projectile.active &&
                    projectile.owner == player.whoAmI &&
                    projectile.type == companionProjectileType
                )
                {
                    projectile.Kill();
                }
            }
        }

        private static void SpawnCompanionAtMouseCursor(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            int projectileType,
            int damage,
            float knockback
        )
        {
            Projectile.NewProjectile(
                source,
                Main.MouseWorld,
                Vector2.Zero,
                projectileType,
                damage,
                knockback,
                player.whoAmI
            );
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