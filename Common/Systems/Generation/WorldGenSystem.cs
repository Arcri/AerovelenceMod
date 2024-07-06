using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Generation;
using System.Collections.Generic;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.Localization;
using AerovelenceMod.Common.Systems.Generation.CrystalCaverns;

namespace AerovelenceMod.Common.Systems.Generation
{
    public class WorldGenSystem : ModSystem
    {
        public static LocalizedText CrystalCavernsTerrainPassMessage { get; private set; }

		public override void SetStaticDefaults()
		{
			CrystalCavernsTerrainPassMessage = Language.GetOrRegister(Mod.GetLocalizationKey($"WorldGen.{nameof(CrystalCavernsTerrainPassMessage)}"));
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			int CCTerrainIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Chests"));
			if (CCTerrainIndex != -1)
			{
				tasks.Insert(CCTerrainIndex + 1, new CrystalCavernsTerrainPass("Crystal Caverns Terrain", 100f));
			}
		}
	}
}
