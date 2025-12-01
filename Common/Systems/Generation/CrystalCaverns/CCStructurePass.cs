using AerovelenceMod.Common.Globals.Worlds;
using AerovelenceMod.Common.Utilities.Generation;
using AerovelenceMod.Common.Utilities.Generation.StructureStamper;
using AerovelenceMod.Content.Items.Accessories.SmallAccessories;
using AerovelenceMod.Content.Items.Weapons.Aurora.Eos;
using AerovelenceMod.Content.Items.Weapons.CrystalCaverns.CrystalCrescent;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using AerovelenceMod.Content.Walls.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace AerovelenceMod.Common.Systems.Generation.CrystalCaverns
{
    public class CCStructurePass : GenPass
    {
        public CCStructurePass(string name, double loadWeight) : base(name, loadWeight)
        {
        }

        private List<Point> _validPoints;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            bool oldNoTileActions = WorldGen.noTileActions;
            WorldGen.noTileActions = true;
            try
            {
                //progress.Message = WorldGenSystem.CrystalCavernsTerrainPassMessage.Value;
                progress.Message = "Generating Crystal Caverns Structures";

                CCTerrainPass mainPass = CCTerrainPass.Instance();

                #region CC Gen Cleanup
                Point surfaceRectOrigin = new(
                    mainPass.Origin.X - mainPass.BiomeWidth / 2,
                    mainPass.Origin.Y - (int)(mainPass.SurfaceHeight * 1.75)
                );
                ShapeData surfaceRectShapeData = new ShapeData();
                WorldUtils.Gen(
                    surfaceRectOrigin,
                    new Shapes.Rectangle(mainPass.BiomeWidth, (int)(mainPass.SurfaceHeight * 1.75)),
                    new Actions.Blank().Output(surfaceRectShapeData)
                );
                WorldUtils.Gen(
                    surfaceRectOrigin,
                    new ModShapes.All(surfaceRectShapeData),
                    Actions.Chain(
                        new GenAction[]
                        {
                            new Modifiers.OnlyWalls(
                                WallID.DirtUnsafe,
                                WallID.FlowerUnsafe,
                                WallID.GrassUnsafe,
                                59,
                                WallID.SnowWallUnsafe,
                                WallID.Sandstone,
                                WallID.HardenedSand
                            ),
                            new Actions.PlaceWall(mainPass.DirtWall)
                        }
                    )
                );
                WorldUtils.Gen(
                    surfaceRectOrigin,
                    new ModShapes.All(surfaceRectShapeData),
                    Actions.Chain(
                        new GenAction[]
                        {
                            new Modifiers.Expand(3, 0),
                            new Modifiers.Dither(0.75),
                            new Modifiers.OnlyWalls(
                                WallID.DirtUnsafe,
                                WallID.FlowerUnsafe,
                                WallID.GrassUnsafe,
                                59,
                                WallID.SnowWallUnsafe,
                                WallID.Sandstone,
                                WallID.HardenedSand
                            ),
                            new Actions.PlaceWall(mainPass.DirtWall)
                        }
                    )
                );
                WorldUtils.Gen(
                    surfaceRectOrigin,
                    new ModShapes.All(surfaceRectShapeData),
                    Actions.Chain(
                        new GenAction[]
                        {
                            new Modifiers.Expand(5, 0),
                            new Modifiers.Dither(0.75),
                            new Modifiers.OnlyWalls(
                                WallID.DirtUnsafe,
                                WallID.FlowerUnsafe,
                                WallID.GrassUnsafe,
                                59,
                                WallID.SnowWallUnsafe,
                                WallID.Sandstone,
                                WallID.HardenedSand
                            ),
                            new Actions.PlaceWall(mainPass.DirtWall)
                        }
                    )
                );
                #endregion

                UnifiedRandom rand = WorldGen.genRand;
                InitializeValidPoints(mainPass.TotalUnderground);

                #region Loot Pools
                List<PrimaryItemConfiguration> smallShrinePrimary = new()
                {
                    new(ModContent.ItemType<CrystalCrescent>(), 1, 1, 1f)
                };

                List<ItemConfiguration> smallShrineSecondary = new()
                {
                    new(ItemID.SuspiciousLookingEye, 1, 1, 1f/5),
                    new(ItemID.Dynamite, 25, 50, 1f/3),
                    new(new List<int> { ItemID.SilverBar, ItemID.TungstenBar, ItemID.GoldBar, ItemID.PlatinumBar }, 3, 10, 1f/2),
                    new(ItemID.HealingPotion, 3, 5, 1f/2),
                    new(new List<int>
                    {
                        ItemID.SpelunkerPotion, ItemID.FeatherfallPotion, ItemID.NightOwlPotion, ItemID.WaterWalkingPotion,
                        ItemID.ArcheryPotion, ItemID.GravitationPotion, ItemID.ThornsPotion, ItemID.InvisibilityPotion,
                        ItemID.HunterPotion, ItemID.BattlePotion, ItemID.TeleportationPotion
                    }, 1, 2, 2f/3), // Vanilla splits the potions into two item slots for caverns chests
                    new(ItemID.RecallPotion, 1, 2, 1f/2),
                    new(new List<int> { ItemID.Torch, ItemID.Glowstick }, 15, 29, 1f/2),
                    new(ModContent.ItemType <CavernCrystalItem>(), 10, 30, 1f),
                    new(ItemID.GoldCoin, 1, 2, 1f/2)
                };

                List<PrimaryItemConfiguration> genericLootPrimary = new()
                {
                    new(ItemID.BandofRegeneration, 1, 1, 1f),
                    new(ItemID.MagicMirror, 1, 1, 1f),
                    new(ItemID.CloudinaBottle, 1, 1, 1f),
                    new(ItemID.HermesBoots, 1, 1, 1f),
                    new(ItemID.Mace, 1, 1, 1f),
                    new(ModContent.ItemType<CrystalStompers>(), 1, 1, 1f)
                };

                List<ItemConfiguration> genericLootSecondary = new()
                {
                    new(ItemID.SuspiciousLookingEye, 1, 1, 1f/5),
                    new(ItemID.Dynamite, 25, 50, 1f/3),
                    new(new List<int> { ItemID.SilverBar, ItemID.TungstenBar, ItemID.GoldBar, ItemID.PlatinumBar }, 3, 10, 1f/2),
                    new(ItemID.HealingPotion, 3, 5, 1f/2),
                    new(new List<int>
                    {
                        ItemID.SpelunkerPotion, ItemID.FeatherfallPotion, ItemID.NightOwlPotion, ItemID.WaterWalkingPotion,
                        ItemID.ArcheryPotion, ItemID.GravitationPotion, ItemID.ThornsPotion, ItemID.InvisibilityPotion,
                        ItemID.HunterPotion, ItemID.BattlePotion, ItemID.TeleportationPotion
                    }, 1, 2, 2f/3), // Vanilla splits the potions into two item slots for caverns chests
                    new(ItemID.RecallPotion, 1, 2, 1f/2),
                    new(new List<int> { ItemID.Torch, ItemID.Glowstick }, 15, 29, 1f/2),
                    new(ModContent.ItemType <CavernCrystalItem>(), 10, 30, 1f),
                    new(ItemID.GoldCoin, 1, 2, 1f/2)
                };

                #endregion

                WorldGen.noTileActions = false;

                StructureStamper.LoadStructure(
                    new Vector2(
                        mainPass.TumblerTunnelEnd.X - 60 + 60 * mainPass.TumblerArenaPolarity,
                        mainPass.TumblerTunnelEnd.Y - 46
                    ),
                    "tumblerarena"
                ).ProtectStructure();

                PlaceStructureSafely("ancientbridge")
                    .ProtectStructure()
                    .ApplyItemConfigurationsToAll(rand, genericLootPrimary, genericLootSecondary);
                PlaceStructureSafely("smallshrine")
                        .ProtectStructure()
                        .ApplyItemConfigurationsToAll(rand, smallShrinePrimary, smallShrineSecondary);
                if (rand.NextBool())
                {
                    PlaceStructureSafely("librarydarkleft")
                    .ProtectStructure()
                    .ApplyItemConfigurationsToAll(rand, genericLootPrimary, genericLootSecondary);
                }
                else
                {
                    PlaceStructureSafely("librarydarkright")
                        .ProtectStructure()
                        .ApplyItemConfigurationsToAll(rand, genericLootPrimary, genericLootSecondary);
                }
                if (rand.NextBool())
                {
                    PlaceStructureSafely("librarylightleft")
                    .ProtectStructure()
                    .ApplyItemConfigurationsToAll(rand, genericLootPrimary, genericLootSecondary);
                } 
                else
                {
                    PlaceStructureSafely("librarylightright")
                    .ProtectStructure()
                    .ApplyItemConfigurationsToAll(rand, genericLootPrimary, genericLootSecondary);
                }

                if (mainPass.WorldSizeScale > 1.2f) // Medium or large world, 1.2f instead of 1f so floating point math doesn't screw it up
                {
                    PlaceStructureSafely("librarydarkleft")
                        .ProtectStructure()
                        .ApplyItemConfigurationsToAll(rand, genericLootPrimary, genericLootSecondary);
                    PlaceStructureSafely("librarydarkright")
                        .ProtectStructure()
                        .ApplyItemConfigurationsToAll(rand, genericLootPrimary, genericLootSecondary);
                    PlaceStructureSafely("librarylightleft")
                        .ProtectStructure()
                        .ApplyItemConfigurationsToAll(rand, genericLootPrimary, genericLootSecondary);
                    PlaceStructureSafely("librarylightright")
                        .ProtectStructure()
                        .ApplyItemConfigurationsToAll(rand, genericLootPrimary, genericLootSecondary);
                }
                if (mainPass.WorldSizeScale > 1.7f) // Large world, otherwise same as last if statement
                {
                    PlaceStructureSafely("librarydarkleft")
                        .ProtectStructure()
                        .ApplyItemConfigurationsToAll(rand, genericLootPrimary, genericLootSecondary);
                    PlaceStructureSafely("librarydarkright")
                        .ProtectStructure()
                        .ApplyItemConfigurationsToAll(rand, genericLootPrimary, genericLootSecondary);
                    PlaceStructureSafely("librarylightleft")
                        .ProtectStructure()
                        .ApplyItemConfigurationsToAll(rand, genericLootPrimary, genericLootSecondary);
                    PlaceStructureSafely("librarylightright")
                        .ProtectStructure()
                        .ApplyItemConfigurationsToAll(rand, genericLootPrimary, genericLootSecondary);
                }

                PlaceRandomCaveHouses();

                /*const int TOTAL_SHRINES = 101;
                for (int i = 0; i < TOTAL_SHRINES; i++)
                {
                    progress.Set((float)i / TOTAL_SHRINES);
                    AeroStructure shrine = PlaceStructureSafely("smallshrine");

                    if (shrine != AeroStructure.Empty)
                    {
                        shrine.ProtectStructure();
                        shrine.ApplyItemConfigurationsToAll(rand, crystalShrinePrimary, crystalShrineSecondary);
                    }
                }*/
            }
            finally
            {
                WorldGen.noTileActions = oldNoTileActions;
            }
        }

        private void PlaceRandomCaveHouses()
        {
            if (_validPoints == null || _validPoints.Count == 0)
                return;

            int houseCount = WorldGen.genRand.Next(5, 11);
            for (int i = 0; i < houseCount; i++)
            {
                if (_validPoints.Count == 0) break;
                int pickIndex = WorldGen.genRand.Next(_validPoints.Count);
                Point chosen = _validPoints[pickIndex];
                _validPoints.RemoveAt(pickIndex);
                HouseGenerator.GenerateCaveHouse(chosen.X, chosen.Y, checkIfProtected: true);
            }
        }

        private void InitializeValidPoints(ShapeData bounds)
        {
            var mainPass = CCTerrainPass.Instance();
            var logger = ModContent.GetInstance<AerovelenceMod>()?.Logger;
            _validPoints = [];
            Rectangle boundRect = ShapeData.GetBounds(Point.Zero, bounds);
            //logger?.Info($"Shape bounds- X={boundRect.X}, Y={boundRect.Y}, Width={boundRect.Width}, Height={boundRect.Height}");
            int biomeCenterX = mainPass.Origin.X;
            int biomeTop = mainPass.Origin.Y;

            //logger?.Info($"Biome center- {biomeCenterX}, Top: {biomeTop}");
            const int SAMPLING_INTERVAL = 2;
            var rand = WorldGen.genRand;
            int minY = int.MaxValue;
            int maxY = int.MinValue;

            for (int localY = 0; localY < boundRect.Height; localY += SAMPLING_INTERVAL)
            {
                for (int localX = -boundRect.Width / 2; localX < boundRect.Width / 2; localX += SAMPLING_INTERVAL)
                {
                    int testX = localX;
                    int testY = boundRect.Y + localY;

                    if (bounds.Contains(testX, testY))
                    {
                        int worldY = biomeTop + localY;
                        minY = Math.Min(minY, worldY);
                        maxY = Math.Max(maxY, worldY);
                    }
                }
            }

            int heightRange = maxY - minY;
            for (int localY = 0; localY < boundRect.Height; localY += SAMPLING_INTERVAL)
            {
                for (int localX = -boundRect.Width / 2; localX < boundRect.Width / 2; localX += SAMPLING_INTERVAL)
                {
                    int testX = localX;
                    int testY = boundRect.Y + localY;

                    if (bounds.Contains(testX, testY))
                    {
                        int worldX = biomeCenterX + localX;
                        int worldY = biomeTop + localY;
                        float heightPosition = (float)(worldY - minY) / heightRange;
                        float probability = 0.5f + (float)Math.Sin(heightPosition * Math.PI) * 0.5f;

                        if (rand.NextFloat() < probability 
                            && (localX + boundRect.Width / 2 < boundRect.Width * (0.225 + mainPass.WorldSizeScale / (40.0f / 3f)) // .30 .375 .45
                            || localX + boundRect.Width / 2 > boundRect.Width * (0.775 - mainPass.WorldSizeScale / (40.0f / 3f)))) // .70 .625 .55
                        {
                            _validPoints.Add(new Point(worldX, worldY));

                            /*if (_validPoints.Count <= 5)
                            {
                                logger?.Info($"Valid point {_validPoints.Count}: Local({testX}, {testY}) -> World({worldX}, {worldY})");
                            }*/
                        }
                    }
                }
            }

            //logger?.Info($"Total valid points found- {_validPoints.Count}");
            if (_validPoints.Count > 0)
            {
                var xValues = _validPoints.Select(p => p.X).OrderBy(x => x).ToList();
                var leftPoints = _validPoints.Count(p => p.X < biomeCenterX);
                var rightPoints = _validPoints.Count(p => p.X >= biomeCenterX);

                //logger?.Info($"X-coordinate range- {xValues.First()} to {xValues.Last()}");
                //logger?.Info($"Points on left side- {leftPoints}");
                //logger?.Info($"Points on right side- {rightPoints}");
            }
        }

        private AeroStructure PlaceStructureSafely(string name, int attempts = 1000)
        {
            var logger = ModContent.GetInstance<AerovelenceMod>()?.Logger;

            if (_validPoints == null || _validPoints.Count == 0)
            {
                //logger?.Info("No valid points for structure placement");
                return AeroStructure.Empty;
            }

            AeroStructure sizeCheck = StructureStamper.LoadStructure(Vector2.Zero, name, placeStructure: false, checkIfProtected: false);
            if (sizeCheck == AeroStructure.Empty)
                return AeroStructure.Empty;
            int structureWidth = sizeCheck.Width;
            int structureHeight = sizeCheck.Height;
            HashSet<Point> triedPositions = [];
            var rand = WorldGen.genRand;

            for (int i = 0; i < attempts && triedPositions.Count < _validPoints.Count; i++)
            {
                Point randomPoint;
                do
                {
                    randomPoint = _validPoints[rand.Next(_validPoints.Count)];
                } while (triedPositions.Contains(randomPoint) && triedPositions.Count < _validPoints.Count);

                triedPositions.Add(randomPoint);
                Vector2 position = new( randomPoint.X - structureWidth / 2, randomPoint.Y - structureHeight / 2 );

                WorldGen.noTileActions = false;


                AeroStructure structure = StructureStamper.LoadStructure(
                    position,
                    name,
                    placeStructure: true,
                    checkIfProtected: true
                );

                if (structure != AeroStructure.Empty)
                {
                    //logger?.Info($"Successfully placed structure at ({position.X}, {position.Y}) on attempt {i + 1}");
                    return structure;
                }
            }

            //logger?.Info($"Failed to place structure after {attempts} attempts");
            return AeroStructure.Empty;
        }
    }
}