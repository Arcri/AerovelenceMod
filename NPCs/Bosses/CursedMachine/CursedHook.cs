using AerovelenceMod.Projectiles; //Change me
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.CursedMachine //Change me
{
    public class CursedHook : ModNPC
    {
        public int t;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Hook"); //DONT Change me
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 37000; //Change me
            npc.damage = 145;
            npc.defense = 15; //Change me
            npc.knockBackResist = 0f;
            npc.width = 42; //Change me
            npc.height = 42; //Change me
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4; //Change me if you want (Rock hit sound)
            npc.DeathSound = SoundID.NPCDeath20; //Change me if you want (Heavy grunt sound)
            npc.value = Item.buyPrice(0, 0, 0, 0); //Change me
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 49000; //Change me
            npc.damage = 200;
        }
        public override void AI()
        {
            int index = NPC.FindFirstNPC(mod.NPCType("CursedMachine"));
            if (index == -1)
            {
                npc.StrikeNPCNoInteraction(9999, 0f, 0, false, false, false);
                npc.netUpdate = true;
                return;
            }
            if (Main.npc[index].life >= (int)(Main.npc[index].lifeMax * 0.4f))
            {
                npc.dontTakeDamage = true;
                if (npc.velocity.X == 0f && npc.velocity.Y == 0f)
                {
                    if (t < 3)
                    {
                        npc.localAI[1] += 1;
                        if (npc.localAI[1] > 4)
                        {
                            npc.localAI[1] = 0;
                            t++;
                        }
                    }
                }
                else
                {
                    if (t > 0)
                    {
                        npc.localAI[1] += 1;
                        if (npc.localAI[1] > 4)
                        {
                            npc.localAI[1] = 0;
                            t--;
                        }
                    }
                }
            }
            else
            {
                npc.localAI[1] += 1;
                if (npc.localAI[1] > 5)
                {
                    t++;
                }
                t = t > 3 ? 0 : t;
            }
            if (Main.npc[index].life >= (int)(Main.npc[index].lifeMax * 0.4f))
            {
                bool flag48 = false;
                bool playerDead = false;
                if ((double)Main.player[Main.npc[index].target].position.Y < Main.worldSurface * 16.0 ||
                    Main.player[Main.npc[index].target].position.Y > (float)(Main.maxTilesY - 200) * 16 |
                    playerDead)
                {
                    npc.localAI[0] -= 4f;
                    flag48 = true;
                }
                if (Main.player[Main.npc[index].target].dead)
                {
                    playerDead = true;
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    if (npc.ai[0] == 0f)
                    {
                        npc.ai[0] = (float)((int)(npc.Center.X / 16f));
                    }
                    if (npc.ai[1] == 0f)
                    {
                        npc.ai[1] = (float)((int)(npc.Center.X / 16f));
                    }
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.ai[0] == 0f || npc.ai[1] == 0f)
                    {
                        npc.localAI[0] = 0f;
                    }
                    npc.localAI[0] -= 1f;
                    if (Main.npc[index].life < Main.npc[index].lifeMax / 2)
                    {
                        npc.localAI[0] -= 2f;
                    }
                    if (Main.npc[index].life < Main.npc[index].lifeMax / 4)
                    {
                        npc.localAI[0] -= 2f;
                    }
                    if (flag48)
                    {
                        npc.localAI[0] -= 6f;
                    }
                    if (!playerDead && npc.localAI[0] <= 0f && npc.ai[0] != 0f)
                    {
                        int num;
                        for (int num736 = 0; num736 < 200; num736 = num + 1)
                        {
                            if (num736 != npc.whoAmI && Main.npc[num736].active && Main.npc[num736].type == npc.type && (Main.npc[num736].velocity.X != 0f || Main.npc[num736].velocity.Y != 0f))
                            {
                                npc.localAI[0] = (float)Main.rand.Next(60, 300);
                            }
                            num = num736;
                        }
                    }
                    if (npc.localAI[0] <= 0f)
                    {
                        npc.localAI[0] = (float)Main.rand.Next(300, 600);
                        bool flag50 = false;
                        int num737 = 0;
                        while (!flag50 && num737 <= 1000)
                        {
                            int num = num737;
                            num737 = num + 1;
                            int num738 = (int)(Main.player[Main.npc[index].target].Center.X / 16f);
                            int num739 = (int)(Main.player[Main.npc[index].target].Center.Y / 16f);
                            if (npc.ai[0] == 0f)
                            {
                                num738 = (int)((Main.player[Main.npc[index].target].Center.X + Main.npc[index].Center.X) / 32f);
                                num739 = (int)((Main.player[Main.npc[index].target].Center.Y + Main.npc[index].Center.Y) / 32f);
                            }
                            if (playerDead)
                            {
                                num738 = (int)Main.npc[index].position.X / 16;
                                num739 = (int)(Main.npc[index].position.Y + 400f) / 16;
                            }
                            int num740 = 20;
                            num740 += (int)(100f * ((float)num737 / 1000f));
                            int num741 = num738 + Main.rand.Next(-num740, num740 + 1);
                            int num742 = num739 + Main.rand.Next(-num740, num740 + 1);
                            if (Main.npc[index].life < Main.npc[index].lifeMax / 2 && Main.rand.Next(6) == 0)
                            {
                                npc.TargetClosest(true);
                                int num743 = (int)(Main.player[npc.target].Center.X / 16f);
                                int num744 = (int)(Main.player[npc.target].Center.Y / 16f);
                                if (Main.tile[num743, num744].wall > 0)
                                {
                                    num741 = num743;
                                    num742 = num744;
                                }
                            }
                            try
                            {
                                int x = (int)MathHelper.Clamp(num741, 1, Main.maxTilesX);
                                int y = (int)MathHelper.Clamp(num742, 1, Main.maxTilesY);
                                if (WorldGen.SolidTile(x, y) || (Main.tile[x, y].wall > 0 && (num737 > 500 || Main.npc[index].life < Main.npc[index].lifeMax / 2)))
                                {
                                    flag50 = true;
                                    npc.ai[0] = (float)num741;
                                    npc.ai[1] = (float)num742;
                                    npc.netUpdate = true;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                if (npc.ai[0] > 0f && npc.ai[1] > 0f)
                {
                    float num745 = 11f;
                    if (Main.npc[index].life < Main.npc[index].lifeMax / 2)
                    {
                        num745 = 13f;
                    }
                    if (Main.npc[index].life < Main.npc[index].lifeMax / 4)
                    {
                        num745 = 18f;
                    }
                    if (Main.expertMode)
                    {
                        num745 += 5f;
                    }
                    if (Main.expertMode && Main.npc[index].life < Main.npc[index].lifeMax / 2)
                    {
                        num745 += 5f;
                    }
                    if (flag48)
                    {
                        num745 *= 3f;
                    }
                    if (playerDead)
                    {
                        num745 *= 3f;
                    }
                    Vector2 vector95 = new Vector2(npc.Center.X, npc.Center.Y);
                    float num746 = npc.ai[0] * 16f - 8f - vector95.X;
                    float num747 = npc.ai[1] * 16f - 8f - vector95.Y;
                    float num748 = (float)Math.Sqrt((double)(num746 * num746 + num747 * num747));
                    if (num748 < 12f + num745)
                    {
                        npc.velocity.X = num746;
                        npc.velocity.Y = num747;
                    }
                    else
                    {
                        num748 = num745 / num748;
                        npc.velocity.X = num746 * num748;
                        npc.velocity.Y = num747 * num748;
                    }
                    Vector2 vector96 = new Vector2(npc.Center.X, npc.Center.Y);
                    float num749 = Main.npc[index].Center.X - vector96.X;
                    float num750 = Main.npc[index].Center.Y - vector96.Y;
                    npc.rotation = (float)Math.Atan2((double)num750, (double)num749) - 1.57f;
                    return;
                }
            }
            else
            {
                npc.dontTakeDamage = false;
                Player target = Main.player[Main.npc[index].target];
                Vector2 distance = target.position - npc.position;
                distance.Normalize();
                npc.velocity = distance * 6;
                npc.localAI[2]++;
                npc.rotation = (float)Math.Atan2(distance.Y, distance.X) + 1.57f;
                if (npc.localAI[2] % 60 == 0)
                {
                    int shot = Projectile.NewProjectile(npc.Center.X, npc.Center.Y,
                    distance.X * 11,
                    distance.Y * 11, ProjectileID.CursedFlameHostile, 15, 35, npc.target);
                }
            }
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override void PostDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Color drawColor)
        {
            int index = NPC.FindFirstNPC(mod.NPCType("CursedMachine"));
            if (index != -1)
            {
                if (Main.npc[index].life >= (int)(Main.npc[index].lifeMax * 0.4f))
                {
                    int i = npc.whoAmI;
                    Vector2 vector2 = new Vector2(Main.npc[i].position.X + (float)(Main.npc[i].width / 2), Main.npc[i].position.Y + (float)(Main.npc[i].height / 2));
                    float num6 = Main.npc[index].Center.X - vector2.X;
                    float num7 = Main.npc[index].Center.Y - vector2.Y;
                    float rotation2 = (float)Math.Atan2((double)num7, (double)num6) - 1.57f;
                    bool flag3 = true;
                    while (flag3)
                    {
                        int num8 = 16;
                        int num9 = 32;
                        float num10 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                        if (num10 < (float)num9)
                        {
                            num8 = (int)num10 - num9 + num8;
                            flag3 = false;
                        }
                        num10 = (float)num8 / num10;
                        num6 *= num10;
                        num7 *= num10;
                        vector2.X += num6;
                        vector2.Y += num7;
                        num6 = Main.npc[index].Center.X - vector2.X;
                        num7 = Main.npc[index].Center.Y - vector2.Y;
                        Microsoft.Xna.Framework.Color color2 = Lighting.GetColor((int)vector2.X / 16, (int)(vector2.Y / 16f));
                        Main.spriteBatch.Draw(mod.GetTexture("NPCs/Bosses/CursedMachine/CursedChain"), new Vector2(vector2.X - Main.screenPosition.X, vector2.Y - Main.screenPosition.Y), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 30, num8)), color2, rotation2, new Vector2((float)30 * 0.5f, (float)26 * 0.5f), 1f, SpriteEffects.None, 0f);
                    }
                }
            }
        }
    }
}