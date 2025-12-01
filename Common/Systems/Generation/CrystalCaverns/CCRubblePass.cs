using AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble;
using Steamworks;
using System;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AerovelenceMod.Common.Systems.Generation.CrystalCaverns
{
    public class CCRubblePass : GenPass
    {
        public CCRubblePass(string name, float loadWeight) : base(name, loadWeight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            //progress.Message = WorldGenSystem.CrystalCavernsTerrainPassMessage.Value;
            progress.Message = "Generating Crystal Caverns Rubble";

            CCTerrainPass mainPass = CCTerrainPass.Instance();

            int[] tileTypes = [ModContent.TileType<CavernStone1x1FloorRubbleNatural>(), ModContent.TileType<CavernStone1x1CeilingRubbleNatural>(), ModContent.TileType<CavernStone1x2FloorRubbleNatural>(), ModContent.TileType<CavernStone1x2CeilingRubbleNatural>(), ModContent.TileType<CavernStone3x2FloorRubbleNatural>(), ModContent.TileType<CavernPot2x2Rubble>()];

            for (int i = 0; i < 1000 * Math.Pow(mainPass.WorldSizeScale, 2); i++)
            {
                bool success = false;
                int attempts = 0;
                while (!success)
                {
                    attempts++;
                    if (attempts > 1000)
                    {
                        break;
                    }
                    int x = WorldGen.genRand.Next(mainPass.Origin.X - mainPass.BiomeWidth / 2, mainPass.Origin.X + mainPass.BiomeWidth / 2);
                    int y = WorldGen.genRand.Next(mainPass.Origin.Y - (int)(mainPass.SurfaceHeight * 2.25), mainPass.Origin.Y + mainPass.UndergroundHeight);

                    // Ensure rubble is only placed within the biome
                    // TotalUnderground is relative to the origin, not the world, so subtract the origin
                    if (y > mainPass.Origin.Y && !mainPass.TotalUnderground.Contains(x - mainPass.Origin.X, y - mainPass.Origin.Y))
                    {
                        continue;
                    }

                    int tileType = WorldGen.genRand.Next(tileTypes);
                    int placeStyle = 0; // Default value
                    
                    if (tileType == ModContent.TileType<CavernStone1x1CeilingRubbleNatural>() || tileType == ModContent.TileType<CavernStone1x2FloorRubbleNatural>() || tileType == ModContent.TileType<CavernStone1x2CeilingRubbleNatural>())
                    {
                        // This tile type has 6 variants so pick one at random
                        placeStyle = WorldGen.genRand.Next(6);
                    }
                    else if (tileType == ModContent.TileType<CavernStone3x2FloorRubbleNatural>())
                    {
                        placeStyle = WorldGen.genRand.Next(7);
                    }
                    else if (tileType == ModContent.TileType<CavernStone1x1FloorRubbleNatural>())
                    {
                        placeStyle = WorldGen.genRand.Next(12);
                    }
                    else if (tileType == ModContent.TileType<CavernPot2x2Rubble>())
                    {
                        placeStyle = WorldGen.genRand.Next(3);
                    }

                    if (Main.tile[x, y].TileType == tileType)
                    {
                        continue;
                    }

                    WorldGen.PlaceTile(x, y, tileType, mute: true, style: placeStyle);
                    success = Main.tile[x, y].TileType == tileType;
                }
            }
        }
    }
}
