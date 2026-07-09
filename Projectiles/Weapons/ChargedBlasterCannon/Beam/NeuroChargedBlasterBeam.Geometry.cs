using Terraria;
using Microsoft.Xna.Framework;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Beam
{
    public partial class NeuroChargedBlasterBeam
    {
        private float PerformBeamHitscan(bool ignoreTiles)
        {
            if (ignoreTiles)
            {
                return MaxBeamLength;
            }

            float[] samples = new float[3];

            Collision.LaserScan(
                Projectile.Center,
                Projectile.velocity,
                BeamTileCollisionWidth * Projectile.scale,
                MaxBeamLength,
                samples
            );

            float averageLength = 0f;

            for (int i = 0; i < samples.Length; i++)
            {
                averageLength += samples[i];
            }

            averageLength /= samples.Length;

            return averageLength;
        }

        public override bool? Colliding(
            Rectangle projHitbox,
            Rectangle targetHitbox
        )
        {
            float collisionPoint = 0f;

            Vector2 beamEnd =
                Projectile.Center + Projectile.velocity * BeamLength;

            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center,
                beamEnd,
                BeamHitboxCollisionWidth * Projectile.scale,
                ref collisionPoint
            );
        }
    }
}