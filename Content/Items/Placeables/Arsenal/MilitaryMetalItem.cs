using AerovelenceMod.Content.Tiles.Arsenal;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Arsenal
{
    public class MilitaryMetalItem : AerovelenceItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Military Metal");

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.autoReuse = true;
            item.consumable = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = ModContent.TileType<MilitaryMetal>();

            item.useStyle = ItemUseStyleID.SwingThrow;
        }
    }
}
