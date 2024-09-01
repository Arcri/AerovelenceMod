using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items
{
    public class GlimmerwoodClockItem : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodClock>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 8)
                .AddRecipeGroup("AerovelenceMod:IronBars", 6)
                .AddIngredient(ItemID.Glass, 6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}