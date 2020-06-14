using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler //credit to Dominic Karma for jump code
{
    [AutoloadBossHead]
    public class CrystalTumbler : ModNPC
    {
        private Player player;
        private float speed;
        float LifePercentLeft;
        int t;
        int i;
        int FlyUpwardTime = 5;
        int Time = 10;
        int RotationTime = 10;
        int TotalRotations = 5;
        int Max = 10;

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.lifeMax = 1600;
            npc.damage = 12;
            npc.defense = 8;
            npc.knockBackResist = 0f;
            npc.width = 122;
            npc.height = 126;
            npc.value = Item.buyPrice(0, 5, 60, 45);
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath44;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 3200;  //boss life scale in expertmode
            npc.damage = 20;  //boss damage increase in expermode
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/CrystalTumbler/Glowmask");
            Vector2 drawPos = npc.Center + new Vector2(0, npc.gfxOffY) - Main.screenPosition;
            //keep an eye on the width and height when doing this. It matters
            spriteBatch.Draw
            (
                texture,
                drawPos,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                npc.rotation,
                texture.Size() * 0.5f,
                npc.scale,
                npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, //adjust this according to the sprite
                0f
                );
        }


        public override void AI()
        {

            t++;
            Time++;
            LifePercentLeft = -(npc.life / npc.lifeMax) + 1f;
            npc.TargetClosest(true);
            var player = Main.player[npc.target];
            if (player.Center.X > npc.Center.X)
            {
                if (npc.velocity.X < 2 * (LifePercentLeft + 1))
                {
                    npc.velocity.X += (0.075f * (LifePercentLeft + 1));
                }
            }
            if (player.Center.X < npc.Center.X)
            {
                if (npc.velocity.X > -2 * (LifePercentLeft + 1))
                {
                    npc.velocity.X -= (0.075f * (LifePercentLeft + 1));

                }
            }
            npc.rotation += npc.velocity.X * 0.025f;
            if (t % 255 == 0)
            {
                npc.velocity.Y -= 10;
            }
            else if (t % 215 == 10)
            {
                Vector2 offset = new Vector2(0, -100);
                Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerSpike1>(), 6, 1f, Main.myPlayer);
                Projectile.NewProjectile(npc.Center + offset, new Vector2(2f + ((float)Main.rand.Next(20) / 10) - 1, -2f + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerSpike2>(), 8, 1f, Main.myPlayer);
                Projectile.NewProjectile(npc.Center + offset, new Vector2(-2f + ((float)Main.rand.Next(20) / 10) - 1, -2f + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerSpike1>(), 6, 1f, Main.myPlayer);
            }
            if (t % 150 == 10)
            {
                Vector2 offset = new Vector2(0, -100);
                Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 4, 1f, Main.myPlayer);
                Projectile.NewProjectile(npc.Center + offset, new Vector2(2f + ((float)Main.rand.Next(20) / 10) - 1, -2f + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard2>(), 5, 1f, Main.myPlayer);
                Projectile.NewProjectile(npc.Center + offset, new Vector2(-2f + ((float)Main.rand.Next(20) / 10) - 1, -2f + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 4, 1f, Main.myPlayer);
            }
            if (npc.life < 800 && npc.life > 201)
                if (t % 70 == 0)
                {
                    Vector2 offset = new Vector2(0, -100);
                    Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 4, 1f, Main.myPlayer);
                }
            if (t % 350 == 0)
            {
                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("CrystalWormHead"));
            }
            if (Main.expertMode)
            {
                if (npc.life < 1600)
                    if (t % 250 == 0)
                    {
                        Vector2 offset = new Vector2(0, -100);
                        Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerHomingShard>(), 12, 1f, Main.myPlayer);
                        Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 6, 1f, Main.myPlayer);
                    }
            }
            if (Time >= Max)
            {
                Time = 0;
            }

            if ((npc.Center.Y - player.Center.Y) < -150)
            {
                i++;
            }
            else
            {
                i = 0;
            }
            if (i % 50 == 0)
            {
                npc.noTileCollide = true;
            }
            if (i % 50 == 10 || i == 0)
            {
                npc.noTileCollide = false;
            }
        }
            private void Move(Vector2 offset)
            {
                speed = 5f;
                Vector2 moveTo = player.Center + offset;
                Vector2 move = moveTo - npc.Center;
                float magnitude = Magnitude(move);
                if (magnitude > speed)
                {
                    move *= speed / magnitude;
                }
                float turnResistance = 2f;
                move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
                magnitude = Magnitude(move);
                if (magnitude > speed)
                {
                    move *= speed / magnitude;
                }
                npc.velocity = move;
            }

            private void DespawnHandler()
            {
                if (!player.active || player.dead)
                {
                    npc.TargetClosest(false);
                    player = Main.player[npc.target];
                    if (!player.active || player.dead)
                    {
                        npc.velocity = new Vector2(0f, -10f);
                        if (npc.timeLeft > 10)
                        {
                            npc.timeLeft = 10;
                        }
                        return;
                    }
                }
            }
        private float Magnitude(Vector2 mag)
        {
            return (float)Math.Sqrt(mag.X * mag.X + mag.Y * mag.Y);
        }
    }
}
