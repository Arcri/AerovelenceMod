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
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 6));
		}
		public override void SetDefaults()
		{
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.width = 66;
			item.height = 24;
			item.autoReuse = true;
			item.consumable = true;
			item.noUseGraphic = true;
			item.placeStyle = 0;
			item.consumable = true;
			item.createTile = ModContent.TileType<BurnshockBarPlaced>();
			item.maxStack = 999;
			item.value = Item.sellPrice(0, 0, 20, 0);
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BurnshockOre>(), 3);
			recipe.AddIngredient(ModContent.ItemType<ChargedStoneItem>(), 1);
			recipe.AddTile(TileID.Furnaces);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}