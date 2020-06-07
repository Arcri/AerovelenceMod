using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Placeble.CrystalCaverns
{
    public class CavernCrystalItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cavern Crystaleeee");
        }

        public override void SetDefaults()
        {
			item.CloneDefaults(ItemID.DartTrap);
            item.createTile = mod.TileType("CavernCrystal"); //put your CustomBlock Tile name
        }
    }
}