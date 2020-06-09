using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
    [AutoloadBossHead]
    public class CrystalTumbler : ModNPC
    {
        float LifePercentLeft;
        int t;
        int i;

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
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 3200;  //boss life scale in expertmode
            npc.damage = 20;  //boss damage increase in expermode
        }




        public override void AI()
        {

            t++;
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
    }
}
