using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class CrystalDirt : AerovelenceItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Crystal Dirt");

        public override void SetDefaults()
        {
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.CrystalDirt>();

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
