using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class BloodThornProfile
    {
        private const int ProjectileCount = 3;
        private const int DelayBetweenThornsTicks = 4;

        private const int TileSearchAttempts = 120;
        private const int MinimumSearchRadiusTiles = 2;
        private const int MaximumSearchRadiusTiles = 10;

        private const float AimNoiseHalfWidth = 28f;
        private const float AimNoiseHalfHeight = 24f;

        private const float MinimumShotSpeed = 10f;
        private const float MaximumShotSpeed = 14f;

        private const float ThornScale = 1f;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.SharpTears;
        }

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
            NeuroWeaponShot[] shots = new NeuroWeaponShot[ProjectileCount];

            for (int i = 0; i < shots.Length; i++)
            {
                Vector2 spawnPosition =
                    FindTileSpawnPosition(
                        basePosition,
                        targetPosition
                    );

                Vector2 aimPoint =
                    targetPosition + RollAimNoise();

                Vector2 velocity =
                    SkySpawnProfileHelper.AimAt(
                        spawnPosition,
                        aimPoint,
                        Main.rand.NextFloat(
                            MinimumShotSpeed,
                            MaximumShotSpeed
                        )
                    );

                shots[i] = new NeuroWeaponShot(
                    ProjectileID.SharpTears,
                    spawnPosition,
                    velocity,
                    ai0: 0f,
                    ai1: ThornScale,
                    delayTicks: i * DelayBetweenThornsTicks
                );
            }

            return shots;
        }

        public static int GetCooldownTicks(Item weapon)
        {
            return SkySpawnProfileHelper.GetFullUseCooldownTicks(weapon);
        }

        private static Vector2 FindTileSpawnPosition(
            Vector2 basePosition,
            Vector2 targetPosition
        )
        {
            if (TryFindSolidTileNearTarget(targetPosition, out Vector2 spawnPosition))
            {
                return spawnPosition;
            }

            if (TryFindGroundBelowTarget(targetPosition, out spawnPosition))
            {
                return spawnPosition;
            }

            // No tile or platform found: fall back to Neuro
            return basePosition;
        }

        private static bool TryFindSolidTileNearTarget(
            Vector2 targetPosition,
            out Vector2 spawnPosition
        )
        {
            Point targetTile = targetPosition.ToTileCoordinates();

            for (int i = 0; i < TileSearchAttempts; i++)
            {
                int offsetX = Main.rand.Next(
                    -MaximumSearchRadiusTiles,
                    MaximumSearchRadiusTiles + 1
                );

                int offsetY = Main.rand.Next(
                    -MaximumSearchRadiusTiles,
                    MaximumSearchRadiusTiles + 1
                );

                if (
                    System.Math.Abs(offsetX) < MinimumSearchRadiusTiles &&
                    System.Math.Abs(offsetY) < MinimumSearchRadiusTiles
                )
                {
                    continue;
                }

                int tileX = targetTile.X + offsetX;
                int tileY = targetTile.Y + offsetY;

                if (!IsUsableSolidTile(tileX, tileY))
                {
                    continue;
                }

                spawnPosition = GetTileCenter(tileX, tileY);
                return true;
            }

            spawnPosition = Vector2.Zero;
            return false;
        }

        private static bool TryFindGroundBelowTarget(
            Vector2 targetPosition,
            out Vector2 spawnPosition
        )
        {
            Point targetTile = targetPosition.ToTileCoordinates();

            for (int y = targetTile.Y; y < targetTile.Y + MaximumSearchRadiusTiles; y++)
            {
                if (!IsUsableSolidTile(targetTile.X, y))
                {
                    continue;
                }

                spawnPosition = GetTileCenter(targetTile.X, y);
                return true;
            }

            spawnPosition = Vector2.Zero;
            return false;
        }

        private static bool IsUsableSolidTile(int tileX, int tileY)
        {
            if (!WorldGen.InWorld(tileX, tileY, 10))
            {
                return false;
            }

            Tile tile = Main.tile[tileX, tileY];

            if (tile == null || !tile.HasUnactuatedTile)
            {
                return false;
            }

            if (WorldGen.SolidTile(tileX, tileY))
            {
                return true;
            }

            return TileID.Sets.Platforms[tile.TileType];
        }

        private static Vector2 GetTileCenter(int tileX, int tileY)
        {
            return new Vector2(
                tileX * 16f + 8f,
                tileY * 16f + 8f
            );
        }

        private static Vector2 RollAimNoise()
        {
            return new Vector2(
                Main.rand.NextFloat(-AimNoiseHalfWidth, AimNoiseHalfWidth),
                Main.rand.NextFloat(-AimNoiseHalfHeight, AimNoiseHalfHeight)
            );
        }
    }
}