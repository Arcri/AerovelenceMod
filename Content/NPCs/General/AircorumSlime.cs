using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.General
{
    public class AircorumSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aircorum Slime");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BlueSlime];
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = 1;
            NPC.lifeMax = 70;
            NPC.damage = 15;
            NPC.defense = 24;
            NPC.knockBackResist = 0f;
            animationType = NPCID.BlueSlime;
            NPC.width = 38;
            NPC.height = 18;
            NPC.value = Item.buyPrice(0, 0, 7, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath44;
        }

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