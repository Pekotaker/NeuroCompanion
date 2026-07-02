using Microsoft.Xna.Framework;
using NeuroCompanion.Neuro.Weapons;
using NeuroCompanion.Neuro.Effects;
using NeuroCompanion.Players;
using Terraria;
using Terraria.Audio;

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

            Vector2 shotDirection =
                NeuroShotDirectionHelper.NormalizeForOwnerFacing(
                    rawDirection,
                    owner,
                    MinimumShotDirectionLengthSquared
                );

            float shotSpeed = GetWeaponShootSpeed(weapon);
            Vector2 shotVelocity = shotDirection * shotSpeed;
            Vector2 shotPosition = Projectile.Center + shotDirection * ShotSpawnOffset;

            PlayWeaponSound(weapon);
            NeuroProjectileSpawner.SpawnCompanionWeaponProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.owner,
                owner,
                owner.GetModPlayer<NeuroCompanionPlayer>(),
                weapon,
                shotPosition,
                shotVelocity
            );
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

            NeuroProjectileSpawner.SpawnCompanionWeaponProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.owner,
                owner,
                owner.GetModPlayer<NeuroCompanionPlayer>(),
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
                NeuroProjectileSpawner.SpawnEvilWeaponProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.owner,
                    owner,
                    neuroPlayer,
                    weapon,
                    owner.Center,
                    Vector2.Zero
                );

                return;
            }

            Vector2 shotDirection =
                NeuroShotDirectionHelper.NormalizeFromTo(
                    Projectile.Center,
                    owner.Center,
                    Vector2.UnitY,
                    MinimumShotDirectionLengthSquared
                );

            float shotSpeed = GetWeaponShootSpeed(weapon);
            Vector2 shotPosition = Projectile.Center + shotDirection * ShotSpawnOffset;
            Vector2 shotVelocity = shotDirection * shotSpeed;

            PlayWeaponSound(weapon);

            NeuroProjectileSpawner.SpawnEvilWeaponProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.owner,
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

            Vector2 shotDirection =
                NeuroShotDirectionHelper.NormalizeFromTo(
                    Projectile.Center,
                    owner.Center,
                    Vector2.UnitY,
                    MinimumShotDirectionLengthSquared
                );

            Vector2 shotPosition = Projectile.Center + shotDirection * ShotSpawnOffset;
            Vector2 shotVelocity = shotDirection * EvilProjectileSpeed;

            int damage = EvilNeuroDamageScaler.GetFallbackBoltDamage(neuroPlayer);

            NeuroProjectileSpawner.SpawnFallbackEvilBolt(
                Projectile.GetSource_FromThis(),
                Projectile.owner,
                shotPosition,
                shotVelocity,
                damage,
                EvilProjectileKnockBack
            );
        }
    }
}