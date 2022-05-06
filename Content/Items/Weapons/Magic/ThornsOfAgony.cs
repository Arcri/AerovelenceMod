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
			Item.staff[Item.type] = true;
		}
		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 0, 78, 80);

			Item.crit = 4;
			Item.mana = 20;
			Item.damage = 18;
			Item.knockBack = 5f;

			Item.useTime = Item.useAnimation = 28;
			Item.useStyle = ItemUseStyleID.Shoot;
			
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;

			Item.shootSpeed = 12f;
			Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Magic.ThornsOfAgony>();

			Item.UseSound = SoundID.Item69;
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
			CreateRecipe(1)
				.AddIngredient(ItemID.Vine, 10)
				.AddRecipeGroup("Wood", 15)
				.AddIngredient(ItemID.JungleSpores, 7)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
