using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
	public class DuneTessen : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dune Tessen");
			Tooltip.SetDefault("Fires sand knives");
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
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<DuneSpike>();
			Item.shootSpeed = 25f;
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
        float numberProjectiles = 3 + Main.rand.Next(3);
			float rotation = MathHelper.ToRadians(20);
			position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f, player.whoAmI);
			}
			return false;
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ModContent.ItemType<HugeAntlionMandible>(), 2)
				.AddIngredient(ItemID.Sandstone, 30)
				.AddIngredient(ItemID.Cactus, 5)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
	public class DuneSpike : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 3;
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
        public override void Kill(int timeLeft)
		{
			Projectile.velocity = Projectile.oldVelocity * 0.2f;
			for (int num361 = 0; num361 < 30; num361++)
			{
				int num362 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.AmberBolt, 0f, 0f, 75, default(Color), 1.2f);
				Dust dust120;
				Dust dust2;
				if (Main.rand.NextBool(2))
				{
					dust120 = Main.dust[num362];
					dust2 = dust120;
					dust2.alpha += 25;
				}
				if (Main.rand.NextBool(2))
				{
					dust120 = Main.dust[num362];
					dust2 = dust120;
					dust2.alpha += 25;
				}
				if (Main.rand.NextBool(2))
				{
					dust120 = Main.dust[num362];
					dust2 = dust120;
					dust2.alpha += 25;
				}
				if (Main.rand.NextBool(2))
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