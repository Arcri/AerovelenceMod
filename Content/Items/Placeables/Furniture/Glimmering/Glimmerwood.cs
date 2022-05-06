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
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;

            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;

            Item.createTile = Mod.Find<ModTile>("GlimmerwoodTile").Type;

            Item.useStyle = ItemUseStyleID.Swing;
        }
    }
}
