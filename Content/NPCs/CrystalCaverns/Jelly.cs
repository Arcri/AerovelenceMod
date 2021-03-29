using System;
using AerovelenceMod.Common.Globals.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class Jelly : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jelly");
            Main.npcFrameCount[npc.type] = 4;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 70;
            npc.damage = 15;
            npc.defense = 24;
            npc.knockBackResist = 0f;
            npc.width = 16;
            npc.height = 24;
            npc.value = Item.buyPrice(0, 0, 2, 0);
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath44;
        }

        int frame;
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 10)
            {
                frame++;
                npc.frameCounter = 0;
            }
            if (frame > 3)
            {
                frame = 0;
            }
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            if (npc.wet)
            {
                npc.ai[0]++;
                npc.velocity.Y = (float)Math.Sin(npc.ai[0] / 20) * 2;
            }
            else
                npc.velocity.Y += 0.02f;
            
            Lighting.AddLight(npc.Center, 0f, 0f, 0.6f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 
            spawnInfo.player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns && spawnInfo.water ? 8f : 0f;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0 || npc.life >= 0)
            {
                int d = 193;
                for (int k = 0; k < 12; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                    Dust.NewDust(npc.position, npc.width, npc.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                }
            }
        }
    }
}