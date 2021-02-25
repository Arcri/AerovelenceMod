using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.CrystalTorrent
{
    public class CrystalTorrentNPC : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (CrystalTorrentWorld.CrystalTorrentUp && (Main.invasionX == (double)Main.spawnTileX))
            {
                pool.Clear();
                foreach (int i in CrystalTorrentInvasion.invaders)
                {
                    pool.Add(i, 1f);
                }
            }
        }
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (CrystalTorrentWorld.CrystalTorrentUp && (Main.invasionX == Main.spawnTileX))
            {
                spawnRate = 100;
                maxSpawns = 10000;
            }
        }
        public override void PostAI(NPC npc)
        {
            if (CrystalTorrentWorld.CrystalTorrentUp && (Main.invasionX == Main.spawnTileX))
            {
                npc.timeLeft = 1000;
            }
        }
        public override void NPCLoot(NPC npc)
        {
            if (CrystalTorrentWorld.CrystalTorrentUp)
            {
                foreach (int invader in CrystalTorrentInvasion.invaders)
                {
                    if (npc.type == invader)
                    {
                        Main.invasionSize -= 1;
                    }
                }
            }
        }
    }
}