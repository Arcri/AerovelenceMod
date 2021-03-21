using AerovelenceMod.Content.Tiles.Trophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Trophies
{
	public class SnowriumTrophy : ModItem
	{
		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 30;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.value = 50000;
			item.rare = ItemRarityID.Blue;
			item.createTile = ModContent.TileType<SnowriumTrophyPlaced>();
			item.placeStyle = 0;
		}
	}
}