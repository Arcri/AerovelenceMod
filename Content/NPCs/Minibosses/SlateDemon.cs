using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.NPCs.MiniBosses
{
    public class SlateDemon : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slate Demon");
            Main.npcFrameCount[NPC.type] = 5;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 70;
            NPC.aiStyle = 22;
            NPC.damage = 15;
            NPC.defense = 24;
            NPC.knockBackResist = 0f;
            NPC.width = 44;
            NPC.height = 32;
            NPC.value = Item.buyPrice(0, 0, 15, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath44;
        }
        int speed = 3;
        int maxFrames = 3;
        int frame;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= speed)
            {
                frame++;
                NPC.frameCounter = 0;
            }

            if (frame > maxFrames)
                frame = 0;

            NPC.frame.Y = frame * frameHeight;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            Vector2 distanceNorm = player.position - NPC.position;
            distanceNorm.Normalize();
            NPC.ai[0]++;
            float optimalRotation = (float)Math.Atan2(player.position.Y - NPC.position.Y, player.position.X - NPC.position.X) - 3.14159265f;
            if (NPC.ai[0] % 256 == 0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ProjectileID.GoldenShowerHostile, 30, 0f, Main.myPlayer, 0f, 0f);
            }
            if (NPC.direction == 1)
            {
                NPC.spriteDirection = 1;
            }
            if (NPC.direction == -1)
            {
                NPC.spriteDirection = -1;
            }
            if (!NPC.noTileCollide)
            {
                if (NPC.collideX)
                {
                    NPC.velocity.X = NPC.oldVelocity.X * -0.5f;
                    if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                    {
                        NPC.velocity.X = 2f;
                    }
                    if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                    {
                        NPC.velocity.X = -2f;
                    }
                }
                if (NPC.collideY)
                {
                    NPC.velocity.Y = NPC.oldVelocity.Y * -0.5f;
                    if (NPC.velocity.Y > 0f && NPC.velocity.Y < 1f)
                    {
                        NPC.velocity.Y = 1f;
                    }
                    if (NPC.velocity.Y < 0f && NPC.velocity.Y > -1f)
                    {
                        NPC.velocity.Y = -1f;
                    }
                }
            }
            if (Main.dayTime && NPC.position.Y <= Main.worldSurface * 16.0)
            {
                if (NPC.timeLeft > 10)
                {
                    NPC.timeLeft = 10;
                }
                NPC.directionY = -1;
                NPC.velocity.Y += -0.5f;
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return !Main.dayTime ? .2f : 0f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0 || NPC.life >= 0)
            {
                int d = 193;
                for (int k = 0; k < 12; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hitDirection, -2.5f, 0, Color.Gray, 0.7f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hitDirection, -2.5f, 0, Color.Gray, 0.7f);
                }
            }
        }
    }
}