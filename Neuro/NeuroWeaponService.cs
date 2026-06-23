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
                return NeuroActionResult.Ok(
                    "Action skipped: put away the item on your cursor before assigning a weapon to Neuro."
                );
            }

            int selectedSlot = player.selectedItem;

            if (selectedSlot < 0 || selectedSlot >= player.inventory.Length)
            {
                return NeuroActionResult.Ok(
                    "Action skipped: could not find the selected inventory slot."
                );
            }

            return MoveInventoryItemToNeuroSlot(player, selectedSlot);
        }

        public static NeuroActionResult TakeStrongestFromInventory(Player player)
        {
            if (PlayerIsHoldingMouseItem())
            {
                return NeuroActionResult.Ok(
                    "Action skipped: put away the item on your cursor before letting Neuro take a weapon."
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
                return NeuroActionResult.Ok(
                    "Action skipped: no valid magic weapon found in the inventory."
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
                return NeuroActionResult.Ok(
                    "Action skipped: could not return Neuro's weapon because the inventory is full."
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

            Item sourceItem = player.inventory[inventorySlot];

            if (!NeuroWeaponValidator.IsValidNeuroWeapon(sourceItem, out string reason))
            {
                return NeuroActionResult.Ok($"Action skipped: {reason}");
            }

            Item newWeapon = sourceItem.Clone();

            if (neuroPlayer.HasNeuroWeapon())
            {
                Item oldWeapon = neuroPlayer.NeuroWeapon.Clone();

                neuroPlayer.SetNeuroWeapon(newWeapon);

                player.inventory[inventorySlot] = oldWeapon;
            }
            else
            {
                neuroPlayer.SetNeuroWeapon(newWeapon);
                sourceItem.TurnToAir();
            }

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