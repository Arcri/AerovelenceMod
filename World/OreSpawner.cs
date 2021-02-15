using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Terraria.GameContent.Generation;

namespace AerovelenceMod.World
{
    public class OreSpawner : ModWorld
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            if (ShiniesIndex == -1)
            {
                return;
            }
            tasks.Insert(ShiniesIndex + 1, new PassLegacy("Filling the caves with glimmer", delegate (GenerationProgress progress)
            {
                progress.Message = "Filling the caves with glimmer";
                //Put your custom tile block name
                for (int k = (int)(Main.maxTilesX * Main.maxTilesY * 15E-05) - 1; k >= 0; k--)                                                                                                                               //      |
                {                                                                                                                                                                                                                         //       |
                    WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)WorldGen.rockLayer, Main.maxTilesY), (double)WorldGen.genRand.Next(3, 15), WorldGen.genRand.Next(2, 15), mod.TileType("SlateOreBlock"), false, 0f, 0f, false, true);
                }
            }));
        }
    }
}
