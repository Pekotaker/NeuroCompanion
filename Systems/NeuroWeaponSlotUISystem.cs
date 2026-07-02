using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NeuroCompanion.Players;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using NeuroCompanion.Neuro.Actions;
using NeuroCompanion.Neuro.Weapons;

namespace NeuroCompanion.Systems
{
    public class NeuroWeaponSlotUISystem : ModSystem
    {
        private const float SlotX = 447f;
        private const float SlotY = 305f;

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextLayerIndex = layers.FindIndex(
                layer => layer.Name.Equals("Vanilla: Mouse Text")
            );

            if (mouseTextLayerIndex == -1)
            {
                return;
            }

            layers.Insert(
                mouseTextLayerIndex,
                new LegacyGameInterfaceLayer(
                    "NeuroCompanion: Neuro Weapon Slot",
                    DrawNeuroWeaponSlot,
                    InterfaceScaleType.UI
                )
            );
        }

        private bool DrawNeuroWeaponSlot()
        {
            Player player = Main.LocalPlayer;

            if (!ShouldDrawNeuroWeaponSlot(player))
            {
                return true;
            }

            NeuroCompanionPlayer neuroPlayer =
                player.GetModPlayer<NeuroCompanionPlayer>();

            Vector2 slotPosition = GetSlotPosition();

            float oldInventoryScale = Main.inventoryScale;
            Main.inventoryScale = 0.85f;

            // Draw the vanilla slot background manually so it is definitely visible.
            Main.spriteBatch.Draw(
                TextureAssets.InventoryBack.Value,
                slotPosition,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                Main.inventoryScale,
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
                0f
            );

            Item displayItem = new Item();
            displayItem.TurnToAir();

            if (neuroPlayer.HasNeuroWeapon())
            {
                displayItem = neuroPlayer.NeuroWeapon.Clone();
            }

            ItemSlot.Draw(
                Main.spriteBatch,
                ref displayItem,
                ItemSlot.Context.InventoryItem,
                slotPosition
            );

            if (IsMouseOverSlot(slotPosition))
            {
                player.mouseInterface = true;

                HandleSlotHover(neuroPlayer);
                HandleSlotClick(player);
            }

            Main.inventoryScale = oldInventoryScale;

            return true;
        }

        private static Vector2 GetSlotPosition()
        {
            return new Vector2(SlotX, SlotY);
        }

        private static bool IsMouseOverSlot(Vector2 slotPosition)
        {
            float slotSize = 52f * Main.inventoryScale;

            return Main.mouseX >= slotPosition.X &&
                   Main.mouseX <= slotPosition.X + slotSize &&
                   Main.mouseY >= slotPosition.Y &&
                   Main.mouseY <= slotPosition.Y + slotSize;
        }

        private static void HandleSlotHover(NeuroCompanionPlayer neuroPlayer)
        {
            if (neuroPlayer.HasNeuroWeapon())
            {
                Item hoverItem = neuroPlayer.NeuroWeapon.Clone();

                ItemSlot.MouseHover(
                    ref hoverItem,
                    ItemSlot.Context.InventoryItem
                );

                return;
            }

            Main.hoverItemName = "Neuro Weapon Slot";
        }

        private static void HandleSlotClick(Player player)
        {
            if (!Main.mouseLeft || !Main.mouseLeftRelease)
            {
                return;
            }

            Main.mouseLeftRelease = false;

            NeuroActionResult result =
                NeuroWeaponService.SwapMouseItemWithNeuroSlot(player);

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                Main.NewText(result.Message);
            }
        }

        private static bool ShouldDrawNeuroWeaponSlot(Player player)
        {
            if (Main.gameMenu || !Main.playerInventory)
            {
                return false;
            }

            if (player == null || !player.active)
            {
                return false;
            }

            // A chest, piggy bank, safe, defender's forge, void vault, etc. is open.
            if (player.chest != -1)
            {
                return false;
            }

            // An NPC shop is open.
            if (Main.npcShop > 0)
            {
                return false;
            }

            // Goblin Tinkerer reforge UI.
            if (Main.InReforgeMenu)
            {
                return false;
            }

            // Guide crafting UI.
            if (Main.InGuideCraftMenu)
            {
                return false;
            }

            return true;
        }
    }
}