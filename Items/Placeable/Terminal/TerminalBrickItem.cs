using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace AerovelenceMod.Items.Placeable.Terminal
{
    public class TerminalBrickItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Terminal Brick");
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
			item.value = Item.sellPrice(0, 0, 9, 0);
            item.createTile = mod.TileType("TerminalBrick"); //put your CustomBlock Tile name
        }
    }
}
