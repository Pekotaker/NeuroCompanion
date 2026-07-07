using Microsoft.Xna.Framework;

namespace NeuroCompanion.Neuro.Weapons.Firing
{
    public readonly struct NeuroWeaponShot
    {
        public int ProjectileType { get; }
        public Vector2 Position { get; }
        public Vector2 Velocity { get; }
        public float Ai0 { get; }
        public float Ai1 { get; }
        public float Scale { get; }
        public int DelayTicks { get; }

        public NeuroWeaponShot(
            int projectileType,
            Vector2 position,
            Vector2 velocity,
            float ai0 = 0f,
            float ai1 = 0f,
            float scale = 1f,
            int delayTicks = 0
        )
        {
            ProjectileType = projectileType;
            Position = position;
            Velocity = velocity;
            Ai0 = ai0;
            Ai1 = ai1;
            Scale = scale;
            DelayTicks = delayTicks;
        }
    }
}