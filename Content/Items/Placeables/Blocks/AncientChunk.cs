using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class AncientChunk : ModItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Ancient Chunk");

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.autoReuse = true;
            item.consumable = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.AncientChunk>();

            item.useStyle = ItemUseStyleID.SwingThrow;
        }
    }
}
