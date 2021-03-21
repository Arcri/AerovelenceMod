using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class CrystalGrassDev : AerovelenceItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Crystal Grass");

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.autoReuse = true;
            item.consumable = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = ModContent.TileType<CrystalGrass>();

            item.useStyle = ItemUseStyleID.SwingThrow;
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DirtBlock);
            recipe.AddTile(TileID.Furnaces);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
