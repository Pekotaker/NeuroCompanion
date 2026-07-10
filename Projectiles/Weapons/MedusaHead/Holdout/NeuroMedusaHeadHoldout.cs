using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeuroCompanion.Projectiles.Weapons.MedusaHead.Holdout
{
    public partial class NeuroMedusaHeadHoldout : ModProjectile
    {
        public override string Texture =>
            "Terraria/Images/Projectile_" + ProjectileID.MedusaHead;

        private bool initialized;
        private bool ownerDamageEnabled;

        private int remainingLifeTicks;

        private Microsoft.Xna.Framework.Vector2 fallbackAnchorPosition;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] =
                Main.projFrames[ProjectileID.MedusaHead];

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.MedusaHead);

            Projectile.DamageType = DamageClass.Magic;

            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = 0;
            Projectile.timeLeft = 2;
            Projectile.netImportant = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            InitializeIfNeeded();
            PreventGenericOwnerDamageOnHoldout();

            remainingLifeTicks--;

            if (remainingLifeTicks <= 0)
            {
                Projectile.Kill();
                return;
            }

            Projectile.hide = false;

            UpdateAimAndPosition();
            UpdateAnimation();

            Projectile.timeLeft = 2;
        }
    }
}