using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Arsenal
{
    public class MilitaryMetalItem : AerovelenceItem
    {
		//public override void SetStaticDefaults() => DisplayName.SetDefault("Military Metal");

        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;

            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;

            //Item.createTile = ModContent.TileType<MilitaryMetal>();

            Item.useStyle = ItemUseStyleID.Swing;
        }
    }
}
