using AerovelenceMod.Content.Tiles.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class SlateOre : ModItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Slate Slab");

        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;

            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;

            Item.createTile = ModContent.TileType<SlateOreBlock>();

            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(silver: 9);
        }
    }
}
