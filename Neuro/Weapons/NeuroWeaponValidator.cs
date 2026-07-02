using Terraria;

namespace NeuroCompanion.Neuro.Weapons
{
    public static class NeuroWeaponValidator
    {
        public static bool IsValidNeuroWeapon(Item item, out string reason)
        {
            NeuroWeaponClassification classification =
                NeuroWeaponClassifier.Classify(item);

            reason = classification.Reason;

            return classification.IsAccepted;
        }
    }
}