namespace NeuroCompanion.Neuro
{
    public enum NeuroCompanionMode
    {
        Follow,
        TimedAttack
    }

    public static class NeuroCompanionModeExtensions
    {
        public static string ToCommandName(this NeuroCompanionMode mode)
        {
            return mode switch
            {
                NeuroCompanionMode.Follow => "follow",
                NeuroCompanionMode.TimedAttack => "attack",
                _ => "unknown"
            };
        }
    }
}