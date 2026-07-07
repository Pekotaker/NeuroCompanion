using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Neuro.Weapons;
using NeuroCompanion.Neuro.Weapons.Firing;
using NeuroCompanion.Players;

using NeuroCompanion.Projectiles.Attacks;
using NeuroCompanion.Projectiles.Globals;

namespace NeuroCompanion.Projectiles.Helpers
{
    public static class NeuroProjectileSpawner
    {
        private static readonly List<PendingWeaponShot> PendingWeaponShots = new List<PendingWeaponShot>();

        public static void UpdateDelayedWeaponShots()
        {
            for (int i = PendingWeaponShots.Count - 1; i >= 0; i--)
            {
                PendingWeaponShot pendingShot = PendingWeaponShots[i];

                if (pendingShot.TicksRemaining > 0)
                {
                    pendingShot.TicksRemaining--;
                    PendingWeaponShots[i] = pendingShot;
                    continue;
                }

                PendingWeaponShots.RemoveAt(i);
                SpawnQueuedWeaponShot(pendingShot);
            }
        }

        public static void SpawnCompanionWeaponProjectile(
            IEntitySource source,
            int projectileOwner,
            Player owner,
            NeuroCompanionPlayer neuroPlayer,
            Item weapon,
            Vector2 position,
            Vector2 velocity,
            Vector2 targetPosition
        )
        {
            SpawnWeaponProjectileWithContext(
                source,
                projectileOwner,
                owner,
                neuroPlayer,
                weapon,
                position,
                velocity,
                targetPosition,
                isEvil: false,
                killOnOwnerHit: false
            );
        }

        public static void SpawnEvilWeaponProjectile(
            IEntitySource source,
            int projectileOwner,
            Player owner,
            NeuroCompanionPlayer neuroPlayer,
            Item weapon,
            Vector2 position,
            Vector2 velocity,
            Vector2 targetPosition
        )
        {
            SpawnWeaponProjectileWithContext(
                source,
                projectileOwner,
                owner,
                neuroPlayer,
                weapon,
                position,
                velocity,
                targetPosition,
                isEvil: true,
                killOnOwnerHit: false
            );
        }

        public static void SpawnFallbackEvilBolt(
            IEntitySource source,
            int projectileOwner,
            Vector2 position,
            Vector2 velocity,
            int damage,
            float knockBack
        )
        {
            if (!TrySpawnProjectile(
                    source,
                    position,
                    velocity,
                    ModContent.ProjectileType<EvilNeuroBoltProjectile>(),
                    damage,
                    knockBack,
                    projectileOwner,
                    out Projectile spawnedProjectile
                ))
            {
                return;
            }

            ApplyEvilOwnerDamageBehavior(
                spawnedProjectile,
                damage,
                killOnOwnerHit: true
            );
        }

        private static void SpawnWeaponProjectileWithContext(
            IEntitySource source,
            int projectileOwner,
            Player owner,
            NeuroCompanionPlayer neuroPlayer,
            Item weapon,
            Vector2 position,
            Vector2 velocity,
            Vector2 targetPosition,
            bool isEvil,
            bool killOnOwnerHit
        )
        {
            if (
                owner == null ||
                neuroPlayer == null ||
                weapon == null ||
                weapon.IsAir
            )
            {
                return;
            }

            int damage = NeuroDamageService.GetNeuroWeaponDamage(
                owner,
                weapon,
                neuroPlayer.NeuroStaffPrefix
            );

            float knockBack = NeuroDamageService.GetNeuroWeaponKnockBack(
                owner,
                weapon,
                neuroPlayer.NeuroStaffPrefix
            );

            int critChance = NeuroDamageService.GetNeuroWeaponCritChance(
                owner,
                weapon,
                neuroPlayer.NeuroStaffPrefix
            );

            NeuroWeaponShot[] shots =
                NeuroWeaponShotProfile.CreateShots(
                    owner,
                    weapon,
                    position,
                    velocity,
                    targetPosition
                );

            if (shots.Length <= 0)
            {
                return;
            }

            for (int i = 0; i < shots.Length; i++)
            {
                NeuroWeaponShot shot = shots[i];

                if (shot.DelayTicks <= 0)
                {
                    SpawnPreparedWeaponShot(
                        source,
                        projectileOwner,
                        owner,
                        neuroPlayer,
                        shot,
                        damage,
                        knockBack,
                        critChance,
                        isEvil,
                        killOnOwnerHit
                    );

                    continue;
                }

                QueueDelayedWeaponShot(
                    source,
                    projectileOwner,
                    owner,
                    shot,
                    damage,
                    knockBack,
                    critChance,
                    isEvil,
                    killOnOwnerHit
                );
            }
        }

        private static void SpawnPreparedWeaponShot(
    IEntitySource source,
    int projectileOwner,
    Player owner,
    NeuroCompanionPlayer neuroPlayer,
    NeuroWeaponShot shot,
    int damage,
    float knockBack,
    int critChance,
    bool isEvil,
    bool killOnOwnerHit
)
        {
            NeuroWeaponProjectileSpawnContext.Begin(
                owner,
                neuroPlayer,
                damage,
                critChance,
                isEvil,
                killOnOwnerHit
            );

            try
            {
                TrySpawnWeaponProjectile(
                    source,
                    shot,
                    damage,
                    knockBack,
                    projectileOwner
                );
            }
            finally
            {
                NeuroWeaponProjectileSpawnContext.End();
            }
        }

