using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class TomeOfInfiniteWisdomProfile
    {
        private const int PageCount = 4;

        private const int FallbackShotIntervalTicks = 3;
        private const int FallbackVolleyCooldownTicks = 25;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.BookStaff;
        }

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity
        )
        {
            if (!IsWeapon(weapon))
            {
                return Array.Empty<NeuroWeaponShot>();
            }

            int shotIntervalTicks =
                GetShotIntervalTicks(weapon);

            NeuroWeaponShot[] shots =
                new NeuroWeaponShot[PageCount];

            for (int pageIndex = 0; pageIndex < PageCount; pageIndex++)
            {
                shots[pageIndex] =
                    new NeuroWeaponShot(
                        weapon.shoot,
                        basePosition,
                        baseVelocity,
                        delayTicks:
                            pageIndex *
                            shotIntervalTicks
                    );
            }

            return shots;
        }

        public static int GetCooldownTicks(Item weapon)
        {
            if (
                weapon == null ||
                weapon.IsAir ||
                weapon.useAnimation <= 0
            )
            {
                return FallbackVolleyCooldownTicks;
            }

            return weapon.useAnimation;
        }

        private static int GetShotIntervalTicks(Item weapon)
        {
            if (
                weapon == null ||
                weapon.IsAir ||
                weapon.useTime <= 0
            )
            {
                return FallbackShotIntervalTicks;
            }

            return weapon.useTime;
        }
    }
}