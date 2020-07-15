using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Thrown
{
	public class StormEdgeProjectile : ModProjectile
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Storm Edge");
		}

		int i;

		public override void SetDefaults()
		{
			projectile.width = 20;
			projectile.height = 30;
			projectile.aiStyle = 3;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.magic = false;
			projectile.penetrate = 3;
			projectile.timeLeft = 600;
			projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			i++;
			if (i % 2 == 0)
			{
				int dust = Dust.NewDust(projectile.position, projectile.width / 2, projectile.height / 2, 132);
			}
			projectile.rotation += 0.1f;
		}
	}
}