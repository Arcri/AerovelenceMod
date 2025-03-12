using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Systems.Language
{
    public static class BestiaryRefresher
    {
        private static List<int> pendingRefreshQueue = [];
        private static bool isRefreshing = false;

        public static void ForceRebuildNPCBestiaryEntries()
        {
            if (isRefreshing)
                return;
            isRefreshing = true;
            pendingRefreshQueue.Clear();
            foreach (var modNpc in ModContent.GetContent<ModNPC>())
            {
                if (modNpc is TranslatableModNPC translatable)
                    pendingRefreshQueue.Add(modNpc.Type);
            }
            ProcessNextBatch();
        }

        private static void ProcessNextBatch()
        {
            const int BATCH_SIZE = 10;
            ModContent.GetInstance<AerovelenceMod>().Logger.Debug($"[BestiaryRefresher] Processing next batch. {pendingRefreshQueue.Count} NPCs remaining.");
            if (pendingRefreshQueue.Count == 0)
            {
                isRefreshing = false;
                return;
            }
            int count = 0;
            List<int> processed = [];
            foreach (int npcType in pendingRefreshQueue)
            {
                var entry = Main.BestiaryDB.FindEntryByNPCID(npcType);
                if (entry == null)
                {
                    processed.Add(npcType);
                    continue;
                }
                ModNPC modNpc = ModContent.GetModNPC(npcType);
                if (modNpc is TranslatableModNPC translatable)
                {
                    ModContent.GetInstance<AerovelenceMod>().Logger.Debug($"Refreshing {modNpc.Name} Bestiary");
                    translatable.ClearCache();
                    translatable.RegisterWithLanguageSystem();
                    for (int i = entry.Info.Count - 1; i >= 0; i--)
                    {
                        if (entry.Info[i] is FlavorTextBestiaryInfoElement || entry.Info[i] is NamePlateInfoElement)
                            entry.Info.RemoveAt(i);
                    }
                    modNpc.SetBestiary(Main.BestiaryDB, entry);
                }
                processed.Add(npcType);
                count++;
                if (count >= BATCH_SIZE)
                    break;
            }
            foreach (int npcType in processed)
                pendingRefreshQueue.Remove(npcType);
            Main.QueueMainThreadAction(() => ProcessNextBatch());
        }
    }
}