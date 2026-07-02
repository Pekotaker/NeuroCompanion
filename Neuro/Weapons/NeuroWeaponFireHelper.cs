using NeuroCompanion.Players;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Neuro
{
    public static class NeuroWeaponFireHelper
    {
        public static bool TryGetAcceptedWeapon(
            Player owner,
            out NeuroCompanionPlayer neuroPlayer,
            out Item weapon,
            out NeuroWeaponClassification classification
        )
        {
            neuroPlayer = null;
            weapon = null;
            classification = null;

            if (owner == null || !owner.active)
            {
                return false;
            }

            neuroPlayer = owner.GetModPlayer<NeuroCompanionPlayer>();

            if (!neuroPlayer.HasNeuroWeapon())
            {
                return false;
            }

            weapon = neuroPlayer.NeuroWeapon;
            classification = NeuroWeaponClassifier.Classify(weapon);

            return classification.IsAccepted;
        }
    }
}