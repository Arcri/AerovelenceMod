using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Placeables.CrystalCaverns
{
	public class CitadelLargeBannerItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Citadel Large Banner");
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 22;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.value = 500;
			Item.createTile = TileType<CitadelLargeBanner>();
		}
	}
} 