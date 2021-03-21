using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class ChargedStone : AerovelenceItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Charged Stone");

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.autoReuse = true;
            item.consumable = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.ChargedStone>();

            item.useStyle = ItemUseStyleID.SwingThrow;
        }
    }
}
