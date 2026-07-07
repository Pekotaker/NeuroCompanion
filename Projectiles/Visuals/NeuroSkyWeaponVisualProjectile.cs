using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Neuro.Weapons.Firing;

namespace NeuroCompanion.Projectiles.Visuals
{
    public class NeuroSkyWeaponVisualProjectile : ModProjectile
    {
        private const int DefaultLifetimeTicks = 90;
        private const float MinimumVelocityLengthSquared = 0.01f;

        private const float LunarFlareStreakLength = 52f;
        private const float LunarFlareStreakWidth = 8f;

        public override string Texture => "Terraria/Images/Projectile_1";

        private int ParentProjectileIndex => (int)Projectile.ai[0];
        private int TextureProjectileType => (int)Projectile.ai[1];
        private int TextureFrameOverride => (int)Projectile.localAI[0];
        private int VisualStyle => (int)Projectile.localAI[1];

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.damage = 0;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = DefaultLifetimeTicks;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Projectile parent = GetParentProjectile();

            if (parent == null)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = parent.Center;
            Projectile.velocity = parent.velocity;

            // Do not blindly copy parent.scale.
            // Meteor Staff meteors reset their real projectile scale to 0 before drawing.
            // If we copy that, the visual overlay becomes invisible too.
            if (parent.scale > 0f)
            {
                Projectile.scale = parent.scale;
            }
            else if (Projectile.scale <= 0f)
            {
                Projectile.scale = 1f;
            }

            if (Projectile.velocity.LengthSquared() > MinimumVelocityLengthSquared)
            {
                Projectile.rotation =
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            Projectile.timeLeft = 2;

            Lighting.AddLight(
                Projectile.Center,
                0.4f,
                0.45f,
                0.7f
            );
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (VisualStyle == NeuroWeaponVisualStyle.LunarFlareStreak)
            {
                DrawLunarFlareStreak();
                return false;
            }

            DrawProjectileTexture();
            return false;
        }

        private Projectile GetParentProjectile()
        {
            int parentIndex = ParentProjectileIndex;

            if (
                parentIndex < 0 ||
                parentIndex >= Main.maxProjectiles
            )
            {
                return null;
            }

            Projectile parent = Main.projectile[parentIndex];

            if (parent == null || !parent.active)
            {
                return null;
            }

            return parent;
        }

        private void DrawProjectileTexture()
        {
            int textureProjectileType = TextureProjectileType;

            if (
                textureProjectileType <= ProjectileID.None ||
                textureProjectileType >= TextureAssets.Projectile.Length
            )
            {
                return;
            }

            Texture2D texture =
                TextureAssets.Projectile[textureProjectileType].Value;

            int frameCount = Main.projFrames[textureProjectileType];

            if (frameCount <= 0)
            {
                frameCount = 1;
            }

            int frame = TextureFrameOverride;

            if (frame < 0)
            {
                frame = 0;
            }

            frame %= frameCount;

            Rectangle sourceRectangle =
                texture.Frame(
                    horizontalFrames: 1,
                    verticalFrames: frameCount,
                    frameX: 0,
                    frameY: frame
                );

            Vector2 origin = sourceRectangle.Size() * 0.5f;

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRectangle,
                Color.White,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );
        }

        private void DrawLunarFlareStreak()
        {
            Texture2D texture = TextureAssets.MagicPixel.Value;

            Rectangle sourceRectangle = new Rectangle(0, 0, 1, 1);

            Vector2 origin = new Vector2(0.5f, 0.5f);

            Vector2 scale = new Vector2(
                LunarFlareStreakWidth,
                LunarFlareStreakLength
            ) * Projectile.scale;

            Color mainColor = new Color(190, 230, 255, 220);
            Color coreColor = new Color(255, 255, 255, 240);

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRectangle,
                mainColor,
                Projectile.rotation,
                origin,
                scale,
                SpriteEffects.None,
                0
            );

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRectangle,
                coreColor,
                Projectile.rotation,
                origin,
                scale * 0.45f,
                SpriteEffects.None,
                0
            );
        }
    }
}