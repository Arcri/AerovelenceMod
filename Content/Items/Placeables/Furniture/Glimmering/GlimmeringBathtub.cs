using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering
{
    public class GlimmeringBathtub: AerovelenceItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Glimmering Bathtub");

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.autoReuse = true;
            item.consumable = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = ModContent.TileType<Tiles.CrystalCaverns.Tiles.Furniture.GlimmeringBathtub>(); //put your CustomBlock Tile name

            item.useStyle = ItemUseStyleID.SwingThrow;
        }

        public override void AddRecipes()
        {
            var modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<Glimmerwood>(), 14);
            modRecipe.AddTile(ModContent.TileType<CrystallineFabricator>());
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}
