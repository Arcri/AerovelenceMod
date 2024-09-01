using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items
{
    public class GlimmerwoodCandleItem : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            CommonItemHelper.SetupPlaceableItem(this, 16, 18, 150, ModContent.TileType<GlimmerwoodCandle>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 4)
                .AddIngredient(ItemID.Torch, 1)
                .AddTile(ModContent.TileType<CrystallineFabricator>())
                .Register();
        }
    }
}