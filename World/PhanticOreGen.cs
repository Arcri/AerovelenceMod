using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AerovelenceMod.World
{
    public class PhanticOreGen : ModWorld
    {
        private const int saveVersion = 0;

        public static bool spawnPhantic;

        public override void Initialize()
        {
            spawnPhantic = false;
        }

        public override TagCompound Save()
        {
            var spawned = new List<string>();
            if (spawnPhantic)
            {
                spawned.Add("phantic");
            }

            return new TagCompound
            {
                ["spawned"] = spawned
            };
        }

        public override void Load(TagCompound tag)
        {
            var spawned = tag.GetList<string>("spawned");
            spawnPhantic = spawned.Contains("phantic");
        }
    }
}