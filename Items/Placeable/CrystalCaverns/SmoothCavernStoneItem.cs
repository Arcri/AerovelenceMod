using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;

namespace AerovelenceMod.Items.Placeable.CrystalCaverns
{
    public class SmoothCavernStoneItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Smooth Cavern Stone");
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
            item.consumable = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = Item.sellPrice(0, 0, 0, 0);
            item.createTile = mod.TileType("SmoothCavernStone"); //put your CustomBlock Tile name
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemType<CavernStoneItem>(), 1);
            modRecipe.AddTile(TileID.WorkBenches);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}