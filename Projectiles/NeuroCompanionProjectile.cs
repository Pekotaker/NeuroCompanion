using Microsoft.Xna.Framework;
using NeuroCompanion.Buffs;
using NeuroCompanion.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles
{
    public partial class NeuroCompanionProjectile : ModProjectile
    {
        private enum CompanionState
        {
            Idle = 0,
            Attacking = 1
        }

        private const int ProjectileWidth = 26;
        private const int ProjectileHeight = 20;

        private const int MinionLifetimeTicks = 18000;
        private const int BuffRefreshTimeTicks = 2;
        private const int AnimationFrameDurationTicks = 8;

        private const int IdleDustChance = 12;
        private const int AttackDustChance = 5;

        private const int ShootCooldownTicks = 50;

        private const float MinionSlotsUsed = 1f;

        private const float IdleOffsetX = 48f;
        private const float IdleOffsetY = -60f;

        private const float CombatOffsetX = 64f;
        private const float CombatOffsetY = -70f;

        private const float TeleportDistance = 2000f;
        private const float FarDistance = 600f;
        private const float ArriveDistance = 20f;

        private const float IdleMoveSpeed = 8f;
        private const float FarMoveSpeed = 16f;

        private const float IdleInertia = 40f;
        private const float FarInertia = 60f;

        private const float TargetSearchRangeFromPlayer = 700f;
        private const float ManualTargetSearchRangeFromPlayer = 1200f;

        private const float ShotSpeed = 11f;
        private const float ShotSpawnOffset = 20f;
        private const float MinimumShotDirectionLengthSquared = 1f;

        private const float IdleRotationFactor = 0.05f;
        private const float AttackRotationFactor = 0.06f;

        private const float SlowdownWhenArrived = 0.9f;
        private const float MinimumFacingVelocity = 0.1f;

        private const float TyphoonSoundVolume = 1f;
        private const float TyphoonSoundPitchVariance = 0.1f;

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.BabySlime}";

        private CompanionState State
        {
            get => (CompanionState)(int)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private float ShootTimer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.BabySlime];

            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = ProjectileWidth;
            Projectile.height = ProjectileHeight;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.minion = true;
            Projectile.minionSlots = MinionSlotsUsed;

            Projectile.DamageType = DamageClass.Summon;

            Projectile.penetrate = -1;
            Projectile.timeLeft = MinionLifetimeTicks;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.netImportant = true;
        }

        public override bool MinionContactDamage()
        {
            return false;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!ShouldStayAlive(owner))
            {
                return;
            }

            NeuroCompanionPlayer neuroPlayer =
                owner.GetModPlayer<NeuroCompanionPlayer>();

            ApplyRecallCommand(owner, neuroPlayer);
            RunCommandedBehavior(owner, neuroPlayer);

            Animate();
            CreateVisualEffects();
        }

        private bool ShouldStayAlive(Player owner)
        {
            if (!owner.active || owner.dead)
            {
                owner.ClearBuff(ModContent.BuffType<NeuroCompanionBuff>());
                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<NeuroCompanionBuff>()))
            {
                Projectile.timeLeft = BuffRefreshTimeTicks;
                return true;
            }

            Projectile.Kill();
            return false;
        }
    }
}