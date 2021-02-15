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
            Main.npcFrameCount[npc.type] = 8;    //boss frame/animation 
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
            else
            {
                frameCounter = 0;
            }
        }
    }
}