using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod;

namespace AerovelenceMod.Items.Others
{
	public class KnowledgeFruit : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fruit of Knowledge");
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
			item.rare = 9;
		}

		public override bool CanUseItem(Player player)
		{
			AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
			return !modPlayer.KnowledgeFruit && player.statLifeMax >= 500;
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
                AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
				modPlayer.KnowledgeFruit = true;
			}
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddIngredient(1291, 1);
            modRecipe.AddIngredient(3456, 3);
            modRecipe.AddIngredient(3457, 3);
            modRecipe.AddIngredient(3458, 3);
            modRecipe.AddIngredient(3459, 3);
            modRecipe.AddIngredient(549, 45);
            modRecipe.AddTile(134);
            modRecipe.SetResult(this, 1);
			modRecipe.AddRecipe();
		}
	}
}
