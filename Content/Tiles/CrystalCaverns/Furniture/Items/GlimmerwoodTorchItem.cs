using AerovelenceMod.Common.Utilities;
using Terraria.ModLoader;
using Terraria.ID;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;
using Newtonsoft.Json.Linq;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items
{
    public class GlimmerwoodTorchItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.width = 10; // Torch item width
            Item.height = 12; // Torch item height
            Item.maxStack = 999; // Torches typically stack up to 999
            Item.value = 11;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing; // Torches use the swing style
            Item.consumable = true;
            Item.createTile = ModContent.TileType<GlimmerwoodTorch>();
            Item.holdStyle = 1; // Enable holding the torch
            Item.placeStyle = 0; // Usually 0 unless there are multiple styles
            Item.flame = true; // Ensure the item is treated as a flame
        }

        public override void SetDefaults()
        {
            CommonItemHelper.SetupTorch(this, ModContent.TileType<GlimmerwoodTorch>(), 50, ItemID.ShimmerTorch, 0.9f, 0.9f, 0.9f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 1)
                .AddIngredient(ItemID.Gel)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}