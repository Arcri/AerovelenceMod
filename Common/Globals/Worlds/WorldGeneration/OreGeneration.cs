using System.Collections.Generic;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Tiles.Ores;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;

namespace AerovelenceMod.Common.Globals.Worlds.WorldGeneration
{
    public class OreGeneration : ModWorld
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int shiniesIndex = tasks.FindIndex(i => i.Name.Equals("Shinies"));
            tasks.TryInsert(shiniesIndex, new PassLegacy("Aerovelence Ores", AerovelenceOres));
        }

        private void AerovelenceOres(GenerationProgress progress)
        {
            int maxTiles = Main.maxTilesX * Main.maxTilesY;
            int oreAmount = (int)(maxTiles * 0.00010);

            progress.Message = "Slate Ore";

            oreAmount = (int) (maxTiles * 0.00015);
            for (int i = 0; i < oreAmount; i++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)WorldGen.rockLayerLow, Main.maxTilesY);

                WorldGen.OreRunner(x, y, WorldGen.genRand.Next(3, 15), WorldGen.genRand.Next(2, 15),
                    (ushort)ModContent.TileType<SlateOreBlock>());
            }
        }
    }
}
