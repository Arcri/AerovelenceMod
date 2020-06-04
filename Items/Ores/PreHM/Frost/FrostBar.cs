using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Ores.PreHM.Frost
{
    public class FrostBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Bar");
        }		
        public override void SetDefaults()
        {
			item.maxStack = 999;
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = 2;
			item.value = Item.sellPrice(0, 0, 10, 0);
            item.consumable = true;
        }
		public override void AddRecipes()  //How to craft this item
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FrostShard>(), 3);			
            recipe.AddTile(TileID.Anvils);   //at work bench
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}