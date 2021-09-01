using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class CavernStone : ModItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Cavern Stone");

        public override void SetDefaults()
        {

            item.useTurn = true;
            item.consumable = true;
            item.autoReuse = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.CavernStone>();

            item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = Item.sellPrice(0, 0, 0, 0);
        }
    }
}
