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
		public int CavernTiles;
		public int CitadelTiles;

		public override void ResetNearbyTileEffects()
		{
			CavernTiles = 0;
		}

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
		{
			
			CavernTiles = tileCounts[ModContent.TileType<CrystalGrass>()] +
						  tileCounts[ModContent.TileType<CrystalDirt>()] +
						  tileCounts[ModContent.TileType<CavernSand>()] +
                          tileCounts[ModContent.TileType<CavernStone>()] +
                          tileCounts[ModContent.TileType<ChargedStone>()] +
                          tileCounts[ModContent.TileType<CavernCrystal>()];
			

			//CitadelTiles = tileCounts[ModContent.TileType<CitadelStone>()];
		}
	}
}