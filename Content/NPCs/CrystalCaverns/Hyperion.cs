using System;
using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Projectiles.NPCs.CrystalCaverns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class Hyperion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hyperion");
            Main.npcFrameCount[npc.type] = 5;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 200;
            npc.aiStyle = 5;
            npc.damage = 40;
            npc.defense = 25;
            npc.knockBackResist = 0.6f;
            npc.width = 64;
            npc.height = 54;
            npc.value = Item.buyPrice(0, 0, 10, 0);
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath44;
        }
        int speed = 3;
        int maxFrames = 4;
        int frame;
        int i;

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 350;
            npc.damage = 60;
            npc.defense = 30;
        }
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
            i++;
            Player player = Main.player[npc.target];




            Vector2 moveTo = player.Center;
            float speed = 10f;
            Vector2 move = moveTo - npc.Center;
            float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
            move *= speed / magnitude;
            if (i % 100 == 0)
            {
                npc.velocity = move;
            }
            
            Vector2 distanceNorm = player.position - npc.position;
            distanceNorm.Normalize();
            npc.ai[0]++;
            float optimalRotation = (float)Math.Atan2(player.position.Y - npc.position.Y, player.position.X - npc.position.X) - 3.14159265f;
            npc.rotation = optimalRotation;
            if (npc.ai[0] % 256 == 0)
            {
                Main.PlaySound(SoundID.Item, (int)npc.Center.X, (int)npc.Center.Y, 94, 0.75f);
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<ElectricLaser>(), 30, 0f, Main.myPlayer, 0f, 0f);
            }
            npc.TargetClosest(false);
            if (player.dead)
            {
                npc.velocity.Y = npc.velocity.Y - 0.04f;
            }
            if (npc.ai[0] % 5 == 0)
            {
                npc.ai[1]++;
                if (npc.ai[1] > 3)
                    npc.ai[1] = 0;
            }
            if (player.dead)
            {
                npc.velocity.Y -= 2;
                npc.localAI[0]++;
                if (npc.localAI[0] > 128)
                {
                    npc.active = false;
                }
            }

            npc.rotation = (npc.Center - player.Center).ToRotation();
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
            int dust = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + npc.height * 0.25f), npc.width, (int)(npc.height * 0.5f), DustID.Electric, npc.velocity.X, 2f);
            Main.dust[dust].velocity.X *= 0.5f;
            Main.dust[dust].scale *= 0.99f;
            Main.dust[dust].velocity.Y *= 0.1f;
            Main.dust[dust].noGravity = true;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.GetTexture("AerovelenceMod/Content/NPCs/CrystalCaverns/Hyperion_Glow");
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, SpriteEffects.None, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 
            spawnInfo.player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns && !Main.dayTime && Main.hardMode ? .2f : 0f;


        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0 || npc.life >= 0)
            {
                int d = 193;
                for (int k = 0; k < 12; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                    Dust.NewDust(npc.position, npc.width, npc.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                }
            }
        }
    }
}

