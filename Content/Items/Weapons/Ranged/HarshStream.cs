using AerovelenceMod.Content.Items.Others.Crafting;
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
			Item.UseSound = SoundID.Item5;
			Item.crit = 4;
			Item.damage = 25;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 20;
			Item.height = 40;
			Item.useTime = 26;
			Item.useAnimation = 26;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 2;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<HarshStreamStream>();
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 10f;
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ModContent.ItemType<HugeAntlionMandible>(), 2)
				.AddIngredient(ItemID.Sandstone, 25)
				.AddIngredient(ItemID.Cactus, 5)
				.AddTile(TileID.Anvils)
				.Register();
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			type = ModContent.ProjectileType<HarshStreamStream>();
			return true;
		}
	}
	public class HarshStreamStream : ModProjectile
	{
        public override void SetDefaults()
        {
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 5;
			Projectile.timeLeft = 600;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
			Projectile.alpha = 255;
			Projectile.timeLeft = 500;
		}
        public override void AI()
		{
			Projectile.velocity.Y += 0.10f;
			for (int t = 0; t < 1; t++)
			{
				for (int num600 = 0; num600 < 6; num600++)
				{
					float num601 = Projectile.velocity.X / 6f * num600;
					float num602 = Projectile.velocity.Y / 6f * num600;
					int num603 = 6;
					int num604 = Dust.NewDust(new Vector2(Projectile.position.X + num603, Projectile.position.Y + num603), Projectile.width - num603 * 2, Projectile.height - num603 * 2, DustID.AmberBolt, 0f, 0f, 75, default(Color), 1.2f);
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
					dust2.velocity += Projectile.velocity * 0.5f;
					Main.dust[num604].position = Projectile.Center;
					Main.dust[num604].position.X -= num601;
					Main.dust[num604].position.Y -= num602;
					dust94 = Main.dust[num604];
					dust2 = dust94;
					dust2.velocity *= 0.2f;
				}
				if (Main.rand.Next(4) == 0)
				{
					int num605 = 6;
					int num606 = Dust.NewDust(new Vector2(Projectile.position.X + num605, Projectile.position.Y + num605), Projectile.width - num605 * 2, Projectile.height - num605 * 2, DustID.AmberBolt, 0f, 0f, 75, default(Color), 0.65f);
					Dust dust95 = Main.dust[num606];
					Dust dust2 = dust95;
					dust2.velocity *= 0.5f;
					dust95 = Main.dust[num606];
					dust2 = dust95;
					dust2.velocity += Projectile.velocity * 0.5f;
				}
			}
		}
		public override void Kill(int timeLeft)
		{
			Projectile.velocity = Projectile.oldVelocity * 0.2f;
			for (int num361 = 0; num361 < 100; num361++)
			{
				int num362 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.AmberBolt, 0f, 0f, 75, default(Color), 1.2f);
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
				dust2.velocity += Projectile.velocity;
				dust120 = Main.dust[num362];
				dust2 = dust120;
				dust2.velocity *= 1f + Main.rand.Next(-100, 101) * 0.01f;
				Main.dust[num362].velocity.X += Main.rand.Next(-50, 51) * 0.015f;
				Main.dust[num362].velocity.Y += Main.rand.Next(-50, 51) * 0.015f;
				Main.dust[num362].position = Projectile.Center;
			}
		}
	}
}