using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod;

namespace AerovelenceMod.Items.Others
{
	public class SoulFig : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul Fig");
			Tooltip.SetDefault("Permanently increases maximum life by 50\n" +
                "You must have 500 max life or more to eat this item.");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.useAnimation = 30;
			item.useTime = 30;
			item.useStyle = 4;
			item.UseSound = SoundID.Item4;
			item.consumable = true;
			item.rare = 7;
		}

		public override bool CanUseItem(Player player)
		{
			AeroPlayer modPlayer = GetModPlayer<AeroPlayer>();
			return !modPlayer.fruit1 && player.statLifeMax >= 500;
		}

		public override bool UseItem(Player player)
		{
			if (player.itemAnimation > 0 && player.itemTime == 0)
			{
				player.itemTime = item.useTime;
				if (Main.myPlayer == player.whoAmI)
				{
					player.HealEffect(50, true);
				}
                AeroPlayer modPlayer = GetModPlayer<AeroPlayer>();
				modPlayer.fruit1 = true;
			}
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddIngredient(1291, 1);
            modRecipe.AddIngredient(1508, 35);
            modRecipe.AddIngredient(12, 100);
            modRecipe.AddTile(134);
			modRecipe.SetResult(this, 1);
			modRecipe.AddRecipe();
		}
	}
}
