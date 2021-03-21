using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.IceExpansion
{
    public class HardenedIce : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 2.5f;
			minPick = 59;
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][TileID.SnowBlock] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileMerge[Type][TileID.IceBlock] = true;
            Main.tileMerge[Type][TileID.CorruptIce] = true;
            Main.tileMerge[Type][TileID.FleshIce] = true;
            Main.tileMerge[Type][TileID.HallowedIce] = true;
            Main.tileMerge[Type][mod.TileType("ValleyGrass")] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
			AddMapEntry(new Color(061, 079, 110));
			dustType = 59;
			soundType = SoundID.Tink;
			drop = ModContent.ItemType<AfflictionCrystal>();

        }
    }
}