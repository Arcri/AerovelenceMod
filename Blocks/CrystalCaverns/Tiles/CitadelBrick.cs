using Terraria;
using Terraria.ModLoader;
using AerovelenceMod.Items.Placeable.CrystalCaverns;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Blocks.CrystalCaverns.Tiles
{
    public class CitadelBrick : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 2.5f;
			minPick = 180;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
			AddMapEntry(new Color(061, 079, 110));
			dustType = 59;
			soundType = SoundID.Tink;
			drop = ModContent.ItemType<CitadelBrickItem>();

        }
    }
}