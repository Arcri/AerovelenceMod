using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering
{
	public class GlimmeringDoor : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glimmering Door");
		}

		public override void SetDefaults()
		{
			item.width = 14;
			item.height = 28;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.consumable = true;
			item.value = 150;
			item.createTile = TileType<GlimmeringDoorClosed>();
		}
		public override void AddRecipes()
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddIngredient(ItemType<Glimmerwood>(), 6);
			modRecipe.SetResult(this, 1);
			modRecipe.AddRecipe();
		}
	}
}