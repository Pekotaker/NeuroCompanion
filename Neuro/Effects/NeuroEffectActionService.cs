using Terraria;

namespace NeuroCompanion.Neuro
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
            NPC target = FindNearestDebuffTarget(player);

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

        private static NPC FindNearestDebuffTarget(Player player)
        {
            NPC bestTarget = null;
            float bestDistance = DebuffEnemySearchRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || !npc.CanBeChasedBy())
                {
                    continue;
                }

                float distance = player.Distance(npc.Center);

                if (distance >= bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                bestTarget = npc;
            }

            return bestTarget;
        }
    }
}