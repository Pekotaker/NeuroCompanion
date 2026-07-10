using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Globals;
using NeuroCompanion.Projectiles.Weapons.MedusaHead.Ray;

namespace NeuroCompanion.Projectiles.Weapons.MedusaHead.Holdout
{
    public partial class NeuroMedusaHeadHoldout
    {
        private void TryFireBurst()
        {
            if (singleShotBurstFired)
            {
                return;
            }

            attackTimer++;

            if (attackTimer < AttackIntervalTicks)
            {
                return;
            }

            if (OwnerDamageEnabled)
            {
                TryFireOwnerBurst();
                return;
            }

            NPC[] targets = FindTargets();

            if (targets.Length <= 0)
            {
                return;
            }

            FireTargetBurst(targets);
        }

        private void TryFireOwnerBurst()
        {
            Player owner = GetOwnerTarget();

            if (owner == null)
            {
                return;
            }

            BeginBurst();

            SpawnRay(owner.Center);
            SpawnRandomRays();

            FinishBurst();
        }

        private void FireTargetBurst(NPC[] targets)
        {
            BeginBurst();

            for (int i = 0; i < targets.Length; i++)
            {
                NPC target = targets[i];

                if (
                    target == null ||
                    !target.active
                )
                {
                    continue;
                }

                SpawnRay(target.Center);
            }

            SpawnRandomRays();

            FinishBurst();
        }

        private void BeginBurst()
        {
            attackTimer = 0;

            PlayAttackSoundOnce();
        }

        private void SpawnRandomRays()
        {
            int rayCount =
                Main.rand.Next(
                    MinimumRandomRayCount,
                    MaximumRandomRayCount + 1
                );

            for (int i = 0; i < rayCount; i++)
            {
                Vector2 direction =
                    Main.rand.NextVector2CircularEdge(1f, 1f);

                if (
                    direction.LengthSquared() <= 0.01f ||
                    direction.HasNaNs()
                )
                {
                    direction = Vector2.UnitX;
                }

                direction.Normalize();

                float rayLength =
                    Main.rand.NextFloat(
                        MinimumRandomRayLength,
                        MaximumRandomRayLength
                    );

                Vector2 endpoint =
                    Projectile.Center +
                    direction * rayLength;

                SpawnRay(endpoint);
            }
        }

        private void SpawnRay(Vector2 endpoint)
        {
            Vector2 rayVector =
                endpoint - Projectile.Center;

            if (
                rayVector.LengthSquared() <= 0.01f ||
                rayVector.HasNaNs()
            )
            {
                return;
            }

            int rayIndex =
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    rayVector,
                    ModContent.ProjectileType<NeuroMedusaHeadRay>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner,
                    OwnerDamageEnabled ? 1f : 0f,
                    IgnoreBlocks ? 1f : 0f
                );

            ApplyRayState(rayIndex);
        }

        private void ApplyRayState(int rayIndex)
        {
            if (
                rayIndex < 0 ||
                rayIndex >= Main.maxProjectiles
            )
            {
                return;
            }

            Projectile ray = Main.projectile[rayIndex];

            if (
                ray == null ||
                !ray.active
            )
            {
                return;
            }

            ray.CritChance = Projectile.CritChance;
            ray.originalDamage = ray.damage;

            ray.friendly = !OwnerDamageEnabled;
            ray.hostile = false;

            ray
                .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                .IgnoreTilesForNeuroMk4 = IgnoreBlocks;

            // The custom Medusa ray handles owner collision itself.
            EvilNeuroPlayerAttackGlobal evilGlobal =
                ray.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            evilGlobal.CanDamageOwner = false;
            evilGlobal.KillOnOwnerHit = false;

            ray.netUpdate = true;
        }

        private void FinishBurst()
        {
            if (!SingleShotOnly)
            {
                return;
            }

            singleShotBurstFired = true;

            if (remainingLifeTicks > PostBurstLifetimeTicks)
            {
                remainingLifeTicks = PostBurstLifetimeTicks;
            }
        }

        private void PlayAttackSoundOnce()
        {
            if (attackSoundPlayed)
            {
                return;
            }

            attackSoundPlayed = true;

            SoundEngine.PlaySound(
                SoundID.NPCDeath17,
                Projectile.Center
            );
        }
    }
}