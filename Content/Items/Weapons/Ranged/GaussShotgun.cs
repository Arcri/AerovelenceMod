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
		}

		public override void SetDefaults()
		{
			item.damage = 31;
			item.rare = ItemRarityID.Yellow;
			item.width = 58;
			item.height = 20;
			item.useAnimation = 35;
			item.useTime = 35;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.shootSpeed = 7f;
			item.knockBack = 6f;
			item.ranged = true;
			item.autoReuse = true;
			item.noMelee = true;
			item.value = Item.buyPrice(0, 1, 0, 0);
			item.shoot = ProjectileID.Bullet;
			item.UseSound = SoundID.Item11;
			item.useAmmo = AmmoID.Bullet;
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
			float numberProjectiles = 2 + Main.rand.Next(1);
			float rotation = MathHelper.ToRadians(20);
			position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(15));
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;

			if (Main.rand.NextBool(5))
			{
				{
					Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileType<SkylightProjectile>(), damage * 2, knockBack, player.whoAmI);
				}
			}

			Vector2[] speeds = randomSpread(speedX, speedY, 10, 10);
			for (int i = 0; i < 5; ++i)
			{
				Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, type, damage, knockBack, player.whoAmI);
			}
			return true;
		}
	}
}