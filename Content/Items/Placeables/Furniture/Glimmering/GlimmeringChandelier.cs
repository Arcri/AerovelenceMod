using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering
{
    public class GlimmeringChandelier : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glimmering Chandelier");
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
			Item.createTile = Mod.Find<ModTile>("GlimmeringChandelier").Type; //put your CustomBlock Tile name
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ModContent.ItemType<Glimmerwood>(), 4)
				.AddIngredient(ModContent.ItemType<CavernCrystal>(), 4)
				.AddIngredient(ItemID.Chain, 1)
				.AddTile(ModContent.TileType<CrystallineFabricator>())
				.Register();
		}
	}
}
