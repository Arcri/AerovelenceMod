using System;
using AerovelenceMod.Content.Buffs;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
    public class ShatteredSoul : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.Homing[projectile.type] = true;
		}
		public override void SetDefaults()
		{
			projectile.width = 8;
			projectile.height = 8;
			projectile.alpha = 175;
			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
			projectile.ranged = true;
			projectile.extraUpdates = 2;
		}
		int counter = 0;
		public override void AI()
		{
			if (projectile.alpha > 30)
			{
				projectile.alpha -= 15;
				if (projectile.alpha < 30)
				{
					projectile.alpha = 30;
				}
			}
			if (projectile.alpha <= 30)
			{
				int dust = Dust.NewDust(projectile.Center - new Vector2(5), 0, 0, DustType<WispDust>());
				Main.dust[dust].velocity *= 0.1f;
				counter++;
				if(counter >= 10)
				{
					float minDist = 480;
					int target2 = -1;
					float dX = 0f;
					float dY = 0f;
					float distance = 0;
					float speed = 1.4f;
					if (projectile.friendly == true && projectile.hostile == false)
					{
						for (int i = 0; i < Main.npc.Length; i++)
						{
							NPC target = Main.npc[i];
							if (!target.friendly && target.dontTakeDamage == false && target.lifeMax > 5 && target.active && target.CanBeChasedBy())
							{
								dX = target.Center.X - projectile.Center.X;
								dY = target.Center.Y - projectile.Center.Y;
								distance = (float)Math.Sqrt((double)(dX * dX + dY * dY));
								if (distance < minDist)
								{
									bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height);
									if (lineOfSight)
									{
										minDist = distance;
										target2 = i;
									}
								}
							}
						}
						if (target2 != -1)
						{
							NPC toHit = Main.npc[target2];
							if (toHit.active == true)
							{
								dX = toHit.Center.X - projectile.Center.X;
								dY = toHit.Center.Y - projectile.Center.Y;
								distance = (float)Math.Sqrt((double)(dX * dX + dY * dY));
								speed /= distance;
								projectile.velocity *= 0.85f;
								projectile.velocity += new Vector2(dX * speed, dY * speed);
							}
						}
					}
				}
			}
		}
		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(projectile.Center - new Vector2(5), 0, 0, DustType<WispDust>(), 0, 0, projectile.alpha);
				dust.velocity *= 0.55f;
				dust.velocity += projectile.velocity * 0.5f;
				dust.scale *= 1.75f;
				dust.noGravity = true;
			}
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.NextBool())
			{
				target.AddBuff(BuffType<SoulFire>(), 300);
			}
		}
	}
}