using AerovelenceMod.Content.Tiles.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class PhanticOre : ModItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Phantic Ore");

        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;

            Item.maxStack = 999;
            Item.useTime = 10;
            Item.useAnimation = 15;

            Item.createTile = ModContent.TileType<PhanticOreBlock>();

            Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(silver: 12);
        }
    }
}
