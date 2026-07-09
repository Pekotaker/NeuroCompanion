using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;

using NeuroCompanion.Projectiles.Globals;

namespace NeuroCompanion.Projectiles.Weapons.SpiritFlame
{
    public class NeuroSpiritFlameProjectile : ModProjectile
    {
        public override string Texture =>
            "Terraria/Images/Projectile_" + ProjectileID.SpiritFlame;

        private const int AnimationFrameDurationTicks = 5;
        private const int ProjectileWidth = 18;
        private const int ProjectileHeight = 18;

        private const int LifetimeTicks = 180;

        private const float TargetSearchRange = 25f * 16f;
        private const float HomingSpeed = 9f;
        private const float HomingTurnStrength = 0.12f;

        private const float HoverHorizontalDistance = 6f;
        private const float HoverVerticalDistance = 4f;
        private const float HoverHorizontalSpeed = 0.05f;
        private const float HoverVerticalSpeed = 0.07f;

        private const int DustChanceDenominator = 3;
        private const float DustScale = 1.1f;

        private const int ImpactDustCount = 34;

        private const float ImpactDustSpeedMin = 1.5f;
        private const float ImpactDustSpeedMax = 7f;

        private const float ImpactDustScaleMin = 0.8f;
        private const float ImpactDustScaleMax = 1.9f;

        private const int ImpactSmokeDustCount = 10;

        private const float OwnerHomingSpeed = 10f;
        private const float OwnerHomingTurnStrength = 0.16f;
        private const float OwnerHitboxPadding = 8f;

        private bool OwnerAttackMode =>
            Projectile.ai[2] >= 0.5f;

        private static readonly Color[] ImpactPurpleColors =
        {
            new Color(85, 35, 135),
            new Color(125, 45, 190),
            new Color(170, 85, 240),
            new Color(205, 145, 255),
            new Color(110, 100, 125),
            new Color(145, 145, 155)
        };

        private bool HasImpacted
        {
            get => Projectile.localAI[2] >= 0.5f;
            set => Projectile.localAI[2] = value ? 1f : 0f;
        }

        private Vector2 AnchorPosition
        {
            get => new Vector2(Projectile.ai[0], Projectile.ai[1]);
            set
            {
                Projectile.ai[0] = value.X;
                Projectile.ai[1] = value.Y;
            }
        }

        private bool HasStartedHoming
        {
            get => Projectile.localAI[0] >= 0.5f;
            set => Projectile.localAI[0] = value ? 1f : 0f;
        }

        private float Age
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = ProjectileWidth;
            Projectile.height = ProjectileHeight;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = LifetimeTicks;
            Projectile.aiStyle = 0;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] =
                Main.projFrames[ProjectileID.SpiritFlame];
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            Age++;

            if (AnchorPosition == Vector2.Zero)
            {
                AnchorPosition = Projectile.Center;
            }

            if (OwnerAttackMode)
            {
                DisableGenericOwnerDamageBehavior();
                HomeTowardOwner();
                UpdateAnimation();
                EmitEffects();
                return;
            }

            NPC target = FindTarget();

            if (target != null)
            {
                HasStartedHoming = true;
                HomeTowardTarget(target);
            }
            else
            {
                if (HasStartedHoming)
                {
                    HasStartedHoming = false;
                    AnchorPosition = Projectile.Center;
                    Projectile.velocity = Vector2.Zero;
                }

                HoverAroundAnchor();
            }

            UpdateAnimation();

