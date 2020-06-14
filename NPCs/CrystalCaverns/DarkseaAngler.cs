using AerovelenceMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.CrystalCaverns
{
    public class DarkseaAngler : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Darksea Angler");
            Main.npcFrameCount[npc.type] = 8;
        }

        int t;

        bool IsElectricityActive = false;

        public override void SetDefaults()
        {
            npc.aiStyle = 16;
            npc.lifeMax = 4;
            npc.damage = 20;
            npc.defense = 24;
            npc.knockBackResist = 0f;
            npc.width = 34;
            npc.height = 30;
            npc.value = Item.buyPrice(0, 0, 1, 0);
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath1;
        }



        public override void AI()
        {
            if (t % 100 == 100)
            {
                bool IsElectricityActive = true;
            }
        }






        private const int Frame_DarkseaAngler_1 = 1;
        private const int Frame_DarkseaAngler_2 = 2;
        private const int Frame_DarkseaAngler_3 = 3;
        private const int Frame_DarkseaAngler_4 = 4;
        private const int Frame_DarkseaAngler_5 = 5;
        private const int Frame_DarkseaAngler_6 = 6;
        private const int Frame_DarkseaAngler_7 = 7;
        private const int Frame_DarkseaAngler_8 = 8;

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            t++;
            if (npc.frameCounter < 10)
            {
                npc.frame.Y = Frame_DarkseaAngler_1 * frameHeight;
            }
            else if (npc.frameCounter < 20)
            {
                npc.frame.Y = Frame_DarkseaAngler_2 * frameHeight;
            }
            else if (npc.frameCounter < 30)
            {
                npc.frame.Y = Frame_DarkseaAngler_3 * frameHeight;
            }
            else if (npc.frameCounter < 30)
            {
                npc.frame.Y = Frame_DarkseaAngler_4 * frameHeight;
            }
            else
            {
                npc.frameCounter = 0;
            }
            if (t % 100 == 1)
                if (npc.frameCounter < 10)
                {
                    npc.frame.Y = Frame_DarkseaAngler_5 * frameHeight;
                }
                else if (npc.frameCounter < 20)
                {
                    npc.frame.Y = Frame_DarkseaAngler_6 * frameHeight;
                }
                else if (npc.frameCounter < 30)
                {
                    npc.frame.Y = Frame_DarkseaAngler_7 * frameHeight;
                }
                else if (npc.frameCounter < 30)
                {
                    npc.frame.Y = Frame_DarkseaAngler_8 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }

        }





        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Sparkle>(), npc.velocity.X, npc.velocity.Y, 0, Color.White, 1);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DarkseaAnglerHead"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DarkseaAnglerTail"), 1f);
            }
        }
    }
}
