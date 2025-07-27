using AerovelenceMod.Common.Globals.Worlds;
using AerovelenceMod.Content.Biomes;
using AerovelenceMod.Content.Buffs;
using AerovelenceMod.Content.Items.Accessories.SmallAccessories;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace AerovelenceMod.Common.Globals.NPCs
{
	public class AeroGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		
		public bool SoulFire;
		public bool Electrified;
		public bool CrystalKunai;

		public override void ResetEffects(NPC npc)
		{
			SoulFire = false;
			Electrified = false;
			CrystalKunai = false;
		}

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (player != null && player.GetModPlayer<OpalOfCaVeaPlayer>().hasOpal)
            {
                player.AddBuff(ModContent.BuffType<Glory>(), 300);
            }
        }

        public override bool CheckDead(NPC npc)
        {
			return true;
		}

		public override void DrawEffects(NPC npc, ref Color drawColor)
		{
			
		}

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsSurfaceBiome>()) || spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsBiome>()))
			{
				pool[0] = 0f;
			}
        }
    }
}