using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.CrystalCaverns
{
    public class CavernStatueItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cavern Statue");
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
            item.createTile = mod.TileType("CavernStatue"); //put your CustomBlock Tile name
        }
    }
}
