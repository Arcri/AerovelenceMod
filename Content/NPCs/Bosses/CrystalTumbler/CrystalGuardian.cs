using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class CrystalGuardian : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
        }

        private readonly int i;

        public override void SetDefaults()
        {
            NPC.aiStyle = 10;
            NPC.lifeMax = 100;
            NPC.damage = 12;
            NPC.defense = 8;
            NPC.knockBackResist = 0f;
            NPC.width = 20;
            NPC.height = 30;
            NPC.value = Item.buyPrice(0, 0, 60, 45);
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 150;
            NPC.damage = 20;
        }
        public override void AI()
        {
            if (i % 215 == 10)
            {
                Vector2 offset = new Vector2(0, -100);
                Projectile.NewProjectile(NPC.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ProjectileID.FrostBlastHostile, 6, 1f, Main.myPlayer);
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            {
                if (NPC.frameCounter < 6)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
                else if (NPC.frameCounter < 8)
                {
                    NPC.frame.Y = 1 * frameHeight;
                }
                else if (NPC.frameCounter < 12)
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