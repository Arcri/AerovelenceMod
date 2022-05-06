using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class GeodeApparation : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = 14;
            NPC.lifeMax = 50;
            NPC.damage = 12;
            NPC.defense = 8;
            NPC.knockBackResist = 0f;
            NPC.width = 122;
            NPC.height = 126;
            NPC.value = Item.buyPrice(0, 0, 60, 45);
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            {
                if (NPC.frameCounter < 5)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
                else if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = 1 * frameHeight;
                }
                else if (NPC.frameCounter < 15)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
        }
    }
}