using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles.Attacks
{
    public class EvilNeuroBoltProjectile : ModProjectile
    {
        private const int ProjectileWidth = 10;
        private const int ProjectileHeight = 10;
        private const int LifetimeTicks = 180;

        public override string Texture => "Terraria/Images/Projectile_1";

        public override void SetDefaults()
        {
            Projectile.width = ProjectileWidth;
            Projectile.height = ProjectileHeight;

            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.penetrate = 1;
            Projectile.timeLeft = LifetimeTicks;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation =
                Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust.NewDust(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                DustID.GemRuby
            );
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.GemRuby
                );
            }
        }
    }
}