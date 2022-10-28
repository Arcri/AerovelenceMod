using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged
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
			Item.damage = 26;
			Item.rare = ItemRarityID.Yellow;
			Item.width = 58;
			Item.height = 20;
			Item.useAnimation = 60;
			Item.useTime = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shootSpeed = 7f;
			Item.knockBack = 6f;
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 5, 0, 0);
			Item.shoot = ProjectileID.Bullet;
			//item.UseSound = SoundID.Item92;
			Item.useAmmo = AmmoID.Bullet;
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

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 50f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}

			switch (shotCounter)
			{
				case 0:
					Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(6)), type, damage, knockback, player.whoAmI);
					Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-6)), type, damage, knockback, player.whoAmI);
					Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
					SoundEngine.PlaySound(SoundID.Item38 with { Volume = 0.85f }, player.Center);


					//Not using a loop because I don't want to, this isn't that bad, and it is easier to control for tweaking
					this.shotCounter++;
					break;

				case 1:

					Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(3)), type, damage, knockback, player.whoAmI);
					Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-3)), type, damage, knockback, player.whoAmI);
					SoundEngine.PlaySound(SoundID.Item38 with { Volume = 0.85f }, player.Center);


					this.shotCounter++;
					break;

				case 2:
					Projectile.NewProjectile(source, position, velocity, ProjectileType<GaussianStar>(), damage * 2, knockback, player.whoAmI);
					SoundEngine.PlaySound(SoundID.Item92 with { Volume = 0.85f }, player.Center);

					for (int i = 0; i < 5; i++)
					{
						Dust dust = Dust.NewDustDirect(position, 0, 0, DustID.Electric, 0, 0, 0, Color.Red);
						dust.noGravity = true;
						dust.scale *= 1.3f;
						dust.velocity *= 0.5f;
						dust.velocity += (velocity * 7) * 0.1f;
					}

					this.shotCounter = 0;
					break;

			}

			if (player.itemAnimation == (Item.useAnimation / 3) - 1 || player.frozen || player.stoned)
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