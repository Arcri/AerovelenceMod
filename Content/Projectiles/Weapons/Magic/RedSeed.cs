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
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
		}


		public override void SetDefaults()
		{
			projectile.width = 18;
			projectile.height = 12;
			
			projectile.penetrate = -1;

			projectile.magic = true;
			projectile.melee = true;
			projectile.friendly = true;
			projectile.tileCollide = true;
		}



        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			Texture2D texture = Main.projectileTexture[projectile.type];
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, projectile.height * 0.5f);

			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / projectile.oldPos.Length);
				spriteBatch.Draw(texture, drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			}

			return (true);
		}

        private bool rotChanged;
		public override void AI()
		{
			projectile.velocity.X *= 0.994f;
			projectile.spriteDirection = projectile.direction;
			if (Main.MouseWorld == projectile.Center)
			{
				projectile.Kill();
			}


			if (!rotChanged)
			{
				projectile.rotation = projectile.DirectionTo(Main.MouseWorld).ToRotation() - MathHelper.PiOver2;
				rotChanged = true;
			}

			int Flower = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.RedPetal>(), 0, 0, 10);
			{
				Main.dust[Flower].scale = 0.53f;
				Main.dust[Flower].noGravity = true;
			}
		}

		
				

        public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 15; ++i)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.RedPetal>(), 0, 0, 100);
			}

			if (Main.myPlayer == projectile.owner)
			{
				Projectile.NewProjectile(projectile.position, Vector2.Zero, ModContent.ProjectileType<GrowthFlower>(), projectile.damage, projectile.knockBack, projectile.owner);
			}
		}
	}
}
