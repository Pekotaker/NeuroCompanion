using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Globals;

namespace NeuroCompanion.Projectiles.Weapons
{
    public class NeuroLastPrismBeam : ModProjectile
    {
        public override string Texture =>
            "Terraria/Images/Projectile_" + ProjectileID.LastPrismLaser;

        private const float MaxDamageMultiplier = 1.5f;
        private const float MaxBeamScale = 1.65f;
        private const float MaxBeamSpread = 0.95f;
        private const float MaxBeamLength = 2400f;

        private const float BeamTileCollisionWidth = 1f;
        private const float BeamHitboxCollisionWidth = 22f;
        private const int NumSamplePoints = 3;

        private const float BeamLengthChangeFactor = 0.75f;
        private const float VisualEffectThreshold = 0.1f;

        private const float OuterBeamOpacityMultiplier = 0.75f;
        private const float InnerBeamOpacityMultiplier = 1f;
        private const float InnerBeamScaleMultiplier = 1f;

        private const float BeamLightBrightness = 0.75f;

        private const float BeamColorSaturation = 1f;
        private const float BeamColorLightness = 0.5f;

        private const int OwnerHitCooldownTicks = 30;

        private int ownerHitCooldown;

        private int BeamID => (int)Projectile.ai[0];

        private int HostPrismIndex => (int)Projectile.ai[1];

        private float BeamLength
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }

        private float ChargeRatioForDrawing
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.penetrate = -1;
            Projectile.alpha = 255;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.timeLeft = 2;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            Projectile hostPrism = GetHostPrism();

            if (hostPrism == null)
            {
                Projectile.Kill();
                return;
            }

            if (ownerHitCooldown > 0)
            {
                ownerHitCooldown--;
            }

            float chargeRatio =
                MathHelper.Clamp(
                    hostPrism.localAI[0] / NeuroLastPrismHoldout.MaxCharge,
                    0f,
                    1f
                );

            ChargeRatioForDrawing = chargeRatio;

            Projectile.damage =
                (int)(hostPrism.damage * GetDamageMultiplier(chargeRatio));

            Projectile.originalDamage = Projectile.damage;

            ApplyDamageBehaviorFromHost(hostPrism, chargeRatio);

            Vector2 hostDirection =
                SafeNormalize(hostPrism.velocity, -Vector2.UnitY);

            float beamIdOffset =
                BeamID - NeuroLastPrismHoldout.NumBeams / 2f + 0.5f;

            float beamSpread;
            float spinRate;
            float beamStartSidewaysOffset;
            float beamStartForwardsOffset;

            if (chargeRatio < 1f)
            {
                Projectile.scale =
                    MathHelper.Lerp(0f, MaxBeamScale, chargeRatio);

                beamSpread =
                    MathHelper.Lerp(MaxBeamSpread, 0f, chargeRatio);

                beamStartSidewaysOffset =
                    MathHelper.Lerp(16f, 3.25f, chargeRatio);

                beamStartForwardsOffset =
                    MathHelper.Lerp(-21f, -17f, chargeRatio);

                if (chargeRatio <= 0.66f)
                {
                    float phaseRatio = chargeRatio * 1.5f;

                    Projectile.Opacity =
                        MathHelper.Lerp(0f, 0.4f, phaseRatio);

                    spinRate =
                        MathHelper.Lerp(10f, 8f, phaseRatio);
                }
                else
                {
                    float phaseRatio =
                        (chargeRatio - 0.66f) * 3f;

                    Projectile.Opacity =
                        MathHelper.Lerp(0.4f, 1f, phaseRatio);

                    spinRate =
                        MathHelper.Lerp(8f, 1.55f, phaseRatio);
                }
            }
            else
            {
                Projectile.scale = MaxBeamScale;
                Projectile.Opacity = 1f;

                beamSpread = 0f;
                spinRate = 1.55f;

                beamStartSidewaysOffset = 3.25f;
                beamStartForwardsOffset = -17f;
            }

            float deviationAngle =
                (
                    hostPrism.localAI[0] +
                    beamIdOffset * spinRate
                ) /
                (
                    spinRate *
                    NeuroLastPrismHoldout.NumBeams
                ) *
                MathHelper.TwoPi;

            Vector2 unitRot =
                Vector2.UnitY.RotatedBy(deviationAngle);

            Vector2 yVector =
                new Vector2(2.5f, beamStartSidewaysOffset);

            float hostAngle =
                hostPrism.velocity.ToRotation();

            Vector2 beamSpanVector =
                (unitRot * yVector).RotatedBy(hostAngle);

            float sinusoidYOffset =
                unitRot.Y *
                MathHelper.Pi /
                NeuroLastPrismHoldout.NumBeams *
                beamSpread;

            Projectile.Center = hostPrism.Center;

            Projectile.position += hostDirection * 16f;
            Projectile.position += hostDirection * beamStartForwardsOffset;
            Projectile.position += beamSpanVector;

            Projectile.velocity =
                hostDirection.RotatedBy(sinusoidYOffset);

            Projectile.velocity =
                SafeNormalize(Projectile.velocity, hostDirection);

            Projectile.rotation =
                Projectile.velocity.ToRotation();

            bool ignoreTiles =
                hostPrism
                    .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                    .IgnoreTilesForNeuroMk4;

            Projectile
                .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                .IgnoreTilesForNeuroMk4 = ignoreTiles;

