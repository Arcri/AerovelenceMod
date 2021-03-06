using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.CrystalCaverns
{
	public class CrystallineFabricatorItem: ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Crystalline Fabricator");
		}

		public override void SetDefaults()
		{
			item.width = 12;
			item.height = 12;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 7;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.createTile = mod.TileType("CrystallineFabricator");
		}
	}
}