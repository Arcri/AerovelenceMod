using System;
using System.IO;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Events.DarkNight;
using AerovelenceMod.Content.Projectiles.NPCs.CrystalCaverns;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.NPCs.General.DarkNight
{
	public class DarkStalker : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Stalker");
			Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.GraniteGolem];
		}
        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 50;
            NPC.aiStyle = 3;
            NPC.damage = 30;
            NPC.defense = 18;
            NPC.lifeMax = 200;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.knockBackResist = 0.35f;
            NPC.lavaImmune = true;
            NPC.value = Item.buyPrice(0, 1, 0, 0);
            NPC.buffImmune[20] = true;
            NPC.buffImmune[24] = true;
			animationType = 0;
        }

		public override void HitEffect(int hitDirection, double damage)
		{
			if (NPC.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>(), NPC.velocity.X, NPC.velocity.Y, 0, Color.White, 1);
				}
				Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/LuminousDefenderGore1"), 1f);
				Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/LuminousDefenderGore2"), 1f);
				Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/LuminousDefenderGore3"), 1f);
				Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/LuminousDefenderGore4"), 1f);
				Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/LuminousDefenderGore5"), 1f);

			}
		}
		public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(ai);
			writer.Write(defending);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
			ai = reader.ReadSingle();
			defending = reader.ReadBoolean();
        }
        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
			if(defending)
			{
				if (delayBetween <= 0)
					FireShards(player.whoAmI);
				damage -= 20;
				if (damage < 2)
					damage = 1;
				NPC.netUpdate = true;
			};
			NPC.netUpdate = true;
		}
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (defending)
			{
				if(delayBetween <= 0)
					FireShards(projectile.owner);
				damage -= 20;
				if (damage < 2)
					damage = 1;
				NPC.netUpdate = true;
			}
        }
		public void FireShards(int player)
        {
			NPC.netUpdate = true;
			SoundEngine.PlaySound(SoundID.Item, (int)NPC.Center.X, (int)NPC.Center.Y, 101, 1.1f);
			delayBetween = 45;
			for(int i = 0; i < 3; i++)
            {
				if(Main.netMode != NetmodeID.MultiplayerClient)
				{
					int damage2 = NPC.damage / 2;
					if (Main.expertMode)
					{
						damage2 = (int)(damage2 / Main.expertDamage);
						NPC.netUpdate = true;
					}
					Projectile.NewProjectile(new Vector2(NPC.Center.X, NPC.position.Y), new Vector2(0, -Main.rand.NextFloat(5, 7f)).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-10, 10) + (-20 + 20 * i))), ModContent.ProjectileType<LuminousShard>(), damage2, 3, Main.myPlayer, player);
					NPC.netUpdate = true;
				}
            }
        }
		bool defending = false;
        float ai = 0;
		float delayBetween = 0;
        public override bool PreAI()
        {
			NPC.TargetClosest(true);
			int untilImmune = 300;
			int immuneTimeLength = 120;
			defending = false; 
			if(delayBetween > 0)
				delayBetween--;
			NPC.netUpdate = true;
			if (ai < 0f)
			{
				if(ai == -immuneTimeLength)
                {
					for (int i = 0; i < 360; i += 12)
					{
						Vector2 circular = new Vector2(96, 0).RotatedBy(MathHelper.ToRadians(i));
						Dust dust2 = Dust.NewDustDirect(NPC.Center - new Vector2(5) + circular, 0, 0, ModContent.DustType<WispDust>(), 0, 0, NPC.alpha);
						dust2.velocity *= 0.15f;
						dust2.velocity += -circular * 0.08f;
						dust2.scale = 2.25f;
						dust2.noGravity = true;
						NPC.netUpdate = true;
					}
				}
				if(ai >= -immuneTimeLength + 20)
				{
					defending = true;
				}
				ai += 1f;
                NPC.velocity.X *= 0.9f;
				NPC.netUpdate = true;
				if (Math.Abs(NPC.velocity.X) < 0.001)
				{
                    NPC.velocity.X = 0.001f * NPC.direction;
					NPC.netUpdate = true;
				}
				if (Math.Abs(NPC.velocity.Y) > 1f)
				{
					ai += 10f;
				}
				if (ai >= 0f)
				{
					NPC.netUpdate = true;
                    NPC.velocity.X += NPC.direction * 0.3f;
					NPC.netUpdate = true;
				}
				return false;
			}
			if (ai < untilImmune)
			{
				if (NPC.justHit)
				{
					ai += 15f; //increase immune timer by 15 when hit
				}
				ai += 1f; //increases immune timer rapidly, 60 / second
				NPC.netUpdate = true;
			}
			else if (Math.Abs(NPC.velocity.Y) <= 0.1f)
			{
				ai = -immuneTimeLength;
				NPC.netUpdate = true;
			}
			return true;
		}
        public override void FindFrame(int frameHeight)
        {
			if (NPC.velocity.Y == 0f)
			{
				if (NPC.direction == 1)
				{
					NPC.spriteDirection = 1;
				}
				if (NPC.direction == -1)
				{
					NPC.spriteDirection = -1;
				}
				if (ai < 0f)
				{
					NPC.frameCounter += 1.0;
					if (NPC.frameCounter > 3.0)
					{
						NPC.frame.Y += frameHeight;
						NPC.frameCounter = 0.0;
					}
					if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
					{
						NPC.frame.Y = frameHeight * 11;
					}
					else if (NPC.frame.Y < frameHeight * 11)
					{
						NPC.frame.Y = frameHeight * 11;
					}
				}
				else if (NPC.velocity.X == 0f)
				{
					NPC.frameCounter += 1.0;
					NPC.frame.Y = 0;
				}
				else
				{
					NPC.frameCounter += 0.2f + Math.Abs(NPC.velocity.X);
					if (NPC.frameCounter > 8.0)
					{
						NPC.frame.Y += frameHeight;
						NPC.frameCounter = 0.0;
					}
					if (NPC.frame.Y / frameHeight >= Main.npcFrameCount[NPC.type] - 10)
					{
						NPC.frame.Y = frameHeight * 2;
					}
					else if (NPC.frame.Y / frameHeight < 2)
					{
						NPC.frame.Y = frameHeight * 2;
					}
				}
			}
			else
			{
				NPC.frameCounter = 0.0;
				NPC.frame.Y = frameHeight;
				NPC.netUpdate = true;
			}
			base.FindFrame(frameHeight);
        }
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return DarkNightWorld.DarkNight && spawnInfo.Player.ZoneOverworldHeight ? 1f : 0f;
		}
	}
}