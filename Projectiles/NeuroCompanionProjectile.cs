using Microsoft.Xna.Framework;
using NeuroCompanion.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles
{
    public class NeuroCompanionProjectile : ModProjectile
    {
        // Temporary texture: vanilla Baby Slime projectile.
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.BabySlime}";

        public override void SetStaticDefaults()
        {
            // Baby Slime uses an animation sheet.
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.BabySlime];

            // Required for pet/minion contact-damage behavior.
            Main.projPet[Projectile.type] = true;

            // Standard minion behavior flags.
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
            // ai[0] is our simple state:
            // 0 = idle/following player
            // 1 = attacking enemy
            //
            // This prevents the companion from accidentally damaging enemies
            // while calmly floating near the player.
            if (Projectile.ai[0] == 1f)
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

            float oldState = Projectile.ai[0];

            NPC target = FindTarget(player);

            if (target != null)
            {
                Projectile.ai[0] = 1f;
                AttackTarget(target);
            }
            else
            {
                Projectile.ai[0] = 0f;
                FollowPlayer(player);
            }

            if (Projectile.ai[0] != oldState)
            {
                Projectile.netUpdate = true;
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

            // If the player manually cancels the buff, remove the minion.
            Projectile.Kill();
            return false;
        }

        private NPC FindTarget(Player player)
        {
            // First, respect the player's manually selected minion target.
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

            // This prevents the minion from targeting enemies through solid walls.
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

        private void AttackTarget(NPC target)
        {
            Vector2 directionToTarget = target.Center - Projectile.Center;
            float distanceToTarget = directionToTarget.Length();

            directionToTarget = directionToTarget.SafeNormalize(Vector2.Zero);

            float speed = 10f;
            float inertia = 18f;

            if (distanceToTarget > 300f)
            {
                speed = 14f;
                inertia = 24f;
            }

            Vector2 desiredVelocity = directionToTarget * speed;

            Projectile.velocity =
                (Projectile.velocity * (inertia - 1f) + desiredVelocity) / inertia;

            FaceMovementDirection();

            // A slightly more aggressive rotation while attacking.
            Projectile.rotation = Projectile.velocity.X * 0.08f;
        }

        private void FollowPlayer(Player player)
        {
            // Idle position: slightly behind and above the player.
            Vector2 idlePosition = player.Center + new Vector2(-48f * player.direction, -60f);

            float distanceToIdlePosition = Vector2.Distance(Projectile.Center, idlePosition);

            // Emergency teleport if the minion gets extremely far away.
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
            if (Projectile.ai[0] == 1f)
            {
                // More dust while attacking.
                if (Main.rand.NextBool(4))
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
                // Less dust while idle.
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