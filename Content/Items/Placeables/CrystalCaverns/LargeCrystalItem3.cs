using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.CrystalCaverns
{
    public class LargeCrystalItem3 : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("LargeCrystalItem3");
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
            Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.consumable = true;
            Item.createTile = Mod.Find<ModTile>("LargeCrystal3").Type;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<Glimmerwood>(), 3)
                .AddIngredient(ModContent.ItemType<CavernCrystal>(), 1)
                .AddTile(ModContent.TileType<CrystallineFabricator>())
                .Register();
        }
    }
}
