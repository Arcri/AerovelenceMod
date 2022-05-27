using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.IceExpansion
{
    public class ValleyDirt : ModTile
    {
        public override void SetStaticDefaults()
        {
			MineResist = 2.5f;
			MinPick = 59;
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][TileID.SnowBlock] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileMerge[Type][TileID.IceBlock] = true;
            Main.tileMerge[Type][TileID.CorruptIce] = true;
            Main.tileMerge[Type][TileID.FleshIce] = true;
            Main.tileMerge[Type][TileID.HallowedIce] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("ValleyGrass").Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
			AddMapEntry(new Color(061, 079, 110));
			DustType = 59;
			HitSound = SoundID.Tink;
			ItemDrop = ItemID.DirtBlock;

        }
    }
}
