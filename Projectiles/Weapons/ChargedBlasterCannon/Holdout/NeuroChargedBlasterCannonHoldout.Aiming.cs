using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Companion;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Holdout
{
    public partial class NeuroChargedBlasterCannonHoldout
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

        private Vector2 GetAnchorPosition(Vector2 aimDirection)
        {
            Projectile companion = FindNeuroCompanionProjectile();

            if (companion != null)
            {
                fallbackAnchorPosition =
                    companion.Center +
                    aimDirection * HoldoutDistanceFromNeuro;
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

        private void UpdateAnimation()
        {
            Projectile.frameCounter++;

            int framesPerAnimationUpdate =
                ChargeTicks >= BeamStartTicks ? 2 :
                ChargeTicks >= PhaseTwoStartTicks ? 3 :
                4;

            if (Projectile.frameCounter < framesPerAnimationUpdate)
            {
                return;
            }

            Projectile.frameCounter = 0;

            int frameCount = Main.projFrames[Type];

            if (frameCount <= 0)
            {
                return;
            }

            Projectile.frame++;

            if (Projectile.frame >= frameCount)
            {
                Projectile.frame = 0;
            }
        }
    }
}

