#region Using directives

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
	public sealed class RedSeed : ModProjectile
	{
		public override string Texture => AerovelenceMod.ProjectileAssets + "RedPetal";



		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
		}


		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 12;
			
			Projectile.penetrate = -1;

			Projectile.DamageType = DamageClass.Magic;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
		}



        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			Texture2D texture = Main.projectileTexture[Projectile.type];
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length);
				spriteBatch.Draw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}

			return (true);
		}

        private bool rotChanged;
		public override void AI()
		{
			Projectile.velocity.X *= 0.994f;
			Projectile.spriteDirection = Projectile.direction;
			if (Main.MouseWorld == Projectile.Center)
			{
				Projectile.Kill();
			}


			if (!rotChanged)
			{
				Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation() - MathHelper.PiOver2;
				rotChanged = true;
			}

			int Flower = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.RedPetal>(), 0, 0, 10);
			{
				Main.dust[Flower].scale = 0.53f;
				Main.dust[Flower].noGravity = true;
			}
		}

		
				

        public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 15; ++i)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.RedPetal>(), 0, 0, 100);
			}

			if (Main.myPlayer == Projectile.owner)
			{
				Projectile.NewProjectile(Projectile.position, Vector2.Zero, ModContent.ProjectileType<GrowthFlower>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
			}
		}
	}
}
