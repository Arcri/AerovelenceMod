using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Blocks.CrystalCaverns.Tiles
{
    public class RuinedCitadelBrick : ModTile
    {
        public override void SetDefaults()
        {
            mineResist = 2.5f;
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][mod.TileType("CrystalGrass")] = true;
            Main.tileMerge[Type][mod.TileType("CavernCrystal")] = true;
            Main.tileMerge[Type][mod.TileType("CavernStone")] = true;
            Main.tileMerge[Type][mod.TileType("FieldStone")] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(102, 108, 117));
            dustType = 116;
            soundType = SoundID.Tink;
            drop = mod.ItemType("CrystalDirtItem");
        }
        public override bool CanExplode(int i, int j)
        {
            return true;
        }
    }
}