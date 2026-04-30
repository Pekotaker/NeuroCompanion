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

            // Required for minion contact damage logic.
            Main.projPet[Projectile.type] = true;

            // Makes the minion work with vanilla minion slot replacement behavior.
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;

            // Allows right-click minion targeting with compatible summon weapons.
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 20;

            Projectile.friendly = true;
            Projectile.hostile = false;

            // This makes Terraria treat it as a minion.
            Projectile.minion = true;

            // This makes it use one summon slot.
            Projectile.minionSlots = 1f;

            Projectile.DamageType = DamageClass.Summon;

            // Infinite penetration: do not disappear after touching one enemy.
            Projectile.penetrate = -1;

            // Minions should stay alive as long as their buff is maintained.
            Projectile.timeLeft = 18000;

            // Minions should follow the player through terrain for now.
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            // Syncs this projectile better in multiplayer / joining worlds.
            Projectile.netImportant = true;

            // Prevents the same minion from damaging the same NPC too rapidly.
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override bool MinionContactDamage()
        {
            // The minion can hurt enemies by touching them.
            // Later we will replace this with better attack behavior.
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!CheckActive(player))
            {
                return;
            }

            FollowPlayer(player);
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

            // If far away, move faster.
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
                // Slow down when already near the idle position.
                Projectile.velocity *= 0.9f;
            }

            // Face movement direction.
            if (Projectile.velocity.X > 0.1f)
            {
                Projectile.spriteDirection = 1;
            }
            else if (Projectile.velocity.X < -0.1f)
            {
                Projectile.spriteDirection = -1;
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;
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
            if (Main.rand.NextBool(10))
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