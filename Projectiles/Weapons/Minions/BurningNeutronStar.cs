using AerovelenceMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Projectiles.Weapons.Minions
{

	public class BurningNeutronStar : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 54;
			projectile.height = 54;
			projectile.minion = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = true;
			projectile.netImportant = true;
			aiType = -1;
			projectile.alpha = 0;
			projectile.penetrate = -10;
			projectile.timeLeft = 18000;
			projectile.minionSlots = 2;
		}
		public override bool? CanCutTiles()
		{
			return true;
		}
		public override bool MinionContactDamage()
		{
			return true;
		}
		public override void AI()
		{
			bool minion = projectile.type == ModContent.ProjectileType<BurningNeutronStar>();
			AeroPlayer mp = Main.player[projectile.owner].GetModPlayer<AeroPlayer>();
			Player player = Main.player[projectile.owner];
			if (Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<BurningNeutronStar>()] > 1)
			{
				projectile.Kill();
			}
			{
				projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
				projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2) + Main.player[projectile.owner].gfxOffY - 60f;
				if (Main.player[projectile.owner].gravDir == -1f)
				{
					projectile.position.Y += 120f;
					projectile.rotation = 3.14f;
				}
				else
				{
					projectile.rotation = 0f;
				}
				projectile.position.X = (int)projectile.position.X;
				projectile.position.Y = (int)projectile.position.Y;
				float num406 = (float)(int)Main.mouseTextColor / 200f - 0.35f;
				num406 *= 0.2f;
				projectile.scale = num406 + 0.95f;
				if (projectile.owner != Main.myPlayer)
				{
					return;
				}
				if (projectile.ai[0] == 0f)
				{
					float num407 = projectile.position.X;
					float num408 = projectile.position.Y;
					float num409 = 700f;
					bool flag11 = false;
					for (int num410 = 0; num410 < 200; num410++)
					{
						if (Main.npc[num410].CanBeChasedBy(this, ignoreDontTakeDamage: true))
						{
							float num411 = Main.npc[num410].position.X + (float)(Main.npc[num410].width / 2);
							float num412 = Main.npc[num410].position.Y + (float)(Main.npc[num410].height / 2);
							float num413 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num411) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num412);
							if (num413 < num409 && Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[num410].position, Main.npc[num410].width, Main.npc[num410].height))
							{
								num409 = num413;
								num407 = num411;
								num408 = num412;
								flag11 = true;
							}
						}
					}
					if (flag11)
					{
						float num414 = 12f;
						Vector2 vector30 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
						float num415 = num407 - vector30.X;
						float num416 = num408 - vector30.Y;
						float num417 = (float)Math.Sqrt(num415 * num415 + num416 * num416);
						float num418 = num417;
						num417 = num414 / num417;
						num415 *= num417;
						num416 *= num417;
						Projectile.NewProjectile(projectile.Center.X - 4f, projectile.Center.Y, num415, num416, ModContent.ProjectileType<SkylightProjectile>(), Player.crystalLeafDamage, Player.crystalLeafKB, projectile.owner);
						projectile.ai[0] = 50f;
					}
				}
				else
				{
					projectile.ai[0] -= 1f;
				}
			}
		}
	}
}