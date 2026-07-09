using Microsoft.Xna.Framework;

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
                Projectile.Resize(36, 36);
                Projectile.scale = 1.55f;
                Projectile.penetrate = -1;
                Projectile.timeLeft = 240;

                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 12;
            }
            else
            {
                Projectile.Resize(18, 18);
                Projectile.scale = 0.9f;
                Projectile.penetrate = 1;
            }
        }

        Projectile.rotation +=
            0.25f * Projectile.direction;

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
                    IsHeavyOrb ? 1.35f : 0.9f
                );

            dust.noGravity = true;
        }
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
                    IsHeavyOrb ? 1.4f : 0.9f
                );

            dust.noGravity = true;
        }
    }
}