using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class CrystalSand : ModItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Crystal Sand");

        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;

            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;

            Item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.CrystalSand>();

            Item.useStyle = ItemUseStyleID.Swing;
        }
    }
}
