using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Neuro.Effects
{
    public static class NeuroPotionEffects
    {
        private const int TicksPerSecond = 60;
        private const int SecondsPerMinute = 60;
        private const int RandomBuffCount = 3;

        private const int RedPotionBuffDurationTicks =
            30 * SecondsPerMinute * TicksPerSecond;

        private static readonly int[] RedPotionBuffs =
        {
            BuffID.ObsidianSkin,
            BuffID.Regeneration,
            BuffID.Swiftness,
            BuffID.Ironskin,
            BuffID.ManaRegeneration,
            BuffID.MagicPower,
            BuffID.Featherfall,
            BuffID.Spelunker,
            BuffID.Archery,
            BuffID.Heartreach,
            BuffID.Hunter,
            BuffID.Endurance,
            BuffID.Lifeforce,
            BuffID.Inferno,
            BuffID.Mining,
            BuffID.Rage,
            BuffID.Wrath,
            BuffID.Dangersense
        };

        private static readonly int[] RedPotionDebuffs =
        {
            BuffID.Poisoned,
            BuffID.Darkness,
            BuffID.Cursed,
            BuffID.OnFire,
            BuffID.Bleeding,
            BuffID.Confused,
            BuffID.Slow,
            BuffID.Weak,
            BuffID.Silenced,
            BuffID.BrokenArmor,
            BuffID.Suffocation
        };

        public static string ApplyPrioritizedRedPotionBuffs(Player player)
        {
            List<int> appliedBuffs = new();

            List<int> missingBuffs = GetAllowedBuffsPlayerDoesNotHave(
                player,
                RedPotionBuffs
            );

            ApplyRandomBuffsFromPool(
                player,
                missingBuffs,
                RandomBuffCount,
                appliedBuffs,
                RedPotionBuffDurationTicks
            );

            if (appliedBuffs.Count < RandomBuffCount)
            {
                List<int> fallbackBuffs = GetAllowedBuffsNotAlreadyApplied(
                    RedPotionBuffs,
                    appliedBuffs
                );

                ApplyRandomBuffsFromPool(
                    player,
                    fallbackBuffs,
                    RandomBuffCount - appliedBuffs.Count,
                    appliedBuffs,
                    RedPotionBuffDurationTicks
                );
            }

            return $"Neuro applied buffs: {NeuroBuffLookup.FormatBuffList(appliedBuffs)}.";
        }

        public static bool TryApplySpecificRedPotionBuff(
            Player player,
            int buffId,
            out string message
        )
        {
            if (!NeuroBuffLookup.IsAllowed(buffId, RedPotionBuffs))
            {
                message =
                    $"{NeuroBuffLookup.GetBuffNameSafe(buffId)} is not an allowed Neuro positive buff.";
                return false;
            }

            player.AddBuff(buffId, RedPotionBuffDurationTicks);

            message = $"Neuro applied {NeuroBuffLookup.GetBuffNameSafe(buffId)} to the player.";
            return true;
        }

        public static int ApplyRedPotionDebuffs(Player player)
        {
            int durationTicks = GetRedPotionDebuffDurationTicks();
            int appliedCount = 0;

            foreach (int debuff in RedPotionDebuffs)
            {
                player.AddBuff(debuff, durationTicks);
                appliedCount++;
            }

            return appliedCount;
        }

        public static bool TryApplySpecificRedPotionDebuff(
            Player player,
            int debuffId,
            out string message
        )
        {
            if (!NeuroBuffLookup.IsAllowed(debuffId, RedPotionDebuffs))
            {
                message =
                    $"{NeuroBuffLookup.GetBuffNameSafe(debuffId)} is not an allowed Neuro debuff.";
                return false;
            }

            player.AddBuff(debuffId, GetRedPotionDebuffDurationTicks());

            message = $"Neuro applied {NeuroBuffLookup.GetBuffNameSafe(debuffId)} to the player.";
            return true;
        }

        public static int ApplyRedPotionDebuffs(NPC npc)
        {
            int appliedCount = 0;
            int durationTicks = GetRedPotionDebuffDurationTicks();

            foreach (int debuff in RedPotionDebuffs)
            {
                if (!CanNpcReceiveDebuff(npc, debuff))
                {
                    continue;
                }

                npc.AddBuff(debuff, durationTicks);
                appliedCount++;
            }

            return appliedCount;
        }

        public static bool TryApplySpecificRedPotionDebuff(
            NPC npc,
            int debuffId,
            out string message
        )
        {
            if (!NeuroBuffLookup.IsAllowed(debuffId, RedPotionDebuffs))
            {
                message =
                    $"{NeuroBuffLookup.GetBuffNameSafe(debuffId)} is not an allowed Neuro debuff.";
                return false;
            }

            if (!CanNpcReceiveDebuff(npc, debuffId))
            {
                message =
                    $"{npc.FullName} is immune to {NeuroBuffLookup.GetBuffNameSafe(debuffId)}.";
                return false;
            }

            npc.AddBuff(debuffId, GetRedPotionDebuffDurationTicks());

            message =
                $"Neuro applied {NeuroBuffLookup.GetBuffNameSafe(debuffId)} to {npc.FullName}.";
            return true;
        }

        public static bool TryFindPositiveBuff(
            string input,
            out int buffId,
            out string error
        )
        {
            return NeuroBuffLookup.TryFindAllowedBuff(
                input,
                RedPotionDebuffs,
                "debuff",
                out buffId,
                out error
            );
        }

        public static bool TryFindDebuff(
            string input,
            out int buffId,
            out string error
        )
        {
            return NeuroBuffLookup.TryFindAllowedBuff(
                input,
                RedPotionBuffs,
                "positive buff",
                out buffId,
                out error
            );
        }

        public static string GetAllowedPositiveBuffListText()
        {
            return NeuroBuffLookup.FormatAllowedBuffList(RedPotionBuffs);
        }

        public static string GetAllowedDebuffListText()
        {
            return NeuroBuffLookup.FormatAllowedBuffList(RedPotionDebuffs);
        }

        private static List<int> GetAllowedBuffsPlayerDoesNotHave(
            Player player,
            int[] allowedBuffs
        )
        {
            List<int> result = new();

            foreach (int buff in allowedBuffs)
            {
                if (!PlayerHasBuff(player, buff))
                {
                    result.Add(buff);
                }
            }

            return result;
        }

        private static List<int> GetAllowedBuffsNotAlreadyApplied(
            int[] allowedBuffs,
            List<int> appliedBuffs
        )
        {
            List<int> result = new();

            foreach (int buff in allowedBuffs)
            {
                if (!appliedBuffs.Contains(buff))
                {
                    result.Add(buff);
                }
            }

            return result;
        }

        private static void ApplyRandomBuffsFromPool(
            Player player,
            List<int> pool,
            int count,
            List<int> appliedBuffs,
            int durationTicks
        )
        {
            while (count > 0 && pool.Count > 0)
            {
                int index = Main.rand.Next(pool.Count);
                int buff = pool[index];

                pool.RemoveAt(index);

                player.AddBuff(buff, durationTicks);
                appliedBuffs.Add(buff);

                count--;
            }
        }

        private static bool PlayerHasBuff(Player player, int buffId)
        {
            for (int i = 0; i < player.buffType.Length; i++)
            {
                if (player.buffType[i] == buffId)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CanNpcReceiveDebuff(NPC npc, int debuffId)
        {
            if (debuffId < 0 || debuffId >= BuffLoader.BuffCount)
            {
                return false;
            }

            if (npc.buffImmune[debuffId])
            {
                return false;
            }

            return true;
        }

        private static int GetRedPotionDebuffDurationTicks()
        {
            int hours = 1;

            if (Main.masterMode)
            {
                hours = 3;
            }
            else if (Main.expertMode)
            {
                hours = 2;
            }

            return hours * SecondsPerMinute * SecondsPerMinute * TicksPerSecond;
        }
    }
}