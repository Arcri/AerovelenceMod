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
			Item.maxStack = 999;
            Item.width = 26;
            Item.height = 22;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<CavernCrystal>(), 3)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}