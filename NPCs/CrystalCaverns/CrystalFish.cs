using AerovelenceMod.Items.Equipement;
using AerovelenceMod.Items.Others.Alchemical;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.CrystalCaverns
{
	//CrystalFish - the modnpc
	//CrystalFishItem - critter item
	//FishRing - summoning item
	//LightningFish - pet proj.
	//lightningFishBuff - pet buff
	public class CrystalFish : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Electric Tetra");
			Main.npcFrameCount[npc.type] = 6;
			Main.npcCatchable[npc.type] = true;
		}

		public override void SetDefaults()
		{
			npc.width = 28;
			npc.height = 22;
			npc.damage = 0;
			npc.defense = 0;
			npc.lifeMax = 5;
			npc.catchItem = (short)ModContent.ItemType<CrystalFishItem>();
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.knockBackResist = .35f;
			npc.aiStyle = 16;
			npc.noGravity = true;
			npc.npcSlots = 0;
			aiType = NPCID.Goldfish;
			npc.dontCountMe = true;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			var effects = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(Main.npcTexture[npc.type], npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame,
							 drawColor, npc.rotation, npc.frame.Size() / 2, npc.scale, effects, 0);
			return false;
		}
		public override bool PreAI()
		{
			npc.spriteDirection = npc.direction;

			return true;
		}
		public override void AI()
		{
			Player player = Main.player[npc.target];
			{
				Player target = Main.player[npc.target];
				int distance = (int)Math.Sqrt((npc.Center.X - target.Center.X) * (npc.Center.X - target.Center.X) + (npc.Center.Y - target.Center.Y) * (npc.Center.Y - target.Center.Y));
				if (distance < 65 && target.wet && npc.wet)
				{
					Vector2 vel = npc.DirectionFrom(target.Center);
					vel.Normalize();
					vel *= 4.5f;
					npc.velocity = vel;
					npc.rotation = npc.velocity.X * .06f;
					if (target.position.X > npc.position.X)
					{
						npc.spriteDirection = -1;
						npc.direction = -1;
						npc.netUpdate = true;
					}
					else if (target.position.X < npc.position.X)
					{
						npc.spriteDirection = 1;
						npc.direction = 1;
						npc.netUpdate = true;
					}
				}

			}
		}
		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter += 0.15f;
			npc.frameCounter %= Main.npcFrameCount[npc.type];
			int frame = (int)npc.frameCounter;
			npc.frame.Y = frame * frameHeight;
		}

        public override void OnCatchNPC(Player player, Item item)
		{
			if (!Main.expertMode)
            {
				if (Main.rand.Next(500) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<FishRing>());
				}
			}

			if (Main.expertMode)
			{
				if (Main.rand.Next(400) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<FishRing>());
				}
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return spawnInfo.player.GetModPlayer<AeroPlayer>().ZoneCrystalCaverns && spawnInfo.water ? 0.16f : 0f;
		}
		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CrystalFishHead"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CrystalFishTail"), 1f);
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
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
			}
		}
	}
}
