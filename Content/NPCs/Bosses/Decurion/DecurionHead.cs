using System;
using System.Collections.Generic;
using AerovelenceMod.Content.Items.Weapons.Thrown;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.Decurion
{
    [AutoloadBossHead]
    public class DecurionHead : ModNPC
    {
        public override void BossHeadRotation(ref float rotation) => rotation = npc.rotation;

        public bool phaseTwo = false;
        public bool EnergyBladeDash = false;
        public int damage;
        internal int phaseTimer = 0;
        public int i;
        private enum DecurionState
        {
            IdleFly = 0,
            Dash = 1,
            EnergyBladeDash = 2,
            DeathRayProbes = 3,
            SpinDash = 4,
            BodyLasers = 5
        }

        /// <summary>
        /// Manages the current AI state of the Decurion.
        /// Gets and sets npc.ai[0] as tracker.
        /// </summary>
        private DecurionState State
        {
            get => (DecurionState)npc.ai[0];
            set => npc.ai[0] = (float)value;
        }

        /// <summary>
        /// Manages several AI state attack timers.
        /// Gets and sets npc.ai[1] as tracker.
        /// </summary>
        private float AttackTimer
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }


        internal bool segmentsCreated = false;
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
            DisplayName.SetDefault("The Decurion"); //DONT Change me
            Main.npcFrameCount[npc.type] = 1;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 175000;        //this is the npc health
            npc.damage = 200;    //this is the npc damage
            npc.defense = 40;         //this is the npc defense
            npc.knockBackResist = 0f;
            npc.width = 66; //this is where you put the npc sprite width.     important
            npc.height = 112; //this is where you put the npc sprite height.   important
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
            npc.lifeMax = 200000; //Change me
            npc.damage = 300;
            npc.defense = 50;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.GetTexture("AerovelenceMod/Content/NPCs/Bosses/Decurion/DecurionHead_Glow");
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, SpriteEffects.None, 0);
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
            if (Main.netMode != NetmodeID.MultiplayerClient && !segmentsCreated)
            {
                npc.ai[3] = npc.whoAmI;
                npc.realLife = npc.whoAmI;
                int currentSegment = npc.whoAmI;
                int numSegments = 40;
                for (int i = 0; i <= numSegments; i++)
                {
                    int segmentSelected = Main.rand.Next(3);
                    int segment = mod.NPCType("DecurionBody");
                    if (segmentSelected == 0)
                    {
                        segment = ModContent.NPCType<DecurionBody>();
                    }
                    else if(segmentSelected == 1)
                    {
                        segment = ModContent.NPCType<DecurionBodyRocketSockets>();
                    }
                    else
                    {
                        segment = ModContent.NPCType<DecurionBodyGunsSockets>();
                    }
                    if (i == numSegments)
                    {
                        segment = ModContent.NPCType<DecurionTail>();
                    }
                    int spawned = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, segment, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                    segmentIDs.Add(spawned);
                    if (i == 0)
                        firstSegment = spawned;
                    Main.npc[spawned].realLife = npc.whoAmI;
                    Main.npc[spawned].ai[1] = currentSegment;
                    Main.npc[spawned].ai[3] = npc.whoAmI;
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
                if (!HPIncrements[0])
                {
                    for (int i = 0; i < segmentIDs.Count; i++)
                    {
                        if (Main.npc[segmentIDs[i]].type == ModContent.NPCType<DecurionBodyGunsSockets>())
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                float angle = 3.141592f * 2 / 4 * j;
                                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DecurionBodyGun1>(), npc.whoAmI, segmentIDs[i], angle);

                                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DecurionBodyGun2>(), npc.whoAmI, segmentIDs[i], angle);
                            }
                        }
                    }
                    for (int i = 0; i < segmentIDs.Count; i++)
                    {
                        if (Main.npc[segmentIDs[i]].type == ModContent.NPCType<DecurionBodyRocketSockets>())
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                float angle = 3.141592f * 2 / 4 * j;
                                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DecurionBodyRocket1>(), npc.whoAmI, segmentIDs[i], angle);

                                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DecurionBodyRocket2>(), npc.whoAmI, segmentIDs[i], angle);
                            }
                        }
                    }
                    HPIncrements[0] = true;
                }
                if (npc.life < npc.lifeMax * 0.4 && npc.ai[1] == 0)
                {
                    npc.ai[1] = 1;
                }
                if (player.velocity.Y != 0)
                {
                    if (npc.ai[1] == 0) //passive mode
                    {
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
           /* if(State == DecurionState.IdleFly)
            {
                npc.alpha += 5;
                Main.NewText("IdleFly");
                if (++AttackTimer >= 200)
                {
                    AttackTimer = 0;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        State = (DecurionState)Main.rand.Next(1, 5);
                    }

                    npc.netUpdate = true;
                }
            }
            else if (State == DecurionState.Dash)
            {
                Main.NewText("Dash");
                Vector2 moveTo = player.Center;
                float speed = 10f;
                Vector2 move = moveTo - npc.Center;
                float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
                move *= speed / magnitude;
                npc.velocity = move;
                if (++AttackTimer == 50)
                {
                    AttackTimer = 0;
                    State = DecurionState.IdleFly;
                }
            }
            else if (State == DecurionState.EnergyBladeDash)
            {
                Main.NewText("Energy Blade Dash");
                npc.alpha = 255;
                EnergyBladeDash = true;
                Vector2 moveTo = player.Center;
                float speed = 10f;
                Vector2 move = moveTo - npc.Center;
                float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
                move *= speed / magnitude;
                npc.velocity = move;
                if (++AttackTimer == 50)
                {
                    AttackTimer = 0;
                    State = DecurionState.IdleFly;
                }
            }*/
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
