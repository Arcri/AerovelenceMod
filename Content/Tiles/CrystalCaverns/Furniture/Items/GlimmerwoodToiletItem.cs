using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items
{
    public class GlimmerwoodToiletItem : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            CommonItemHelper.SetupPlaceableItem(this, 16, 32, 150, ModContent.TileType<GlimmerwoodToilet>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 6)
                .AddTile(ModContent.TileType<CrystallineFabricator>())
                .Register();
        }
    }
}