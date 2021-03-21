using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Tiles.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Crafting
{
    public class StarglassBar : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Starglass Bar");
			Tooltip.SetDefault("Gleams with mysterious energy");
		}
		public override void SetDefaults()
		{
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTurn = true;
			item.width = 32;
			item.height = 24;
			item.useAnimation = 15;
			item.useTime = 15;
			item.autoReuse = true;
			item.consumable = true;
			item.placeStyle = 0;
			item.consumable = true;
			item.createTile = ModContent.TileType<PhanticBarPlaced>();
			item.maxStack = 999;
			item.value = Item.sellPrice(0, 0, 20, 0);
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<PhanticOre>(), 3);
			recipe.AddTile(TileID.Furnaces);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}