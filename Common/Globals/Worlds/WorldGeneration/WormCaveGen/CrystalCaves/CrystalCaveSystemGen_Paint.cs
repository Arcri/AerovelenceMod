using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles;
using AerovelenceMod.Content.Walls;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Globals.Worlds.WorldGeneration.WormCaveGen.CrystalCaves
{
    /// <summary>
    /// Represents a crystal cave system; a type of worm cave system world gen. Uses `Create` as its primary
    /// factory method.
    /// </summary>
    public partial class CrystalCaveSystemGen : WormSystemGen {
		protected override bool PaintTileInner(int i, int j, float percToEdge)
		{
			Tile t = Framing.GetTileSafely(i, j);
			bool changed = t.HasTile || t.WallType != (ushort)ModContent.WallType<CavernWall>() || t.LiquidAmount > 0;

			t.HasTile = true;
			t.WallType = (ushort)ModContent.WallType<CavernWall>();
			t.LiquidAmount = 0;
			return changed;
		}

		protected override bool PaintTileOuter(int i, int j, float percToEdge)
		{
			Tile t = Framing.GetTileSafely(i, j);
			bool changed = !t.HasTile
					|| t.TileType != (ushort)ModContent.TileType<CavernStone>()
					|| t.WallType != (ushort)ModContent.WallType<CavernWall>();

			t.TileType = (ushort)ModContent.TileType<CavernStone>();
			t.WallType = (ushort)ModContent.WallType<CavernWall>();
			t.Slope = 0;
			t.HasTile = true;
			return changed;
		}
	}
}