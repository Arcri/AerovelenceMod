using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Blocks.CrystalCaverns.Tiles
{
    public class CrystalDirt : ModTile
    {
        public override void SetDefaults()
        {
            mineResist = 2.5f;
            minPick = 59;
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][mod.TileType("CrystalGrass")] = true;
            Main.tileMerge[Type][mod.TileType("CavernStone")] = true;
            Main.tileMerge[Type][mod.TileType("FieldStone")] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
            AddMapEntry(new Color(065, 065, 085));
            dustType = 59;
            soundType = SoundID.Tink;
            drop = mod.ItemType("Crystal Grass");

        }
    }
}