using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Items.Weapons.Ranged
{
	public class ShotgunAxe : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shotgun-Axe");
			Tooltip.SetDefault("'First we sharpen the axe, then we chop the tree.'\n" +
							   "'My axe is plenty sharp, and a shotgun.'");
		}

		public override void SetDefaults()
		{
			item.damage = 48;
			item.rare = ItemRarityID.Yellow;
			item.width = 78;
			item.height = 32;
			item.useAnimation = 35;
			item.useTime = 35;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.shootSpeed = 7f;
			item.knockBack = 6f;
			item.ranged = true;
			item.autoReuse = true;
			item.UseSound = SoundID.Item36;
			item.noMelee = true;
			item.value = Item.buyPrice(0, 1, 0, 0);
			item.shoot = ProjectileID.Bullet;
			item.useAmmo = AmmoID.Bullet;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}
		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.crit = 4;
				item.damage = 73;
				item.width = 78;
				item.height = 32;
				item.noMelee = false;
				item.useTime = 60;
				item.useAnimation = 60;
				item.shoot = ProjectileID.None;
				item.UseSound = SoundID.Item1;
				item.useStyle = ItemUseStyleID.SwingThrow;
				item.knockBack = 4;
				item.autoReuse = false;
			}
			else
			{
				item.damage = 48;
				item.rare = ItemRarityID.Yellow;
				item.width = 78;
				item.height = 32;
				item.useAnimation = 35;
				item.useTime = 35;
				item.useStyle = ItemUseStyleID.HoldingOut;
				item.shootSpeed = 7f;
				item.knockBack = 6f;
				item.ranged = true;
				item.autoReuse = true;
				item.UseSound = SoundID.Item36;
				item.noMelee = true;
				item.shoot = ProjectileID.Bullet;
				item.useAmmo = AmmoID.Bullet;
			}
			return base.CanUseItem(player);
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-4, 0);
		}

		public static Vector2[] RandomSpread(float speedX, float speedY, int angle, int num)
		{
			var posArray = new Vector2[num];
			float spread = (float)(angle * 0.0555);
			float baseSpeed = (float)System.Math.Sqrt(speedX * speedX + speedY * speedY);
			double baseAngle = System.Math.Atan2(speedX, speedY);
			double randomAngle;
			for (int i = 0; i < num; ++i)
			{
				randomAngle = baseAngle + (Main.rand.NextFloat() - 0.2f) * spread;
				posArray[i] = new Vector2(baseSpeed * (float)System.Math.Sin(randomAngle), baseSpeed * (float)System.Math.Cos(randomAngle));
			}
			return posArray;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			float numberProjectiles = 2 + Main.rand.Next(1);
			float rotation = MathHelper.ToRadians(20);
			position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(15));
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;


			Vector2[] speeds = RandomSpread(speedX, speedY, 10, 10);
			for (int i = 0; i < 5; ++i)
			{
				Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, type, damage, knockBack, player.whoAmI);
			}
			return true;
		}
	}
}