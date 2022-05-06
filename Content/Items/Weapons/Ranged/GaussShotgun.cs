using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
	public class GaussShotgun : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gauss Shotgun");
			Tooltip.SetDefault("Fires a homing Gaussian Star");
		}

		public override void SetDefaults()
		{
			Item.damage = 21;
			Item.rare = ItemRarityID.Yellow;
			Item.width = 58;
			Item.height = 20;
			Item.useAnimation = 40;
			Item.useTime = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shootSpeed = 7f;
			Item.knockBack = 6f;
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 1, 0, 0);
			Item.shoot = ProjectileID.Bullet;
			Item.UseSound = SoundID.Item92;
			Item.useAmmo = AmmoID.Bullet;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-8, 0);
		}

		public static Vector2[] randomSpread(float speedX, float speedY, int angle, int num)
		{
			var posArray = new Vector2[num];
			float spread = (float)(angle * 0.0555);
			float baseSpeed = (float)System.Math.Sqrt(speedX * speedX + speedY * speedY);
			double baseAngle = System.Math.Atan2(speedX, speedY);
			double randomAngle;
			for (int i = 0; i < num; ++i)
			{
				randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
				posArray[i] = new Vector2(baseSpeed * (float)System.Math.Sin(randomAngle), baseSpeed * (float)System.Math.Cos(randomAngle));
			}
			return posArray;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(new Vector2(position.X, position.Y - 8), new Vector2(speedX, speedY), ProjectileType<GaussianStar>(), 32, 5f, player.whoAmI);
			position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(15));
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;
			Vector2[] speeds = randomSpread(speedX, speedY, 10, 10);
			for (int i = 0; i < 5; ++i)
			{
				Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, type, damage, knockBack, player.whoAmI);
			}
			return true;
		}
	}
}