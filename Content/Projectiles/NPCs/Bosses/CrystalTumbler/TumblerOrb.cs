using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;

namespace AerovelenceMod.Content.Projectiles.NPCs.Bosses.CrystalTumbler
{
	public class TumblerOrb : ModProjectile
	{
        public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 4;
		}
        public override void SetDefaults()
		{
			Projectile.width = 106;
			Projectile.height = 106;
			Projectile.aiStyle = 88;
			Projectile.damage = 15;
			Projectile.hostile = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}
		
		public override void AI()
		{
			Projectile.frameCounter++;
			if (Projectile.frameCounter % 7 == 0)
			{
				Projectile.frame++;
				Projectile.frameCounter = 0;
				if (Projectile.frame >= 4)
					Projectile.frame = 0;
			}
			if (Projectile.localAI[1] == 0f)
			{
				SoundEngine.PlaySound(SoundID.Item121, Projectile.position);
				Projectile.localAI[1] = 1f;
			}
			if (Projectile.ai[0] < 180f)
			{
				Projectile.alpha -= 5;
				if (Projectile.alpha < 0)
				{
					Projectile.alpha = 0;
				}
			}
			else
			{
				Projectile.alpha += 5;
				if (Projectile.alpha > 255)
				{
					Projectile.alpha = 255;
					Projectile.Kill();
					return;
				}
			}
			Projectile.ai[0]++;
			if (Projectile.ai[0] % 60f == 0f && Projectile.ai[0] < 180f && Main.netMode != NetmodeID.MultiplayerClient)
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
					float num865 = Vector2.Distance(center9, Projectile.Center);
					if (num865 < speed && Collision.CanHit(Projectile.Center, 1, 1, center9, 1, 1))
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
					Vector2 vector69 = array7[i] - Projectile.Center;
					float ai = Main.rand.Next(100);
					Vector2 vector70 = Vector2.Normalize(vector69.RotatedByRandom(0.78539818525314331)) * 7f;
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vector70, ProjectileType<TumblerOrbArc>(), 10, 0f, Main.myPlayer, vector69.ToRotation(), ai);
				}
			}                     
			Lighting.AddLight(Projectile.Center, 0.4f, 0.85f, 0.9f);
			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
			if (Projectile.alpha >= 150 || !(Projectile.ai[0] < 180f))
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
				Vector2 value42 = new Vector2((float)(-Projectile.width) * 0.2f * Projectile.scale, 0f).RotatedBy(num869 * ((float)Math.PI * 2f)).RotatedBy(Projectile.velocity.ToRotation());
				int num870 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, 226, (0f - Projectile.velocity.X) / 3f, (0f - Projectile.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
				Main.dust[num870].position = Projectile.Center + value42;
				Main.dust[num870].velocity = Vector2.Normalize(Main.dust[num870].position - Projectile.Center) * 2f;
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
				Vector2 value43 = new Vector2((float)(-Projectile.width) * 0.6f * Projectile.scale, 0f).RotatedBy(num872 * ((float)Math.PI * 2f)).RotatedBy(Projectile.velocity.ToRotation());
				int num873 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, 226, (0f - Projectile.velocity.X) / 3f, (0f - Projectile.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
				Main.dust[num873].velocity = Vector2.Zero;
				Main.dust[num873].position = Projectile.Center + value43;
				Main.dust[num873].noGravity = true;
			}
		}
	}
}