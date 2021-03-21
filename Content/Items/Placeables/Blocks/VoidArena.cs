using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class VoidArena : AerovelenceItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Void Arena Block");

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.consumable = true;
            item.autoReuse = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.VoidArena>();

            item.useStyle = ItemUseStyleID.SwingThrow;
        }
    }
}
