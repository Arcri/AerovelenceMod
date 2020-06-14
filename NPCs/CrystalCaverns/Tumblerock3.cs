using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.CrystalCaverns
{
    public class Tumblerock3 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tumblerock");
        }
        public override void SetDefaults()
        {
            npc.aiStyle = 26;
            npc.lifeMax = 50;
            npc.damage = 20;
            npc.defense = 24;
            npc.knockBackResist = 0f;
            npc.width = 38;
            npc.height = 18;
            npc.value = Item.buyPrice(0, 0, 6, 0);
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath44;
        }
        public override void AI()
        {
            npc.rotation += npc.velocity.X * 0.05f;
        }
    }
}