using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class ValleyGrass : AerovelenceItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Valley Grass");

        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;

            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;

            Item.createTile = ModContent.TileType<Tiles.IceExpansion.ValleyGrass>();

            Item.useStyle = ItemUseStyleID.Swing;
        }
    }
}
