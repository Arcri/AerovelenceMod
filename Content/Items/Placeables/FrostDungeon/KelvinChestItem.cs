using AerovelenceMod.Content.Tiles.FrostDungeon.Furniture;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Placeables.FrostDungeon
{
	public class KelvinChestItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Kelvin Chest");
		}

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 22;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.value = 500;
			item.createTile = TileType<KelvinChest>();
		}
	}
}