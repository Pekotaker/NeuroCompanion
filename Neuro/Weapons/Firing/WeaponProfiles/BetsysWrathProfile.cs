using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class BetsysWrathProfile
    {
        // private const int ProjectileCount = 3;

        private const float CloseShotSpeedMultiplier = 0.9f;
        private const float NormalShotSpeedMultiplier = 1f;
        private const float FarShotSpeedMultiplier = 1.1f;

        private const int VanillaReuseDelayTicks = 10;

        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.ApprenticeStaffT3;
        }

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity
        )
        {
            return new[]
            {
                new NeuroWeaponShot(
                    weapon.shoot,
                    basePosition,
                    baseVelocity * CloseShotSpeedMultiplier
                ),

                new NeuroWeaponShot(
                    weapon.shoot,
                    basePosition,
                    baseVelocity * NormalShotSpeedMultiplier
                ),

                new NeuroWeaponShot(
                    weapon.shoot,
                    basePosition,
                    baseVelocity * FarShotSpeedMultiplier
                )
            };
        }

        public static int GetCooldownTicks(Item weapon)
        {
            if (weapon == null || weapon.IsAir)
            {
                return VanillaReuseDelayTicks;
            }

            return weapon.useTime + VanillaReuseDelayTicks;
        }
    }
}