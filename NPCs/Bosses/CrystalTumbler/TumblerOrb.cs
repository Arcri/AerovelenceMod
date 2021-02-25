using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
	public class TumblerOrb : ModProjectile
	{
		int t;
		public override void SetDefaults()
		{
			projectile.width = 80;
			projectile.height = 80;
			projectile.aiStyle = 88;
			projectile.damage = 15;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
		}
		
		public override void AI()
		{
			if (projectile.localAI[1] == 0f)
			{
				Main.PlaySound(SoundID.Item121, projectile.position);
				projectile.localAI[1] = 1f;
			}
			if (projectile.ai[0] < 180f)
			{
				projectile.alpha -= 5;
				if (projectile.alpha < 0)
				{
					projectile.alpha = 0;
				}
			}
			else
			{
				projectile.alpha += 5;
				if (projectile.alpha > 255)
				{
					projectile.alpha = 255;
					projectile.Kill();
					return;
				}
			}
			projectile.ai[0]++;
			if (projectile.ai[0] % 60f == 0f && projectile.ai[0] < 180f && Main.netMode != NetmodeID.MultiplayerClient)
			{
				int[] array6 = new int[5];
				Vector2[] array7 = new Vector2[5];
				int numberProjectiles = 0;
				float speed = 2000f;
				for (int t = 0; t < 255; t++)
				{
					if (!Main.player[t].active || Main.player[t].dead)
					{
						continue;
					}
					Vector2 center9 = Main.player[t].Center;
					float num865 = Vector2.Distance(center9, projectile.Center);
					if (num865 < speed && Collision.CanHit(projectile.Center, 1, 1, center9, 1, 1))
					{
						array6[numberProjectiles] = t;
						array7[numberProjectiles] = center9;
						int num866 = numberProjectiles + 1;
						numberProjectiles = num866;
						if (num866 >= array7.Length)
						{
							break;
						}
					}
				}
				for (int i = 0; i < numberProjectiles; i++)
				{
					Vector2 vector69 = array7[i] - projectile.Center;
					float ai = Main.rand.Next(100);
					Vector2 vector70 = Vector2.Normalize(vector69.RotatedByRandom(0.78539818525314331)) * 7f;
					Projectile.NewProjectile(projectile.Center, vector70, ProjectileType<TumblerOrbArc>(), 10, 0f, Main.myPlayer, vector69.ToRotation(), ai);
				}
			}                     
			Lighting.AddLight(projectile.Center, 0.4f, 0.85f, 0.9f);
			if (++projectile.frameCounter >= 4)
			{
				projectile.frameCounter = 0;
				if (++projectile.frame >= Main.projFrames[projectile.type])
				{
					projectile.frame = 0;
				}
			}
			if (projectile.alpha >= 150 || !(projectile.ai[0] < 180f))
			{
				return;
			}
			for (int num868 = 0; num868 < 1; num868++)
			{
				float num869 = (float)Main.rand.NextDouble() * 1f - 0.5f;
				if (num869 < -0.5f)
				{
					num869 = -0.5f;
				}
				if (num869 > 0.5f)
				{
					num869 = 0.5f;
				}
				Vector2 value42 = new Vector2((float)(-projectile.width) * 0.2f * projectile.scale, 0f).RotatedBy(num869 * ((float)Math.PI * 2f)).RotatedBy(projectile.velocity.ToRotation());
				int num870 = Dust.NewDust(projectile.Center - Vector2.One * 5f, 10, 10, 226, (0f - projectile.velocity.X) / 3f, (0f - projectile.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
				Main.dust[num870].position = projectile.Center + value42;
				Main.dust[num870].velocity = Vector2.Normalize(Main.dust[num870].position - projectile.Center) * 2f;
				Main.dust[num870].noGravity = true;
			}
			for (int num871 = 0; num871 < 1; num871++)
			{
				float num872 = (float)Main.rand.NextDouble() * 1f - 0.5f;
				if (num872 < -0.5f)
				{
					num872 = -0.5f;
				}
				if (num872 > 0.5f)
				{
					num872 = 0.5f;
				}
				Vector2 value43 = new Vector2((float)(-projectile.width) * 0.6f * projectile.scale, 0f).RotatedBy(num872 * ((float)Math.PI * 2f)).RotatedBy(projectile.velocity.ToRotation());
				int num873 = Dust.NewDust(projectile.Center - Vector2.One * 5f, 10, 10, 226, (0f - projectile.velocity.X) / 3f, (0f - projectile.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
				Main.dust[num873].velocity = Vector2.Zero;
				Main.dust[num873].position = projectile.Center + value43;
				Main.dust[num873].noGravity = true;
			}
		}
	}
}