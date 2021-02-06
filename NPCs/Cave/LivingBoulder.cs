using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Cave
{
    public class LivingBoulder : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Boulder");
        }
        public override void SetDefaults()
        {
            npc.aiStyle = 26;
            npc.lifeMax = 50;
            npc.damage = 20;
            npc.defense = 24;
            npc.knockBackResist = 1f;
            npc.width = 38;
            npc.height = 18;
            npc.value = Item.buyPrice(0, 0, 4, 0);
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
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.ZoneRockLayerHeight ? .3f : 0f;
        }
    }
}