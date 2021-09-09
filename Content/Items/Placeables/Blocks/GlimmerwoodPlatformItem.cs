using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class GlimmerwoodPlatformItem : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Glimmerwood Platform");

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.autoReuse = true;
            item.consumable = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.Furniture.GlimmerwoodPlatform>();

            item.useStyle = ItemUseStyleID.SwingThrow;
        }
    }
}