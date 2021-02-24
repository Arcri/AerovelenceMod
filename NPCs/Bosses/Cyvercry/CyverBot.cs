using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.Cyvercry
{
    public class CyverBot : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyver Bot");
            Main.npcFrameCount[npc.type] = 4;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 540;
            npc.damage = 40;
            npc.defense = 12;
            npc.width = 66;
            npc.height = 40;
            npc.boss = false;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.value = Item.buyPrice(0, 0, 24, 48);
            npc.alpha = 255;
            npc.scale = 1;
            npc.knockBackResist = 0f;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 720;
            npc.damage = 20;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return !npc.dontTakeDamage;
        }
        float dynamicCounter = 0;
        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                npc.netUpdate = true;
            dynamicCounter++;
            npc.spriteDirection = -1;
            if(npc.alpha > 0)
            {
                npc.dontTakeDamage = true;
                npc.alpha -= 2;
            }
            else
            {
                npc.dontTakeDamage = false;
                npc.alpha = 0;
            }
            int type = DustID.Electric;
            if (npc.ai[2] == -1)
                type = 235;
            Vector2 from = npc.Center + new Vector2(18, 0).RotatedBy(npc.rotation);
            int dust = Dust.NewDust(from + new Vector2(-4, -4), 0, 0, type, 0, 0, npc.alpha, default, 1.25f);
            Vector2 dustVelo = new Vector2(3, 0).RotatedBy(npc.rotation);
            Main.dust[dust].velocity += dustVelo;
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0.2f;
            Main.dust[dust].scale *= 0.5f * (0.5f + 0.75f * (255 - npc.alpha) / 255f);
            if (type == 235)
                Main.dust[dust].scale *= 2;
            npc.ai[1]++;
            if(npc.ai[0] < 0)
            {
                npc.active = false;
            }
            Player player = Main.player[(int)npc.ai[0]];
            if(!player.active || player.dead)
            {
                npc.active = false;
            }
            bool found = false;
            int total = 0;
            int ofTotal = 0;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC checkNpc = Main.npc[i];
                if (npc.type == checkNpc.type && checkNpc.active && npc.active && checkNpc.ai[0] == npc.ai[0])
                {
                    if(total == 0)
                    {
                        npc.ai[1] = checkNpc.ai[1];
                    }
                    if (checkNpc == npc)
                    {
                        found = true;
                    }
                    if (!found)
                        ofTotal++;
                    total++;
                }
            }
            if ((int)npc.ai[1] % 300 == (int)(ofTotal * 300f / total % 300) && !npc.dontTakeDamage)
            {
                FireLaser(ProjectileID.RayGunnerLaser, 1.5f, 8);
            }
            Vector2 dynamicAddition = new Vector2(48, 0).RotatedBy(MathHelper.ToRadians(dynamicCounter * 3));
            float rotationSpeed = .75f;
            Vector2 properLoop = new Vector2(540 + dynamicAddition.X, 0).RotatedBy(MathHelper.ToRadians(npc.ai[1] * rotationSpeed + (ofTotal * 360f / total)));
            properLoop.Y *= 0.75f;
            Vector2 toPos = properLoop + player.Center;
            toPos -= npc.Center;
            float dist = toPos.Length();
            toPos = toPos.SafeNormalize(Vector2.Zero);
            float speed = 3 + 15f * (255 - npc.alpha) / 255f;
            if (speed > dist)
                speed = dist;
            npc.velocity *= 0.7f;
            npc.velocity += 0.35f * toPos * speed;
            npc.rotation = (player.Center - npc.Center).ToRotation() + MathHelper.ToRadians(180);
        }
        public void FireLaser(int type, float speed = 6f, float recoilMult = 2f, float ai1 = 0, float ai2 = 0)
        {
            Player player = Main.player[npc.target];
            Vector2 toPlayer = player.Center - npc.Center;
            toPlayer = toPlayer.SafeNormalize(new Vector2(1, 0));
            toPlayer *= speed;
            Vector2 from = npc.Center - new Vector2(96, 0).RotatedBy(npc.rotation);
            int damage = 50;
            if (Main.expertMode)
            {
                damage = (int)(damage / Main.expertDamage);
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(from, toPlayer, type, damage, 3, Main.myPlayer, ai1, ai2);
            }
            npc.velocity -= toPlayer * recoilMult;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D primaryTexture = Main.npcTexture[npc.type];
            Texture2D texture = mod.GetTexture("NPCs/Bosses/Cyvercry/CyverBotRed");
            Vector2 drawOrigin = new Vector2(npc.width * 0.5f, npc.height * 0.5f);
            Color color2 = new Color(100, 100, 100, 0);
            if (npc.ai[2] == -1)
                for (int i = 0; i < 360; i += 30)
                {
                    Vector2 rotationalPos = new Vector2(Main.rand.NextFloat(2, 3), 0).RotatedBy(MathHelper.ToRadians(i));
                    spriteBatch.Draw(texture, npc.Center - Main.screenPosition + rotationalPos, npc.frame, color2 * ((255f - npc.alpha) / 255f) * ((255f - npc.alpha) / 255f), npc.rotation, drawOrigin, npc.scale, SpriteEffects.None, 0);
                }
            spriteBatch.Draw(primaryTexture, npc.Center - Main.screenPosition, npc.frame, npc.GetAlpha(drawColor), npc.rotation, drawOrigin, npc.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/Cyvercry/GlowmaskBot");
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, npc.GetAlpha(Color.White), npc.rotation, npc.frame.Size() / 2f, npc.scale, SpriteEffects.None, 0);
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 10)
            {
                npc.frame.Y = npc.frame.Y + frameHeight;
                npc.frameCounter = 5;
                if (npc.frame.Y > 3 * frameHeight)
                {
                    npc.frame.Y = 0;
                    npc.frameCounter = 0;
                }
                if (npc.frame.Y == 2 * frameHeight) //booster frame
                {
                    int type = DustID.Electric;
                    if(npc.ai[2] == -1)
                        type = 235;
                    Vector2 from = npc.Center + new Vector2(24 + (type == DustID.Electric ? 12 : 0), 0).RotatedBy(npc.rotation);
                    if (npc.alpha == 0)
                        for (int i = 0; i < 360; i += 40)
                        {
                            Vector2 circular = new Vector2(20, 0).RotatedBy(MathHelper.ToRadians(i));
                            circular.Y *= 0.6f;
                            circular = circular.RotatedBy(npc.rotation);
                            Vector2 dustVelo = new Vector2(4, 0).RotatedBy(npc.rotation);
                            Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, type, 0, 0, npc.alpha);
                            dust.velocity *= 0.1f;
                            dust.velocity += dustVelo;
                            dust.noGravity = true;
                            if (type == 235)
                                dust.scale *= 2.25f;
                            dust.alpha = npc.alpha;
                        }
                }
            }
        }
    }
}