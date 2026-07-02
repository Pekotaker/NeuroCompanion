using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Neuro.Weapons
{
    public static class NeuroWeaponClassifier
    {
        private const float MinimumUsefulShootSpeed = 0.1f;

        public static NeuroWeaponClassification Classify(Item item)
        {
            if (item == null || item.IsAir)
            {
                return Invalid("No item selected.");
            }

            if (item.damage <= 0)
            {
                return Invalid($"{item.Name} does not deal damage.");
            }

            if (item.DamageType != DamageClass.Magic)
            {
                return Invalid($"{item.Name} is not a magic weapon.");
            }

            if (item.mana <= 0)
            {
                return Invalid($"{item.Name} does not use mana.");
            }

            if (item.shoot <= ProjectileID.None)
            {
                return Invalid($"{item.Name} does not shoot a projectile.");
            }

            Projectile projectile = GetProjectileSample(item.shoot);

            if (projectile == null)
            {
                return Invalid($"{item.Name}'s projectile could not be inspected.");
            }

            if (LooksLikeSupportWeapon(item, projectile))
            {
                return new NeuroWeaponClassification(
                    NeuroWeaponKind.Support,
                    $"{item.Name} is a support, stationary, or persistent-field magic weapon. Neuro cannot use it yet."
                );
            }

            if (IsTargetAreaWeapon(item))
            {
                return new NeuroWeaponClassification(
                    NeuroWeaponKind.TargetedArea,
                    $"{item.Name} is a targeted-area magic weapon. Neuro will place its attack at the target position."
                );
            }

            if (IsControlledProjectileWeapon(projectile))
            {
                return new NeuroWeaponClassification(
                    NeuroWeaponKind.Controlled,
                    $"{item.Name} is a controllable projectile weapon. Neuro will treat it like a normal direct attack."
                );
            }

            if (item.channel && LooksLikeHeldBeamWeapon(projectile))
            {
                return new NeuroWeaponClassification(
                    NeuroWeaponKind.HeldBeam,
                    $"{item.Name} is a held beam/channeling weapon. Neuro cannot use this yet because it needs a custom beam implementation."
                );
            }

            if (item.channel)
            {
                return new NeuroWeaponClassification(
                    NeuroWeaponKind.Channeling,
                    $"{item.Name} is a channeling magic weapon."
                );
            }

            return new NeuroWeaponClassification(
                NeuroWeaponKind.DirectFire,
                $"{item.Name} is a direct-fire magic weapon."
            );
        }

        public static string GetKindDisplayName(NeuroWeaponKind kind)
        {
            return kind switch
            {
                NeuroWeaponKind.DirectFire => "Direct-fire",
                NeuroWeaponKind.Controlled => "Controlled projectile",
                NeuroWeaponKind.Channeling => "Channeling",
                NeuroWeaponKind.TargetedArea => "Targeted-area",
                NeuroWeaponKind.Support => "Support",
                NeuroWeaponKind.HeldBeam => "Held beam",
                _ => "Invalid"
            };
        }

        private static NeuroWeaponClassification Invalid(string reason)
        {
            return new NeuroWeaponClassification(
                NeuroWeaponKind.Invalid,
                reason
            );
        }

        private static Projectile GetProjectileSample(int projectileType)
        {
            if (!ContentSamples.ProjectilesByType.TryGetValue(
                    projectileType,
                    out Projectile projectile
                ))
            {
                return null;
            }

            return projectile;
        }

        private static bool LooksLikeSupportWeapon(
            Item item,
            Projectile projectile
        )
        {
            // Stationary placement-style weapons usually have little or no shoot speed.
            // Example category: cloud/wall/field weapons.
            if (item.shootSpeed <= MinimumUsefulShootSpeed)
            {
                return true;
            }

            // Reject known support-style projectile AI categories,
            // not individual item IDs.
            return projectile.aiStyle == ProjAIStyleID.CursedFlameWall ||
                   projectile.aiStyle == ProjAIStyleID.RainCloud ||
                   projectile.aiStyle == ProjAIStyleID.MagnetSphere ||
                   projectile.aiStyle == ProjAIStyleID.IceRod ||
                   projectile.aiStyle == ProjAIStyleID.FrostHydra ||
                   projectile.aiStyle == ProjAIStyleID.LunarSentry ||
                   projectile.aiStyle == ProjAIStyleID.SporeTrap;
        }

        private static bool IsControlledProjectileWeapon(Projectile projectile)
        {
            return projectile.aiStyle == ProjAIStyleID.MagicMissile;
        }

        private static bool LooksLikeHeldBeamWeapon(Projectile projectile)
        {
            return projectile.aiStyle == ProjAIStyleID.ThickLaser ||
                   projectile.aiStyle == ProjAIStyleID.HeldProjectile;
        }

        private static bool IsTargetAreaWeapon(Item item)
        {
            switch (item.type)
            {
                case ItemID.PrincessWeapon:
                    return true;
                default:
                    return false;
            }
        }
    }
}
