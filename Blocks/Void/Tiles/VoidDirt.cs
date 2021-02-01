using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Blocks.Void.Tiles
{
    public class VoidDirt : ModTile
    {
        public override void SetDefaults()
        {
            mineResist = 2.5f;
            minPick = 59;
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][mod.TileType("VoidGrass")] = true;
            Main.tileMerge[Type][mod.TileType("VoidSilt")] = true;
            Main.tileMerge[Type][mod.TileType("VoidStone")] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
            AddMapEntry(new Color(102, 108, 117));
            dustType = 59;
            soundType = SoundID.Tink;
            drop = mod.ItemType("Void Grass");

        }
    }
}