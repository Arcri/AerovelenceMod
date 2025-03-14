using AerovelenceMod.Content.Biomes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace AerovelenceMod.Content.Items.Weapons.Caverns
{
    public class CrystallineTumbler : ModNPC
    {
        private int frameVariant;
        private bool initializedFrames = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.width = 22;
            NPC.height = 22;

            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = true;
            NPC.lifeMax = 10;
            NPC.damage = 18;
            NPC.defense = 0;
            NPC.aiStyle = 26;
            NPC.friendly = true;
            NPC.knockBackResist = 0f;
            NPC.lifeRegen = 0;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemSapphire, NPC.velocity.X, NPC.velocity.Y, 0, Color.White);
            }
        }
        public float timer = 0f;    
        public override void AI()
        {
            NPC.rotation += NPC.velocity.X * 0.05f;
            if (timer == 420)
                NPC.life = 0;
            timer++;
        }
    }
}