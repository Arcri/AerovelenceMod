using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Placeables.CrystalCaverns
{
	public class AzurianShieldItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Azurian Shield Item Placed Delete Soon");
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
			item.createTile = TileType<CitadelShield>();
		}
	}
}