using NeuroCompanion.Players;
using Terraria;

namespace NeuroCompanion.Neuro.Weapons.Firing
{
    public sealed class NeuroWeaponProjectileSpawnContext
    {
        public static NeuroWeaponProjectileSpawnContext Current { get; private set; }

        public Player Owner { get; }
        public NeuroCompanionPlayer NeuroPlayer { get; }
        public int Damage { get; }
        public int CritChance { get; }
        public bool IsEvil { get; }
        public bool KillOnOwnerHit { get; }

        private NeuroWeaponProjectileSpawnContext(
            Player owner,
            NeuroCompanionPlayer neuroPlayer,
            int damage,
            int critChance,
            bool isEvil,
            bool killOnOwnerHit
        )
        {
            Owner = owner;
            NeuroPlayer = neuroPlayer;
            Damage = damage;
            CritChance = critChance;
            IsEvil = isEvil;
            KillOnOwnerHit = killOnOwnerHit;
        }

        public static void Begin(
            Player owner,
            NeuroCompanionPlayer neuroPlayer,
            int damage,
            int critChance,
            bool isEvil,
            bool killOnOwnerHit
        )
        {
            Current = new NeuroWeaponProjectileSpawnContext(
                owner,
                neuroPlayer,
                damage,
                critChance,
                isEvil,
                killOnOwnerHit
            );
        }

        public static void End()
        {
            Current = null;
        }
    }
}