            EmitEffects();
        }

        public override void OnHitNPC(
            NPC target,
            NPC.HitInfo hit,
            int damageDone
        )
        {
            HasImpacted = true;
            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            if (!HasImpacted)
            {
                return;
            }

            SoundEngine.PlaySound(
                SoundID.Item14,
                Projectile.Center
            );

            EmitImpactExplosion();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture =
                TextureAssets.Projectile[ProjectileID.SpiritFlame].Value;

            int frameCount = Main.projFrames[Type];

            if (frameCount <= 0)
            {
                frameCount = 1;
            }

            int frameHeight =
                texture.Height / frameCount;

            Rectangle sourceRectangle =
                new Rectangle(
                    0,
                    frameHeight * Projectile.frame,
                    texture.Width,
                    frameHeight
                );

            Vector2 origin =
                sourceRectangle.Size() * 0.5f;

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRectangle,
                lightColor,
                0f,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            return false;
        }

        private void DisableGenericOwnerDamageBehavior()
        {
            EvilNeuroPlayerAttackGlobal evilGlobal =
                Projectile.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            evilGlobal.CanDamageOwner = false;
            evilGlobal.KillOnOwnerHit = false;
        }

        private void HomeTowardOwner()
        {
            if (
                Projectile.owner < 0 ||
                Projectile.owner >= Main.maxPlayers
            )
            {
                Projectile.Kill();
                return;
            }

            Player owner = Main.player[Projectile.owner];

            if (
                owner == null ||
                !owner.active ||
                owner.dead
            )
            {
                Projectile.Kill();
                return;
            }

            if (ProjectileHitsOwner(owner))
            {
                DamageOwnerAndExplode(owner);
                return;
            }

            Vector2 toOwner =
                owner.Center - Projectile.Center;

            if (toOwner.LengthSquared() <= 0.01f)
            {
                DamageOwnerAndExplode(owner);
                return;
            }

            Vector2 desiredVelocity =
                toOwner.SafeNormalize(Vector2.UnitY) * OwnerHomingSpeed;

            Projectile.velocity =
                Vector2.Lerp(
                    Projectile.velocity,
                    desiredVelocity,
                    OwnerHomingTurnStrength
                );

            if (Projectile.velocity.LengthSquared() <= 0.01f)
            {
                Projectile.velocity = desiredVelocity;
            }

            Projectile.Center += Projectile.velocity;
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
            if (HasImpacted)
            {
                return;
            }

            HasImpacted = true;

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
                    GetImpactPurpleColor(),
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
                    GetImpactPurpleColor(),
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

        private static Color GetImpactPurpleColor()
        {
            return ImpactPurpleColors[
                Main.rand.Next(ImpactPurpleColors.Length)
            ];
        }

        private void UpdateAnimation()
        {
            Projectile.frameCounter++;

            if (Projectile.frameCounter < AnimationFrameDurationTicks)
            {
                return;
            }

            Projectile.frameCounter = 0;

            int frameCount = Main.projFrames[Type];

            if (frameCount <= 0)
            {
                frameCount = 1;
            }

            Projectile.frame++;

            if (Projectile.frame >= frameCount)
            {
                Projectile.frame = 0;
            }
        }

        private void HoverAroundAnchor()
        {
            float horizontalOffset =
                (float)System.Math.Sin(
                    Age * HoverHorizontalSpeed + Projectile.whoAmI
                ) * HoverHorizontalDistance;

            float verticalOffset =
                (float)System.Math.Cos(
                    Age * HoverVerticalSpeed + Projectile.whoAmI
                ) * HoverVerticalDistance;

            Projectile.Center =
                AnchorPosition +
                new Vector2(
                    horizontalOffset,
                    verticalOffset
                );

            Projectile.velocity = Vector2.Zero;
        }

        private void HomeTowardTarget(NPC target)
        {
            Vector2 toTarget =
                target.Center - Projectile.Center;

            if (toTarget.LengthSquared() <= 0.01f)
            {
                return;
            }

            Vector2 desiredVelocity =
                toTarget.SafeNormalize(Vector2.UnitY) * HomingSpeed;

            Projectile.velocity =
                Vector2.Lerp(
                    Projectile.velocity,
                    desiredVelocity,
                    HomingTurnStrength
                );

            if (Projectile.velocity.LengthSquared() <= 0.01f)
            {
                Projectile.velocity = desiredVelocity;
            }

            Projectile.Center += Projectile.velocity;
        }

        private NPC FindTarget()
        {
            NPC bestTarget = null;
            float bestDistanceSquared =
                TargetSearchRange * TargetSearchRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (
                    npc == null ||
                    !npc.CanBeChasedBy(Projectile)
                )
                {
                    continue;
                }

                float distanceSquared =
                    Vector2.DistanceSquared(
                        Projectile.Center,
                        npc.Center
                    );

                if (distanceSquared >= bestDistanceSquared)
                {
                    continue;
                }

                if (
                    !Collision.CanHitLine(
                        Projectile.position,
                        Projectile.width,
                        Projectile.height,
                        npc.position,
                        npc.width,
                        npc.height
                    )
                )
                {
                    continue;
                }

                bestTarget = npc;
                bestDistanceSquared = distanceSquared;
            }

            return bestTarget;
        }

        private void EmitEffects()
        {
            Lighting.AddLight(
                Projectile.Center,
                0.45f,
                0.1f,
                0.65f
            );

            if (!Main.rand.NextBool(DustChanceDenominator))
            {
                return;
            }

            Dust dust =
                Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Shadowflame,
                    0f,
                    0f,
                    100,
                    default,
                    DustScale
                );

            dust.noGravity = true;
            dust.velocity *= 0.25f;
        }
    }
}