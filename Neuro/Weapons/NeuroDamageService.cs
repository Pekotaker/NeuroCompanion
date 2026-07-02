using NeuroCompanion.Players;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace NeuroCompanion.Neuro.Weapons
{
    public static class NeuroDamageService
    {
        public static int GetNeuroWeaponDamage(
            Player owner,
            Item weapon,
            int staffPrefix
        )
        {
            if (owner == null || weapon == null || weapon.IsAir)
            {
                return 0;
            }

            int playerScaledDamage = (int)owner
                .GetTotalDamage(DamageClass.Magic)
                .ApplyTo(weapon.damage);

            float staffDamageMultiplier =
                GetStaffPrefixDamageMultiplier(staffPrefix);

            int finalDamage = (int)Math.Round(
                playerScaledDamage * staffDamageMultiplier
            );

            return finalDamage < 1 ? 1 : finalDamage;
        }

        public static float GetNeuroWeaponKnockBack(
            Player owner,
            Item weapon,
            int staffPrefix
        )
        {
            if (owner == null || weapon == null || weapon.IsAir)
            {
                return 0f;
            }

            float playerScaledKnockBack = owner
                .GetTotalKnockback(DamageClass.Magic)
                .ApplyTo(weapon.knockBack);

            return playerScaledKnockBack *
                   GetStaffPrefixKnockBackMultiplier(staffPrefix);
        }

        public static bool IsAllowedUniversalPrefix(int prefix)
        {
            if (prefix == 0)
            {
                return true;
            }

            switch (prefix)
            {
                case PrefixID.Keen:
                case PrefixID.Superior:
                case PrefixID.Forceful:
                case PrefixID.Broken:
                case PrefixID.Damaged:
                case PrefixID.Shoddy:
                case PrefixID.Hurtful:
                case PrefixID.Strong:
                case PrefixID.Unpleasant:
                case PrefixID.Weak:
                case PrefixID.Ruthless:
                case PrefixID.Godly:
                case PrefixID.Demonic:
                case PrefixID.Zealous:
                case PrefixID.Frenzying:
                    return true;

                default:
                    return false;
            }
        }

        public static int ChooseRandomUniversalPrefix(UnifiedRandom rand)
        {
            int[] allowedPrefixes =
            {
                PrefixID.Keen,
                PrefixID.Superior,
                PrefixID.Forceful,
                PrefixID.Broken,
                PrefixID.Damaged,
                PrefixID.Shoddy,
                PrefixID.Hurtful,
                PrefixID.Strong,
                PrefixID.Unpleasant,
                PrefixID.Weak,
                PrefixID.Ruthless,
                PrefixID.Godly,
                PrefixID.Demonic,
                PrefixID.Zealous,
                PrefixID.Frenzying
            };

            return allowedPrefixes[rand.Next(allowedPrefixes.Length)];
        }

        public static int GetNeuroWeaponCritChance(
            Player owner,
            Item weapon,
            int staffPrefix
        )
        {
            if (owner == null || weapon == null || weapon.IsAir)
            {
                return 0;
            }

            int critChance = owner.GetWeaponCrit(weapon);

            critChance += GetStaffPrefixCritBonus(staffPrefix);

            if (critChance < 0)
            {
                return 0;
            }

            return critChance;
        }

        public static int GetStaffPrefixShootCooldownTicks(
            int baseCooldownTicks,
            int staffPrefix
        )
        {
            if (baseCooldownTicks < 1)
            {
                baseCooldownTicks = NeuroCompanionPlayer.DefaultNeuroStaffShootCooldownTicks;
            }

            float multiplier = GetStaffPrefixUseTimeMultiplier(staffPrefix);

            int finalCooldown = (int)Math.Round(baseCooldownTicks * multiplier);

            return finalCooldown < 1 ? 1 : finalCooldown;
        }

        public static int GetWeaponInherentShootCooldownTicks(Item weapon)
        {
            if (weapon == null || weapon.IsAir)
            {
                return 1;
            }

            int useTime = weapon.useTime;

            if (useTime <= 0)
            {
                useTime = 1;
            }

            return useTime;
        }

        public static int GetEffectiveNeuroShootCooldownTicks(
            Item weapon,
            int staffBaseCooldownTicks,
            int staffPrefix
        )
        {
            int staffCooldownTicks = GetStaffPrefixShootCooldownTicks(
                staffBaseCooldownTicks,
                staffPrefix
            );

            int weaponCooldownTicks = GetWeaponInherentShootCooldownTicks(weapon);

            if (staffCooldownTicks > weaponCooldownTicks)
            {
                return staffCooldownTicks;
            }

            return weaponCooldownTicks;
        }

        private static float GetStaffPrefixDamageMultiplier(int prefix)
        {
            switch (prefix)
            {
                case PrefixID.Superior:
                    return 1.10f;

                case PrefixID.Broken:
                    return 0.70f;

                case PrefixID.Damaged:
                    return 0.85f;

                case PrefixID.Shoddy:
                    return 0.90f;

                case PrefixID.Hurtful:
                    return 1.10f;

                case PrefixID.Unpleasant:
                    return 1.05f;

                case PrefixID.Ruthless:
                    return 1.18f;

                case PrefixID.Godly:
                    return 1.15f;

                case PrefixID.Demonic:
                    return 1.15f;

                default:
                    return 1f;
            }
        }

        private static float GetStaffPrefixKnockBackMultiplier(int prefix)
        {
            switch (prefix)
            {
                case PrefixID.Superior:
                    return 1.10f;

                case PrefixID.Forceful:
                    return 1.15f;

                case PrefixID.Broken:
                    return 0.80f;

                case PrefixID.Shoddy:
                    return 0.85f;

                case PrefixID.Strong:
                    return 1.15f;

                case PrefixID.Unpleasant:
                    return 1.15f;

                case PrefixID.Weak:
                    return 0.80f;

                case PrefixID.Ruthless:
                    return 0.90f;

                case PrefixID.Godly:
                    return 1.15f;

                default:
                    return 1f;
            }
        }

        private static int GetStaffPrefixCritBonus(int prefix)
        {
            switch (prefix)
            {
                case PrefixID.Keen:
                    return 3;

                case PrefixID.Superior:
                    return 3;

                case PrefixID.Godly:
                    return 5;

                case PrefixID.Demonic:
                    return 5;

                case PrefixID.Zealous:
                    return 5;

                default:
                    return 0;
            }
        }

        private static float GetStaffPrefixUseTimeMultiplier(int prefix)
        {
            switch (prefix)
            {
                case PrefixID.Frenzying:
                    return 0.90f;

                default:
                    return 1f;
            }
        }
    }
}