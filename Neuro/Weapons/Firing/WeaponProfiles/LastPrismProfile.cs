using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Weapons;
using NeuroCompanion.Projectiles.Globals;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class LastPrismProfile
    {
        private const float PrismSpawnOffset = 0f;
        private const float FallbackShotSpeed = 30f;

        public static int ProjectileType =>
            ModContent.ProjectileType<NeuroLastPrismHoldout>();

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.LastPrism;
        }

        public static NeuroWeaponShot[] CreateShots(
            Player owner,
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
            if (TryRefreshExistingPrism(owner, targetPosition))
            {
                return Array.Empty<NeuroWeaponShot>();
            }

            Vector2 direction =
                targetPosition - basePosition;

            if (direction.LengthSquared() <= 0.01f)
            {
                direction = baseVelocity;
            }

            if (direction.LengthSquared() <= 0.01f)
            {
                direction = -Vector2.UnitY;
            }

            direction.Normalize();

            float speed =
                weapon != null && weapon.shootSpeed > 0f
                    ? weapon.shootSpeed
                    : FallbackShotSpeed;

            Vector2 spawnPosition =
                basePosition + direction * PrismSpawnOffset;

            return new[]
            {
                new NeuroWeaponShot(
                    ProjectileType,
                    spawnPosition,
                    direction * speed,
                    ai0: targetPosition.X,
                    ai1: targetPosition.Y,
                    forceVisible: true,
                    useSkyDrawLayer: true
                )
            };
        }

        public static int GetCooldownTicks(int channelTicks)
        {
            return 5;
        }

        private static bool TryRefreshExistingPrism(
            Player owner,
            Vector2 targetPosition
        )
        {
            if (owner == null)
            {
                return false;
            }

            int prismType =
                ModContent.ProjectileType<NeuroLastPrismHoldout>();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];

                if (
                    projectile != null &&
                    projectile.active &&
                    projectile.owner == owner.whoAmI &&
                    projectile.type == prismType
                )
                {
                    if (projectile.ModProjectile is NeuroLastPrismHoldout prism)
                    {
                        prism.RefreshTarget(targetPosition);
                        ApplyCurrentSpawnContextToExistingPrism(projectile);
                    }

                    return true;
                }
            }

            return false;
        }

        private static void ApplyCurrentSpawnContextToExistingPrism(
            Projectile projectile
        )
        {
            NeuroWeaponProjectileSpawnContext context =
                NeuroWeaponProjectileSpawnContext.Current;

            if (context == null)
            {
                return;
            }

            projectile.damage = context.Damage;
            projectile.originalDamage = context.Damage;
            projectile.CritChance = context.CritChance;

            EvilNeuroPlayerAttackGlobal evilGlobal =
                projectile.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            if (context.IsEvil)
            {
                projectile.friendly = false;
                projectile.hostile = false;

                evilGlobal.CanDamageOwner = true;
                evilGlobal.KillOnOwnerHit = context.KillOnOwnerHit;
            }
            else
            {
                projectile.friendly = true;
                projectile.hostile = false;

                evilGlobal.CanDamageOwner = false;
                evilGlobal.KillOnOwnerHit = false;
            }

            projectile.netUpdate = true;
        }
    }
}