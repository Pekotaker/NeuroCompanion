using Microsoft.Xna.Framework;
using NeuroCompanion.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles
{
    public class NeuroCompanionProjectile : ModProjectile
    {
        private enum CompanionState
        {
            Idle = 0,
            Attacking = 1
        }

        private const int ProjectileWidth = 26;
        private const int ProjectileHeight = 20;

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

        private const float TargetSearchRange = 900f;
        private const float ManualTargetSearchRange = 1000f;
        private const float ProjectileTargetSearchRange = 700f;

        private const float ShotSpeed = 11f;
        private const float ShotSpawnOffset = 20f;

        private const float IdleRotationFactor = 0.05f;
        private const float AttackRotationFactor = 0.06f;

        private const float SlowdownWhenArrived = 0.9f;
        private const float MinimumFacingVelocity = 0.1f;

        // Temporary texture: vanilla Baby Slime projectile.
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
            Projectile.timeLeft = 18000;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.netImportant = true;
        }

        public override bool MinionContactDamage()
        {
            // Neuro's body should not damage by touching enemies.
            // Damage comes from NeuroCompanionShot instead.
            return false;
        }

        public override bool? CanDamage()
        {
            // Completely disable contact damage from the companion body.
            return false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!ShouldStayAlive(owner))
            {
                return;
            }

            NPC target = FindTarget(owner);

            if (target == null)
            {
                State = CompanionState.Idle;
                ShootTimer = 0f;
                FollowOwner(owner);
            }
            else
            {
                State = CompanionState.Attacking;
                HoverNearOwnerForCombat(owner, target);
                ShootAtTargetWhenReady(target);
            }

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

        private NPC FindTarget(Player owner)
        {
            NPC manualTarget = FindManualTarget(owner);

            if (manualTarget != null)
            {
                return manualTarget;
            }

            return FindClosestTarget(owner);
        }

        private NPC FindManualTarget(Player owner)
        {
            if (!owner.HasMinionAttackTargetNPC)
            {
                return null;
            }

            NPC selectedTarget = Main.npc[owner.MinionAttackTargetNPC];

            if (IsValidTarget(selectedTarget, owner, ManualTargetSearchRange))
            {
                return selectedTarget;
            }

            return null;
        }

        private NPC FindClosestTarget(Player owner)
        {
            NPC closestTarget = null;
            float closestDistance = ProjectileTargetSearchRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!IsValidTarget(npc, owner, TargetSearchRange))
                {
                    continue;
                }

                float distanceToProjectile = Vector2.Distance(Projectile.Center, npc.Center);

                if (distanceToProjectile < closestDistance)
                {
                    closestDistance = distanceToProjectile;
                    closestTarget = npc;
                }
            }

            return closestTarget;
        }

        private bool IsValidTarget(NPC npc, Player owner, float maxDistanceFromOwner)
        {
            if (!npc.active)
            {
                return false;
            }

            if (!npc.CanBeChasedBy(this))
            {
                return false;
            }

            float distanceFromOwner = Vector2.Distance(owner.Center, npc.Center);

            if (distanceFromOwner > maxDistanceFromOwner)
            {
                return false;
            }

            return HasLineOfSightTo(npc);
        }

        private bool HasLineOfSightTo(NPC npc)
        {
            return Collision.CanHitLine(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                npc.position,
                npc.width,
                npc.height
            );
        }

        private void FollowOwner(Player owner)
        {
            Vector2 idlePosition = GetIdlePosition(owner);
            MoveToward(idlePosition, IdleMoveSpeed, IdleInertia, FarMoveSpeed, FarInertia);
            RotateForMovement(IdleRotationFactor);
        }

        private Vector2 GetIdlePosition(Player owner)
        {
            return owner.Center + new Vector2(-IdleOffsetX * owner.direction, IdleOffsetY);
        }

        private void HoverNearOwnerForCombat(Player owner, NPC target)
        {
            Vector2 combatPosition = GetCombatPosition(owner, target);
            MoveToward(combatPosition, IdleMoveSpeed, IdleInertia, FarMoveSpeed, FarInertia);

            FaceTarget(target);
            RotateForMovement(AttackRotationFactor);
        }

        private Vector2 GetCombatPosition(Player owner, NPC target)
        {
            float sideFacingTarget = target.Center.X >= owner.Center.X ? 1f : -1f;

            return owner.Center + new Vector2(CombatOffsetX * sideFacingTarget, CombatOffsetY);
        }

        private void MoveToward(
            Vector2 destination,
            float normalSpeed,
            float normalInertia,
            float farSpeed,
            float farInertia
        )
        {
            float distance = Vector2.Distance(Projectile.Center, destination);

            if (distance > TeleportDistance)
            {
                TeleportTo(destination);
                return;
            }

            float speed = distance > FarDistance ? farSpeed : normalSpeed;
            float inertia = distance > FarDistance ? farInertia : normalInertia;

            if (distance > ArriveDistance)
            {
                Vector2 direction = destination - Projectile.Center;
                direction = direction.SafeNormalize(Vector2.Zero);

                Vector2 desiredVelocity = direction * speed;

                Projectile.velocity =
                    (Projectile.velocity * (inertia - 1f) + desiredVelocity) / inertia;
            }
            else
            {
                Projectile.velocity *= SlowdownWhenArrived;
            }

            FaceMovementDirection();
        }

        private void TeleportTo(Vector2 destination)
        {
            Projectile.Center = destination;
            Projectile.velocity = Vector2.Zero;
            Projectile.netUpdate = true;
        }

        private void ShootAtTargetWhenReady(NPC target)
        {
            ShootTimer++;

            if (ShootTimer < ShootCooldownTicks)
            {
                return;
            }

            if (!HasLineOfSightTo(target))
            {
                return;
            }

            ShootTimer = 0f;

            // Projectile-spawning should only happen on the projectile owner's client.
            if (Projectile.owner != Main.myPlayer)
            {
                return;
            }

            Vector2 shotDirection = target.Center - Projectile.Center;
            shotDirection = shotDirection.SafeNormalize(Vector2.UnitX * Projectile.spriteDirection);

            Vector2 shotVelocity = shotDirection * ShotSpeed;
            Vector2 shotPosition = Projectile.Center + shotDirection * ShotSpawnOffset;

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                shotPosition,
                shotVelocity,
                ModContent.ProjectileType<NeuroCompanionShot>(),
                Projectile.damage,
                Projectile.knockBack,
                Projectile.owner
            );

            Projectile.netUpdate = true;
        }

        private void FaceMovementDirection()
        {
            if (Projectile.velocity.X > MinimumFacingVelocity)
            {
                Projectile.spriteDirection = 1;
            }
            else if (Projectile.velocity.X < -MinimumFacingVelocity)
            {
                Projectile.spriteDirection = -1;
            }
        }

        private void FaceTarget(NPC target)
        {
            Projectile.spriteDirection = target.Center.X >= Projectile.Center.X ? 1 : -1;
        }

        private void RotateForMovement(float rotationFactor)
        {
            Projectile.rotation = Projectile.velocity.X * rotationFactor;
        }

        private void Animate()
        {
            Projectile.frameCounter++;

            if (Projectile.frameCounter < AnimationFrameDurationTicks)
            {
                return;
            }

            Projectile.frameCounter = 0;
            Projectile.frame++;

            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
        }

        private void CreateVisualEffects()
        {
            int dustChance = State == CompanionState.Attacking
                ? AttackDustChance
                : IdleDustChance;

            if (Main.rand.NextBool(dustChance))
            {
                Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.GemSapphire
                );
            }
        }
    }
}