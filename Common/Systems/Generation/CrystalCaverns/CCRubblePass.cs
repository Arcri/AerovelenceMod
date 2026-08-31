using AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble;
using Iced.Intel;
using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
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

            // Copied from CCTerrainPass.cs
            Point surfaceRectOrigin = new Point(mainPass.Origin.X - mainPass.BiomeWidth / 2, mainPass.Origin.Y - (int)(mainPass.SurfaceHeight * 2.25));
            Point upperUndergroundOrigin = new Point(mainPass.Origin.X - mainPass.BiomeWidth / 2, mainPass.Origin.Y);
            Point lowerUndergroundOrigin = new Point(mainPass.Origin.X, mainPass.Origin.Y + (int)(.5 * mainPass.UndergroundHeight));
            Point upperUndergroundWallOrigin = new Point(mainPass.Origin.X - mainPass.BiomeWidth / 2 + 1, mainPass.Origin.Y);
            Point lowerUndergroundWallOrigin = new Point(mainPass.Origin.X, mainPass.Origin.Y + (int)(.5 * mainPass.UndergroundHeight) - 1);

            // Sets of rubble to be placed
            int[] potTileTypes = [ModContent.TileType<CavernPot2x2Rubble>()];
            int[] rubbleTileTypes = [
                ModContent.TileType<CavernStone1x1FloorRubbleNatural>(),
                ModContent.TileType<CavernStone1x1CeilingRubbleNatural>(),
                ModContent.TileType<CavernStone1x2FloorRubbleNatural>(),
                ModContent.TileType<CavernStone1x2CeilingRubbleNatural>(),
                ModContent.TileType<CavernStone3x2FloorRubbleNatural>(),
                ModContent.TileType<CavernStone3x2FloorRubbleNatural>()];

            // Tiles that rubble can be placed on
            int[] validRubblePlacementTiles = [
                mainPass.StoneTile, mainPass.ChargedTile, mainPass.LushTile];
            int[] validPotPlacementTiles = [
                mainPass.DirtTile, mainPass.GrassTile, mainPass.StoneTile,
                mainPass.ChargedTile, mainPass.LushTile, mainPass.SandTile];

            // Vanilla rubble to be removed
            ushort[] rubbleRemovalTypes = [
                TileID.Pots,
                TileID.Stalactite,
                TileID.SmallPiles,
                TileID.LargePiles,
                TileID.LargePiles2];

            // Remove vanilla rubble
            WorldUtils.Gen(mainPass.Origin, new ModShapes.All(mainPass.TotalBiome), Actions.Chain(new GenAction[]
            {
                new Modifiers.OnlyTiles(rubbleRemovalTypes),
                new Actions.ClearTile()
            }));

            // Place rubble
            for (int i = 0; i < 1250 * Math.Pow(mainPass.WorldSizeScale, 2); i++)
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
                        continue;

                    // Ensure it is placed on valid tiles, compatible with 1 and 2 tile high rubble
                    if (!validRubblePlacementTiles.Contains(Main.tile[x, y + 1].TileType) && !validRubblePlacementTiles.Contains(Main.tile[x, y + 2].TileType))
                        continue;

                    int tileType = WorldGen.genRand.Next(rubbleTileTypes);
                    int placeStyle = 0; // Default value

                    if (Main.tile[x, y].TileType == tileType)
                        continue;

                    if (tileType == ModContent.TileType<CavernStone1x1CeilingRubbleNatural>() || 
                        tileType == ModContent.TileType<CavernStone1x2FloorRubbleNatural>() || 
                        tileType == ModContent.TileType<CavernStone1x2CeilingRubbleNatural>() ||
                        tileType == ModContent.TileType<CavernStone3x2FloorRubbleNatural>())
                    {
                        // These tile types have 6 variants each so pick one variant at random
                        placeStyle = WorldGen.genRand.Next(6);
                    }
                    else if (tileType == ModContent.TileType<CavernStone1x1FloorRubbleNatural>())
                    {
                        placeStyle = WorldGen.genRand.Next(12);
                    }

                    WorldGen.PlaceTile(x, y, tileType, mute: true, style: placeStyle);
                    success = Main.tile[x, y].TileType == tileType;
                }
            }

            // Place pots
            for (int i = 0; i < 250 * Math.Pow(mainPass.WorldSizeScale, 2); i++)
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
                        continue;

                    // Ensure it is placed on valid tiles, compatible with 1 and 2 tile high rubble
                    if (!validPotPlacementTiles.Contains(Main.tile[x, y + 1].TileType) && !validPotPlacementTiles.Contains(Main.tile[x, y + 2].TileType))
                        continue;

                    // Prevent pots from spawning on the surface but allow them in caves still
                    if (y < mainPass.Origin.Y && Main.tile[x, y].WallType == WallID.None)
                        continue;

                    int tileType = WorldGen.genRand.Next(potTileTypes);
                    int placeStyle = 0; // Default value

                    if (Main.tile[x, y].TileType == tileType)
                        continue;

                    if (tileType == ModContent.TileType<CavernPot2x2Rubble>())
                    {
                        placeStyle = WorldGen.genRand.Next(9);
                    }

                    Console.WriteLine(placeStyle);

                    WorldGen.PlaceTile(x, y, tileType, mute: true, style: placeStyle);
                    success = Main.tile[x, y].TileType == tileType;
                }
            }
        }
    }
}
