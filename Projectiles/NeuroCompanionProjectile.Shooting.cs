using Microsoft.Xna.Framework;
using NeuroCompanion.Neuro;
using NeuroCompanion.Players;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles
{
    public partial class NeuroCompanionProjectile
    {
        private void ShootWeaponAtTargetWhenReady(Player owner, NPC target)
        {
            if (!NeuroWeaponFireHelper.TryGetAcceptedWeapon(
                    owner,
                    out NeuroCompanionPlayer neuroPlayer,
                    out Item weapon,
                    out NeuroWeaponClassification classification
                ))
            {
                return;
            }

            ShootTimer++;

            int cooldownTicks = GetWeaponCooldownTicks(
                owner,
                weapon,
                classification
            );

            if (ShootTimer < cooldownTicks)
            {
                return;
            }

            ShootWeaponAtTarget(owner, target);
        }

        private void ShootWeaponAtTarget(Player owner, NPC target)
        {
            if (
                !CanDetectTargetsThroughBlocks(owner) &&
                !CanShootTarget(target, owner)
            )
            {
                return;
            }

            if (!NeuroWeaponFireHelper.TryGetAcceptedWeapon(
                    owner,
                    out _,
                    out Item weapon,
                    out NeuroWeaponClassification classification
                ))
            {
                return;
            }

            if (classification.Kind == NeuroWeaponKind.TargetedArea)
            {
                ShootWeaponAtWorldPosition(owner, target.Center);
                return;
            }

            Vector2 shotDirection = target.Center - Projectile.Center;

            ShootWeaponInDirection(owner, shotDirection);
        }

        private void ShootWeaponTowardCursor(Player owner)
        {
            if (!NeuroWeaponFireHelper.TryGetAcceptedWeapon(
                    owner,
                    out _,
                    out _,
                    out NeuroWeaponClassification classification
                ))
            {
                return;
            }

            if (classification.Kind == NeuroWeaponKind.TargetedArea)
            {
                ShootWeaponAtWorldPosition(owner, Main.MouseWorld);
                return;
            }

            Vector2 shotDirection = Main.MouseWorld - Projectile.Center;

            ShootWeaponInDirection(owner, shotDirection);
        }

        private void ShootWeaponInDirection(Player owner, Vector2 rawDirection)
        {
            if (Projectile.owner != Main.myPlayer)
            {
                return;
            }

            if (!NeuroWeaponFireHelper.TryGetAcceptedWeapon(
                    owner,
                    out _,
                    out Item weapon,
                    out _
                ))
            {
                return;
            }

            ShootTimer = 0f;

            Vector2 shotDirection = NormalizeShotDirection(rawDirection, owner);

            float shotSpeed = GetWeaponShootSpeed(weapon);
            Vector2 shotVelocity = shotDirection * shotSpeed;
            Vector2 shotPosition = Projectile.Center + shotDirection * ShotSpawnOffset;

            PlayWeaponSound(weapon);
            SpawnWeaponProjectile(owner, weapon, shotPosition, shotVelocity);
        }

        private Vector2 NormalizeShotDirection(
            Vector2 rawDirection,
            Player owner
        )
        {
            if (rawDirection.LengthSquared() < MinimumShotDirectionLengthSquared)
            {
                rawDirection = Vector2.UnitX * owner.direction;
            }

            return rawDirection.SafeNormalize(Vector2.UnitX * owner.direction);
        }

        private static float GetWeaponShootSpeed(Item weapon)
        {
            if (weapon.shootSpeed > 0f)
            {
                return weapon.shootSpeed;
            }

            return ShotSpeed;
        }

        private void PlayWeaponSound(Item weapon)
        {
            if (!weapon.UseSound.HasValue)
            {
                return;
            }

            SoundEngine.PlaySound(
                weapon.UseSound.Value,
                Projectile.Center
            );
        }

        private void SpawnWeaponProjectile(
            Player owner,
            Item weapon,
            Vector2 shotPosition,
            Vector2 shotVelocity
        )
        {
            NeuroCompanionPlayer neuroPlayer =
                owner.GetModPlayer<NeuroCompanionPlayer>();

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

            int projectileIndex = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                shotPosition,
                shotVelocity,
                weapon.shoot,
                damage,
                knockBack,
                Projectile.owner
            );

            if (
                projectileIndex < 0 ||
                projectileIndex >= Main.maxProjectiles
            )
            {
                return;
            }

            Projectile spawnedProjectile = Main.projectile[projectileIndex];

            spawnedProjectile.DamageType = DamageClass.Magic;
            spawnedProjectile.originalDamage = damage;

            ApplyNeuroStaffProjectileBehavior(neuroPlayer, spawnedProjectile);

            spawnedProjectile.CritChance = NeuroDamageService.GetNeuroWeaponCritChance(
                owner,
                weapon,
                neuroPlayer.NeuroStaffPrefix
            );
            spawnedProjectile.netUpdate = true;
        }
        private static int GetWeaponCooldownTicks(
            Player owner,
            Item weapon,
            NeuroWeaponClassification classification
        )
        {
            NeuroCompanionPlayer neuroPlayer =
                owner.GetModPlayer<NeuroCompanionPlayer>();

            if (!classification.IsAccepted)
            {
                return NeuroCompanionPlayer.DefaultNeuroStaffShootCooldownTicks;
            }

            return NeuroDamageService.GetEffectiveNeuroShootCooldownTicks(
                weapon,
                neuroPlayer.NeuroStaffShootCooldownTicks,
                neuroPlayer.NeuroStaffPrefix
            );
        }

        private void ShootWeaponAtWorldPosition(Player owner, Vector2 worldPosition)
        {
            if (Projectile.owner != Main.myPlayer)
            {
                return;
            }

            if (!NeuroWeaponFireHelper.TryGetAcceptedWeapon(
                    owner,
                    out _,
                    out Item weapon,
                    out _
                ))
            {
                return;
            }

            ShootTimer = 0f;

            PlayWeaponSound(weapon);

            SpawnWeaponProjectile(
                owner,
                weapon,
                worldPosition,
                Vector2.Zero
            );
        }

        private void ShootEvilProjectileAtOwner(Player owner)
        {
            if (Projectile.owner != Main.myPlayer)
            {
                return;
            }

            NeuroCompanionPlayer neuroPlayer =
                owner.GetModPlayer<NeuroCompanionPlayer>();

            if (NeuroWeaponFireHelper.TryGetAcceptedWeapon(
                    owner,
                    out _,
                    out Item weapon,
                    out NeuroWeaponClassification classification
                ))
            {
                ShootEquippedWeaponAtOwner(
                    owner,
                    neuroPlayer,
                    weapon,
                    classification
                );

                return;
            }

            ShootFallbackEvilBoltAtOwner(owner, neuroPlayer);
        }

        private void ShootEquippedWeaponAtOwner(
            Player owner,
            NeuroCompanionPlayer neuroPlayer,
            Item weapon,
            NeuroWeaponClassification classification
        )
        {
            ShootTimer = 0f;

            if (classification.Kind == NeuroWeaponKind.TargetedArea)
            {
                SpawnEvilWeaponProjectile(
                    owner,
                    neuroPlayer,
                    weapon,
                    owner.Center,
                    Vector2.Zero
                );

                return;
            }

            Vector2 shotDirection = owner.Center - Projectile.Center;

            if (shotDirection.LengthSquared() < MinimumShotDirectionLengthSquared)
            {
                shotDirection = Vector2.UnitY;
            }

            shotDirection = shotDirection.SafeNormalize(Vector2.UnitY);

            float shotSpeed = GetWeaponShootSpeed(weapon);
            Vector2 shotPosition = Projectile.Center + shotDirection * ShotSpawnOffset;
            Vector2 shotVelocity = shotDirection * shotSpeed;

            PlayWeaponSound(weapon);

            SpawnEvilWeaponProjectile(
                owner,
                neuroPlayer,
                weapon,
                shotPosition,
                shotVelocity
            );
        }
        private void ShootFallbackEvilBoltAtOwner(
            Player owner,
            NeuroCompanionPlayer neuroPlayer
        )
        {
            ShootTimer = 0f;

            Vector2 shotDirection = owner.Center - Projectile.Center;

            if (shotDirection.LengthSquared() < MinimumShotDirectionLengthSquared)
            {
                shotDirection = Vector2.UnitY;
            }

            shotDirection = shotDirection.SafeNormalize(Vector2.UnitY);

            Vector2 shotPosition = Projectile.Center + shotDirection * ShotSpawnOffset;
            Vector2 shotVelocity = shotDirection * EvilProjectileSpeed;

            int damage = GetFallbackEvilBoltDamage(neuroPlayer);

            int projectileIndex = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                shotPosition,
                shotVelocity,
                ModContent.ProjectileType<EvilNeuroBoltProjectile>(),
                damage,
                EvilProjectileKnockBack,
                Projectile.owner
            );

            if (
                projectileIndex < 0 ||
                projectileIndex >= Main.maxProjectiles
            )
            {
                return;
            }

            Projectile spawnedProjectile = Main.projectile[projectileIndex];

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


        private void SpawnEvilWeaponProjectile(
            Player owner,
            NeuroCompanionPlayer neuroPlayer,
            Item weapon,
            Vector2 shotPosition,
            Vector2 shotVelocity
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

            int projectileIndex = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                shotPosition,
                shotVelocity,
                weapon.shoot,
                damage,
                knockBack,
                Projectile.owner
            );

            if (
                projectileIndex < 0 ||
                projectileIndex >= Main.maxProjectiles
            )
            {
                return;
            }

            Projectile spawnedProjectile = Main.projectile[projectileIndex];

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

        private static int GetFallbackEvilBoltDamage(
    NeuroCompanionPlayer neuroPlayer
)
        {
            int baseDamage = GetFallbackEvilBoltBaseDamage(neuroPlayer);
            float difficultyMultiplier = GetWorldDifficultyDamageMultiplier();

            int finalDamage = (int)System.Math.Round(
                baseDamage * difficultyMultiplier,
                System.MidpointRounding.AwayFromZero
            );

            return finalDamage < 1 ? 1 : finalDamage;
        }

        private static int GetFallbackEvilBoltBaseDamage(
            NeuroCompanionPlayer neuroPlayer
        )
        {
            if (neuroPlayer.NeuroStaffCanDetectThroughBlocks)
            {
                return 400;
            }

            if (neuroPlayer.NeuroStaffShootCooldownTicks <= 1)
            {
                return 200;
            }

            if (neuroPlayer.NeuroStaffShootCooldownTicks <= 30)
            {
                return 150;
            }

            return 100;
        }

        private static float GetWorldDifficultyDamageMultiplier()
        {
            int difficultyLevel = GetWorldDifficultyLevel();

            return 1f + difficultyLevel * 0.33f;
        }

        private static int GetWorldDifficultyLevel()
        {
            if (Main.masterMode && (Main.getGoodWorld || Main.zenithWorld))
            {
                return 3;
            }

            if (Main.masterMode)
            {
                return 2;
            }

            if (Main.expertMode)
            {
                return 1;
            }

            return 0;
        }
    }
}