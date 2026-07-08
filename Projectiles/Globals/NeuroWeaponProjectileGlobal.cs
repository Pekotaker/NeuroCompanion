using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Neuro.Weapons.Firing;

namespace NeuroCompanion.Projectiles.Globals
{
    public class NeuroWeaponProjectileGlobal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsNeuroWeaponProjectile { get; private set; }

        public bool UseSkyDrawLayer { get; set; }

        public int FrameOverride { get; set; } = -1;

        public float ScaleOverride { get; set; } = -1f;

        private bool useNeuroStellarTuneAi;
        private Vector2 neuroStellarTuneStartPosition;
        private float neuroStellarTuneTimer;
        private float neuroStellarTuneWaveDirection = 1f;

        public override bool PreAI(Projectile projectile)
        {
            if (
                useNeuroStellarTuneAi &&
                projectile.type == ProjectileID.SparkleGuitar
            )
            {
                RunNeuroStellarTuneAi(projectile);
                return false;
            }

            return true;
        }

        public override void PostAI(Projectile projectile)
        {
            if (FrameOverride >= 0)
            {
                projectile.frame = FrameOverride;
            }

            if (ScaleOverride > 0f)
            {
                projectile.scale = ScaleOverride;
            }
        }

        public override void DrawBehind(
            Projectile projectile,
            int index,
            List<int> behindNPCsAndTiles,
            List<int> behindNPCs,
            List<int> behindProjectiles,
            List<int> overPlayers,
            List<int> overWiresUI
        )
        {
            if (!UseSkyDrawLayer)
            {
                return;
            }

            overPlayers.Add(index);
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            NeuroWeaponProjectileSpawnContext context =
                NeuroWeaponProjectileSpawnContext.Current;

            if (context == null)
            {
                return;
            }

            if (context.Owner == null || projectile.owner != context.Owner.whoAmI)
            {
                return;
            }

            ApplyNeuroWeaponBehavior(projectile, context);
        }

        private void ApplyNeuroWeaponBehavior(
            Projectile projectile,
            NeuroWeaponProjectileSpawnContext context
        )
        {
            IsNeuroWeaponProjectile = true;

            projectile.DamageType = DamageClass.Magic;

            TryInitializeNeuroStellarTuneAi(projectile);

            projectile.damage = context.Damage;
            projectile.originalDamage = context.Damage;

            if (context.IsEvil)
            {
                ApplyEvilBehavior(projectile, context);
            }
            else
            {
                ApplyFriendlyBehavior(projectile, context);
            }

            projectile.netUpdate = true;
        }

        private void TryInitializeNeuroStellarTuneAi(Projectile projectile)
        {
            if (projectile.type != ProjectileID.SparkleGuitar)
            {
                return;
            }

            useNeuroStellarTuneAi = true;
            neuroStellarTuneStartPosition = projectile.Center;
            neuroStellarTuneTimer = 0f;

            neuroStellarTuneWaveDirection = Main.rand.NextBool() ? 1f : -1f;

            projectile.tileCollide = false;
            projectile.timeLeft = 90;
            projectile.localAI[0] = 0f;
        }

        private void RunNeuroStellarTuneAi(Projectile projectile)
        {
            const float DurationTicks = 90f;
            const float WaveAmplitude = 96f;

            Vector2 oldCenter = projectile.Center;

            neuroStellarTuneTimer++;

            if (neuroStellarTuneTimer >= DurationTicks)
            {
                projectile.Kill();
                return;
            }

            float progress = neuroStellarTuneTimer / DurationTicks;

            Vector2 targetPosition = new Vector2(
                projectile.ai[0],
                projectile.ai[1]
            );

            Vector2 pathDirection =
                targetPosition - neuroStellarTuneStartPosition;

            if (pathDirection.LengthSquared() <= 0.01f)
            {
                pathDirection = projectile.velocity;

                if (pathDirection.LengthSquared() <= 0.01f)
                {
                    pathDirection = Vector2.UnitX;
                }
            }

            pathDirection.Normalize();

            Vector2 perpendicular = new Vector2(
                -pathDirection.Y,
                pathDirection.X
            );

            float smoothProgress =
                progress * progress * (3f - 2f * progress);

            Vector2 basePoint = Vector2.Lerp(
                neuroStellarTuneStartPosition,
                targetPosition,
                smoothProgress
            );

            // One S-shape only:
            // start centered -> curve one way -> cross center -> curve the other way -> end centered.
            float sCurveOffset =
                (float)System.Math.Sin(progress * MathHelper.TwoPi);

            projectile.Center =
                basePoint +
                perpendicular *
                sCurveOffset *
                WaveAmplitude *
                neuroStellarTuneWaveDirection;

            projectile.velocity = projectile.Center - oldCenter;

            if (projectile.velocity.LengthSquared() > 0.01f)
            {
                projectile.rotation = projectile.velocity.ToRotation();
            }
            else
            {
                projectile.rotation += 0.1f;
            }

            projectile.localAI[0] = neuroStellarTuneTimer;
            projectile.netUpdate = true;
        }

        private static void ApplyFriendlyBehavior(
            Projectile projectile,
            NeuroWeaponProjectileSpawnContext context
        )
        {
            projectile.friendly = true;
            projectile.hostile = false;

            projectile.CritChance = context.CritChance;

            if (
                context.NeuroPlayer != null &&
                context.NeuroPlayer.NeuroStaffCanDetectThroughBlocks
            )
            {
                projectile.tileCollide = false;

                projectile
                    .GetGlobalProjectile<NeuroMk4ProjectileGlobal>()
                    .IgnoreTilesForNeuroMk4 = true;
            }
        }

        private static void ApplyEvilBehavior(
            Projectile projectile,
            NeuroWeaponProjectileSpawnContext context
        )
        {
            projectile.friendly = false;
            projectile.hostile = false;

            EvilNeuroPlayerAttackGlobal evilGlobal =
                projectile.GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>();

            evilGlobal.CanDamageOwner = true;
            evilGlobal.KillOnOwnerHit = context.KillOnOwnerHit;
        }
    }
}