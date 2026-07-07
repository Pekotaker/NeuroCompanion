using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

using NeuroCompanion.Neuro.Weapons.Firing;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class LunarFlareProfile
    {
        private static readonly float[] SpawnXOffsets =
        {
            -48f,
            0f,
            48f
        };

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
                new NeuroWeaponShot[SpawnXOffsets.Length];

            float skyTopY = SkySpawnProfileHelper.GetScreenTopY();

            float shotSpeed =
                SkySpawnProfileHelper.GetShotSpeed(
                    weapon,
                    baseVelocity
                );

            for (int i = 0; i < SpawnXOffsets.Length; i++)
            {
                Vector2 spawnPosition = new Vector2(
                    targetPosition.X + SpawnXOffsets[i],
                    skyTopY
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
                    velocity
                );
            }

            return shots;
        }
    }
}