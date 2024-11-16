using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.WorldBuilding;
using Terraria.IO;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using System;
using AerovelenceMod.Common.Systems.Generation.GenUtils;
using AerovelenceMod.Common.Utilities.StructureStamper;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Building;
using AerovelenceMod.Content.Walls.CrystalCaverns.Natural;
using Terraria.Graphics.Shaders;
using Terraria.GameContent.Generation;
using ReLogic.Utilities;
using System.Security.Cryptography.X509Certificates;

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
        public ushort DirtWall { get; private set; }
        public ushort StoneWall { get; private set; }

        public Point Origin { get; private set; }

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
            _instance.SurfaceHeight = 50 * _instance.WorldSizeScale;
            _instance.UndergroundHeight = 400 * _instance.WorldSizeScale;
            _instance.BiomeHeight = _instance.UndergroundHeight + _instance.SurfaceHeight;
            
            GrassTile = (ushort)ModContent.TileType<CrystalGrass>();
            DirtTile = (ushort)ModContent.TileType<CrystalDirt>();
            StoneTile = (ushort)ModContent.TileType<CavernStone>();
            SandTile = (ushort)ModContent.TileType<CavernSand>();
            CrystalTile = (ushort)ModContent.TileType<CavernCrystal>();
            DirtWall = (ushort)ModContent.WallType<CavernDirtWall>();
            StoneWall = (ushort)ModContent.WallType<CavernStoneWall>();

            Origin = determineOrigin(BiomeWidth, UndergroundHeight, SurfaceHeight, BiomeHeight); //center x, top of underground y
            if (!Origin.Equals(Point.Zero)) 
            {
                Origin = new Point(Origin.X, Origin.Y + (int)(SurfaceHeight * 0.7));
                // BIOME SURFACE
                ShapeData surfaceMoundShapeData = new ShapeData();
                ShapeData surfaceRectShapeData = new ShapeData();
                ShapeData surfaceExposedShapeData = new ShapeData();
                ShapeData lightningBoltShapeData = new ShapeData();
                ShapeData lightningBoltEdgeShapeData = new ShapeData();
                Point surfaceRectOrigin = new Point(Origin.X - BiomeWidth / 2, Origin.Y - (int)(SurfaceHeight * 1.75));

            // Surface conversion
                WorldUtils.Gen(surfaceRectOrigin, new Shapes.Rectangle(BiomeWidth, (int)(SurfaceHeight * 1.75)), new Actions.Blank().Output(surfaceRectShapeData));

                // Tile replacement
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(TileID.LivingWood, TileID.LeafBlock),
                    new Actions.Clear()
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(TileID.Sand, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand),
                    new AeroActions.SwapSolidTileInclusive(SandTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.SkipTiles(GrassTile, DirtTile, StoneTile, SandTile),
                    new AeroActions.SwapSolidTileInclusive(DirtTile)
                }));

                // Dithering
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.OnlyTiles(TileID.Sand, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand),
                    new Modifiers.SkipTiles(GrassTile, DirtTile, StoneTile, SandTile),
                    new AeroActions.SwapSolidTileInclusive(SandTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.OnlyTiles(TileID.Sand, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand),
                    new Modifiers.SkipTiles(GrassTile, DirtTile, StoneTile, SandTile),
                    new AeroActions.SwapSolidTileInclusive(SandTile),
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.SkipTiles(GrassTile, DirtTile, StoneTile, SandTile, TileID.Sand, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand),
                    new AeroActions.SwapSolidTileInclusive(DirtTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.SkipTiles(GrassTile, DirtTile, StoneTile, SandTile, TileID.Sand, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand),
                    new AeroActions.SwapSolidTileInclusive(DirtTile),
                }));

                // Grass
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 0),
                    new Modifiers.OnlyTiles(DirtTile),
                    new Modifiers.IsTouchingAir(true),
                    new AeroActions.SwapSolidTileInclusive(GrassTile)
                }));

                // Walls
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new AeroActions.NotTouchingAir(true),
                    new Actions.PlaceWall(DirtWall)
                }));

                // Moved to CrystalCavernsCaveFixPass
                
                /*WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyWalls(WallID.DirtUnsafe, WallID.FlowerUnsafe, WallID.GrassUnsafe, 59),
                    new Actions.ClearWall(),
                    new Actions.PlaceWall(DirtWall)
                }));*/

                // Surface mound
                WorldUtils.Gen(Origin, new Shapes.Mound(BiomeWidth / 2, SurfaceHeight), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Blotches(5, 1, 0.2),
                    new Modifiers.Blotches(4, 2, 0.3),
                    new Modifiers.Blotches(3, 2, 0.3),
                    new Actions.Blank().Output(surfaceMoundShapeData)
                }));

                WorldUtils.Gen(Origin, new ModShapes.All(surfaceMoundShapeData), Actions.Chain(new GenAction[]
                {
                    new Actions.SetTile(DirtTile)
                }));

                WorldUtils.Gen(Origin, new ModShapes.All(surfaceMoundShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(DirtTile),
                    new Modifiers.IsTouchingAir(true),
                    new Actions.SetTile(GrassTile)
                }));

                WorldUtils.Gen(Origin, new ModShapes.All(surfaceMoundShapeData), Actions.Chain(new GenAction[]
                {
                    new AeroActions.NotTouchingAir(true),
                    new Actions.PlaceWall(DirtWall)
                }));

            // BIOME UNDERGROUND

                // Upper underground
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2, Origin.Y), new Shapes.Rectangle(BiomeWidth, (int)(.5 * UndergroundHeight)), new Actions.SetTile(StoneTile));
                // Lower underground
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight)), new Shapes.Mound(BiomeWidth/2, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
				{
					new Modifiers.Flip(false, true),
					new Actions.SetTile(StoneTile),
				}));
                
                // Upper underground walls
                WorldUtils.Gen(new Point(Origin.X - BiomeWidth / 2, Origin.Y), new Shapes.Rectangle(BiomeWidth - 1, (int)(.5 * UndergroundHeight - 1)), new Actions.PlaceWall(StoneWall));
                // Lower underground walls
                WorldUtils.Gen(new Point(Origin.X, Origin.Y + (int)(.5 * UndergroundHeight) - 1), new Shapes.Mound(BiomeWidth / 2 - 1, (int)(.5 * UndergroundHeight)), Actions.Chain(new GenAction[]
				{
                    new Modifiers.Flip(false, true),
                    new Actions.PlaceWall(StoneWall)
                }));

                // Main lightning bolt cave
                WorldUtils.Gen(new Point(Origin.X, Origin.Y - (int)(SurfaceHeight * 1.75)), new AeroShapes.LightningBoltShape(400 * WorldSizeScale, 60 * (int)((WorldSizeScale - 1) * 0.8 + 1), 2, 30), Actions.Chain(new GenAction[]
                {
                    new Modifiers.SkipTiles(CrystalTile),
                    new Actions.ClearTile().Output(lightningBoltShapeData),
                }));

                WorldUtils.Gen(new Point(Origin.X, Origin.Y - (int)(SurfaceHeight * 1.75)), new ModShapes.All(lightningBoltShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 0),
                    new Modifiers.OnlyTiles(DirtTile, StoneTile),
                    new Modifiers.IsTouchingAir(true),
                    new Modifiers.RectangleMask(-BiomeWidth / 2, BiomeWidth / 2, 5, (int)(SurfaceHeight * 1.75) + (int)(UndergroundHeight * 0.6)),
                    new Modifiers.Blotches(3, 3, 0.035).Output(lightningBoltEdgeShapeData),
                    new Modifiers.Blotches(4, 4, 0.015).Output(lightningBoltEdgeShapeData)

                }));

                WorldUtils.Gen(new Point(Origin.X, Origin.Y - (int)(SurfaceHeight * 1.75)), new ModShapes.All(lightningBoltShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 0),
                    new Modifiers.OnlyTiles(DirtTile, StoneTile),
                    new Modifiers.IsTouchingAir(true),
                    new Modifiers.RectangleMask(-BiomeWidth / 2, BiomeWidth / 2, 5, (int)(SurfaceHeight * 1.75)),
                    new Modifiers.Blotches(3, 3, 1),
                    new Modifiers.NotInShape(lightningBoltShapeData),
                    new Actions.SetTileKeepWall(StoneTile)
                }));
                WorldUtils.Gen(new Point(Origin.X, Origin.Y - (int)(SurfaceHeight * 1.75)), new ModShapes.All(lightningBoltEdgeShapeData), Actions.Chain(new GenAction[]
                {
                    new Actions.SetTileKeepWall(StoneTile)
                }));

                // Surface crystal growths
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(GrassTile, DirtTile, SandTile),
                    new Modifiers.IsTouchingAir(true),
                    new AeroActions.SolidBelow(10),
                    new AeroActions.NotSolidAbove(50),
                    new Actions.Blank().Output(surfaceExposedShapeData),
                }));

                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceExposedShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Offset(0, 2),
                    new Modifiers.Dither(.985), // 1/66.66 chance
                    new Modifiers.OnlyTiles(GrassTile, DirtTile, SandTile),
                    new AeroActions.PlaceTail(CrystalTile, 7, new Vector2D(0, -25), 1, 8, 5)
                }));

                int tumblerArenaPolarity = WorldGen.genRand.NextBool().ToDirectionInt();

                Point tumblerTunnelEnd = WorldGen.digTunnel(Origin.X, Origin.Y + UndergroundHeight / 2, 3 * tumblerArenaPolarity, 0, 60 * WorldSizeScale, 5).ToPoint();
                WorldGen.digTunnel(Origin.X, Origin.Y + UndergroundHeight / 2, -3 * tumblerArenaPolarity, 0, 60 * WorldSizeScale, 5);
                StructureStamper.LoadStructure(new Vector2(tumblerTunnelEnd.X - 60 + 60 * tumblerArenaPolarity, tumblerTunnelEnd.Y - 46), "tumblerarena");
            }
		}

		private Point determineOrigin(int biomeWidth, int undergroundHeight, int surfaceHeight, int biomeHeight)
		{
            int worldSizeScale = Main.maxTilesY / 1200;
            Point fallbackPoint = Point.Zero;
			// Using Point.Zero as a standin for a 'null' value, aka no valid spawn location found and the biome will not generate
			Point surfacePoint = Point.Zero;

            for (int attempts = 0; attempts < 5000; attempts++)
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

                // Check on the left side, mid-left side, center line, mid-right side, and right side of the biome
                if (!checkPoint(-(int)(.5 * biomeWidth), surfacePoint, undergroundHeight) ||
                    !checkPoint(-(int)(.25 * biomeWidth), surfacePoint, undergroundHeight) ||
                    !checkPoint(0, surfacePoint, undergroundHeight) ||
                    !checkPoint((int)(.25 * biomeWidth), surfacePoint, undergroundHeight) ||
                    !checkPoint((int)(.5 * biomeWidth), surfacePoint, undergroundHeight))
					continue;
				// Check on the left bound, mid-left side, center line, mid-right side, and right bound for suboptimal but acceptable results
                if (!checkPointFallback(-(int)(.5 * biomeWidth), surfacePoint) ||
                    !checkPointFallback(-(int)(.25 * biomeWidth), surfacePoint) ||
                    !checkPointFallback(0, surfacePoint) ||
                    !checkPointFallback((int)(.25 * biomeWidth), surfacePoint) ||
                    !checkPointFallback((int)(.5 * biomeWidth), surfacePoint))
                {
                    fallbackPoint = surfacePoint;
                    continue;
                }
                Console.WriteLine("Crystal Caverns generation process finished in " + attempts + " attempts.");
				surfacePoint.Y = determineOriginY(biomeWidth, surfacePoint); // Correct the Y position of the biome to the average of the right and left bound's surrounding terrain height
                GenVars.structures.AddProtectedStructure(new Rectangle(surfacePoint.X - (int)(.5 * biomeWidth), surfacePoint.Y, biomeWidth, biomeHeight), 0);
                return surfacePoint;
            }
            Console.WriteLine("Could not find a suitable location to place the Crystal Caverns");
			if (fallbackPoint != Point.Zero)
			{
				Console.WriteLine("Falling back to a location overlapping with an evil biome to generate the Crystal Caverns");
                surfacePoint.Y = determineOriginY(biomeWidth, surfacePoint); // Correct the Y position of the biome to the average of the right and left bound's surrounding terrain height
                GenVars.structures.AddProtectedStructure(new Rectangle(surfacePoint.X - (int)(.5 * biomeWidth), surfacePoint.Y, biomeWidth, biomeHeight), 0);
            }
            return fallbackPoint;

		}

		private bool checkPoint(int xOffset, Point surfacePoint, int undergroundHeight) 
		{
			Point point = new Point(surfacePoint.X + xOffset, surfacePoint.Y);
			//surfacePoint argument means only the central point is taken into consideration, while point means all three are
			if (WorldUtils.Find(surfacePoint, Searches.Chain(new Searches.Down(100), new Conditions.IsTile(
				TileID.JungleGrass,
				TileID.IceBlock)), out Point _))
				return false;
            if (WorldUtils.Find(point, Searches.Chain(new Searches.Down(undergroundHeight), new Conditions.IsTile(
                TileID.Sandstone,
                TileID.BlueDungeonBrick,
                TileID.GreenDungeonBrick,
                TileID.PinkDungeonBrick,
                TileID.LihzahrdBrick)), out Point _))
                return false;
            if (WorldUtils.Find(point, Searches.Chain(new Searches.Down(undergroundHeight + 100), new AeroConditions.HasShimmer()), out Point _))
                return false;            
            return true;
        }

		private bool checkPointFallback(int xOffset, Point surfacePoint)
		{
            Point point = new Point(surfacePoint.X + xOffset, surfacePoint.Y);
            if (WorldUtils.Find(point, Searches.Chain(new Searches.Down(100), new Conditions.IsTile(
                TileID.JungleGrass,
                TileID.IceBlock)), out Point _))
                return false;
            if (WorldUtils.Find(point, Searches.Chain(new Searches.Down(100), new Conditions.IsTile(
                    TileID.Crimstone,
                    TileID.Ebonstone,
                    TileID.Crimsand,
                    TileID.Ebonsand,
                    TileID.CorruptGrass,
                    TileID.CrimsonGrass)), out Point _))
                return false;
			return true;
        }

		private int determineOriginY(int biomeWidth, Point surfacePoint)
		{
			int xOffset = (int)(.5 * biomeWidth);
            Point leftPoint = new Point(surfacePoint.X - xOffset, (int)Main.worldSurface);
            Point rightPoint = new Point(surfacePoint.X + xOffset, (int)Main.worldSurface);
            for (int attempts = 2; attempts < 6; attempts += 2) // This for loop is meant to solve corruption chasms dragging the average very far down
			{
                WorldUtils.Find(leftPoint, Searches.Chain(new Searches.Up(1000), new Conditions.IsSolid().AreaOr(1, 50).Not()), out leftPoint);
                leftPoint.Y += 50; // Adjust result to point to surface, not 50 tiles above 

                WorldUtils.Find(rightPoint, Searches.Chain(new Searches.Up(1000), new Conditions.IsSolid().AreaOr(1, 50).Not()), out rightPoint);
                rightPoint.Y += 50; // Adjust result to point to surface, not 50 tiles above 

				if (leftPoint.Y < (int)Main.worldSurface && rightPoint.Y < (int)Main.worldSurface)
				{
					break;
				}

                leftPoint = new Point(surfacePoint.X - xOffset + attempts, leftPoint.Y);
                rightPoint = new Point(surfacePoint.X + xOffset + attempts, rightPoint.Y);
            }
            return (leftPoint.Y + rightPoint.Y) / 2;
        }
	}
}