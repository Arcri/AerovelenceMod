#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common.Utilities;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
	internal sealed class DustiliteShard : ModProjectile
	{
		public override string Texture => AerovelenceMod.CrystalCavernsAssets + "DustiliteShards";

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;

			Main.projFrames[projectile.type] = 3;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 6;

			projectile.light = 0.1f;
			projectile.penetrate = 2;
			projectile.timeLeft = 300;

			projectile.ranged = true;
			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			if (projectile.localAI[0] == 0)
			{
				projectile.localAI[0] = 1;
				projectile.frame = Main.rand.Next(Main.projFrames[projectile.type]);
			}

			projectile.velocity *= 0.99f;
			projectile.velocity.Y += 0.2f;

			projectile.rotation += projectile.velocity.Length() * 0.1f * projectile.direction;

			if (Main.rand.Next(20) == 0)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.Dustilite>(), projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100);
			}

			return (false);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = -oldVelocity.X;
			}
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.velocity.Y = -oldVelocity.Y * 0.7f;
			}
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; ++i)
			{
				Dust newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.Dustilite>(), 0, 0, 100)];
				newDust.noGravity = true;

				if (Main.rand.Next(3) == 0)
				{
					newDust.scale = 1.3f;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			this.DrawProjectileTrailCentered(spriteBatch, lightColor);

			return this.DrawProjectileCentered(spriteBatch, lightColor);
		}
	}
}
