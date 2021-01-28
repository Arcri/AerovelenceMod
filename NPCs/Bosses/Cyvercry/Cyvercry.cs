using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.Cyvercry //Change me
{
    [AutoloadBossHead]
    public class Cyvercry : ModNPC
    {
        public int i;
        public int i2;
        public int i3;
        public int i4;
        public int spawn;
        public bool frozen;
        internal Vector2 toRotate;
        internal float oldRotation = 0;
        internal float MovementSpeedDelta = 0.225f;
        internal float SpeedMax;
        internal float ChargeSpeedMax;
        internal float ShotSpeed;
        internal const float rotationChangeConstant = 0.125f;
        public bool half;
        public int rain;
        public bool shoot;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyvercry"); //DONT Change me
            Main.npcFrameCount[npc.type] = 5;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void SetDefaults()
        {
            npc.lifeMax = 46000;
            npc.damage = 18;
            npc.defense = 12;
            npc.knockBackResist = 0f;
            npc.width = 162;
            npc.height = 96;
            npc.aiStyle = -1;
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Cyvercry");
            npc.value = Item.buyPrice(0, 22, 11, 5);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 54000;
            npc.damage = 22;
            npc.defense = 18;
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
                else if (npc.frameCounter < 30)
                {
                    npc.frame.Y = 4 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
        }



        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/Cyvercry/Glowmask");
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
            for (int k = 0; k < npc.oldPos.Length; k++)
            {
                Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, npc.gfxOffY);
                Color color = npc.GetAlpha(lightColor) * ((float)(npc.oldPos.Length - k) / (float)npc.oldPos.Length);
                spriteBatch.Draw(Main.npcTexture[npc.type], drawPos, npc.frame, color, npc.rotation, drawOrigin, npc.scale, SpriteEffects.None, 0f);
            }
            return true;
        }

        public override void AI()
        {
            var player = Main.player[npc.target];
            Vector2 distanceNorm = player.position - npc.position;
            distanceNorm.Normalize();
            npc.ai[0]++;
            i++;
            spawn++;
            float optimalRotation = (float)Math.Atan2(player.position.Y - npc.position.Y, player.position.X - npc.position.X) - 3.1415926f;
            npc.rotation = optimalRotation;
            int DirectionValue = npc.Center.X < player.Center.X ? -1 : 1;
            Vector2 optimal = new Vector2();
            if (npc.life < npc.lifeMax / 2)
            {
                npc.defense = 18;
                half = true;
                if (!shoot)
                {
                    i3++;
                    if (i % 5 == 0)
                    {
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 5, ProjectileID.RayGunnerLaser, 40, 0f, Main.myPlayer, 0f, 0f);
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 5, 0, ProjectileID.RayGunnerLaser, 40, 0f, Main.myPlayer, 0f, 0f);
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, -5, ProjectileID.RayGunnerLaser, 40, 0f, Main.myPlayer, 0f, 0f);
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, -5, 0, ProjectileID.RayGunnerLaser, 40, 0f, Main.myPlayer, 0f, 0f);
                    }
                    if (i > 25)
                    {
                        shoot = true;
                    }
                }
            }
            if (frozen)
            {
                i4++;
                if (i4 % 12 == 0)
                {
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 5, ProjectileID.RayGunnerLaser, 40, 0f, Main.myPlayer, 0f, 0f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 5, 0, ProjectileID.RayGunnerLaser, 40, 0f, Main.myPlayer, 0f, 0f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, -5, ProjectileID.RayGunnerLaser, 40, 0f, Main.myPlayer, 0f, 0f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, -5, 0, ProjectileID.RayGunnerLaser, 40, 0f, Main.myPlayer, 0f, 0f);
                }
            }
            if (half)
            {
                rain++;
            }
            if (rain > 42)
            {
                if (Main.rand.Next(2) == 0)
                {
                    int shot = Projectile.NewProjectile(player.position.X - 1000, player.position.Y + Main.rand.Next(-100, 100), 4, 0, ProjectileID.MartianWalkerLaser, 30, 0f, Main.myPlayer, 0f, 0f);
                    Main.projectile[shot].tileCollide = false;
                }
                else
                {
                    int shot = Projectile.NewProjectile(player.position.X + 1000, player.position.Y + Main.rand.Next(-100, 100), -4, 0, ProjectileID.MartianWalkerLaser, 30, 0f, Main.myPlayer, 0f, 0f);
                    Main.projectile[shot].tileCollide = false;
                }
                rain = 0;
            }
            if (npc.ai[0] % 68 == 0 && !frozen)
            {
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ProjectileID.RayGunnerLaser, 24, 0f, Main.myPlayer, 0f, 0f);
            }
            if (spawn >= 550)
            {
                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("CyverBot"));
                spawn = 0;
            }
            if (!frozen)
            {
                if (player.position.Y > npc.position.Y)
                {
                    if (npc.velocity.Y < 10)
                    {
                        npc.velocity.Y += 0.2f;
                    }
                }
                else
                {
                    if (npc.velocity.Y > -10)
                    {
                        npc.velocity.Y -= 0.2f;
                    }
                }
                if (player.position.X > npc.position.X)
                {
                    if (npc.velocity.X < 10)
                    {
                        npc.velocity.X += 0.2f;
                    }
                }
                else
                {
                    if (npc.velocity.X > -10)
                    {
                        npc.velocity.X -= 0.2f;
                    }
                }
            }
            if (i >= 400)
            {
                i2++;
                frozen = true;
                if (i2 >= 100)
                {
                    i = 0;
                    i2 = 0;
                    frozen = false;
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
    }
}