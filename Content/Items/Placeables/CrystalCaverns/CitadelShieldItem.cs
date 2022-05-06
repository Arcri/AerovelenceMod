using AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.CrystalCaverns
{
    public class CitadelShieldItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Citadel Shield");
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
            Item.createTile = Mod.Find<ModTile>("CitadelShield").Type; //put your CustomBlock Tile name
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<Glimmerwood>(), 20)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(ModContent.TileType<CrystallineFabricator>())
                .Register();
        }
    }
}
