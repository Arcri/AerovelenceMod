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



		public override void TileCountsAvailable(int[] tileCounts)
		{
			cavernTiles = tileCounts[TileType<CavernStone>()];
		}

		public override void ResetNearbyTileEffects()
		{
			AeroPlayer modPlayer = Main.LocalPlayer.GetModPlayer<AeroPlayer>();
			cavernTiles = 0;
		}
    }
}