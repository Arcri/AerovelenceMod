using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles
{
    public class GlimmerwoodBeam : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 2.5f;
			minPick = 59;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(061, 079, 110));
			dustType = 59;
			soundType = SoundID.Tink;
            drop = ModContent.ItemType<Items.Placeables.Blocks.GlimmerwoodBeam>();

        }
    }
}