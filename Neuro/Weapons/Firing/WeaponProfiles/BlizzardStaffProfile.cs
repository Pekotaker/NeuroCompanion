using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

using NeuroCompanion.Neuro.Weapons.Firing;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class BlizzardStaffProfile
    {
        private const int IciclesPerPair = 2;
        private const int NormalPairCount = 2;
        private const int MythicalPairCount = 3;

        private const int PairDelayTicks = 5;

        private const float HorizontalSpawnHalfWidth = 220f;
        private const float AimNoiseHalfWidth = 160f;
        private const float AimNoiseVerticalHalfHeight = 40f;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.BlizzardStaff;
        }

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
            int pairCount = ShouldUseMythicalExtraPair(weapon)
                ? MythicalPairCount
                : NormalPairCount;

            int totalIcicles = pairCount * IciclesPerPair;

            NeuroWeaponShot[] shots =
                new NeuroWeaponShot[totalIcicles];

            float skyY = SkySpawnProfileHelper.GetNeuroSkyY(basePosition);

            float shotSpeed =
                SkySpawnProfileHelper.GetShotSpeed(
                    weapon,
                    baseVelocity
                );

            int shotIndex = 0;

            for (int pairIndex = 0; pairIndex < pairCount; pairIndex++)
            {
                int delayTicks = pairIndex * PairDelayTicks;

                for (int i = 0; i < IciclesPerPair; i++)
                {
                    Vector2 spawnPosition = new Vector2(
                        basePosition.X + Main.rand.NextFloat(
                            -HorizontalSpawnHalfWidth,
                            HorizontalSpawnHalfWidth
                        ),
                        skyY + Main.rand.NextFloat(-20f, 20f)
                    );

                    Vector2 noisyAimPoint = targetPosition + new Vector2(
                        Main.rand.NextFloat(
                            -AimNoiseHalfWidth,
                            AimNoiseHalfWidth
                        ),
                        Main.rand.NextFloat(
                            -AimNoiseVerticalHalfHeight,
                            AimNoiseVerticalHalfHeight
                        )
                    );

                    Vector2 velocity =
                        SkySpawnProfileHelper.AimAt(
                            spawnPosition,
                            noisyAimPoint,
                            shotSpeed
                        );

                    shots[shotIndex] = new NeuroWeaponShot(
                        weapon.shoot,
                        spawnPosition,
                        velocity,
                        delayTicks: delayTicks,
                        forceVisible: true
                    );

                    shotIndex++;
                }
            }

            return shots;
        }

        private static bool ShouldUseMythicalExtraPair(Item weapon)
        {
            return weapon != null && weapon.prefix == PrefixID.Mythical;
        }
    }
}