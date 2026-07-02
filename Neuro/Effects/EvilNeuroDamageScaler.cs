using NeuroCompanion.Players;
using Terraria;

namespace NeuroCompanion.Neuro
{
    public static class EvilNeuroDamageScaler
    {
        private const int Mk1BaseDamage = 100;
        private const int Mk2BaseDamage = 150;
        private const int Mk3BaseDamage = 200;
        private const int Mk4BaseDamage = 400;

        private const float DifficultyDamageIncreasePerLevel = 0.33f;

        public static int GetFallbackBoltDamage(
            NeuroCompanionPlayer neuroPlayer
        )
        {
            int baseDamage = GetFallbackBoltBaseDamage(neuroPlayer);
            float difficultyMultiplier = GetWorldDifficultyDamageMultiplier();

            int finalDamage = (int)System.Math.Round(
                baseDamage * difficultyMultiplier,
                System.MidpointRounding.AwayFromZero
            );

            return finalDamage < 1 ? 1 : finalDamage;
        }

        private static int GetFallbackBoltBaseDamage(
            NeuroCompanionPlayer neuroPlayer
        )
        {
            if (neuroPlayer.NeuroStaffCanDetectThroughBlocks)
            {
                return Mk4BaseDamage;
            }

            if (neuroPlayer.NeuroStaffShootCooldownTicks <= 1)
            {
                return Mk3BaseDamage;
            }

            if (neuroPlayer.NeuroStaffShootCooldownTicks <= 30)
            {
                return Mk2BaseDamage;
            }

            return Mk1BaseDamage;
        }

        private static float GetWorldDifficultyDamageMultiplier()
        {
            int difficultyLevel = GetWorldDifficultyLevel();

            return 1f + difficultyLevel * DifficultyDamageIncreasePerLevel;
        }

        private static int GetWorldDifficultyLevel()
        {
            if (Main.masterMode && (Main.getGoodWorld || Main.zenithWorld))
            {
                return 3;
            }

            if (Main.masterMode)
            {
                return 2;
            }

            if (Main.expertMode)
            {
                return 1;
            }

            return 0;
        }
    }
}