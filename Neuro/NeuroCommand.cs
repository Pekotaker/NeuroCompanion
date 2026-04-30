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
        DebuffNearestEnemy
    }

    public class NeuroCommand
    {
        public NeuroCommandType Type { get; }
        public int DurationSeconds { get; }

        public NeuroCommand(
            NeuroCommandType type,
            int durationSeconds = 10
        )
        {
            Type = type;
            DurationSeconds = durationSeconds;
        }
    }
}