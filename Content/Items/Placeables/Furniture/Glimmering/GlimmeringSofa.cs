using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering
{
    public class GlimmeringSofa : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glimmering Sofa");
		}
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = Item.sellPrice(0, 0, 0, 0);
            item.createTile = mod.TileType("GlimmeringSofa"); //put your CustomBlock Tile name
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemType<Glimmerwood>(), 5);
            modRecipe.AddIngredient(ItemID.Silk, 2);
            modRecipe.AddTile(TileType<CrystallineFabricator>());
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}
