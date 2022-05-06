using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles
{
	public class InvisibleProj : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.aiStyle = 1;
			Projectile.penetrate = -1;
			Projectile.width = 14;
			Projectile.height = 18;
			Projectile.alpha = 0;
			Projectile.damage = 6;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		private int TimeLeft = 0;



		public override void AI()
		{
			TimeLeft++;
			if (TimeLeft % 1 == 0)
			{
				Projectile.Kill();
			}
		}
	}
}