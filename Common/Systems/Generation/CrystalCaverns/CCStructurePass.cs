using AerovelenceMod.Common.Globals.Worlds;
using AerovelenceMod.Common.Utilities.Generation.StructureStamper;
using AerovelenceMod.Content.Items.Weapons.Aurora.Eos;
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

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            bool oldNoTileActions = WorldGen.noTileActions;
            WorldGen.noTileActions = true;
            try
            {
                progress.Message = WorldGenSystem.CrystalCavernsTerrainPassMessage.Value;
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
                List<(Vector2 Position, List<PrimaryItemConfiguration> Primary, List<ItemConfiguration> Secondary)> shrinesToProcess = new();
                List<PrimaryItemConfiguration> crystalShrinePrimary = new()
                {
                    new(ItemID.BandofRegeneration, 1, 1, 1f),
                    new(ItemID.MagicMirror, 1, 1, 1f),
                    new(ItemID.CloudinaBottle, 1, 1, 1f),
                    new(ItemID.HermesBoots, 1, 1, 1f),
                    new(ItemID.EnchantedBoomerang, 1, 1, 1f),
                    new(ItemID.ShoeSpikes, 1, 1, 1f),
                    new(ItemID.FlareGun, 1, 1, 1f),
                    new(ItemID.Extractinator, 1, 1, 1f),
                    new(ItemID.LavaCharm, 1, 1, 1f),
                    new(ItemID.LuckyHorseshoe, 1, 1, 1f),
                    new(ModContent.ItemType<Eos>(), 1, 1, 1f)
                };

                List<ItemConfiguration> crystalShrineSecondary = new()
                {
                    new(ItemID.SuspiciousLookingEye, 1, 1),
                    new(ItemID.Dynamite, 1, 1),
                    new(ItemID.JestersArrow, 25, 50),
                    new(new List<int> { ItemID.SilverBar, ItemID.TungstenBar, ItemID.GoldBar, ItemID.PlatinumBar }, 3, 10),
                    new(new List<int> { ItemID.FlamingArrow, ItemID.ThrowingKnife }, 25, 50),
                    new(ItemID.HealingPotion, 3, 5),
                    new(new List<int>
                    {
                        ItemID.SpelunkerPotion, ItemID.FeatherfallPotion, ItemID.NightOwlPotion, ItemID.WaterWalkingPotion,
                        ItemID.ArcheryPotion, ItemID.GravitationPotion, ItemID.ThornsPotion, ItemID.InvisibilityPotion,
                        ItemID.HunterPotion, ItemID.BattlePotion, ItemID.TeleportationPotion
                    }, 1, 2),
                    new(ItemID.RecallPotion, 1, 2),
                    new(new List<int> { ItemID.Torch, ItemID.Glowstick }, 15, 29),
                    new(ItemID.GoldCoin, 1, 2)
                };
                AeroStructure tumblerArena = StructureStamper.LoadStructure(
                    new Vector2(
                        mainPass.TumblerTunnelEnd.X - 60 + 60 * mainPass.TumblerArenaPolarity,
                        mainPass.TumblerTunnelEnd.Y - 46
                    ),
                    "tumblerarena"
                ).ProtectStructure();
                AeroStructure crystalShrine = PlaceStructureSafely("smallshrine", 20, 20)
                    .ProtectStructure()
                    .ApplyItemConfigurationsToAll(rand, crystalShrinePrimary, crystalShrineSecondary);
                AeroStructure crystalShrine2 = PlaceStructureSafely("smallshrine", 20, 20)
                    .ProtectStructure()
                    .ApplyItemConfigurationsToAll(rand, crystalShrinePrimary, crystalShrineSecondary);


                const int TOTAL_SHRINES = 101;

                for (int i = 0; i < TOTAL_SHRINES; i++)
                {
                    progress.Set((float)i / TOTAL_SHRINES);

                    AeroStructure shrine = PlaceStructureSafely("smallshrine", 20, 20);

                    if (shrine != AeroStructure.Empty)
                    {
                        shrine.ProtectStructure();
                        shrine.ApplyItemConfigurationsToAll(rand, crystalShrinePrimary, crystalShrineSecondary);
                    }
                }
            }
            finally
            {
                WorldGen.noTileActions = oldNoTileActions;
            }
        }

        private AeroStructure PlaceStructureSafely(string name, int xMarginPercentage = 10, int yMarginPercentage = 10, int attempts = 5000)
        {
            for (int i = 0; i < attempts; i++)
            {
                Vector2 randVector = GetRandomUndergroundVector(xMarginPercentage, yMarginPercentage);
                AeroStructure checkStructure = StructureStamper.LoadStructure(
                    randVector,
                    name,
                    placeStructure: false,
                    checkIfProtected: true
                );

                if (checkStructure != AeroStructure.Empty)
                {
                    randVector = new Vector2(
                        randVector.X - checkStructure.Width / 2,
                        randVector.Y - checkStructure.Height / 2
                    );
                    AeroStructure structure = StructureStamper.LoadStructure(
                        randVector,
                        name,
                        placeStructure: true,
                        checkIfProtected: true
                    );

                    if (structure != AeroStructure.Empty)
                    {
                        return structure;
                    }
                }
            }
            return AeroStructure.Empty;
        }

        private Vector2 GetRandomUndergroundVector(int xMarginPercentage = 10, int yMarginPercentage = 10)
        {
            CCTerrainPass mainPass = CCTerrainPass.Instance();
            UnifiedRandom rand = WorldGen.genRand;
            int x = mainPass.Origin.X - mainPass.BiomeWidth / 2 + (int)(rand.NextFloat(xMarginPercentage * 0.01f, 1 - xMarginPercentage * 0.01f) * mainPass.BiomeWidth);
            int y = mainPass.Origin.Y - (int)(mainPass.SurfaceHeight * 0.75) + (int)(rand.NextFloat(yMarginPercentage * 0.01f, 1 - yMarginPercentage * 0.01f) * mainPass.BiomeHeight);
            return new Vector2(x, y);
        }
    }
}
