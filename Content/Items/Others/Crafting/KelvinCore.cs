using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Crafting
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
            recipe.AddIngredient(ModContent.ItemType<CavernCrystal>(), 3);
            recipe.AddTile(TileID.Furnaces);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}