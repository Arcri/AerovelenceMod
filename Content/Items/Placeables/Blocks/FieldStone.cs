using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class FieldStone : ModItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Crystal Stone");

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.useTurn = true;
            Item.autoReuse = true;

            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;

            Item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.FieldStone>();

            Item.useStyle = ItemUseStyleID.Swing;
        }
    }
}
