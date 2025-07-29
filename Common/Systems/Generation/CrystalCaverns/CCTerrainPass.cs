using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.WorldBuilding;
using Terraria.IO;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using System;
using AerovelenceMod.Content.Walls.CrystalCaverns.Natural;
using ReLogic.Utilities;
using System.Linq;
using AerovelenceMod.Content.Tiles.Citadel;
using AerovelenceMod.Common.Utilities.Generation;

namespace AerovelenceMod.Common.Systems.Generation.CrystalCaverns
{
    public sealed class CCTerrainPass : GenPass
    {
        public float WorldSizeScale { get; private set; }
        public int BiomeWidth { get; private set; }
        public int SurfaceHeight { get; private set; }
        public int UndergroundHeight { get; private set; }
        public int BiomeHeight { get; private set; }

        public ushort GrassTile { get; private set; }
        public ushort DirtTile { get; private set; }
        public ushort StoneTile { get; private set; }
        public ushort SandTile { get; private set; }
        public ushort CrystalTile { get; private set; }
        public ushort ChargedTile { get; private set; }
        public ushort BrickTile { get; private set; }
        public ushort LushTile { get; private set; }
        public ushort LivingWoodTile { get; private set; }
        public ushort LivingLeafTile { get; private set; }
        public ushort LivingWoodPlatformTile { get; private set; }
        public ushort LivingWoodDoorTile { get; private set; }

        public ushort DirtWall { get; private set; }
        public ushort StoneWall { get; private set; }
        public ushort BrickWall { get; private set; }
        public ushort LushWall { get; private set; }
        public ushort LivingWoodWall { get; private set; }
        public ushort LivingLeafWall { get; private set; }

        private ushort[] LivingWoodTiles { get; set; }
        private ushort[] ReplaceWithDirtTiles { get; set; }
        private ushort[] ReplaceWithStoneTiles { get; set; }
        private ushort[] ReplaceWithSandTiles { get; set; }
        private ushort[] ReplaceWithChargedTiles { get; set; }
        private ushort[] ReplaceWithBrickTiles { get; set; }
        private ushort[] ClearTiles {  get; set; }

        private ushort[] ReplaceWithBrickWalls { get; set; }

        public Point Origin { get; private set; }
        public Point TumblerTunnelEnd { get; private set; }
        public int TumblerArenaPolarity { get; private set; }
        public ShapeData LowerUnderground { get; private set; }
        public ShapeData UpperUnderground { get; private set; }
        public ShapeData TotalUnderground { get; private set; }

        private static CCTerrainPass _instance;
        private static readonly object _lock = new object();

