using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
	public class TitaniumRocketLauncher : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Titanium Rocket Launcher");
			Tooltip.SetDefault("Fires a titanium rocket");
		}
		public override void SetDefaults()
		{
			item.damage = 120;
			item.ranged = true;
			item.width = 62;
			item.height = 32;
			item.useTime = 50;
			item.useAnimation = 50;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.knockBack = 0.2f;
			item.value = Item.sellPrice(0, 1, 50, 0);
			item.rare = ItemRarityID.Orange;
			item.UseSound = SoundID.Item11;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<TitaniumRocket>();
			item.shootSpeed = 15f;
			item.useAmmo = AmmoID.Rocket;
		}
		public override void AddRecipes()
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddIngredient(ItemID.TitaniumBar, 15);
			modRecipe.AddIngredient(ItemID.SoulofMight, 10);
			modRecipe.AddIngredient(ItemID.HallowedBar, 5);
			modRecipe.AddTile(TileID.MythrilAnvil);
			modRecipe.SetResult(this);
			modRecipe.AddRecipe();
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-6, -2);
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(3));
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;
			Vector2 muzzleOffset = new Vector2(-7, -8);
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}
			return true;
		}

		public class TitaniumRocket : ModProjectile
		{
			public override void SetDefaults()
			{
				projectile.width = 58;
				projectile.height = 22;
				projectile.maxPenetrate = 1;
				projectile.damage = 11;
				projectile.hostile = false;
				projectile.tileCollide = true;
				projectile.ignoreWater = true;
				projectile.friendly = true;
				projectile.ranged = true;
				projectile.timeLeft = 200;
			}
			int i;
			private readonly int oneHelixRevolutionInUpdateTicks = 30;

			public override void AI()
			{
				i++;
				++projectile.localAI[0];
				float piFraction = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;
				Vector2 newDustPosition = new Vector2(0, (float)Math.Sin((projectile.localAI[0] % oneHelixRevolutionInUpdateTicks) * piFraction)) * projectile.height;
				Dust newDust = Dust.NewDustPerfect(projectile.Center + newDustPosition.RotatedBy(projectile.velocity.ToRotation()), 91);
				newDust.noGravity = true;
				newDustPosition.Y *= -1;
				newDust = Dust.NewDustPerfect(projectile.Center + newDustPosition.RotatedBy(projectile.velocity.ToRotation()), 91);
				newDust.noGravity = true;
				newDust.velocity *= 0f;
				projectile.rotation = projectile.velocity.ToRotation();
				if (Math.Abs(projectile.velocity.X) >= 8f || Math.Abs(projectile.velocity.Y) >= 8f)
				{
					for (int i = 0; i < 2; i++)
					{
						int num255 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 91, 0f, 0f, 100);
						Dust dust = Main.dust[num255];

						Main.dust[num255].noGravity = true;
						num255 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, 91, 0f, 0f, 100, default(Color), 0.5f);
						Main.dust[num255].fadeIn = 1f + Main.rand.Next(5) * 0.1f;
						dust = Main.dust[num255];
						Main.dust[num255].noGravity = true;
						dust.scale *= 0.99f;
					}
				}
			}
			public override bool? CanHitNPC(NPC target)
			{
				if (target.townNPC)
				{
					return false;
				}
				return base.CanHitNPC(target);
			}

			public override void Kill(int timeLeft)
			{
				Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 14);

				projectile.position.X = projectile.position.X + (projectile.width / 2);
				projectile.position.Y = projectile.position.Y + (projectile.width / 2);
				projectile.width = 30;
				projectile.height = 30;
				projectile.position.X = projectile.position.X - (projectile.width / 2);
				projectile.position.Y = projectile.position.Y - (projectile.width / 2);

				for (int i = 0; i < 35; i++)
				{
					int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 91, 0f, 0f, 100, default, 1f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 5f;
					dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 91, 0f, 0f, 100, default, 1f);
					Main.dust[dust].velocity *= 2f;
				}
			}
		}
	}
}