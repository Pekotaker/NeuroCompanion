using Microsoft.Xna.Framework;

using System;

using Terraria;
using Terraria.DataStructures;

using NeuroCompanion.Projectiles.Globals;

namespace NeuroCompanion.Projectiles.Weapons
{
    public partial class NeuroLastPrismBeam
    {
        private void UpdateOwnerHitCooldown()
        {
            if (ownerHitCooldown > 0)
            {
                ownerHitCooldown--;
            }
        }

        private void UpdateDamageFromHost(
            Projectile hostPrism,
            float chargeRatio
        )
        {
            Projectile.damage =
                Math.Max(
                    1,
                    (int)(
                        hostPrism.damage *
                        GetDamageMultiplier(chargeRatio) *
                        DamageBalanceMultiplier
                    )
                );

            Projectile.originalDamage = Projectile.damage;
            Projectile.CritChance = hostPrism.CritChance;
        }

        private void ApplyDamageBehaviorFromHost(
            Projectile hostPrism,
            float chargeRatio
        )
        {
            bool canDamageOwner =
                hostPrism
                    .GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>()
                    .CanDamageOwner;

            EvilNeuroPlayerAttackGlobal evilGlobal =
                Projectile.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            if (canDamageOwner)
            {
                Projectile.friendly = false;
                Projectile.hostile = false;

                evilGlobal.CanDamageOwner = true;
                evilGlobal.KillOnOwnerHit = false;

                return;
            }

            evilGlobal.CanDamageOwner = false;
            evilGlobal.KillOnOwnerHit = false;

            Projectile.friendly =
                hostPrism.friendly &&
                chargeRatio >= VisualEffectThreshold &&
                hostPrism.localAI[0] > NeuroLastPrismHoldout.DamageStart;

            Projectile.hostile = false;
        }

        private float GetDamageMultiplier(float chargeRatio)
        {
            float eased =
                chargeRatio * chargeRatio * chargeRatio;

            return MathHelper.Lerp(
                1f,
                MaxDamageMultiplier,
                eased
            );
        }

        private void TryDamageOwnerWithBeam()
        {
            bool canDamageOwner =
                Projectile
                    .GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>()
                    .CanDamageOwner;

            if (!canDamageOwner)
            {
                return;
            }

            if (ownerHitCooldown > 0)
            {
                return;
            }

            if (Projectile.owner < 0 || Projectile.owner >= Main.maxPlayers)
            {
                return;
            }

            Player owner = Main.player[Projectile.owner];

            if (!owner.active || owner.dead)
            {
                return;
            }

            float collisionPoint = 0f;

            bool hit =
                Collision.CheckAABBvLineCollision(
                    owner.position,
                    new Vector2(owner.width, owner.height),
                    Projectile.Center,
                    Projectile.Center + Projectile.velocity * BeamLength,
                    BeamHitboxCollisionWidth * Projectile.scale,
                    ref collisionPoint
                );

            if (!hit)
            {
                return;
            }

            int hitDirection =
                owner.Center.X < Projectile.Center.X ? -1 : 1;

            PlayerDeathReason deathReason =
                PlayerDeathReason.ByCustomReason(
                    $"{owner.name} was vaporized by Evil Neuro."
                );

            owner.Hurt(
                deathReason,
                Projectile.damage,
                hitDirection,
                pvp: true
            );

            ownerHitCooldown = OwnerHitCooldownTicks;
        }
    }
}