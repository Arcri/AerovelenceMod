using AerovelenceMod.Common.Systems.Generation.GenUtils;
using AerovelenceMod.Content.Walls.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AerovelenceMod.Common.Systems.Generation.CrystalCaverns
{
    public class CrystalCavernsPolish : GenPass
    {
        public CrystalCavernsPolish(string name, double loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = WorldGenSystem.CrystalCavernsTerrainPassMessage.Value;
            CrystalCavernsTerrainPass mainPass = CrystalCavernsTerrainPass.Instance();

            Point surfaceRectOrigin = new Point(mainPass.Origin.X - mainPass.BiomeWidth / 2, mainPass.Origin.Y - (int)(mainPass.SurfaceHeight * 1.75));
            ShapeData surfaceRectShapeData = new ShapeData();
            WorldUtils.Gen(surfaceRectOrigin, new Shapes.Rectangle(mainPass.BiomeWidth, (int)(mainPass.SurfaceHeight * 1.75)), new Actions.Blank().Output(surfaceRectShapeData));

            WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
            {
                new Modifiers.OnlyWalls(WallID.DirtUnsafe, WallID.FlowerUnsafe, WallID.GrassUnsafe, 59, WallID.SnowWallUnsafe, WallID.Sandstone, WallID.HardenedSand),
                new Actions.PlaceWall(mainPass.DirtWall)
            }));
            WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
            {
                new Modifiers.Expand(3, 0),
                new Modifiers.Dither(0.75),
                new Modifiers.OnlyWalls(WallID.DirtUnsafe, WallID.FlowerUnsafe, WallID.GrassUnsafe, 59, WallID.SnowWallUnsafe, WallID.Sandstone, WallID.HardenedSand),
                new Actions.PlaceWall(mainPass.DirtWall)
            }));
            WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
            {
                new Modifiers.Expand(5, 0),
                new Modifiers.Dither(0.75),
                new Modifiers.OnlyWalls(WallID.DirtUnsafe, WallID.FlowerUnsafe, WallID.GrassUnsafe, 59, WallID.SnowWallUnsafe, WallID.Sandstone, WallID.HardenedSand),
                new Actions.PlaceWall(mainPass.DirtWall),
            }));
            
        }
    }
}
