using Terraria.ModLoader;

namespace AerovelenceMod.Projectiles
{
    public class BurnshockCrystal : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.aiStyle = 1;
			projectile.width = 16;
			projectile.height = 8;
			projectile.alpha =  0;
			projectile.penetrate = 10;
			projectile.damage = 45;
			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = false;
		}
	}
}