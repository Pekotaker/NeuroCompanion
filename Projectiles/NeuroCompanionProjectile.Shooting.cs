using Microsoft.Xna.Framework;
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
            ShootTimer++;

            if (ShootTimer < ShootCooldownTicks)
            {
                return;
            }

            ShootWeaponAtTarget(owner, target);
        }

        private void ShootWeaponAtTarget(Player owner, NPC target)
        {
            if (!CanShootTarget(target, owner))
            {
                return;
            }

            Vector2 shotDirection = target.Center - Projectile.Center;

            ShootWeaponInDirection(owner, shotDirection);
        }

        private void ShootWeaponTowardCursor(Player owner)
        {
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

            if (!Neuro.NeuroWeaponValidator.IsValidNeuroWeapon(weapon, out _))
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
            int damage = GetMagicWeaponDamage(owner, weapon);
            float knockBack = GetMagicWeaponKnockBack(owner, weapon);

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
            spawnedProjectile.netUpdate = true;
        }

        private static int GetMagicWeaponDamage(Player owner, Item weapon)
        {
            int scaledDamage = (int)owner
                .GetTotalDamage(DamageClass.Magic)
                .ApplyTo(weapon.damage);

            return scaledDamage < 1 ? 1 : scaledDamage;
        }

        private static float GetMagicWeaponKnockBack(Player owner, Item weapon)
        {
            return owner
                .GetTotalKnockback(DamageClass.Magic)
                .ApplyTo(weapon.knockBack);
        }
    }
}