using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class SmoothCavernStone : ModItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Smooth Cavern Stone");

        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;

            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;

            Item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.SmoothCavernStone>();

            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<CavernStone>())
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
