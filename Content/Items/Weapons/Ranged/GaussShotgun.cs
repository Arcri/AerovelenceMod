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
		private int shotCounter = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gauss Shotgun");
			Tooltip.SetDefault("Fires three bullets, then two, then a homing Gaussian Star");
		}

		public override void SetDefaults()
		{
			item.damage = 26;
			item.rare = ItemRarityID.Yellow;
			item.width = 58;
			item.height = 20;
			item.useAnimation = 60;
			item.useTime = 20;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.shootSpeed = 7f;
			item.knockBack = 6f;
			item.ranged = true;
			item.autoReuse = true;
			item.noMelee = true;
			item.value = Item.buyPrice(0, 5, 0, 0);
			item.shoot = ProjectileID.Bullet;
			//item.UseSound = SoundID.Item92;
			item.useAmmo = AmmoID.Bullet;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-8, 0);
		}

		//public static Vector2[] randomSpread(float speedX, float speedY, int angle, int num)
		//{
			/*
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
			*/
		//}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 50f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}

			switch (shotCounter)
            {
				case 0:
					Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(6)), type, damage, knockBack, player.whoAmI);
					Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(-6)), type, damage, knockBack, player.whoAmI);
					Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
					Main.PlaySound(SoundID.Item38, player.Center);


					//Not using a loop because I don't want to, this isn't that bad, and it is easier to control for tweaking
					this.shotCounter++;
					break;

				case 1:

					Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(3)), type, damage, knockBack, player.whoAmI);
					Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(-3)), type, damage, knockBack, player.whoAmI);
					Main.PlaySound(SoundID.Item38, player.Center);


					this.shotCounter++;
					break;

				case 2:
					Projectile.NewProjectile(position, new Vector2(speedX, speedY), ProjectileType<GaussianStar>(), damage * 2, knockBack, player.whoAmI);
					Main.PlaySound(SoundID.Item92, player.Center);

					for (int i = 0; i < 5; i++)
                    {
						Dust dust = Dust.NewDustDirect(position, 0, 0, DustID.Electric, 0, 0, 0, Color.Red);
						dust.noGravity = true;
						dust.scale *= 1.3f;
						dust.velocity *= 0.5f;
						dust.velocity += (new Vector2(speedX,speedY) * 7) * 0.1f;
					}

					this.shotCounter = 0;
					break;

            }

			if (player.itemAnimation == (item.useAnimation / 3) - 1)
            {
				this.shotCounter = 0; //Makes it so that in rare scenarious where the shot counter is misaligned, it will fix itself on the next shot 
            }

			return false;
		}

        public override bool OnPickup(Player player) //Another check for the problem mentioned above
        {
			this.shotCounter = 0;
			return base.OnPickup(player);
        }
    }
}