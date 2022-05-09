using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Biomes;
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
			Main.npcFrameCount[NPC.type] = 5;
		}

		public override void SetDefaults()
		{
			NPC.width = NPCID.CaveBat;
			NPC.height = NPCID.CaveBat;
			NPC.damage = 15;
			NPC.defense = 10;
			NPC.lifeMax = 50;
			NPC.catchItem = (short)ModContent.ItemType<CrystalFishItem>();
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = .35f;
			NPC.aiStyle = 14;
			NPC.noGravity = true;
			NPC.npcSlots = 0;
			AIType = NPCID.CaveBat;
			AnimationType = NPCID.CaveBat;
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsBiome>()) ? .1f : 0f;

		public override void HitEffect(int hitDirection, double damage)
		{
			if (NPC.life <= 0)
			{
				NPC.position.X = NPC.position.X + (NPC.width / 2.0f);
				NPC.position.Y = NPC.position.Y + (NPC.height / 2.0f);
				NPC.width = 30;
				NPC.height = 30;
				NPC.position.X = NPC.position.X - (NPC.width / 2.0f);
				NPC.position.Y = NPC.position.Y - (NPC.height / 2.0f);
				for (int num621 = 0; num621 < 10; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 226, 0f, 0f, 100, new Color(112, 244, 250), 2f);
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
