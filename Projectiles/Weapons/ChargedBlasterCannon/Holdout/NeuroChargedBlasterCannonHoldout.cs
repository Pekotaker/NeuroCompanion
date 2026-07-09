using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Neuro.Weapons.Firing;
using NeuroCompanion.Projectiles.Companion;
using NeuroCompanion.Projectiles.Globals;

using NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Orb;
using NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Beam;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Holdout
{
    public class NeuroChargedBlasterCannonHoldout : ModProjectile
    {
        public override string Texture =>
            "Terraria/Images/Projectile_" + ProjectileID.ChargedBlasterCannon;

        public const int DurationTicks = 300;
        public const int RefreshGraceTicks = 12;

        private const int PhaseTwoStartTicks = 80;
        private const int BeamStartTicks = 220;

        private const int BeamDurationTicks = 300;
        private const int CycleResetTicks = BeamStartTicks + BeamDurationTicks;

        private const int RapidOrbCooldownTicks = 8;
        private const int HeavyOrbCooldownTicks = 24;

        private const float HoldoutDistanceFromNeuro = 34f;
        private const float AimResponsiveness = 0.18f;
        private const float MuzzleDistance = 34f;

        private const float SmallOrbDamageMultiplier = 0.45f;
        private const float HeavyOrbDamageMultiplier = 0.85f;

        private const int HeavyOrbHumIntervalTicks = 6;

        private const float InitialPewSoundVolume = 1f;
        private const float InitialPewSoundPitch = 0f;

        private const float SmallOrbHumSoundVolume = 0.25f;
        private const float SmallOrbHumSoundPitch = -0.35f;

        private const float HeavyOrbHumSoundVolume = 0.45f;
        private const float HeavyOrbHumSoundPitch = -0.2f;

        private const float BeamStartHumSoundVolume = 1.05f;
        private const float BeamStartHumSoundPitch = -0.05f;

        private const int SmallOrbPhaseCannonDustIntervalTicks = 6;
        private const int HeavyOrbPhaseCannonDustIntervalTicks = 4;
        private const int BeamPhaseCannonDustIntervalTicks = 2;

        private const int SmallOrbPhaseCannonDustCount = 1;
        private const int HeavyOrbPhaseCannonDustCount = 2;
        private const int BeamPhaseCannonDustCount = 2;

        private const float SmallOrbPhaseCannonDustScale = 0.45f;
        private const float HeavyOrbPhaseCannonDustScale = 0.55f;
        private const float BeamPhaseCannonDustScale = 0.8f;

        private const int CannonDustType = DustID.Electric;

        private const float CannonDustOriginOffset = 0f;

        private const float SmallOrbPhaseSuctionRadiusMin = 2f;
        private const float SmallOrbPhaseSuctionRadiusMax = 8f;

        private const float HeavyOrbPhaseSuctionRadiusMin = 2f;
        private const float HeavyOrbPhaseSuctionRadiusMax = 8f;

        private const float SmallOrbPhaseSuctionSpeed = 5f;
        private const float HeavyOrbPhaseSuctionSpeed = 6.5f;

        private const float SuctionDustRandomVelocity = 0.45f;

        private const float BeamPhaseCannonDustSpeed = 12f;
        private const float BeamPhaseCannonDustMinSpeedMultiplier = 0.8f;
        private const float BeamPhaseCannonDustConeRadians = 0.32f;
        private const float BeamPhaseDustSpawnBackOffset = 0f;
        private const float BeamPhaseDustSpawnSideSpread = 8f;

        private bool initialized;
        private bool beamSpawned;
        private bool initialPewSoundPlayed;

        private int rapidOrbTimer;
        private int heavyOrbTimer;
        private int heavyHumTimer;

        private int cannonDustTimer;

        private Vector2 fallbackAnchorPosition;

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

        public void RefreshFromContext(
            Vector2 targetPosition,
            NeuroWeaponProjectileSpawnContext context
        )
        {
            RefreshTarget(targetPosition);

            if (context != null)
            {
                ApplySpawnContext(context);
            }

            Projectile.netUpdate = true;
        }

        private void RefreshTarget(Vector2 targetPosition)
        {
            Projectile.ai[0] = targetPosition.X;
            Projectile.ai[1] = targetPosition.Y;

            if (RemainingLifeTicks < RefreshGraceTicks)
            {
                RemainingLifeTicks = RefreshGraceTicks;
            }
        }

        private void ApplySpawnContext(
            NeuroWeaponProjectileSpawnContext context
        )
        {
            Projectile.damage = context.Damage;
            Projectile.originalDamage = context.Damage;
            Projectile.CritChance = context.CritChance;

            OwnerDamageEnabled = context.IsEvil;

            Projectile.friendly = !OwnerDamageEnabled;
            Projectile.hostile = false;

            Projectile
                .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                .IgnoreTilesForNeuroMk4 =
                    context.NeuroPlayer != null &&
                    context.NeuroPlayer.NeuroStaffCanDetectThroughBlocks;

            PreventGenericOwnerDamageOnHoldout();
        }

        private void PreventGenericOwnerDamageOnHoldout()
        {
            EvilNeuroPlayerAttackGlobal evilGlobal =
                Projectile.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            evilGlobal.CanDamageOwner = false;
            evilGlobal.KillOnOwnerHit = false;
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
                ChargeTicks >= BeamStartTicks ? 2 :
                ChargeTicks >= PhaseTwoStartTicks ? 3 :
                4;

            if (Projectile.frameCounter < framesPerAnimationUpdate)
            {
                return;
            }

            Projectile.frameCounter = 0;

            int frameCount = Main.projFrames[Type];

            if (frameCount <= 0)
            {
                return;
            }

            Projectile.frame++;

            if (Projectile.frame >= frameCount)
            {
                Projectile.frame = 0;
            }
        }

        private void FireCurrentChargePhase()
        {
            if (Projectile.owner != Main.myPlayer)
            {
                return;
            }

            if (SingleShotOnly)
            {
                FireSingleOrbAndFinish();
                return;
            }

            if (ChargeTicks >= CycleResetTicks)
            {
                ResetChargeCycle();
                return;
            }

            if (ChargeTicks < PhaseTwoStartTicks)
            {
                rapidOrbTimer++;

                if (rapidOrbTimer >= RapidOrbCooldownTicks)
                {
                    rapidOrbTimer = 0;
                    SpawnOrb(isHeavy: false);
                }

                return;
            }

            if (ChargeTicks < BeamStartTicks)
            {
                heavyHumTimer++;

                if (heavyHumTimer >= HeavyOrbHumIntervalTicks)
                {
                    heavyHumTimer = 0;
                    PlayHeavyOrbHumSound();
                }

                heavyOrbTimer++;

                if (heavyOrbTimer >= HeavyOrbCooldownTicks)
                {
                    heavyOrbTimer = 0;
                    SpawnOrb(isHeavy: true);
                }

                return;
            }

            if (!beamSpawned)
            {
                beamSpawned = true;
                PlayBeamStartSound();
                SpawnBeam();
            }
        }

        private void PlayInitialPewSound()
        {
            if (initialPewSoundPlayed)
            {
                return;
            }

            initialPewSoundPlayed = true;

            SoundEngine.PlaySound(
                SoundID.Item75 with
                {
                    Volume = InitialPewSoundVolume,
                    Pitch = InitialPewSoundPitch
                },
                Projectile.Center
            );
        }

        private void PlaySmallOrbHumSound()
        {
            SoundEngine.PlaySound(
                SoundID.Item15 with
                {
                    Volume = SmallOrbHumSoundVolume,
                    Pitch = SmallOrbHumSoundPitch
                },
                Projectile.Center
            );
        }

        private void PlayHeavyOrbHumSound()
        {
            SoundEngine.PlaySound(
                SoundID.Item15 with
                {
                    Volume = HeavyOrbHumSoundVolume,
                    Pitch = HeavyOrbHumSoundPitch
                },
                Projectile.Center
            );
        }

        private void PlayBeamStartSound()
        {
            SoundEngine.PlaySound(
                SoundID.Item15 with
                {
                    Volume = BeamStartHumSoundVolume,
                    Pitch = BeamStartHumSoundPitch
                },
                Projectile.Center
            );
        }

        private static Color GetCannonDustColor()
        {
            return ChargedBlasterCannonEffectColor.GetColor();
        }

        private void FireSingleOrbAndFinish()
        {
            if (ChargeTicks == 1f)
            {
                SpawnOrb(isHeavy: false);
            }

            if (ChargeTicks >= 2f)
            {
                Projectile.Kill();
            }
        }

        private void ResetChargeCycle()
        {
            ChargeTicks = 0f;
            rapidOrbTimer = 0;
            heavyOrbTimer = 0;
            heavyHumTimer = 0;
            cannonDustTimer = 0;

            beamSpawned = false;
            initialPewSoundPlayed = false;

            Projectile.netUpdate = true;
        }

        private void SpawnOrb(bool isHeavy)
        {
            Vector2 direction =
                SafeNormalize(Projectile.velocity, Vector2.UnitX);

            Vector2 spawnPosition =
                Projectile.Center + direction * MuzzleDistance;

            float speed = isHeavy ? 12f : 18f;

            int damage =
                (int)(
                    Projectile.damage *
                    (
                        isHeavy
                            ? HeavyOrbDamageMultiplier
                            : SmallOrbDamageMultiplier
                    )
                );

            if (damage < 1)
            {
                damage = 1;
            }

            int projectileIndex =
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPosition,
                    direction * speed,
                    ModContent.ProjectileType<NeuroChargedBlasterOrb>(),
                    damage,
                    Projectile.knockBack,
                    Projectile.owner,
                    isHeavy ? 1f : 0f
                );

            ApplyChildProjectileState(
                projectileIndex,
                useGenericOwnerDamage: true
            );

            if (isHeavy)
            {
                return;
            }

            if (!initialPewSoundPlayed)
            {
                PlayInitialPewSound();
            }
            else
            {
                PlaySmallOrbHumSound();
            }
        }

        private void EmitCannonPhaseDust()
        {
            if (SingleShotOnly)
            {
                return;
            }

            int intervalTicks;
            int dustCount;
            float dustScale;

            bool beamPhase =
                ChargeTicks >= BeamStartTicks;

            if (beamPhase)
            {
                intervalTicks = BeamPhaseCannonDustIntervalTicks;
                dustCount = BeamPhaseCannonDustCount;
                dustScale = BeamPhaseCannonDustScale;
            }
            else if (ChargeTicks >= PhaseTwoStartTicks)
            {
                intervalTicks = HeavyOrbPhaseCannonDustIntervalTicks;
                dustCount = HeavyOrbPhaseCannonDustCount;
                dustScale = HeavyOrbPhaseCannonDustScale;
            }
            else
            {
                intervalTicks = SmallOrbPhaseCannonDustIntervalTicks;
                dustCount = SmallOrbPhaseCannonDustCount;
                dustScale = SmallOrbPhaseCannonDustScale;
            }

            cannonDustTimer++;

            if (cannonDustTimer < intervalTicks)
            {
                return;
            }

            cannonDustTimer = 0;

            Vector2 direction =
                SafeNormalize(Projectile.velocity, Vector2.UnitX);

            Vector2 perpendicular =
                direction.RotatedBy(MathHelper.PiOver2);

            Vector2 cannonPosition =
                Projectile.Center + direction * CannonDustOriginOffset;

            for (int i = 0; i < dustCount; i++)
            {
                if (beamPhase)
                {
                    EmitBeamPhaseCannonDust(
                        cannonPosition,
                        direction,
                        perpendicular,
                        dustScale
                    );
                }
                else
                {
                    EmitOrbPhaseSuctionDust(
                        cannonPosition,
                        dustScale
                    );
                }
            }
        }

        private void EmitBeamPhaseCannonDust(
            Vector2 cannonPosition,
            Vector2 beamDirection,
            Vector2 perpendicular,
            float dustScale
        )
        {
            Vector2 dustPosition =
                cannonPosition -
                beamDirection * BeamPhaseDustSpawnBackOffset +
                perpendicular *
                Main.rand.NextFloat(
                    -BeamPhaseDustSpawnSideSpread,
                    BeamPhaseDustSpawnSideSpread
                );

            Vector2 dustVelocity =
                beamDirection
                    .RotatedBy(
                        Main.rand.NextFloat(
                            -BeamPhaseCannonDustConeRadians,
                            BeamPhaseCannonDustConeRadians
                        )
                    ) *
                Main.rand.NextFloat(
                    BeamPhaseCannonDustSpeed *
                    BeamPhaseCannonDustMinSpeedMultiplier,
                    BeamPhaseCannonDustSpeed
                );

            Dust dust =
                Dust.NewDustPerfect(
                    dustPosition,
                    CannonDustType,
                    dustVelocity,
                    100,
                    GetCannonDustColor(),
                    dustScale
                );

            dust.noGravity = true;
        }

        private void EmitOrbPhaseSuctionDust(
            Vector2 cannonPosition,
            float dustScale
        )
        {
            bool heavyOrbPhase =
                ChargeTicks >= PhaseTwoStartTicks;

            float radiusMin =
                heavyOrbPhase
                    ? HeavyOrbPhaseSuctionRadiusMin
                    : SmallOrbPhaseSuctionRadiusMin;

            float radiusMax =
                heavyOrbPhase
                    ? HeavyOrbPhaseSuctionRadiusMax
                    : SmallOrbPhaseSuctionRadiusMax;

            float suctionSpeed =
                heavyOrbPhase
                    ? HeavyOrbPhaseSuctionSpeed
                    : SmallOrbPhaseSuctionSpeed;

            Vector2 fromDirection =
                Main.rand.NextVector2CircularEdge(1f, 1f);

            Vector2 dustPosition =
                cannonPosition +
                fromDirection *
                Main.rand.NextFloat(radiusMin, radiusMax);

            Vector2 toCannon =
                (cannonPosition - dustPosition)
                .SafeNormalize(Vector2.UnitX);

            Vector2 dustVelocity =
                toCannon *
                Main.rand.NextFloat(
                    suctionSpeed * 0.65f,
                    suctionSpeed
                ) +
                Main.rand.NextVector2Circular(
                    SuctionDustRandomVelocity,
                    SuctionDustRandomVelocity
                );

            Dust dust =
                Dust.NewDustPerfect(
                    dustPosition,
                    CannonDustType,
                    dustVelocity,
                    100,
                    GetCannonDustColor(),
                    dustScale
                );

            dust.noGravity = true;
        }

        private void SpawnBeam()
        {
            Vector2 direction =
                SafeNormalize(Projectile.velocity, Vector2.UnitX);

            int projectileIndex =
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center + direction * MuzzleDistance,
                    direction,
                    ModContent.ProjectileType<NeuroChargedBlasterBeam>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner,
                    Projectile.whoAmI
                );

            ApplyChildProjectileState(
                projectileIndex,
                useGenericOwnerDamage: false
            );
        }

        private void ApplyChildProjectileState(
            int projectileIndex,
            bool useGenericOwnerDamage
        )
        {
            if (
                projectileIndex < 0 ||
                projectileIndex >= Main.maxProjectiles
            )
            {
                return;
            }

            Projectile child = Main.projectile[projectileIndex];

            if (child == null || !child.active)
            {
                return;
            }

            child.CritChance = Projectile.CritChance;
            child.originalDamage = child.damage;

            child
                .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                .IgnoreTilesForNeuroMk4 =
                    Projectile
                        .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                        .IgnoreTilesForNeuroMk4;

            EvilNeuroPlayerAttackGlobal evilGlobal =
                child.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            if (OwnerDamageEnabled)
            {
                child.friendly = false;
                child.hostile = false;

                evilGlobal.CanDamageOwner = useGenericOwnerDamage;
                evilGlobal.KillOnOwnerHit = false;
            }
            else
            {
                child.friendly = true;
                child.hostile = false;

                evilGlobal.CanDamageOwner = false;
                evilGlobal.KillOnOwnerHit = false;
            }
        }

        private Vector2 GetAnchorPosition(Vector2 aimDirection)
        {
            Projectile companion = FindNeuroCompanionProjectile();

            if (companion != null)
            {
                fallbackAnchorPosition =
                    companion.Center +
                    aimDirection * HoldoutDistanceFromNeuro;
            }

            return fallbackAnchorPosition;
        }

        private Projectile FindNeuroCompanionProjectile()
        {
            int companionType =
                ModContent.ProjectileType<NeuroCompanionProjectile>();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];

                if (
                    projectile != null &&
                    projectile.active &&
                    projectile.owner == Projectile.owner &&
                    projectile.type == companionType
                )
                {
                    return projectile;
                }
            }

            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects =
                Projectile.spriteDirection == -1
                    ? SpriteEffects.FlipVertically
                    : SpriteEffects.None;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

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