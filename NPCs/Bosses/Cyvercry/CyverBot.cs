using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.Cyvercry
{
    public class CyverBot : ModNPC
    {
        internal Vector2 toRotate;
        internal float oldRotation = 0;
        internal float MovementSpeedDelta = 0.225f;
        internal float SpeedMax;
        internal float ChargeSpeedMax;
        internal float ShotSpeed;
        internal const float rotationChangeConstant = 0.125f;
        private int timer = 0;
        private int t = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyver Bot");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.lifeMax = 320;
            npc.damage = 16;
            npc.defense = 12;
            npc.width = 66;
            npc.height = 36;
            npc.aiStyle = -1;
            npc.boss = false;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.value = Item.buyPrice(0, 0, 24, 48);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 360;
            npc.damage = 20;
        }

        public override void AI()
        {
            var player = Main.player[npc.target];
            Vector2 distanceNorm = player.position - npc.position;
            distanceNorm.Normalize();
            npc.ai[0]++;
            float optimalRotation = (float)Math.Atan2(player.position.Y - npc.position.Y, player.position.X - npc.position.X) - 3.14159265f;
            npc.rotation = optimalRotation;
            int DirectionValue = npc.Center.X < player.Center.X ? -1 : 1;
            Vector2 optimal = new Vector2();
            if (npc.ai[0] % 256 == 0)
            {
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ProjectileID.RayGunnerLaser, 30, 0f, Main.myPlayer, 0f, 0f);
            }
            if (player.position.Y > npc.position.Y)
            {
                if (npc.velocity.Y < 6)
                {
                    npc.velocity.Y += 0.175f;
                }
            }
            else
            {
                if (npc.velocity.Y > -6)
                {
                    npc.velocity.Y -= 0.2f;
                }
            }
            if (player.position.X > npc.position.X)
            {
                if (npc.velocity.X < 6)
                {
                    npc.velocity.X += 0.2f;
                }
            }
            else
            {
                if (npc.velocity.X > -6)
                {
                    npc.velocity.X -= 0.2f;
                }
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
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            {
                if (npc.frameCounter < 10)
                {
                    npc.frame.Y = 0 * frameHeight;
                }
                else if (npc.frameCounter < 15)
                {
                    npc.frame.Y = 1 * frameHeight;
                }
                else if (npc.frameCounter < 20)
                {
                    npc.frame.Y = 2 * frameHeight;
                }
                else if (npc.frameCounter < 25)
                {
                    npc.frame.Y = 3 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
        }
    }
}