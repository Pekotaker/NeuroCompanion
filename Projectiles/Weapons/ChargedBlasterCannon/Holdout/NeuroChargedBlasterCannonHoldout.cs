using Microsoft.Xna.Framework;
using NeuroCompanion.Projectiles.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Holdout
{
    public partial class NeuroChargedBlasterCannonHoldout : ModProjectile
    {
        public override string Texture =>
            "Terraria/Images/Projectile_" + ProjectileID.ChargedBlasterCannon;

        private bool initialized;
        private bool beamSpawned;
        private bool initialPewSoundPlayed;

        private int rapidOrbTimer = RapidOrbCooldownTicks - 1;
        private int heavyOrbTimer = HeavyOrbCooldownTicks - 1;
        private int heavyHumTimer;

        private int cannonDustTimer;

        private Vector2 fallbackAnchorPosition;

        private int smallOrbsFired;
        private int heavyOrbsFired; 

        private float ChargeTicks
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        private float RemainingLifeTicks
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }

        public bool OwnerDamageEnabled
        {
            get => Projectile.localAI[2] >= 0.5f;
            private set => Projectile.localAI[2] = value ? 1f : 0f;
        }

        public bool BeamPhaseActive =>
            !SingleShotOnly &&
            ChargeTicks >= BeamStartTicks &&
            ChargeTicks < CycleResetTicks;

        private bool SingleShotOnly =>
            Projectile.ai[2] >= 0.5f;

        private Vector2 TargetPosition =>
            new Vector2(Projectile.ai[0], Projectile.ai[1]);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] =
                Main.projFrames[ProjectileID.ChargedBlasterCannon];

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;

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

                OwnerDamageEnabled =
                    Projectile
                        .GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>()
                        .CanDamageOwner;

                if (RemainingLifeTicks <= 0f)
                {
                    RemainingLifeTicks = DurationTicks;
                }
            }

            PreventGenericOwnerDamageOnHoldout();

            RemainingLifeTicks--;

            if (RemainingLifeTicks <= 0f)
            {
                Projectile.Kill();
                return;
            }

            ChargeTicks++;

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
            FireCurrentChargePhase();
            EmitCannonPhaseDust();

            Projectile.rotation =
                Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            Projectile.spriteDirection =
                Projectile.velocity.X >= 0f ? 1 : -1;

            Projectile.timeLeft = 2;

            Lighting.AddLight(
                Projectile.Center,
                0.35f,
                0.55f,
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