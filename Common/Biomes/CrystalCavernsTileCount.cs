using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace ExampleMod.Content.Biomes
{
	public class CrystalCavernsTileCount : ModSystem
	{
		public int CavernTiles;
		public int CitadelTiles;

		public override void ResetNearbyTileEffects()
		{
			CavernTiles = 0;
			CitadelTiles = 0;
		}

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
		{
			CavernTiles = tileCounts[ModContent.TileType<CavernStone>()] +
						  tileCounts[ModContent.TileType<CrystalGrass>()] +
						  tileCounts[ModContent.TileType<CrystalDirt>()] +
						  tileCounts[ModContent.TileType<CavernCrystal>()];

			//CitadelTiles = tileCounts[ModContent.TileType<CitadelStone>()];
		}
	}
}