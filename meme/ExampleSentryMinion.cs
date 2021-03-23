using AerovelenceMod.Content.Buffs;
using AerovelenceMod.Content.Items.Weapons.Ranged;
using AerovelenceMod.Content.Projectiles.Weapons.Minions;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.meme
{
	public class ExampleSentryMinion : ModProjectile
	{
		int i;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Example Sentry");
			Main.projFrames[projectile.type] = 12;
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
		}
		public enum ExampleSentryState
		{
			FirstProjectileShoot = 0,
			SecondProjectileShoot = 1,
			ThirdProjectileShoot = 2
		}

		private ExampleSentryState State
		{
			get => (ExampleSentryState)projectile.ai[0];
			set => projectile.ai[0] = (float)value;
		}

		private float AttackTimer
		{
			get => projectile.ai[1];
			set => projectile.ai[1] = value;
		}
		public override void SetDefaults()
		{
			projectile.width = 44;
			projectile.height = 26;
			projectile.minion = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;
			projectile.alpha = 0;
			projectile.sentry = true;
			projectile.penetrate = -1;
			projectile.timeLeft = 18000;
			projectile.minionSlots = 1;
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
			if (Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<ExampleSentryMinion>()] > 1)
			{
				projectile.Kill();
			}
			if (State == ExampleSentryState.FirstProjectileShoot)
			{
				if (++AttackTimer >= 200)
				{
					AttackTimer = 0;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						State = ExampleSentryState.SecondProjectileShoot;
					}
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
					if (projectile.owner != Main.myPlayer)
					{
						return;
					}
					if (projectile.ai[0] == 0f)
					{
						float projposX = projectile.position.X;
						float projPosY = projectile.position.Y;
						float distance = 700f;
						bool npcNearby = false;
						for (int k = 0; k < 200; k++)
						{
							if (Main.npc[k].CanBeChasedBy(this, ignoreDontTakeDamage: true))
							{
								float toPosX = Main.npc[k].position.X + Main.npc[k].width / 2;
								float toPosY = Main.npc[k].position.Y + Main.npc[k].height / 2;
								float npcDist = Math.Abs(projectile.position.X + projectile.width / 2 - toPosX) + Math.Abs(projectile.position.Y + projectile.height / 2 - toPosY);
								if (npcDist < distance && Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[k].position, Main.npc[k].width, Main.npc[k].height))
								{
									distance = npcDist;
									projposX = toPosX;
									projPosY = toPosY;
									npcNearby = true;
								}
							}
						}
						if (npcNearby)
						{
							float shootSpeed = 12f;
							Vector2 pos = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
							float x = projposX - pos.X;
							float y = projPosY - pos.Y;
							float sentryPos = (float)Math.Sqrt(x * x + y * y);
							sentryPos = shootSpeed / sentryPos;
							x *= sentryPos;
							y *= sentryPos;
							Projectile.NewProjectile(projectile.Center.X - 4f, projectile.Center.Y, projectile.position.X, projectile.position.Y, ProjectileID.LaserMachinegunLaser, 50, 1, projectile.owner);
							projectile.ai[0] = 50f;
						}
					}
					else
					{
						projectile.ai[0] -= 1f;
					}
				}
			}
			else if (State == ExampleSentryState.SecondProjectileShoot)
			{
				if (AttackTimer++ == 0)
				{
					Projectile.NewProjectile(projectile.Center.X - 4f, projectile.Center.Y, projectile.position.X, projectile.position.Y, ProjectileID.DD2LightningAuraT1, 50, 1, projectile.owner);
				}
				else if (AttackTimer == 10)
				{
					Projectile.NewProjectile(projectile.Center.X - 4f, projectile.Center.Y, projectile.position.X, projectile.position.Y, ProjectileID.DD2LightningAuraT1, 50, 1, projectile.owner);
				}
				else if (AttackTimer == 20)
				{
					Projectile.NewProjectile(projectile.Center.X - 4f, projectile.Center.Y, projectile.position.X, projectile.position.Y, ProjectileID.DD2LightningAuraT1, 50, 1, projectile.owner);
					AttackTimer = 0;
					State = ExampleSentryState.ThirdProjectileShoot;
				}
			}
			else if (State == ExampleSentryState.ThirdProjectileShoot)
			{
				if (AttackTimer++ == 0)
				{
				}
				else if (AttackTimer == 10)
				{

				}
				else if (AttackTimer == 20)
				{
					State = ExampleSentryState.FirstProjectileShoot;
				}
			}
		}
	}
}
/*public override void AI()
{
	Player player = Main.player[projectile.owner];
	ExamplePlayer modPlayer = player.GetModPlayer<ExamplePlayer>();

	if (Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<ExampleSentryMinion>()] > 1)
	{
		projectile.Kill();
	}

	float distance = 700f;
	bool npcNearby = false;
	if (projectile.ai[0] == 0f)
	{
		for (int k = 0; k < 200; k++)
		{
			if (Main.npc[k].CanBeChasedBy(this, ignoreDontTakeDamage: true))
			{
				float toPosX = Main.npc[k].position.X + Main.npc[k].width / 2;
				float toPosY = Main.npc[k].position.Y + Main.npc[k].height / 2;
				float npcDist = Math.Abs(projectile.position.X + projectile.width / 2 - toPosX) + Math.Abs(projectile.position.Y + projectile.height / 2 - toPosY);
				if (npcDist < distance && Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[k].position, Main.npc[k].width, Main.npc[k].height))
				{
					distance = npcDist;
					npcNearby = true;
				}
			}
			if (npcNearby)
			{
				float posX = projectile.position.X;
				float posY = projectile.position.Y;
				float fireSpeed = 12f;
				Vector2 pos = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
				float npcPosX = posX - pos.X;
				float npcPosY = posY - pos.Y;
				float sentryPos = (float)Math.Sqrt(npcPosX * npcPosX + npcPosY * npcPosY);
				sentryPos = fireSpeed / sentryPos;
				npcPosX *= sentryPos;
				npcPosY *= sentryPos;
				Projectile.NewProjectile(projectile.Center.X - 4f, projectile.Center.Y, npcPosX, npcPosY, ModContent.ProjectileType<SkylightProjectile>(), Player.crystalLeafDamage, Player.crystalLeafKB, projectile.owner);
				projectile.ai[0] = 50f;
			}
		}

		if (player.dead)
		{
			modPlayer.ExampleSentry = false;
		}
		if (modPlayer.ExampleSentry)
		{
			projectile.timeLeft = 2;
		}
		i++;


		projectile.frameCounter++;
		if (projectile.frameCounter % 10 == 0)
		{
			projectile.frame++;
			projectile.frameCounter = 0;
			if (projectile.frame >= 6)
				projectile.frame = 0;
		}

		if (i % 2 == 0)
		{
			Dust.NewDust(projectile.position, projectile.width / 2, projectile.height / 2, 132);
		}
	}
}*/