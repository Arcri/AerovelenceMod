using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.SkyWraith //Change me
{
    public class WormProbeCircler : ModNPC
    {
        float timer = 0;
        int timer2 = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rotater"); //DONT Change me
            Main.npcFrameCount[npc.type] = 1;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 500; //Change me
            npc.damage = 0;
            npc.defense = 15; //Change me
            npc.knockBackResist = 0f;
            npc.width = 46; //Change me
            npc.height = 58; //Change me
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.reflectingProjectiles = true;
            npc.HitSound = SoundID.NPCHit4; //Change me if you want (Rock hit sound)
            npc.DeathSound = SoundID.NPCDeath14; //Change me if you want (Heavy grunt sound)
            npc.value = Item.buyPrice(0, 0, 0, 0); //Change me
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 1130; //Change me
            npc.damage = 0;
        }
        public override void AI()
        {
            timer2++;
            timer += (float)Math.PI / 32;
            if (!Main.npc[(int)npc.ai[0]].active | timer2 >= 720) //12 second limit
            {
                npc.active = false;
            }
            npc.position = Main.npc[(int)npc.ai[0]].position + new Vector2((float)Math.Cos(timer + npc.ai[1]), (float)Math.Sin(timer + npc.ai[1])) * 50;
        }
        public override bool CheckActive()
        {
            return false;
        }
    }
}