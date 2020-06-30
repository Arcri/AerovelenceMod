using AerovelenceMod;
using AerovelenceMod.Blocks.CrystalCaverns.Tiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod
{
	public class AeroWorld : ModWorld
	{
		public static int cavernTiles;
		public static float rotationTime = 0;

		public override void PreUpdate()
		{
			rotationTime += (float)Math.PI / 65;
			if (rotationTime >= Math.PI * 3)
			{
				rotationTime = 0;
			}
		}


		public override void TileCountsAvailable(int[] tileCounts)
		{
			cavernTiles = tileCounts[TileType<CavernStone>()] + tileCounts[TileType<CrystalGrass>()] + tileCounts[TileType<CrystalDirt>()] + tileCounts[TileType<CavernCrystal>()];
		}

		public override void ResetNearbyTileEffects()
		{
			AeroPlayer modPlayer = Main.LocalPlayer.GetModPlayer<AeroPlayer>();
			cavernTiles = 0;
		}
    }
}