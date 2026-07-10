using Microsoft.Xna.Framework;
using NeuroCompanion.Players;
using NeuroCompanion.Projectiles.Companion;
using Terraria;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles.Weapons.MedusaHead.Holdout
{
    public partial class NeuroMedusaHeadHoldout
    {
        private void UpdateAimAndPosition()
        {
            float aimSpeed = Projectile.velocity.Length();

            if (aimSpeed <= 0.01f)
            {
                aimSpeed = FallbackAimSpeed;
            }

            Vector2 currentDirection =
                SafeNormalize(
                    Projectile.velocity,
                    Vector2.UnitX
                );

            Projectile.Center =
                GetAnchorPosition(currentDirection);

            Vector2 targetDirection =
                SafeNormalize(
                    TargetPosition - Projectile.Center,
                    currentDirection
                );

            Vector2 aimDirection =
                Vector2.Lerp(
                    currentDirection,
                    targetDirection,
                    AimResponsiveness
                );

            aimDirection =
                SafeNormalize(
                    aimDirection,
                    targetDirection
                );

            Projectile.velocity =
                aimDirection * aimSpeed;

            Projectile.Center =
                GetAnchorPosition(aimDirection);

            Projectile.direction =
                aimDirection.X >= 0f ? 1 : -1;

            Projectile.spriteDirection =
                Projectile.direction;

            Projectile.rotation = 0f;
        }

        private Vector2 GetAnchorPosition(
            Vector2 aimDirection
        )
        {
            Projectile companion =
                FindNeuroCompanionProjectile();

            if (companion == null)
            {
                return fallbackAnchorPosition;
            }

            float facingDirection =
                aimDirection.X >= 0f ? 1f : -1f;

            bool usesMk4Sprite =
                IsUsingMk4CompanionSprite();

            float offsetX =
                usesMk4Sprite
                    ? Mk4HoldoutFaceOffsetX
                    : HoldoutFaceOffsetX;

            float offsetY =
                usesMk4Sprite
                    ? Mk4HoldoutFaceOffsetY
                    : HoldoutFaceOffsetY;

            fallbackAnchorPosition =
                companion.Center +
                new Vector2(
                    offsetX * facingDirection,
                    offsetY
                );

            return fallbackAnchorPosition;
        }

        private bool IsUsingMk4CompanionSprite()
        {
            if (
                Projectile.owner < 0 ||
                Projectile.owner >= Main.maxPlayers
            )
            {
                return false;
            }

            Player owner = Main.player[Projectile.owner];

            if (
                owner == null ||
                !owner.active
            )
            {
                return false;
            }

            NeuroCompanionPlayer neuroPlayer =
                owner.GetModPlayer<NeuroCompanionPlayer>();

            return neuroPlayer.NeuroStaffVisualTier == 4;
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

        private static Vector2 SafeNormalize(
            Vector2 value,
            Vector2 fallback
        )
        {
            if (
                value.LengthSquared() <= 0.01f ||
                value.HasNaNs()
            )
            {
                return fallback;
            }

            value.Normalize();
            return value;
        }
    }
}