using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class NeuroWeaponFiringProfileRegistry
    {
        public static bool IsSupportedChannelingProfile(Item weapon)
        {
            return LaserMachinegunProfile.IsWeapon(weapon) ||
                   LastPrismProfile.IsWeapon(weapon) ||
                   ChargedBlasterCannonProfile.IsWeapon(weapon);
        }

        public static int GetProjectileType(Item weapon)
        {
            if (LaserMachinegunProfile.IsWeapon(weapon))
            {
                return LaserMachinegunProfile.ProjectileType;
            }

            if (LastPrismProfile.IsWeapon(weapon))
            {
                return LastPrismProfile.ProjectileType;
            }

            if (ChargedBlasterCannonProfile.IsWeapon(weapon))
            {
                return ChargedBlasterCannonProfile.ProjectileType;
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

            if (LastPrismProfile.IsWeapon(weapon))
            {
                return LastPrismProfile.CreateShots(
                    owner,
                    weapon,
                    basePosition,
                    baseVelocity,
                    targetPosition
                );
            }

            if (ChargedBlasterCannonProfile.IsWeapon(weapon))
            {
                return ChargedBlasterCannonProfile.CreateShots(
                    owner,
                    weapon,
                    basePosition,
                    baseVelocity,
                    targetPosition
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

            if (SkyFractureProfile.IsWeapon(weapon))
            {
                return SkyFractureProfile.CreateShots(
                    weapon,
                    basePosition,
                    baseVelocity,
                    targetPosition
                );
            }

            if (NightglowProfile.IsWeapon(weapon))
            {
                return NightglowProfile.CreateShots(
                    weapon,
                    basePosition,
                    baseVelocity,
                    targetPosition
                );
            }

            if (ShadowflameHexDollProfile.IsWeapon(weapon))
            {
                return ShadowflameHexDollProfile.CreateShots(
                    weapon,
                    basePosition,
                    baseVelocity,
                    targetPosition
                );
            }

            if (BloodThornProfile.IsWeapon(weapon))
            {
                return BloodThornProfile.CreateShots(
                    weapon,
                    basePosition,
                    baseVelocity,
                    targetPosition
                );
            }

            if (StellarTuneProfile.IsWeapon(weapon))
            {
                return StellarTuneProfile.CreateShots(
                    weapon,
                    basePosition,
                    baseVelocity,
                    targetPosition
                );
            }

            if (BetsysWrathProfile.IsWeapon(weapon))
            {
                return BetsysWrathProfile.CreateShots(
                    weapon,
                    basePosition,
                    baseVelocity
                );
            }

            if (SpiritFlameProfile.IsWeapon(weapon))
            {
                return SpiritFlameProfile.CreateShots(
                    weapon,
                    basePosition,
                    baseVelocity
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

            if (LastPrismProfile.IsWeapon(weapon))
            {
                return LastPrismProfile.GetCooldownTicks(channelTicks);
            }

            if (ChargedBlasterCannonProfile.IsWeapon(weapon))
            {
                return ChargedBlasterCannonProfile.GetCooldownTicks(channelTicks);
            }

            return 1;
        }

        public static bool TryGetProfileShootCooldownTicks(
            Item weapon,
            out int cooldownTicks
        )
        {
            if (MeteorStaffProfile.IsWeapon(weapon))
            {
                cooldownTicks = MeteorStaffProfile.GetCooldownTicks(weapon);
                return true;
            }

            if (BlizzardStaffProfile.IsWeapon(weapon))
            {
                cooldownTicks = BlizzardStaffProfile.GetCooldownTicks(weapon);
                return true;
            }

            if (LunarFlareProfile.IsWeapon(weapon))
            {
                cooldownTicks = LunarFlareProfile.GetCooldownTicks(weapon);
                return true;
            }

            if (SkyFractureProfile.IsWeapon(weapon))
            {
                cooldownTicks = SkyFractureProfile.GetCooldownTicks();
                return true;
            }

            if (NightglowProfile.IsWeapon(weapon))
            {
                cooldownTicks = NightglowProfile.GetCooldownTicks(weapon);
                return true;
            }

            if (ShadowflameHexDollProfile.IsWeapon(weapon))
            {
                cooldownTicks = ShadowflameHexDollProfile.GetCooldownTicks(weapon);
                return true;
            }

            if (BloodThornProfile.IsWeapon(weapon))
            {
                cooldownTicks = BloodThornProfile.GetCooldownTicks(weapon);
                return true;
            }

            if (StellarTuneProfile.IsWeapon(weapon))
            {
                cooldownTicks = StellarTuneProfile.GetCooldownTicks(weapon);
                return true;
            }

            if (BetsysWrathProfile.IsWeapon(weapon))
            {
                cooldownTicks = BetsysWrathProfile.GetCooldownTicks(weapon);
                return true;
            }

            if (SpiritFlameProfile.IsWeapon(weapon))
            {
                cooldownTicks = SpiritFlameProfile.GetCooldownTicks(weapon);
                return true;
            }

            cooldownTicks = 0;
            return false;
        }
    }
}