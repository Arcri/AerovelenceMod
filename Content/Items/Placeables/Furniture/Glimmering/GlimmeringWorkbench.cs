using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering
{
    public class GlimmeringWorkbench: ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glimmering Workbench");
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
            item.createTile = mod.TileType("GlimmeringWorkbench"); //put your CustomBlock Tile name
        }


        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Glimmerwood>(), 10);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
