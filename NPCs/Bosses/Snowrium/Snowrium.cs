using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
	[AutoloadBossHead]
    public class Snowrium : ModNPC
    {
        public override void SetDefaults()
        {
            npc.aiStyle = 5;  //5 is the flying AI
            npc.lifeMax = 4800;   //boss life
            npc.damage = 32;  //boss damage
            npc.defense = 24;    //boss defense
			animationType = NPCID.DemonEye;
            npc.knockBackResist = 0f;
            npc.width = 130;
            npc.height = 98;
            Main.npcFrameCount[npc.type] = 4;    //boss frame/animation 
            npc.value = Item.buyPrice(0, 40, 75, 45);
            npc.npcSlots = 1f;
            npc.boss = true;  
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit5;
	        npc.DeathSound = SoundID.NPCDeath1;
            npc.buffImmune[24] = true;
        }
	public override void NPCLoot()
		{
			if (Main.expertMode)
			{
			Item.NewItem(npc.getRect(), mod.ItemType("SnowriumBag"));
		}
		switch (Main.rand.Next(6))
		{
			case 0:
			Item.NewItem(npc.getRect(), mod.ItemType("IcySaber"));
			break;

			case 1:
			Item.NewItem(npc.getRect(), mod.ItemType("CrystalArch"));
			break;

			case 2:
			Item.NewItem(npc.getRect(), mod.ItemType("DeepFreeze"));
			break;
			
			case 3:
			Item.NewItem(npc.getRect(), mod.ItemType("CryoBall"));
			break;
			
			case 4:
			Item.NewItem(npc.getRect(), mod.ItemType("Snowball"));
			break;
			
			case 5:
			Item.NewItem(npc.getRect(), mod.ItemType("FrozenBliss"));
			break;
		}
		Item.NewItem(npc.getRect(), mod.ItemType("FrostShard"), Main.rand.Next(10) + 10);
	}
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.579f * bossLifeScale);  //boss life scale in expertmode
            npc.damage = (int)(npc.damage * 0.6f);  //boss damage increase in expermode
        }
		public override void OnHitPlayer(Player player, int damage, bool crit) {
			if (Main.expertMode || Main.rand.NextBool()) {
				player.AddBuff(44, 600, true);
			}

		}
		public override void AI()
        {
			if (!Main.player[Main.myPlayer].ZoneSnow || Main.dayTime)
			{
				npc.defense = 999999;
			} 
			else if (Main.player[Main.myPlayer].ZoneSnow && !Main.dayTime)
			{
				npc.defense = 24;
			}
            npc.ai[0]++;
            Player P = Main.player[npc.target];
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest(true);
            }
            npc.netUpdate = true;
 
            npc.ai[1]++;
            if (npc.ai[1] >= 150)  // 230 is projectile fire rate
            {
                float Speed = 20f;  //projectile speed
                Vector2 vector8 = new Vector2(npc.position.X + (npc.width / 2), npc.position.Y + (npc.height / 2));
                int damage = 16;  //projectile damage
                int type = 128;  //put your projectile
                //Main.PlaySound(0, (int)npc.position.X, (int)npc.position.Y, 17);
                float rotation = (float)Math.Atan2(vector8.Y - (P.position.Y + (P.height * 0.5f)), vector8.X - (P.position.X + (P.width * 0.5f)));
                int num54 = Projectile.NewProjectile(vector8.X, vector8.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, 0);
                npc.ai[1] = 0;
            }
            if (npc.ai[0] % 400 == 3)  //Npc spown rate
 
            {
                NPC.NewNPC((int)npc.position.X, (int)npc.position.Y, 184);  //NPC name
            }
        }
    }
}