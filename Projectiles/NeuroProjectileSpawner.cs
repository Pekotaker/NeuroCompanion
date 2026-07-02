using Microsoft.Xna.Framework;
using NeuroCompanion.Neuro.Weapons;
using NeuroCompanion.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles
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

            if (!TrySpawnProjectile(
                    source,
                    position,
                    velocity,
                    weapon.shoot,
                    damage,
                    knockBack,
                    projectileOwner,
                    out Projectile spawnedProjectile
                ))
            {
                return;
            }

            spawnedProjectile.DamageType = DamageClass.Magic;
            spawnedProjectile.originalDamage = damage;

            ApplyNeuroStaffProjectileBehavior(neuroPlayer, spawnedProjectile);

            spawnedProjectile.CritChance =
                NeuroDamageService.GetNeuroWeaponCritChance(
                    owner,
                    weapon,
                    neuroPlayer.NeuroStaffPrefix
                );

            spawnedProjectile.netUpdate = true;
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

            if (!TrySpawnProjectile(
                    source,
                    position,
                    velocity,
                    weapon.shoot,
                    damage,
                    knockBack,
                    projectileOwner,
                    out Projectile spawnedProjectile
                ))
            {
                return;
            }

            spawnedProjectile.friendly = false;
            spawnedProjectile.hostile = false;

            spawnedProjectile.damage = damage;
            spawnedProjectile.originalDamage = damage;
            spawnedProjectile.DamageType = DamageClass.Magic;

            EvilNeuroPlayerAttackGlobal evilGlobal =
                spawnedProjectile.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            evilGlobal.CanDamageOwner = true;
            evilGlobal.KillOnOwnerHit = false;

            spawnedProjectile.netUpdate = true;
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

            spawnedProjectile.friendly = false;
            spawnedProjectile.hostile = false;

            spawnedProjectile.damage = damage;
            spawnedProjectile.originalDamage = damage;

            EvilNeuroPlayerAttackGlobal evilGlobal =
                spawnedProjectile.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            evilGlobal.CanDamageOwner = true;
            evilGlobal.KillOnOwnerHit = true;

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

        private static void ApplyNeuroStaffProjectileBehavior(
            NeuroCompanionPlayer neuroPlayer,
            Projectile spawnedProjectile
        )
        {
            if (neuroPlayer == null || spawnedProjectile == null)
            {
                return;
            }

            if (!neuroPlayer.NeuroStaffCanDetectThroughBlocks)
            {
                return;
            }

            spawnedProjectile.tileCollide = false;

            spawnedProjectile
                .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                .IgnoreTilesForNeuroMk4 = true;
        }
    }
}