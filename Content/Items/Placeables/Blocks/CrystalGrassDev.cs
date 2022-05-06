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
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;

            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;

            Item.createTile = ModContent.TileType<CrystalGrass>();

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
