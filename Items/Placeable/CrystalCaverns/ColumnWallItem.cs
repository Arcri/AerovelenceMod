using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;
using AerovelenceMod.Blocks.CrystalCaverns.Walls;

namespace AerovelenceMod.Items.Placeable.CrystalCaverns
{
	public class ColumnWallItem : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Column Wall Wall");
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
			item.createWall = WallType<ColumnWall>();
		}
		public override void AddRecipes()
		{
			ModRecipe modRecipe = new ModRecipe(mod);
			modRecipe.AddIngredient(ModContent.ItemType<CrystalColumnItem>(), 1);
			modRecipe.AddTile(TileID.WorkBenches);
			modRecipe.SetResult(this, 4);
			modRecipe.AddRecipe();
		}
	}
}