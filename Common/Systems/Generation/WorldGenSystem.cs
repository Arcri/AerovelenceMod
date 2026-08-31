using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.WorldBuilding;
using Terraria.Localization;
using AerovelenceMod.Common.Systems.Generation.CrystalCaverns;

namespace AerovelenceMod.Common.Systems.Generation
{
    public class WorldGenSystem : ModSystem
    {
        public static LocalizedText CrystalCavernsTerrainPassMessage { get; private set; }
        public static LocalizedText CrystalCavernsStructurePassMessage { get; private set; }
		public static LocalizedText CrystalCavernsRubblePassMessage { get; private set; }

        public override void SetStaticDefaults()
		{
			CrystalCavernsTerrainPassMessage = Terraria.Localization.Language.GetOrRegister(Mod.GetLocalizationKey($"WorldGen.{nameof(CrystalCavernsTerrainPassMessage)}"));
            CrystalCavernsStructurePassMessage = Terraria.Localization.Language.GetOrRegister(Mod.GetLocalizationKey($"WorldGen.{nameof(CrystalCavernsStructurePassMessage)}"));
			CrystalCavernsRubblePassMessage = Terraria.Localization.Language.GetOrRegister(Mod.GetLocalizationKey($"WorldGen.{nameof(CrystalCavernsRubblePassMessage)}"));
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			int CCTerrainIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Chests"));
			if (CCTerrainIndex != -1)
			{
				tasks.Insert(CCTerrainIndex + 1, CCTerrainPass.Instance("Crystal Caverns Terrain", 100f));
			}
			int CCPolishIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
			if (CCPolishIndex != -1)
			{
				tasks.Insert(CCPolishIndex + 1, new CCStructurePass("Crystal Caverns Polish", 101f));
			}
			int CCRubbleIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Tile Cleanup"));
            if (CCRubbleIndex != -1)
			{
				tasks.Insert(CCRubbleIndex + 1, new CCRubblePass("Crystal Caverns Rubble", 102f));
			}
        }
	}
}
