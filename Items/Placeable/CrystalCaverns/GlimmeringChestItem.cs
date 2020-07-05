using AerovelenceMod.Blocks.CrystalCaverns.Tiles.Furniture;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Items.Placeable.CrystalCaverns
{
	public class GlimmeringChestItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glimmering Chest");
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
			item.createTile = TileType<GlimmeringChest>();
		}
	}
}