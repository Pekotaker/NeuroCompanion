using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeuroCompanion.Players;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles.Companion
{
    public partial class NeuroCompanionProjectile
    {
        private const string BaseTexturePath =
            "NeuroCompanion/Projectiles/Companion/NeuroCompanionProjectile";

        private const string Mk2TexturePath =
            "NeuroCompanion/Projectiles/Companion/NeuroCompanionProjectile_Mk2";

        private const string Mk3TexturePath =
            "NeuroCompanion/Projectiles/Companion/NeuroCompanionProjectile_Mk3";

        private const string Mk4TexturePath =
            "NeuroCompanion/Projectiles/Companion/NeuroCompanionProjectile_Mk4";

        private const string EvilTexturePath =
            "NeuroCompanion/Projectiles/Companion/NeuroCompanionProjectile_Evil";

        private const string Mk4EvilTexturePath =
            "NeuroCompanion/Projectiles/Companion/NeuroCompanionProjectile_Mk4_Evil";

        private void Animate()
        {
            Projectile.frameCounter++;

            if (Projectile.frameCounter < AnimationFrameDurationTicks)
            {
                return;
            }

            Projectile.frameCounter = 0;
            Projectile.frame++;

            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
        }

        private void CreateVisualEffects()
        {
            int dustChance = State == CompanionState.Attacking
                ? AttackDustChance
                : IdleDustChance;

            if (!Main.rand.NextBool(dustChance))
            {
                return;
            }

            int dustType = IsEvilVisualActive()
                ? DustID.GemRuby
                : DustID.GemSapphire;

            Dust.NewDust(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                dustType
            );
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = GetCurrentCompanionTexture();

            int frameCount = Main.projFrames[Projectile.type];
            int frameHeight = texture.Height / frameCount;

            Rectangle sourceRectangle = new Rectangle(
                0,
                Projectile.frame * frameHeight,
                texture.Width,
                frameHeight
            );

            Vector2 origin = sourceRectangle.Size() / 2f;

            SpriteEffects spriteEffects = Projectile.spriteDirection == -1
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;

            Vector2 drawPosition =
                Projectile.Center
                + new Vector2(0f, Projectile.gfxOffY)
                - Main.screenPosition;

            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                sourceRectangle,
                Projectile.GetAlpha(lightColor),
                Projectile.rotation,
                origin,
                Projectile.scale,
                spriteEffects,
                0
            );

            return false;
        }

        private Texture2D GetCurrentCompanionTexture()
        {
            if (IsEvilVisualActive())
            {
                return ModContent
                    .Request<Texture2D>(GetEvilTexturePath())
                    .Value;
            }

            return ModContent
                .Request<Texture2D>(GetStaffTexturePath())
                .Value;
        }

        private string GetEvilTexturePath()
        {
            int visualTier = GetOwnerStaffVisualTier();

            if (visualTier == 4)
            {
                return Mk4EvilTexturePath;
            }

            return EvilTexturePath;
        }

        private string GetStaffTexturePath()
        {
            int visualTier = GetOwnerStaffVisualTier();

            switch (visualTier)
            {
                case 2:
                    return Mk2TexturePath;

                case 3:
                    return Mk3TexturePath;

                case 4:
                    return Mk4TexturePath;

                default:
                    return BaseTexturePath;
            }
        }

        private int GetOwnerStaffVisualTier()
        {
            if (Projectile.owner < 0 || Projectile.owner >= Main.maxPlayers)
            {
                return 1;
            }

            Player owner = Main.player[Projectile.owner];

            if (owner == null || !owner.active)
            {
                return 1;
            }

            NeuroCompanionPlayer neuroPlayer =
                owner.GetModPlayer<NeuroCompanionPlayer>();

            return neuroPlayer.NeuroStaffVisualTier;
        }

        private bool IsEvilVisualActive()
        {
            if (Projectile.owner < 0 || Projectile.owner >= Main.maxPlayers)
            {
                return false;
            }

            Player owner = Main.player[Projectile.owner];

            if (owner == null || !owner.active)
            {
                return false;
            }

            NeuroCompanionPlayer neuroPlayer =
                owner.GetModPlayer<NeuroCompanionPlayer>();

            return neuroPlayer.ShouldUseEvilSprite();
        }
    }
}