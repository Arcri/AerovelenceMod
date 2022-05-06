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
	}
}