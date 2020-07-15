using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Dusts;

namespace AerovelenceMod.NPCs.Bosses.TheFallen //credit to Dominic Karma for jump code
{
    [AutoloadBossHead]
    public class TheFallen : ModNPC
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
            npc.noGravity = true;
            npc.noTileCollide = false;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalTumbler");
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath44;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 3200;  //boss life scale in expertmode
            npc.damage = 20;  //boss damage increase in expermode
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
            }
        }



        public override void AI()
        {
            t++;
            npc.TargetClosest(true);
            var player = Main.player[npc.target];
            if (player.Center.X > npc.Center.X)
            {
                if (npc.velocity.X < 6)
                {
                    npc.velocity.X += 0.15f;
                }
            }
            if (player.Center.X < npc.Center.X)
            {
                if (npc.velocity.X > -6)
                {
                    npc.velocity.X -= 0.15f;
                }
            }

            if (!(t % 400 >= 0 && t % 500 <= 50))
            {
                if (player.Center.Y > npc.Center.Y + 250)
                {
                    if (npc.velocity.Y < 4)
                    {
                        npc.velocity.Y += 0.2f;
                    }
                }
            }
            else
            {
                if (player.Center.Y > npc.Center.Y)
                {
                    if (npc.velocity.Y < 4)
                    {
                        npc.velocity.Y += 0.2f;
                    }
                }
            }
            if (!(t % 400 >= 0 && t % 500 <= 50))
            {
                if (player.Center.Y < npc.Center.Y + 250)
                {
                    if (npc.velocity.Y > -4)
                    {
                        npc.velocity.Y -= 0.2f;
                    }
                }
            }
            else
            {
                if (player.Center.Y < npc.Center.Y)
                {
                    if (npc.velocity.Y > -4)
                    {
                        npc.velocity.Y -= 0.2f;
                    }
                }
            }
            if (t % 300 == 0)
            {
                npc.velocity.Y -= 0.2f;
                npc.velocity.X -= 0.2f;
            }
            npc.rotation = npc.velocity.X * 0.1f;
        }

        private float Magnitude(Vector2 mag)
        {
            return (float)Math.Sqrt(mag.X * mag.X + mag.Y * mag.Y);
        }
    }
}