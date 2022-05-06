using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AerovelenceMod.Content.Projectiles.NPCs.Bosses.CrystalTumbler
{
    public class TumblerOrbArc : ModProjectile
	{
        public override void SetStaticDefaults()
        {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}
        public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.aiStyle = 88;
			Projectile.damage = 12;
			Projectile.hostile = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 4;
			Projectile.timeLeft = 120 * (Projectile.extraUpdates + 1);
		}
        public override void PostDraw(Color lightColor)
        {
        Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture+"Glow");
			Main.EntitySpriteDraw(
				texture,
				Projectile.Center - Main.screenPosition,
				new Rectangle(0, 0, texture.Width, texture.Height),
				Color.White,
				Projectile.rotation,
				texture.Size(),
				Projectile.scale,
				SpriteEffects.None,
				0
			);
		}
		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 vector = new Vector2(TextureAssets.Projectile[Projectile.type].Width() * 0.5f, Projectile.height * 0.5f);
			for (int i = 0; i < Projectile.oldPos.Length; i++)
			{
				Vector2 position = Projectile.oldPos[i] - Main.screenPosition + vector + new Vector2(0f, Projectile.gfxOffY);
				Color color = Color.White;
				Main.spriteBatch.Draw((Texture2D)TextureAssets.Projectile[Projectile.type], position, null, color, Projectile.rotation, vector, Projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}
		public override void AI()
		{
			float num30 = Projectile.rotation + (float)Math.PI / 2f + ((Main.rand.NextBool(2)) ? (-1f) : 1f) * ((float)Math.PI / 2f);
			float num31 = (float)Main.rand.NextDouble() * 2f + 2f;
			Vector2 vector = new Vector2((float)Math.Cos(num30) * num31, (float)Math.Sin(num30) * num31);
			int num32 = Dust.NewDust(Projectile.oldPos[Projectile.oldPos.Length - 1], 0, 0, 229, vector.X, vector.Y);
			Main.dust[num32].noGravity = true;
			Main.dust[num32].scale = 1.7f;
			Projectile.frameCounter++;
			Lighting.AddLight(Projectile.Center, 0.3f, 0.45f, 0.5f);
			if (Projectile.velocity == Vector2.Zero)
			{
				if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
				{
					Projectile.frameCounter = 0;
					bool flag34 = true;
					for (int num874 = 1; num874 < Projectile.oldPos.Length; num874++)
					{
						if (Projectile.oldPos[num874] != Projectile.oldPos[0])
						{
							flag34 = false;
						}
					}
					if (flag34)
					{
						Projectile.Kill();
						return;
					}
				}
				if (Main.rand.NextBool(Projectile.extraUpdates))
				{
					for (int num875 = 0; num875 < 2; num875++)
					{
						float num876 = Projectile.rotation + ((Main.rand.Next(2) == 1) ? (-1f) : 1f) * ((float)Math.PI / 2f);
						float num877 = (float)Main.rand.NextDouble() * 0.8f + 1f;
						Vector2 vector71 = new Vector2((float)Math.Cos(num876) * num877, (float)Math.Sin(num876) * num877);
						int num878 = Dust.NewDust(Projectile.Center, 0, 0, 226, vector71.X, vector71.Y);
						Main.dust[num878].noGravity = true;
						Main.dust[num878].scale = 1.2f;
					}
					if (Main.rand.NextBool(5))
					{
						Vector2 value44 = Projectile.velocity.RotatedBy(1.5707963705062866) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
						int num879 = Dust.NewDust(Projectile.Center + value44 - Vector2.One * 4f, 8, 8, 31, 0f, 0f, 100, default, 1.5f);
						Dust dust128 = Main.dust[num879];
						Dust dust2 = dust128;
						dust2.velocity *= 0.5f;
						Main.dust[num879].velocity.Y = 0f - Math.Abs(Main.dust[num879].velocity.Y);
					}
				}
			}
			else
			{
				if (Projectile.frameCounter < Projectile.extraUpdates * 2)
				{
					return;
				}
				Projectile.frameCounter = 0;
				float num880 = Projectile.velocity.Length();
				UnifiedRandom unifiedRandom = new UnifiedRandom((int)Projectile.ai[1]);
				int num881 = 0;
				Vector2 spinningpoint14 = -Vector2.UnitY;
				while (true)
				{
					int num882 = unifiedRandom.Next();
					Projectile.ai[1] = num882;
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
					if (vector72.X * (float)(Projectile.extraUpdates + 1) * 2f * num880 + Projectile.localAI[0] > 40f)
					{
						flag35 = true;
					}
					if (vector72.X * (float)(Projectile.extraUpdates + 1) * 2f * num880 + Projectile.localAI[0] < -40f)
					{
						flag35 = true;
					}
					if (flag35)
					{
						if (num881++ >= 100)
						{
							Projectile.velocity = Vector2.Zero;
							Projectile.localAI[1] = 1f;
							break;
						}
						continue;
					}
					spinningpoint14 = vector72;
					break;
				}
				if (Projectile.velocity != Vector2.Zero)
				{
					Projectile.localAI[0] += spinningpoint14.X * (float)(Projectile.extraUpdates + 1) * 2f * num880;
					Projectile.velocity = spinningpoint14.RotatedBy(Projectile.ai[0] + (float)Math.PI / 2f) * num880;
					Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
				}
			}
		}
	}
}