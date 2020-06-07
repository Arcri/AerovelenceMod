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
            Main.npcFrameCount[npc.type] = 4;
        }
        public override void SetDefaults()
        {
            npc.aiStyle = 16;
            npc.lifeMax = 4;
            npc.damage = 20;
            npc.defense = 24;
            npc.knockBackResist = 0f;
            npc.width = 38;
            npc.height = 18;
            npc.value = Item.buyPrice(0, 0, 1, 0);
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath1;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CrystalCaverns/DarkseaAnglerHead"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CrystalCaverns/DarkseaAnglerTail"), npc.scale);
            }
        }
    }
}
