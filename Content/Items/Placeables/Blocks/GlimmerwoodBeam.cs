using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class GlimmerwoodBeam : AerovelenceItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Glimmerwood Beam");

        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.consumable = true;
            Item.autoReuse = true;

            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;

            Item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.GlimmerwoodBeam>();

            Item.useStyle = ItemUseStyleID.Swing;
        }
    }
}
