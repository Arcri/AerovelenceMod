using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items
{
    public class GlimmerwoodStoolItem : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodStool>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}