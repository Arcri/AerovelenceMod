using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Blocks.FrostDungeon.Tiles
{
    public class SubzeroBrickItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Subzero Brick");
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
            item.useStyle = 1;
			item.value = Item.sellPrice(0, 0, 9, 0);
            item.createTile = mod.TileType("SubzeroBrick"); //put your CustomBlock Tile name
        }
    }
}
