using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

using NeuroCompanion.Neuro.Weapons.Firing;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class MeteorStaffProfile
    {
        private const float SpawnHeightAboveNeuro = 600f;

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

            Vector2 spawnPosition = new Vector2(
                targetPosition.X,
                basePosition.Y - SpawnHeightAboveNeuro
            );

            Vector2 velocity =
                SkySpawnProfileHelper.AimAt(
                    spawnPosition,
                    targetPosition,
                    shotSpeed
                );

            return new[]
            {
                new NeuroWeaponShot(
                    weapon.shoot,
                    spawnPosition,
                    velocity
                )
            };
        }
    }
}