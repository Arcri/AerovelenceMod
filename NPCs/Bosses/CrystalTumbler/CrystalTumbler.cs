using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
	[AutoloadBossHead]
    public class CrystalTumbler : ModNPC
    {
        public override void SetDefaults()
        {
            npc.aiStyle = 26;  //5 is the flying AI
            npc.lifeMax = 4800;   //boss life
            npc.damage = 32;  //boss damage
            npc.defense = 24;    //boss defense
            npc.knockBackResist = 0f;
            npc.width = 116;
            npc.height = 120; 
            npc.value = Item.buyPrice(0, 40, 75, 45);
            npc.npcSlots = 1f;
            npc.boss = true;  
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit2;
	        npc.DeathSound = SoundID.NPCDeath1;
            npc.buffImmune[24] = true;
        }
		public override void AI()
		{
			npc.rotation += npc.velocity.X * 0.05f;
		}
	}
}