using AerovelenceMod.Content.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Placeables.MusicBoxes
{
    public class CrystalTumblerBoxItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Music Box (Crystal Tumbler)");
            Tooltip.SetDefault("Composed by A44");

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CrystalTumbler"), ModContent.ItemType<CrystalTumblerBoxItem>(), ModContent.TileType<CrystalTumblerBox>());
		}
		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = TileType<CrystalTumblerBox>();
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.LightRed;
			Item.value = 100000;
			Item.accessory = true;
		}
	}
}
