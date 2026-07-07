using Terraria;
using Terraria.DataStructures;

using NeuroCompanion.Players;

namespace NeuroCompanion.Neuro.Weapons.Firing
{
    public readonly struct PendingNeuroWeaponShot
    {
        public IEntitySource Source { get; }
        public int ProjectileOwner { get; }
        public Player Owner { get; }
        public NeuroCompanionPlayer NeuroPlayer { get; }
        public NeuroWeaponShot Shot { get; }
        public int Damage { get; }
        public float KnockBack { get; }
        public int CritChance { get; }
        public bool IsEvil { get; }
        public bool KillOnOwnerHit { get; }
        public int RemainingTicks { get; }

        public PendingNeuroWeaponShot(
            IEntitySource source,
            int projectileOwner,
            Player owner,
            NeuroCompanionPlayer neuroPlayer,
            NeuroWeaponShot shot,
            int damage,
            float knockBack,
            int critChance,
            bool isEvil,
            bool killOnOwnerHit,
            int remainingTicks
        )
        {
            Source = source;
            ProjectileOwner = projectileOwner;
            Owner = owner;
            NeuroPlayer = neuroPlayer;
            Shot = shot;
            Damage = damage;
            KnockBack = knockBack;
            CritChance = critChance;
            IsEvil = isEvil;
            KillOnOwnerHit = killOnOwnerHit;
            RemainingTicks = remainingTicks;
        }

        public PendingNeuroWeaponShot WithRemainingTicks(int remainingTicks)
        {
            return new PendingNeuroWeaponShot(
                Source,
                ProjectileOwner,
                Owner,
                NeuroPlayer,
                Shot,
                Damage,
                KnockBack,
                CritChance,
                IsEvil,
                KillOnOwnerHit,
                remainingTicks
            );
        }
    }
}