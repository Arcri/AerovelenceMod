using System;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.TheFallen
{
    [AutoloadBossHead]
    public class TheFallenSpirit : ModNPC
    {
        int t;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 10;    //boss frame/animation 
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.lifeMax = 50000;
            npc.damage = 12;
            npc.defense = 55;
            npc.knockBackResist = 0f;
            npc.width = 386;
            npc.height = 216;
            npc.value = Item.buyPrice(0, 5, 60, 45);
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = false;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/TheFallen");
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath44;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 67500;  //boss life scale in expertmode
            npc.damage = 88;  //boss damage increase in expermode
        }


        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Sparkle>(), npc.velocity.X, npc.velocity.Y, 0, Color.White, 1);
                }
            }
        }


        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/TheFallen/Glowmask");
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
                else if (npc.frameCounter < 35)
                {
                    npc.frame.Y = 5 * frameHeight;
                }
                else if (npc.frameCounter < 40)
                {
                    npc.frame.Y = 6 * frameHeight;
                }
                else if (npc.frameCounter < 45)
                {
                    npc.frame.Y = 7 * frameHeight;
                }
                else if (npc.frameCounter < 50)
                {
                    npc.frame.Y = 8 * frameHeight;
                }
                else if (npc.frameCounter < 55)
                {
                    npc.frame.Y = 9 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
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