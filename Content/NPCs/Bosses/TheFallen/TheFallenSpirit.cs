using System;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
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
            Main.npcFrameCount[NPC.type] = 10;    //boss frame/animation 
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 50000;
            NPC.damage = 12;
            NPC.defense = 55;
            NPC.knockBackResist = 0f;
            NPC.width = 386;
            NPC.height = 216;
            NPC.value = Item.buyPrice(0, 5, 60, 45);
            NPC.npcSlots = 1f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/TheFallen");
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath44;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 67500;  //boss life scale in expertmode
            NPC.damage = 88;  //boss damage increase in expermode
        }


        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>(), NPC.velocity.X, NPC.velocity.Y, 0, Color.White, 1);
                }
            }
        }


        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
        Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/TheFallen/Glowmask");
            Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.White) * ((float)(NPC.oldPos.Length - k) / NPC.oldPos.Length);
                Main.EntitySpriteDraw((Texture2D)TextureAssets.Npc[NPC.type], drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            }
            return true;
        }
        
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            {
                if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
                else if (NPC.frameCounter < 15)
                {
                    NPC.frame.Y = 1 * frameHeight;
                }
                else if (NPC.frameCounter < 20)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
                else if (NPC.frameCounter < 25)
                {
                    NPC.frame.Y = 3 * frameHeight;
                }
                else if (NPC.frameCounter < 30)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
                else if (NPC.frameCounter < 35)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
                else if (NPC.frameCounter < 40)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
                else if (NPC.frameCounter < 45)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
                else if (NPC.frameCounter < 50)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
                else if (NPC.frameCounter < 55)
                {
                    NPC.frame.Y = 9 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
        }


        public override void AI()
        {
            t++;
            NPC.TargetClosest(true);
            var player = Main.player[NPC.target];
            if (player.Center.X > NPC.Center.X)
            {
                if (NPC.velocity.X < 6)
                {
                    NPC.velocity.X += 0.15f;
                }
            }
            if (player.Center.X < NPC.Center.X)
            {
                if (NPC.velocity.X > -6)
                {
                    NPC.velocity.X -= 0.15f;
                }
            }

            if (!(t % 400 >= 0 && t % 500 <= 50))
            {
                if (player.Center.Y > NPC.Center.Y + 250)
                {
                    if (NPC.velocity.Y < 4)
                    {
                        NPC.velocity.Y += 0.2f;
                    }
                }
            }
            else
            {
                if (player.Center.Y > NPC.Center.Y)
                {
                    if (NPC.velocity.Y < 4)
                    {
                        NPC.velocity.Y += 0.2f;
                    }
                }
            }
            if (!(t % 400 >= 0 && t % 500 <= 50))
            {
                if (player.Center.Y < NPC.Center.Y + 250)
                {
                    if (NPC.velocity.Y > -4)
                    {
                        NPC.velocity.Y -= 0.2f;
                    }
                }
            }
            else
            {
                if (player.Center.Y < NPC.Center.Y)
                {
                    if (NPC.velocity.Y > -4)
                    {
                        NPC.velocity.Y -= 0.2f;
                    }
                }
            }
            if (t % 300 == 0)
            {
                NPC.velocity.Y -= 0.2f;
                NPC.velocity.X -= 0.2f;
            }
            NPC.rotation = NPC.velocity.X * 0.1f;
        }
    }
}