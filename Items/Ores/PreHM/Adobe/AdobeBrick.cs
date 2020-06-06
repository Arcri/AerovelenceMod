using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Items.Tiles;

namespace AerovelenceMod.Items.Ores.PreHM.Adobe
{
    public class AdobeBrick : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Adobe Brick");
			Tooltip.SetDefault("It's a chunk of mud");
		}
		
        public override void SetDefaults()
        {
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.consumable = true;
			item.placeStyle = 0;
			item.consumable = true;
			item.createTile = ModContent.TileType<Tiles.AdobeBrickPlaced>();
			item.maxStack = 999;
			item.value = Item.sellPrice(0, 0, 17, 0);
        }
		public override void AddRecipes()  //How to craft this gun
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AdobeOreItem>(), 5);
			recipe.AddIngredient(ItemID.PlatinumBar, 1);
            recipe.AddTile(TileID.Furnaces);
            recipe.SetResult(this);
            recipe.AddRecipe();

        }
    }
}