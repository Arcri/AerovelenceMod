using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Globals.Worlds
{
    public class ZoneWorld : ModWorld
    {
        public static int CavernTiles { get; private set; }
        public static int CitadelTiles { get; private set; }

        public override void ResetNearbyTileEffects()
        {
            CavernTiles = 0;
            CitadelTiles = 0;
        }

        public override void TileCountsAvailable(int[] tileCounts)
        {
            CavernTiles = tileCounts[ModContent.TileType<CavernStone>()] + 
                          tileCounts[ModContent.TileType<CrystalGrass>()] + 
                          tileCounts[ModContent.TileType<CrystalDirt>()] + 
                          tileCounts[ModContent.TileType<CavernCrystal>()];
            
            CitadelTiles = tileCounts[ModContent.TileType<CitadelStone>()];
        }
    }
}
