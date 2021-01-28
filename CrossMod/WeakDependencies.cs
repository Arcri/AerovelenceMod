/*using AerovelenceMod.Items.Armor.Vanity;
using AerovelenceMod.Items.BossBags;
using AerovelenceMod.Items.BossSummons;
using AerovelenceMod.Items.Placeable.Trophies;
using AerovelenceMod.Items.Weapons.Magic;
using AerovelenceMod.Items.Weapons.Melee;
using AerovelenceMod.Items.Weapons.Ranged;
using AerovelenceMod.Items.Weapons.Thrown;
using AerovelenceMod.NPCs.Bosses.CrystalTumbler;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace AerovelenceMod.CrossMod
{
    internal class WeakReferences
    {
        public static void PerformModSupport()
        {
            PerformBossChecklistSupport();
        }

        private static void PerformBossChecklistSupport()
        {
            Mod mod = ModLoader.GetMod("BossChecklist");
            Aerovelence.AeroMod inst = ModContent.GetInstance<Aerovelence.AeroMod>();
            if (mod != null)
            {
                mod.Call(new object[14]
                {
             "AddBoss",
              6.25f,
              ModContent.NPCType<CrystalTumbler>(),
              inst,
              "Crystal Tumbler",
              (Func<bool>)(() => AeroWorld.downedCrystalTumbler),
              ModContent.ItemType<AncientGeode>(),
              new List<int>
              {
                  ModContent.ItemType<CrystalTumblerTrophy>(),
                  ModContent.ItemType<CrystalTumblerMask>(),
              },
              new List<int>
              {
                  ModContent.ItemType<CrystalTumblerBag>(),
                  ModContent.ItemType<DiamondDuster>(),
                  ModContent.ItemType<DarkCrystalStaff>(),
                  ModContent.ItemType<CavernMauler>(),
                  ModContent.ItemType<CavernousImpaler>(),
                  ModContent.ItemType<PrismThrasher>(),
                  ModContent.ItemType<CrystallineQuadshot>(),
                  ModContent.ItemType<PrismPiercer>(),
                  ModContent.ItemType<PrismaticPulsar>()
              },
              "Use a [i:" + ModContent.ItemType<AncientGeode>() + "] in the Crystal Caverns.",
              null,
              "AerovelenceMod/CrossMod/BossChecklist/CrystalTumbler",
              "AerovelenceMod/NPCs/Bosses/InfectedEye/CrystalTumbler_Head_Boss",
              (Func<bool>)(() => AeroWorld.downedCrystalTumbler)
                 });
            }
        }
    }
}*/