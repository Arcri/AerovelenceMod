#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

#endregion

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
	public sealed class ThornsOfAgony : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Thorns of Agony");
			Tooltip.SetDefault("Casts an agonizing thorny flower that can pierce");
			Item.staff[item.type] = true;
		}
		public override void SetDefaults()
		{
			item.width = item.height = 16;
			item.rare = ItemRarityID.Blue;
			item.value = Item.sellPrice(0, 0, 78, 80);

			item.crit = 4;
			item.mana = 20;
			item.damage = 18;
			item.knockBack = 5f;

			item.useTime = item.useAnimation = 28;
			item.useStyle = ItemUseStyleID.HoldingOut;
			
			item.magic = true;
			item.noMelee = true;

			item.shootSpeed = 12f;
			item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Magic.ThornsOfAgony>();

			item.UseSound = SoundID.Item69;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 velocity = new Vector2(speedX, speedY);
			for (int i = 0; i < 3; ++i)
			{
				Projectile.NewProjectile(position + velocity * 2, velocity.RotatedByRandom(MathHelper.PiOver4), type, damage, knockBack, player.whoAmI, 10);
			}

			return (false);
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Vine, 10);
			recipe.AddRecipeGroup("Wood", 15);
			recipe.AddIngredient(ItemID.JungleSpores, 7);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
