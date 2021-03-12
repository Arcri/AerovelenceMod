using AerovelenceMod.Blocks.CrystalCaverns.Tiles;
using AerovelenceMod.Items.Placeable.CrystalCaverns;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Others.Crafting
{
    public class KelvinCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kelvin Core");
        }		
        public override void SetDefaults()
        {
			item.maxStack = 999;
            item.width = 26;
            item.height = 22;
            item.value = 10;
            item.rare = ItemRarityID.Green;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CavernCrystalItem>(), 3);
            recipe.AddTile(TileID.Furnaces);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}