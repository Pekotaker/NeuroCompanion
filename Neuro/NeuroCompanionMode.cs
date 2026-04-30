namespace NeuroCompanion.Neuro
{
    public enum NeuroCompanionMode
    {
        AttackNearest,
        FollowOnly,
        StayClose
    }

    public static class NeuroCompanionModeExtensions
    {
        public static string ToCommandName(this NeuroCompanionMode mode)
        {
            return mode switch
            {
                NeuroCompanionMode.AttackNearest => "attack",
                NeuroCompanionMode.FollowOnly => "follow",
                NeuroCompanionMode.StayClose => "stayclose",
                _ => "unknown"
            };
        }
    }
}