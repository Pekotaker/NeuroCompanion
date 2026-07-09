using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles.Weapons.ChargedBlasterCannon.Orb;

public class NeuroChargedBlasterOrb : ModProjectile
{
    private const int SmallOrbWidth = 14;
    private const int SmallOrbHeight = 14;
    private const float SmallOrbScale = 0.65f;

    private const int HeavyOrbWidth = 40;
    private const int HeavyOrbHeight = 40;
    private const float HeavyOrbScale = 1f;

    private const int SmallOrbPenetration = 1;
    private const int HeavyOrbPenetration = -1;

    private const int SmallOrbLifetimeTicks = 180;
    private const int HeavyOrbLifetimeTicks = 240;

    private const int HeavyOrbLocalNpcHitCooldownTicks = 12;

    private const int SmallOrbTrailDustCountPerTick = 1;
    private const int HeavyOrbTrailDustCountPerTick = 1;

    private const float SmallOrbTrailDustScale = 0.35f;
    private const float HeavyOrbTrailDustScale = 0.4f;

    private const float SmallOrbTrailLength = 72f;
    private const float HeavyOrbTrailLength = 96f;

    private const float SmallOrbTrailSideSpread = 6f;
    private const float HeavyOrbTrailSideSpread = 9f;

    private const float SmallOrbTrailPullVelocity = 5.8f;
    private const float HeavyOrbTrailPullVelocity = 7.2f;

    private const float TrailDustRandomVelocity = 0.75f;

    private const int OrbTrailDustType = DustID.Electric;
    private const int OrbDeathDustType = DustID.Electric;

    private const int SmallOrbDeathDustCount = 8;
    private const int HeavyOrbDeathDustCount = 18;

    private const float SmallOrbDeathDustScale = 0.95f;
    private const float HeavyOrbDeathDustScale = 1.55f;

    private const float DeathDustMaxVelocity = 2f;

    private const float OrbLightRed = 0.2f;
    private const float OrbLightGreen = 0.45f;
    private const float OrbLightBlue = 1f;

    public override string Texture =>
        "Terraria/Images/Projectile_" + ProjectileID.ChargedBlasterOrb;

    private bool initialized;

    private bool IsHeavyOrb =>
        Projectile.ai[0] >= 0.5f;

    private static Color GetOrbEffectColor()
    {
        return ChargedBlasterCannonEffectColor.GetColor();
    }

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

        Projectile.timeLeft = SmallOrbLifetimeTicks;
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
                Projectile.Resize(HeavyOrbWidth, HeavyOrbHeight);
                Projectile.scale = HeavyOrbScale;
                Projectile.penetrate = HeavyOrbPenetration;
                Projectile.timeLeft = HeavyOrbLifetimeTicks;

                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = HeavyOrbLocalNpcHitCooldownTicks;
            }
            else
            {
                Projectile.Resize(SmallOrbWidth, SmallOrbHeight);
                Projectile.scale = SmallOrbScale;
                Projectile.penetrate = SmallOrbPenetration;
            }
        }

        UpdateAnimation();

        Projectile.rotation =
            Projectile.velocity.ToRotation();

        Lighting.AddLight(
            Projectile.Center,
            OrbLightRed,
            OrbLightGreen,
            OrbLightBlue
        );

        EmitDraggedTrailDust();
    }

    private void EmitDraggedTrailDust()
    {
        Vector2 direction =
            Projectile.velocity.SafeNormalize(Vector2.UnitX);

        Vector2 perpendicular =
            direction.RotatedBy(MathHelper.PiOver2);

        int dustCount =
            IsHeavyOrb
                ? HeavyOrbTrailDustCountPerTick
                : SmallOrbTrailDustCountPerTick;

        float dustScale =
            IsHeavyOrb
                ? HeavyOrbTrailDustScale
                : SmallOrbTrailDustScale;

        float trailLength =
            IsHeavyOrb
                ? HeavyOrbTrailLength
                : SmallOrbTrailLength;

        float sideSpread =
            IsHeavyOrb
                ? HeavyOrbTrailSideSpread
                : SmallOrbTrailSideSpread;

        float pullVelocity =
            IsHeavyOrb
                ? HeavyOrbTrailPullVelocity
                : SmallOrbTrailPullVelocity;

        for (int i = 0; i < dustCount; i++)
        {
            float distanceBehindOrb =
                Main.rand.NextFloat(0f, trailLength);

            Vector2 dustPosition =
                Projectile.Center -
                direction * distanceBehindOrb +
                perpendicular * Main.rand.NextFloat(-sideSpread, sideSpread);

            Vector2 dustVelocity =
                direction * Main.rand.NextFloat(
                    pullVelocity * 0.65f,
                    pullVelocity
                ) +
                Main.rand.NextVector2Circular(
                    TrailDustRandomVelocity,
                    TrailDustRandomVelocity
                );

            Dust dust =
                Dust.NewDustPerfect(
                    dustPosition,
                    OrbTrailDustType,
                    dustVelocity,
                    100,
                    GetOrbEffectColor(),
                    dustScale
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
        int dustCount =
            IsHeavyOrb
                ? HeavyOrbDeathDustCount
                : SmallOrbDeathDustCount;

        float dustScale =
            IsHeavyOrb
                ? HeavyOrbDeathDustScale
                : SmallOrbDeathDustScale;

        for (int i = 0; i < dustCount; i++)
        {
            Dust dust =
                Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    OrbDeathDustType,
                    Main.rand.NextFloat(-DeathDustMaxVelocity, DeathDustMaxVelocity),
                    Main.rand.NextFloat(-DeathDustMaxVelocity, DeathDustMaxVelocity),
                    100,
                    GetOrbEffectColor(),
                    dustScale
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