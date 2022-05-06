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
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 70;
            NPC.damage = 15;
            NPC.defense = 24;
            NPC.knockBackResist = 0f;
            NPC.width = 16;
            NPC.height = 24;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath44;
        }

        int frame;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10)
            {
                frame++;
                NPC.frameCounter = 0;
            }
            if (frame > 3)
            {
                frame = 0;
            }
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            if (NPC.wet)
            {
                NPC.ai[0]++;
                NPC.velocity.Y = (float)Math.Sin(NPC.ai[0] / 20) * 2;
            }
            else
                NPC.velocity.Y += 0.02f;
            
            Lighting.AddLight(NPC.Center, 0f, 0f, 0.6f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 
            spawnInfo.Player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns && spawnInfo.water ? 8f : 0f;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0 || NPC.life >= 0)
            {
                int d = 193;
                for (int k = 0; k < 12; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                }
            }
        }
    }
}