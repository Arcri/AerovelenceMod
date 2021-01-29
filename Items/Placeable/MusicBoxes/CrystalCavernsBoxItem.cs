using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;
using AerovelenceMod.Blocks.MusicBoxes;

namespace AerovelenceMod.Items.Placeable.MusicBoxes
{
    public class CrystalCavernsBoxItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Music Box (Crystal Caverns)");
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
			item.createTile = TileType<CrystalCavernsBox>();
			item.width = 24;
			item.height = 24;
			item.rare = ItemRarityID.LightRed;
			item.value = 100000;
			item.accessory = true;
		}
	}
}
