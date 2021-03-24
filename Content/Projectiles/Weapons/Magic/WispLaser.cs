using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Content.Projectiles
{
	public class WispLaser : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 1;
			projectile.height = 1;
			projectile.alpha = 255;
			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
			projectile.magic = true;	
		}

		public override void AI()
		{
			for (int j = 0; j < 10; j++)
			{
				float x = projectile.position.X - projectile.velocity.X / 10f * j;
				float y = projectile.position.Y - projectile.velocity.Y / 10f * j;
				Dust dust = Dust.NewDustDirect(new Vector2(x, y), 1, 1, 160);
				dust.position.X = x;
				dust.position.Y = y;
				dust.velocity *= 0f;
				dust.noGravity = true;
			}

			if(projectile.ai[0] > 120)
			{ 
				projectile.Kill(); 
			}
			projectile.ai[0]++;
		}

		public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
	}
}