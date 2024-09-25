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
    public class CrystalCavernsTerrainPass : GenPass
	{
		public CrystalCavernsTerrainPass(string name, float loadWeight) : base(name, loadWeight)
		{
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = WorldGenSystem.CrystalCavernsTerrainPassMessage.Value;
            int worldSizeScale = Main.maxTilesY / 1200;
            
            int biomeWidth = 400 * worldSizeScale;
			int surfaceHeight = 50 * worldSizeScale;
			int undergroundHeight = 400 * worldSizeScale;
			int biomeHeight = undergroundHeight + surfaceHeight;

            ushort grassTile = (ushort)ModContent.TileType<CrystalGrass>();
            ushort dirtTile = (ushort)ModContent.TileType<CrystalDirt>();
            ushort stoneTile = (ushort)ModContent.TileType<CavernStone>();
            ushort sandTile = (ushort)ModContent.TileType<CavernSand>();
            ushort crystalTile = (ushort)ModContent.TileType<CavernCrystal>();
            ushort dirtWall = (ushort)ModContent.WallType<CavernDirtWall>();
            ushort stoneWall = (ushort)ModContent.WallType<CavernStoneWall>();

            Point origin = determineOrigin(biomeWidth, undergroundHeight, surfaceHeight, biomeHeight); //center x, top of underground y
			origin.Y += (int)(surfaceHeight * 0.7);
			if (!origin.Equals(Point.Zero)) 
            {
            // BIOME SURFACE
                ShapeData surfaceMoundShapeData = new ShapeData();
                ShapeData surfaceRectShapeData = new ShapeData();
                ShapeData surfaceExposedShapeData = new ShapeData();
                Point surfaceRectOrigin = new Point(origin.X - biomeWidth / 2, origin.Y - (int)(surfaceHeight * 1.5));

            // Surface conversion
                WorldUtils.Gen(surfaceRectOrigin, new Shapes.Rectangle(biomeWidth, (int)(surfaceHeight * 1.5)), new Actions.Blank().Output(surfaceRectShapeData));

                // Tile replacement
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(TileID.Sand, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand),
                    new AeroActions.SwapSolidTileInclusive(sandTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.SkipTiles(grassTile, dirtTile, stoneTile, sandTile),
                    new AeroActions.SwapSolidTileInclusive(dirtTile)
                }));

                // Dithering
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.OnlyTiles(TileID.Sand, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand),
                    new Modifiers.SkipTiles(grassTile, dirtTile, stoneTile, sandTile),
                    new AeroActions.SwapSolidTileInclusive(sandTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.OnlyTiles(TileID.Sand, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand),
                    new Modifiers.SkipTiles(grassTile, dirtTile, stoneTile, sandTile),
                    new AeroActions.SwapSolidTileInclusive(sandTile),
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(3, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.SkipTiles(grassTile, dirtTile, stoneTile, sandTile, TileID.Sand, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand),
                    new AeroActions.SwapSolidTileInclusive(dirtTile)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 0),
                    new Modifiers.Dither(0.75),
                    new Modifiers.SkipTiles(grassTile, dirtTile, stoneTile, sandTile, TileID.Sand, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand),
                    new AeroActions.SwapSolidTileInclusive(dirtTile),
                }));

                // Grass
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Expand(5, 0),
                    new Modifiers.OnlyTiles(dirtTile),
                    new Modifiers.IsTouchingAir(true),
                    new AeroActions.SwapSolidTileInclusive(grassTile)
                }));

                // Walls
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new AeroConditions.NotTouchingAir(true),
                    new Actions.PlaceWall(dirtWall)
                }));
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyWalls(WallID.DirtUnsafe, WallID.FlowerUnsafe, WallID.GrassUnsafe, 59),
                    new Actions.PlaceWall(dirtWall)
                }));

                // Surface mound
                WorldUtils.Gen(origin, new Shapes.Mound(biomeWidth / 2, surfaceHeight), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Blotches(5, 1, 0.2),
                    new Modifiers.Blotches(4, 2, 0.3),
                    new Modifiers.Blotches(3, 2, 0.3),
                    new Actions.Blank().Output(surfaceMoundShapeData)
                }));

                WorldUtils.Gen(origin, new ModShapes.All(surfaceMoundShapeData), Actions.Chain(new GenAction[]
                {
                    new Actions.SetTile(dirtTile)
                }));

                WorldUtils.Gen(origin, new ModShapes.All(surfaceMoundShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(dirtTile),
                    new Modifiers.IsTouchingAir(true),
                    new Actions.SetTile(grassTile)
                }));

                WorldUtils.Gen(origin, new ModShapes.All(surfaceMoundShapeData), Actions.Chain(new GenAction[]
                {
                    new AeroConditions.NotTouchingAir(true),
                    new Actions.PlaceWall(dirtWall)
                }));

                // Crystal growths
                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(grassTile, dirtTile, sandTile),
                    new Modifiers.IsTouchingAir(true),
                    new Actions.Blank().Output(surfaceExposedShapeData),
                }));

                WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceExposedShapeData), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Offset(0, 2),
                    new Modifiers.Dither(.9925), // 1/133.33 chance
                    new AeroActions.PlaceTail(crystalTile, 8, new Vector2D(0, -20), 1, 7, 7)
                }));

            // BIOME UNDERGROUND

                // Upper underground
                WorldUtils.Gen(new Point(origin.X - biomeWidth / 2, origin.Y), new Shapes.Rectangle(biomeWidth, (int)(.5 * undergroundHeight)), new Actions.SetTile(stoneTile));
                // Lower underground
                WorldUtils.Gen(new Point(origin.X, origin.Y + (int)(.5 * undergroundHeight)), new Shapes.Mound(biomeWidth/2, (int)(.5 * undergroundHeight)), Actions.Chain(new GenAction[]
				{
					new Modifiers.Flip(false, true),
					new Actions.SetTile(stoneTile),
				}));
                
                // Upper underground walls
                WorldUtils.Gen(new Point(origin.X - biomeWidth / 2, origin.Y), new Shapes.Rectangle(biomeWidth - 1, (int)(.5 * undergroundHeight - 1)), new Actions.PlaceWall(stoneWall));
                // Lower underground walls
                WorldUtils.Gen(new Point(origin.X, origin.Y + (int)(.5 * undergroundHeight) - 1), new Shapes.Mound(biomeWidth / 2 - 1, (int)(.5 * undergroundHeight)), Actions.Chain(new GenAction[]
				{
                    new Modifiers.Flip(false, true),
                    new Actions.PlaceWall(stoneWall)
                }));
                
                // Main lightning bolt cave
                WorldUtils.Gen(new Point(origin.X, origin.Y - (int)(surfaceHeight * 1.5)), new AeroShapes.LightningBoltShape(400 * worldSizeScale, 60 * (int)((worldSizeScale - 1) * 0.8 + 1), 2, 30), new Actions.ClearTile());

                int tumblerArenaPolarity = WorldGen.genRand.NextBool().ToDirectionInt();

                Point tumblerTunnelEnd = WorldGen.digTunnel(origin.X, origin.Y + undergroundHeight / 2, 3 * tumblerArenaPolarity, 0, 60 * worldSizeScale, 5).ToPoint();
                WorldGen.digTunnel(origin.X, origin.Y + undergroundHeight / 2, -3 * tumblerArenaPolarity, 0, 60 * worldSizeScale, 5);
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