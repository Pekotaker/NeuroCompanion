using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles.Visuals
{
    public class NeuroLifeDrainStreamDust : ModDust
    {
        private const float ArrivalDistance = 6f;

        private const float StartingScale = 0.85f;
        private const float EndingScale = 0.55f;

        private const int StartingAlpha = 20;
        private const int EndingAlpha = 140;

        private const int VanillaDustCellSize = 10;
        private const int VanillaDustSheetWidth = 1000;
        private const int VanillaDustVariantCount = 3;
        private const int VanillaDustDrawFrameSize = 8;

        // Reuse the vanilla Life Drain particle sprite,
        // but replace its movement behavior completely.
        public override string Texture => null;

        public override void OnSpawn(Dust dust)
        {
            SetLifeDrainDustFrame(dust);

            dust.noGravity = true;
            dust.velocity = Vector2.Zero;

            dust.scale = StartingScale;
            dust.alpha = StartingAlpha;
        }

        public override bool Update(Dust dust)
        {
            if (
                dust.customData
                is not NeuroLifeDrainStreamState state
            )
            {
                dust.active = false;
                return false;
            }

            if (!state.TryGetDestination(out Vector2 destination))
            {
                dust.active = false;
                return false;
            }

            state.RemainingLifetimeTicks--;

            if (state.RemainingLifetimeTicks <= 0)
            {
                dust.active = false;
                return false;
            }

            Vector2 toDestination =
                destination - dust.position;

            float distance =
                toDestination.Length();

            if (distance <= ArrivalDistance)
            {
                dust.active = false;
                return false;
            }

            Vector2 direction =
                toDestination / distance;

            float movementDistance =
                MathHelper.Min(
                    state.SpeedPixelsPerTick,
                    distance
                );

            Vector2 movement =
                direction * movementDistance;

            dust.position += movement;
            dust.velocity = movement;
            dust.rotation = movement.ToRotation();

            dust.scale -= 0.0025f;
            dust.alpha =
                System.Math.Min(
                    200,
                    dust.alpha + 1
                );

            if (dust.scale <= 0.2f)
            {
                dust.active = false;
            }

            return false;
        }

        public override Color? GetAlpha(
            Dust dust,
            Color lightColor
        )
        {
            float opacity =
                1f - dust.alpha / 255f;

            return new Color(
                255,
                45,
                45
            ) * opacity;
        }

        private static void SetLifeDrainDustFrame(Dust dust)
        {
            int vanillaDustType = DustID.LifeDrain;

            int frameX =
                vanillaDustType *
                VanillaDustCellSize %
                VanillaDustSheetWidth;

            int frameY =
                vanillaDustType *
                VanillaDustCellSize /
                VanillaDustSheetWidth *
                VanillaDustCellSize *
                VanillaDustVariantCount;

            frameY +=
                Main.rand.Next(VanillaDustVariantCount) *
                VanillaDustCellSize;

            dust.frame = new Rectangle(
                frameX,
                frameY,
                VanillaDustDrawFrameSize,
                VanillaDustDrawFrameSize
            );
        }
    }

    internal sealed class NeuroLifeDrainStreamState
    {
        public Entity Destination { get; }

        public float SpeedPixelsPerTick { get; }

        public int RemainingLifetimeTicks { get; set; }

        public NeuroLifeDrainStreamState(
            Entity destination,
            float speedPixelsPerTick,
            int maximumLifetimeTicks
        )
        {
            Destination = destination;
            SpeedPixelsPerTick = speedPixelsPerTick;
            RemainingLifetimeTicks = maximumLifetimeTicks;
        }

        public bool TryGetDestination(
            out Vector2 destination
        )
        {
            switch (Destination)
            {
                case Player player
                    when player.active &&
                         !player.dead:

                    destination = player.Center;
                    return true;

                case Projectile projectile
                    when projectile.active:

                    destination = projectile.Center;
                    return true;

                default:
                    destination = Vector2.Zero;
                    return false;
            }
        }
    }
}