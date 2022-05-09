using AerovelenceMod.Content.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Placeables.MusicBoxes
{
    public class CrystalCavernsBoxItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Music Box (Crystal Caverns)");
            Tooltip.SetDefault("Composed by A44");

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CrystalCaverns"), ModContent.ItemType<CrystalCavernsBoxItem>(), ModContent.TileType<CrystalCavernsBox>());
		}
		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = TileType<CrystalCavernsBox>();
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.LightRed;
			Item.value = 100000;
			Item.accessory = true;
		}
	}
}
