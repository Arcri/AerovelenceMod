using AerovelenceMod.Content.Projectiles.NPCs.Bosses.Decurion;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.NPCs.Bosses.Decurion
{
    public class DecurionBodyRocket2 : ModNPC
    {
        int i;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 2000;
            NPC.damage = 120;
            NPC.defense = 23;
            NPC.knockBackResist = 0f;
            NPC.width = 16;
            NPC.height = 14;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            Main.npcFrameCount[NPC.type] = 1;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.npcSlots = 1f;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath44;
        }
        public override void AI()
        {
            i++;
            Player player = Main.player[NPC.target];
            float optimalRotation = (float)Math.Atan2(player.position.Y - NPC.position.Y, player.position.X - NPC.position.X) - 3.14159265f;
            Vector2 distanceNorm = player.position - NPC.position;
            distanceNorm.Normalize();
            if (i % Main.rand.Next(1000, 2000) == 0)
            {
                //SoundEngine.PlaySound(SoundID.Item, (int)NPC.Center.X, (int)NPC.Center.Y, 94, 0.75f); !!COMMENTING THIS FOR NOW
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, distanceNorm.X * 10, distanceNorm.Y * 10, ModContent.ProjectileType<DecurionRocket>(), 30, 0f, Main.myPlayer, 0f, 0f);
            }

            NPC parent = Main.npc[(int)NPC.ai[0]];

            if (!parent.active)
            {
                NPC.active = false;
            }
            Vector2 offset = new Vector2(24, -20);
            NPC.Center = parent.Center + offset.RotatedBy(parent.rotation);
            
            NPC.rotation = optimalRotation;
        }
    }
}