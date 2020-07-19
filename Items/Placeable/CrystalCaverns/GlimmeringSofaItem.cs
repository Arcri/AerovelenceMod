using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;

namespace AerovelenceMod.Items.Placeable.CrystalCaverns
{
    public class GlimmeringSofaItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glimmering Sofa");
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
            item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = Item.sellPrice(0, 0, 0, 0);
            item.createTile = mod.TileType("GlimmeringSofa"); //put your CustomBlock Tile name
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<Glimmerwood>(), 5);
            modRecipe.AddIngredient(ItemID.Torch, 1);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}