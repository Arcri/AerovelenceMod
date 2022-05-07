using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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
			Item.UseSound = SoundID.Item45;
			Item.crit = 4;
			Item.damage = 25;
			Item.DamageType = DamageClass.Melee;
			Item.width = 20;
			Item.height = 40;
			Item.useTime = 26;
			Item.useAnimation = 26;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.knockBack = 2;
			Item.value = Item.sellPrice(0, 0, 50, 0);
			Item.rare = ItemRarityID.Orange;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<SlimyKnivesProj>();
			Item.shootSpeed = 17f;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			float numberProjectiles = 2 + Main.rand.Next(2); // 3, 4, or 5 shots
			float rotation = MathHelper.ToRadians(45);
			position += Vector2.Normalize(velocity) * 45f;
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
				Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
			}
			return false; // return false to stop vanilla from calling Projectile.NewProjectile.
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ItemID.SlimeBlock, 30)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
	public class SlimyKnivesProj : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 600;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
		}
        public override void AI()
        {
			Projectile.alpha += 2;
			if (Projectile.alpha >= 220)
				Projectile.Kill();
			Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(137, 200);
		}
        public override void Kill(int timeLeft)
		{
			Projectile.velocity = Projectile.oldVelocity * 0.2f;
			for (int num361 = 0; num361 < 30; num361++)
			{
				int num362 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 33, 0f, 0f, 75, default(Color), 1.2f);
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