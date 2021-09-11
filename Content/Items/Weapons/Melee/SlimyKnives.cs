using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
	public class SlimyKnives : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slimy Knives");
			Tooltip.SetDefault("Fires slimy knives");
		}
		public override void SetDefaults()
		{
			item.UseSound = SoundID.Item45;
			item.crit = 4;
			item.damage = 25;
			item.melee = true;
			item.width = 20;
			item.height = 40;
			item.useTime = 26;
			item.useAnimation = 26;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.noMelee = true;
			item.knockBack = 2;
			item.value = Item.sellPrice(0, 0, 50, 0);
			item.rare = ItemRarityID.Orange;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<SlimyKnivesProj>();
			item.shootSpeed = 17f;
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			float numberProjectiles = 2 + Main.rand.Next(2);
			float rotation = MathHelper.ToRadians(5);
			position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
				Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
			}
			return false;
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.SlimeBlock, 30);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
	public class SlimyKnivesProj : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 8;
			projectile.height = 8;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.ranged = true;
			projectile.penetrate = 1;
			projectile.timeLeft = 600;
			projectile.ignoreWater = true;
			projectile.tileCollide = true;
			projectile.extraUpdates = 1;
		}
        public override void AI()
        {
			projectile.alpha += 2;
			if (projectile.alpha >= 220)
				projectile.Kill();
			projectile.rotation = projectile.velocity.ToRotation();
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(137, 200);
		}
        public override void Kill(int timeLeft)
		{
			projectile.velocity = projectile.oldVelocity * 0.2f;
			for (int num361 = 0; num361 < 30; num361++)
			{
				int num362 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 75, default(Color), 1.2f);
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