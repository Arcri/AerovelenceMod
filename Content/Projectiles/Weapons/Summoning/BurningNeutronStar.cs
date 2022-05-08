using System;
using AerovelenceMod.Content.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{

	public class BurningNeutronStar : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 54;
			Projectile.height = 54;
			Projectile.minion = true;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.netImportant = true;
			AIType = -1;
			Projectile.alpha = 0;
			Projectile.penetrate = -10;
			Projectile.timeLeft = 18000;
			Projectile.minionSlots = 2;
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
			bool minion = Projectile.type == ModContent.ProjectileType<BurningNeutronStar>();
			AeroPlayer mp = Main.player[Projectile.owner].GetModPlayer<AeroPlayer>();
			Player player = Main.player[Projectile.owner];
			if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<BurningNeutronStar>()] > 1)
			{
				Projectile.Kill();
			}
			{
				Projectile.position.X = Main.player[Projectile.owner].Center.X - (float)(Projectile.width / 2);
				Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (float)(Projectile.height / 2) + Main.player[Projectile.owner].gfxOffY - 60f;
				if (Main.player[Projectile.owner].gravDir == -1f)
				{
					Projectile.position.Y += 120f;
					Projectile.rotation = 3.14f;
				}
				else
				{
					Projectile.rotation = 0f;
				}
				Projectile.position.X = (int)Projectile.position.X;
				Projectile.position.Y = (int)Projectile.position.Y;
				float num406 = (float)(int)Main.mouseTextColor / 200f - 0.35f;
				num406 *= 0.2f;
				Projectile.scale = num406 + 0.95f;
				if (Projectile.owner != Main.myPlayer)
				{
					return;
				}
				if (Projectile.ai[0] == 0f)
				{
					float num407 = Projectile.position.X;
					float num408 = Projectile.position.Y;
					float num409 = 700f;
					bool flag11 = false;
					for (int num410 = 0; num410 < 200; num410++)
					{
						if (Main.npc[num410].CanBeChasedBy(this, ignoreDontTakeDamage: true))
						{
							float num411 = Main.npc[num410].position.X + (float)(Main.npc[num410].width / 2);
							float num412 = Main.npc[num410].position.Y + (float)(Main.npc[num410].height / 2);
							float num413 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num411) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num412);
							if (num413 < num409 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[num410].position, Main.npc[num410].width, Main.npc[num410].height))
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
						Vector2 vector30 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
						float num415 = num407 - vector30.X;
						float num416 = num408 - vector30.Y;
						float num417 = (float)Math.Sqrt(num415 * num415 + num416 * num416);
						float num418 = num417;
						num417 = num414 / num417;
						num415 *= num417;
						num416 *= num417;
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X - 4f, Projectile.Center.Y, num415, num416, ModContent.ProjectileType<SkylightProjectile>(), Player.crystalLeafDamage, Player.crystalLeafKB, Projectile.owner);
						Projectile.ai[0] = 50f;
					}
				}
				else
				{
					Projectile.ai[0] -= 1f;
				}
			}
		}
	}
}