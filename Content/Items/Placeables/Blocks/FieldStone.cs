using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class FieldStone : ModItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Crystal Stone");

        public override void SetDefaults()
        {
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.FieldStone>();

            item.useStyle = ItemUseStyleID.SwingThrow;
        }
    }
}
