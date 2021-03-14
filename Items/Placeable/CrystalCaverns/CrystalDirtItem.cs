using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;
using AerovelenceMod.Items.Placeable.Ores;

namespace AerovelenceMod.Items.Placeable.CrystalCaverns
{
    public class CrystalDirtItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crystal Dirt");
		}
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.consumable = true;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = Item.sellPrice(0, 0, 0, 0);
            item.createTile = mod.TileType("CrystalDirt"); //put your CustomBlock Tile name
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DirtBlock, 1);
            recipe.AddTile(TileID.Furnaces);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
