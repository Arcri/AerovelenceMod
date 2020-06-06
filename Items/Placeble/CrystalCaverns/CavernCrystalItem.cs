using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Placeble.CrystalCaverns
{
    public class CavernStalactiteItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cavern Crystaleeee");
        }

        public override void SetDefaults()
        {
			item.CloneDefaults(ItemID.DartTrap);
            item.createTile = mod.TileType("CavernStalactite"); //put your CustomBlock Tile name
        }
    }
}