using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Placeable.Ores
{
    public class BurnshockOreItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burnshock Ore");
		}
        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.useTurn = true;
            item.consumable = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = Item.sellPrice(0, 0, 12, 0);
            item.createTile = mod.TileType("BurnshockOreBlock");
        }
    }
}
