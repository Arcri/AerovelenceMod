using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Flora
{
	public class CrystalVines : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileCut[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileNoFail[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLighted[Type] = false;
			SoundType = SoundID.Grass;
			DustType = 116;
			AddMapEntry(new Color(100, 125, 255));
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			Tile tile = Framing.GetTileSafely(i, j + 1);
			if (tile.HasTile && tile.TileType == Type)
			{
				WorldGen.KillTile(i, j + 1);
			}
		}


		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tileAbove = Framing.GetTileSafely(i, j - 2);
			int type = -1;
			if (tileAbove.HasTile && !tileAbove.BottomSlope)
			{
				type = tileAbove.TileType;
			}

			if (type == ModContent.TileType<CrystalGrass>() || type == Type)
			{
				return true;
			}

			WorldGen.KillTile(i, j);
			return true;
		}

		public override void RandomUpdate(int i, int j)
		{
			Tile tileBelow = Framing.GetTileSafely(i, j + 1);
			if (WorldGen.genRand.NextBool(15) && !tileBelow.HasTile && !tileBelow.Lava)
			{
				bool placeVine = false;
				int yTest = j;
				while (yTest > j - 10)
				{
					Tile testTile = Framing.GetTileSafely(i, yTest);
					if (testTile.BottomSlope)
					{
						break;
					}
					else if (!testTile.HasTile || testTile.TileType != ModContent.TileType<CrystalGrass>())
					{
						yTest--;
						continue;
					}
					placeVine = true;
					break;
				}
				if (placeVine)
				{
					tileBelow.TileType = Type;
					tileBelow.HasTile;
					WorldGen.SquareTileFrame(i, j + 1, true);
					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
					}
				}
			}
		}
	}
}