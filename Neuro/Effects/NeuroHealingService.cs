using System;

using Terraria;
using Terraria.Audio;
using Terraria.ID;

using NeuroCompanion.Neuro.Actions;

namespace NeuroCompanion.Neuro.Effects
{
    public static class NeuroHealingService
    {
        public static NeuroActionResult HealPlayer(
            Player player,
            int requestedHealAmount
        )
        {
            if (
                player == null ||
                !player.active
            )
            {
                return NeuroActionResult.Fail(
                    "No active player was available."
                );
            }

            if (player.dead)
            {
                return NeuroActionResult.Fail(
                    "Neuro cannot heal a dead player."
                );
            }

            if (requestedHealAmount <= 0)
            {
                return NeuroActionResult.Fail(
                    "Heal amount must be greater than zero."
                );
            }

            int missingHealth =
                Math.Max(
                    0,
                    player.statLifeMax2 -
                    player.statLife
                );

            if (missingHealth == 0)
            {
                return NeuroActionResult.Fail(
                    "The player is already at full health."
                );
            }

            int actualHealAmount =
                Math.Min(
                    requestedHealAmount,
                    missingHealth
                );

            ApplyHealing(
                player,
                actualHealAmount
            );

            return NeuroActionResult.Ok(
                $"Neuro healed the player for {actualHealAmount} HP."
            );
        }

        private static void ApplyHealing(
            Player player,
            int healAmount
        )
        {
            player.statLife += healAmount;

            player.HealEffect(
                healAmount,
                broadcast: true
            );

            if (Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(
                    SoundID.Item4,
                    player.Center
                );
            }
        }
    }
}