using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural.Flora
{
    public class CrystalFlora : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileMergeDirt[Type] = true;
            DustType = 116;
            HitSound = SoundID.Grass;
            AddMapEntry(new Color(100, 125, 255));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 2;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            if (!tileBelow.HasTile || tileBelow.IsHalfBlock || TileID.Sets.Platforms[tileBelow.TileType] || tileBelow.TileType != ModContent.TileType<CrystalGrass>())
            {
                WorldGen.KillTile(i, j);
            }
            return true;
        }
    }
}