using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items
{
    public class GlimmerwoodBedItem : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            CommonItemHelper.SetupPlaceableItem(this, 32, 22, 150, ModContent.TileType<GlimmerwoodBed>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 15)
                .AddIngredient(ItemID.Silk, 5)
                .AddTile(ModContent.TileType<CrystallineFabricator>())
                .Register();
        }
    }
}