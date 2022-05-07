using System;
using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Items.Equipment;
using AerovelenceMod.Content.Items.Others.Alchemical;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
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
			Main.npcFrameCount[NPC.type] = 6;
			Main.npcCatchable[NPC.type] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 28;
			NPC.height = 22;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.lifeMax = 5;
			NPC.catchItem = (short)ModContent.ItemType<CrystalFishItem>();
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = .35f;
			NPC.aiStyle = 16;
			NPC.noGravity = true;
			NPC.npcSlots = 0;
			AIType = NPCID.Goldfish;
			NPC.dontCountMe = true;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			Main.EntitySpriteDraw((Texture2D)TextureAssets.Npc[NPC.type], NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame,
							 drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			return false;
		}
		public override bool PreAI()
		{
			NPC.spriteDirection = NPC.direction;

			return true;
		}
		public override void AI()
		{
			Player player = Main.player[NPC.target];
			{
				Player target = Main.player[NPC.target];
				int distance = (int)Math.Sqrt((NPC.Center.X - target.Center.X) * (NPC.Center.X - target.Center.X) + (NPC.Center.Y - target.Center.Y) * (NPC.Center.Y - target.Center.Y));
				if (distance < 65 && target.wet && NPC.wet)
				{
					Vector2 vel = NPC.DirectionFrom(target.Center);
					vel.Normalize();
					vel *= 4.5f;
					NPC.velocity = vel;
					NPC.rotation = NPC.velocity.X * .06f;
					if (target.position.X > NPC.position.X)
					{
						NPC.spriteDirection = -1;
						NPC.direction = -1;
						NPC.netUpdate = true;
					}
					else if (target.position.X < NPC.position.X)
					{
						NPC.spriteDirection = 1;
						NPC.direction = 1;
						NPC.netUpdate = true;
					}
				}

			}
		}
		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.15f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

        public override void OnCatchNPC(Player player, Item item)
		{
			var source = NPC.GetSource_FromAI();
			if (!Main.expertMode)
            {
				if (Main.rand.Next(500) == 0)
				{
					Item.NewItem(source, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<FishRing>());
				}
			}

			if (Main.expertMode)
			{
				if (Main.rand.Next(400) == 0)
				{
					Item.NewItem(source, (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<FishRing>());
				}
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) =>
            spawnInfo.Player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns && spawnInfo.Water ? .2f : 0f;

		public override void HitEffect(int hitDirection, double damage)
		{
			var source = NPC.GetSource_Death();
			if (NPC.life <= 0)
			{
				Gore.NewGore(source, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/CrystalFishHead").Type);
				Gore.NewGore(source, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/CrystalFishTail").Type);
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
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
			}
		}
	}
}
