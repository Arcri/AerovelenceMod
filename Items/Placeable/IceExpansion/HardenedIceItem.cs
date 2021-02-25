using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace AerovelenceMod.Items.Placeable.IceExpansion
{
    public class HardenedIceItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Valley Dirt");
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
            item.createTile = mod.TileType("HardenedIce"); //put your CustomBlock Tile name
            item.consumable = true;
        }
    }
}
