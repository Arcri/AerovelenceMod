using System;
using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Projectiles.NPCs.CrystalCaverns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class Hyperion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hyperion");
            Main.npcFrameCount[NPC.type] = 5;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 200;
            NPC.aiStyle = 5;
            NPC.damage = 40;
            NPC.defense = 25;
            NPC.knockBackResist = 0.6f;
            NPC.width = 64;
            NPC.height = 54;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath44;
        }
        int speed = 3;
        int maxFrames = 4;
        int frame;
        int i;

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 350;
            NPC.damage = 60;
            NPC.defense = 30;
        }
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
            i++;
            Player player = Main.player[NPC.target];




            Vector2 moveTo = player.Center;
            float speed = 10f;
            Vector2 move = moveTo - NPC.Center;
            float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
            move *= speed / magnitude;
            if (i % 100 == 0)
            {
                NPC.velocity = move;
            }
            
            Vector2 distanceNorm = player.position - NPC.position;
            distanceNorm.Normalize();
            NPC.ai[0]++;
            float optimalRotation = (float)Math.Atan2(player.position.Y - NPC.position.Y, player.position.X - NPC.position.X) - 3.14159265f;
            NPC.rotation = optimalRotation;
            if (NPC.ai[0] % 256 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item, (int)NPC.Center.X, (int)NPC.Center.Y, 94, 0.75f);
                Projectile.NewProjectile(NPC.Center.X, NPC.Center.Y, distanceNorm.X * 4, distanceNorm.Y * 4, ModContent.ProjectileType<ElectricLaser>(), 30, 0f, Main.myPlayer, 0f, 0f);
            }
            NPC.TargetClosest(false);
            if (player.dead)
            {
                NPC.velocity.Y = NPC.velocity.Y - 0.04f;
            }
            if (NPC.ai[0] % 5 == 0)
            {
                NPC.ai[1]++;
                if (NPC.ai[1] > 3)
                    NPC.ai[1] = 0;
            }
            if (player.dead)
            {
                NPC.velocity.Y -= 2;
                NPC.localAI[0]++;
                if (NPC.localAI[0] > 128)
                {
                    NPC.active = false;
                }
            }

            NPC.rotation = (NPC.Center - player.Center).ToRotation();
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
            int dust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + NPC.height * 0.25f), NPC.width, (int)(NPC.height * 0.5f), DustID.Electric, NPC.velocity.X, 2f);
            Main.dust[dust].velocity.X *= 0.5f;
            Main.dust[dust].scale *= 0.99f;
            Main.dust[dust].velocity.Y *= 0.1f;
            Main.dust[dust].noGravity = true;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/CrystalCaverns/Hyperion_Glow");
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 
            spawnInfo.Player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns && !Main.dayTime && Main.hardMode ? .2f : 0f;


        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0 || NPC.life >= 0)
            {
                int d = 193;
                for (int k = 0; k < 12; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                }
            }
        }
    }
}

