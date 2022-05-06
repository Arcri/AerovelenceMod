using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles
{
	public class LightBeam : ModProjectile
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

		Vector2 projectileOldPosition;
		float playerOldPositionY;
		float originOffsetX = 0;
		public override void AI()
		{
			if (Projectile.ai[0] == 0)
			{
				Random rand = new Random();
				originOffsetX = rand.Next(-360, 360);
				projectileOldPosition = new Vector2(Projectile.position.X - originOffsetX, Projectile.position.Y);
				var Y = 1080 - (Main.player[Main.myPlayer].position.Y - Projectile.position.Y);
				playerOldPositionY = Main.player[Main.myPlayer].position.Y;
				originOffsetX /= (180 / (1080 / Y));
			}

			if (Projectile.ai[0] < 25)
			{
				base.AI();

				for (int i = 0; i < Main.screenHeight / 3; i++)
				{
					float x = projectileOldPosition.X + originOffsetX * i;
					float y = playerOldPositionY - 1080 + i * 6;
					Dust.NewDustDirect(new Vector2(x, y), 1, 1, 206, 0, 0, 0, Color.White, 0.9f);
				}
				
				int offsetX = 5;
				int	height = 0;
				if (Projectile.ai[0] % 5 == 0)
				{
					for (int j = 0; j < (Main.screenHeight * Projectile.ai[0] / 5) / 6; j++)
					{
						if (height > 1080)
						{
							offsetX += 5;
							height = 0;
						}
						float x = projectileOldPosition.X + originOffsetX * height + offsetX;
						float x2 = projectileOldPosition.X + originOffsetX * height - offsetX;
						float y = playerOldPositionY - 1080 + height * 6;

						Dust.NewDustDirect(new Vector2(x, y), 1, 1, 206, 0, 0, 0, Color.White, 0.9f);
						Dust.NewDustDirect(new Vector2(x2, y), 1, 1, 206, 0, 0, 0, Color.White, 0.9f);
						height++;
					}
				}
				Projectile.ai[0]++;
			}
		}

		public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
	}
}