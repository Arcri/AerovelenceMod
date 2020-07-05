using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;

namespace AerovelenceMod.Items.Placeable.CrystalCaverns
{
    public class CavernStoneItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cavern Stone");
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
            item.createTile = mod.TileType("CavernStone"); //put your CustomBlock Tile name
        }
    }
}
