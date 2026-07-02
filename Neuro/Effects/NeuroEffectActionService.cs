using Terraria;

using NeuroCompanion.Neuro.Actions;
using NeuroCompanion.Neuro.Context;

namespace NeuroCompanion.Neuro.Effects
{
    public static class NeuroEffectActionService
    {
        private const float DebuffEnemySearchRange = 700f;

        public static NeuroActionResult BuffPlayer(
            Player player,
            int specificBuffId
        )
        {
            if (specificBuffId >= 0)
            {
                if (NeuroPotionEffects.TryApplySpecificRedPotionBuff(
                        player,
                        specificBuffId,
                        out string specificMessage
                    ))
                {
                    return NeuroActionResult.Ok(specificMessage);
                }

                return NeuroActionResult.Fail(specificMessage);
            }

            return NeuroActionResult.Ok(
                NeuroPotionEffects.ApplyPrioritizedRedPotionBuffs(player)
            );
        }

        public static NeuroActionResult DebuffPlayer(
            Player player,
            int specificDebuffId
        )
        {
            if (specificDebuffId >= 0)
            {
                if (NeuroPotionEffects.TryApplySpecificRedPotionDebuff(
                        player,
                        specificDebuffId,
                        out string specificMessage
                    ))
                {
                    return NeuroActionResult.Ok(specificMessage);
                }

                return NeuroActionResult.Fail(specificMessage);
            }

            int appliedCount = NeuroPotionEffects.ApplyRedPotionDebuffs(player);

            return NeuroActionResult.Ok(
                $"Neuro applied {appliedCount} Red Potion-style debuffs to the player."
            );
        }

        public static NeuroActionResult DebuffNearestEnemy(
            Player player,
            int specificDebuffId
        )
        {
            NPC target = NeuroNpcTargetFinder.FindNearestEnemy(
                player,
                DebuffEnemySearchRange
            );

            if (target == null)
            {
                return NeuroActionResult.Fail("No valid enemy found nearby.");
            }

            if (specificDebuffId >= 0)
            {
                if (NeuroPotionEffects.TryApplySpecificRedPotionDebuff(
                        target,
                        specificDebuffId,
                        out string specificMessage
                    ))
                {
                    return NeuroActionResult.Ok(specificMessage);
                }

                return NeuroActionResult.Fail(specificMessage);
            }

            int appliedCount = NeuroPotionEffects.ApplyRedPotionDebuffs(target);

            return NeuroActionResult.Ok(
                $"Neuro applied {appliedCount} Red Potion-style debuffs to {target.FullName}."
            );
        }
    }
}