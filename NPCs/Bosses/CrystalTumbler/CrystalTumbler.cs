using AerovelenceMod.Dusts;
using AerovelenceMod.Items.BossBags;
using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
    [AutoloadBossHead]
    public class CrystalTumbler : ModNPC
    {
        private Player player;

        private float speed;
        float LifePercentLeft;
        int t;
        int i;
        public bool P;
        public int spinTimer;
        public bool Phase2;
        int Time = 0;
        int cheeseCheck;
        int FlyUpwardTime = 20;
        int RotationTime = (int)2.5 * 60;
        int rotationalSpeed = 2;
        int TotalRotations = 3;
        int Max = 10;

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.lifeMax = 1600;
            npc.damage = 12;
            npc.defense = 8;
            npc.knockBackResist = 0f;
            npc.width = 120;
            npc.height = 128;
            npc.value = Item.buyPrice(0, 5, 60, 45);
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            bossBag = ModContent.ItemType<CrystalTumblerBag>();
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalTumbler");
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
            spriteBatch.Draw
            (
                texture,
                drawPos,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                npc.rotation,
                texture.Size() * 0.5f,
                npc.scale,
                npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f
                );
        }

        public override void NPCLoot()
        {
            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            if (!Main.expertMode)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.LesserHealingPotion, Main.rand.Next(4, 12), false, 0, false, false);
                switch (Main.rand.Next(5))
                {
                    case 0:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CavernMauler"), 1, false, 0, false, false);
                        break;
                    case 1:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CavernousImpaler"), 1, false, 0, false, false);
                        break;
                    case 2:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CrystallineQuadshot"), 1, false, 0, false, false);
                        break;
                    case 3:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PrismThrasher"), 1, false, 0, false, false);
                        break;
                    case 4:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PrismPiercer"), 1, false, 0, false, false);
                        break;
                    case 5:
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DiamondDuster"), 1, false, 0, false, false);
                        break;
                }
            }
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Sparkle>(), npc.velocity.X, npc.velocity.Y, 0, Color.White, 1);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore1"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore4"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore5"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore6"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/TumblerGore7"), 1f);
            }
        }



        public override void AI()
        {
            t++;

            var player = Main.player[npc.target];
            Vector2 delta = player.Center - npc.Center;
            float magnitude = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
            Vector2 offset = new Vector2(0, -100);

            Vector2 offsetLightning = new Vector2(0, -500);
            Vector2 LightningTarget = player.Center;
            Vector2 move = player.position - npc.Center;
            LifePercentLeft = -(npc.life / npc.lifeMax) + 1f;
            Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
            if (player.active || !player.dead)
            {
                if (Vector2.Distance(npc.Center, player.Center) <= 500)
                {
                    npc.noTileCollide = npc.noGravity = false;
                    if (npc.life <= npc.lifeMax / 2)
                    {
                        Phase2 = true;
                    }
                    npc.TargetClosest(true);

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
                    if (t % 250 == 0)
                    {

                        npc.velocity.Y -= Main.rand.Next(12) + 7;
                        for (int num325 = 0; num325 < 20; num325++)
                            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Electric, npc.velocity.X, npc.velocity.Y, 0, default, 1);
                    }


                    /*
                    if (t % Main.rand.Next(130, 150) == 10)
                    {
                        Vector2 offset = new Vector2(0, -100);
                        Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 4, 1f, Main.myPlayer);
                        Projectile.NewProjectile(npc.Center + offset, new Vector2(2f + ((float)Main.rand.Next(20) / 10) - 1, -2f + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard2>(), 5, 1f, Main.myPlayer);
                        Projectile.NewProjectile(npc.Center + offset, new Vector2(-2f + ((float)Main.rand.Next(20) / 10) - 1, -2f + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 4, 1f, Main.myPlayer);
                        npc.netUpdate = true;
                    }
                    if (npc.life < 800 && npc.life > 201)
                        if (t % 70 == 0)
                        {
                            Vector2 offset = new Vector2(0, -100);
                            Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 4, 1f, Main.myPlayer);
                            npc.netUpdate = true;
                        }*/

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
                    if (Phase2)
                    {

                        if (magnitude > 0)
                        {
                            delta *= 8f / magnitude;
                        }
                        else
                        {
                            delta = new Vector2(0f, 15f);
                        }
                        if (t % 250 == 0)
                        {
                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, delta.X, delta.Y, ModContent.ProjectileType<TumblerShockBlast>(), 10, 3f, Main.myPlayer, BuffID.OnFire, 600f);
                            npc.netUpdate = true;
                        }
                        if (t % 50 == 0)
                        {

                        }
                        if (Main.expertMode)
                        {
                            if (t % 250 == 0)
                            {
                                Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerHomingShard>(), 12, 1f, Main.myPlayer);
                                Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerShard1>(), 6, 1f, Main.myPlayer);
                                npc.netUpdate = true;
                            }
                            /*if (t % 275 == 0)
                            {
                                if (player.position.X > npc.position.X)
                                {
                                    npc.velocity.X = ((10 * npc.velocity.X + move.X + 10) / 5f);
                                }
                                else if (player.position.X < npc.position.X)
                                {
                                    npc.velocity.X = ((10 * npc.velocity.X + move.X - 10) / 5f);
                                }*/
                        }
                    }
                    /*if (t % 50 == 0)
                    {
                        Projectile.NewProjectile(player.Center - new Vector2(0, 16f * 5f), new Vector2(0f, 0f), ProjectileID.CultistBossLightningOrbArc, 10, 3f, Main.myPlayer, BuffID.OnFire, -0f);
                    }*/
                }
                if (Vector2.Distance(npc.Center, player.Center) >= 500)
                {
                    cheeseCheck++;
                    npc.rotation += npc.velocity.X * 0.025f;
                    if (cheeseCheck >= 180)
                    {
                        Time++;
                        // Rise for a bit.
                        if (Time < FlyUpwardTime)
                        {
                            npc.noTileCollide = npc.noGravity = true;
                            npc.velocity = Vector2.Lerp(npc.velocity, Vector2.UnitY * -12, 0.2f); // Quickly rise upward (but don't stop all movement and rise immediately)
                        }
                        // Redirect towards the target before spinning.
                        if (Time == FlyUpwardTime)
                        {
                            npc.velocity = npc.DirectionTo(player.Center) * rotationalSpeed;
                        }
                        // And spin around.
                        if (Time > FlyUpwardTime && Time < RotationTime + FlyUpwardTime)
                        {
                            npc.velocity = npc.velocity.RotatedBy(MathHelper.TwoPi / RotationTime * TotalRotations);
                        }
                        // After time is >= RotationTime + FlyUpwardTime, do nothing for a bit and let the lunge happen. Maybe decelerate at the end. Be sure to change npc.noTileCollide and npc.noGravity back to normal.
                        if (t % Main.rand.Next(190, 215) == 10)
                        {
                            Projectile.NewProjectile(npc.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerSpike1>(), 6, 1f, Main.myPlayer);
                            Projectile.NewProjectile(npc.Center + offset, new Vector2(2f + ((float)Main.rand.Next(20) / 10) - 1, -2f + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerSpike2>(), 8, 1f, Main.myPlayer);
                            Projectile.NewProjectile(npc.Center + offset, new Vector2(-2f + ((float)Main.rand.Next(20) / 10) - 1, -2f + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<TumblerSpike1>(), 6, 1f, Main.myPlayer);
                            npc.netUpdate = true;
                            Time = 0;
                            cheeseCheck = 0;
                        }
                    }
                }
                if (!player.active || player.dead)
                {
                    npc.noTileCollide = true;
                    npc.TargetClosest(false);
                    npc.velocity.Y = 20f;
                    if (npc.timeLeft > 10)
                    {
                        npc.timeLeft = 10;
                    }
                }
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

        private float Magnitude(Vector2 mag)
        {
            return (float)Math.Sqrt(mag.X * mag.X + mag.Y * mag.Y);
        }
    }
}