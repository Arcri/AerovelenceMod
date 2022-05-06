using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class CitadelBrickItem : ModItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Citadel Brick");

        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;

            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;


            Item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.CitadelBrick>();

            Item.useStyle = ItemUseStyleID.Swing;
        }
    }
}
