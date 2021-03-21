using AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.Blocks
{
    public class AfflictionCrystal : AerovelenceItem
    {
		public override void SetStaticDefaults() => DisplayName.SetDefault("Crystal Dirt");

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.autoReuse = true;
            item.consumable = true;

            item.maxStack = 999;
            item.useAnimation = 15;
            item.useTime = 10;

            item.createTile = ModContent.TileType<CavernsPots>();

            item.useStyle = ItemUseStyleID.SwingThrow;
        }
    }
}
