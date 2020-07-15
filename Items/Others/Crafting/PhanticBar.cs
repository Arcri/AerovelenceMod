using AerovelenceMod.Blocks.Ores;
using AerovelenceMod.Items.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Others.Crafting
{
	public class PhanticBar : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantic Bar");
			Tooltip.SetDefault("You hear faint echoes from the bar");
		}
		public override void SetDefaults()
		{
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
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
			recipe.AddIngredient(ModContent.ItemType<PhanticOreItem>(), 3);
			recipe.AddTile(TileID.Furnaces);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}