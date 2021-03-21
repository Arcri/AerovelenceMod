using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CursedMachine
{
    [AutoloadBossHead]
    public class CursedMachine : ModNPC
    {
        public static int cursedBoss = -1;
        readonly bool hasBeenBelow500 = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Cursed Machine");
            Main.npcFrameCount[npc.type] = 4;
        }
        public override void SetDefaults()
        {
            npc.noTileCollide = true;
            npc.width = 86;
            npc.height = 86;
            npc.damage = 50;
            npc.defense = 14;
            npc.lifeMax = 30000;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 15);
            npc.noGravity = true;
            npc.boss = true;
            npc.npcSlots = 16f;
            npc.buffImmune[20] = true;
            /*npc.lifeMax = 455555;
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
			music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/CursedMachine");*/
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 600000;
            npc.damage = 360;
        }
        public override void AI()
        {
            bool flag43 = false;
            bool flag44 = false;
            npc.TargetClosest();
            if (Main.player[npc.target].dead)
            {
                flag44 = true;
                flag43 = true;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int num733 = 6000;
                if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) + Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) > (float)num733)
                {
                    npc.active = false;
                    npc.life = 0;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                    }
                }
            }
            cursedBoss = npc.whoAmI;
            if (npc.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] = 1f;
                int num734 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<CursedHook>(), npc.whoAmI);
                num734 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<CursedHook>(), npc.whoAmI);
                num734 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<CursedHook>(), npc.whoAmI);
            }
            int[] array2 = new int[3];
            float num735 = 0f;
            float num736 = 0f;
            int num737 = 0;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<CursedHook>())
                {
                    num735 += Main.npc[i].Center.X;
                    num736 += Main.npc[i].Center.Y;
                    array2[num737] = i;
                    num737++;
                    if (num737 > 2)
                    {
                        break;
                    }
                }
            }
            num735 /= num737;
            num736 /= num737;
            float velocity = 2.5f;
            float num740 = 0.025f;
            if (npc.life < npc.lifeMax / 2)
            {
                velocity = 5f;
                num740 = 0.05f;
            }
            if (npc.life < npc.lifeMax / 4)
            {
                velocity = 7f;
            }
            if (!Main.player[npc.target].ZoneJungle || Main.player[npc.target].position.Y < Main.worldSurface * 16.0 || Main.player[npc.target].position.Y > (Main.maxTilesY - 200) * 16)
            {
                flag43 = true;
                velocity += 8f;
                num740 = 0.15f;
            }
            if (Main.expertMode)
            {
                velocity += 1f;
                velocity *= 1.1f;
                num740 += 0.01f;
                num740 *= 1.1f;
            }
            Vector2 vector88 = new Vector2(num735, num736);
            float num741 = Main.player[npc.target].Center.X - vector88.X;
            float num742 = Main.player[npc.target].Center.Y - vector88.Y;
            if (flag44)
            {
                num742 *= -1f;
                num741 *= -1f;
                velocity += 8f;
            }
            float num743 = (float)Math.Sqrt(num741 * num741 + num742 * num742);
            int num744 = 500;
            if (flag43)
            {
                num744 += 350;
            }
            if (Main.expertMode)
            {
                num744 += 150;
            }
            if (num743 >= num744)
            {
                num743 = (float)num744 / num743;
                num741 *= num743;
                num742 *= num743;
            }
            num735 += num741;
            num736 += num742;
            vector88 = new Vector2(npc.Center.X, npc.Center.Y);
            num741 = num735 - vector88.X;
            num742 = num736 - vector88.Y;
            num743 = (float)Math.Sqrt(num741 * num741 + num742 * num742);
            if (num743 < velocity)
            {
                num741 = npc.velocity.X;
                num742 = npc.velocity.Y;
            }
            else
            {
                num743 = velocity / num743;
                num741 *= num743;
                num742 *= num743;
            }
            if (npc.velocity.X < num741)
            {
                npc.velocity.X += num740;
                if (npc.velocity.X < 0f && num741 > 0f)
                {
                    npc.velocity.X += num740 * 2f;
                }
            }
            else if (npc.velocity.X > num741)
            {
                npc.velocity.X -= num740;
                if (npc.velocity.X > 0f && num741 < 0f)
                {
                    npc.velocity.X -= num740 * 2f;
                }
            }
            if (npc.velocity.Y < num742)
            {
                npc.velocity.Y += num740;
                if (npc.velocity.Y < 0f && num742 > 0f)
                {
                    npc.velocity.Y += num740 * 2f;
                }
            }
            else if (npc.velocity.Y > num742)
            {
                npc.velocity.Y -= num740;
                if (npc.velocity.Y > 0f && num742 < 0f)
                {
                    npc.velocity.Y -= num740 * 2f;
                }
            }
            Vector2 vector89 = new Vector2(npc.Center.X, npc.Center.Y);
            float num745 = Main.player[npc.target].Center.X - vector89.X;
            float num746 = Main.player[npc.target].Center.Y - vector89.Y;
            npc.rotation = (float)Math.Atan2(num746, num745) + 1.57f;
            if (npc.life > npc.lifeMax / 2)
            {
                npc.defense = 36;
                npc.damage = (int)(50f * Main.damageMultiplier);
                if (flag43)
                {
                    npc.defense *= 2;
                    npc.damage *= 2;
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    return;
                }
                npc.localAI[1] += 1f;
                if (npc.life < npc.lifeMax * 0.9)
                {
                    npc.localAI[1] += 1f;
                }
                if (npc.life < npc.lifeMax * 0.8)
                {
                    npc.localAI[1] += 1f;
                }
                if (npc.life < npc.lifeMax * 0.7)
                {
                    npc.localAI[1] += 1f;
                }
                if (npc.life < npc.lifeMax * 0.6)
                {
                    npc.localAI[1] += 1f;
                }
                if (flag43)
                {
                    npc.localAI[1] += 3f;
                }
                if (Main.expertMode)
                {
                    npc.localAI[1] += 1f;
                }
                if (Main.expertMode && npc.justHit && Main.rand.Next(2) == 0)
                {
                    npc.localAI[3] = 1f;
                }
                if (!(npc.localAI[1] > 80f))
                {
                    return;
                }
                npc.localAI[1] = 0f;
                bool flag45 = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                if (npc.localAI[3] > 0f)
                {
                    flag45 = true;
                    npc.localAI[3] = 0f;
                }
                if (flag45)
                {
                    Vector2 vector90 = new Vector2(npc.Center.X, npc.Center.Y);
                    float num747 = 15f;
                    if (Main.expertMode)
                    {
                        num747 = 17f;
                    }
                    float num748 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector90.X;
                    float num749 = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - vector90.Y;
                    float num750 = (float)Math.Sqrt(num748 * num748 + num749 * num749);
                    num750 = num747 / num750;
                    num748 *= num750;
                    num749 *= num750;
                    int num751 = 22;
                    int num752 = 275;
                    int maxValue2 = 4;
                    int maxValue3 = 8;
                    if (Main.expertMode)
                    {
                        maxValue2 = 2;
                        maxValue3 = 6;
                    }
                    if (npc.life < npc.lifeMax * 0.8 && Main.rand.Next(maxValue2) == 0)
                    {
                        num751 = 27;
                        npc.localAI[1] = -30f;
                        num752 = 276;
                    }
                    else if (npc.life < npc.lifeMax * 0.8 && Main.rand.Next(maxValue3) == 0)
                    {
                        num751 = 31;
                        npc.localAI[1] = -120f;
                        num752 = 277;
                    }
                    if (flag43)
                    {
                        num751 *= 2;
                    }
                    if (Main.expertMode)
                    {
                        num751 = (int)((double)num751 * 0.9);
                    }
                    vector90.X += num748 * 3f;
                    vector90.Y += num749 * 3f;
                    int num753 = Projectile.NewProjectile(vector90.X, vector90.Y, num748, num749, num752, num751, 0f, Main.myPlayer);
                    if (num752 != 277)
                    {
                        Main.projectile[num753].timeLeft = 300;
                    }
                }
                return;
            }
            npc.defense = 10;
            npc.damage = (int)(70f * Main.damageMultiplier);
            if (flag43)
            {
                npc.defense *= 4;
                npc.damage *= 2;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.localAI[0] == 1f)
                {
                    npc.localAI[0] = 2f;
                    for (int num754 = 0; num754 < 8; num754++)
                    {
                        int num755 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<CursedGuy>(), npc.whoAmI);
                    }
                    if (Main.expertMode)
                    {
                        for (int num756 = 0; num756 < 200; num756++)
                        {
                            if (Main.npc[num756].active && Main.npc[num756].type == ModContent.NPCType<CursedHook>())
                            {
                                for (int num757 = 0; num757 < 3; num757++)
                                {
                                    int num758 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<CursedGuy>(), npc.whoAmI);
                                    Main.npc[num758].ai[3] = num756 + 1;
                                }
                            }
                        }
                    }
                }
                else if (Main.expertMode && Main.rand.Next(60) == 0)
                {
                    int num759 = 0;
                    for (int num760 = 0; num760 < 200; num760++)
                    {
                        if (Main.npc[num760].active && Main.npc[num760].type == ModContent.NPCType<CursedGuy>() && Main.npc[num760].ai[3] == 0f)
                        {
                            num759++;
                        }
                    }
                    if (num759 < 8 && Main.rand.Next((num759 + 1) * 10) <= 1)
                    {
                        int num761 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<CursedGuy>(), npc.whoAmI);
                    }
                }
            }
            if (npc.localAI[2] == 0f)
            {
                Gore.NewGore(new Vector2(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height)), npc.velocity, 378, npc.scale);
                Gore.NewGore(new Vector2(npc.position.X + (float)Main.rand.Next(npc.width), npc.position.Y + (float)Main.rand.Next(npc.height)), npc.velocity, 379, npc.scale);
                Gore.NewGore(new Vector2(npc.position.X + (float)Main.rand.Next(npc.width), npc.position.Y + (float)Main.rand.Next(npc.height)), npc.velocity, 380, npc.scale);
                npc.localAI[2] = 1f;
            }
            npc.localAI[1] += 1f;
            if (npc.life < npc.lifeMax * 0.4)
            {
                npc.localAI[1] += 1f;
            }
            if (npc.life < npc.lifeMax * 0.3)
            {
                npc.localAI[1] += 1f;
            }
            if (npc.life < npc.lifeMax * 0.2)
            {
                npc.localAI[1] += 1f;
            }
            if (npc.life < npc.lifeMax * 0.1)
            {
                npc.localAI[1] += 1f;
            }
            if (npc.localAI[1] >= 350f)
            {
                float num762 = 8f;
                Vector2 vector91 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num763 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector91.X + Main.rand.Next(-10, 11);
                float num764 = Math.Abs(num763 * 0.2f);
                float num765 = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - vector91.Y + Main.rand.Next(-10, 11);
                if (num765 > 0f)
                {
                    num764 = 0f;
                }
                num765 -= num764;
                float num766 = (float)Math.Sqrt(num763 * num763 + num765 * num765);
                num766 = num762 / num766;
                num763 *= num766;
                num765 *= num766;
                int Spore = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, 265);
                Main.npc[Spore].velocity.X = num763;
                Main.npc[Spore].velocity.Y = num765;
                Main.npc[Spore].netUpdate = true;
                npc.localAI[1] = 0f;
            }
        }
    }


    public class CursedHook : ModNPC
    {
        public static int cursedBoss = -1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Hook"); //DONT Change me
        }
        public override void SetDefaults()
        {
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.width = 40;
            npc.height = 40;
            npc.damage = 60;
            npc.defense = 24;
            npc.lifeMax = 4000;
            npc.dontTakeDamage = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.buffImmune[20] = true;
            /*npc.lifeMax = 37000;
			npc.damage = 145;
			npc.defense = 15;
			npc.knockBackResist = 0f;
			npc.width = 42;
			npc.height = 42;
			npc.aiStyle = -1;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath20;
			npc.value = Item.buyPrice(0, 0, 0, 0);*/
        }
        public override void AI()
        {
            bool flag46 = false;
            bool flag47 = false;
            if (cursedBoss < 0)
            {
                npc.StrikeNPCNoInteraction(9999, 0f, 0);
                npc.netUpdate = true;
                return;
            }
            if (Main.player[Main.npc[cursedBoss].target].dead)
            {
                flag47 = true;
            }
            if ((cursedBoss != -1 && !Main.player[Main.npc[cursedBoss].target].ZoneJungle) || Main.player[Main.npc[cursedBoss].target].position.Y < Main.worldSurface * 16.0 || Main.player[Main.npc[cursedBoss].target].position.Y > (float)((Main.maxTilesY - 200) * 16) || flag47)
            {
                npc.localAI[0] -= 4f;
                flag46 = true;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (npc.ai[0] == 0f)
                {
                    npc.ai[0] = (int)(npc.Center.X / 16f);
                }
                if (npc.ai[1] == 0f)
                {
                    npc.ai[1] = (int)(npc.Center.X / 16f);
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.ai[0] == 0f || npc.ai[1] == 0f)
                {
                    npc.localAI[0] = 0f;
                }
                npc.localAI[0] -= 1f;
                if (Main.npc[cursedBoss].life < Main.npc[cursedBoss].lifeMax / 2)
                {
                    npc.localAI[0] -= 2f;
                }
                if (Main.npc[cursedBoss].life < Main.npc[cursedBoss].lifeMax / 4)
                {
                    npc.localAI[0] -= 2f;
                }
                if (flag46)
                {
                    npc.localAI[0] -= 6f;
                }
                if (!flag47 && npc.localAI[0] <= 0f && npc.ai[0] != 0f)
                {
                    for (int num768 = 0; num768 < 200; num768++)
                    {
                        if (num768 != npc.whoAmI && Main.npc[num768].active && Main.npc[num768].type == npc.type && (Main.npc[num768].velocity.X != 0f || Main.npc[num768].velocity.Y != 0f))
                        {
                            npc.localAI[0] = Main.rand.Next(60, 300);
                        }
                    }
                }
                if (npc.localAI[0] <= 0f)
                {
                    npc.localAI[0] = Main.rand.Next(300, 600);
                    bool flag48 = false;
                    int num769 = 0;
                    while (!flag48 && num769 <= 1000)
                    {
                        num769++;
                        int num770 = (int)(Main.player[Main.npc[cursedBoss].target].Center.X / 16f);
                        int num771 = (int)(Main.player[Main.npc[cursedBoss].target].Center.Y / 16f);
                        if (npc.ai[0] == 0f)
                        {
                            num770 = (int)((Main.player[Main.npc[cursedBoss].target].Center.X + Main.npc[cursedBoss].Center.X) / 32f);
                            num771 = (int)((Main.player[Main.npc[cursedBoss].target].Center.Y + Main.npc[cursedBoss].Center.Y) / 32f);
                        }
                        if (flag47)
                        {
                            num770 = (int)Main.npc[cursedBoss].position.X / 16;
                            num771 = (int)(Main.npc[cursedBoss].position.Y + 400f) / 16;
                        }
                        int num772 = 20;
                        num772 += (int)(100f * ((float)num769 / 1000f));
                        int num773 = num770 + Main.rand.Next(-num772, num772 + 1);
                        int num774 = num771 + Main.rand.Next(-num772, num772 + 1);
                        if (Main.npc[cursedBoss].life < Main.npc[cursedBoss].lifeMax / 2 && Main.rand.Next(6) == 0)
                        {
                            npc.TargetClosest();
                            int num775 = (int)(Main.player[npc.target].Center.X / 16f);
                            int num776 = (int)(Main.player[npc.target].Center.Y / 16f);
                            if (Main.tile[num775, num776].wall > 0)
                            {
                                num773 = num775;
                                num774 = num776;
                            }
                        }
                        try
                        {
                            if (WorldGen.SolidTile(num773, num774) || (Main.tile[num773, num774].wall > 0 && (num769 > 500 || Main.npc[cursedBoss].life < Main.npc[cursedBoss].lifeMax / 2)))
                            {
                                flag48 = true;
                                npc.ai[0] = num773;
                                npc.ai[1] = num774;
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
                float num777 = 6f;
                if (Main.npc[cursedBoss].life < Main.npc[cursedBoss].lifeMax / 2)
                {
                    num777 = 8f;
                }
                if (Main.npc[cursedBoss].life < Main.npc[cursedBoss].lifeMax / 4)
                {
                    num777 = 10f;
                }
                if (Main.expertMode)
                {
                    num777 += 1f;
                }
                if (Main.expertMode && Main.npc[cursedBoss].life < Main.npc[cursedBoss].lifeMax / 2)
                {
                    num777 += 1f;
                }
                if (flag46)
                {
                    num777 *= 2f;
                }
                if (flag47)
                {
                    num777 *= 2f;
                }
                Vector2 vector92 = new Vector2(npc.Center.X, npc.Center.Y);
                float num778 = npc.ai[0] * 16f - 8f - vector92.X;
                float num779 = npc.ai[1] * 16f - 8f - vector92.Y;
                float num780 = (float)Math.Sqrt(num778 * num778 + num779 * num779);
                if (num780 < 12f + num777)
                {
                    npc.velocity.X = num778;
                    npc.velocity.Y = num779;
                }
                else
                {
                    num780 = num777 / num780;
                    npc.velocity.X = num778 * num780;
                    npc.velocity.Y = num779 * num780;
                }
                Vector2 vector93 = new Vector2(npc.Center.X, npc.Center.Y);
                float num781 = Main.npc[cursedBoss].Center.X - vector93.X;
                float num782 = Main.npc[cursedBoss].Center.Y - vector93.Y;
                npc.rotation = (float)Math.Atan2(num782, num781) - 1.57f;
            }
        }
    }

    public class CursedGuy : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Guy");
        }
        public override void SetDefaults()
        {
            npc.width = 24;
            npc.height = 24;
            npc.damage = 60;
            npc.defense = 20;
            npc.lifeMax = 1000;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.buffImmune[20] = true;
        }
        public static int cursedBoss = -1;
        public override void AI()
        {
            if (cursedBoss < 0)
            {
                npc.StrikeNPCNoInteraction(9999, 0f, 0);
                npc.netUpdate = true;
                return;
            }
            int num783 = cursedBoss;
            if (npc.ai[3] > 0f)
            {
                num783 = (int)npc.ai[3] - 1;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] -= 1f;
                if (npc.localAI[0] <= 0f)
                {
                    npc.localAI[0] = Main.rand.Next(120, 480);
                    npc.ai[0] = Main.rand.Next(-100, 101);
                    npc.ai[1] = Main.rand.Next(-100, 101);
                    npc.netUpdate = true;
                }
            }
            npc.TargetClosest();
            float num784 = 0.2f;
            float num785 = 200f;
            if (Main.npc[cursedBoss].life < Main.npc[cursedBoss].lifeMax * 0.25)
            {
                num785 += 100f;
            }
            if (Main.npc[cursedBoss].life < Main.npc[cursedBoss].lifeMax * 0.1)
            {
                num785 += 100f;
            }
            if (Main.expertMode)
            {
                float num786 = 1f - npc.life / npc.lifeMax;
                num785 += num786 * 300f;
                num784 += 0.3f;
            }
            if (!Main.npc[num783].active || cursedBoss < 0)
            {
                npc.active = false;
                return;
            }
            float num787 = Main.npc[num783].position.X + Main.npc[num783].width / 2;
            float num788 = Main.npc[num783].position.Y + Main.npc[num783].height / 2;
            Vector2 vector94 = new Vector2(num787, num788);
            float num789 = num787 + npc.ai[0];
            float num790 = num788 + npc.ai[1];
            float num791 = num789 - vector94.X;
            float num792 = num790 - vector94.Y;
            float num793 = (float)Math.Sqrt(num791 * num791 + num792 * num792);
            num793 = num785 / num793;
            num791 *= num793;
            num792 *= num793;
            if (npc.position.X < num787 + num791)
            {
                npc.velocity.X += num784;
                if (npc.velocity.X < 0f && num791 > 0f)
                {
                    npc.velocity.X *= 0.9f;
                }
            }
            else if (npc.position.X > num787 + num791)
            {
                npc.velocity.X -= num784;
                if (npc.velocity.X > 0f && num791 < 0f)
                {
                    npc.velocity.X *= 0.9f;
                }
            }
            if (npc.position.Y < num788 + num792)
            {
                npc.velocity.Y += num784;
                if (npc.velocity.Y < 0f && num792 > 0f)
                {
                    npc.velocity.Y *= 0.9f;
                }
            }
            else if (npc.position.Y > num788 + num792)
            {
                npc.velocity.Y -= num784;
                if (npc.velocity.Y > 0f && num792 < 0f)
                {
                    npc.velocity.Y *= 0.9f;
                }
            }
            if (npc.velocity.X > 8f)
            {
                npc.velocity.X = 8f;
            }
            if (npc.velocity.X < -8f)
            {
                npc.velocity.X = -8f;
            }
            if (npc.velocity.Y > 8f)
            {
                npc.velocity.Y = 8f;
            }
            if (npc.velocity.Y < -8f)
            {
                npc.velocity.Y = -8f;
            }
            if (num791 > 0f)
            {
                npc.spriteDirection = 1;
                npc.rotation = (float)Math.Atan2(num792, num791);
            }
            if (num791 < 0f)
            {
                npc.spriteDirection = -1;
                npc.rotation = (float)Math.Atan2(num792, num791) + 3.14f;
            }
        }
    }
}