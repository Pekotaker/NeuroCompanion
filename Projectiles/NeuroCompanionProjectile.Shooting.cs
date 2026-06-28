using Microsoft.Xna.Framework;
using NeuroCompanion.Neuro;
using NeuroCompanion.Players;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles
{
    public partial class NeuroCompanionProjectile
    {
        private void ShootWeaponAtTargetWhenReady(Player owner, NPC target)
        {
            NeuroCompanionPlayer neuroPlayer =
                owner.GetModPlayer<NeuroCompanionPlayer>();

            if (!neuroPlayer.HasNeuroWeapon())
            {
                return;
            }

            Item weapon = neuroPlayer.NeuroWeapon;

            NeuroWeaponClassification classification =
                NeuroWeaponClassifier.Classify(weapon);

            if (!classification.IsAccepted)
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

            NeuroCompanionPlayer neuroPlayer =
                owner.GetModPlayer<NeuroCompanionPlayer>();

            if (!neuroPlayer.HasNeuroWeapon())
            {
                return;
            }

            Item weapon = neuroPlayer.NeuroWeapon;

            NeuroWeaponClassification classification =
                NeuroWeaponClassifier.Classify(weapon);

            if (!classification.IsAccepted)
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
            NeuroCompanionPlayer neuroPlayer =
                owner.GetModPlayer<NeuroCompanionPlayer>();

            if (!neuroPlayer.HasNeuroWeapon())
            {
                return;
            }

            Item weapon = neuroPlayer.NeuroWeapon;

            NeuroWeaponClassification classification =
                NeuroWeaponClassifier.Classify(weapon);

            if (!classification.IsAccepted)
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

            NeuroCompanionPlayer neuroPlayer =
                owner.GetModPlayer<NeuroCompanionPlayer>();

            if (!neuroPlayer.HasNeuroWeapon())
            {
                return;
            }

            Item weapon = neuroPlayer.NeuroWeapon;

            NeuroWeaponClassification classification =
                NeuroWeaponClassifier.Classify(weapon);

            if (!classification.IsAccepted)
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

        private static int ClampInt(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        private void ShootWeaponAtWorldPosition(Player owner, Vector2 worldPosition)
        {
            if (Projectile.owner != Main.myPlayer)
            {
                return;
            }

            NeuroCompanionPlayer neuroPlayer =
                owner.GetModPlayer<NeuroCompanionPlayer>();

            if (!neuroPlayer.HasNeuroWeapon())
            {
                return;
            }

            Item weapon = neuroPlayer.NeuroWeapon;

            NeuroWeaponClassification classification =
                NeuroWeaponClassifier.Classify(weapon);

            if (!classification.IsAccepted)
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
    }
}