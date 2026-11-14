using NeuroCompanion.Players;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Neuro.Weapons.Firing
{
    public sealed class NeuroWeaponProjectileSpawnContext
    {
        public static NeuroWeaponProjectileSpawnContext Current { get; private set; }

        public Player Owner { get; }
        public NeuroCompanionPlayer NeuroPlayer { get; }
        public int Damage { get; }
        public int CritChance { get; }
        public DamageClass WeaponDamageClass { get; }
        public bool IsEvil { get; }
        public bool KillOnOwnerHit { get; }

        public bool SingleShotOnly { get; }

        private NeuroWeaponProjectileSpawnContext(
            Player owner,
            NeuroCompanionPlayer neuroPlayer,
            int damage,
            int critChance,
            DamageClass weaponDamageClass,
            bool isEvil,
            bool killOnOwnerHit,
            bool singleShotOnly
        )
        {
            Owner = owner;
            NeuroPlayer = neuroPlayer;
            Damage = damage;
            CritChance = critChance;
            WeaponDamageClass = weaponDamageClass;
            IsEvil = isEvil;
            KillOnOwnerHit = killOnOwnerHit;
            SingleShotOnly = singleShotOnly;
        }

        public static void Begin(
            Player owner,
            NeuroCompanionPlayer neuroPlayer,
            int damage,
            int critChance,
            DamageClass weaponDamageClass,
            bool isEvil,
            bool killOnOwnerHit,
            bool singleShotOnly = false
        )
        {
            Current = new NeuroWeaponProjectileSpawnContext(
                owner,
                neuroPlayer,
                damage,
                critChance,
                weaponDamageClass,
                isEvil,
                killOnOwnerHit,
                singleShotOnly
            );
        }

        public static void End()
        {
            Current = null;
        }
    }
}