using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Weapons.LifeDrain;

namespace NeuroCompanion.Neuro.Weapons.Firing.WeaponProfiles
{
    public static class LifeDrainProfile
    {
        public static bool IsWeapon(Item weapon)
        {
            return weapon != null &&
                   !weapon.IsAir &&
                   weapon.type == ItemID.SoulDrain;
        }

        public static NeuroWeaponShot[] CreateShots(
            Item weapon,
            Vector2 targetPosition
        )
        {
            return new[]
            {
                new NeuroWeaponShot(
                    ModContent.ProjectileType<NeuroLifeDrainField>(),
                    targetPosition,
                    Vector2.Zero,
                    ai0: targetPosition.X,
                    ai1: targetPosition.Y,
                    forceVisible: true
                )
            };
        }

        public static int GetCooldownTicks(Item weapon)
        {
            if (weapon == null || weapon.IsAir)
            {
                return 12;
            }

            return Math.Max(1, weapon.useTime);
        }
    }
}