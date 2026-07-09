using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Orb;

public class NeuroChargedBlasterOrb : ModProjectile
{
    public override string Texture =>
        "Terraria/Images/Projectile_" + ProjectileID.ChargedBlasterOrb;

    private bool initialized;

    private bool IsHeavyOrb =>
        Projectile.ai[0] >= 0.5f;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] =
            Main.projFrames[ProjectileID.ChargedBlasterOrb];
    }

    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 18;

        Projectile.DamageType = DamageClass.Magic;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.penetrate = 1;
        Projectile.tileCollide = true;
        Projectile.ignoreWater = true;

        Projectile.timeLeft = 180;
        Projectile.alpha = 0;

        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        if (!initialized)
        {
            initialized = true;

            if (IsHeavyOrb)
            {
                Projectile.Resize(28, 28);
                Projectile.scale = 1.15f;
                Projectile.penetrate = -1;
                Projectile.timeLeft = 240;

                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 12;
            }
            else
            {
                Projectile.Resize(14, 14);
                Projectile.scale = 0.65f;
                Projectile.penetrate = 1;
            }
        }

        UpdateAnimation();

        Projectile.rotation =
            Projectile.velocity.ToRotation();

        Lighting.AddLight(
            Projectile.Center,
            0.2f,
            0.45f,
            1f
        );

        if (Main.rand.NextBool(IsHeavyOrb ? 2 : 4))
        {
            Dust dust =
                Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.BlueTorch,
                    Projectile.velocity.X * 0.15f,
                    Projectile.velocity.Y * 0.15f,
                    100,
                    default,
                    IsHeavyOrb ? 1f : 0.65f
                );

            dust.noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture =
            TextureAssets.Projectile[Type].Value;

        int frameCount = Main.projFrames[Type];

        if (frameCount <= 0)
        {
            frameCount = 1;
        }

        int frameHeight =
            texture.Height / frameCount;

        Rectangle sourceRectangle =
            new Rectangle(
                0,
                frameHeight * Projectile.frame,
                texture.Width,
                frameHeight
            );

        Vector2 origin =
            sourceRectangle.Size() * 0.5f;

        Main.EntitySpriteDraw(
            texture,
            Projectile.Center - Main.screenPosition,
            sourceRectangle,
            lightColor,
            Projectile.rotation,
            origin,
            Projectile.scale,
            SpriteEffects.None,
            0
        );

        return false;
    }

    public override void OnKill(int timeLeft)
    {
        int dustCount = IsHeavyOrb ? 18 : 8;

        for (int i = 0; i < dustCount; i++)
        {
            Dust dust =
                Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Electric,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-2f, 2f),
                    100,
                    default,
                    IsHeavyOrb ? 1.05f : 0.65f
                );

            dust.noGravity = true;
        }
    }

    private void UpdateAnimation()
    {
        int frameCount = Main.projFrames[Type];

        if (frameCount <= 1)
        {
            return;
        }

        Projectile.frameCounter++;

        int ticksPerFrame =
            IsHeavyOrb ? 4 : 3;

        if (Projectile.frameCounter < ticksPerFrame)
        {
            return;
        }

        Projectile.frameCounter = 0;
        Projectile.frame++;

        if (Projectile.frame >= frameCount)
        {
            Projectile.frame = 0;
        }
    }
}