using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

using NeuroCompanion.Neuro.Weapons.Firing;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class MeteorStaffProfile
    {
        private const float HorizontalSpawnNoise = 40f;

        private const float MinimumMeteorVisualScale = 0.68f;
        private const float MaximumMeteorVisualScale = 1.12f;

        private static readonly int[] MeteorProjectileTypes =
        {
            ProjectileID.Meteor1,
            ProjectileID.Meteor2,
            ProjectileID.Meteor3
        };

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.MeteorStaff;
        }

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
            float shotSpeed =
                SkySpawnProfileHelper.GetShotSpeed(
                    weapon,
                    baseVelocity
                );

            Vector2 skyAnchor =
                SkySpawnProfileHelper.GetMidpointSkyAnchor(
                    basePosition,
                    targetPosition
                );

            Vector2 spawnPosition = new Vector2(
                skyAnchor.X + Main.rand.NextFloat(
                    -HorizontalSpawnNoise,
                    HorizontalSpawnNoise
                ),
                skyAnchor.Y
            );

            Vector2 velocity =
                SkySpawnProfileHelper.AimAt(
                    spawnPosition,
                    targetPosition,
                    shotSpeed
                );

            int projectileType = PickMeteorProjectileType();

            return new[]
            {
                new NeuroWeaponShot(
                    projectileType,
                    spawnPosition,
                    velocity,
                    scale: Main.rand.NextFloat(
                        MinimumMeteorVisualScale,
                        MaximumMeteorVisualScale
                    ),
                    visualProjectileType: projectileType,
                    visualStyle: NeuroWeaponVisualStyle.ProjectileTexture
                )
            };
        }

        public static int GetCooldownTicks(Item weapon)
        {
            return SkySpawnProfileHelper.GetFullUseCooldownTicks(weapon);
        }

        private static int PickMeteorProjectileType()
        {
            return MeteorProjectileTypes[
                Main.rand.Next(MeteorProjectileTypes.Length)
            ];
        }
    }
}