using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;


namespace AerovelenceMod.Content.Items.Weapons.Ranged.Sandstorm
{
	public class Sandstorm : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Converts wooden arrows into Sandstorm Arrows" +
				"\nRight click to create a sand aura" +
                "\nSandstorm Arrows that pass through the aura explode on contact");
			DisplayName.SetDefault("Sandstorm");
		}

		public override void SetDefaults() {
			item.damage = 15;
			item.ranged = true;
			item.width = 30;
			item.height = 30;
			item.useTime = 36;
			item.useAnimation = 36;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true; 
			item.knockBack = 2;
			item.rare = ItemRarityID.Blue;
			item.UseSound = SoundID.Item5;

			item.value = Item.sellPrice(0, 1, 35, 0);

			item.autoReuse = true;
			item.shoot = AmmoID.Arrow;
			item.useAmmo = AmmoID.Arrow;
			item.shootSpeed = 13f;
			item.noMelee = true;

		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-3, 0);
		}
		public override bool CanUseItem(Player player)
        {
			if (player.altFunctionUse == 2)
            {
				item.noUseGraphic = true;

				item.useStyle = ItemUseStyleID.SwingThrow;
				item.useTime = 60;
				item.useAnimation = 60;
				item.noUseGraphic = true;
				//item.damage = 6;
				item.shootSpeed = 3f;
				item.shoot = ModContent.ProjectileType<SandBall1>();
				item.UseSound = SoundID.DD2_JavelinThrowersAttack.WithVolume(0.4f);
			}
            else
            {
				//item.damage = 12;
				item.ranged = true;
				item.width = 30;
				item.height = 30;
				item.useTime = 26;
				item.useAnimation = 26;
				item.useStyle = ItemUseStyleID.HoldingOut;
				item.noMelee = true;
				item.knockBack = 2;
				item.UseSound = SoundID.Item5;
				item.noUseGraphic = false;
				item.autoReuse = true;
				item.shoot = AmmoID.Arrow;
				item.useAmmo = AmmoID.Arrow;
				item.shootSpeed = 8f;
				item.noMelee = true;
			}
			return base.CanUseItem(player);
        }

        public override bool ConsumeAmmo(Player player)
        {
			if (player.altFunctionUse == 2)
            {
				return false;
            }
			return base.ConsumeAmmo(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			int typeToUse = 0;
			int damageToUse = 0;

			if (player.altFunctionUse == 2)
			{
				typeToUse = ModContent.ProjectileType<SandBall1>();
				damageToUse = damage / 2; //player.inventory[player.selectedItem].damage / 2;
			}
            else
            {
				if (type == ProjectileID.WoodenArrowFriendly)
				{
					typeToUse = ModContent.ProjectileType<SandstormArrow>();
				}
                else
                {
					typeToUse = type;
                }
				damageToUse = damage;//player.inventory[player.selectedItem].damage;
            }
			Projectile.NewProjectile(position, new Vector2(speedX, speedY), typeToUse, damageToUse, knockBack, item.owner);
			return false;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

        public override void AddRecipes()
        {
			ModRecipe recipe = new ModRecipe(mod);

			recipe.AddIngredient(ItemID.GoldBow, 1);
			recipe.AddIngredient(ItemID.Amber, 3);
			recipe.AddIngredient(ItemID.SandstoneBrick, 15);


			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();

			ModRecipe recipe1 = new ModRecipe(mod);

			recipe1.AddIngredient(ItemID.PlatinumBow, 1);
			recipe1.AddIngredient(ItemID.Amber, 2);
			recipe1.AddIngredient(ItemID.SandstoneBrick, 15);

			recipe1.AddTile(TileID.Anvils);
			recipe1.SetResult(this);
			recipe1.AddRecipe();
		}

	}
}