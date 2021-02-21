#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

#endregion

namespace AerovelenceMod.Projectiles
{
	internal sealed class WallOfIce : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.None;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wall of Ice");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.alpha = 255;
			projectile.penetrate = 10;
			projectile.timeLeft = 2700;

			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			projectile.position.Y = projectile.ai[0];
			projectile.height = (int)projectile.ai[1];

			projectile.direction = System.Math.Sign(projectile.Center.X - Main.player[projectile.owner].Center.X);

			projectile.velocity.X = projectile.direction * 1E-06f;

			RemoveDuplicate();

			SpawnDust();

			return (false);
		}

		private void RemoveDuplicate()
		{
			if (projectile.owner == Main.myPlayer)
			{
				for (int i = 0; i < Main.maxProjectiles; ++i)
				{
					if (Main.projectile[i].active && i != projectile.whoAmI && Main.projectile[i].type == projectile.type &&
						Main.projectile[i].owner == projectile.owner && Main.projectile[i].timeLeft > projectile.timeLeft)
					{
						projectile.Kill();
						return;
					}
				}
			}
		}

		private void SpawnDust()
		{
			float dustAmount = projectile.width * projectile.height * 0.0045f;

			for (int i = 0; i < dustAmount; ++i)
			{
				Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Ice, 0f, 0f, 100);
				dust.noGravity = true;
				dust.velocity *= 0.5f;
				dust.velocity.Y -= 0.5f;
				dust.scale = 1.4f;
				dust.position += new Vector2(6f, -2f);
			}
		}
	}
}
