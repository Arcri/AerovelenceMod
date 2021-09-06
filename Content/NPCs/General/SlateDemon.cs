using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.General
{
    public class SlateDemon : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slate Demon");
            Main.npcFrameCount[npc.type] = 5;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 70;
            npc.aiStyle = 22;
            npc.damage = 15;
            npc.defense = 24;
            npc.knockBackResist = 0f;
            npc.width = 44;
            npc.height = 32;
            npc.value = Item.buyPrice(0, 0, 15, 0);
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath44;
        }
        int speed = 3;
        int maxFrames = 3;
        int frame;
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= speed)
            {
                frame++;
                npc.frameCounter = 0;
            }

            if (frame > maxFrames)
                frame = 0;

            npc.frame.Y = frame * frameHeight;
        }
		public override void AI()
		{
            Player player = Main.player[npc.target];
            Vector2 distanceNorm = player.position - npc.position;
            distanceNorm.Normalize();
            npc.ai[0]++;
            float optimalRotation = (float)Math.Atan2(player.position.Y - npc.position.Y, player.position.X - npc.position.X) - 3.14159265f;
            if (npc.ai[0] % 256 == 0)
            {
                Main.PlaySound(SoundID.Item, (int)npc.Center.X, (int)npc.Center.Y, 94, 0.75f);
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ProjectileID.GoldenShowerHostile, 30, 0f, Main.myPlayer, 0f, 0f);
            }
            if (npc.direction == 1)
            {
                npc.spriteDirection = 1;
            }
            if (npc.direction == -1)
            {
                npc.spriteDirection = -1;
            }
            if (!npc.noTileCollide)
            {
                if (npc.collideX)
                {
                    npc.velocity.X = npc.oldVelocity.X * -0.5f;
                    if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                    {
                        npc.velocity.X = 2f;
                    }
                    if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    {
                        npc.velocity.X = -2f;
                    }
                }
                if (npc.collideY)
                {
                    npc.velocity.Y = npc.oldVelocity.Y * -0.5f;
                    if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
                    {
                        npc.velocity.Y = 1f;
                    }
                    if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
                    {
                        npc.velocity.Y = -1f;
                    }
                }
            }
            if (Main.dayTime && npc.position.Y <= Main.worldSurface * 16.0)
            {
                if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
                npc.directionY = -1;
                npc.velocity.Y += -0.5f;
            }
		}
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return !Main.dayTime ? .2f : 0f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0 || npc.life >= 0)
            {
                int d = 193;
                for (int k = 0; k < 12; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, d, 2.5f * hitDirection, -2.5f, 0, Color.Gray, 0.7f);
                    Dust.NewDust(npc.position, npc.width, npc.height, d, 2.5f * hitDirection, -2.5f, 0, Color.Gray, 0.7f);
                }
            }
        }
    }
}