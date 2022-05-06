using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class CrystalDirtItem : ModItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Crystal Dirt");

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.useTurn = true;
            Item.autoReuse = true;

            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;

            Item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.CrystalDirt>();

            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.DirtBlock)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
