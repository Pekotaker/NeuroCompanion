using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Companion;
using NeuroCompanion.Projectiles.Weapons.SpiritFlame;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class SpiritFlameProfile
    {
        private const float NeuroShotSpawnOffset = NeuroCompanionProjectile.ShotSpawnOffset;

        private const int SpawnAttemptCount = 9;

        private const int SpawnCollisionWidth = 16;
        private const int SpawnCollisionHeight = 16;

        private const float SpawnRingRadius = 96f;
        private const float SpawnRingHalfThickness = 4f;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.SpiritFlame;
        }

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity
        )
        {
            Vector2 neuroCenter =
                GetNeuroCenterFromShotPosition(
                    basePosition,
                    baseVelocity
                );

            Vector2 spawnPosition =
                FindSpawnPositionAroundNeuro(neuroCenter);

            bool ownerAttackMode =
                NeuroWeaponProjectileSpawnContext.Current != null &&
                NeuroWeaponProjectileSpawnContext.Current.IsEvil;

            return new[]
            {
                new NeuroWeaponShot(
                    ModContent.ProjectileType<NeuroSpiritFlameProjectile>(),
                    spawnPosition,
                    Vector2.Zero,
                    ai0: spawnPosition.X,
                    ai1: spawnPosition.Y,
                    ai2: ownerAttackMode ? 1f : 0f
                )
            };
        }

        public static int GetCooldownTicks(Item weapon)
        {
            if (weapon == null || weapon.IsAir)
            {
                return 1;
            }

            return weapon.useTime;
        }

        private static Vector2 GetNeuroCenterFromShotPosition(
            Vector2 basePosition,
            Vector2 baseVelocity
        )
        {
            if (
                baseVelocity.LengthSquared() <= 0.01f ||
                baseVelocity.HasNaNs()
            )
            {
                return basePosition;
            }

            Vector2 direction = baseVelocity;
            direction.Normalize();

            return basePosition - direction * NeuroShotSpawnOffset;
        }

        private static Vector2 FindSpawnPositionAroundNeuro(
            Vector2 neuroCenter
        )
        {
            Vector2 fallbackPosition =
                neuroCenter + CreateRandomSpawnOffset();

            for (int i = 0; i < SpawnAttemptCount; i++)
            {
                Vector2 candidatePosition =
                    neuroCenter + CreateRandomSpawnOffset();

                if (!IsBlocked(candidatePosition))
                {
                    return candidatePosition;
                }

                fallbackPosition = candidatePosition;
            }

            return fallbackPosition;
        }

        private static Vector2 CreateRandomSpawnOffset()
        {
            float radius =
                SpawnRingRadius +
                Main.rand.NextFloat(
                    -SpawnRingHalfThickness,
                    SpawnRingHalfThickness
                );

            return Main.rand.NextVector2CircularEdge(1f, 1f) * radius;
        }

        private static bool IsBlocked(Vector2 position)
        {
            Vector2 topLeft =
                position -
                new Vector2(
                    SpawnCollisionWidth * 0.5f,
                    SpawnCollisionHeight * 0.5f
                );

            return Collision.SolidCollision(
                topLeft,
                SpawnCollisionWidth,
                SpawnCollisionHeight
            );
        }
    }
}