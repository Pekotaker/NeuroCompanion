using NeuroCompanion.Players;
using NeuroCompanion.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Neuro
{
    public static class NeuroCommandRequirements
    {
        public static bool TryValidate(
            Player player,
            NeuroCommand command,
            out NeuroActionResult result
        )
        {
            result = null;

            if (RequiresCompanion(command.Type) && !HasCompanionSummoned(player))
            {
                result = NeuroActionResult.Ok(
                    "Neuro companion is not summoned, so the action was skipped. Use the Neuro Companion Staff first."
                );

                return false;
            }

            if (RequiresWeapon(command.Type) && !HasUsableNeuroWeapon(player, out string weaponReason))
            {
                result = NeuroActionResult.Ok(
                    $"Action skipped: {weaponReason}"
                );

                return false;
            }

            return true;
        }

        private static bool RequiresCompanion(NeuroCommandType commandType)
        {
            return
                commandType == NeuroCommandType.Recall ||
                commandType == NeuroCommandType.Follow ||
                commandType == NeuroCommandType.AttackOnce ||
                commandType == NeuroCommandType.AttackPlayer ||
                commandType == NeuroCommandType.StartTimedAttack;
        }

        private static bool RequiresWeapon(NeuroCommandType commandType)
        {
            return
                commandType == NeuroCommandType.AttackOnce ||
                commandType == NeuroCommandType.StartTimedAttack;
        }

        private static bool HasCompanionSummoned(Player player)
        {
            return player.ownedProjectileCounts[
                ModContent.ProjectileType<NeuroCompanionProjectile>()
            ] > 0;
        }

        private static bool HasUsableNeuroWeapon(Player player, out string reason)
        {
            NeuroCompanionPlayer neuroPlayer =
                player.GetModPlayer<NeuroCompanionPlayer>();

            if (!neuroPlayer.HasNeuroWeapon())
            {
                reason = "Neuro has no magic weapon equipped. Use /neuro weapon set or /neuro weapon take first.";
                return false;
            }

            if (!NeuroWeaponValidator.IsValidNeuroWeapon(neuroPlayer.NeuroWeapon, out reason))
            {
                reason = $"Neuro's equipped weapon is no longer valid: {reason}";
                return false;
            }

            reason = string.Empty;
            return true;
        }
    }
}