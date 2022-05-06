using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
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
			Item.damage = 48;
			Item.rare = ItemRarityID.Yellow;
			Item.width = 78;
			Item.height = 32;
			Item.useAnimation = 35;
			Item.useTime = 35;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shootSpeed = 7f;
			Item.knockBack = 6f;
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;
			Item.UseSound = SoundID.Item36;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 1, 0, 0);
			Item.shoot = ProjectileID.Bullet;
			Item.useAmmo = AmmoID.Bullet;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}
		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				Item.crit = 4;
				Item.damage = 73;
				Item.width = 78;
				Item.height = 32;
				Item.noMelee = false;
				Item.useTime = 60;
				Item.useAnimation = 60;
				Item.shoot = ProjectileID.None;
				Item.UseSound = SoundID.Item1;
				Item.useStyle = ItemUseStyleID.Swing;
				Item.knockBack = 4;
				Item.autoReuse = false;
			}
			else
			{
				Item.damage = 48;
				Item.rare = ItemRarityID.Yellow;
				Item.width = 78;
				Item.height = 32;
				Item.useAnimation = 35;
				Item.useTime = 35;
				Item.useStyle = ItemUseStyleID.Shoot;
				Item.shootSpeed = 7f;
				Item.knockBack = 6f;
				Item.DamageType = DamageClass.Ranged;
				Item.autoReuse = true;
				Item.UseSound = SoundID.Item36;
				Item.noMelee = true;
				Item.shoot = ProjectileID.Bullet;
				Item.useAmmo = AmmoID.Bullet;
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