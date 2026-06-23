using NeuroCompanion.Players;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Neuro
{
    public static class NeuroWeaponService
    {
        public static string GetStatusText(Player player)
        {
            NeuroCompanionPlayer neuroPlayer =
                player.GetModPlayer<NeuroCompanionPlayer>();

            if (!neuroPlayer.HasNeuroWeapon())
            {
                return "Neuro weapon slot is empty.";
            }

            Item weapon = neuroPlayer.NeuroWeapon;

            return $"Neuro weapon: {weapon.HoverName} | Damage: {weapon.damage} | Mana: {weapon.mana}";
        }

        public static NeuroActionResult SetFromSelectedItem(Player player)
        {
            if (PlayerIsHoldingMouseItem())
            {
                return NeuroActionResult.Fail(
                    "Put away the item on your cursor before assigning a weapon to Neuro."
                );
            }

            int selectedSlot = player.selectedItem;

            if (selectedSlot < 0 || selectedSlot >= player.inventory.Length)
            {
                return NeuroActionResult.Fail("Could not find the selected inventory slot.");
            }

            return MoveInventoryItemToNeuroSlot(player, selectedSlot);
        }

        public static NeuroActionResult TakeStrongestFromInventory(Player player)
        {
            if (PlayerIsHoldingMouseItem())
            {
                return NeuroActionResult.Fail(
                    "Put away the item on your cursor before letting Neuro take a weapon."
                );
            }

            int selectedSlot = player.selectedItem;

            int bestSlot = -1;
            int bestDamage = -1;

            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (i == selectedSlot)
                {
                    continue;
                }

                Item item = player.inventory[i];

                if (!NeuroWeaponValidator.IsValidNeuroWeapon(item, out _))
                {
                    continue;
                }

                if (item.damage <= bestDamage)
                {
                    continue;
                }

                bestDamage = item.damage;
                bestSlot = i;
            }

            if (bestSlot == -1)
            {
                return NeuroActionResult.Fail(
                    "No valid magic weapon found in the inventory."
                );
            }

            return MoveInventoryItemToNeuroSlot(player, bestSlot);
        }

        public static NeuroActionResult ReturnWeaponToInventory(Player player)
        {
            NeuroCompanionPlayer neuroPlayer =
                player.GetModPlayer<NeuroCompanionPlayer>();

            if (!neuroPlayer.HasNeuroWeapon())
            {
                return NeuroActionResult.Ok("Neuro weapon slot is already empty.");
            }

            int emptySlot = FindEmptyInventorySlot(player);

            if (emptySlot == -1)
            {
                return NeuroActionResult.Fail(
                    "Could not return Neuro's weapon because the inventory is full."
                );
            }

            Item returnedWeapon = neuroPlayer.NeuroWeapon.Clone();

            player.inventory[emptySlot] = returnedWeapon;
            neuroPlayer.ClearNeuroWeapon();

            return NeuroActionResult.Ok(
                $"Returned {returnedWeapon.HoverName} to the player inventory."
            );
        }

        private static NeuroActionResult MoveInventoryItemToNeuroSlot(
            Player player,
            int inventorySlot
        )
        {
            NeuroCompanionPlayer neuroPlayer =
                player.GetModPlayer<NeuroCompanionPlayer>();

            if (neuroPlayer.HasNeuroWeapon())
            {
                return NeuroActionResult.Fail(
                    $"Neuro already has {neuroPlayer.NeuroWeapon.HoverName}. Use /neuro weapon return first."
                );
            }

            Item item = player.inventory[inventorySlot];

            if (!NeuroWeaponValidator.IsValidNeuroWeapon(item, out string reason))
            {
                return NeuroActionResult.Fail(reason);
            }

            neuroPlayer.SetNeuroWeapon(item);
            item.TurnToAir();

            return NeuroActionResult.Ok(
                $"Neuro equipped {neuroPlayer.NeuroWeapon.HoverName}."
            );
        }

        private static int FindEmptyInventorySlot(Player player)
        {
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].IsAir)
                {
                    return i;
                }
            }

            return -1;
        }

        private static bool PlayerIsHoldingMouseItem()
        {
            return Main.mouseItem != null && !Main.mouseItem.IsAir;
        }
    }
}