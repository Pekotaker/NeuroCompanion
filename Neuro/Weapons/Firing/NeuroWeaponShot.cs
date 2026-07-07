using Microsoft.Xna.Framework;

using Terraria.ID;

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
        public bool ForceVisible { get; }
        public bool UseSkyDrawLayer { get; }
        public int FrameOverride { get; }
        public int VisualProjectileType { get; }
        public int VisualFrameOverride { get; }
        public int VisualStyle { get; }

        public NeuroWeaponShot(
            int projectileType,
            Vector2 position,
            Vector2 velocity,
            float ai0 = 0f,
            float ai1 = 0f,
            float scale = 1f,
            int delayTicks = 0,
            bool forceVisible = false,
            bool useSkyDrawLayer = false,
            int frameOverride = -1,
            int visualProjectileType = ProjectileID.None,
            int visualFrameOverride = -1,
            int visualStyle = NeuroWeaponVisualStyle.None
        )
        {
            ProjectileType = projectileType;
            Position = position;
            Velocity = velocity;
            Ai0 = ai0;
            Ai1 = ai1;
            Scale = scale;
            DelayTicks = delayTicks;
            ForceVisible = forceVisible;
            UseSkyDrawLayer = useSkyDrawLayer;
            FrameOverride = frameOverride;
            VisualProjectileType = visualProjectileType;
            VisualFrameOverride = visualFrameOverride;
            VisualStyle = visualStyle;
        }
    }
}