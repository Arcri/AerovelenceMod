using AerovelenceMod.Dusts;
using AerovelenceMod.Projectiles.NPCs.CrystalCaverns;
using Microsoft.Xna.Framework;
using System;
using System.IO;
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
			Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.GraniteGolem];
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
            npc.value = Item.buyPrice(0, 1, 0, 0);
            npc.buffImmune[20] = true;
            npc.buffImmune[24] = true;
			animationType = 0;
        }

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Sparkle>(), npc.velocity.X, npc.velocity.Y, 0, Color.White, 1);
				}
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/LuminousDefenderGore1"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/LuminousDefenderGore2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/LuminousDefenderGore3"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/LuminousDefenderGore4"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/LuminousDefenderGore5"), 1f);

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
				npc.netUpdate = true;
			};
			npc.netUpdate = true;
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
				npc.netUpdate = true;
			}
        }
		public void FireShards(int player)
        {
			npc.netUpdate = true;
			Main.PlaySound(SoundID.Item, (int)npc.Center.X, (int)npc.Center.Y, 101, 1.1f);
			delayBetween = 45;
			for(int i = 0; i < 3; i++)
            {
				if(Main.netMode != NetmodeID.MultiplayerClient)
				{
					int damage2 = npc.damage / 2;
					if (Main.expertMode)
					{
						damage2 = (int)(damage2 / Main.expertDamage);
						npc.netUpdate = true;
					}
					Projectile.NewProjectile(new Vector2(npc.Center.X, npc.position.Y), new Vector2(0, -Main.rand.NextFloat(5, 7f)).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-10, 10) + (-20 + 20 * i))), ModContent.ProjectileType<LuminousShard>(), damage2, 3, Main.myPlayer, player);
					npc.netUpdate = true;
				}
            }
        }
		bool defending = false;
        float ai = 0;
		float delayBetween = 0;
        public override bool PreAI()
        {
			npc.TargetClosest(true);
			int untilImmune = 300;
			int immuneTimeLength = 120;
			defending = false; 
			if(delayBetween > 0)
				delayBetween--;
			npc.netUpdate = true;
			if (ai < 0f)
			{
				if(ai == -immuneTimeLength)
                {
					for (int i = 0; i < 360; i += 12)
					{
						Vector2 circular = new Vector2(96, 0).RotatedBy(MathHelper.ToRadians(i));
						Dust dust2 = Dust.NewDustDirect(npc.Center - new Vector2(5) + circular, 0, 0, ModContent.DustType<WispDust>(), 0, 0, npc.alpha);
						dust2.velocity *= 0.15f;
						dust2.velocity += -circular * 0.08f;
						dust2.scale = 2.25f;
						dust2.noGravity = true;
						npc.netUpdate = true;
					}
				}
				if(ai >= -immuneTimeLength + 20)
				{
					defending = true;
				}
				ai += 1f;
                npc.velocity.X *= 0.9f;
				npc.netUpdate = true;
				if (Math.Abs(npc.velocity.X) < 0.001)
				{
                    npc.velocity.X = 0.001f * npc.direction;
					npc.netUpdate = true;
				}
				if (Math.Abs(npc.velocity.Y) > 1f)
				{
					ai += 10f;
				}
				if (ai >= 0f)
				{
					npc.netUpdate = true;
                    npc.velocity.X += npc.direction * 0.3f;
					npc.netUpdate = true;
				}
				return false;
			}
			if (ai < untilImmune)
			{
				if (npc.justHit)
				{
					ai += 15f; //increase immune timer by 15 when hit
				}
				ai += 1f; //increases immune timer rapidly, 60 / second
				npc.netUpdate = true;
			}
			else if (Math.Abs(npc.velocity.Y) <= 0.1f)
			{
				ai = -immuneTimeLength;
				npc.netUpdate = true;
			}
			return true;
		}
        public override void FindFrame(int frameHeight)
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
				if (ai < 0f)
				{
					npc.frameCounter += 1.0;
					if (npc.frameCounter > 3.0)
					{
						npc.frame.Y += frameHeight;
						npc.frameCounter = 0.0;
					}
					if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
					{
						npc.frame.Y = frameHeight * 11;
					}
					else if (npc.frame.Y < frameHeight * 11)
					{
						npc.frame.Y = frameHeight * 11;
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
						npc.frame.Y += frameHeight;
						npc.frameCounter = 0.0;
					}
					if (npc.frame.Y / frameHeight >= Main.npcFrameCount[npc.type] - 10)
					{
						npc.frame.Y = frameHeight * 2;
					}
					else if (npc.frame.Y / frameHeight < 2)
					{
						npc.frame.Y = frameHeight * 2;
					}
				}
			}
			else
			{
				npc.frameCounter = 0.0;
				npc.frame.Y = frameHeight;
				npc.netUpdate = true;
			}
			base.FindFrame(frameHeight);
        }
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return spawnInfo.player.GetModPlayer<AeroPlayer>().ZoneCrystalCaverns ? .30f : 0f;
		}
	}
}