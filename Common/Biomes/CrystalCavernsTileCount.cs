using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Biomes
{
	public class CrystalCavernsTileCount : ModSystem
	{
		public int FieldsTiles;
		public int CavernTiles;
		public int CitadelTiles;

		public override void ResetNearbyTileEffects()
		{
			FieldsTiles = 0;
			CavernTiles = 0;
		}

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
		{
			FieldsTiles = tileCounts[ModContent.TileType<CrystalGrassTile>()] +
						  tileCounts[ModContent.TileType<CrystalDirtTile>()] +
						  tileCounts[ModContent.TileType<CavernStoneTile>()];

			CavernTiles = tileCounts[ModContent.TileType<CavernStoneTile>()] +
						  tileCounts[ModContent.TileType<CavernCrystalTile>()];

			//CitadelTiles = tileCounts[ModContent.TileType<CitadelStone>()];
		}
	}
}