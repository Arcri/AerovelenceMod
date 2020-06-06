using Terraria;
using Terraria.ModLoader;
using AerovelenceMod.Items.Placeble.CrystalCaverns;

namespace AerovelenceMod.Blocks.CrystalCaverns.Tiles
{
    public class CavernStone : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 2.5f;
			minPick = 59;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
			dustType = 59;
			soundType = 21;
			drop = ModContent.ItemType<CavernStoneItem>();

        }
    }
}