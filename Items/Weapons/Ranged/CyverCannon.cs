using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class CyverCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyver-Cannon");
            Tooltip.SetDefault("Fires lasers that split off into homing bolts");
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 20;
            item.useTime = 20;
            item.shootSpeed = 20f;
            item.knockBack = 2f;
            item.width = 70;
            item.height = 38;
            item.damage = 30;
            item.shoot = mod.ProjectileType("CyverCannonProj");
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 5, 20, 0);
            item.noMelee = true;
            item.noUseGraphic = true;
            item.ranged = true;
            item.channel = true;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class CyverCannonProj : ModProjectile
	{
		int i;
		public int Timer;
		public float shootSpeed = 0.5f;

		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 6;
		}

		public override void SetDefaults()
		{
			projectile.width = 38;
			projectile.height = 70;
			projectile.aiStyle = 75;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.hide = true;
			projectile.ranged = true;
			projectile.ignoreWater = true;
		}


		public override void AI()
		{
			Player player = Main.player[projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter);
			{
				projectile.ai[0] += 1f;
				int num2 = 0;
				if (projectile.ai[0] >= 40f)
				{
					num2++;
				}
				if (projectile.ai[0] >= 80f)
				{
					num2++;
				}
				if (projectile.ai[0] >= 120f)
				{
					num2++;
				}
				int num3 = 24;
				int num4 = 6;
				projectile.ai[1] += 1f;
				bool flag = false;
				if (projectile.ai[1] >= (float)(num3 - num4 * num2))
				{
					projectile.ai[1] = 0f;
					flag = true;
				}
				projectile.frameCounter += 1 + num2;
				if (projectile.frameCounter >= 4)
				{
					projectile.frameCounter = 0;
					projectile.frame++;
					if (projectile.frame >= 6)
					{
						projectile.frame = 0;
					}
				}
				if (projectile.soundDelay <= 0)
				{
					projectile.soundDelay = num3 - num4 * num2;
					if (projectile.ai[0] != 1f)
					{
						Main.PlaySound(SoundID.Item91, base.projectile.position);
					}
				}
				if (projectile.ai[1] == 1f && projectile.ai[0] != 1f)
				{
					Vector2 spinningpoint = Vector2.UnitX * 24f;
					spinningpoint = spinningpoint.RotatedBy(projectile.rotation - (float)Math.PI / 2f);
					Vector2 value = base.projectile.Center + spinningpoint;
					for (int i = 0; i < 2; i++)
					{
						int num5 = Dust.NewDust(value - Vector2.One * 8f, 16, 16, 135, base.projectile.velocity.X / 2f, base.projectile.velocity.Y / 2f, 100);
						Main.dust[num5].velocity *= 0.66f;
						Main.dust[num5].noGravity = true;
						Main.dust[num5].scale = 1.4f;
					}
				}
				if (flag && Main.myPlayer == projectile.owner)
				{
					if (player.channel && player.CheckMana(player.inventory[player.selectedItem], -1, pay: true) && !player.noItems && !player.CCed)
					{
						float num6 = player.inventory[player.selectedItem].shootSpeed * projectile.scale;
						Vector2 value2 = vector;
						Vector2 value3 = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY) - value2;
						if (player.gravDir == -1f)
						{
							value3.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - value2.Y;
						}
						Vector2 velocity = Vector2.Normalize(value3);
						if (float.IsNaN(velocity.X) || float.IsNaN(velocity.Y))
						{
							velocity = -Vector2.UnitY;
						}
						velocity *= num6;
						if (velocity.X != base.projectile.velocity.X || velocity.Y != base.projectile.velocity.Y)
						{
							projectile.netUpdate = true;
						}
						base.projectile.velocity = velocity;
						float scaleFactor = 14f;
						int num8 = 7;
						int DarkLaser = mod.ProjectileType("DarkLaser");
						for (int j = 0; j < 2; j++)
						{
							value2 = base.projectile.Center + new Vector2(Main.rand.Next(-num8, num8 + 1), Main.rand.Next(-num8, num8 + 1));
							Vector2 spinningpoint2 = Vector2.Normalize(base.projectile.velocity) * scaleFactor;
							spinningpoint2 = spinningpoint2.RotatedBy(Main.rand.NextDouble() * 0.19634954631328583 - 0.098174773156642914);
							if (float.IsNaN(spinningpoint2.X) || float.IsNaN(spinningpoint2.Y))
							{
								spinningpoint2 = -Vector2.UnitY;
							}
							Projectile.NewProjectile(value2.X, value2.Y, spinningpoint2.X, spinningpoint2.Y, DarkLaser, projectile.damage, projectile.knockBack, projectile.owner);
						}
					}
					else
					{
						projectile.Kill();
					}
				}
			}
		}
		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = mod.GetTexture("Items/Weapons/Ranged/CyverCannonProj_Glow");
			spriteBatch.Draw(
				texture,
				new Vector2
				(
					projectile.Center.Y - Main.screenPosition.X,
					projectile.Center.X - Main.screenPosition.Y
				),
				new Rectangle(0, 0, texture.Width, texture.Height),
				Color.White,
				projectile.rotation,
				texture.Size(),
				projectile.scale,
				SpriteEffects.None,
				0f
			);
		}
	}
}

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class DarkLaser : ModProjectile
	{
		int t;
		public int i;
		public int counter = 0;
		public override void SetDefaults()
		{
			projectile.width = 5;
			projectile.height = 5;
			projectile.friendly = true;
			projectile.alpha = 255;
			projectile.extraUpdates = 2;
			projectile.scale = 1f;
			projectile.timeLeft = 600;
			projectile.magic = true;
			projectile.ignoreWater = true;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}
		public override void AI()
		{

			int num294 = Main.rand.Next(3, 7);
			for (int num295 = 0; num295 < num294; num295++)
			{
				counter++;
				if (counter >= 17)
				{
					int num296 = Dust.NewDust(base.projectile.Center - projectile.velocity / 2f, 0, 0, 242, 0f, 0f, 100, default(Color), 2.1f);
					Dust dust105 = Main.dust[num296];
					Dust dust2 = dust105;
					dust2.velocity *= 2f;
					Main.dust[num296].noGravity = true;
				}
			}
			if (projectile.ai[1] != 1f)
			{
				projectile.ai[1] = 1f;
				base.projectile.position += base.projectile.velocity;
				projectile.velocity = projectile.velocity;
			}
			i++;
			if (i % Main.rand.Next(1, 50) == 0)
			{
				Projectile.NewProjectile(projectile.Center.X + projectile.velocity.X, projectile.Center.Y + projectile.velocity.Y, projectile.velocity.X - 2f, projectile.velocity.Y - 2, ModContent.ProjectileType<CannonSplit>(), projectile.damage, projectile.knockBack * 0.85f, projectile.owner, 0f, 0f);
			}
		}
	}
}

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class CannonSplit : ModProjectile
	{
		int t;
		public int i;
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.Homing[projectile.type] = true;
		}
		public override void SetDefaults()
		{
			projectile.width = 5;
			projectile.height = 5;
			projectile.friendly = true;
			projectile.extraUpdates = 2;
			projectile.scale = 1f;
			projectile.timeLeft = 600;
			projectile.ranged = true;
			projectile.ignoreWater = true;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}
		public override void AI()
		{
			int count = 0;

			foreach (Projectile proj in Main.projectile.Where(x => x.active && x.whoAmI != projectile.whoAmI && x.type == projectile.type))
			{
				count++;

				if (count >= 5)
					proj.Kill();
			}
			if (projectile.alpha > 30)
			{
				projectile.alpha -= 15;
				if (projectile.alpha < 30)
				{
					projectile.alpha = 30;
				}
			}
			
			if (projectile.localAI[0] == 0f)
			{
				AdjustMagnitude(ref projectile.velocity);
				projectile.localAI[0] = 1f;
			}
			Vector2 move = Vector2.Zero;
			float distance = 400f;
			bool target = false;
			for (int k = 0; k < 200; k++)
			{
				if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && Main.npc[k].type != NPCID.TargetDummy)
				{
					Vector2 newMove = Main.npc[k].Center - projectile.Center;
					float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
					if (distanceTo < distance)
					{
						move = newMove;
						distance = distanceTo;
						target = true;
					}
				}
				projectile.rotation = projectile.velocity.ToRotation();
			}
			if (target)
			{
				AdjustMagnitude(ref move);
				projectile.velocity = (5 * projectile.velocity + move) / 6f;
				AdjustMagnitude(ref projectile.velocity);
			}
			if (projectile.alpha <= 30)
			{
				int dust = Dust.NewDust(projectile.position, projectile.width / 2, projectile.height / 2, 242);
				Main.dust[dust].velocity *= 0.1f;
				Main.dust[dust].noGravity = true;
			}
		}

		private void AdjustMagnitude(ref Vector2 vector)
		{
			float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
			if (magnitude > 3f)
			{
				vector *= 5f / magnitude;
			}
		}
	}
}