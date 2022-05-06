
using AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering;
using AerovelenceMod.Content.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Placeables.Walls
{
	public class GlimmerwoodWallItem : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Glimmerwood Wall");
		}

		public override void SetDefaults()
		{
			Item.width = 12;
			Item.height = 12;
			Item.maxStack = 999;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 7;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createWall = WallType<GlimmerwoodWall>();
		}
		public override void AddRecipes()
		{
			CreateRecipe(4)
				.AddIngredient(ModContent.ItemType<Glimmerwood>(), 1)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}