using AerovelenceMod.Content.Tiles.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class StarglassCluster : ModItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Starglass Cluster");

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.autoReuse = true;
            item.consumable = true;

            item.maxStack = 999;
            item.useTime = 10;
            item.useAnimation = 15;

            item.createTile = ModContent.TileType<StarglassOreBlock>();

            item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = Item.sellPrice(silver: 12);
        }
    }
}
