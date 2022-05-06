using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Tools
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
			Item.width = 28;
			Item.height = 50;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.mana = 100;
			Item.UseSound = SoundID.Item4;
			Item.consumable = false;
			Item.rare = ItemRarityID.Cyan;
		}

		public const int DayLength = 54000;
		public const int NightLength = 32400;




		public override bool? UseItem(Player player)
		{


			if (Main.dayTime != true)
            {
				Main.dayTime = true;
				Main.time = 0;
            }
			else
            {
				Main.dayTime = false;
				Main.time = 0;
			}
			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ItemID.LifeFruit, 1)
				.AddIngredient(ItemID.LunarBar, 25)
				.AddTile(TileID.MythrilAnvil)
				.Register();

		}
	}
}
