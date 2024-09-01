using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items
{
    public class GlimmerwoodWorkbenchItem : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            CommonItemHelper.SetupPlaceableItem(this, 32, 16, 150, ModContent.TileType<GlimmerwoodWorkbench>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 10)
                .Register();
        }
    }
}