using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.WorldBuilding;
using Terraria.IO;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using System;
using AerovelenceMod.Common.Systems.Generation.GenUtils;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Building;
using AerovelenceMod.Content.Walls.CrystalCaverns.Natural;
using Terraria.Graphics.Shaders;
using Terraria.GameContent.Generation;
using ReLogic.Utilities;
using System.Security.Cryptography.X509Certificates;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items;
using Terraria.ModLoader.IO;
using Terraria.Enums;
using System.Collections.Generic;
using System.Linq;
using AerovelenceMod.Content.Tiles.Citadel;

namespace AerovelenceMod.Common.Systems.Generation.CrystalCaverns
{
    public sealed class CrystalCavernsTerrainPass : GenPass
    {
        public int WorldSizeScale { get; private set; }
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
        public ushort LivingWoodTile { get; private set; }
        public ushort LivingLeafTile { get; private set; }
        public ushort LivingWoodPlatformTile { get; private set; }
        public ushort LivingWoodDoorTile { get; private set; }

        public ushort DirtWall { get; private set; }
        public ushort StoneWall { get; private set; }
        public ushort BrickWall { get; private set; }
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

        private static CrystalCavernsTerrainPass _instance;
        private static readonly object _lock = new object();

        public static CrystalCavernsTerrainPass Instance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CrystalCavernsTerrainPass("Crystal Caverns Terrain", 100f);
                    }
                }
            }
            return _instance;
        }

        public static CrystalCavernsTerrainPass Instance(string name, float loadWeight)
        { 
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CrystalCavernsTerrainPass(name, loadWeight);
                    }
                }
            }
            return _instance;
        }

        private CrystalCavernsTerrainPass(string name, float loadWeight) : base(name, loadWeight)
		{
        }

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
            progress.Message = WorldGenSystem.CrystalCavernsTerrainPassMessage.Value;

            _instance.WorldSizeScale = Main.maxTilesY / 1200;

            _instance.BiomeWidth = 400 * _instance.WorldSizeScale;
            _instance.SurfaceHeight = 100 * _instance.WorldSizeScale;
            _instance.UndergroundHeight = 400 * _instance.WorldSizeScale;
            _instance.BiomeHeight = _instance.UndergroundHeight + _instance.SurfaceHeight;
            
            GrassTile = (ushort)ModContent.TileType<CrystalGrassTile>();
            // DirtTile = TileID.Granite; 
            DirtTile = (ushort)ModContent.TileType<CrystalDirtTile>();
            StoneTile = (ushort)ModContent.TileType<CavernStoneTile>();
            SandTile = (ushort)ModContent.TileType<CavernSandTile>();
            CrystalTile = (ushort)ModContent.TileType<CavernCrystalTile>();
            ChargedTile = (ushort)ModContent.TileType<ChargedStoneTile>();
            BrickTile = (ushort)ModContent.TileType<CitadelBrickTile>();
            // DirtWall = WallID.Granite; 
            DirtWall = (ushort)ModContent.WallType<CavernDirtWall>();
            StoneWall = (ushort)ModContent.WallType<CavernStoneWall>();
            BrickWall = (ushort)ModContent.WallType<CitadelBrickWall>();
            /*ushort LivingWoodTile = (ushort)ModContent.TileType<FreshGlimmerwood>();
            ushort LivingLeafTile = (ushort)ModContent.TileType<ChargedStone>();
            ushort LivingWoodWall = (ushort)ModContent.WallType<GlimmerwoodWall>();
            ushort LivingLeafWall = (ushort)ModContent.WallType<CavernStoneWall>();
            ushort LivingWoodPlatformTile = (ushort)ModContent.TileType<GlimmerwoodPlatform>();
            ushort LivingWoodDoorTile = TileID.ClosedDoor;*/
            LivingWoodTile = TileID.LivingWood;
            LivingLeafTile = TileID.LeafBlock;
            LivingWoodWall = WallID.LivingWoodUnsafe;
            LivingLeafWall = WallID.LivingLeaf;
            LivingWoodPlatformTile = TileID.Platforms;
            LivingWoodDoorTile = TileID.ClosedDoor;

            ReplaceWithChargedTiles = [TileID.ClayBlock, TileID.Diamond, TileID.Ruby, TileID.Emerald, TileID.Sapphire, TileID.Topaz, TileID.Amethyst];
            ReplaceWithSandTiles = [SandTile, TileID.Sand, TileID.Sandstone, TileID.Crimsand, TileID.Ebonsand, TileID.Silt, TileID.Slush];
            ReplaceWithStoneTiles = [TileID.Stone, TileID.Dirt, TileID.Grass, TileID.Mud, TileID.JungleGrass, TileID.MushroomGrass, TileID.Marble, TileID.Granite, TileID.HardenedSand, TileID.IceBlock, TileID.SnowBlock, TileID.Ebonstone, TileID.Crimstone, TileID.CorruptGrass, TileID.CrimsonGrass, TileID.Hive];
            ReplaceWithBrickTiles = [TileID.SandstoneBrick];
            ReplaceWithDirtTiles = [TileID.Silt, StoneTile, SandTile, ChargedTile, CrystalTile, TileID.Copper, TileID.Tin, TileID.Iron, TileID.Lead, TileID.Silver, TileID.Tungsten, TileID.Gold, TileID.Platinum, TileID.Demonite, TileID.Crimtane];
            ClearTiles = [/*TileID.Larva*/];

            ReplaceWithBrickWalls = [WallID.SandstoneBrick];

            LivingWoodTiles = [LivingWoodTile, LivingLeafTile, LivingWoodPlatformTile, LivingWoodDoorTile];
            // Terrible array I need to fix but probably won't

            Origin = determineOrigin(BiomeWidth, UndergroundHeight, SurfaceHeight, BiomeHeight); //center x, top of underground y
            if (!Origin.Equals(Point.Zero)) 
            {
                Origin = new Point(Origin.X, Origin.Y + (int)(SurfaceHeight));
                TumblerTunnelEnd = Point.Zero;
                TumblerArenaPolarity = 1;
                // BIOME SURFACE
                ShapeData surfaceRectShapeData = new ShapeData();
                ShapeData surfaceExposedShapeData = new ShapeData();
                ShapeData lightningBoltShapeData = new ShapeData();
                Point surfaceRectOrigin = new Point(Origin.X - BiomeWidth / 2, Origin.Y - (int)(SurfaceHeight * 1.75));

            // BIOME SURFACE
                WorldUtils.Gen(surfaceRectOrigin, new Shapes.Rectangle(BiomeWidth, (int)(SurfaceHeight * 1.75)), new Actions.Blank().Output(surfaceRectShapeData));

                // Living tree shenanigans
                /*WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(TileID.LivingWood),
                    new Actions.SetTileKeepWall(LivingWoodTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(TileID.LeafBlock),
                    new Actions.SetTileKeepWall(LivingLeafTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyWalls(WallID.LivingWoodUnsafe, WallID.LivingWood),
                    new Actions.PlaceWall(LivingWoodWall)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyWalls(WallID.LivingLeaf),
                    new Actions.PlaceWall(LivingLeafWall)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(TileID.Platforms),
                    new Actions.SetTileKeepWall(LivingWoodPlatformTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[] // This is so broken 😭
                {
                    new Modifiers.OnlyTiles(TileID.ClosedDoor, TileID.OpenDoor),
                    new Actions.ClearTile(),
                    new Actions.PlaceTile(LivingWoodDoorTile, 1)
                }));*/

                // Tile replacement
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(ReplaceWithChargedTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(ChargedTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(ReplaceWithSandTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(SandTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(ReplaceWithStoneTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(StoneTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(ReplaceWithBrickTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(BrickTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(ReplaceWithDirtTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(DirtTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(ClearTiles),
                    new Actions.ClearTile()
                }));

                // Charged Stone dithering
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.OnlyTiles(ReplaceWithChargedTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(ChargedTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.OnlyTiles(ReplaceWithChargedTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(ChargedTile),
                }));

                // Sand dithering
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.OnlyTiles(ReplaceWithSandTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(SandTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.OnlyTiles(ReplaceWithSandTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(SandTile),
                }));

                // Stone dithering
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.OnlyTiles(ReplaceWithStoneTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(StoneTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.OnlyTiles(ReplaceWithStoneTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(StoneTile),
                }));

                // Brick dithering
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.OnlyTiles(ReplaceWithBrickTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(BrickTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.OnlyTiles(ReplaceWithBrickTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(BrickTile),
                }));

                // Dirt dithering
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.OnlyTiles(ReplaceWithDirtTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(DirtTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.OnlyTiles(ReplaceWithDirtTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(DirtTile),
                }));

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
                    new AeroGenUtils.NotTouchingAir(true),
                    new Modifiers.SkipTiles(LivingLeafTile, LivingWoodTile, TileID.SmallPiles, TileID.LargePiles, TileID.LargePiles2),
                    new AeroGenUtils.NotTouchingTiles(true, LivingLeafTile, LivingWoodTile, TileID.SmallPiles, TileID.LargePiles, TileID.LargePiles2),
                    new Actions.PlaceWall(DirtWall)
                }));
                // Catches the area below the vanilla dirt walls
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new AeroGenUtils.IsBelowSurface(5),
                    new Modifiers.SkipTiles(LivingLeafTile, LivingWoodTile, TileID.SmallPiles, TileID.LargePiles, TileID.LargePiles2),
                    new AeroGenUtils.NotTouchingTiles(true, LivingLeafTile, LivingWoodTile, TileID.SmallPiles, TileID.LargePiles, TileID.LargePiles2),
                    new Actions.PlaceWall(DirtWall)
                }));

                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyWalls(WallID.DirtUnsafe, WallID.FlowerUnsafe, WallID.GrassUnsafe, 59, WallID.SnowWallUnsafe, WallID.Sandstone, WallID.HardenedSand, WallID.CrimstoneUnsafe, WallID.EbonstoneUnsafe),
                    new Actions.PlaceWall(DirtWall)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyWalls(ReplaceWithBrickWalls),
                    new Actions.PlaceWall(BrickWall)
                }));

                // Surface to underground dithering
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 - 5, Origin.Y - (int)(SurfaceHeight * 0.1)), new Shapes.Rectangle(BiomeWidth + 10, (int)(SurfaceHeight * 0.1)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Dither(0.1),
                    new Modifiers.OnlyTiles(DirtTile),
                    new AeroGenUtils.SwapSolidTileInclusive(StoneTile)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 - 5, Origin.Y - (int)(SurfaceHeight * 0.2)), new Shapes.Rectangle(BiomeWidth + 10, (int)(SurfaceHeight * 0.2)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Dither(0.4),
                    new Modifiers.OnlyTiles(DirtTile),
                    new AeroGenUtils.SwapSolidTileInclusive(StoneTile)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 - 5, Origin.Y - (int)(SurfaceHeight * 0.3)), new Shapes.Rectangle(BiomeWidth + 10, (int)(SurfaceHeight * 0.3)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Dither(0.7),
                    new Modifiers.OnlyTiles(DirtTile),
                    new AeroGenUtils.SwapSolidTileInclusive(StoneTile)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 - 5, Origin.Y - (int)(SurfaceHeight * 0.1)), new Shapes.Rectangle(BiomeWidth + 10, (int)(SurfaceHeight * 0.1)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Dither(0.1),
                    new Modifiers.OnlyTiles(ReplaceWithBrickTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(BrickTile)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 - 5, Origin.Y - (int)(SurfaceHeight * 0.2)), new Shapes.Rectangle(BiomeWidth + 10, (int)(SurfaceHeight * 0.2)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Dither(0.4),
                    new Modifiers.OnlyTiles(ReplaceWithBrickTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(BrickTile)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 - 5, Origin.Y - (int)(SurfaceHeight * 0.3)), new Shapes.Rectangle(BiomeWidth + 10, (int)(SurfaceHeight * 0.3)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Dither(0.7),
                    new Modifiers.OnlyTiles(ReplaceWithBrickTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(BrickTile)
                }));

                // Surface to underground wall dithering
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 - 5, Origin.Y - (int)(SurfaceHeight * 0.1)), new Shapes.Rectangle(BiomeWidth + 10, (int)(SurfaceHeight * 0.1)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Dither(0.1),
                    new Modifiers.OnlyWalls(DirtWall),
                    new Actions.PlaceWall(StoneWall)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 - 5, Origin.Y - (int)(SurfaceHeight * 0.2)), new Shapes.Rectangle(BiomeWidth + 10, (int)(SurfaceHeight * 0.2)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Dither(0.4),
                    new Modifiers.OnlyWalls(DirtWall),
                    new Actions.PlaceWall(StoneWall)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 - 5, Origin.Y - (int)(SurfaceHeight * 0.3)), new Shapes.Rectangle(BiomeWidth + 10, (int)(SurfaceHeight * 0.3)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Dither(0.7),
                    new Modifiers.OnlyWalls(DirtWall),
                    new Actions.PlaceWall(StoneWall)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 - 5, Origin.Y - (int)(SurfaceHeight * 0.1)), new Shapes.Rectangle(BiomeWidth + 10, (int)(SurfaceHeight * 0.1)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Dither(0.1),
                    new Modifiers.OnlyWalls(ReplaceWithBrickWalls),
                    new Actions.PlaceWall(BrickWall)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 - 5, Origin.Y - (int)(SurfaceHeight * 0.2)), new Shapes.Rectangle(BiomeWidth + 10, (int)(SurfaceHeight * 0.2)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Dither(0.4),
                    new Modifiers.OnlyWalls(ReplaceWithBrickWalls),
                    new Actions.PlaceWall(BrickWall)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 - 5, Origin.Y - (int)(SurfaceHeight * 0.3)), new Shapes.Rectangle(BiomeWidth + 10, (int)(SurfaceHeight * 0.3)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Dither(0.7),
                    new Modifiers.OnlyWalls(ReplaceWithBrickWalls),
                    new Actions.PlaceWall(BrickWall)
                }));

                // BIOME UNDERGROUND

                // Upper underground
                ShapeData upperUndergroundDitheringShapeData = new ShapeData();
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2, Origin.Y), new Shapes.Rectangle(BiomeWidth, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[] 
                {
                    new Modifiers.SkipTiles(LivingWoodTiles),
                    new Modifiers.OnlyTiles(ReplaceWithStoneTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(StoneTile).Output(upperUndergroundDitheringShapeData)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2, Origin.Y), new ModShapes.All(upperUndergroundDitheringShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 3),
                    new Modifiers.Dither(0.8),
                    new Modifiers.OnlyTiles(ReplaceWithStoneTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(StoneTile)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2, Origin.Y), new ModShapes.All(upperUndergroundDitheringShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 5),
                    new Modifiers.Dither(0.95),
                    new Modifiers.OnlyTiles(ReplaceWithStoneTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(StoneTile)
                }));
                // Sand UG tiles
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2, Origin.Y), new Shapes.Rectangle(BiomeWidth, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(ReplaceWithSandTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(SandTile)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2, Origin.Y), new Shapes.Rectangle(BiomeWidth, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 3),
                    new Modifiers.Dither(0.8),
                    new Modifiers.OnlyTiles(ReplaceWithSandTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(SandTile)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2, Origin.Y), new Shapes.Rectangle(BiomeWidth, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 5),
                    new Modifiers.Dither(0.95),
                    new Modifiers.OnlyTiles(ReplaceWithSandTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(SandTile)
                }));
                // Charged UG tiles
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2, Origin.Y), new Shapes.Rectangle(BiomeWidth, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(ReplaceWithChargedTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(ChargedTile)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2, Origin.Y), new Shapes.Rectangle(BiomeWidth, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 3),
                    new Modifiers.Dither(0.8),
                    new Modifiers.OnlyTiles(ReplaceWithChargedTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(ChargedTile)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2, Origin.Y), new Shapes.Rectangle(BiomeWidth, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 5),
                    new Modifiers.Dither(0.95),
                    new Modifiers.OnlyTiles(ReplaceWithChargedTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(ChargedTile)
                }));
                // Brick UG tiles
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2, Origin.Y), new Shapes.Rectangle(BiomeWidth, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(ReplaceWithBrickTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(BrickTile)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2, Origin.Y), new Shapes.Rectangle(BiomeWidth, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 3),
                    new Modifiers.Dither(0.8),
                    new Modifiers.OnlyTiles(ReplaceWithBrickTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(BrickTile)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2, Origin.Y), new Shapes.Rectangle(BiomeWidth, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 5),
                    new Modifiers.Dither(0.95),
                    new Modifiers.OnlyTiles(ReplaceWithBrickTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(BrickTile)
                }));
                // Clear other tiles
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2, Origin.Y), new Shapes.Rectangle(BiomeWidth, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(ClearTiles),
                    new Actions.ClearTile()
                }));

                // Lower underground
                ShapeData lowerUndergroundDitheringShapeData = new ShapeData();
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight)), new Shapes.Mound(BiomeWidth / 2, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Flip(false, true),
                    new Modifiers.SkipTiles(LivingWoodTiles),
                    new Modifiers.OnlyTiles(ReplaceWithStoneTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(StoneTile).Output(lowerUndergroundDitheringShapeData)
                }));
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight)), new ModShapes.All(lowerUndergroundDitheringShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 3),
                    new Modifiers.Dither(0.6),
                    new Modifiers.OnlyTiles(ReplaceWithStoneTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(StoneTile)
                }));
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight)), new ModShapes.All(lowerUndergroundDitheringShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 5),
                    new Modifiers.Dither(0.85),
                    new Modifiers.OnlyTiles(ReplaceWithStoneTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(StoneTile)
                }));
                // Sand UG tiles
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight)), new Shapes.Mound(BiomeWidth / 2, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Flip(false, true),
                    new Modifiers.SkipTiles(LivingWoodTiles),
                    new Modifiers.OnlyTiles(ReplaceWithSandTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(SandTile)
                }));
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight)), new ModShapes.All(lowerUndergroundDitheringShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 3),
                    new Modifiers.Dither(0.6),
                    new Modifiers.OnlyTiles(ReplaceWithSandTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(SandTile)
                }));
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight)), new ModShapes.All(lowerUndergroundDitheringShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 5),
                    new Modifiers.Dither(0.85),
                    new Modifiers.OnlyTiles(ReplaceWithSandTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(SandTile)
                }));
                // Charged UG tiles
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight)), new Shapes.Mound(BiomeWidth / 2, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Flip(false, true),
                    new Modifiers.SkipTiles(LivingWoodTiles),
                    new Modifiers.OnlyTiles(ReplaceWithChargedTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(ChargedTile)
                }));
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight)), new ModShapes.All(lowerUndergroundDitheringShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 3),
                    new Modifiers.Dither(0.6),
                    new Modifiers.OnlyTiles(ReplaceWithChargedTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(ChargedTile)
                }));
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight)), new ModShapes.All(lowerUndergroundDitheringShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 5),
                    new Modifiers.Dither(0.85),
                    new Modifiers.OnlyTiles(ReplaceWithChargedTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(ChargedTile)
                }));
                // Brick UG tiles
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight)), new Shapes.Mound(BiomeWidth / 2, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Flip(false, true),
                    new Modifiers.SkipTiles(LivingWoodTiles),
                    new Modifiers.OnlyTiles(ReplaceWithBrickTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(BrickTile)
                }));
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight)), new ModShapes.All(lowerUndergroundDitheringShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 3),
                    new Modifiers.Dither(0.6),
                    new Modifiers.OnlyTiles(ReplaceWithBrickTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(BrickTile)
                }));
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight)), new ModShapes.All(lowerUndergroundDitheringShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 5),
                    new Modifiers.Dither(0.85),
                    new Modifiers.OnlyTiles(ReplaceWithBrickTiles),
                    new AeroGenUtils.SwapSolidTileInclusive(BrickTile)
                }));
                // Clear other tiles
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight)), new Shapes.Mound(BiomeWidth / 2, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Flip(false, true),
                    new Modifiers.OnlyTiles(ClearTiles),
                    new Actions.ClearTile()
                }));

                // Upper underground walls
                ShapeData upperUndergroundWallDitheringShapeData = new ShapeData();
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 + 1, Origin.Y), new Shapes.Rectangle(BiomeWidth - 1, (int)(.5 * UndergroundHeight - 1)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.SkipWalls(ReplaceWithBrickWalls.Concat([LivingWoodWall, BrickWall]).ToArray()),
                    new Actions.PlaceWall(StoneWall).Output(upperUndergroundWallDitheringShapeData)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 + 1, Origin.Y), new ModShapes.All(upperUndergroundWallDitheringShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 3),
                    new Modifiers.Dither(0.85),
                    new Modifiers.SkipWalls(ReplaceWithBrickWalls.Concat([LivingWoodWall, BrickWall]).ToArray()),
                    new Actions.PlaceWall(StoneWall)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 + 1, Origin.Y), new ModShapes.All(upperUndergroundWallDitheringShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 5),
                    new Modifiers.Dither(0.95),
                    new Modifiers.SkipWalls(ReplaceWithBrickWalls.Concat([LivingWoodWall, BrickWall]).ToArray()),
                    new Actions.PlaceWall(StoneWall)
                }));

                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 + 1, Origin.Y), new Shapes.Rectangle(BiomeWidth - 1, (int)(.5 * UndergroundHeight - 1)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyWalls(ReplaceWithBrickWalls),
                    new Actions.PlaceWall(BrickWall)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 + 1, Origin.Y), new Shapes.Rectangle(BiomeWidth - 1, (int)(.5 * UndergroundHeight - 1)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 3),
                    new Modifiers.Dither(0.85),
                    new Modifiers.OnlyWalls(ReplaceWithBrickWalls),
                    new Actions.PlaceWall(BrickWall)
                }));
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 + 1, Origin.Y), new Shapes.Rectangle(BiomeWidth - 1, (int)(.5 * UndergroundHeight - 1)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 5),
                    new Modifiers.Dither(0.95),
                    new Modifiers.OnlyWalls(ReplaceWithBrickWalls),
                    new Actions.PlaceWall(BrickWall)
                }));

                // Lower underground walls
                ShapeData lowerUndergroundWallDitheringShapeData = new ShapeData();
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight) - 1), new Shapes.Mound(BiomeWidth / 2 - 1, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Flip(false, true),
                    new Modifiers.SkipWalls(ReplaceWithBrickWalls),
                    new Actions.PlaceWall(StoneWall).Output(lowerUndergroundWallDitheringShapeData)
                }));
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight) - 1), new ModShapes.All(lowerUndergroundWallDitheringShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 3),
                    new Modifiers.Dither(0.6),
                    new Modifiers.SkipWalls(ReplaceWithBrickWalls),
                    new Actions.PlaceWall(StoneWall)
                }));
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight) - 1), new ModShapes.All(lowerUndergroundWallDitheringShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 5),
                    new Modifiers.Dither(0.85),
                    new Modifiers.SkipWalls(ReplaceWithBrickWalls),
                    new Actions.PlaceWall(StoneWall)
                }));

                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight) - 1), new Shapes.Mound(BiomeWidth / 2 - 1, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Flip(false, true),
                    new Modifiers.OnlyWalls(ReplaceWithBrickWalls),
                    new Actions.PlaceWall(BrickWall)
                }));
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight) - 1), new Shapes.Mound(BiomeWidth / 2 - 1, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Flip(false, true),
                    new Modifiers.Expand(3, 3),
                    new Modifiers.Dither(0.6),
                    new Modifiers.OnlyWalls(ReplaceWithBrickWalls),
                    new Actions.PlaceWall(BrickWall)
                }));
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight) - 1), new Shapes.Mound(BiomeWidth / 2 - 1, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Flip(false, true),
                    new Modifiers.Expand(5, 5),
                    new Modifiers.Dither(0.85),
                    new Modifiers.OnlyWalls(ReplaceWithBrickWalls),
                    new Actions.PlaceWall(BrickWall)
                }));

                // Clear a lot of the walls to allow the background to be visible frequently
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2 + 1, Origin.Y), new ModShapes.All(upperUndergroundWallDitheringShapeData), Actions.Chain(new GenAction[]
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
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight) - 1), new ModShapes.All(lowerUndergroundWallDitheringShapeData), Actions.Chain(new GenAction[]
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

                // Main lightning bolt cave
                WorldUtils.Gen(new Point(Origin.X, Origin.Y - (int)(SurfaceHeight * 1.75)), new AeroShapes.LightningBoltShape(550 * WorldSizeScale, 50 * (int)((WorldSizeScale - 1) * 0.8 + 1), 2, 30), Actions.Chain(new GenAction[]
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
                        new AeroGenUtils.IsTouchingWall(true, DirtWall),
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
                    new AeroGenUtils.PlaceBlob(StoneTile, 5.5f, 5.5f, 1.5f, 1.5f),
                }));

                // Surface crystal growths
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceExposedShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Offset(0, 2),
                    new Modifiers.Dither(.985), // 1/66.66 chance
                    new Modifiers.OnlyTiles(GrassTile, DirtTile, SandTile, StoneTile),
                    new AeroGenUtils.PlaceTail(CrystalTile, 6, new Vector2D(0, -20), 0, 4, 3)
                }));

                TumblerArenaPolarity = WorldGen.genRand.NextBool().ToDirectionInt();

                TumblerTunnelEnd = WorldGen.digTunnel(Origin.X, Origin.Y + UndergroundHeight / 2, 3 * TumblerArenaPolarity, 0, 65 * WorldSizeScale, 5).ToPoint();
                WorldGen.digTunnel(Origin.X, Origin.Y + UndergroundHeight / 2, -3 * TumblerArenaPolarity, 0, 65 * WorldSizeScale, 5);
                // Moved to CC Polish
                //StructureStamper.LoadStructure(new Vector2(TumblerTunnelEnd.X - 60 + 60 * tumblerArenaPolarity, TumblerTunnelEnd.Y - 46), "tumblerarena");
            }
		}

		private Point determineOrigin(int biomeWidth, int undergroundHeight, int surfaceHeight, int biomeHeight)
		{
            int worldSizeScale = Main.maxTilesY / 1200;
            Point fallbackPoint = Point.Zero;
            Point evilFallbackPoint = Point.Zero;
            // Using Point.Zero as a standin for a 'null' value, aka no valid spawn location found and the biome will not generate
            Point surfacePoint = Point.Zero;

            for (int attempts = 0; attempts < 10000; attempts++)
			{
                int x = WorldGen.genRand.Next(500 * worldSizeScale, Main.maxTilesX - (500 * worldSizeScale));
				while (Main.maxTilesX * .4 < x && x < Main.maxTilesX * .6)
				{
					x = WorldGen.genRand.Next(500 * worldSizeScale, Main.maxTilesX - (500 * worldSizeScale));
				}

                Point initialPoint = new Point(x, (int)Main.worldSurface);

				bool flag = WorldUtils.Find(initialPoint, Searches.Chain(new Searches.Up(200 * worldSizeScale), new Conditions.IsTile(TileID.LivingWood, TileID.LeafBlock).AreaOr(1, 50)), out Point _);
				if (flag)
					continue;
                flag = WorldUtils.Find(initialPoint, Searches.Chain(new Searches.Up(1000), new Conditions.IsSolid().AreaOr(1, 50).Not()), out surfacePoint);
                // Adjust result to point to surface, not 50 tiles above 
                surfacePoint.Y += 50;

                // Search up to 1000 tiles above for an area 50 tiles tall and 1 tile wide without a single solid tile. Basically find the surface.
                if (!flag)
                    continue;

                // Check on the left bound, mid-left side, center line, mid-right side, and right bound for suboptimal but sometimes acceptable results
                if (!checkPointEvilFallback(-(int)(.5 * biomeWidth), surfacePoint) ||
                    !checkPointEvilFallback(-(int)(.25 * biomeWidth), surfacePoint) ||
                    !checkPointEvilFallback(0, surfacePoint) ||
                    !checkPointEvilFallback((int)(.25 * biomeWidth), surfacePoint) ||
                    !checkPointEvilFallback((int)(.5 * biomeWidth), surfacePoint))
                    continue;
                evilFallbackPoint = surfacePoint;
                // Check on the left bound, mid-left side, center line, mid-right side, and right bound for suboptimal but acceptable results
                if (!checkPointFallback(-(int)(.5 * biomeWidth), surfacePoint) ||
                    !checkPointFallback(-(int)(.25 * biomeWidth), surfacePoint) ||
                    !checkPointFallback(0, surfacePoint) ||
                    !checkPointFallback((int)(.25 * biomeWidth), surfacePoint) ||
                    !checkPointFallback((int)(.5 * biomeWidth), surfacePoint))
                    continue;
                fallbackPoint = surfacePoint;
                // Check on the left side, mid-left side, center line, mid-right side, and right side of the biome
                if (!checkPoint(-(int)(.5 * biomeWidth), surfacePoint) ||
                    !checkPoint(-(int)(.25 * biomeWidth), surfacePoint) ||
                    !checkPoint(0, surfacePoint) ||
                    !checkPoint((int)(.25 * biomeWidth), surfacePoint) ||
                    !checkPoint((int)(.5 * biomeWidth), surfacePoint))
					continue;

                Console.WriteLine("Crystal Caverns generation process finished in " + attempts + " attempts.");
				surfacePoint.Y = determineOriginY(biomeWidth, surfacePoint); // Correct the Y position of the biome to the average of the right and left bound's surrounding terrain height
                GenVars.structures.AddProtectedStructure(new Rectangle(surfacePoint.X - (int)(.5 * biomeWidth), surfacePoint.Y, biomeWidth, biomeHeight), 0);
                return surfacePoint;
            }
            Console.WriteLine("Could not find a suitable location to place the Crystal Caverns");
			if (fallbackPoint != Point.Zero)
			{
				Console.WriteLine("Falling back to a location overlapping with evil to generate the Crystal Caverns");
                surfacePoint.Y = determineOriginY(biomeWidth, surfacePoint); // Correct the Y position of the biome to the average of the right and left bound's surrounding terrain height
                GenVars.structures.AddProtectedStructure(new Rectangle(surfacePoint.X - (int)(.5 * biomeWidth), surfacePoint.Y, biomeWidth, biomeHeight), 0);
                return fallbackPoint;
            }
            if (evilFallbackPoint != Point.Zero)
            {
                Console.WriteLine("Falling back to a location overlapping with evil / jungle / ice to generate the Crystal Caverns");
                surfacePoint.Y = determineOriginY(biomeWidth, surfacePoint); // Correct the Y position of the biome to the average of the right and left bound's surrounding terrain height
                GenVars.structures.AddProtectedStructure(new Rectangle(surfacePoint.X - (int)(.5 * biomeWidth), surfacePoint.Y, biomeWidth, biomeHeight), 0);
            }
            return evilFallbackPoint;
        }

		private bool checkPoint(int xOffset, Point surfacePoint) 
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
            if (WorldUtils.Find(point, Searches.Chain(new Searches.Down(UndergroundHeight + SurfaceHeight + 100), new AeroConditions.HasShimmer()), out Point _))
                return false;            
            return true;
        }

		private bool checkPointFallback(int xOffset, Point surfacePoint)
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
            if (WorldUtils.Find(point, Searches.Chain(new Searches.Down(UndergroundHeight + SurfaceHeight + 100), new AeroConditions.HasShimmer()), out Point _))
                return false;
            return true;
        }

        private bool checkPointEvilFallback(int xOffset, Point surfacePoint)
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

        private int determineOriginY(int biomeWidth, Point surfacePoint)
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

/*				if (leftPoint.Y < (int)Main.worldSurface && rightPoint.Y < (int)Main.worldSurface)
				{
					break;
				}*/

                leftPoint = new Point(surfacePoint.X - xOffset + attempts, leftPoint.Y);
                rightPoint = new Point(surfacePoint.X + xOffset + attempts, rightPoint.Y);
            }
            return (leftPoint.Y + rightPoint.Y) / 2;
        }
	}
}