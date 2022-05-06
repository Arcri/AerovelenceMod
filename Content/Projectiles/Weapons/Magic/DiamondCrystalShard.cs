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
	internal sealed class DiamondCrystalShard : ModProjectile
	{
		public override string Texture => AerovelenceMod.CrystalCavernsAssets + "DiamondCavernShards";

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;

			Main.projFrames[Projectile.type] = 3;
		}
		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 6;

			Projectile.light = 0.1f;
			Projectile.penetrate = 2;
			Projectile.timeLeft = 300;

			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;
		}

		public override bool PreAI()
		{
			if (Projectile.localAI[0] == 0)
			{
				Projectile.localAI[0] = 1;
				Projectile.frame = Main.rand.Next(Main.projFrames[Projectile.type]);
			}

			Projectile.velocity *= 0.99f;
			Projectile.velocity.Y += 0.2f;

			Projectile.rotation += Projectile.velocity.Length() * 0.1f * Projectile.direction;

			if (Main.rand.Next(20) == 0)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Crystal>(), Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);
			}

			return (false);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.velocity.Y = -oldVelocity.Y * 0.7f;
			}
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; ++i)
			{
				Dust newDust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Crystal>(), 0, 0, 100)];
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
