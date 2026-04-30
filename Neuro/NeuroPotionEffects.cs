using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Neuro
{
    public static class NeuroPotionEffects
    {
        private const int TicksPerSecond = 60;
        private const int SecondsPerMinute = 60;

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

        public static void ApplyRandomRedPotionBuffs(Player player)
        {
            const int attempts = 3;

            for (int i = 0; i < attempts; i++)
            {
                int buff = RedPotionBuffs[Main.rand.Next(RedPotionBuffs.Length)];
                player.AddBuff(buff, RedPotionBuffDurationTicks);
            }
        }

        public static void ApplyRedPotionDebuffs(Player player)
        {
            int durationTicks = GetRedPotionDebuffDurationTicks();

            foreach (int debuff in RedPotionDebuffs)
            {
                player.AddBuff(debuff, durationTicks);
            }
        }

        public static int ApplyRedPotionDebuffs(NPC npc)
        {
            int appliedCount = 0;
            int durationTicks = GetRedPotionDebuffDurationTicks();

            foreach (int debuff in RedPotionDebuffs)
            {
                if (debuff < 0 || debuff >= BuffLoader.BuffCount)
                {
                    continue;
                }

                if (npc.buffImmune[debuff])
                {
                    continue;
                }

                npc.AddBuff(debuff, durationTicks);
                appliedCount++;
            }

            return appliedCount;
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