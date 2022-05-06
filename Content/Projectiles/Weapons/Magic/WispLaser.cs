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
			Projectile.width = 1;
			Projectile.height = 1;
			Projectile.alpha = 255;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Magic;	
		}

		public override void AI()
		{
			for (int j = 0; j < 10; j++)
			{
				float x = Projectile.position.X - Projectile.velocity.X / 10f * j;
				float y = Projectile.position.Y - Projectile.velocity.Y / 10f * j;
				Dust dust = Dust.NewDustDirect(new Vector2(x, y), 1, 1, 160);
				dust.position.X = x;
				dust.position.Y = y;
				dust.velocity *= 0f;
				dust.noGravity = true;
			}

			if(Projectile.ai[0] > 120)
			{ 
				Projectile.Kill(); 
			}
			Projectile.ai[0]++;
		}

		public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
	}
}