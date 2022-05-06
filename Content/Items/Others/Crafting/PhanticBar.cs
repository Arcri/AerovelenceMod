using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Tiles.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Crafting
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
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.width = 32;
			Item.height = 24;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.placeStyle = 0;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<PhanticBarPlaced>();
			Item.maxStack = 999;
			Item.value = Item.sellPrice(0, 0, 20, 0);
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ModContent.ItemType<PhanticOre>(), 3)
				.AddTile(TileID.Furnaces)
				.Register();
		}
	}
}