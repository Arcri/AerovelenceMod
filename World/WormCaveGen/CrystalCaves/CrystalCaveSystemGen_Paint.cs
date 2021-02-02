using AerovelenceMod.Blocks.CrystalCaverns.Tiles;
using AerovelenceMod.Blocks.CrystalCaverns.Walls;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.World.WormCaveGen.CrystalCaves
{
    /// <summary>
    /// Represents a crystal cave system; a type of worm cave system world gen. Uses `Create` as its primary
    /// factory method.
    /// </summary>
    public partial class CrystalCaveSystemGen : WormSystemGen {
		protected override bool PaintTileInner(int i, int j, float percToEdge)
		{
			Tile t = Framing.GetTileSafely(i, j);
			bool changed = t.active() || t.wall != (ushort)ModContent.WallType<CavernWall>() || t.liquid > 0;

			t.active(false);
			t.wall = (ushort)ModContent.WallType<CavernWall>();
			t.liquid = 0;
			return changed;
		}

		protected override bool PaintTileOuter(int i, int j, float percToEdge)
		{
			Tile t = Framing.GetTileSafely(i, j);
			bool changed = !t.active()
					|| t.type != (ushort)ModContent.TileType<CavernStone>()
					|| t.wall != (ushort)ModContent.WallType<CavernWall>();

			t.type = (ushort)ModContent.TileType<CavernStone>();
			t.wall = (ushort)ModContent.WallType<CavernWall>();
			t.slope(0);
			t.active(true);
			return changed;
		}
	}
}