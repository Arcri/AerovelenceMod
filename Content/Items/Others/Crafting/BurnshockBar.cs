using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles;
using AerovelenceMod.Content.Tiles.Ores;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Crafting
{
    public class BurnshockBar : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burnshock Bar");
			Tooltip.SetDefault("Formed from the raging crystal thunderstorms");
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6));
		}
		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.width = 66;
			Item.height = 24;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.noUseGraphic = true;
			Item.placeStyle = 0;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<BurnshockBarPlaced>();
			Item.maxStack = 999;
			Item.value = Item.sellPrice(0, 0, 20, 0);
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ModContent.ItemType<BurnshockOre>(), 3)
				.AddIngredient(ModContent.ItemType<ChargedStoneItem>(), 1)
				.AddTile(TileID.Furnaces)
				.Register();
		}
	}
}