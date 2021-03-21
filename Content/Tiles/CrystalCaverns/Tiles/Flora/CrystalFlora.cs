using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Flora
{
	public class CrystalFlora : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileNoFail[Type] = true;
			Main.tileMergeDirt[Type] = true;
			dustType = 116;
			soundType = SoundID.Grass;
			AddMapEntry(new Color(100, 125, 255));
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 2;
		}
		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height)
		{
			offsetY = 2;
		}
		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tileBelow = Framing.GetTileSafely(i, j + 1);
			if (!tileBelow.active() || tileBelow.halfBrick() || tileBelow.topSlope() || tileBelow.type != ModContent.TileType<CrystalGrass>())
			{
				WorldGen.KillTile(i, j);
			}
			return true;
		}
	}
}