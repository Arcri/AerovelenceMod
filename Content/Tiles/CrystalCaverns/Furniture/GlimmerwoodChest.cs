using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodChest : ModTile
    {
        public override void SetStaticDefaults()
        {
            CommonTileHelper.SetupChest(this, DustID.BlueCrystalShard, ModContent.ItemType<GlimmerwoodChestItem>(), new Color(123, 123, 123), "Glimmerwood Chest");
        }

        public override bool RightClick(int i, int j)
        {
            return CommonTileHelper.HandleRightClick(this, i, j, Main.LocalPlayer, ItemID.GoldenKey);
        }

        public override void MouseOver(int i, int j)
        {
            CommonTileHelper.HandleMouseOver(this, i, j, ModContent.ItemType<GlimmerwoodChestItem>(), ItemID.GoldenKey);
        }
    }

    public class GlimmerwoodChestItem : ModItem
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodChest>());
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