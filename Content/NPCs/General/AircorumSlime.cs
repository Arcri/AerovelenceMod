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
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.BlueSlime];
        }
        public override void SetDefaults()
        {
            npc.aiStyle = 1;
            npc.lifeMax = 70;
            npc.damage = 15;
            npc.defense = 24;
            npc.knockBackResist = 0f;
            animationType = NPCID.BlueSlime;
            npc.width = 38;
            npc.height = 18;
            npc.value = Item.buyPrice(0, 0, 7, 0);
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath44;
        }

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