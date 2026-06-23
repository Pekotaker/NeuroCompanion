using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Neuro
{
    public static class NeuroWeaponValidator
    {
        public static bool IsValidNeuroWeapon(Item item, out string reason)
        {
            reason = string.Empty;

            if (item == null || item.IsAir)
            {
                reason = "No item selected.";
                return false;
            }

            if (item.damage <= 0)
            {
                reason = $"{item.Name} does not deal damage.";
                return false;
            }

            if (item.DamageType != DamageClass.Magic)
            {
                reason = $"{item.Name} is not a magic weapon.";
                return false;
            }

            if (item.mana <= 0)
            {
                reason = $"{item.Name} does not use mana.";
                return false;
            }

            if (item.shoot <= ProjectileID.None)
            {
                reason = $"{item.Name} does not shoot a projectile.";
                return false;
            }

            if (item.channel)
            {
                reason = $"{item.Name} is a channeling weapon, so Neuro cannot use it yet.";
                return false;
            }

            if (IsBannedWeapon(item.type))
            {
                reason = $"{item.Name} is not supported because it is not a simple direct-fire magic weapon.";
                return false;
            }

            return true;
        }

        private static bool IsBannedWeapon(int itemType)
        {
            return itemType == ItemID.ClingerStaff ||
                   itemType == ItemID.NimbusRod ||
                   itemType == ItemID.CrimsonRod ||
                   itemType == ItemID.MagnetSphere ||
                   itemType == ItemID.RainbowGun ||
                   itemType == ItemID.LastPrism ||
                   itemType == ItemID.MagicMissile ||
                   itemType == ItemID.Flamelash ||
                   itemType == ItemID.RainbowRod;
        }
    }
}