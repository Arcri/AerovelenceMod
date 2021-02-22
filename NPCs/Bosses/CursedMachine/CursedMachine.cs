using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace AerovelenceMod.NPCs.Bosses.CursedMachine
{
    [AutoloadBossHead]
    public class CursedMachine : ModNPC
    {
        private int Mode = 0;
        int timer = 0;
        int t = 0;
        bool spawned = false;
        bool hasBeenBelow500 = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Cursed Machine");
            Main.npcFrameCount[npc.type] = 4;
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 455555;
            npc.damage = 200;
            npc.defense = 3;
            npc.knockBackResist = 0f;
            npc.width = 82;
            npc.height = 118;
            npc.aiStyle = -1;
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.value = Item.buyPrice(3, 0, 0, 0);
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/CursedMachine");
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 600000;
            npc.damage = 360;
        }
        public override void AI()
        {
            Player target = Main.player[npc.target];
            Vector2 distance = target.position - npc.position;
            npc.rotation = (float)Math.Atan2(distance.Y, distance.X) + 1.57f;
            distance.Normalize();
            if (npc.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] = 1f;
                for (int i = 0; i < 4; i++)
                {
                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("CursedHook"), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                }
            }
            timer++;
            npc.TargetClosest(false);
            var player = Main.player[npc.target];
            if (timer % 3 == 0)
            {
                t++;
                if (t > 4)
                    t = 0;
            }
            npc.ai[0]++;
            int maxAI = 0;
            int stage = 0;
            float MovementSpeed = 1;
            if (npc.life > (int)(npc.lifeMax * 0.8f))
            {
                maxAI = 1;
                stage = 0;
                MovementSpeed = 4.5f;
                if (npc.ai[1] == 0)
                {
                    if (npc.ai[0] % 15 == 0)
                    {
                        int shot = Projectile.NewProjectile(npc.Center.X, npc.Center.Y,
                        distance.X * 17,
                        distance.Y * 17, ProjectileID.CursedBullet, 15, 35, npc.target);
                        Main.projectile[shot].friendly = false;
                        Main.projectile[shot].hostile = true;
                    }
                }
            }
            if (npc.life > (int)(npc.lifeMax * 0.6f) && npc.life <= (int)(npc.lifeMax * 0.8f))
            {
                maxAI = 2;
                stage = 1;
                MovementSpeed = 5.5f;
                if (npc.ai[1] == 0)
                {
                    if (npc.ai[0] % 25 == 0)
                    {
                        int shot = Projectile.NewProjectile(npc.Center.X, npc.Center.Y,
                        distance.X * 17,
                        distance.Y * 17, ProjectileID.CursedBullet, 15, 35, npc.target);
                        Main.projectile[shot].friendly = false;
                        Main.projectile[shot].hostile = true;
                    }
                }
                if (npc.ai[1] == 1)
                {
                    if (npc.ai[0] % 15 == 0)
                    {
                        int shot = Projectile.NewProjectile(npc.Center.X, npc.Center.Y,
                        distance.X * 17,
                        distance.Y * 17, ProjectileID.CursedBullet, 15, 35, npc.target);
                        Main.projectile[shot].friendly = false;
                        Main.projectile[shot].hostile = true;
                    }
                    if (npc.ai[0] % 22 == 0)
                    {
                        int shot = Projectile.NewProjectile(npc.Center.X, npc.Center.Y,
                        distance.X * 28,
                        distance.Y * 28, ProjectileID.ChlorophyteOrb, 15, 35, npc.target);
                        Main.projectile[shot].friendly = false;
                        Main.projectile[shot].hostile = true;
                    }
                }
            }
            if (npc.life > (int)(npc.lifeMax * 0.5f) && npc.life <= (int)(npc.lifeMax * 0.6f))
            {
                maxAI = 2;
                stage = 2;
                MovementSpeed = 5.5f;
                if (npc.ai[1] == 0)
                {
                    if (npc.ai[0] % 7 == 0)
                    {
                        int shot = Projectile.NewProjectile(npc.Center.X, npc.Center.Y,
                        distance.X * 28,
                        distance.Y * 28, ProjectileID.ChlorophyteOrb, 15, 35, npc.target);
                        Main.projectile[shot].friendly = false;
                        Main.projectile[shot].hostile = true;
                    }
                }
                if (npc.ai[1] == 1)
                {
                    if (npc.ai[0] % 50 == 0)
                    {
                        for (int i = 0; i < (Main.expertMode ? 10 : 7); i++)
                        {
                            float angle = i * 6.28318f / (float)(Main.expertMode ? 10 : 7);
                            int shot = Projectile.NewProjectile(npc.Center.X, npc.Center.Y,
                            (float)Math.Cos(angle) * 17,
                            (float)Math.Sin(angle) * 17, ProjectileID.CursedBullet, 15, 30, npc.target);
                            Main.projectile[shot].friendly = false;
                            Main.projectile[shot].hostile = true;
                        }
                    }
                }
            }
            if (npc.life > (int)(npc.lifeMax * 0.4f) && npc.life <= (int)(npc.lifeMax * 0.5f))
            {
                maxAI = 3;
                stage = 3;
                MovementSpeed = 6f;
                if (npc.ai[1] == 0)
                {
                    if (npc.ai[0] % 50 == 0)
                    {
                        for (int i = 0; i < (Main.expertMode ? 10 : 7); i++)
                        {
                            float angle = i * 6.28318f / (float)(Main.expertMode ? 10 : 7);
                            int shot = Projectile.NewProjectile(npc.Center.X, npc.Center.Y,
                            (float)Math.Cos(angle) * 17,
                            (float)Math.Sin(angle) * 17, ProjectileID.CursedBullet, 15, 30, npc.target);
                            Main.projectile[shot].friendly = false;
                            Main.projectile[shot].hostile = true;
                        }
                    }
                }
                if (npc.ai[1] == 1)
                {
                    if (npc.ai[0] % 13 == 0)
                    {
                        int shot = Projectile.NewProjectile(npc.Center.X, npc.Center.Y,
                        distance.X * 28,
                        distance.Y * 28, ProjectileID.ChlorophyteOrb, 15, 35, npc.target);
                        Main.projectile[shot].friendly = false;
                        Main.projectile[shot].hostile = true;
                    }
                }
                if (npc.ai[1] == 2)
                {
                    if (npc.ai[0] % 35 == 0)
                    {
                        int valueX = new Random().Next(-1, 2);
                        int valueY = new Random().Next(-1, 2);
                        while (valueX == 0)
                        {
                            valueX = new Random().Next(-1, 2);
                        }
                        while (valueY == 0)
                        {
                            valueY = new Random().Next(-1, 2);
                        }
                        Projectile.NewProjectile(player.Center.X + 700 * valueX, player.Center.Y + 700 * valueY,
                            0,
                            0,
                            mod.ProjectileType("CursedDart"),
                            40, 16, Main.myPlayer);
                    }
                }
            }
            if (npc.life < (int)(npc.lifeMax * 0.38f))
            {
                if (npc.localAI[1] == 0)
                {
                    int aura = Projectile.NewProjectile(npc.Center.X - 700, npc.Center.Y,
                        0,
                        0,
                        mod.ProjectileType("CursedAura"),
                        60, 16, Main.myPlayer);
                    Main.projectile[aura].ai[0] = npc.whoAmI;
                    for (int i = 0; i < 4; i++)
                    {
                        int valueX = new Random().Next(-1, 2);
                        int valueY = new Random().Next(-1, 2);
                        while (valueX == 0)
                        {
                            valueX = new Random().Next(-1, 2);
                        }
                        while (valueY == 0)
                        {
                            valueY = new Random().Next(-1, 2);
                        }
                        Projectile.NewProjectile(player.Center.X + 600 * valueX, player.Center.Y + 600 * valueY,
                            0,
                            0,
                            mod.ProjectileType("CursedMonster"),
                            90, 16, Main.myPlayer);
                    }
                    npc.localAI[1] = 1;
                }
            }
            if (npc.life > (int)(npc.lifeMax * 0.1f) && npc.life <= (int)(npc.lifeMax * 0.4f))
            {
                maxAI = 1;
                if (npc.ai[1] == 0)
                {
                    if (npc.ai[0] % 15 == 0)
                    {
                        int valueX = new Random().Next(-1, 2);
                        int valueY = new Random().Next(-1, 2);
                        while (valueX == 0)
                        {
                            valueX = new Random().Next(-1, 2);
                        }
                        while (valueY == 0)
                        {
                            valueY = new Random().Next(-1, 2);
                        }
                        Projectile.NewProjectile(player.Center.X + 700 * valueX, player.Center.Y + 700 * valueY,
                            0,
                            0,
                            mod.ProjectileType("CursedDart"),
                            40, 16, Main.myPlayer);
                    }
                }
                stage = 4;
                MovementSpeed = 11f;
            }
            if (npc.life <= (int)(npc.lifeMax * 0.1f))
            {
                maxAI = 3;
                if (npc.ai[1] == 0)
                {
                    if (npc.ai[0] % 15 == 0)
                    {
                        int valueX = new Random().Next(-1, 2);
                        int valueY = new Random().Next(-1, 2);
                        while (valueX == 0)
                        {
                            valueX = new Random().Next(-1, 2);
                        }
                        while (valueY == 0)
                        {
                            valueY = new Random().Next(-1, 2);
                        }
                        Projectile.NewProjectile(player.Center.X + 700 * valueX, player.Center.Y + 700 * valueY,
                            0,
                            0,
                            mod.ProjectileType("CursedDart"),
                            40, 16, Main.myPlayer);
                    }
                }
                if (npc.ai[1] == 1)
                {
                    if (npc.ai[0] % 90 == 0)
                    {
                        for (int i = 0; i < (Main.expertMode ? 8 : 7); i++)
                        {
                            float angle = i * 6.28318f / (float)(Main.expertMode ? 8 : 7);
                            int shot = Projectile.NewProjectile(npc.Center.X, npc.Center.Y,
                            (float)Math.Cos(angle) * 13,
                            (float)Math.Sin(angle) * 13, mod.ProjectileType("CursedDart"), 15, 30, npc.target);
                        }
                    }
                }
                if (npc.ai[1] == 2)
                {
                    if (npc.ai[0] % 18 == 0)
                    {
                        int shot = Projectile.NewProjectile(npc.Center.X, npc.Center.Y,
                        distance.X * 28,
                        distance.Y * 28, mod.ProjectileType("CursedDart"), 15, 40, npc.target);
                        Main.projectile[shot].friendly = false;
                        Main.projectile[shot].hostile = true;
                    }
                }
                stage = 5;
                MovementSpeed = 14f;
            }
            if (npc.ai[0] % 280 == 0)
            {
                npc.ai[1]++;
                npc.ai[1] = npc.ai[1] > (maxAI - 1) ? 0 : npc.ai[1];
            }
            if (!target.dead)
                if (!(npc.life < (int)(npc.lifeMax * 0.4) & NPC.CountNPCS(mod.NPCType("CursedHook")) > 0))
                    npc.velocity = distance * MovementSpeed;
                else
                    npc.velocity = Vector2.Zero;
            else
            {
                npc.velocity = new Vector2(-25, 25);
                npc.ai[2]++;
                if (npc.ai[2] > 160)
                    npc.active = false;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            {
                if (npc.frameCounter < 4)
                {
                    npc.frame.Y = 0 * frameHeight;
                }
                else if (npc.frameCounter < 10)
                {
                    npc.frame.Y = 1 * frameHeight;
                }
                else if (npc.frameCounter < 15)
                {
                    npc.frame.Y = 2 * frameHeight;
                }
                else if (npc.frameCounter < 20)
                {
                    npc.frame.Y = 3 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (damage > (double)(base.npc.lifeMax / 25))
            {
                damage = 0.0;
                return false;
            }
            if (npc.life < (int)(npc.lifeMax * 0.4) & NPC.CountNPCS(mod.NPCType("CursedHook")) > 0)
                damage = (int)((0.002 * new Random().Next(1, 5)) * damage); //0.2% to 1% regular damage
            return true;
        }
    }
}