using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod;

namespace AerovelenceMod.Items.Others
{
	public class ClockOfLife : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Clock of Life");
			Tooltip.SetDefault("Changes the time to day if the moon is up, and vice versa");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.useAnimation = 30;
			item.useTime = 30;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.mana = 100;
			item.UseSound = SoundID.Item4;
			item.consumable = false;
			item.rare = ItemRarityID.Cyan;
		}

		public const int DayLength = 54000;
		public const int NightLength = 32400;




		public override bool UseItem(Player player)
		{


			if (Main.dayTime != true)
            {
				Main.dayTime = true;
				Main.time = 0;
            }
			else
            {
				Main.dayTime = false;
				Main.time = 0; // 27000.0;
			}
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddIngredient(ItemID.LifeFruit, 1);
			modRecipe.AddIngredient(ItemID.LunarBar, 25);
			modRecipe.AddTile(TileID.MythrilAnvil);
			modRecipe.SetResult(this, 1);
			modRecipe.AddRecipe();

		}
	}
}
