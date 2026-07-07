using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

using NeuroCompanion.Neuro.Weapons.Firing;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class LunarFlareProfile
    {
        private const int ProjectileCount = 3;

        private const float HorizontalSpawnHalfWidth = 120f;
        private const float VerticalSpawnJitter = 40f;

        private const int DelayBetweenFlaresTicks = 3;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.LunarFlareBook;
        }

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
            NeuroWeaponShot[] shots =
                new NeuroWeaponShot[ProjectileCount];

            float skyY = SkySpawnProfileHelper.GetNeuroSkyY(basePosition);

            float shotSpeed =
                SkySpawnProfileHelper.GetShotSpeed(
                    weapon,
                    baseVelocity
                );

            for (int i = 0; i < shots.Length; i++)
            {
                Vector2 spawnPosition = new Vector2(
                    basePosition.X + Main.rand.NextFloat(
                        -HorizontalSpawnHalfWidth,
                        HorizontalSpawnHalfWidth
                    ),
                    skyY + Main.rand.NextFloat(
                        -VerticalSpawnJitter,
                        VerticalSpawnJitter
                    )
                );

                Vector2 velocity =
                    SkySpawnProfileHelper.AimAt(
                        spawnPosition,
                        targetPosition,
                        shotSpeed
                    );

                shots[i] = new NeuroWeaponShot(
                    weapon.shoot,
                    spawnPosition,
                    velocity,
                    delayTicks: i * DelayBetweenFlaresTicks,
                    forceVisible: true
                );
            }

            return shots;
        }
    }
}