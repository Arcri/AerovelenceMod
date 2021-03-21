using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class ValleyDirt : AerovelenceItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Valley Dirt");

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.autoReuse = true;
            item.consumable = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = ModContent.TileType<Tiles.IceExpansion.ValleyDirt>();

            item.useStyle = ItemUseStyleID.SwingThrow;
        }
    }
}
