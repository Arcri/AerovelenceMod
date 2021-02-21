using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using AerovelenceMod.Items.Placeable.CrystalCaverns;

namespace AerovelenceMod.Blocks.CrystalCaverns.Tiles
{
    public class ChargedStone : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 2.5f;
			minPick = 59;
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][mod.TileType("CrystalDirt")] = true;
            Main.tileMerge[Type][mod.TileType("CrystalGrass")] = true;
            Main.tileMerge[Type][mod.TileType("CavernStone")] = true;
            Main.tileMerge[Type][mod.TileType("ChargedStone")] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(089, 120, 179));
			dustType = 59;
			soundType = SoundID.Tink;
			drop = ModContent.ItemType<ChargedStoneItem>();
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.6f;
            b = 0.9f;
        }
    }
}