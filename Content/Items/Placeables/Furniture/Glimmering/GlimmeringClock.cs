using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering
{
    public class GlimmeringClock : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glimmering Clock");
		}
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.createTile = Mod.Find<ModTile>("GlimmeringClock").Type; //put your CustomBlock Tile name
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemType<Glimmerwood>(), 10)
                .AddRecipeGroup(("IronBar"), 3)
                .AddIngredient(ItemID.Glass, 6)
                .AddTile(TileType<CrystallineFabricator>())
                .Register();
        }
    }
}
