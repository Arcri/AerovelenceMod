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
            Item.useTurn = true;
            Item.consumable = true;
            Item.autoReuse = true;

            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;

            Item.createTile = ModContent.TileType<BurnshockOreBlock>();

            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(silver: 12);
        }
    }
}
