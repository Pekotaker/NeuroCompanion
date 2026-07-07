using System;

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
        public static void SpawnCompanionWeaponProjectile(
            IEntitySource source,
            int projectileOwner,
            Player owner,
            NeuroCompanionPlayer neuroPlayer,
            Item weapon,
            Vector2 position,
            Vector2 velocity,
            Vector2 targetPosition,
            Action<PendingNeuroWeaponShot> queueDelayedShot = null
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
                killOnOwnerHit: false,
                queueDelayedShot
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
            Vector2 targetPosition,
            Action<PendingNeuroWeaponShot> queueDelayedShot = null
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
                killOnOwnerHit: false,
                queueDelayedShot
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

        public static void SpawnPendingWeaponShot(
            PendingNeuroWeaponShot pendingShot
        )
        {
            if (
                pendingShot.Owner == null ||
                pendingShot.NeuroPlayer == null ||
                pendingShot.Shot.ProjectileType <= ProjectileID.None
            )
            {
                return;
            }

            NeuroWeaponProjectileSpawnContext.Begin(
                pendingShot.Owner,
                pendingShot.NeuroPlayer,
                pendingShot.Damage,
                pendingShot.CritChance,
                pendingShot.IsEvil,
                pendingShot.KillOnOwnerHit
            );

            try
            {
                TrySpawnWeaponProjectile(
                    pendingShot.Source,
                    pendingShot.Shot,
                    pendingShot.Damage,
                    pendingShot.KnockBack,
                    pendingShot.ProjectileOwner
                );
            }
            finally
            {
                NeuroWeaponProjectileSpawnContext.End();
            }
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
            bool killOnOwnerHit,
            Action<PendingNeuroWeaponShot> queueDelayedShot
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
                PendingNeuroWeaponShot pendingShot =
                    new PendingNeuroWeaponShot(
                        source,
                        projectileOwner,
                        owner,
                        neuroPlayer,
                        shots[i],
                        damage,
                        knockBack,
                        critChance,
                        isEvil,
                        killOnOwnerHit,
                        Math.Max(0, shots[i].DelayTicks)
                    );

                if (
                    pendingShot.RemainingTicks > 0 &&
                    queueDelayedShot != null
                )
                {
                    queueDelayedShot(pendingShot);
                    continue;
                }

                SpawnPendingWeaponShot(pendingShot);
            }
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

            if (shot.ForceVisible)
            {
                spawnedProjectile.alpha = 0;
                spawnedProjectile.hide = false;
            }

            spawnedProjectile.netUpdate = true;

            return true;
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
    }
}