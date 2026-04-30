using Microsoft.Xna.Framework;
using NeuroCompanion.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles
{
    public class NeuroCompanionProjectile : ModProjectile
    {
        private const float StateIdle = 0f;
        private const float StateApproachingTarget = 1f;
        private const float StateDashing = 2f;
        private const float StateRecovering = 3f;

        // Temporary texture: vanilla Baby Slime projectile.
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.BabySlime}";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.BabySlime];

            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 20;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.minion = true;
            Projectile.minionSlots = 1f;

            Projectile.DamageType = DamageClass.Summon;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.netImportant = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override bool? CanDamage()
        {
            // Only deal contact damage during the dash.
            // This prevents the companion from passively grinding enemies while hovering.
            if (Projectile.ai[0] == StateDashing)
            {
                return null;
            }

            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!CheckActive(player))
            {
                return;
            }

            if (Projectile.ai[0] == StateDashing)
            {
                ContinueDash();
            }
            else if (Projectile.ai[0] == StateRecovering)
            {
                RecoverAfterDash(player);
            }
            else
            {
                NPC target = FindTarget(player);

                if (target != null)
                {
                    ApproachTarget(target);
                }
                else
                {
                    Projectile.ai[0] = StateIdle;
                    Projectile.ai[1] = 0f;
                    FollowPlayer(player);
                }
            }

            AnimateProjectile();
            CreateVisualEffects();
        }

        private bool CheckActive(Player player)
        {
            if (!player.active || player.dead)
            {
                player.ClearBuff(ModContent.BuffType<NeuroCompanionBuff>());
                return false;
            }

            if (player.HasBuff(ModContent.BuffType<NeuroCompanionBuff>()))
            {
                Projectile.timeLeft = 2;
                return true;
            }

            Projectile.Kill();
            return false;
        }

        private NPC FindTarget(Player player)
        {
            // Respect the player's manually selected minion target first.
            if (player.HasMinionAttackTargetNPC)
            {
                NPC selectedTarget = Main.npc[player.MinionAttackTargetNPC];

                if (IsValidTarget(selectedTarget, player, 1000f))
                {
                    return selectedTarget;
                }
            }

            NPC closestTarget = null;
            float closestDistance = 700f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!IsValidTarget(npc, player, 900f))
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

        private bool IsValidTarget(NPC npc, Player player, float maxDistanceFromPlayer)
        {
            if (!npc.active)
            {
                return false;
            }

            if (!npc.CanBeChasedBy(this))
            {
                return false;
            }

            float distanceFromPlayer = Vector2.Distance(player.Center, npc.Center);

            if (distanceFromPlayer > maxDistanceFromPlayer)
            {
                return false;
            }

            bool hasLineOfSight = Collision.CanHitLine(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                npc.position,
                npc.width,
                npc.height
            );

            return hasLineOfSight;
        }

        private void ApproachTarget(NPC target)
        {
            Projectile.ai[0] = StateApproachingTarget;
            Projectile.ai[1]++;

            Vector2 targetPosition = target.Center + new Vector2(0f, -40f);
            Vector2 directionToTarget = targetPosition - Projectile.Center;
            float distanceToTarget = directionToTarget.Length();

            directionToTarget = directionToTarget.SafeNormalize(Vector2.Zero);

            float speed = 10f;
            float inertia = 18f;

            if (distanceToTarget > 350f)
            {
                speed = 14f;
                inertia = 24f;
            }

            Vector2 desiredVelocity = directionToTarget * speed;

            Projectile.velocity =
                (Projectile.velocity * (inertia - 1f) + desiredVelocity) / inertia;

            FaceMovementDirection();

            Projectile.rotation = Projectile.velocity.X * 0.06f;

            // Dash if close enough, or if we have been approaching for about one second.
            if (distanceToTarget < 140f || Projectile.ai[1] >= 50f)
            {
                StartDash(target);
            }
        }

        private void StartDash(NPC target)
        {
            Projectile.ai[0] = StateDashing;
            Projectile.ai[1] = 0f;

            Vector2 dashDirection = target.Center - Projectile.Center;
            dashDirection = dashDirection.SafeNormalize(Vector2.UnitX);

            Projectile.velocity = dashDirection * 13f;

            Projectile.netUpdate = true;

            FaceMovementDirection();
        }

        private void ContinueDash()
        {
            Projectile.ai[1]++;

            Projectile.velocity *= 0.88f;

            FaceMovementDirection();

            Projectile.rotation += Projectile.spriteDirection * 0.35f;

            if (Projectile.ai[1] >= 8f)
            {
                Projectile.ai[0] = StateRecovering;
                Projectile.ai[1] = 0f;

  
                Projectile.velocity *= 0.45f;

                Projectile.netUpdate = true;
            }
        }

        private void RecoverAfterDash(Player player)
        {
            Projectile.ai[1]++;

            // Slow down after the dash.
            Projectile.velocity *= 0.82f;

            FaceMovementDirection();

            Projectile.rotation = Projectile.velocity.X * 0.05f;

            // Recovery lasts about 1/3 second.
            if (Projectile.ai[1] >= 20f)
            {
                Projectile.ai[0] = StateIdle;
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }

            // If it drifted too far during recovery, gently pull it back toward the player.
            float distanceFromPlayer = Vector2.Distance(Projectile.Center, player.Center);

            if (distanceFromPlayer > 500f)
            {
                FollowPlayer(player);
            }
        }

        private void FollowPlayer(Player player)
        {
            Vector2 idlePosition = player.Center + new Vector2(-48f * player.direction, -60f);

            float distanceToIdlePosition = Vector2.Distance(Projectile.Center, idlePosition);

            if (distanceToIdlePosition > 2000f)
            {
                Projectile.Center = player.Center;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
                return;
            }

            float speed = 8f;
            float inertia = 40f;

            if (distanceToIdlePosition > 600f)
            {
                speed = 16f;
                inertia = 60f;
            }

            if (distanceToIdlePosition > 20f)
            {
                Vector2 directionToIdlePosition = idlePosition - Projectile.Center;
                directionToIdlePosition = directionToIdlePosition.SafeNormalize(Vector2.Zero);

                Vector2 desiredVelocity = directionToIdlePosition * speed;

                Projectile.velocity =
                    (Projectile.velocity * (inertia - 1f) + desiredVelocity) / inertia;
            }
            else
            {
                Projectile.velocity *= 0.9f;
            }

            FaceMovementDirection();

            Projectile.rotation = Projectile.velocity.X * 0.05f;
        }

        private void FaceMovementDirection()
        {
            if (Projectile.velocity.X > 0.1f)
            {
                Projectile.spriteDirection = 1;
            }
            else if (Projectile.velocity.X < -0.1f)
            {
                Projectile.spriteDirection = -1;
            }
        }

        private void AnimateProjectile()
        {
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        private void CreateVisualEffects()
        {
            if (Projectile.ai[0] == StateDashing)
            {
                // Dash trail.
                if (Main.rand.NextBool(2))
                {
                    Dust.NewDust(
                        Projectile.position,
                        Projectile.width,
                        Projectile.height,
                        DustID.GemSapphire
                    );
                }
            }
            else if (Projectile.ai[0] == StateApproachingTarget)
            {
                if (Main.rand.NextBool(5))
                {
                    Dust.NewDust(
                        Projectile.position,
                        Projectile.width,
                        Projectile.height,
                        DustID.GemSapphire
                    );
                }
            }
            else
            {
                if (Main.rand.NextBool(12))
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
}