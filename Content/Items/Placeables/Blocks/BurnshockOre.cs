using AerovelenceMod.Content.Tiles.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class BurnshockOre : ModItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Burnshock Ore");

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.consumable = true;
            item.autoReuse = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = ModContent.TileType<BurnshockOreBlock>();

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.value = Item.sellPrice(silver: 12);
        }
    }
}
