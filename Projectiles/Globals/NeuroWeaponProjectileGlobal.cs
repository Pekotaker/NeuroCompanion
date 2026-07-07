using System.Collections.Generic;

using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

using NeuroCompanion.Neuro.Weapons.Firing;

namespace NeuroCompanion.Projectiles.Globals
{
    public class NeuroWeaponProjectileGlobal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsNeuroWeaponProjectile { get; private set; }

        public bool UseSkyDrawLayer { get; set; }

        public int FrameOverride { get; set; } = -1;

        public override void PostAI(Projectile projectile)
        {
            if (FrameOverride < 0)
            {
                return;
            }

            projectile.frame = FrameOverride;
        }

        public override void DrawBehind(
            Projectile projectile,
            int index,
            List<int> behindNPCsAndTiles,
            List<int> behindNPCs,
            List<int> behindProjectiles,
            List<int> overPlayers,
            List<int> overWiresUI
        )
        {
            if (!UseSkyDrawLayer)
            {
                return;
            }

            overPlayers.Add(index);
        }

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