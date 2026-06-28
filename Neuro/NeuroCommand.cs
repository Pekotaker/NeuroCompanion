namespace NeuroCompanion.Neuro
{
    public enum NeuroCommandType
    {
        Recall,
        Follow,
        AttackOnce,
        StartTimedAttack,
        BuffPlayer,
        DebuffPlayer,
        DebuffNearestEnemy,
        WeaponStatus,
        EquipWeaponFromInventory,
        ReturnWeaponToPlayer,
    }

    public class NeuroCommand
    {
        public const int NoSpecificEffect = -1;

        public NeuroCommandType Type { get; }
        public int DurationSeconds { get; }
        public int EffectBuffId { get; }

        public bool HasSpecificEffect => EffectBuffId >= 0;

        public NeuroCommand(
            NeuroCommandType type,
            int durationSeconds = 10,
            int effectBuffId = NoSpecificEffect
        )
        {
            Type = type;
            DurationSeconds = durationSeconds;
            EffectBuffId = effectBuffId;
        }
    }
}