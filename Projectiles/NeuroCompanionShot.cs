using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles
{
    public class NeuroCompanionShot : ModProjectile
    {
        private const int ProjectileWidth = 12;
        private const int ProjectileHeight = 12;
        private const int LifetimeTicks = 180;
        private const int HitCooldownTicks = 10;

        private const float LightRed = 1f;
        private const float LightGreen = 0.45f;
        private const float LightBlue = 0.15f;

        private const float RotationOffset = MathHelper.PiOver2;

        // Temporary texture: vanilla Imp Staff fireball.
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.ImpFireball}";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.ImpFireball];
        }

        public override void SetDefaults()
        {
            Projectile.width = ProjectileWidth;
            Projectile.height = ProjectileHeight;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType = DamageClass.Summon;

            Projectile.penetrate = 1;
            Projectile.timeLeft = LifetimeTicks;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = HitCooldownTicks;

            // Makes the shot update more smoothly.
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            RotateAlongVelocity();
            AnimateIfNeeded();
            CreateFireEffects();
        }

        private void RotateAlongVelocity()
        {
            if (Projectile.velocity.LengthSquared() > 0f)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + RotationOffset;
            }
        }

        private void AnimateIfNeeded()
        {
            int frameCount = Main.projFrames[Projectile.type];

            if (frameCount <= 1)
            {
                return;
            }

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= frameCount)
                {
                    Projectile.frame = 0;
                }
            }
        }

        private void CreateFireEffects()
        {
            Lighting.AddLight(Projectile.Center, LightRed, LightGreen, LightBlue);

            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Torch
                );
            }
        }
    }
}