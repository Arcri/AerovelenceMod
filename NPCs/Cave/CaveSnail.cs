using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Cave
{
    public class CaveSnail : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cave Snail");
            Main.npcFrameCount[npc.type] = 6;
        }
        public override void SetDefaults()
        {
            npc.aiStyle = 67;
            npc.lifeMax = 4;
            npc.defense = 24;
            npc.knockBackResist = 0f;
            npc.width = 38;
            npc.height = 18;
            npc.value = Item.buyPrice(0, 0, 4, 50);
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.buffImmune[24] = true;
        }
        private const int Frame_AdobeSnail_0 = 0;
        private const int Frame_AdobeSnail_1 = 1;
        private const int Frame_AdobeSnail_2 = 2;
        private const int Frame_AdobeSnail_3 = 3;
        private const int Frame_AdobeSnail_4 = 4;
        private const int Frame_AdobeSnail_5 = 5;


        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter < 10)
            {
                npc.frame.Y = Frame_AdobeSnail_0 * frameHeight;
            }
            else if (npc.frameCounter < 20)
            {
                npc.frame.Y = Frame_AdobeSnail_1 * frameHeight;
            }
            else if (npc.frameCounter < 30)
            {
                npc.frame.Y = Frame_AdobeSnail_2 * frameHeight;
            }
            else if (npc.frameCounter < 40)
            {
                npc.frame.Y = Frame_AdobeSnail_3 * frameHeight;
            }
            else if (npc.frameCounter < 50)
            {
                npc.frame.Y = Frame_AdobeSnail_4 * frameHeight;
            }
            else if (npc.frameCounter < 60)
            {
                npc.frame.Y = Frame_AdobeSnail_5 * frameHeight;
            }
            else
            {
                npc.frameCounter = 0;
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.ZoneRockLayerHeight ? .2f : 0f;
        }
    }
}