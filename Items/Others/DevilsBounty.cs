using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod;

namespace AerovelenceMod.Items.Others
{
	public class DevilsBounty : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Devil's Bounty");
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
			AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>(mod);
			return !modPlayer.fruit3 && player.statLifeMax >= 500;
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
                AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>(mod);
				modPlayer.fruit3 = true;
			}
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddIngredient(1291, 1);
            modRecipe.AddIngredient(3467, 25);
            modRecipe.AddTile(134);
            modRecipe.SetResult(this, 1);
			modRecipe.AddRecipe();
		}
	}
}
