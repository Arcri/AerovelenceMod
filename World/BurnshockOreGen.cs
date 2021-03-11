using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AerovelenceMod.World
{
    public class BurnshockOreGen : ModWorld
    {
        private const int saveVersion = 0;

        public static bool spawnBurnshock;

        public override void Initialize()
        {
            spawnBurnshock = false;
        }

        public override TagCompound Save()
        {
            var spawned = new List<string>();
            if (spawnBurnshock)
            {
                spawned.Add("burnshock");
            }

            return new TagCompound
            {
                ["spawned"] = spawned
            };
        }

        public override void Load(TagCompound tag)
        {
            var spawned = tag.GetList<string>("spawned");
            spawnBurnshock = spawned.Contains("phantic");
        }
    }
}