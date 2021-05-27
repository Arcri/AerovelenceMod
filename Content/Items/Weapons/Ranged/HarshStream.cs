using AerovelenceMod.Content.Items.Placeables.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
	public class HarshStream : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Harsh Stream");
			Tooltip.SetDefault("Replaces arrows with a bolt of hot sand");
		}
		public override void SetDefaults()
		{
			item.UseSound = SoundID.Item5;
			item.crit = 4;
			item.damage = 25;
			item.ranged = true;
			item.width = 20;
			item.height = 40;
			item.useTime = 26;
			item.useAnimation = 26;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.knockBack = 2;
			item.value = Item.sellPrice(0, 0, 20, 0);
			item.rare = ItemRarityID.Blue;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<HarshStreamStream>();
			item.useAmmo = AmmoID.Arrow;
			item.shootSpeed = 10f;
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<SlateOre>(), 30);
			recipe.AddRecipeGroup("Wood", 10);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			type = ModContent.ProjectileType<HarshStreamStream>();
			return true;
		}
	}
	public class HarshStreamStream : ModProjectile
	{
        public override void SetDefaults()
        {
			projectile.width = 8;
			projectile.height = 8;
			projectile.aiStyle = 1;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.ranged = true;
			projectile.penetrate = 5;
			projectile.timeLeft = 600;
			projectile.ignoreWater = true;
			projectile.tileCollide = true;
			projectile.extraUpdates = 1;
			projectile.alpha = 255;
			projectile.timeLeft = 500;
		}
        public override void AI()
		{
			projectile.velocity.Y += 0.10f;
			for (int t = 0; t < 1; t++)
			{
				for (int num600 = 0; num600 < 6; num600++)
				{
					float num601 = projectile.velocity.X / 6f * num600;
					float num602 = projectile.velocity.Y / 6f * num600;
					int num603 = 6;
					int num604 = Dust.NewDust(new Vector2(projectile.position.X + num603, projectile.position.Y + num603), projectile.width - num603 * 2, projectile.height - num603 * 2, DustID.AmberBolt, 0f, 0f, 75, default(Color), 1.2f);
					Dust dust94;
					Dust dust2;
					if (Main.rand.Next(2) == 0)
					{
						dust94 = Main.dust[num604];
						dust2 = dust94;
						dust2.alpha += 25;
					}
					if (Main.rand.Next(2) == 0)
					{
						dust94 = Main.dust[num604];
						dust2 = dust94;
						dust2.alpha += 25;
					}
					if (Main.rand.Next(2) == 0)
					{
						dust94 = Main.dust[num604];
						dust2 = dust94;
						dust2.alpha += 25;
					}
					Main.dust[num604].noGravity = true;
					dust94 = Main.dust[num604];
					dust2 = dust94;
					dust2.velocity *= 0.3f;
					dust94 = Main.dust[num604];
					dust2 = dust94;
					dust2.velocity += projectile.velocity * 0.5f;
					Main.dust[num604].position = projectile.Center;
					Main.dust[num604].position.X -= num601;
					Main.dust[num604].position.Y -= num602;
					dust94 = Main.dust[num604];
					dust2 = dust94;
					dust2.velocity *= 0.2f;
				}
				if (Main.rand.Next(4) == 0)
				{
					int num605 = 6;
					int num606 = Dust.NewDust(new Vector2(projectile.position.X + num605, projectile.position.Y + num605), projectile.width - num605 * 2, projectile.height - num605 * 2, DustID.AmberBolt, 0f, 0f, 75, default(Color), 0.65f);
					Dust dust95 = Main.dust[num606];
					Dust dust2 = dust95;
					dust2.velocity *= 0.5f;
					dust95 = Main.dust[num606];
					dust2 = dust95;
					dust2.velocity += projectile.velocity * 0.5f;
				}
			}
		}
		public override void Kill(int timeLeft)
		{
			projectile.velocity = projectile.oldVelocity * 0.2f;
			for (int num361 = 0; num361 < 100; num361++)
			{
				int num362 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.AmberBolt, 0f, 0f, 75, default(Color), 1.2f);
				Dust dust120;
				Dust dust2;
				if (Main.rand.Next(2) == 0)
				{
					dust120 = Main.dust[num362];
					dust2 = dust120;
					dust2.alpha += 25;
				}
				if (Main.rand.Next(2) == 0)
				{
					dust120 = Main.dust[num362];
					dust2 = dust120;
					dust2.alpha += 25;
				}
				if (Main.rand.Next(2) == 0)
				{
					dust120 = Main.dust[num362];
					dust2 = dust120;
					dust2.alpha += 25;
				}
				if (Main.rand.Next(2) == 0)
				{
					Main.dust[num362].scale = 0.6f;
				}
				else
				{
					Main.dust[num362].noGravity = true;
				}
				dust120 = Main.dust[num362];
				dust2 = dust120;
				dust2.velocity *= 0.3f;
				dust120 = Main.dust[num362];
				dust2 = dust120;
				dust2.velocity += projectile.velocity;
				dust120 = Main.dust[num362];
				dust2 = dust120;
				dust2.velocity *= 1f + Main.rand.Next(-100, 101) * 0.01f;
				Main.dust[num362].velocity.X += Main.rand.Next(-50, 51) * 0.015f;
				Main.dust[num362].velocity.Y += Main.rand.Next(-50, 51) * 0.015f;
				Main.dust[num362].position = projectile.Center;
			}
		}
	}
}