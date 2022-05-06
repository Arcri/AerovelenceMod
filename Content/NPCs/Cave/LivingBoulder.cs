using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Cave
{
    public class LivingBoulder : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Boulder");
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = 26;
            NPC.lifeMax = 50;
            NPC.damage = 12;
            NPC.defense = 24;
            NPC.knockBackResist = 1f;
            NPC.width = 38;
            NPC.height = 18;
            NPC.value = Item.buyPrice(0, 0, 4, 0);
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.damage = 20;
            NPC.lifeMax = 75;
            NPC.defense = 10;
        }
        public override void AI()
        {
            NPC.rotation += NPC.velocity.X * 0.05f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneRockLayerHeight ? .3f : 0f;
        }
    }
}