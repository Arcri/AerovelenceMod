using AerovelenceMod.Content.Items.Placeables.Blocks;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering
{
    public class Glimmerwood : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Glimmerwood");

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.autoReuse = true;
            item.consumable = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = mod.TileType("GlimmerwoodTile");

            item.useStyle = ItemUseStyleID.SwingThrow;
        }
    }
}