            float hitscanBeamLength =
                PerformBeamHitscan(ignoreTiles);

            BeamLength =
                MathHelper.Lerp(
                    BeamLength,
                    hitscanBeamLength,
                    BeamLengthChangeFactor
                );

            if (chargeRatio >= VisualEffectThreshold)
            {
                ProduceBeamDust(GetOuterBeamColor());
            }

            DelegateMethods.v3_1 =
                GetOuterBeamColor().ToVector3() *
                BeamLightBrightness *
                chargeRatio;

            Utils.PlotTileLine(
                Projectile.Center,
                Projectile.Center + Projectile.velocity * BeamLength,
                Projectile.width * Projectile.scale,
                DelegateMethods.CastLight
            );

            TryDamageOwnerWithBeam();

            Projectile.timeLeft = 2;
        }

        private Projectile GetHostPrism()
        {
            int hostIndex = HostPrismIndex;

            if (hostIndex < 0 || hostIndex >= Main.maxProjectiles)
            {
                return null;
            }

            Projectile hostPrism = Main.projectile[hostIndex];

            if (
                hostPrism == null ||
                !hostPrism.active ||
                hostPrism.type != ModContent.ProjectileType<NeuroLastPrismHoldout>()
            )
            {
                return null;
            }

            return hostPrism;
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

            if (canDamageOwner)
            {
                Projectile.friendly = false;
                Projectile.hostile = false;

                Projectile
                    .GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>()
                    .CanDamageOwner = true;

                Projectile
                    .GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>()
                    .KillOnOwnerHit = false;

                return;
            }

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

        private float PerformBeamHitscan(bool ignoreTiles)
        {
            if (ignoreTiles)
            {
                return MaxBeamLength;
            }

            float[] samples = new float[NumSamplePoints];

            Collision.LaserScan(
                Projectile.Center,
                Projectile.velocity,
                BeamTileCollisionWidth * Projectile.scale,
                MaxBeamLength,
                samples
            );

            float averageLength = 0f;

            for (int i = 0; i < samples.Length; i++)
            {
                averageLength += samples[i];
            }

            averageLength /= samples.Length;

            return averageLength;
        }

        public override bool? Colliding(
            Rectangle projHitbox,
            Rectangle targetHitbox
        )
        {
            float collisionPoint = 0f;

            Vector2 beamEnd =
                Projectile.Center + Projectile.velocity * BeamLength;

            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center,
                beamEnd,
                BeamHitboxCollisionWidth * Projectile.scale,
                ref collisionPoint
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

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero || BeamLength <= 0f)
            {
                return false;
            }

            Texture2D texture =
                TextureAssets.Projectile[Type].Value;

            Vector2 drawScale =
                new Vector2(Projectile.scale);

            float visualBeamLength =
                BeamLength - 14.5f * Projectile.scale * Projectile.scale;

            if (visualBeamLength <= 0f)
            {
                return false;
            }

            Vector2 startPosition =
                Projectile.Center.Floor()
                + Projectile.velocity * Projectile.scale * 10.5f
                - Main.screenPosition;

            Vector2 endPosition =
                startPosition + Projectile.velocity * visualBeamLength;

            DrawBeam(
                Main.spriteBatch,
                texture,
                startPosition,
                endPosition,
                drawScale,
                GetOuterBeamColor() *
                OuterBeamOpacityMultiplier *
                Projectile.Opacity
            );

            DrawBeam(
                Main.spriteBatch,
                texture,
                startPosition,
                endPosition,
                drawScale * InnerBeamScaleMultiplier,
                Color.White *
                InnerBeamOpacityMultiplier *
                Projectile.Opacity
            );

            return false;
        }

        private void DrawBeam(
            SpriteBatch spriteBatch,
            Texture2D texture,
            Vector2 startPosition,
            Vector2 endPosition,
            Vector2 drawScale,
            Color beamColor
        )
        {
            Utils.LaserLineFraming lineFraming =
                new Utils.LaserLineFraming(
                    DelegateMethods.RainbowLaserDraw
                );

            DelegateMethods.c_1 = beamColor;

            Utils.DrawLaser(
                spriteBatch,
                texture,
                startPosition,
                endPosition,
                drawScale,
                lineFraming
            );
        }

        private Color GetOuterBeamColor()
        {
            float hue =
                BeamID / (float)NeuroLastPrismHoldout.NumBeams;

            Color color =
                Main.hslToRgb(
                    hue,
                    BeamColorSaturation,
                    BeamColorLightness
                );

            color.A = 64;

            return color;
        }

        private void ProduceBeamDust(Color beamColor)
        {
            Vector2 endPosition =
                Projectile.Center +
                Projectile.velocity *
                (BeamLength - 14.5f * Projectile.scale);

            float angle =
                Projectile.rotation +
                (Main.rand.NextBool() ? 1f : -1f) *
                MathHelper.PiOver2;

            Vector2 dustVelocity =
                angle.ToRotationVector2() *
                Main.rand.NextFloat(1f, 1.8f);

            Dust dust =
                Dust.NewDustDirect(
                    endPosition,
                    0,
                    0,
                    DustID.FireworkFountain_Pink,
                    dustVelocity.X,
                    dustVelocity.Y,
                    0,
                    beamColor,
                    Main.rand.NextFloat(0.7f, 1.1f)
                );

            dust.noGravity = true;
            dust.color = beamColor;
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