using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles
{
    public class FreshGlimmerwood : ModTile
    {
        public override void SetStaticDefaults()
        {
			mineResist = 1.0f;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
			AddMapEntry(new Color(052, 056, 073));
			dustType = 37;
			soundType = SoundID.Dig;
            drop = ModContent.ItemType<Items.Placeables.Blocks.FreshGlimmerwood>();

        }
    }
}