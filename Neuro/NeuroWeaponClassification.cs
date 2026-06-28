namespace NeuroCompanion.Neuro
{
    public enum NeuroWeaponKind
    {
        Invalid,
        DirectFire,
        Controlled,
        Channeling,
        TargetedArea,
        HeldBeam,
        Support
    }

    public sealed class NeuroWeaponClassification
    {
        public NeuroWeaponKind Kind { get; }
        public string Reason { get; }

        public bool IsAccepted =>
            Kind == NeuroWeaponKind.DirectFire ||
            Kind == NeuroWeaponKind.Controlled ||
            Kind == NeuroWeaponKind.Channeling ||
            Kind == NeuroWeaponKind.TargetedArea;

        public NeuroWeaponClassification(
            NeuroWeaponKind kind,
            string reason
        )
        {
            Kind = kind;
            Reason = reason;
        }
    }
}