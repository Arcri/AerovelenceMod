using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
    public class EyeOfTheBeholder : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye of the Beholder");
        }

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 100;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 999;
        }
        public override void AI()
        {
            Projectile.alpha += 2;
        }
    }
}