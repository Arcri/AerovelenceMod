using Terraria;
using Terraria.ModLoader;
using AerovelenceMod.Items.Placeable.CrystalCaverns;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Blocks.CrystalCaverns.Tiles
{
    public class GlimmerwoodTile : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 1.0f;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
			AddMapEntry(new Color(052, 056, 073));
			dustType = 37;
			soundType = SoundID.Dig;
			drop = ModContent.ItemType<Glimmerwood>();

        }
    }
}