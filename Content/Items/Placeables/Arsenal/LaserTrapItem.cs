using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Arsenal
{
    public class LaserTrapItem : AerovelenceItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Laser Trap");


        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;

            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;

            //Item.createTile = ModContent.TileType<LaserTrap>();

            Item.useStyle = ItemUseStyleID.Swing;
        }
    }
}
