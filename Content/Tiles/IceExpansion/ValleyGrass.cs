using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Flora;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.IceExpansion
{
    public class ValleyGrass : ModTile
	{
		public static int _type;
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			SetModTree(new ValleyTree());
			Main.tileMerge[Type][Mod.Find<ModTile>("ValleyGrass").Type] = true;
			Main.tileBlendAll[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(100, 155, 255));
			ItemDrop = Mod.Find<ModItem>("ValleyDirt").Type;
			TileID.Sets.Grass[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = Mod.Find<ModTile>("ValleyDirt").Type;
		}
		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (!effectOnly)
			{
				fail = true;
				Main.tile[i, j].TileType = (ushort)Mod.Find<ModTile>("ValleyDirt").Type;
				WorldGen.SquareTileFrame(i, j, true);
				Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.Marble, 0f, 0f, 0, new Color(121, 121, 121), 1f);
			}
		}
		public static bool PlaceObject(int x, int y, int type, bool mute = false, int style = 0, int alternate = 0, int random = -1, int direction = -1)
		{
			TileObject toBePlaced;
			if (!TileObject.CanPlace(x, y, type, style, direction, out toBePlaced, false))
			{
				return false;
			}
			toBePlaced.random = random;
			if (TileObject.Place(toBePlaced) && !mute)
			{
				WorldGen.SquareTileFrame(x, y, true);
			}
			return false;
		}
		public override void RandomUpdate(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			Tile tileBelow = Framing.GetTileSafely(i, j + 1);
			Tile tileAbove = Framing.GetTileSafely(i, j - 1);
			if (WorldGen.genRand.NextBool(25) && !tileAbove.HasTile && tile.LiquidType != LiquidID.Lava)
			{
				if (!tile.BottomSlope && !tile.TopSlope && !tile.IsHalfBlock && !tile.TopSlope)
				{
					tileAbove.TileType = (ushort)ModContent.TileType<CrystalFlora>();
					tileAbove.HasTile = true;
					tileAbove.TileFrameY = 0;
					tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(8) * 18);
					WorldGen.SquareTileFrame(i, j + 1, true);
					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
					}
				}
			}
			if (WorldGen.genRand.NextBool(15) && !tileBelow.HasTile && tile.LiquidType != LiquidID.Lava)
			{
				if (!tile.BottomSlope)
				{
					tileBelow.TileType = (ushort)ModContent.TileType<CrystalVines>();
					tileBelow.HasTile = true;
					WorldGen.SquareTileFrame(i, j + 1, true);
					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
					}
				}
			}
		}
		public override int SaplingGrowthType(ref int style)
		{
			style = 0;
			return TileID.Saplings;
		}
	}
}