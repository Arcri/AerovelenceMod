using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;

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
			Item.damage = 120;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 62;
			Item.height = 32;
			Item.useTime = 50;
			Item.useAnimation = 50;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 0.2f;
			Item.value = Item.sellPrice(0, 1, 50, 0);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item11;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<TitaniumRocket>();
			Item.shootSpeed = 15f;
			Item.useAmmo = AmmoID.Rocket;
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ItemID.TitaniumBar, 15)
				.AddIngredient(ItemID.SoulofMight, 10)
				.AddIngredient(ItemID.HallowedBar, 5)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-6, -2);
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(3));
		}

		public class TitaniumRocket : ModProjectile
		{
			public override void SetDefaults()
			{
				Projectile.width = 58;
				Projectile.height = 22;
				Projectile.maxPenetrate = 1;
				Projectile.damage = 11;
				Projectile.hostile = false;
				Projectile.tileCollide = true;
				Projectile.ignoreWater = true;
				Projectile.friendly = true;
				Projectile.DamageType = DamageClass.Ranged;
				Projectile.timeLeft = 200;
			}
			int i;
			private readonly int oneHelixRevolutionInUpdateTicks = 30;

			public override void AI()
			{
				i++;
				++Projectile.localAI[0];
				float piFraction = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;
				Vector2 newDustPosition = new Vector2(0, (float)Math.Sin((Projectile.localAI[0] % oneHelixRevolutionInUpdateTicks) * piFraction)) * Projectile.height;
				Dust newDust = Dust.NewDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), 91);
				newDust.noGravity = true;
				newDustPosition.Y *= -1;
				newDust = Dust.NewDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), 91);
				newDust.noGravity = true;
				newDust.velocity *= 0f;
				Projectile.rotation = Projectile.velocity.ToRotation();
				if (Math.Abs(Projectile.velocity.X) >= 8f || Math.Abs(Projectile.velocity.Y) >= 8f)
				{
					for (int i = 0; i < 2; i++)
					{
						int num255 = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 91, 0f, 0f, 100);
						Dust dust = Main.dust[num255];

						Main.dust[num255].noGravity = true;
						num255 = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y), Projectile.width, Projectile.height, 91, 0f, 0f, 100, default(Color), 0.5f);
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
				SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.position);

				Projectile.position.X = Projectile.position.X + (Projectile.width / 2);
				Projectile.position.Y = Projectile.position.Y + (Projectile.width / 2);
				Projectile.width = 30;
				Projectile.height = 30;
				Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
				Projectile.position.Y = Projectile.position.Y - (Projectile.width / 2);

				for (int i = 0; i < 35; i++)
				{
					int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 91, 0f, 0f, 100, default, 1f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 5f;
					dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 91, 0f, 0f, 100, default, 1f);
					Main.dust[dust].velocity *= 2f;
				}
			}
		}
	}
}
