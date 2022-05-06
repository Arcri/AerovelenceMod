using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Cave
{
    public class CaveSnail : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cave Snail");
            Main.npcFrameCount[NPC.type] = 6;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = 67;
            NPC.lifeMax = 30;
            NPC.defense = 5;
            NPC.knockBackResist = 0f;
            NPC.width = 38;
            NPC.height = 18;
            NPC.value = Item.buyPrice(0, 0, 4, 50);
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.damage = 0;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.buffImmune[24] = true;
        }
        private const int Frame_AdobeSnail_0 = 0;
        private const int Frame_AdobeSnail_1 = 1;
        private const int Frame_AdobeSnail_2 = 2;
        private const int Frame_AdobeSnail_3 = 3;
        private const int Frame_AdobeSnail_4 = 4;
        private const int Frame_AdobeSnail_5 = 5;


        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter < 10)
            {
                NPC.frame.Y = Frame_AdobeSnail_0 * frameHeight;
            }
            else if (NPC.frameCounter < 20)
            {
                NPC.frame.Y = Frame_AdobeSnail_1 * frameHeight;
            }
            else if (NPC.frameCounter < 30)
            {
                NPC.frame.Y = Frame_AdobeSnail_2 * frameHeight;
            }
            else if (NPC.frameCounter < 40)
            {
                NPC.frame.Y = Frame_AdobeSnail_3 * frameHeight;
            }
            else if (NPC.frameCounter < 50)
            {
                NPC.frame.Y = Frame_AdobeSnail_4 * frameHeight;
            }
            else if (NPC.frameCounter < 60)
            {
                NPC.frame.Y = Frame_AdobeSnail_5 * frameHeight;
            }
            else
            {
                NPC.frameCounter = 0;
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneRockLayerHeight ? .2f : 0f;
        }
    }
}