using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using AerovelenceMod.Blocks.CrystalCaverns.Tiles;
using Terraria.ID;

namespace AerovelenceMod.Blocks.CrystalCaverns.Rubble
{
	public class CavernsRubbleFloor : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileMergeDirt[Type] = true;
			dustType = 116;
			drop = mod.ItemType("CavernCrystal");
			soundType = SoundID.Shatter;
			AddMapEntry(new Color(100, 125, 255));
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 59;
		}
		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height)
		{
			offsetY = 2;
		}
		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tileBelow = Framing.GetTileSafely(i, j + 1);
			if (!tileBelow.active() || tileBelow.halfBrick() || tileBelow.topSlope() || tileBelow.type != ModContent.TileType<CavernStone>())
			{
				WorldGen.KillTile(i, j);
			}
			return true;
		}
        public override void NearbyEffects(int i, int j, bool closer)
		{
			Lighting.AddLight(new Vector2(i, j) * 16, new Vector3(0.0f, 0.20f, 0.60f));
		}
	}
}