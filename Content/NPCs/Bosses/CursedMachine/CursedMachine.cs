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
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.noTileCollide = true;
            NPC.width = 86;
            NPC.height = 86;
            NPC.damage = 50;
            NPC.defense = 14;
            NPC.lifeMax = 30000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 15);
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.npcSlots = 16f;
            NPC.buffImmune[20] = true;
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
            NPC.lifeMax = 600000;
            NPC.damage = 360;
        }
        public override void AI()
        {
            bool flag43 = false;
            bool flag44 = false;
            NPC.TargetClosest();
            if (Main.player[NPC.target].dead)
            {
                flag44 = true;
                flag43 = true;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int num733 = 6000;
                if (Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) + Math.Abs(NPC.Center.Y - Main.player[NPC.target].Center.Y) > (float)num733)
                {
                    NPC.active = false;
                    NPC.life = 0;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                    }
                }
            }
            cursedBoss = NPC.whoAmI;
            if (NPC.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] = 1f;
                int num734 = NPC.NewNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<CursedHook>(), NPC.whoAmI);
                num734 = NPC.NewNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<CursedHook>(), NPC.whoAmI);
                num734 = NPC.NewNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<CursedHook>(), NPC.whoAmI);
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
            if (NPC.life < NPC.lifeMax / 2)
            {
                velocity = 5f;
                num740 = 0.05f;
            }
            if (NPC.life < NPC.lifeMax / 4)
            {
                velocity = 7f;
            }
            if (!Main.player[NPC.target].ZoneJungle || Main.player[NPC.target].position.Y < Main.worldSurface * 16.0 || Main.player[NPC.target].position.Y > (Main.maxTilesY - 200) * 16)
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
            float num741 = Main.player[NPC.target].Center.X - vector88.X;
            float num742 = Main.player[NPC.target].Center.Y - vector88.Y;
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
            vector88 = new Vector2(NPC.Center.X, NPC.Center.Y);
            num741 = num735 - vector88.X;
            num742 = num736 - vector88.Y;
            num743 = (float)Math.Sqrt(num741 * num741 + num742 * num742);
            if (num743 < velocity)
            {
                num741 = NPC.velocity.X;
                num742 = NPC.velocity.Y;
            }
            else
            {
                num743 = velocity / num743;
                num741 *= num743;
                num742 *= num743;
            }
            if (NPC.velocity.X < num741)
            {
                NPC.velocity.X += num740;
                if (NPC.velocity.X < 0f && num741 > 0f)
                {
                    NPC.velocity.X += num740 * 2f;
                }
            }
            else if (NPC.velocity.X > num741)
            {
                NPC.velocity.X -= num740;
                if (NPC.velocity.X > 0f && num741 < 0f)
                {
                    NPC.velocity.X -= num740 * 2f;
                }
            }
            if (NPC.velocity.Y < num742)
            {
                NPC.velocity.Y += num740;
                if (NPC.velocity.Y < 0f && num742 > 0f)
                {
                    NPC.velocity.Y += num740 * 2f;
                }
            }
            else if (NPC.velocity.Y > num742)
            {
                NPC.velocity.Y -= num740;
                if (NPC.velocity.Y > 0f && num742 < 0f)
                {
                    NPC.velocity.Y -= num740 * 2f;
                }
            }
            Vector2 vector89 = new Vector2(NPC.Center.X, NPC.Center.Y);
            float num745 = Main.player[NPC.target].Center.X - vector89.X;
            float num746 = Main.player[NPC.target].Center.Y - vector89.Y;
            NPC.rotation = (float)Math.Atan2(num746, num745) + 1.57f;
            if (NPC.life > NPC.lifeMax / 2)
            {
                NPC.defense = 36;
                NPC.damage = (int)(50f * Main.damageMultiplier);
                if (flag43)
                {
                    NPC.defense *= 2;
                    NPC.damage *= 2;
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    return;
                }
                NPC.localAI[1] += 1f;
                if (NPC.life < NPC.lifeMax * 0.9)
                {
                    NPC.localAI[1] += 1f;
                }
                if (NPC.life < NPC.lifeMax * 0.8)
                {
                    NPC.localAI[1] += 1f;
                }
                if (NPC.life < NPC.lifeMax * 0.7)
                {
                    NPC.localAI[1] += 1f;
                }
                if (NPC.life < NPC.lifeMax * 0.6)
                {
                    NPC.localAI[1] += 1f;
                }
                if (flag43)
                {
                    NPC.localAI[1] += 3f;
                }
                if (Main.expertMode)
                {
                    NPC.localAI[1] += 1f;
                }
                if (Main.expertMode && NPC.justHit && Main.rand.Next(2) == 0)
                {
                    NPC.localAI[3] = 1f;
                }
                if (!(NPC.localAI[1] > 80f))
                {
                    return;
                }
                NPC.localAI[1] = 0f;
                bool flag45 = Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
                if (NPC.localAI[3] > 0f)
                {
                    flag45 = true;
                    NPC.localAI[3] = 0f;
                }
                if (flag45)
                {
                    Vector2 vector90 = new Vector2(NPC.Center.X, NPC.Center.Y);
                    float num747 = 15f;
                    if (Main.expertMode)
                    {
                        num747 = 17f;
                    }
                    float num748 = Main.player[NPC.target].position.X + Main.player[NPC.target].width * 0.5f - vector90.X;
                    float num749 = Main.player[NPC.target].position.Y + Main.player[NPC.target].height * 0.5f - vector90.Y;
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
                    if (NPC.life < NPC.lifeMax * 0.8 && Main.rand.Next(maxValue2) == 0)
                    {
                        num751 = 27;
                        NPC.localAI[1] = -30f;
                        num752 = 276;
                    }
                    else if (NPC.life < NPC.lifeMax * 0.8 && Main.rand.Next(maxValue3) == 0)
                    {
                        num751 = 31;
                        NPC.localAI[1] = -120f;
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
            NPC.defense = 10;
            NPC.damage = (int)(70f * Main.damageMultiplier);
            if (flag43)
            {
                NPC.defense *= 4;
                NPC.damage *= 2;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.localAI[0] == 1f)
                {
                    NPC.localAI[0] = 2f;
                    for (int num754 = 0; num754 < 8; num754++)
                    {
                        int num755 = NPC.NewNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<CursedGuy>(), NPC.whoAmI);
                    }
                    if (Main.expertMode)
                    {
                        for (int num756 = 0; num756 < 200; num756++)
                        {
                            if (Main.npc[num756].active && Main.npc[num756].type == ModContent.NPCType<CursedHook>())
                            {
                                for (int num757 = 0; num757 < 3; num757++)
                                {
                                    int num758 = NPC.NewNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<CursedGuy>(), NPC.whoAmI);
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
                        int num761 = NPC.NewNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<CursedGuy>(), NPC.whoAmI);
                    }
                }
            }
            if (NPC.localAI[2] == 0f)
            {
                Gore.NewGore(new Vector2(NPC.position.X + Main.rand.Next(NPC.width), NPC.position.Y + Main.rand.Next(NPC.height)), NPC.velocity, 378, NPC.scale);
                Gore.NewGore(new Vector2(NPC.position.X + (float)Main.rand.Next(NPC.width), NPC.position.Y + (float)Main.rand.Next(NPC.height)), NPC.velocity, 379, NPC.scale);
                Gore.NewGore(new Vector2(NPC.position.X + (float)Main.rand.Next(NPC.width), NPC.position.Y + (float)Main.rand.Next(NPC.height)), NPC.velocity, 380, NPC.scale);
                NPC.localAI[2] = 1f;
            }
            NPC.localAI[1] += 1f;
            if (NPC.life < NPC.lifeMax * 0.4)
            {
                NPC.localAI[1] += 1f;
            }
            if (NPC.life < NPC.lifeMax * 0.3)
            {
                NPC.localAI[1] += 1f;
            }
            if (NPC.life < NPC.lifeMax * 0.2)
            {
                NPC.localAI[1] += 1f;
            }
            if (NPC.life < NPC.lifeMax * 0.1)
            {
                NPC.localAI[1] += 1f;
            }
            if (NPC.localAI[1] >= 350f)
            {
                float num762 = 8f;
                Vector2 vector91 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                float num763 = Main.player[NPC.target].position.X + Main.player[NPC.target].width * 0.5f - vector91.X + Main.rand.Next(-10, 11);
                float num764 = Math.Abs(num763 * 0.2f);
                float num765 = Main.player[NPC.target].position.Y + Main.player[NPC.target].height * 0.5f - vector91.Y + Main.rand.Next(-10, 11);
                if (num765 > 0f)
                {
                    num764 = 0f;
                }
                num765 -= num764;
                float num766 = (float)Math.Sqrt(num763 * num763 + num765 * num765);
                num766 = num762 / num766;
                num763 *= num766;
                num765 *= num766;
                int Spore = NPC.NewNPC((int)NPC.Center.X, (int)NPC.Center.Y, 265);
                Main.npc[Spore].velocity.X = num763;
                Main.npc[Spore].velocity.Y = num765;
                Main.npc[Spore].netUpdate = true;
                NPC.localAI[1] = 0f;
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
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.width = 40;
            NPC.height = 40;
            NPC.damage = 60;
            NPC.defense = 24;
            NPC.lifeMax = 4000;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.buffImmune[20] = true;
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
                NPC.StrikeNPCNoInteraction(9999, 0f, 0);
                NPC.netUpdate = true;
                return;
            }
            if (Main.player[Main.npc[cursedBoss].target].dead)
            {
                flag47 = true;
            }
            if ((cursedBoss != -1 && !Main.player[Main.npc[cursedBoss].target].ZoneJungle) || Main.player[Main.npc[cursedBoss].target].position.Y < Main.worldSurface * 16.0 || Main.player[Main.npc[cursedBoss].target].position.Y > (float)((Main.maxTilesY - 200) * 16) || flag47)
            {
                NPC.localAI[0] -= 4f;
                flag46 = true;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f)
                {
                    NPC.ai[0] = (int)(NPC.Center.X / 16f);
                }
                if (NPC.ai[1] == 0f)
                {
                    NPC.ai[1] = (int)(NPC.Center.X / 16f);
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f || NPC.ai[1] == 0f)
                {
                    NPC.localAI[0] = 0f;
                }
                NPC.localAI[0] -= 1f;
                if (Main.npc[cursedBoss].life < Main.npc[cursedBoss].lifeMax / 2)
                {
                    NPC.localAI[0] -= 2f;
                }
                if (Main.npc[cursedBoss].life < Main.npc[cursedBoss].lifeMax / 4)
                {
                    NPC.localAI[0] -= 2f;
                }
                if (flag46)
                {
                    NPC.localAI[0] -= 6f;
                }
                if (!flag47 && NPC.localAI[0] <= 0f && NPC.ai[0] != 0f)
                {
                    for (int num768 = 0; num768 < 200; num768++)
                    {
                        if (num768 != NPC.whoAmI && Main.npc[num768].active && Main.npc[num768].type == NPC.type && (Main.npc[num768].velocity.X != 0f || Main.npc[num768].velocity.Y != 0f))
                        {
                            NPC.localAI[0] = Main.rand.Next(60, 300);
                        }
                    }
                }
                if (NPC.localAI[0] <= 0f)
                {
                    NPC.localAI[0] = Main.rand.Next(300, 600);
                    bool flag48 = false;
                    int num769 = 0;
                    while (!flag48 && num769 <= 1000)
                    {
                        num769++;
                        int num770 = (int)(Main.player[Main.npc[cursedBoss].target].Center.X / 16f);
                        int num771 = (int)(Main.player[Main.npc[cursedBoss].target].Center.Y / 16f);
                        if (NPC.ai[0] == 0f)
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
                            NPC.TargetClosest();
                            int num775 = (int)(Main.player[NPC.target].Center.X / 16f);
                            int num776 = (int)(Main.player[NPC.target].Center.Y / 16f);
                            if (Main.tile[num775, num776].WallType > 0)
                            {
                                num773 = num775;
                                num774 = num776;
                            }
                        }
                        try
                        {
                            if (WorldGen.SolidTile(num773, num774) || (Main.tile[num773, num774].WallType > 0 && (num769 > 500 || Main.npc[cursedBoss].life < Main.npc[cursedBoss].lifeMax / 2)))
                            {
                                flag48 = true;
                                NPC.ai[0] = num773;
                                NPC.ai[1] = num774;
                                NPC.netUpdate = true;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            if (NPC.ai[0] > 0f && NPC.ai[1] > 0f)
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
                Vector2 vector92 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num778 = NPC.ai[0] * 16f - 8f - vector92.X;
                float num779 = NPC.ai[1] * 16f - 8f - vector92.Y;
                float num780 = (float)Math.Sqrt(num778 * num778 + num779 * num779);
                if (num780 < 12f + num777)
                {
                    NPC.velocity.X = num778;
                    NPC.velocity.Y = num779;
                }
                else
                {
                    num780 = num777 / num780;
                    NPC.velocity.X = num778 * num780;
                    NPC.velocity.Y = num779 * num780;
                }
                Vector2 vector93 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num781 = Main.npc[cursedBoss].Center.X - vector93.X;
                float num782 = Main.npc[cursedBoss].Center.Y - vector93.Y;
                NPC.rotation = (float)Math.Atan2(num782, num781) - 1.57f;
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
            NPC.width = 24;
            NPC.height = 24;
            NPC.damage = 60;
            NPC.defense = 20;
            NPC.lifeMax = 1000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.buffImmune[20] = true;
        }
        public static int cursedBoss = -1;
        public override void AI()
        {
            if (cursedBoss < 0)
            {
                NPC.StrikeNPCNoInteraction(9999, 0f, 0);
                NPC.netUpdate = true;
                return;
            }
            int num783 = cursedBoss;
            if (NPC.ai[3] > 0f)
            {
                num783 = (int)NPC.ai[3] - 1;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] -= 1f;
                if (NPC.localAI[0] <= 0f)
                {
                    NPC.localAI[0] = Main.rand.Next(120, 480);
                    NPC.ai[0] = Main.rand.Next(-100, 101);
                    NPC.ai[1] = Main.rand.Next(-100, 101);
                    NPC.netUpdate = true;
                }
            }
            NPC.TargetClosest();
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
                float num786 = 1f - NPC.life / NPC.lifeMax;
                num785 += num786 * 300f;
                num784 += 0.3f;
            }
            if (!Main.npc[num783].active || cursedBoss < 0)
            {
                NPC.active = false;
                return;
            }
            float num787 = Main.npc[num783].position.X + Main.npc[num783].width / 2;
            float num788 = Main.npc[num783].position.Y + Main.npc[num783].height / 2;
            Vector2 vector94 = new Vector2(num787, num788);
            float num789 = num787 + NPC.ai[0];
            float num790 = num788 + NPC.ai[1];
            float num791 = num789 - vector94.X;
            float num792 = num790 - vector94.Y;
            float num793 = (float)Math.Sqrt(num791 * num791 + num792 * num792);
            num793 = num785 / num793;
            num791 *= num793;
            num792 *= num793;
            if (NPC.position.X < num787 + num791)
            {
                NPC.velocity.X += num784;
                if (NPC.velocity.X < 0f && num791 > 0f)
                {
                    NPC.velocity.X *= 0.9f;
                }
            }
            else if (NPC.position.X > num787 + num791)
            {
                NPC.velocity.X -= num784;
                if (NPC.velocity.X > 0f && num791 < 0f)
                {
                    NPC.velocity.X *= 0.9f;
                }
            }
            if (NPC.position.Y < num788 + num792)
            {
                NPC.velocity.Y += num784;
                if (NPC.velocity.Y < 0f && num792 > 0f)
                {
                    NPC.velocity.Y *= 0.9f;
                }
            }
            else if (NPC.position.Y > num788 + num792)
            {
                NPC.velocity.Y -= num784;
                if (NPC.velocity.Y > 0f && num792 < 0f)
                {
                    NPC.velocity.Y *= 0.9f;
                }
            }
            if (NPC.velocity.X > 8f)
            {
                NPC.velocity.X = 8f;
            }
            if (NPC.velocity.X < -8f)
            {
                NPC.velocity.X = -8f;
            }
            if (NPC.velocity.Y > 8f)
            {
                NPC.velocity.Y = 8f;
            }
            if (NPC.velocity.Y < -8f)
            {
                NPC.velocity.Y = -8f;
            }
            if (num791 > 0f)
            {
                NPC.spriteDirection = 1;
                NPC.rotation = (float)Math.Atan2(num792, num791);
            }
            if (num791 < 0f)
            {
                NPC.spriteDirection = -1;
                NPC.rotation = (float)Math.Atan2(num792, num791) + 3.14f;
            }
        }
    }
}