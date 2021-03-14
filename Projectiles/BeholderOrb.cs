using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using AerovelenceMod.Dusts;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Items.Weapons.Melee;

namespace AerovelenceMod.Projectiles
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
			projectile.width = 52;
			projectile.height = 52;
			projectile.melee = true;
			projectile.damage = 50;
			projectile.timeLeft = 120;
			projectile.light = 0.5f;
		}
		public override void AI()
		{
			i++;
			projectile.rotation += rot;
			projectile.scale *= 1.005f;
			if (i % 2 == 0)
			{
				int dust = Dust.NewDust(projectile.position, projectile.width / 2, projectile.height / 2, 132);
			}
			projectile.alpha += 2;
			rot *= 0.99f;
			if (projectile.ai[0] == 0f)
			{
				projectile.ai[0] = projectile.velocity.X;
				projectile.ai[1] = projectile.velocity.Y;
			}
			if (Math.Sqrt(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y) > 2.0)
			{
				projectile.velocity *= 0.99f;
			}
			for (int num437 = 0; num437 < 1000; num437++)
			{
				if (num437 != projectile.whoAmI && Main.projectile[num437].active && Main.projectile[num437].owner == projectile.owner && Main.projectile[num437].type == projectile.type && projectile.timeLeft > Main.projectile[num437].timeLeft && Main.projectile[num437].timeLeft > 30)
				{
					projectile.alpha += 10;
					Main.projectile[num437].timeLeft = 30;
				}
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
				float num445 = Math.Abs(projectile.position.X + projectile.width / 2 - num443) + Math.Abs(projectile.position.Y + projectile.height / 2 - num444);
				if (num445 < num439 && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num442].Center, 1, 1))
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
			if (projectile.timeLeft < 30)
			{
				flag14 = false;
			}
			if (flag14)
			{
				int num446 = Main.rand.Next(num438);
				num446 = array[num446];
				num440 = Main.npc[num446].position.X + Main.npc[num446].width / 2;
				num441 = Main.npc[num446].position.Y + Main.npc[num446].height / 2;
				projectile.localAI[0] += 1f;
				if (projectile.localAI[0] > 8f)
				{
					projectile.localAI[0] = 0f;
					float num447 = 6f;
					Vector2 vector31 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
					vector31 += projectile.velocity * 4f;
					float num448 = num440 - vector31.X;
					float num449 = num441 - vector31.Y;
					float num450 = (float)Math.Sqrt(num448 * num448 + num449 * num449);
					float num451 = num450;
					num450 = num447 / num450;
					num448 *= num450;
					num449 *= num450;
					Projectile.NewProjectile(vector31.X, vector31.Y, num448, num449, ModContent.ProjectileType<CavernousImpalerProjectile3>(), projectile.damage, projectile.knockBack, projectile.owner);
				}
			}
		}
	}
}