using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Other.ArmorSetBonus
{
    public class BurnshockCrystal : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.aiStyle = 1;
			Projectile.width = 16;
			Projectile.height = 8;
			Projectile.alpha =  0;
			Projectile.penetrate = 10;
			Projectile.damage = 45;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = false;
		}
	}
}