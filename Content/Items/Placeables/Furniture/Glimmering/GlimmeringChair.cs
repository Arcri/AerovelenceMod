using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering
{
    public class GlimmeringChair: ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glimmering Chair");
		}
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.consumable = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.createTile = Mod.Find<ModTile>("GlimmeringChair").Type; //put your CustomBlock Tile name
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemType<Glimmerwood>(), 4)
                .AddTile(TileType<CrystallineFabricator>())
                .Register();
        }
    }
}
