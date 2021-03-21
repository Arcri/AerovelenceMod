using AerovelenceMod.Content.Items.Accessories;
using AerovelenceMod.Content.Items.BossSummons;
using AerovelenceMod.Content.Items.Weapons.Magic;
using AerovelenceMod.Content.Items.Weapons.Melee;
using AerovelenceMod.Content.Items.Weapons.Ranged;
using AerovelenceMod.Content.Items.Weapons.Thrown;
using System;
using System.Collections.Generic;
using AerovelenceMod.Common.Globals.Worlds;
using Terraria.ModLoader;

namespace AerovelenceMod.Common
{
    internal static class WeakReferences
    {
        public static void SetupModSupport() => SetupBossChecklistSupport();

        // TODO - Isnt this method of adding bosses obsolete?
        private static void SetupBossChecklistSupport()
        {
            Mod bossChecklist = ModLoader.GetMod("BossChecklist");
            if (bossChecklist != null)
            {

                bossChecklist.Call
                (
                    "AddBossWithInfo",
                    "Crystal Tumbler",
                    1.5f,
                    (Func<bool>)(() => DownedWorld.DownedCrystalTumbler),
                    "Use a [i:" + ModContent.ItemType<LargeGeode>() + "] in the Crystal Caverns"
                );
                
                bossChecklist.Call
                (
                    "AddBossWithInfo", 
                    "Rimegeist", 
                    5.5f, 
                    (Func<bool>)(() => DownedWorld.DownedRimegeist), 
                    "Use a [i:" + ModContent.ItemType<GlowingSnow>() + "] at night in the snow biome"
                );
                
                bossChecklist.Call
                (
                    "AddBossWithInfo", 
                    "LightningMoth", 
                    6.5f, 
                    (Func<bool>)(() => DownedWorld.DownedRimegeist), 
                    "Use a [i:" + ModContent.ItemType<GlowingSnow>() + "] at night in the Crystal Fields"
                );
                
                bossChecklist.Call
                (
                    "AddBossWithInfo", 
                    "Cyvercry",
                    9.5f, 
                    (Func<bool>)(() => DownedWorld.DownedCyvercry), 
                    "Use a [i:" + ModContent.ItemType<ObsidianEye>() + "] at night"
                );
                
                bossChecklist.Call
                    (
                    "AddBossWithInfo", 
                    "TheFallen", 
                    12.5f, 
                    (Func<bool>)(() => DownedWorld.DownedCyvercry), 
                    "Use a [i:" + ModContent.ItemType<ObsidianEye>() + "] in the sky"
                    );

                bossChecklist.Call
                (
                    "AddToBossLoot", 
                    "AerovelenceMod", 
                    "Crystal Tumbler",
                    new List<int> { ModContent.ItemType<DiamondDuster>(), ModContent.ItemType<DarkCrystalStaff>(), 
                        ModContent.ItemType<CavernMauler>(), ModContent.ItemType<CavernousImpaler>(), ModContent.ItemType<PrismThrasher>(), 
                        ModContent.ItemType<CrystallineQuadshot>(), ModContent.ItemType<PrismPiercer>(), ModContent.ItemType<PrismaticSoul>()
                });

                bossChecklist.Call
                (
                    "AddToBossSpawnItems", 
                    "AerovelenceMod", 
                    "Crystal Tumbler", 
                    new List<int> { ModContent.ItemType<LargeGeode>()
                });
            }
        }
    }
}
