using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.WorldBuilding;
using Terraria.IO;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using AerovelenceMod.Common.Systems.Generation.GenConditions;
using System;

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
            int biomeWidth = 300 * worldSizeScale;
			int surfaceHeight = 100 * worldSizeScale;
			int undergroundHeight = 300 * worldSizeScale;
			int biomeHeight = undergroundHeight + surfaceHeight;
			Point origin = determineOrigin(biomeWidth, undergroundHeight, surfaceHeight, biomeHeight); //center x, top of underground y
			//origin.Y += surfaceHeight;
			if (!origin.Equals(Point.Zero)) 
			{
                WorldUtils.Gen(new Point(origin.X - biomeWidth / 2, origin.Y - surfaceHeight), new Shapes.Rectangle(biomeWidth, biomeHeight), new Actions.SetTile((ushort)ModContent.TileType<CavernStone>()));
                WorldUtils.Gen(new Point(origin.X - biomeWidth / 2, origin.Y - 150), new Shapes.Rectangle(biomeWidth, 150), new Actions.Clear()); //Clear space above the biome
				WorldUtils.Gen(origin, new Shapes.Mound(biomeWidth / 2, surfaceHeight), new Actions.SetTile((ushort)ModContent.TileType<CavernStone>())); //Biome surface 

                WorldUtils.Gen(new Point(origin.X - biomeWidth / 2, origin.Y), new Shapes.Rectangle(biomeWidth, undergroundHeight / 2), new Actions.SetTile((ushort)ModContent.TileType<ChargedStone>()));

                WorldUtils.Gen(new Point(origin.X, origin.Y + undergroundHeight / 2), new Shapes.Mound(biomeWidth/2, undergroundHeight / 2), Actions.Chain(new GenAction[] //Biome underground
				{
					new Modifiers.Flip(false, true),
					new Actions.SetTile((ushort)ModContent.TileType<ChargedStone>()),
				}));
				WorldUtils.Gen(new Point(origin.X, origin.Y + 2), new Shapes.Mound(biomeWidth / 2 - 2, undergroundHeight - 3), Actions.Chain(new GenAction[] // Biome underground walls
				{
				new Modifiers.Flip(false, true),
				new Actions.PlaceWall(WallID.Stone)
				}));
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

                // Check on the left side, center line, and right side of the biome
                if (!checkPoint(-(int)(.5 * biomeWidth + 100), surfacePoint, undergroundHeight) || 
					!checkPoint(0, surfacePoint, undergroundHeight) || 
					!checkPoint((int)(.5 * biomeWidth) - 100, surfacePoint, undergroundHeight))
					continue;
				// Check on the left bound, center line, and right bound for suboptimal but acceptable results
                if (!checkPointFallback(-(int)(.5 * biomeWidth), surfacePoint) ||
					!checkPointFallback(0, surfacePoint) ||
					!checkPointFallback((int)(.5 * biomeWidth), surfacePoint))
                {
                    fallbackPoint = surfacePoint;
                    continue;
                }
                Console.WriteLine("Crystal Caverns generation process finished in " + attempts + " attempts.");
				surfacePoint.Y = determineOriginY(biomeWidth, surfacePoint); // Correct the Y position of the biome to the average of the right and left bound's surrounding terrain height
                GenVars.structures.AddProtectedStructure(new Rectangle(surfacePoint.X - (int)(.5 * biomeWidth), surfacePoint.Y - surfaceHeight, biomeWidth, biomeHeight), 0);
                return surfacePoint;
            }
            Console.WriteLine("Could not find a suitable location to place the Crystal Caverns");
			if (fallbackPoint != Point.Zero)
			{
				Console.WriteLine("Falling back to a location overlapping with an evil biome to generate the Crystal Caverns");
                surfacePoint.Y = determineOriginY(biomeWidth, surfacePoint); // Correct the Y position of the biome to the average of the right and left bound's surrounding terrain height
                GenVars.structures.AddProtectedStructure(new Rectangle(surfacePoint.X - (int)(.5 * biomeWidth), surfacePoint.Y - surfaceHeight, biomeWidth, biomeHeight), 0);
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
            for (int attempts = 3; attempts < 12; attempts += 3) // This for loop is meant to solve corruption chasms dragging the average very far down
			{
                WorldUtils.Find(leftPoint, Searches.Chain(new Searches.Up(1000), new Conditions.IsSolid().AreaOr(1, 50).Not()), out leftPoint);
                leftPoint.Y += 50; // Adjust result to point to surface, not 50 tiles above 

                WorldUtils.Find(rightPoint, Searches.Chain(new Searches.Up(1000), new Conditions.IsSolid().AreaOr(1, 50).Not()), out rightPoint);
                rightPoint.Y += 50; // Adjust result to point to surface, not 50 tiles above 

				if (leftPoint.Y < (int)Main.worldSurface && rightPoint.Y < (int)Main.worldSurface)
				{
					break;
				}

                leftPoint = new Point(surfacePoint.X - xOffset + attempts, (int)Main.worldSurface);
                rightPoint = new Point(surfacePoint.X + xOffset + attempts, (int)Main.worldSurface);
            }
            return (leftPoint.Y + rightPoint.Y) / 2;
        }
	}
}