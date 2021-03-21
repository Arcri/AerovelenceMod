using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class SmoothCavernStone : AerovelenceItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Smooth Cavern Stone");

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.autoReuse = true;
            item.consumable = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.SmoothCavernStone>();

            item.useStyle = ItemUseStyleID.SwingThrow;
        }

        public override void AddRecipes()
        {
            var modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<CavernStone>());
            modRecipe.AddTile(TileID.WorkBenches);
            modRecipe.SetResult(this);
            modRecipe.AddRecipe();
        }
    }
}
