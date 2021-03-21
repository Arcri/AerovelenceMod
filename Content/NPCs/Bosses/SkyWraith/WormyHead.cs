using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.SkyWraith
{
    [AutoloadBossHead]
    public class WormyHead : ModNPC
    {
        internal bool segmentsCreated = false;
        internal int phaseTimer = 0;
        public int firstSegment = 0;
        public List<bool> HPIncrements = new List<bool>()
        {
            false,
            false,
            false,
            false
        };
        public List<int> segmentIDs = new List<int>();
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky Wraith"); //DONT Change me
            Main.npcFrameCount[npc.type] = 1;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 122663;        //this is the npc health
            npc.damage = 100;    //this is the npc damage
            npc.defense = 15;         //this is the npc defense
            npc.knockBackResist = 0f;
            npc.width = 74; //this is where you put the npc sprite width.     important
            npc.height = 76; //this is where you put the npc sprite height.   important
            npc.boss = true;
            npc.lavaImmune = true;       //this make the npc immune to lava
            npc.noGravity = true;           //this make the npc float
            npc.noTileCollide = true;        //this make the npc go tru walls
            npc.behindTiles = true;
            Main.npcFrameCount[npc.type] = 1;
            npc.value = Item.buyPrice(0, 30, 15, 7);
            npc.npcSlots = 100f;
            npc.netAlways = true;
            npc.HitSound = SoundID.NPCHit1; //Change me if you want (Rock hit sound)
            npc.DeathSound = SoundID.NPCDeath1; //Change me if you want (Heavy grunt sound)
			music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/SkyWraith");
        }
	public override void NPCLoot()
		{
			if (Main.expertMode)
			{
			Item.NewItem(npc.getRect(), mod.ItemType("FragileIceCrystal"));
		}
		switch (Main.rand.Next(3))
		{
			case 0:
			Item.NewItem(npc.getRect(), mod.ItemType("DiamondswiftBlade"));
			break;

			case 1:
			Item.NewItem(npc.getRect(), mod.ItemType("HurricaneBow"));
			break;

			case 2:
			Item.NewItem(npc.getRect(), mod.ItemType("SkyfractureBar"));
			break;
		}
	}
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 164097; //Change me
            npc.damage = 300;
            npc.defense = -100;
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override void AI()
        {
            npc.chaseable = !NPC.AnyNPCs(mod.NPCType("WormProbeCircler"));
            npc.TargetClosest(false);
            var player = Main.player[npc.target];
            Vector2 dist = player.position - npc.position;
            if (Main.netMode != NetmodeID.MultiplayerClient && !segmentsCreated)
            {
                npc.ai[3] = (float)npc.whoAmI;
                npc.realLife = npc.whoAmI;
                int currentSegment = npc.whoAmI;
                int numSegments = 40;
                for (int i = 0; i <= numSegments; i++)
                {
                    int segment = mod.NPCType("WormyBody");
                    if (i == numSegments)
                    {
                        segment = mod.NPCType("WormyTail");
                    }
                    int spawned = NPC.NewNPC((int)(npc.Center.X), (int)(npc.Center.Y), segment, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                    segmentIDs.Add(spawned);
                    if (i == 0)
                        firstSegment = spawned;
                    Main.npc[spawned].realLife = npc.whoAmI;
                    Main.npc[spawned].ai[1] = (float)currentSegment;
                    Main.npc[spawned].ai[3] = (float)npc.whoAmI;
                    Main.npc[currentSegment].localAI[0] = spawned;
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawned, 0f, 0f, 0f, 0, 0, 0);
                    currentSegment = spawned;
                }
                segmentsCreated = true;
            }
            int TilePosX = (int)(npc.position.X / 16f) - 1;
            int TileCenterX = (int)((npc.Center.X) / 16f) + 2;
            int TilePosY = (int)(npc.position.Y / 16f) - 1;
            int TileCenterY = (int)((npc.Center.Y) / 16f) + 2;
            #region World Map Antibugging
            if (TilePosX < 0)
            {
                TilePosX = 0;
            }
            if (TileCenterX > Main.maxTilesX)
            {
                TileCenterX = Main.maxTilesX;
            }
            if (TilePosY < 0)
            {
                TilePosY = 0;
            }
            if (TileCenterY > Main.maxTilesY)
            {
                TileCenterY = Main.maxTilesY;
            }
            #endregion
            float acceleration = 0.07f;
            if (player.dead)
            {
                npc.ai[1] = 3;
            }
            else
            {
                phaseTimer++;
                if (phaseTimer % 960 == 0) //16 seconds
                {
                    npc.ai[1]++;
                }
                if (npc.ai[1] >= 3) //max of 3 phases
                {
                    npc.ai[1] = 0;
                }
                if (npc.life < npc.lifeMax * 0.8 & !HPIncrements[0])
                {
                    for (int i = 0; i < segmentIDs.Count; i++)
                    {
                        if (i % 3 == 0)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                float angle = 3.141592f * 2 / 4 * j;
                                int spawned = NPC.NewNPC((int)(npc.Center.X), (int)(npc.Center.Y), mod.NPCType("WormProbeCircler"), npc.whoAmI, segmentIDs[i], angle);
                            }
                        }
                    }
                    HPIncrements[0] = true;
                }
                if (npc.life < npc.lifeMax * 0.6 & !HPIncrements[1])
                {
                    for (int i = 0; i < segmentIDs.Count; i++)
                    {
                        if (i % 3 == 0)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                float angle = 3.141592f * 2 / 4 * j;
                                int spawned = NPC.NewNPC((int)(npc.Center.X), (int)(npc.Center.Y), mod.NPCType("WormProbeCircler"), npc.whoAmI, segmentIDs[i], angle);
                            }
                        }
                    }
                    HPIncrements[1] = true;
                }
                if (npc.life < npc.lifeMax * 0.4 & !HPIncrements[2])
                {
                    for (int i = 0; i < segmentIDs.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                float angle = 3.141592f * 2 / 4 * j;
                                int spawned = NPC.NewNPC((int)(npc.Center.X), (int)(npc.Center.Y), mod.NPCType("WormProbeCircler"), npc.whoAmI, segmentIDs[i], angle);
                            }
                        }
                    }
                    HPIncrements[2] = true;
                }
                if (npc.life < npc.lifeMax * 0.2 & !HPIncrements[3])
                {
                    for (int i = 0; i < segmentIDs.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                float angle = 3.141592f * 2 / 4 * j;
                                int spawned = NPC.NewNPC((int)(npc.Center.X), (int)(npc.Center.Y), mod.NPCType("WormProbeCircler"), npc.whoAmI, segmentIDs[i], angle);
                            }
                        }
                    }
                    HPIncrements[3] = true;
                }
                if (npc.life < npc.lifeMax * 0.4 & npc.ai[1] == 0)
                {
                    npc.ai[1] = 1;
                }
                if (player.velocity.Y != 0)
                {
                    if (npc.ai[1] == 0) //passive mode
                    {
                        if (phaseTimer % 180 == 0)
                        {
                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, ProjectileID.CultistBossLightningOrb, 18, 0);
                        }
                        if (Vector2.Distance(player.Center, npc.Center) > 40)
                        {
                            float xMax = 11f;
                            float yMax = 13f;
                            if (player.Center.X < npc.Center.X & npc.velocity.X > -xMax)
                            {
                                npc.velocity.X -= acceleration * 6;
                            }
                            if (player.Center.X > npc.Center.X & npc.velocity.X < xMax)
                            {
                                npc.velocity.X += acceleration * 6;
                            }
                            if (player.Center.Y < npc.Center.Y & npc.velocity.Y > -yMax)
                            {
                                npc.velocity.Y -= acceleration * 1.2f;
                            }
                            if (player.Center.Y > npc.Center.Y & npc.velocity.Y < yMax)
                            {
                                npc.velocity.Y += acceleration * 1.2f;
                            }
                        }
                    }
                    if (npc.ai[1] == 1)//aggressive mode
                    {
                        if (phaseTimer % 100 == 0)
                        {
                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, ProjectileID.CultistBossLightningOrb, 18, 0);
                        }
                        Vector2 distnorm = player.Center - npc.Center;
                        distnorm.Normalize();
                        distnorm *= 16;
                        if (npc.velocity != Vector2.Zero)
                        {
                            if (npc.velocity.X < distnorm.X)
                            {
                                npc.velocity.X += acceleration * 6;
                            }
                            else if (npc.velocity.X > distnorm.X)
                            {
                                npc.velocity.X -= acceleration * 6;
                            }
                            if (npc.velocity.Y < distnorm.Y)
                            {
                                npc.velocity.Y += acceleration * 2;
                            }
                            else if (npc.velocity.Y > distnorm.Y)
                            {
                                npc.velocity.Y -= acceleration * 2;
                            }
                            if (distnorm.X > distnorm.Y & Math.Abs(npc.velocity.X) < 11)
                            {
                                npc.velocity.Y += acceleration * 2 * Math.Sign(player.Center.Y - npc.Center.Y);
                            }
                            if (distnorm.X < distnorm.Y & Math.Abs(npc.velocity.Y) < 16)
                            {
                                npc.velocity.X += acceleration * 6 * Math.Sign(player.Center.X - npc.Center.X);
                            }
                        }
                        else
                            npc.velocity = -Vector2.UnitY;
                    }
                    if (npc.ai[1] == 2)//medium mode
                    {
                        Vector2 rotationVector = new Vector2((float)Math.Cos(phaseTimer / 16f), (float)Math.Sin(phaseTimer / 16f));
                        Vector2 distnorm = (player.Center + rotationVector * 360) - npc.Center;
                        distnorm.Normalize();
                        distnorm *= 16;
                        if (npc.velocity != Vector2.Zero)
                        {
                            if (npc.velocity.X < distnorm.X)
                            {
                                npc.velocity.X += acceleration * 6;
                            }
                            else if (npc.velocity.X > distnorm.X)
                            {
                                npc.velocity.X -= acceleration * 6;
                            }
                            if (npc.velocity.Y < distnorm.Y)
                            {
                                npc.velocity.Y += acceleration * 2;
                            }
                            else if (npc.velocity.Y > distnorm.Y)
                            {
                                npc.velocity.Y -= acceleration * 2;
                            }
                        }
                        else
                            npc.velocity = -Vector2.UnitY;
                    }
                }
                else
                {
                    Vector2 rotationVector = new Vector2((float)Math.Cos(phaseTimer / 16f), (float)Math.Sin(phaseTimer / 16f));
                    Vector2 distnorm = (player.Center + rotationVector * 360) - npc.Center;
                    distnorm.Normalize();
                    distnorm *= 16;
                    npc.velocity.Y = distnorm.Y;
                    npc.velocity.X = (Math.Sign(player.Center.X - npc.Center.X) * (Math.Abs(player.velocity.X) + 5f));
                }
            }
            npc.rotation = Main.npc[firstSegment].rotation;
            if (npc.ai[1] == 3) //player is dead
            {
                npc.velocity.X *= 0.99f;
                npc.velocity.Y = -20;
                npc.localAI[1]++;
                if (npc.localAI[1] > 260)
                    npc.active = false;
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return true;       //this make that the npc does not have a health bar
        }
    }
}
