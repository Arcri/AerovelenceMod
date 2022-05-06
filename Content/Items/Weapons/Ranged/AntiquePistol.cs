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
			Item.value = Item.sellPrice(1);
			Item.damage = 25;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 1;
			Item.height = 1;
			Item.useTime = 32;
			Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 5f;
			Item.rare = ItemRarityID.Orange;
			Item.noMelee = true;
			Item.UseSound = SoundID.Item11;
			Item.autoReuse = true;
            Item.shootSpeed = 14f;
            Item.shoot = AmmoID.Bullet;
            Item.useAmmo = AmmoID.Bullet;
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
			CreateRecipe(1)
				.AddIngredient(ItemID.FlintlockPistol, 1)
				.AddRecipeGroup("IronBar", 15)
				.Register();
		}
	}
}