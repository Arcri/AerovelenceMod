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
			projectile.width = 1;
			projectile.height = 1;
			projectile.alpha = 255;
			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
			projectile.magic = true;
		}

		Vector2 projectileOldPosition;
		float playerOldPositionY;
		float originOffsetX = 0;
		public override void AI()
		{
			if (projectile.ai[0] == 0)
			{
				Random rand = new Random();
				originOffsetX = rand.Next(-360, 360);
				projectileOldPosition = new Vector2(projectile.position.X - originOffsetX, projectile.position.Y);
				var Y = 1080 - (Main.player[Main.myPlayer].position.Y - projectile.position.Y);
				playerOldPositionY = Main.player[Main.myPlayer].position.Y;
				originOffsetX /= (180 / (1080 / Y));
			}

			if (projectile.ai[0] < 25)
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
				if (projectile.ai[0] % 5 == 0)
				{
					for (int j = 0; j < (Main.screenHeight * projectile.ai[0] / 5) / 6; j++)
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
				projectile.ai[0]++;
			}
		}

		public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
	}
}