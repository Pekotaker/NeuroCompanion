using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace NeuroCompanion.Projectiles.Weapons.SpiritFlame
{
    public partial class NeuroSpiritFlameProjectile
    {
        public override void OnHitNPC(
            NPC target,
            NPC.HitInfo hit,
            int damageDone
        )
        {
            hasImpacted = true;
            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            if (!hasImpacted)
            {
                return;
            }

            SoundEngine.PlaySound(
                SoundID.Item14,
                Projectile.Center
            );

            EmitImpactExplosion();
        }

        private bool ProjectileHitsOwner(Player owner)
        {
            Rectangle projectileHitbox = Projectile.Hitbox;

            projectileHitbox.Inflate(
                (int)OwnerHitboxPadding,
                (int)OwnerHitboxPadding
            );

            if (projectileHitbox.Intersects(owner.Hitbox))
            {
                return true;
            }

            Vector2 oldCenter =
                Projectile.oldPosition +
                new Vector2(
                    Projectile.width,
                    Projectile.height
                ) * 0.5f;

            Vector2 currentCenter =
                Projectile.Center;

            float collisionPoint = 0f;

            return Collision.CheckAABBvLineCollision(
                owner.position,
                new Vector2(owner.width, owner.height),
                oldCenter,
                currentCenter,
                Projectile.width + OwnerHitboxPadding,
                ref collisionPoint
            );
        }

        private void DamageOwnerAndExplode(Player owner)
        {
            if (hasImpacted)
            {
                return;
            }

            hasImpacted = true;

            int hitDirection =
                owner.Center.X < Projectile.Center.X
                    ? -1
                    : 1;

            PlayerDeathReason deathReason =
                PlayerDeathReason.ByCustomReason(
                    $"{owner.name} was haunted by Evil Neuro."
                );

            owner.Hurt(
                deathReason,
                Projectile.damage,
                hitDirection,
                pvp: true
            );

            Projectile.Kill();
        }

        private void EmitImpactExplosion()
        {
            for (int i = 0; i < ImpactDustCount; i++)
            {
                EmitImpactDust(
                    DustID.Shadowflame,
                    GetImpactDustColor(),
                    Main.rand.NextFloat(
                        ImpactDustScaleMin,
                        ImpactDustScaleMax
                    ),
                    Main.rand.NextFloat(
                        ImpactDustSpeedMin,
                        ImpactDustSpeedMax
                    )
                );
            }

            for (int i = 0; i < ImpactSmokeDustCount; i++)
            {
                EmitImpactDust(
                    DustID.Smoke,
                    GetImpactDustColor(),
                    Main.rand.NextFloat(1.1f, 2.2f),
                    Main.rand.NextFloat(0.6f, 3.2f)
                );
            }
        }

        private void EmitImpactDust(
            int dustType,
            Color color,
            float scale,
            float speed
        )
        {
            Vector2 velocity =
                Main.rand.NextVector2CircularEdge(1f, 1f) *
                speed;

            Dust dust =
                Dust.NewDustPerfect(
                    Projectile.Center,
                    dustType,
                    velocity,
                    100,
                    color,
                    scale
                );

            dust.noGravity = true;
            dust.fadeIn = Main.rand.NextFloat(0.4f, 1.1f);

            if (dustType == DustID.Smoke)
            {
                dust.noGravity = false;
                dust.velocity *= 0.65f;
            }
        }

        private static Color GetImpactDustColor()
        {
            return ImpactDustColors[
                Main.rand.Next(ImpactDustColors.Length)
            ];
        }
    }
}