        private static void QueueDelayedWeaponShot(
            IEntitySource source,
            int projectileOwner,
            Player owner,
            NeuroWeaponShot shot,
            int damage,
            float knockBack,
            int critChance,
            bool isEvil,
            bool killOnOwnerHit
        )
        {
            PendingWeaponShots.Add(
                new PendingWeaponShot(
                    source,
                    projectileOwner,
                    owner.whoAmI,
                    shot,
                    damage,
                    knockBack,
                    critChance,
                    isEvil,
                    killOnOwnerHit,
                    shot.DelayTicks
                )
            );
        }

        private static void SpawnQueuedWeaponShot(PendingWeaponShot pendingShot)
        {
            if (pendingShot.ProjectileOwner != Main.myPlayer)
            {
                return;
            }

            if (
                pendingShot.OwnerWhoAmI < 0 ||
                pendingShot.OwnerWhoAmI >= Main.maxPlayers
            )
            {
                return;
            }

            Player owner = Main.player[pendingShot.OwnerWhoAmI];

            if (owner == null || !owner.active)
            {
                return;
            }

            NeuroCompanionPlayer neuroPlayer =
                owner.GetModPlayer<NeuroCompanionPlayer>();

            SpawnPreparedWeaponShot(
                pendingShot.Source,
                pendingShot.ProjectileOwner,
                owner,
                neuroPlayer,
                pendingShot.Shot,
                pendingShot.Damage,
                pendingShot.KnockBack,
                pendingShot.CritChance,
                pendingShot.IsEvil,
                pendingShot.KillOnOwnerHit
            );
        }

        private static void ApplyEvilOwnerDamageBehavior(
            Projectile spawnedProjectile,
            int damage,
            bool killOnOwnerHit
        )
        {
            spawnedProjectile.friendly = false;
            spawnedProjectile.hostile = false;

            spawnedProjectile.damage = damage;
            spawnedProjectile.originalDamage = damage;

            EvilNeuroPlayerAttackGlobal evilGlobal =
                spawnedProjectile.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            evilGlobal.CanDamageOwner = true;
            evilGlobal.KillOnOwnerHit = killOnOwnerHit;

            spawnedProjectile.netUpdate = true;
        }

        private static bool TrySpawnWeaponProjectile(
            IEntitySource source,
            NeuroWeaponShot shot,
            int damage,
            float knockBack,
            int projectileOwner
        )
        {
            if (
                shot.ProjectileType <= ProjectileID.None ||
                shot.ProjectileType >= ProjectileLoader.ProjectileCount
            )
            {
                return false;
            }

            if (!TrySpawnProjectile(
                    source,
                    shot.Position,
                    shot.Velocity,
                    shot.ProjectileType,
                    damage,
                    knockBack,
                    projectileOwner,
                    out Projectile spawnedProjectile,
                    shot.Ai0,
                    shot.Ai1
                ))
            {
                return false;
            }

            spawnedProjectile.scale *= shot.Scale;
            spawnedProjectile.netUpdate = true;

            return true;
        }

        private static bool TrySpawnProjectile(
            IEntitySource source,
            Vector2 position,
            Vector2 velocity,
            int projectileType,
            int damage,
            float knockBack,
            int projectileOwner,
            out Projectile spawnedProjectile,
            float ai0 = 0f,
            float ai1 = 0f
        )
        {
            spawnedProjectile = null;

            int projectileIndex = Projectile.NewProjectile(
                source,
                position,
                velocity,
                projectileType,
                damage,
                knockBack,
                projectileOwner,
                ai0,
                ai1
            );

            if (
                projectileIndex < 0 ||
                projectileIndex >= Main.maxProjectiles
            )
            {
                return false;
            }

            spawnedProjectile = Main.projectile[projectileIndex];
            return spawnedProjectile != null;
        }

        private struct PendingWeaponShot
        {
            public IEntitySource Source { get; }
            public int ProjectileOwner { get; }
            public int OwnerWhoAmI { get; }
            public NeuroWeaponShot Shot { get; }
            public int Damage { get; }
            public float KnockBack { get; }
            public int CritChance { get; }
            public bool IsEvil { get; }
            public bool KillOnOwnerHit { get; }
            public int TicksRemaining { get; set; }

            public PendingWeaponShot(
                IEntitySource source,
                int projectileOwner,
                int ownerWhoAmI,
                NeuroWeaponShot shot,
                int damage,
                float knockBack,
                int critChance,
                bool isEvil,
                bool killOnOwnerHit,
                int ticksRemaining
            )
            {
                Source = source;
                ProjectileOwner = projectileOwner;
                OwnerWhoAmI = ownerWhoAmI;
                Shot = shot;
                Damage = damage;
                KnockBack = knockBack;
                CritChance = critChance;
                IsEvil = isEvil;
                KillOnOwnerHit = killOnOwnerHit;
                TicksRemaining = ticksRemaining;
            }
        }
    }
}