using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class AntiquePistol : ModItem
	{
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-8f, 0f);
		}

		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Antique Pistol");
            Tooltip.SetDefault("Chance to do 5x normal damage");
		}

		public override void SetDefaults()
		{
			item.value = Item.sellPrice(1);
			item.damage = 25;
			item.ranged = true;
			item.width = 1;
			item.height = 1;
			item.useTime = 32;
			item.useAnimation = 32;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.knockBack = 5f;
			item.rare = ItemRarityID.Orange;
			item.noMelee = true;
			item.UseSound = SoundID.Item11;
			item.autoReuse = true;
            item.shootSpeed = 14f;
            item.shoot = AmmoID.Bullet;
            item.useAmmo = AmmoID.Bullet;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (Main.rand.Next(4) == 0)
			{
				damage *= 5;
			}
			return true;
		}
		public override void AddRecipes()
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddIngredient(ItemID.FlintlockPistol, 1);
			modRecipe.AddRecipeGroup("IronBar", 15);
			modRecipe.SetResult(this, 1);
			modRecipe.AddRecipe();
		}
	}
}