using AerovelenceMod.Content.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Placeables.MusicBoxes
{
    public class LightningMothBoxItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Music Box (Lightning Moth)");
            Tooltip.SetDefault("Composed by A44");
		}
		public override void SetDefaults()
		{
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.consumable = true;
			item.createTile = TileType<LightningMothBox>();
			item.width = 24;
			item.height = 24;
			item.rare = ItemRarityID.LightRed;
			item.value = 100000;
			item.accessory = true;
		}
	}
}
