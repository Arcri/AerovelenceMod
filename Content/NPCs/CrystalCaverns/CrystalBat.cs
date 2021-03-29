using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Items.Others.Alchemical;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
	public class CrystalBat : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crystal Bat");
			Main.npcFrameCount[npc.type] = 5;
		}

		public override void SetDefaults()
		{
			npc.width = NPCID.CaveBat;
			npc.height = NPCID.CaveBat;
			npc.damage = 15;
			npc.defense = 10;
			npc.lifeMax = 50;
			npc.catchItem = (short)ModContent.ItemType<CrystalFishItem>();
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.knockBackResist = .35f;
			npc.aiStyle = 14;
			npc.noGravity = true;
			npc.npcSlots = 0;
			aiType = NPCID.CaveBat;
			animationType = NPCID.CaveBat;
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns ? .1f : 0f;

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (npc.width / 2.0f);
				npc.position.Y = npc.position.Y + (npc.height / 2.0f);
				npc.width = 30;
				npc.height = 30;
				npc.position.X = npc.position.X - (npc.width / 2.0f);
				npc.position.Y = npc.position.Y - (npc.height / 2.0f);
				for (int num621 = 0; num621 < 10; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 226, 0f, 0f, 100, new Color(112, 244, 250), 2f);
					Main.dust[num622].velocity *= 1f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.3f;
						Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
					}
				}
			}
		}
	}
}
