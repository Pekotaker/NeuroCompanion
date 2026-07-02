using NeuroCompanion.Neuro.Weapons.Firing;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles.Globals
{
    public class NeuroWeaponProjectileGlobal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsNeuroWeaponProjectile { get; private set; }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            NeuroWeaponProjectileSpawnContext context =
                NeuroWeaponProjectileSpawnContext.Current;

            if (context == null)
            {
                return;
            }

            if (context.Owner == null || projectile.owner != context.Owner.whoAmI)
            {
                return;
            }

            ApplyNeuroWeaponBehavior(projectile, context);
        }

        private void ApplyNeuroWeaponBehavior(
            Projectile projectile,
            NeuroWeaponProjectileSpawnContext context
        )
        {
            IsNeuroWeaponProjectile = true;

            projectile.DamageType = DamageClass.Magic;

            projectile.damage = context.Damage;
            projectile.originalDamage = context.Damage;

            if (context.IsEvil)
            {
                ApplyEvilBehavior(projectile, context);
            }
            else
            {
                ApplyFriendlyBehavior(projectile, context);
            }

            projectile.netUpdate = true;
        }

        private static void ApplyFriendlyBehavior(
            Projectile projectile,
            NeuroWeaponProjectileSpawnContext context
        )
        {
            projectile.friendly = true;
            projectile.hostile = false;

            projectile.CritChance = context.CritChance;

            if (
                context.NeuroPlayer != null &&
                context.NeuroPlayer.NeuroStaffCanDetectThroughBlocks
            )
            {
                projectile.tileCollide = false;

                projectile
                    .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                    .IgnoreTilesForNeuroMk4 = true;
            }
        }

        private static void ApplyEvilBehavior(
            Projectile projectile,
            NeuroWeaponProjectileSpawnContext context
        )
        {
            projectile.friendly = false;
            projectile.hostile = false;

            EvilNeuroPlayerAttackGlobal evilGlobal =
                projectile.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            evilGlobal.CanDamageOwner = true;
            evilGlobal.KillOnOwnerHit = context.KillOnOwnerHit;
        }
    }
}