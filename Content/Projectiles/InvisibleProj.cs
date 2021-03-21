using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles
{
	public class InvisibleProj : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.aiStyle = 1;
			projectile.penetrate = -1;
			projectile.width = 14;
			projectile.height = 18;
			projectile.alpha = 0;
			projectile.damage = 6;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
		}

		private int TimeLeft = 0;



		public override void AI()
		{
			TimeLeft++;
			if (TimeLeft % 1 == 0)
			{
				projectile.Kill();
			}
		}
	}
}