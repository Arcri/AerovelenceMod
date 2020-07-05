using Terraria;
using Terraria.ModLoader;
using AerovelenceMod.Items.Placeable.CrystalCaverns;
using System;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Blocks.CrystalCaverns.Tiles
{
    public class CavernCrystal : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 2.5f;
			minPick = 59;
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<CavernStone>()] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
			AddMapEntry(new Color(115, 230, 250));
			dustType = 59;
			soundType = SoundID.Tink;
			drop = ModContent.ItemType<CavernCrystalItem>();
		}
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)   //light colors
        {
            r = 0.0f;
            g = 0.5f;
            b = 0.9f;
        }
    }
}