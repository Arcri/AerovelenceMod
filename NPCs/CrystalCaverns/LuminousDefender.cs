using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.CrystalCaverns
{
    public class LuminousDefender : ModNPC
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Luminous Defender");
			Main.npcFrameCount[npc.type] = 20;
		}
        public override void SetDefaults()
        {
            npc.width = 48;
            npc.height = 50;
            npc.aiStyle = 3;
            npc.damage = 30;
            npc.defense = 18;
            npc.lifeMax = 200;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath44;
            npc.knockBackResist = 0.35f;
            npc.lavaImmune = true;
            npc.value = Item.buyPrice(0, 0, 4, 0);
            npc.buffImmune[20] = true;
            npc.buffImmune[24] = true;
        }

        public override void AI()
        {
			int num = 200;
			{
				int num8 = 300;
				int num9 = 120;
				npc.dontTakeDamage = false;
				if (npc.ai[2] < 0f)
				{
					npc.dontTakeDamage = true;
					npc.ai[2] += 1f;
                    npc.velocity.X *= 0.9f;
					if (Math.Abs(npc.velocity.X) < 0.001)
					{
                        npc.velocity.X = 0.001f * npc.direction;
					}
					if (Math.Abs(npc.velocity.Y) > 1f)
					{
						npc.ai[2] += 10f;
					}
					if (npc.ai[2] >= 0f)
					{
						npc.netUpdate = true;
                        npc.velocity.X += npc.direction * 0.3f;
					}
					return;
				}
				if (npc.ai[2] < num8)
				{
					if (npc.justHit)
					{
						npc.ai[2] += 15f;
					}
					npc.ai[2] += 1f;
				}
				else if (npc.velocity.Y == 0f)
				{
					npc.ai[2] = -num9;
					npc.netUpdate = true;
				}
			}
			{
				if (npc.velocity.Y == 0f)
				{
					if (npc.direction == 1)
					{
						npc.spriteDirection = 1;
					}
					if (npc.direction == -1)
					{
						npc.spriteDirection = -1;
					}
					if (npc.ai[2] < 0f)
					{
						npc.frameCounter += 1.0;
						if (npc.frameCounter > 3.0)
						{
							npc.frame.Y += num;
							npc.frameCounter = 0.0;
						}
						if (npc.frame.Y >= Main.npcFrameCount[npc.type] * num)
						{
							npc.frame.Y = num * 11;
						}
						else if (npc.frame.Y < num * 11)
						{
							npc.frame.Y = num * 11;
						}
					}
					else if (npc.velocity.X == 0f)
					{
						npc.frameCounter += 1.0;
						npc.frame.Y = 0;
					}
					else
					{
						npc.frameCounter += 0.2f + Math.Abs(npc.velocity.X);
						if (npc.frameCounter > 8.0)
						{
							npc.frame.Y += num;
							npc.frameCounter = 0.0;
						}
						if (npc.frame.Y / num >= Main.npcFrameCount[npc.type] - 10)
						{
							npc.frame.Y = num * 2;
						}
						else if (npc.frame.Y / num < 2)
						{
							npc.frame.Y = num * 2;
						}
					}
				}
				else
				{
					npc.frameCounter = 0.0;
					npc.frame.Y = num;
				}
			}
		}
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.GetModPlayer<AeroPlayer>().ZoneCrystalCaverns ? .1f : 0f;
        }
    }
}