        public static CCTerrainPass Instance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CCTerrainPass("Crystal Caverns Terrain", 100f);
                    }
                }
            }
            return _instance;
        }

        public static CCTerrainPass Instance(string name, float loadWeight)
        { 
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CCTerrainPass(name, loadWeight);
                    }
                }
            }
            return _instance;
        }

        private CCTerrainPass(string name, float loadWeight) : base(name, loadWeight)
		{
        }

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
            progress.Message = WorldGenSystem.CrystalCavernsTerrainPassMessage.Value;

            WorldSizeScale = Main.maxTilesY / 1200.0f;

            BiomeWidth = (int)(400 * WorldSizeScale);
            SurfaceHeight = (int)(100 * WorldSizeScale);
            UndergroundHeight = (int)(400 * WorldSizeScale);
            BiomeHeight = UndergroundHeight + SurfaceHeight;
            
            GrassTile = (ushort)ModContent.TileType<CrystalGrassTile>();
            DirtTile = (ushort)ModContent.TileType<CrystalDirtTile>();
            StoneTile = (ushort)ModContent.TileType<CavernStoneTile>();
            SandTile = (ushort)ModContent.TileType<CavernSandTile>();
            CrystalTile = (ushort)ModContent.TileType<CavernCrystalTile>();
            ChargedTile = (ushort)ModContent.TileType<ChargedStoneTile>();
            BrickTile = (ushort)ModContent.TileType<CitadelBrickTile>();
            LushTile = (ushort)ModContent.TileType<LushGrowthTile>();
            DirtWall = (ushort)ModContent.WallType<CavernDirtWallUnsafe>();
            StoneWall = (ushort)ModContent.WallType<CavernStoneWallUnsafe>();
            BrickWall = (ushort)ModContent.WallType<CitadelBrickWall>();
            LushWall = (ushort)ModContent.WallType<CrystalGrassWallUnsafe>();
            LivingWoodTile = TileID.LivingWood;
            LivingLeafTile = TileID.LeafBlock;
            LivingWoodWall = WallID.LivingWoodUnsafe;
            LivingLeafWall = WallID.LivingLeaf;
            LivingWoodPlatformTile = TileID.Platforms;
            LivingWoodDoorTile = TileID.ClosedDoor;

            ReplaceWithChargedTiles = [TileID.ClayBlock, TileID.Diamond, TileID.Ruby, TileID.Emerald, TileID.Sapphire, TileID.Topaz, TileID.Amethyst];
            ReplaceWithSandTiles = [TileID.Sand, TileID.Sandstone, TileID.Crimsand, TileID.Ebonsand, TileID.Silt, TileID.Slush];
            ReplaceWithStoneTiles = [TileID.Stone, TileID.Marble, TileID.Granite, TileID.HardenedSand, TileID.IceBlock, TileID.Ebonstone, TileID.Crimstone, TileID.Hive];
            ReplaceWithBrickTiles = [TileID.SandstoneBrick];
            ReplaceWithDirtTiles = [TileID.Dirt, TileID.DirtiestBlock, TileID.Mud, TileID.Grass, TileID.JungleGrass, TileID.MushroomGrass, TileID.CorruptGrass, TileID.CrimsonGrass, TileID.SnowBlock];

            ReplaceWithBrickWalls = [WallID.SandstoneBrick, BrickWall];

            LivingWoodTiles = [LivingWoodTile, LivingLeafTile, LivingWoodPlatformTile, LivingWoodDoorTile];

            Origin = DetermineOrigin(BiomeWidth, UndergroundHeight, SurfaceHeight, BiomeHeight); //center x, top of underground y
            if (!Origin.Equals(Point.Zero)) 
            {
                Origin = new Point(Origin.X, Origin.Y + SurfaceHeight);
                TumblerTunnelEnd = Point.Zero;
                TumblerArenaPolarity = 1;
                ShapeData surfaceRectShapeData = new ShapeData();
                ShapeData surfaceExposedShapeData = new ShapeData();
                ShapeData lightningBoltShapeData = new ShapeData();

                GenShape upperUndergroundShape = new Shapes.Rectangle(BiomeWidth, (int)(.5 * UndergroundHeight));
                GenShape lowerUndergroundShape = new Shapes.Mound(BiomeWidth / 2, (int)(.5 * UndergroundHeight));
                GenShape upperUndergroundWallShape = new Shapes.Rectangle(BiomeWidth - 1, (int)(.5 * UndergroundHeight - 1));
                GenShape lowerUndergroundWallShape = new Shapes.Mound(BiomeWidth / 2 - 1, (int)(.5 * UndergroundHeight));

                Point surfaceRectOrigin = new Point(Origin.X - BiomeWidth / 2, Origin.Y - (int)(SurfaceHeight * 1.75));
                Point upperUndergroundOrigin = new Point(Origin.X - BiomeWidth / 2, Origin.Y);
                Point lowerUndergroundOrigin = new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight));
                Point upperUndergroundWallOrigin = new Point(Origin.X - BiomeWidth / 2 + 1, Origin.Y);
                Point lowerUndergroundWallOrigin = new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight) - 1);

                // BIOME SURFACE
                WorldUtils.Gen(surfaceRectOrigin, new Shapes.Rectangle(BiomeWidth, (int)(SurfaceHeight * 1.75)), new Actions.Blank().Output(surfaceRectShapeData));

                void TileReplacement(ushort[] toBeReplaced, ushort replaceWith)
                {
                    WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                    {
                        new Modifiers.OnlyTiles(toBeReplaced),
                        new AeroGenUtils.SwapSolidTileInclusive(replaceWith)
                    }));
                }

                // Tile replacement
                TileReplacement(ReplaceWithChargedTiles, ChargedTile);
                TileReplacement(ReplaceWithSandTiles, SandTile);
                TileReplacement(ReplaceWithStoneTiles, StoneTile);
                TileReplacement(ReplaceWithBrickTiles, BrickTile);
                TileReplacement(ReplaceWithDirtTiles, DirtTile);

                void SurfaceDithering(ushort[] toBeReplaced, ushort replaceWith)
                {
                    WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Expand(3, 0),
                        new Modifiers.Dither(0.75),
                        new Modifiers.OnlyTiles(toBeReplaced),
                        new AeroGenUtils.SwapSolidTileInclusive(replaceWith)
                    }));
                    WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Expand(5, 0),
                        new Modifiers.Dither(0.75),
                        new Modifiers.OnlyTiles(toBeReplaced),
                        new AeroGenUtils.SwapSolidTileInclusive(replaceWith),
                    }));
                }

                SurfaceDithering(ReplaceWithChargedTiles, ChargedTile);
                SurfaceDithering(ReplaceWithSandTiles, SandTile);
                SurfaceDithering(ReplaceWithStoneTiles, StoneTile);
                SurfaceDithering(ReplaceWithBrickTiles, BrickTile);
                SurfaceDithering(ReplaceWithDirtTiles, DirtTile);

                // Grass
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 0),
                    new Modifiers.OnlyTiles(DirtTile),
                    new Modifiers.IsTouchingAir(true),
                    new AeroGenUtils.SwapSolidTileInclusive(GrassTile)
                }));

                // Walls
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyWalls(ReplaceWithBrickWalls),
                    new Actions.PlaceWall(BrickWall)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new AeroGenUtils.NotTouchingAir(true),
                    new Modifiers.SkipTiles(LivingLeafTile, LivingWoodTile, TileID.SmallPiles, TileID.LargePiles, TileID.LargePiles2),
                    new Modifiers.SkipWalls(ReplaceWithBrickWalls),
                    new AeroGenUtils.NotTouchingTiles(true, LivingLeafTile, LivingWoodTile, TileID.SmallPiles, TileID.LargePiles, TileID.LargePiles2),
                    new Actions.PlaceWall(DirtWall)
                }));
                // Catches the area below the vanilla dirt walls
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new AeroGenUtils.IsBelowSurface(5),
                    new Modifiers.SkipTiles(LivingLeafTile, LivingWoodTile, TileID.SmallPiles, TileID.LargePiles, TileID.LargePiles2),
                    new Modifiers.SkipWalls(ReplaceWithBrickWalls),
                    new AeroGenUtils.NotTouchingTiles(true, LivingLeafTile, LivingWoodTile, TileID.SmallPiles, TileID.LargePiles, TileID.LargePiles2),
                    new Actions.PlaceWall(DirtWall)
                }));

                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyWalls(WallID.DirtUnsafe, WallID.FlowerUnsafe, WallID.GrassUnsafe, 59, WallID.SnowWallUnsafe, WallID.Sandstone, WallID.HardenedSand, WallID.CrimstoneUnsafe, WallID.EbonstoneUnsafe),
                    new Actions.PlaceWall(DirtWall)
                }));

                // Surface to underground dithering
                void TransitionDithering(ushort[] toBeReplaced, ushort replaceWith)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 - 5, Origin.Y - (int)(SurfaceHeight * 0.1 * (i + 1))), new Shapes.Rectangle(BiomeWidth + 10, (int)(SurfaceHeight * 0.1 * (i + 1))), Actions.Chain(new GenAction[]
                        {
                            new Modifiers.Dither(0.1 + i * 0.3),
                            new Modifiers.OnlyTiles(toBeReplaced),
                            new AeroGenUtils.SwapSolidTileInclusive(replaceWith)
                        }));
                    }
                }

                //TransitionDithering([DirtTile], StoneTile);
                //TransitionDithering(ReplaceWithBrickTiles, BrickTile);

                // Surface to underground wall dithering
                void TransitionWallDithering(ushort[] toBeReplaced, ushort replaceWith)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 - 5, Origin.Y - (int)(SurfaceHeight * 0.1 * (i + 1))), new Shapes.Rectangle(BiomeWidth + 10, (int)(SurfaceHeight * 0.1 * (i + 1))), Actions.Chain(new GenAction[]
                        {
                            new Modifiers.Dither(0.1 + i * 0.3),
                            new Modifiers.OnlyWalls(toBeReplaced),
                            new Actions.PlaceWall(replaceWith)
                        }));
                    }
                }

                //TransitionWallDithering([DirtWall], StoneWall);
                //TransitionWallDithering(ReplaceWithBrickWalls, BrickWall);

                // BIOME UNDERGROUND
                void GenUpperUnderground(ushort[] toBeReplaced, ushort replaceWith)
                {
                    WorldUtils.Gen(upperUndergroundOrigin, upperUndergroundShape, Actions.Chain(new GenAction[]
                    {
                        new Modifiers.OnlyTiles(toBeReplaced),
                        new AeroGenUtils.SwapSolidTileInclusive(replaceWith)
                    }));
                    WorldUtils.Gen(upperUndergroundOrigin, upperUndergroundShape, Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Expand(3, 3),
                        new Modifiers.Dither(0.8),
                        new Modifiers.OnlyTiles(toBeReplaced),
                        new AeroGenUtils.SwapSolidTileInclusive(replaceWith)
                    }));
                    WorldUtils.Gen(upperUndergroundOrigin, upperUndergroundShape, Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Expand(5, 5),
                        new Modifiers.Dither(0.95),
                        new Modifiers.OnlyTiles(toBeReplaced),
                        new AeroGenUtils.SwapSolidTileInclusive(replaceWith)
                    }));
                }

                GenUpperUnderground(ReplaceWithStoneTiles, StoneTile);
                GenUpperUnderground(ReplaceWithSandTiles, SandTile);
                GenUpperUnderground(ReplaceWithChargedTiles, ChargedTile);
                GenUpperUnderground(ReplaceWithBrickTiles, BrickTile);
                GenUpperUnderground(ReplaceWithDirtTiles, DirtTile);

                // Lower underground

                void GenLowerUnderground(ushort[] toBeReplaced, ushort replaceWith)
                {
                    WorldUtils.Gen(lowerUndergroundOrigin, lowerUndergroundShape, Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Flip(false, true),
                        new Modifiers.SkipTiles(LivingWoodTiles),
                        new Modifiers.OnlyTiles(toBeReplaced),
                        new AeroGenUtils.SwapSolidTileInclusive(replaceWith)
                    }));
                    WorldUtils.Gen(lowerUndergroundOrigin, lowerUndergroundShape, Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Flip(false, true),
                        new Modifiers.Expand(3, 3),
                        new Modifiers.Dither(0.6),
                        new Modifiers.OnlyTiles(toBeReplaced),
                        new AeroGenUtils.SwapSolidTileInclusive(replaceWith)
                    }));
                    WorldUtils.Gen(lowerUndergroundOrigin, lowerUndergroundShape, Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Flip(false, true),
                        new Modifiers.Expand(5, 5),
                        new Modifiers.Dither(0.85),
                        new Modifiers.OnlyTiles(toBeReplaced),
                        new AeroGenUtils.SwapSolidTileInclusive(replaceWith)
                    }));
                }

                GenLowerUnderground(ReplaceWithStoneTiles, StoneTile);
                GenLowerUnderground(ReplaceWithSandTiles, SandTile);
                GenLowerUnderground(ReplaceWithChargedTiles, ChargedTile);
                GenLowerUnderground(ReplaceWithBrickTiles, BrickTile);
                GenLowerUnderground(ReplaceWithDirtTiles, DirtTile);

                // Upper underground walls

                void GenUpperUndergroundWalls(ushort[] targetWalls, ushort replaceWith, bool onlyWalls)
                {
                    WorldUtils.Gen(upperUndergroundWallOrigin, upperUndergroundWallShape, Actions.Chain(new GenAction[]
                    {
                        onlyWalls ? new Modifiers.OnlyWalls(targetWalls) : new Modifiers.SkipWalls(targetWalls),
                        new Actions.PlaceWall(replaceWith)
                    }));
                    WorldUtils.Gen(upperUndergroundWallOrigin, upperUndergroundWallShape, Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Expand(3, 3),
                        new Modifiers.Dither(0.85),
                        onlyWalls ? new Modifiers.OnlyWalls(targetWalls) : new Modifiers.SkipWalls(targetWalls),
                        new Actions.PlaceWall(replaceWith)
                    }));
                    WorldUtils.Gen(upperUndergroundWallOrigin, upperUndergroundWallShape, Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Expand(5, 5),
                        new Modifiers.Dither(0.95),
                        onlyWalls ? new Modifiers.OnlyWalls(targetWalls) : new Modifiers.SkipWalls(targetWalls),
                        new Actions.PlaceWall(replaceWith)
                    }));
                }

                GenUpperUndergroundWalls(ReplaceWithBrickWalls.Concat([LivingWoodWall, BrickWall]).ToArray(), StoneWall, false);
                GenUpperUndergroundWalls(ReplaceWithBrickWalls, BrickWall, true);

                // Lower underground walls

                void GenLowerUndergroundWalls(ushort[] targetWalls, ushort replaceWith, bool onlyWalls)
                {
                    WorldUtils.Gen(lowerUndergroundWallOrigin, lowerUndergroundWallShape, Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Flip(false, true),
                        onlyWalls ? new Modifiers.OnlyWalls(targetWalls) : new Modifiers.SkipWalls(targetWalls),
                        new Actions.PlaceWall(replaceWith)
                    }));
                    WorldUtils.Gen(lowerUndergroundWallOrigin, lowerUndergroundWallShape, Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Flip(false, true),
                        new Modifiers.Expand(3, 3),
                        new Modifiers.Dither(0.6),
                        onlyWalls ? new Modifiers.OnlyWalls(targetWalls) : new Modifiers.SkipWalls(targetWalls),
                        new Actions.PlaceWall(replaceWith)
                    }));
                    WorldUtils.Gen(lowerUndergroundWallOrigin, lowerUndergroundWallShape, Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Flip(false, true),
                        new Modifiers.Expand(5, 5),
                        new Modifiers.Dither(0.85),
                        onlyWalls ? new Modifiers.OnlyWalls(targetWalls) : new Modifiers.SkipWalls(targetWalls),
                        new Actions.PlaceWall(replaceWith)
                    }));
                }

                GenLowerUndergroundWalls(ReplaceWithBrickWalls, StoneWall, false);
                GenLowerUndergroundWalls(ReplaceWithBrickWalls, BrickWall, true);

                // Clear a lot of the walls to allow the background to be visible frequently
                WorldUtils.Gen(upperUndergroundWallOrigin, upperUndergroundWallShape, Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 5),
                    new Modifiers.Dither(0.999975),
                    new Modifiers.IsNotSolid(),
                    new Modifiers.SkipWalls(BrickWall),
                    new AeroGenUtils.IsBelowSurface(-25),
                    new AeroGenUtils.ClearWallRunner(),
                    new AeroGenUtils.ClearWallRunner(),
                    new AeroGenUtils.ClearWallRunner()
                }));
                WorldUtils.Gen(lowerUndergroundWallOrigin, lowerUndergroundWallShape, Actions.Chain(new GenAction[]
                {
                    new Modifiers.Flip(false, true),
                    new Modifiers.Expand(5, 5),
                    new Modifiers.Dither(0.999975),
                    new Modifiers.IsNotSolid(),
                    new Modifiers.SkipWalls(BrickWall),
                    new AeroGenUtils.IsBelowSurface(-25),
                    new AeroGenUtils.ClearWallRunner(),
                    new AeroGenUtils.ClearWallRunner(),
                    new AeroGenUtils.ClearWallRunner()
                }));

                // Lush growths
                ShapeData lushBiomeUpperOrigins = new ShapeData();
                ShapeData lushBiomeLowerOrigins = new ShapeData();
                WorldUtils.Gen(upperUndergroundOrigin, upperUndergroundShape, Actions.Chain(new GenAction[]
                {
                    new Modifiers.Dither(0.9998),
                    new Modifiers.OnlyTiles(StoneTile, DirtTile, GrassTile, SandTile, ChargedTile),
                    new Actions.Blank().Output(lushBiomeUpperOrigins)
                }));
                WorldUtils.Gen(lowerUndergroundOrigin, lowerUndergroundShape, Actions.Chain(new GenAction[]
                {
                    new Modifiers.Flip(false, true),
                    new Modifiers.Dither(0.9998),
                    new Modifiers.OnlyTiles(StoneTile, DirtTile, GrassTile, SandTile, ChargedTile),
                    new Actions.Blank().Output(lushBiomeLowerOrigins)
                }));
                for (int i = 0; i < WorldGen.genRand.Next(100); i++)
                {
                    WorldUtils.Gen(upperUndergroundOrigin, new ModShapes.All(lushBiomeUpperOrigins), Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Offset(WorldGen.genRand.Next(-10, 11), WorldGen.genRand.Next(-10, 11)),
                        new AeroGenUtils.PlaceBlob(LushTile, (int)(7f * WorldSizeScale), (int)(7f * WorldSizeScale), [new Modifiers.IsTouchingAir(true), new Modifiers.OnlyTiles(StoneTile, DirtTile, GrassTile, SandTile, ChargedTile)]),
                        new AeroGenUtils.PlaceBlobWall(LushWall, (int)(7f * WorldSizeScale), (int)(7f * WorldSizeScale), [new Modifiers.IsTouchingAir(true), new Modifiers.OnlyWalls(StoneWall)]),
                    }));
                    WorldUtils.Gen(lowerUndergroundOrigin, new ModShapes.All(lushBiomeLowerOrigins), Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Offset(WorldGen.genRand.Next(-10, 11), WorldGen.genRand.Next(-10, 11)),
                        new AeroGenUtils.PlaceBlob(LushTile, (int)(7f * WorldSizeScale), (int)(7f * WorldSizeScale), [new Modifiers.IsTouchingAir(true), new Modifiers.OnlyTiles(StoneTile, DirtTile, GrassTile, SandTile, ChargedTile)]),
                        new AeroGenUtils.PlaceBlobWall(LushWall, (int)(7f * WorldSizeScale), (int)(7f * WorldSizeScale), [new Modifiers.IsTouchingAir(true), new Modifiers.OnlyWalls(StoneWall)]),
                    }));
                }

                // Main lightning bolt cave
                WorldUtils.Gen(new Point(Origin.X, Origin.Y - (int)(SurfaceHeight * 1.75)), new AeroGenUtils.LightningBoltShape((int)(550 * WorldSizeScale), 50 * (int)((WorldSizeScale - 1) * 0.8 + 1), 2, 30), Actions.Chain(new GenAction[]
                {
                    new Modifiers.SkipTiles(CrystalTile, TileID.LeafBlock, TileID.LivingWood),
                    new Actions.ClearTile().Output(lightningBoltShapeData),
                    // Don't ask me why but chaining a chain seems to allow layers in this case but only one layer
                    Actions.Chain(new GenAction[]
                    {
                        new Modifiers.Expand(3, 0),
                        new Modifiers.OnlyTiles(DirtTile, StoneTile),
                        new Modifiers.IsTouchingAir(true),
                        new Modifiers.RectangleMask(-BiomeWidth / 2, BiomeWidth / 2, 5, (int)(SurfaceHeight * 4)), // SurfaceHeight * 1.75 was what I started it at
                        new Modifiers.Blotches(4, 4, 1),
                        new Modifiers.NotInShape(lightningBoltShapeData),
                        new AeroGenUtils.IsTouchingWall(true, DirtWall, StoneWall),
                        new Actions.SetTileKeepWall(StoneTile),
                    })
                }));

                // Surface object generation
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(GrassTile, DirtTile, SandTile),
                    new Modifiers.IsTouchingAir(true),
                    new AeroGenUtils.SolidBelow(10),
                    new AeroGenUtils.NotSolidAbove(50),
                    new Actions.Blank().Output(surfaceExposedShapeData),
                }));

                // Surface rock blobs
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Offset(0, 2),
                    new Modifiers.Dither(.9875), // 1/80 chance
                    new Modifiers.IsTouchingAir(),
                    new AeroGenUtils.NotTouchingTiles(true, LivingWoodTiles),
                    new Modifiers.OnlyTiles(GrassTile, DirtTile, SandTile, StoneTile),
                    new AeroGenUtils.PlaceBlob(StoneTile, 5.5f, 5.5f, 1.5f, 1.5f, []),
                }));

                // Surface crystal growths
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceExposedShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Offset(0, 2),
                    new Modifiers.Dither(.985), // 1/66.66 chance
                    new Modifiers.OnlyTiles(GrassTile, DirtTile, SandTile, StoneTile),
                    new AeroGenUtils.PlaceTail(CrystalTile, 6, new Vector2D(0, -20), 0, 4, 3)
                }));

                // Tumbler arena prep
                TumblerArenaPolarity = WorldGen.genRand.NextBool().ToDirectionInt();

                TumblerTunnelEnd = WorldGen.digTunnel(Origin.X, Origin.Y + UndergroundHeight / 2, 3 * TumblerArenaPolarity, 0, (int)(65 * WorldSizeScale), 5).ToPoint();
                WorldGen.digTunnel(Origin.X, Origin.Y + UndergroundHeight / 2, -3 * TumblerArenaPolarity, 0, (int)(65 * WorldSizeScale), 5);

                // Underground crystal growths
                WorldUtils.Gen(upperUndergroundOrigin, upperUndergroundShape, Actions.Chain(new GenAction[]
                {
                    new AeroGenUtils.NotSolidAbove(20),
                    new Modifiers.Offset(0, 2),
                    new Modifiers.Dither(.985), // 1/66.66 chance
                    new Modifiers.OnlyTiles(StoneTile, ChargedTile),
                    new AeroGenUtils.PlaceTail(CrystalTile, 4, new Vector2D(0, -10), 0, 4, 3)
                }));
                WorldUtils.Gen(upperUndergroundOrigin, upperUndergroundShape, Actions.Chain(new GenAction[]
                {
                    new AeroGenUtils.NotSolidBelow(20),
                    new Modifiers.Offset(0, -2),
                    new Modifiers.Dither(.985), // 1/66.66 chance
                    new Modifiers.OnlyTiles(StoneTile, ChargedTile),
                    new AeroGenUtils.PlaceTail(CrystalTile, 4, new Vector2D(0, 10), 0, 4, 3)
                }));
                WorldUtils.Gen(lowerUndergroundOrigin, lowerUndergroundShape, Actions.Chain(new GenAction[]
                {
                    new Modifiers.Flip(false, true),
                    new AeroGenUtils.NotSolidAbove(20),
                    new Modifiers.Offset(0, 2),
                    new Modifiers.Dither(.985), // 1/66.66 chance
                    new Modifiers.OnlyTiles(StoneTile, ChargedTile),
                    new AeroGenUtils.PlaceTail(CrystalTile, 4, new Vector2D(0, -10), 0, 4, 3)
                }));
                WorldUtils.Gen(lowerUndergroundOrigin, lowerUndergroundShape, Actions.Chain(new GenAction[]
                {
                    new Modifiers.Flip(false, true),
                    new AeroGenUtils.NotSolidBelow(20),
                    new Modifiers.Offset(0, -2),
                    new Modifiers.Dither(.985), // 1/66.66 chance
                    new Modifiers.OnlyTiles(StoneTile, ChargedTile),
                    new AeroGenUtils.PlaceTail(CrystalTile, 4, new Vector2D(0, 10), 0, 4, 3)
                }));


                ShapeData upperUndergroundShapeData = new ShapeData();
                ShapeData lowerUndergroundShapeData = new ShapeData();
                ShapeData totalUndergroundShapeData = new ShapeData();
                // Must use same origin or total underground shape will not build correctly
                WorldUtils.Gen(
                    //new Point(Origin.X - BiomeWidth, Origin.Y),
                    Origin,
                    upperUndergroundShape,
                    Actions.Chain(
                        new GenAction[]
                        {
                            new Modifiers.Offset(-BiomeWidth / 2, 0),
                            new Actions.Blank().Output(upperUndergroundShapeData),
                            new Actions.Blank().Output(totalUndergroundShapeData),
                        }
                    )
                );
                WorldUtils.Gen(
                    //new Point(Origin.X - BiomeWidth / 2, Origin.Y + (int)(.5 * UndergroundHeight)),
                    Origin,
                    lowerUndergroundShape,
                    Actions.Chain(
                        new GenAction[]
                        {
                            new Modifiers.Offset(0, -(int)(0.5 * UndergroundHeight)), // Positive Y shifts upwards because of Modifiers.Flip()
                            new Modifiers.Flip(false, true),
                            new Actions.Blank().Output(lowerUndergroundShapeData),
                            new Actions.Blank().Output(totalUndergroundShapeData),
                        }
                    )
                );
                UpperUnderground = upperUndergroundShapeData;
                LowerUnderground = lowerUndergroundShapeData;
                TotalUnderground = totalUndergroundShapeData;
            }
		}

		private Point DetermineOrigin(int biomeWidth, int undergroundHeight, int surfaceHeight, int biomeHeight)
        {
            Point fallbackPoint = Point.Zero;
            Point evilFallbackPoint = Point.Zero;
            // Using Point.Zero as a standin for a 'null' value, aka no valid spawn location found and the biome will not generate
            Point surfacePoint = Point.Zero;

            for (int attempts = 0; attempts < 10000; attempts++)
			{
                int x = WorldGen.genRand.Next((int)(500 * WorldSizeScale), Main.maxTilesX - (int)(500 * WorldSizeScale));
				while (Main.maxTilesX * .4 < x && x < Main.maxTilesX * .6)
				{
					x = WorldGen.genRand.Next((int)(500 * WorldSizeScale), Main.maxTilesX - (int)(500 * WorldSizeScale));
				}

                Point initialPoint = new Point(x, (int)Main.worldSurface);

				bool flag = WorldUtils.Find(initialPoint, Searches.Chain(new Searches.Up((int)(200 * WorldSizeScale)), new Conditions.IsTile(TileID.LivingWood, TileID.LeafBlock).AreaOr(1, 50)), out Point _);
				if (flag)
					continue;
                flag = WorldUtils.Find(initialPoint, Searches.Chain(new Searches.Up(1000), new Conditions.IsSolid().AreaOr(1, 50).Not()), out surfacePoint);
                // Adjust result to point to surface, not 50 tiles above 
                surfacePoint.Y += 50;

                // Search up to 1000 tiles above for an area 50 tiles tall and 1 tile wide without a single solid tile. Basically find the surface.
                if (!flag)
                    continue;

                // Check on the left bound, mid-left side, center line, mid-right side, and right bound for suboptimal but sometimes acceptable results
                if (!CheckPointEvilFallback(-(int)(.5 * biomeWidth), surfacePoint) ||
                    !CheckPointEvilFallback(-(int)(.25 * biomeWidth), surfacePoint) ||
                    !CheckPointEvilFallback(0, surfacePoint) ||
                    !CheckPointEvilFallback((int)(.25 * biomeWidth), surfacePoint) ||
                    !CheckPointEvilFallback((int)(.5 * biomeWidth), surfacePoint))
                    continue;
                evilFallbackPoint = surfacePoint;
                // Check on the left bound, mid-left side, center line, mid-right side, and right bound for suboptimal but acceptable results
                if (!CheckPointFallback(-(int)(.5 * biomeWidth), surfacePoint) ||
                    !CheckPointFallback(-(int)(.25 * biomeWidth), surfacePoint) ||
                    !CheckPointFallback(0, surfacePoint) ||
                    !CheckPointFallback((int)(.25 * biomeWidth), surfacePoint) ||
                    !CheckPointFallback((int)(.5 * biomeWidth), surfacePoint))
                    continue;
                fallbackPoint = surfacePoint;
                // Check on the left side, mid-left side, center line, mid-right side, and right side of the biome
                if (!CheckPoint(-(int)(.5 * biomeWidth), surfacePoint) ||
                    !CheckPoint(-(int)(.25 * biomeWidth), surfacePoint) ||
                    !CheckPoint(0, surfacePoint) ||
                    !CheckPoint((int)(.25 * biomeWidth), surfacePoint) ||
                    !CheckPoint((int)(.5 * biomeWidth), surfacePoint))
					continue;

                Console.WriteLine("Crystal Caverns generation process finished in " + attempts + " attempts.");
				surfacePoint.Y = DetermineOriginY(biomeWidth, surfacePoint); // Correct the Y position of the biome to the average of the right and left bound's surrounding terrain height
                GenVars.structures.AddProtectedStructure(new Rectangle(surfacePoint.X - (int)(.5 * biomeWidth), surfacePoint.Y, biomeWidth, biomeHeight), 0);
                return surfacePoint;
            }
            Console.WriteLine("Could not find a suitable location to place the Crystal Caverns");
			if (fallbackPoint != Point.Zero)
			{
				Console.WriteLine("Falling back to a location overlapping with evil to generate the Crystal Caverns");
                surfacePoint.Y = DetermineOriginY(biomeWidth, surfacePoint); // Correct the Y position of the biome to the average of the right and left bound's surrounding terrain height
                GenVars.structures.AddProtectedStructure(new Rectangle(surfacePoint.X - (int)(.5 * biomeWidth), surfacePoint.Y, biomeWidth, biomeHeight), 0);
                return fallbackPoint;
            }
            if (evilFallbackPoint != Point.Zero)
            {
                Console.WriteLine("Falling back to a location overlapping with evil / jungle / ice to generate the Crystal Caverns");
                surfacePoint.Y = DetermineOriginY(biomeWidth, surfacePoint); // Correct the Y position of the biome to the average of the right and left bound's surrounding terrain height
                GenVars.structures.AddProtectedStructure(new Rectangle(surfacePoint.X - (int)(.5 * biomeWidth), surfacePoint.Y, biomeWidth, biomeHeight), 0);
            }
            return evilFallbackPoint;
        }

		private bool CheckPoint(int xOffset, Point surfacePoint) 
		{
			Point point = new Point(surfacePoint.X + xOffset, surfacePoint.Y);
			//surfacePoint argument means only the central point is taken into consideration, while point means all three are
			if (WorldUtils.Find(point, Searches.Chain(new Searches.Down(UndergroundHeight + SurfaceHeight), new Conditions.IsTile(
				TileID.JungleGrass,
				TileID.IceBlock,
                TileID.SnowBlock)), out Point _))
				return false;
            if (WorldUtils.Find(point, Searches.Chain(new Searches.Down(UndergroundHeight + SurfaceHeight), new Conditions.IsTile(
                TileID.Sandstone,
                TileID.BlueDungeonBrick,
                TileID.GreenDungeonBrick,
                TileID.PinkDungeonBrick,
                TileID.LihzahrdBrick)), out Point _))
                return false;
            if (WorldUtils.Find(point, Searches.Chain(new Searches.Down(UndergroundHeight + SurfaceHeight), new Conditions.IsTile(
                TileID.Crimstone,
                TileID.Ebonstone,
                TileID.Crimsand,
                TileID.Ebonsand,
                TileID.CorruptGrass,
                TileID.CrimsonGrass)), out Point _))
                return false;
            if (WorldUtils.Find(point, Searches.Chain(new Searches.Down(UndergroundHeight + SurfaceHeight + 100), new AeroGenUtils.HasShimmer()), out Point _))
                return false;            
            return true;
        }

		private bool CheckPointFallback(int xOffset, Point surfacePoint)
		{
            Point point = new Point(surfacePoint.X + xOffset, surfacePoint.Y);
            if (WorldUtils.Find(point, Searches.Chain(new Searches.Down(UndergroundHeight + SurfaceHeight), new Conditions.IsTile(
                TileID.JungleGrass,
                TileID.IceBlock)), out Point _))
                return false;
            if (WorldUtils.Find(point, Searches.Chain(new Searches.Down(UndergroundHeight + SurfaceHeight), new Conditions.IsTile(
                TileID.Sandstone,
                TileID.BlueDungeonBrick,
                TileID.GreenDungeonBrick,
                TileID.PinkDungeonBrick,
                TileID.LihzahrdBrick)), out Point _))
                return false;
            if (WorldUtils.Find(point, Searches.Chain(new Searches.Down(UndergroundHeight + SurfaceHeight + 100), new AeroGenUtils.HasShimmer()), out Point _))
                return false;
            return true;
        }

        private bool CheckPointEvilFallback(int xOffset, Point surfacePoint)
        {
            Point point = new Point(surfacePoint.X + xOffset, surfacePoint.Y);
            if (WorldUtils.Find(point, Searches.Chain(new Searches.Down(UndergroundHeight + SurfaceHeight), new Conditions.IsTile(
                TileID.Sandstone,
                TileID.BlueDungeonBrick,
                TileID.GreenDungeonBrick,
                TileID.PinkDungeonBrick,
                TileID.LihzahrdBrick)), out Point _))
                return false;
            return true;
        }

        private int DetermineOriginY(int biomeWidth, Point surfacePoint)
		{
			int xOffset = (int)(.5 * biomeWidth);
            Point leftPoint = new Point(surfacePoint.X - xOffset, (int)Main.worldSurface);
            Point rightPoint = new Point(surfacePoint.X + xOffset, (int)Main.worldSurface);
            for (int attempts = -4; attempts < 6; attempts += 2) // This for loop is meant to solve corruption chasms dragging the average very far down
			{
                WorldUtils.Find(leftPoint, Searches.Chain(new Searches.Up(1000), new Conditions.IsSolid().AreaOr(1, 25).Not()), out leftPoint);
                leftPoint.Y += 25; // Adjust result to point to surface, not 50 tiles above 

                WorldUtils.Find(rightPoint, Searches.Chain(new Searches.Up(1000), new Conditions.IsSolid().AreaOr(1, 25).Not()), out rightPoint);
                rightPoint.Y += 25; // Adjust result to point to surface, not 50 tiles above 

                leftPoint = new Point(surfacePoint.X - xOffset + attempts, leftPoint.Y);
                rightPoint = new Point(surfacePoint.X + xOffset + attempts, rightPoint.Y);
            }
            return (leftPoint.Y + rightPoint.Y) / 2;
        }
    }
}