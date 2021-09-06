using System;
using System.Collections.Generic;
using System.Linq;

namespace AerovelenceMod.Common
{
    internal static class WeakReferences
	{
		public static class BossChecklistIntegration
		{
			private static readonly Version BossChecklistAPIVersion = new Version(1, 1); // Do not change this yourself.
			public class BossChecklistBossInfo
			{
				internal string key = "";
				internal string modSource = "";
				internal string internalName = "";
				internal string displayName = "";
				internal float progression = 0f;
				internal Func<bool> downed = () => false;
				internal bool isBoss = false;
				internal bool isMiniboss = false;
				internal bool isEvent = false;
				internal List<int> npcIDs = new List<int>();
				internal List<int> spawnItem = new List<int>();
				internal List<int> loot = new List<int>();
				internal List<int> collection = new List<int>();
			}


			public static readonly Dictionary<string, BossChecklistBossInfo> bossInfos = new Dictionary<string, BossChecklistBossInfo>();



			public static void UnloadBossChecklistIntegration()
			{
				bossInfos.Clear();
			}
			public static float DownedBossProgress()
			{
				if (bossInfos.Count == 0)
					return 0;
				return (float)bossInfos.Count(x => x.Value.downed()) / bossInfos.Count();
			}
			public static bool BossDowned(string bossKey) => bossInfos.TryGetValue(bossKey, out var bossInfo) && bossInfo.downed();
        }
	}
}