using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
    public class CrystalGuardian : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 3;
        }

        private readonly int i;

        public override void SetDefaults()
        {
            npc.aiStyle = 10;
            npc.lifeMax = 100;
            npc.damage = 12;
            npc.defense = 8;
            npc.knockBackResist = 0f;
            npc.width = 20;
            npc.height = 30;
            npc.value = Item.buyPrice(0, 0, 60, 45);
            npc.npcSlots = 1f;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath44;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 150;
            npc.damage = 20;
        }
        public override void AI()
        {
            if (i % 215 == 10)
            {
                Vector2 offset = new Vector2(0, -100);
                Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ProjectileID.FrostBlastHostile, 6, 1f, Main.myPlayer);
            }
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            {
                if (npc.frameCounter < 6)
                {
                    npc.frame.Y = 0 * frameHeight;
                }
                else if (npc.frameCounter < 8)
                {
                    npc.frame.Y = 1 * frameHeight;
                }
                else if (npc.frameCounter < 12)
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