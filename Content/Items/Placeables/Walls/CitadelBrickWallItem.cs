using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Placeables.Walls
{
	public class CitadelBrickWallItem : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Citadel Brick Wall");
		}

		public override void SetDefaults()
		{
			item.width = 12;
			item.height = 12;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 7;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			//item.createWall = WallType<CitadelBrickWall>();
		}
		public override void AddRecipes()
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			//modRecipe.AddIngredient(ModContent.ItemType<CitadelBrick>(), 1);
			modRecipe.AddTile(TileID.WorkBenches);
			modRecipe.SetResult(this, 4);
			modRecipe.AddRecipe();
		}
	}
}