using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using NeuroCompanion.Projectiles.Globals;

namespace NeuroCompanion.Projectiles.Weapons.LifeDrain
{
    public partial class NeuroLifeDrainField : ModProjectile
    {
        public override string Texture =>
            "Terraria/Images/Projectile_" + ProjectileID.SoulDrain;

        private bool initialized;
        private bool ownerDamageEnabled;

        private Vector2 DrainCenter =>
            new Vector2(
                Projectile.ai[0],
                Projectile.ai[1]
            );

        private bool OwnerDamageEnabled =>
            ownerDamageEnabled;

        private int streamSpawnTimer =
            StreamSpawnIntervalTicks - 1;

        public override void SetDefaults()
        {
            Projectile.width = DrainDiameter;
            Projectile.height = DrainDiameter;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = -1;

            // The drain area itself is never shortened or stopped by tiles.
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            // Each field damages each NPC once.
            // Repeated uses create new fields at the weapon's normal use rate.
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.aiStyle = 0;
            Projectile.timeLeft = LifetimeTicks;

            Projectile.alpha = 255;
            Projectile.netImportant = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? CanDamage()
        {
            // Evil owner damage is managed by the existing owner-damage global.
            if (OwnerDamageEnabled)
            {
                return false;
            }

            return true;
        }

        public override void AI()
        {
            InitializeIfNeeded();

            Projectile.Center = DrainCenter;

            if (!OwnerDamageEnabled)
            {
                UpdateDrainEffects();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Life Drain is represented by dust and streams rather than
            // by drawing the vanilla projectile texture directly.
            return false;
        }

        private void InitializeIfNeeded()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;

            ownerDamageEnabled =
                Projectile
                    .GetGlobalProjectile<EvilNeuroPlayerAttackGlobal>()
                    .CanDamageOwner;

            Projectile.friendly = !ownerDamageEnabled;
            Projectile.hostile = false;
        }
    }
}