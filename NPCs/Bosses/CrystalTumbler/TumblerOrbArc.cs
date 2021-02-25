using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
    public class TumblerOrbArc : ModProjectile
	{
		int t;
        public override void SetStaticDefaults()
        {
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 40;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}
        public override void SetDefaults()
		{
			projectile.width = 14;
			projectile.height = 14;
			projectile.aiStyle = 88;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = true;
			projectile.extraUpdates = 4;
			projectile.timeLeft = 120 * (projectile.extraUpdates + 1);
		}
		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = ModContent.GetTexture(Texture+"Glow");
			spriteBatch.Draw(
				texture,
				projectile.Center - Main.screenPosition,
				new Rectangle(0, 0, texture.Width, texture.Height),
				Color.White,
				projectile.rotation,
				texture.Size(),
				projectile.scale,
				SpriteEffects.None,
				0f
			);
		}
		public override bool PreDraw(SpriteBatch sb, Color lightColor)
		{
			Vector2 vector = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int i = 0; i < projectile.oldPos.Length; i++)
			{
				Vector2 position = projectile.oldPos[i] - Main.screenPosition + vector + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - i) / projectile.oldPos.Length);
				sb.Draw(Main.projectileTexture[projectile.type], position, null, color, projectile.rotation, vector, projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}
		public override void AI()
		{
			float num30 = projectile.rotation + (float)Math.PI / 2f + ((Main.rand.Next(2) == 1) ? (-1f) : 1f) * ((float)Math.PI / 2f);
			float num31 = (float)Main.rand.NextDouble() * 2f + 2f;
			Vector2 vector = new Vector2((float)Math.Cos(num30) * num31, (float)Math.Sin(num30) * num31);
			int num32 = Dust.NewDust(projectile.oldPos[projectile.oldPos.Length - 1], 0, 0, 229, vector.X, vector.Y);
			Main.dust[num32].noGravity = true;
			Main.dust[num32].scale = 1.7f;
			projectile.frameCounter++;
			Lighting.AddLight(projectile.Center, 0.3f, 0.45f, 0.5f);
			if (projectile.velocity == Vector2.Zero)
			{
				if (projectile.frameCounter >= projectile.extraUpdates * 2)
				{
					projectile.frameCounter = 0;
					bool flag34 = true;
					for (int num874 = 1; num874 < projectile.oldPos.Length; num874++)
					{
						if (projectile.oldPos[num874] != projectile.oldPos[0])
						{
							flag34 = false;
						}
					}
					if (flag34)
					{
						projectile.Kill();
						return;
					}
				}
				if (Main.rand.Next(projectile.extraUpdates) == 0)
				{
					for (int num875 = 0; num875 < 2; num875++)
					{
						float num876 = projectile.rotation + ((Main.rand.Next(2) == 1) ? (-1f) : 1f) * ((float)Math.PI / 2f);
						float num877 = (float)Main.rand.NextDouble() * 0.8f + 1f;
						Vector2 vector71 = new Vector2((float)Math.Cos(num876) * num877, (float)Math.Sin(num876) * num877);
						int num878 = Dust.NewDust(projectile.Center, 0, 0, 226, vector71.X, vector71.Y);
						Main.dust[num878].noGravity = true;
						Main.dust[num878].scale = 1.2f;
					}
					if (Main.rand.Next(5) == 0)
					{
						Vector2 value44 = projectile.velocity.RotatedBy(1.5707963705062866) * ((float)Main.rand.NextDouble() - 0.5f) * projectile.width;
						int num879 = Dust.NewDust(projectile.Center + value44 - Vector2.One * 4f, 8, 8, 31, 0f, 0f, 100, default, 1.5f);
						Dust dust128 = Main.dust[num879];
						Dust dust2 = dust128;
						dust2.velocity *= 0.5f;
						Main.dust[num879].velocity.Y = 0f - Math.Abs(Main.dust[num879].velocity.Y);
					}
				}
			}
			else
			{
				if (projectile.frameCounter < projectile.extraUpdates * 2)
				{
					return;
				}
				projectile.frameCounter = 0;
				float num880 = projectile.velocity.Length();
				UnifiedRandom unifiedRandom = new UnifiedRandom((int)projectile.ai[1]);
				int num881 = 0;
				Vector2 spinningpoint14 = -Vector2.UnitY;
				while (true)
				{
					int num882 = unifiedRandom.Next();
					projectile.ai[1] = num882;
					num882 %= 100;
					float f = (float)num882 / 100f * ((float)Math.PI * 2f);
					Vector2 vector72 = f.ToRotationVector2();
					if (vector72.Y > 0f)
					{
						vector72.Y *= -1f;
					}
					bool flag35 = false;
					if (vector72.Y > -0.02f)
					{
						flag35 = true;
					}
					if (vector72.X * (float)(projectile.extraUpdates + 1) * 2f * num880 + projectile.localAI[0] > 40f)
					{
						flag35 = true;
					}
					if (vector72.X * (float)(projectile.extraUpdates + 1) * 2f * num880 + projectile.localAI[0] < -40f)
					{
						flag35 = true;
					}
					if (flag35)
					{
						if (num881++ >= 100)
						{
							projectile.velocity = Vector2.Zero;
							projectile.localAI[1] = 1f;
							break;
						}
						continue;
					}
					spinningpoint14 = vector72;
					break;
				}
				if (projectile.velocity != Vector2.Zero)
				{
					projectile.localAI[0] += spinningpoint14.X * (float)(projectile.extraUpdates + 1) * 2f * num880;
					projectile.velocity = spinningpoint14.RotatedBy(projectile.ai[0] + (float)Math.PI / 2f) * num880;
					projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2f;
				}
			}
		}
	}
}