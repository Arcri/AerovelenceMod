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

            Item.useTurn = true;
            Item.consumable = true;
            Item.autoReuse = true;

            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;

            Item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.CavernStone>();

            Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(0, 0, 0, 0);
        }
    }
}
