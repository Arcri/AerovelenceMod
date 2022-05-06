using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class VoidArena : AerovelenceItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Void Arena Block");

        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.consumable = true;
            Item.autoReuse = true;

            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;

            Item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.VoidArena>();

            Item.useStyle = ItemUseStyleID.Swing;
        }
    }
}
