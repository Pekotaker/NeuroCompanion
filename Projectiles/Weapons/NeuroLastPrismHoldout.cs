using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles.Weapons
{
    public partial class NeuroLastPrismHoldout : ModProjectile
    {
        public override string Texture =>
            "Terraria/Images/Projectile_" + ProjectileID.LastPrism;

        public const int NumBeams = 6;
        public const int DurationTicks = 260;
        public const int RefreshGraceTicks = 12;

        public const float MaxCharge = 200f;
        public const float DamageStart = 12f;

        private const int NumAnimationFrames = 5;
        private const float PrismDistanceFromNeuro = 28f;
        private const float AimResponsiveness = 0.18f;
        private const int SoundInterval = 20;

        private bool initialized;
        private Vector2 fallbackAnchorPosition;

        private float FrameCounter
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        private float RemainingLifeTicks
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
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
            // Do not CloneDefaults(ProjectileID.LastPrism).
            // Vanilla Last Prism is a player-held projectile and can lock the
            // player into item-use animation. Neuro's prism must be independent.

            Projectile.width = 40;
            Projectile.height = 40;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.alpha = 0;
            Projectile.scale = 1f;

            Projectile.aiStyle = 0;
            Projectile.timeLeft = DurationTicks + 2;
            Projectile.netImportant = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            if (!initialized)
            {
                initialized = true;
                fallbackAnchorPosition = Projectile.Center;

                if (RemainingLifeTicks <= 0f)
                {
                    RemainingLifeTicks = DurationTicks;
                }
            }

            RemainingLifeTicks--;

            if (RemainingLifeTicks <= 0f)
            {
                Projectile.Kill();
                return;
            }

            FrameCounter++;

            Projectile.hide = false;

            Vector2 currentDirection =
                SafeNormalize(
                    Projectile.velocity,
                    SafeNormalize(
                        TargetPosition - Projectile.Center,
                        Vector2.UnitX
                    )
                );

            Projectile.Center = GetAnchorPosition(currentDirection);

            UpdateAim();

            Projectile.Center =
                GetAnchorPosition(
                    SafeNormalize(
                        Projectile.velocity,
                        currentDirection
                    )
                );

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

            Projectile.timeLeft = 2;

            Lighting.AddLight(
                Projectile.Center,
                0.8f,
                0.65f,
                1f
            );
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