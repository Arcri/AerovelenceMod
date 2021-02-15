using AerovelenceMod.Items.BossBags;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.LightningMoth
{
    [AutoloadBossHead]
    public class LightningMoth : ModNPC
    {

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 26;    //boss frame/animation 
        }
        public override void SetDefaults()
        {
            npc.aiStyle = NPCID.Mothron;
            npc.lifeMax = 9500;   //boss life
            npc.damage = 32;  //boss damage
            npc.defense = 9;    //boss defense
            npc.alpha = 0;
            npc.knockBackResist = 0f;
            npc.width = 222;
            npc.height = 174;
            npc.value = Item.buyPrice(0, 5, 75, 45);
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit44;
            npc.DeathSound = SoundID.NPCHit46;
            npc.buffImmune[24] = true;
            bossBag = ModContent.ItemType<SnowriumBag>();
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Snowrium");
        }
        private const int Frame_AdobeSnail_0 = 0;
        private const int Frame_AdobeSnail_1 = 1;
        private const int Frame_AdobeSnail_2 = 2;
        private const int Frame_AdobeSnail_3 = 3;
        private const int Frame_AdobeSnail_4 = 4;
        private const int Frame_AdobeSnail_5 = 5;
        private const int Frame_AdobeSnail_6 = 6;
        private const int Frame_AdobeSnail_7 = 7;
        private const int Frame_AdobeSnail_8 = 8;
        private const int Frame_AdobeSnail_9 = 9;
        private const int Frame_AdobeSnail_10 = 10;
        private const int Frame_AdobeSnail_11 = 11;
        private const int Frame_AdobeSnail_12 = 12;
        private const int Frame_AdobeSnail_13 = 13;
        private const int Frame_AdobeSnail_14 = 14;
        private const int Frame_AdobeSnail_15 = 15;
        private const int Frame_AdobeSnail_16 = 16;
        private const int Frame_AdobeSnail_17 = 17;
        private const int Frame_AdobeSnail_18 = 18;
        private const int Frame_AdobeSnail_19 = 19;
        private const int Frame_AdobeSnail_20 = 20;
        private const int Frame_AdobeSnail_21 = 21;
        private const int Frame_AdobeSnail_22 = 22;
        private const int Frame_AdobeSnail_23 = 23;
        private const int Frame_AdobeSnail_24 = 24;
        private const int Frame_AdobeSnail_25 = 25;
        private const int Frame_AdobeSnail_26 = 26;

        public float frameCounter = 0f;
        public override void FindFrame(int frameHeight)
        {
            frameCounter += npc.velocity.X * 0.5f + 0.5f;
            if (frameCounter < 10)
            {
                npc.frame.Y = Frame_AdobeSnail_0 * frameHeight;
            }
            else if (frameCounter < 20)
            {
                npc.frame.Y = Frame_AdobeSnail_1 * frameHeight;
            }
            else if (frameCounter < 30)
            {
                npc.frame.Y = Frame_AdobeSnail_2 * frameHeight;
            }
            else if (frameCounter < 40)
            {
                npc.frame.Y = Frame_AdobeSnail_3 * frameHeight;
            }
            else if (frameCounter < 50)
            {
                npc.frame.Y = Frame_AdobeSnail_4 * frameHeight;
            }
            else if (frameCounter < 60)
            {
                npc.frame.Y = Frame_AdobeSnail_5 * frameHeight;
            }
            else if (frameCounter < 70)
            {
                npc.frame.Y = Frame_AdobeSnail_6 * frameHeight;
            }
            else if (frameCounter < 80)
            {
                npc.frame.Y = Frame_AdobeSnail_7 * frameHeight;
            }

            else if (frameCounter < 90)
            {
                npc.frame.Y = Frame_AdobeSnail_8 * frameHeight;
            }
            else if (frameCounter < 100)
            {
                npc.frame.Y = Frame_AdobeSnail_9 * frameHeight;
            }
            else if (frameCounter < 120)
            {
                npc.frame.Y = Frame_AdobeSnail_10 * frameHeight;
            }
            else if (frameCounter < 130)
            {
                npc.frame.Y = Frame_AdobeSnail_11 * frameHeight;
            }
            else if (frameCounter < 140)
            {
                npc.frame.Y = Frame_AdobeSnail_12 * frameHeight;
            }
            else if (frameCounter < 150)
            {
                npc.frame.Y = Frame_AdobeSnail_13 * frameHeight;
            }
            else if (frameCounter < 160)
            {
                npc.frame.Y = Frame_AdobeSnail_14 * frameHeight;
            }
            else if (frameCounter < 170)
            {
                npc.frame.Y = Frame_AdobeSnail_15 * frameHeight;
            }
            else if (frameCounter < 180)
            {
                npc.frame.Y = Frame_AdobeSnail_16 * frameHeight;
            }
            else if (frameCounter < 190)
            {
                npc.frame.Y = Frame_AdobeSnail_17 * frameHeight;
            }
            else if (frameCounter < 200)
            {
                npc.frame.Y = Frame_AdobeSnail_18 * frameHeight;
            }
            else if (frameCounter < 210)
            {
                npc.frame.Y = Frame_AdobeSnail_19 * frameHeight;
            }
            else if (frameCounter < 220)
            {
                npc.frame.Y = Frame_AdobeSnail_20 * frameHeight;
            }
            else if (frameCounter < 230)
            {
                npc.frame.Y = Frame_AdobeSnail_21 * frameHeight;
            }
            else if (frameCounter < 240)
            {
                npc.frame.Y = Frame_AdobeSnail_22 * frameHeight;
            }
            else if (frameCounter < 250)
            {
                npc.frame.Y = Frame_AdobeSnail_23 * frameHeight;
            }
            else if (frameCounter < 260)
            {
                npc.frame.Y = Frame_AdobeSnail_24 * frameHeight;
            }
            else if (frameCounter < 270)
            {
                npc.frame.Y = Frame_AdobeSnail_25 * frameHeight;
            }
            else if (frameCounter < 280)
            {
                npc.frame.Y = Frame_AdobeSnail_26 * frameHeight;
            }
            else
            {
                frameCounter = 0;
            }
        }
    }
}