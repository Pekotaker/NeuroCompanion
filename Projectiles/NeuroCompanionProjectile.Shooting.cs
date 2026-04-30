using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles
{
    public partial class NeuroCompanionProjectile
    {
        private void ShootTyphoonAtTargetWhenReady(Player owner, NPC target)
        {
            ShootTimer++;

            if (ShootTimer < ShootCooldownTicks)
            {
                return;
            }

            ShootTyphoonAtTarget(owner, target);
        }

        private void ShootTyphoonAtTarget(Player owner, NPC target)
        {
            if (!CanShootTarget(target, owner))
            {
                return;
            }

            Vector2 shotDirection = target.Center - Projectile.Center;

            ShootTyphoonInDirection(owner, shotDirection);
        }

        private void ShootTyphoonTowardCursor(Player owner)
        {
            Vector2 shotDirection = Main.MouseWorld - Projectile.Center;

            ShootTyphoonInDirection(owner, shotDirection);
        }

        private void ShootTyphoonInDirection(Player owner, Vector2 rawDirection)
        {
            if (Projectile.owner != Main.myPlayer)
            {
                return;
            }

            ShootTimer = 0f;

            Vector2 shotDirection = NormalizeShotDirection(rawDirection, owner);
            Vector2 shotVelocity = shotDirection * ShotSpeed;
            Vector2 shotPosition = Projectile.Center + shotDirection * ShotSpawnOffset;

            PlayTyphoonSound();
            SpawnTyphoonProjectile(owner, shotPosition, shotVelocity);
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

        private void PlayTyphoonSound()
        {
            SoundEngine.PlaySound(
                SoundID.Item84 with
                {
                    Volume = TyphoonSoundVolume,
                    PitchVariance = TyphoonSoundPitchVariance
                },
                Projectile.Center
            );
        }

        private void SpawnTyphoonProjectile(
            Player owner,
            Vector2 shotPosition,
            Vector2 shotVelocity
        )
        {
            int typhoonDamage = GetRazorbladeTyphoonSummonDamage(owner);

            int typhoonProjectileIndex = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                shotPosition,
                shotVelocity,
                ProjectileID.Typhoon,
                typhoonDamage,
                Projectile.knockBack,
                Projectile.owner
            );

            if (
                typhoonProjectileIndex < 0 ||
                typhoonProjectileIndex >= Main.maxProjectiles
            )
            {
                return;
            }

            Projectile typhoonProjectile = Main.projectile[typhoonProjectileIndex];

            typhoonProjectile.DamageType = DamageClass.Summon;
            typhoonProjectile.originalDamage = typhoonDamage;
            typhoonProjectile.netUpdate = true;
        }

        private static int GetRazorbladeTyphoonSummonDamage(Player owner)
        {
            int vanillaTyphoonDamage =
                ContentSamples.ItemsByType[ItemID.RazorbladeTyphoon].damage;

            int scaledDamage = (int)owner
                .GetTotalDamage(DamageClass.Summon)
                .ApplyTo(vanillaTyphoonDamage);

            return scaledDamage < 1 ? 1 : scaledDamage;
        }
    }
}