#region Using directives

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
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
			Projectile.width = Projectile.height = 16;

			Projectile.alpha = 255;
			Projectile.penetrate = 10;
			Projectile.timeLeft = 2700;

			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Projectile.position.Y = Projectile.ai[0];
			Projectile.height = (int)Projectile.ai[1];

			Projectile.direction = System.Math.Sign(Projectile.Center.X - Main.player[Projectile.owner].Center.X);

			Projectile.velocity.X = Projectile.direction * 1E-06f;

			RemoveDuplicate();

			SpawnDust();

			return (false);
		}

		private void RemoveDuplicate()
		{
			if (Projectile.owner == Main.myPlayer)
			{
				for (int i = 0; i < Main.maxProjectiles; ++i)
				{
					if (Main.projectile[i].active && i != Projectile.whoAmI && Main.projectile[i].type == Projectile.type &&
						Main.projectile[i].owner == Projectile.owner && Main.projectile[i].timeLeft > Projectile.timeLeft)
					{
						Projectile.Kill();
						return;
					}
				}
			}
		}

		private void SpawnDust()
		{
			float dustAmount = Projectile.width * Projectile.height * 0.0045f;

			for (int i = 0; i < dustAmount; ++i)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 100);
				dust.noGravity = true;
				dust.velocity *= 0.5f;
				dust.velocity.Y -= 0.5f;
				dust.scale = 1.4f;
				dust.position += new Vector2(6f, -2f);
			}
		}
	}
}
