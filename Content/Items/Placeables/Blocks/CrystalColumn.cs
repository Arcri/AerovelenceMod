using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class CrystalColumn : AerovelenceItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Crystal Column");

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.consumable = true;
            item.autoReuse = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.CrystalColumn>();

            item.useStyle = ItemUseStyleID.SwingThrow;
        }
    }
}
