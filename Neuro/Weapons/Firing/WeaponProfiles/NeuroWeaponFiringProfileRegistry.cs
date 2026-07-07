using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class NeuroWeaponFiringProfileRegistry
    {
        public static bool IsSupportedChannelingProfile(Item weapon)
        {
            return LaserMachinegunProfile.IsWeapon(weapon);
        }

        public static int GetProjectileType(Item weapon)
        {
            if (LaserMachinegunProfile.IsWeapon(weapon))
            {
                return LaserMachinegunProfile.ProjectileType;
            }

            return ProjectileID.None;
        }

        public static NeuroWeaponShot[] CreateShots(
            Player owner,
            Item weapon,
            Vector2 basePosition,
            Vector2 baseVelocity,
            Vector2 targetPosition
        )
        {
            if (LaserMachinegunProfile.IsWeapon(weapon))
            {
                return LaserMachinegunProfile.CreateShots(
                    basePosition,
                    baseVelocity
                );
            }

            if (BubbleGunProfile.IsWeapon(weapon))
            {
                return BubbleGunProfile.CreateShots(
                    weapon,
                    basePosition,
                    baseVelocity
                );
            }

            if (BeeGunProfile.IsWeapon(weapon))
            {
                return BeeGunProfile.CreateShots(
                    owner,
                    weapon,
                    basePosition,
                    baseVelocity
                );
            }

            if (WaspGunProfile.IsWeapon(weapon))
            {
                return WaspGunProfile.CreateShots(
                    weapon,
                    basePosition,
                    baseVelocity
                );
            }

            if (BatScepterProfile.IsWeapon(weapon))
            {
                return BatScepterProfile.CreateShots(
                    weapon,
                    basePosition,
                    baseVelocity
                );
            }

            if (FangStaffProfile.IsWeapon(weapon))
            {
                return FangStaffProfile.CreateShots(
                    weapon,
                    basePosition,
                    baseVelocity
                );
            }

            if (MeteorStaffProfile.IsWeapon(weapon))
            {
                return MeteorStaffProfile.CreateShots(
                    weapon,
                    basePosition,
                    baseVelocity,
                    targetPosition
                );
            }

            if (BlizzardStaffProfile.IsWeapon(weapon))
            {
                return BlizzardStaffProfile.CreateShots(
                    weapon,
                    basePosition,
                    baseVelocity,
                    targetPosition
                );
            }

            if (LunarFlareProfile.IsWeapon(weapon))
            {
                return LunarFlareProfile.CreateShots(
                    weapon,
                    basePosition,
                    baseVelocity,
                    targetPosition
                );
            }

            return null;
        }

        public static int GetCooldownTicks(
            Item weapon,
            int channelTicks
        )
        {
            if (LaserMachinegunProfile.IsWeapon(weapon))
            {
                return LaserMachinegunProfile.GetCooldownTicks(channelTicks);
            }

            return 1;
        }
    }
}