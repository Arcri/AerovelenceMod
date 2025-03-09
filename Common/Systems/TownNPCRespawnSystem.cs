using System.IO;
using Terraria.ModLoader.IO;
using Terraria.ModLoader;
using Terraria;
using AerovelenceMod.Content.NPCs.TownNPC.RockCollector;

namespace AerovelenceMod.Common.Systems
{
    public class TownNPCRespawnSystem : ModSystem
    {
        public static bool unlockedRockCollectorSpawn = false;

        public override void ClearWorld() => unlockedRockCollectorSpawn = false;

        public override void SaveWorldData(TagCompound tag) => tag[nameof(unlockedRockCollectorSpawn)] = unlockedRockCollectorSpawn;

        public override void LoadWorldData(TagCompound tag)
        {
            unlockedRockCollectorSpawn = tag.GetBool(nameof(unlockedRockCollectorSpawn));
            unlockedRockCollectorSpawn |= NPC.AnyNPCs(ModContent.NPCType<RockCollector>());
        }

        public override void NetSend(BinaryWriter writer) => writer.WriteFlags(unlockedRockCollectorSpawn);
        
        public override void NetReceive(BinaryReader reader) => reader.ReadFlags(out unlockedRockCollectorSpawn);
    }
}