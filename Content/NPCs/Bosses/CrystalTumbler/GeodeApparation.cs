using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class GeodeApparation : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 3;
        }
        public override void SetDefaults()
        {
            npc.aiStyle = 14;
            npc.lifeMax = 50;
            npc.damage = 12;
            npc.defense = 8;
            npc.knockBackResist = 0f;
            npc.width = 122;
            npc.height = 126;
            npc.value = Item.buyPrice(0, 0, 60, 45);
            npc.npcSlots = 1f;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath44;
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            {
                if (npc.frameCounter < 5)
                {
                    npc.frame.Y = 0 * frameHeight;
                }
                else if (npc.frameCounter < 10)
                {
                    npc.frame.Y = 1 * frameHeight;
                }
                else if (npc.frameCounter < 15)
                {
                    npc.frame.Y = 2 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
        }
    }
}