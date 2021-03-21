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
            projectile.width = 150;
            projectile.height = 100;
            projectile.aiStyle = 0;
            projectile.tileCollide = false;
            projectile.ignoreWater = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 999;
        }
        public override void AI()
        {
            projectile.alpha += 2;
        }
    }
}