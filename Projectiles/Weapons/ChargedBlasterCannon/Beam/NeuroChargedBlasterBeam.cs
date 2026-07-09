using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeuroCompanion.Projectiles.Globals;
using NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Holdout;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Beam
{
    public class NeuroChargedBlasterBeam : ModProjectile
    {
        public override string Texture =>
            "Terraria/Images/Projectile_" + ProjectileID.ChargedBlasterLaser;

        private const float MaxBeamLength = 2400f;
        private const float BeamTileCollisionWidth = 1f;
        private const float BeamHitboxCollisionWidth = 14f;
        private const float BeamLengthChangeFactor = 0.75f;
        private const float BeamStartDistanceFromCannon = 24f;
        private const float BeamDrawStartOffset = 0f;

        private const float BeamDrawScale = 1.1f;
        private const float BeamLightBrightness = 2.2f;

        private const byte BeamColorRed = 150;
        private const byte BeamColorGreen = 230;
        private const byte BeamColorBlue = 255;
        private const byte BeamColorAlpha = 255;

        private const int BeamDustChanceDenominator = 2;
        private const float BeamDustScale = 1.45f;

        private const float BeamDustMinDistance = 24f;
        private const int OwnerHitCooldownTicks = 30;


        private int ownerHitCooldown;

        private int HostCannonIndex =>
            (int)Projectile.ai[0];

        private float BeamLength
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.alpha = 255;
            Projectile.timeLeft = 2;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            Projectile hostProjectile = GetHostCannon();

            if (
                hostProjectile == null ||
                hostProjectile.ModProjectile
                    is not NeuroChargedBlasterCannonHoldout hostCannon
            )
            {
                Projectile.Kill();
                return;
            }

            if (!hostCannon.BeamPhaseActive)
            {
                Projectile.Kill();
                return;
            }

            if (ownerHitCooldown > 0)
            {
                ownerHitCooldown--;
            }

            Vector2 direction =
                SafeNormalize(hostProjectile.velocity, Vector2.UnitX);

            Projectile.Center =
                hostProjectile.Center + direction * BeamStartDistanceFromCannon;

            Projectile.velocity = direction;
            Projectile.rotation = direction.ToRotation();

            ApplyStateFromHost(
                hostProjectile,
                hostCannon
            );

            bool ignoreTiles =
                hostProjectile
                    .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                    .IgnoreTilesForNeuroMk4;

            float targetBeamLength =
                PerformBeamHitscan(ignoreTiles);

            BeamLength =
                MathHelper.Lerp(
                    BeamLength,
                    targetBeamLength,
                    BeamLengthChangeFactor
                );

            ProduceVisualEffects();

            TryDamageOwnerWithBeam(hostCannon);

            Projectile.timeLeft = 2;
        }

        private Projectile GetHostCannon()
        {
            if (
                HostCannonIndex < 0 ||
                HostCannonIndex >= Main.maxProjectiles
            )
            {
                return null;
            }

            Projectile hostProjectile =
                Main.projectile[HostCannonIndex];

            if (
                hostProjectile == null ||
                !hostProjectile.active ||
                hostProjectile.type !=
                ModContent.ProjectileType<NeuroChargedBlasterCannonHoldout>()
            )
            {
                return null;
            }

            return hostProjectile;
        }

        private void ApplyStateFromHost(
            Projectile hostProjectile,
            NeuroChargedBlasterCannonHoldout hostCannon
        )
        {
            Projectile.damage = hostProjectile.damage;
            Projectile.originalDamage = hostProjectile.damage;
            Projectile.CritChance = hostProjectile.CritChance;

            Projectile.friendly = !hostCannon.OwnerDamageEnabled;
            Projectile.hostile = false;

            Projectile
                .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                .IgnoreTilesForNeuroMk4 =
                    hostProjectile
                        .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                        .IgnoreTilesForNeuroMk4;

            EvilNeuroPlayerAttackGlobal evilGlobal =
                Projectile.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            // Owner damage for this beam is handled by TryDamageOwnerWithBeam.
            // Do not let the generic Evil Neuro hitbox logic hit the player.
            evilGlobal.CanDamageOwner = false;
            evilGlobal.KillOnOwnerHit = false;
        }

        private float PerformBeamHitscan(bool ignoreTiles)
        {
            if (ignoreTiles)
            {
                return MaxBeamLength;
            }

            float[] samples = new float[3];

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

        private void ProduceVisualEffects()
        {
            Color beamColor = GetBeamColor();

            DelegateMethods.v3_1 =
                beamColor.ToVector3() *
                BeamLightBrightness;

            Utils.PlotTileLine(
                Projectile.Center,
                Projectile.Center + Projectile.velocity * BeamLength,
                Projectile.width * Projectile.scale,
                DelegateMethods.CastLight
            );

            if (Main.rand.NextBool(BeamDustChanceDenominator))
            {
                Vector2 dustPosition =
                    Projectile.Center +
                    Projectile.velocity *
                    Main.rand.NextFloat(BeamDustMinDistance, BeamLength);

                Dust dust =
                    Dust.NewDustDirect(
                        dustPosition,
                        0,
                        0,
                        DustID.BlueTorch,
                        0f,
                        0f,
                        100,
                        beamColor,
                        BeamDustScale
                    );

                dust.noGravity = true;
            }
        }

        private void TryDamageOwnerWithBeam(
            NeuroChargedBlasterCannonHoldout hostCannon
        )
        {
            if (!hostCannon.OwnerDamageEnabled)
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
                    $"{owner.name} was blasted by Evil Neuro."
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

            Vector2 startPosition =
                Projectile.Center.Floor()
                + Projectile.velocity * BeamDrawStartOffset
                - Main.screenPosition;

            Vector2 endPosition =
                startPosition + Projectile.velocity * BeamLength;

            Utils.LaserLineFraming lineFraming =
                new Utils.LaserLineFraming(
                    DelegateMethods.RainbowLaserDraw
                );

            DelegateMethods.c_1 =
                GetBeamColor();

            Utils.DrawLaser(
                Main.spriteBatch,
                texture,
                startPosition,
                endPosition,
                new Vector2(BeamDrawScale),
                lineFraming
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

        private static Color GetBeamColor()
        {
            return new Color(
                BeamColorRed,
                BeamColorGreen,
                BeamColorBlue,
                BeamColorAlpha
            );
        }
    }
}