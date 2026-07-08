using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Companion;

using NeuroCompanion.Projectiles.Weapons.LastPrism.Beam;

namespace NeuroCompanion.Projectiles.Weapons.LastPrism.Holdout
{
    public partial class NeuroLastPrismHoldout
    {
        private void UpdateAim()
        {
            float speed = Projectile.velocity.Length();

            if (speed <= 0.01f)
            {
                speed = 30f;
            }

            Vector2 currentAim =
                SafeNormalize(Projectile.velocity, Vector2.UnitX);

            Vector2 targetAim =
                SafeNormalize(TargetPosition - Projectile.Center, currentAim);

            Vector2 aim =
                Vector2.Lerp(
                    currentAim,
                    targetAim,
                    AimResponsiveness
                );

            aim = SafeNormalize(aim, targetAim) * speed;

            if ((aim - Projectile.velocity).LengthSquared() > 0.01f)
            {
                Projectile.netUpdate = true;
            }

            Projectile.velocity = aim;
        }

        private void PlaySounds()
        {
            if (Projectile.soundDelay > 0)
            {
                return;
            }

            Projectile.soundDelay = SoundInterval;

            if (FrameCounter > 1f)
            {
                SoundEngine.PlaySound(SoundID.Item15, Projectile.Center);
            }
        }

        private void FireBeams()
        {
            Vector2 beamVelocity =
                SafeNormalize(Projectile.velocity, -Vector2.UnitY);

            int damage = Projectile.damage;
            float knockBack = Projectile.knockBack;

            for (int i = 0; i < NumBeams; i++)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    beamVelocity,
                    ModContent.ProjectileType<NeuroLastPrismBeam>(),
                    damage,
                    knockBack,
                    Projectile.owner,
                    i,
                    Projectile.whoAmI
                );
            }

            Projectile.netUpdate = true;
        }

        private Vector2 GetAnchorPosition(Vector2 aimDirection)
        {
            Projectile companion = FindNeuroCompanionProjectile();

            if (companion != null)
            {
                fallbackAnchorPosition =
                    companion.Center + aimDirection * PrismDistanceFromNeuro;
            }

            return fallbackAnchorPosition;
        }

        private Projectile FindNeuroCompanionProjectile()
        {
            int companionType =
                ModContent.ProjectileType<NeuroCompanionProjectile>();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];

                if (
                    projectile != null &&
                    projectile.active &&
                    projectile.owner == Projectile.owner &&
                    projectile.type == companionType
                )
                {
                    return projectile;
                }
            }

            return null;
        }
    }
}