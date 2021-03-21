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
			item.width = 16;
			item.height = 16;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = Item.sellPrice(0, 0, 0, 0);
			item.consumable = true;
			item.createTile = mod.TileType("GlimmeringChandelier"); //put your CustomBlock Tile name
		}
		public override void AddRecipes()
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddIngredient(ModContent.ItemType<Glimmerwood>(), 4);
			modRecipe.AddIngredient(ModContent.ItemType<CavernCrystal>(), 4);
			modRecipe.AddIngredient(ItemID.Chain, 1);
			modRecipe.AddTile(ModContent.TileType<CrystallineFabricator>());
			modRecipe.SetResult(this, 1);
			modRecipe.AddRecipe();
		}
	}
}
