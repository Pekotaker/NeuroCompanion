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
            Vector2 velocity
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
            Vector2 velocity
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
            bool isEvil,
            bool killOnOwnerHit
        )
        {
            if (
                owner == null ||
                neuroPlayer == null ||
                weapon == null ||
                weapon.IsAir ||
                weapon.shoot <= ProjectileID.None
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
                TrySpawnProjectile(
                    source,
                    position,
                    velocity,
                    weapon.shoot,
                    damage,
                    knockBack,
                    projectileOwner,
                    out _
                );
            }
            finally
            {
                NeuroWeaponProjectileSpawnContext.End();
            }
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
            out Projectile spawnedProjectile
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
                projectileOwner
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