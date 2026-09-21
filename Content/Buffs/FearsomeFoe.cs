using AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Buffs
{
    public class FearsomeFoeGNPC : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            NPC npc = new();
            if (npc.type == ModContent.NPCType<Cyvercry2>() || npc.type == ModContent.NPCType<CrystalTumbler>())
            {
                spawnRate = (int)(spawnRate * 10); //1/10 of normal spawn rate
                maxSpawns = (int)(maxSpawns * 0.5f); 
            }
        }
    }
}