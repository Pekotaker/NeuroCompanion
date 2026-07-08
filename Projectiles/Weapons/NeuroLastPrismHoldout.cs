using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles.Weapons
{
    public class NeuroLastPrismHoldout : ModProjectile
    {
        public override string Texture =>
            "Terraria/Images/Projectile_" + ProjectileID.LastPrism;

        public const int NumBeams = 6;
        public const int DurationTicks = 150;

        public const float MaxCharge = 90f;
        public const float DamageStart = 20f;

        private const int NumAnimationFrames = 5;
        private const float AimResponsiveness = 0.12f;
        private const int SoundInterval = 20;

        private readonly int[] beamIndexes = new int[NumBeams];

        private float FrameCounter
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        private Vector2 TargetPosition =>
            new Vector2(Projectile.ai[0], Projectile.ai[1]);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = NumAnimationFrames;
            ProjectileID.Sets.NeedsUUID[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.LastPrism);

            Projectile.width = 40;
            Projectile.height = 40;

            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.netImportant = true;
            Projectile.timeLeft = DurationTicks + 2;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            FrameCounter++;

            UpdateAim();
            UpdateAnimation();
            PlaySounds();

            Projectile.rotation =
                Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.spriteDirection =
                Projectile.velocity.X >= 0f ? 1 : -1;

            if (FrameCounter == 1f)
            {
                FireBeams();
            }

            if (FrameCounter >= DurationTicks)
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;

            Lighting.AddLight(
                Projectile.Center,
                0.8f,
                0.65f,
                1f
            );
        }

        private void UpdateAim()
        {
            float speed = Projectile.velocity.Length();

            if (speed <= 0.01f)
            {
                speed = 30f;
            }

            Vector2 currentAim =
                SafeNormalize(Projectile.velocity, Vector2.UnitX);

            Vector2 targetAim =
                SafeNormalize(TargetPosition - Projectile.Center, currentAim);

            Vector2 aim =
                Vector2.Lerp(
                    currentAim,
                    targetAim,
                    AimResponsiveness
                );

            aim = SafeNormalize(aim, targetAim) * speed;

            if ((aim - Projectile.velocity).LengthSquared() > 0.01f)
            {
                Projectile.netUpdate = true;
            }

            Projectile.velocity = aim;
        }

        private void UpdateAnimation()
        {
            Projectile.frameCounter++;

            int framesPerAnimationUpdate =
                FrameCounter >= MaxCharge ? 2 :
                FrameCounter >= MaxCharge * 0.66f ? 3 :
                4;

            if (Projectile.frameCounter < framesPerAnimationUpdate)
            {
                return;
            }

            Projectile.frameCounter = 0;

            Projectile.frame++;

            if (Projectile.frame >= NumAnimationFrames)
            {
                Projectile.frame = 0;
            }
        }

        private void PlaySounds()
        {
            if (Projectile.soundDelay > 0)
            {
                return;
            }

            Projectile.soundDelay = SoundInterval;

            if (FrameCounter > 1f)
            {
                SoundEngine.PlaySound(SoundID.Item15, Projectile.Center);
            }
        }

        private void FireBeams()
        {
            Vector2 beamVelocity =
                SafeNormalize(Projectile.velocity, -Vector2.UnitY);

            int damage = Projectile.damage;
            float knockBack = Projectile.knockBack;

            for (int i = 0; i < NumBeams; i++)
            {
                int beamIndex = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    beamVelocity,
                    ModContent.ProjectileType<NeuroLastPrismBeam>(),
                    damage,
                    knockBack,
                    Projectile.owner,
                    i,
                    Projectile.whoAmI
                );

                beamIndexes[i] = beamIndex;
            }

            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects =
                Projectile.spriteDirection == -1
                    ? SpriteEffects.FlipHorizontally
                    : SpriteEffects.None;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            int frameHeight =
                texture.Height / Main.projFrames[Type];

            Rectangle sourceRectangle =
                new Rectangle(
                    0,
                    frameHeight * Projectile.frame,
                    texture.Width,
                    frameHeight
                );

            Vector2 origin =
                sourceRectangle.Size() * 0.5f;

            Color drawColor =
                new Color(255, 220, 255);

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRectangle,
                drawColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                effects,
                0
            );

            return false;
        }

        private static Vector2 SafeNormalize(
            Vector2 value,
            Vector2 fallback
        )
        {
            if (value.LengthSquared() <= 0.01f || value.HasNaNs())
            {
                return fallback;
            }

            value.Normalize();
            return value;
        }
    }
}