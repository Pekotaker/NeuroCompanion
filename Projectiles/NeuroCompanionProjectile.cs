using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles
{
    public class NeuroCompanionProjectile : ModProjectile
    {
        // Temporary texture: use vanilla Baby Slime projectile sprite.
        // This texture is animated, so we must tell tModLoader how many frames it has.
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.BabySlime}";

        public override void SetStaticDefaults()
        {
            // Baby Slime uses an animation sheet.
            // Without this, the whole sheet gets drawn as one tall image.
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.BabySlime];
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 20;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType = DamageClass.Summon;

            Projectile.damage = 8;
            Projectile.knockBack = 2f;

            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            // Gravity.
            if (Projectile.velocity.Y < 10f)
            {
                Projectile.velocity.Y += 0.25f;
            }

            // Spin while moving.
            Projectile.rotation += Projectile.velocity.X * 0.05f;

            // Animate the baby slime sprite.
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            // Small visual sparkle so you can clearly see that this is your custom projectile.
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.GemSapphire
                );
            }
        }
    }
}