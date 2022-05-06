using System;
using AerovelenceMod.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
	public class BeholderOrb : ModProjectile
	{
		int i;
		float rot = 0f;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Beholder Orb");
		}

		public override void SetDefaults()
		{
			Projectile.width = 52;
			Projectile.height = 52;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.damage = 50;
			Projectile.timeLeft = 120;
			Projectile.light = 0.5f;
		}
		public override void AI()
		{
			i++;
			Player player = Main.player[Projectile.owner];
			Vector2 moveTo = Main.MouseWorld;
			float speed = 10f;
			Vector2 move = moveTo - Projectile.Center;
			float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
			move *= speed / magnitude;
			if (i % 5 == 0)
			{
				Projectile.velocity += move;
			}
			Projectile.rotation += rot;
			Projectile.scale *= 1.005f;
			if (i % 2 == 0)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, 132);
			}
			Projectile.alpha += 2;
			rot *= 0.99f;
			if (Projectile.ai[0] == 0f)
			{
				Projectile.ai[0] = Projectile.velocity.X;
				Projectile.ai[1] = Projectile.velocity.Y;
			}
			if (Math.Sqrt(Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y) > 2.0)
			{
				Projectile.velocity *= 0.99f;
			}
			int[] array = new int[20];
			int num438 = 0;
			float num439 = 1000f;
			bool flag14 = false;
			float num440 = 0f;
			float num441 = 0f;
			for (int num442 = 0; num442 < 200; num442++)
			{
				if (!Main.npc[num442].CanBeChasedBy(this))
				{
					continue;
				}
				float num443 = Main.npc[num442].position.X + Main.npc[num442].width / 2;
				float num444 = Main.npc[num442].position.Y + Main.npc[num442].height / 2;
				float num445 = Math.Abs(Projectile.position.X + Projectile.width / 2 - num443) + Math.Abs(Projectile.position.Y + Projectile.height / 2 - num444);
				if (num445 < num439 && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[num442].Center, 1, 1))
				{
					if (num438 < 20)
					{
						array[num438] = num442;
						num438++;
						num440 = num443;
						num441 = num444;
					}
					flag14 = true;
				}
			}
			if (Projectile.timeLeft < 30)
			{
				flag14 = false;
			}
			if (flag14)
			{
				int num446 = Main.rand.Next(num438);
				num446 = array[num446];
				num440 = Main.npc[num446].position.X + Main.npc[num446].width / 2;
				num441 = Main.npc[num446].position.Y + Main.npc[num446].height / 2;
				Projectile.localAI[0] += 1f;
				if (Projectile.localAI[0] > 8f)
				{
					Projectile.localAI[0] = 0f;
					float num447 = 6f;
					Vector2 vector31 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
					vector31 += Projectile.velocity * 4f;
					float num448 = num440 - vector31.X;
					float num449 = num441 - vector31.Y;
					float num450 = (float)Math.Sqrt(num448 * num448 + num449 * num449);
					num450 = num447 / num450;
					num448 *= num450;
					num449 *= num450;
					Projectile.NewProjectile(vector31.X, vector31.Y, num448, num449, ModContent.ProjectileType<CavernousImpalerProjectile3>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
				}
			}
		}
	}
}