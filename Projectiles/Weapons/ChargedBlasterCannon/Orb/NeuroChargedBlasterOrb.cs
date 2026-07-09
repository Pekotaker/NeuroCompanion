using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Orb
{
    public partial class NeuroChargedBlasterOrb : ModProjectile
    {


        public override string Texture =>
            "Terraria/Images/Projectile_" + ProjectileID.ChargedBlasterOrb;

        private bool initialized;

        private bool IsHeavyOrb =>
            Projectile.ai[0] >= 0.5f;

        private static Color GetOrbEffectColor()
        {
            return ChargedBlasterCannonEffectColor.GetColor();
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] =
                Main.projFrames[ProjectileID.ChargedBlasterOrb];
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = SmallOrbLifetimeTicks;
            Projectile.alpha = 0;

            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (!initialized)
            {
                initialized = true;

                if (IsHeavyOrb)
                {
                    Projectile.Resize(HeavyOrbWidth, HeavyOrbHeight);
                    Projectile.scale = HeavyOrbScale;
                    Projectile.penetrate = HeavyOrbPenetration;
                    Projectile.timeLeft = HeavyOrbLifetimeTicks;

                    Projectile.usesLocalNPCImmunity = true;
                    Projectile.localNPCHitCooldown = HeavyOrbLocalNpcHitCooldownTicks;
                }
                else
                {
                    Projectile.Resize(SmallOrbWidth, SmallOrbHeight);
                    Projectile.scale = SmallOrbScale;
                    Projectile.penetrate = SmallOrbPenetration;
                }
            }

            UpdateAnimation();

            Projectile.rotation =
                Projectile.velocity.ToRotation();

            Lighting.AddLight(
                Projectile.Center,
                OrbLightRed,
                OrbLightGreen,
                OrbLightBlue
            );

            EmitDraggedTrailDust();
        }

        private void UpdateAnimation()
        {
            int frameCount = Main.projFrames[Type];

            if (frameCount <= 1)
            {
                return;
            }

            Projectile.frameCounter++;

            int ticksPerFrame =
                IsHeavyOrb ? 4 : 3;

            if (Projectile.frameCounter < ticksPerFrame)
            {
                return;
            }

            Projectile.frameCounter = 0;
            Projectile.frame++;

            if (Projectile.frame >= frameCount)
            {
                Projectile.frame = 0;
            }
        }
    }
}