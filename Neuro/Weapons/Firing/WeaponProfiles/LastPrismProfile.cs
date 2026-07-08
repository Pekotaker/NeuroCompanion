using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Weapons;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class LastPrismProfile
    {
        private const float PrismSpawnOffset = 56f;
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
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
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
            return NeuroLastPrismHoldout.DurationTicks + 30;
        }
    }
}