#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;

#endregion

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
	public sealed class ResplendentBloom : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.staff[item.type] = true;
		}
		public override void SetDefaults()
		{
			item.width = item.height = 16;
			item.rare = ItemRarityID.Green;
			item.value = Item.sellPrice(0, 0, 60, 0);
			
			item.mana = 3;
			item.crit = 5;
			item.damage = 25;
			item.knockBack = 3f;

			item.useTime = 6;
			item.useAnimation = 18;
			item.reuseDelay = item.useAnimation + 10;
			item.useStyle = ItemUseStyleID.HoldingOut;
			
			item.magic = true;
			item.noMelee = true;
			item.autoReuse = true;
			
			item.shootSpeed = 8f;
			item.shoot = ModContent.ProjectileType<ResplendentPollen>();
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			float	minDistance = 20f,
					maxDistance = 60f;

			int[] projectileTypes = {
				ModContent.ProjectileType<ResplendentPetal>(),
				ModContent.ProjectileType<ResplendentPollen>()
			};

			Vector2 spawnPos = Vector2.UnitX;

			for (int i = 0; i < 50; ++i)
			{
				float f = Main.rand.NextFloat() * MathHelper.TwoPi;
				spawnPos = position + f.ToRotationVector2() * MathHelper.Lerp(minDistance, maxDistance, Main.rand.NextFloat());

				if (Collision.CanHit(position, 0, 0, spawnPos + (spawnPos - position).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
				{
					break;
				}
			}

			Vector2 velocity = Main.MouseWorld - spawnPos;
			Vector2 safeVelocity = new Vector2(speedX, speedY).SafeNormalize(Vector2.UnitY) * item.shootSpeed;

			velocity = velocity.SafeNormalize(safeVelocity) * item.shootSpeed;
			velocity = Vector2.Lerp(velocity, safeVelocity, 0.25f);

			Projectile.NewProjectile(spawnPos, velocity, projectileTypes[Main.rand.Next(projectileTypes.Length)], damage, knockBack, player.whoAmI);

			return (false);
		}

		/*public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Materials.Leaf>(), 5);
			recipe.AddIngredient(ModContent.ItemType<Materials.ScatteredFlower>(), 15);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}*/
	}
}
