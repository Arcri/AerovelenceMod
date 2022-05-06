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
        public override void BossHeadRotation(ref float rotation) => rotation = NPC.rotation;

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
            get => (DecurionState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        /// <summary>
        /// Manages several AI state attack timers.
        /// Gets and sets npc.ai[1] as tracker.
        /// </summary>
        private float AttackTimer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
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
            Main.npcFrameCount[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 175000;        //this is the npc health
            NPC.damage = 200;    //this is the npc damage
            NPC.defense = 40;         //this is the npc defense
            NPC.knockBackResist = 0f;
            NPC.width = 66; //this is where you put the npc sprite width.     important
            NPC.height = 112; //this is where you put the npc sprite height.   important
            NPC.boss = true;
            NPC.lavaImmune = true;       //this make the npc immune to lava
            NPC.noGravity = true;           //this make the npc float
            NPC.noTileCollide = true;        //this make the npc go tru walls
            NPC.behindTiles = true;
            Main.npcFrameCount[NPC.type] = 1;
            NPC.value = Item.buyPrice(0, 30, 15, 7);
            NPC.npcSlots = 100f;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit1; //Change me if you want (Rock hit sound)
            NPC.DeathSound = SoundID.NPCDeath1; //Change me if you want (Heavy grunt sound)
            music = Mod.GetSoundSlot(SoundType.Music, "Sounds/Music/SkyWraith");
        }
        public override void NPCLoot()
        {
            if (Main.expertMode)
            {
                Item.NewItem(NPC.getRect(), Mod.Find<ModItem>("FragileIceCrystal").Type);
            }
            switch (Main.rand.Next(3))
            {
                case 0:
                    Item.NewItem(NPC.getRect(), Mod.Find<ModItem>("DiamondswiftBlade").Type);
                    break;

                case 1:
                    Item.NewItem(NPC.getRect(), Mod.Find<ModItem>("HurricaneBow").Type);
                    break;

                case 2:
                    Item.NewItem(NPC.getRect(), Mod.Find<ModItem>("SkyfractureBar").Type);
                    break;
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 200000; //Change me
            NPC.damage = 300;
            NPC.defense = 50;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Decurion/DecurionHead_Glow");
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override void AI()
        {
            NPC.chaseable = !NPC.AnyNPCs(Mod.Find<ModNPC>("WormProbeCircler").Type);
            NPC.TargetClosest(false);
            var player = Main.player[NPC.target];
            if (Main.netMode != NetmodeID.MultiplayerClient && !segmentsCreated)
            {
                NPC.ai[3] = NPC.whoAmI;
                NPC.realLife = NPC.whoAmI;
                int currentSegment = NPC.whoAmI;
                int numSegments = 40;
                for (int i = 0; i <= numSegments; i++)
                {
                    int segmentSelected = Main.rand.Next(3);
                    int segment = Mod.Find<ModNPC>("DecurionBody").Type;
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
                    int spawned = NPC.NewNPC((int)NPC.Center.X, (int)NPC.Center.Y, segment, NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                    segmentIDs.Add(spawned);
                    if (i == 0)
                        firstSegment = spawned;
                    Main.npc[spawned].realLife = NPC.whoAmI;
                    Main.npc[spawned].ai[1] = currentSegment;
                    Main.npc[spawned].ai[3] = NPC.whoAmI;
                    Main.npc[currentSegment].localAI[0] = spawned;
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawned, 0f, 0f, 0f, 0, 0, 0);
                    currentSegment = spawned;
                }
                segmentsCreated = true;
            }
            int TilePosX = (int)(NPC.position.X / 16f) - 1;
            int TileCenterX = (int)((NPC.Center.X) / 16f) + 2;
            int TilePosY = (int)(NPC.position.Y / 16f) - 1;
            int TileCenterY = (int)((NPC.Center.Y) / 16f) + 2;
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
                NPC.ai[1] = 3;
            }
            else
            {
                phaseTimer++;
                if (phaseTimer % 960 == 0) //16 seconds
                {
                    NPC.ai[1]++;
                }
                if (NPC.ai[1] >= 3) //max of 3 phases
                {
                    NPC.ai[1] = 0;
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
                                NPC.NewNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DecurionBodyGun1>(), NPC.whoAmI, segmentIDs[i], angle);

                                NPC.NewNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DecurionBodyGun2>(), NPC.whoAmI, segmentIDs[i], angle);
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
                                NPC.NewNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DecurionBodyRocket1>(), NPC.whoAmI, segmentIDs[i], angle);

                                NPC.NewNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DecurionBodyRocket2>(), NPC.whoAmI, segmentIDs[i], angle);
                            }
                        }
                    }
                    HPIncrements[0] = true;
                }
                if (NPC.life < NPC.lifeMax * 0.4 && NPC.ai[1] == 0)
                {
                    NPC.ai[1] = 1;
                }
                if (player.velocity.Y != 0)
                {
                    if (NPC.ai[1] == 0) //passive mode
                    {
                        if (Vector2.Distance(player.Center, NPC.Center) > 40)
                        {
                            float xMax = 11f;
                            float yMax = 13f;
                            if (player.Center.X < NPC.Center.X & NPC.velocity.X > -xMax)
                            {
                                NPC.velocity.X -= acceleration * 6;
                            }
                            if (player.Center.X > NPC.Center.X & NPC.velocity.X < xMax)
                            {
                                NPC.velocity.X += acceleration * 6;
                            }
                            if (player.Center.Y < NPC.Center.Y & NPC.velocity.Y > -yMax)
                            {
                                NPC.velocity.Y -= acceleration * 1.2f;
                            }
                            if (player.Center.Y > NPC.Center.Y & NPC.velocity.Y < yMax)
                            {
                                NPC.velocity.Y += acceleration * 1.2f;
                            }
                        }
                    }
                    if (NPC.ai[1] == 1)//aggressive mode
                    {
                        Vector2 distnorm = player.Center - NPC.Center;
                        distnorm.Normalize();
                        distnorm *= 16;
                        if (NPC.velocity != Vector2.Zero)
                        {
                            if (NPC.velocity.X < distnorm.X)
                            {
                                NPC.velocity.X += acceleration * 6;
                            }
                            else if (NPC.velocity.X > distnorm.X)
                            {
                                NPC.velocity.X -= acceleration * 6;
                            }
                            if (NPC.velocity.Y < distnorm.Y)
                            {
                                NPC.velocity.Y += acceleration * 2;
                            }
                            else if (NPC.velocity.Y > distnorm.Y)
                            {
                                NPC.velocity.Y -= acceleration * 2;
                            }
                            if (distnorm.X > distnorm.Y & Math.Abs(NPC.velocity.X) < 11)
                            {
                                NPC.velocity.Y += acceleration * 2 * Math.Sign(player.Center.Y - NPC.Center.Y);
                            }
                            if (distnorm.X < distnorm.Y & Math.Abs(NPC.velocity.Y) < 16)
                            {
                                NPC.velocity.X += acceleration * 6 * Math.Sign(player.Center.X - NPC.Center.X);
                            }
                        }
                        else
                            NPC.velocity = -Vector2.UnitY;
                    }
                    if (NPC.ai[1] == 2)//medium mode
                    {
                        Vector2 rotationVector = new Vector2((float)Math.Cos(phaseTimer / 16f), (float)Math.Sin(phaseTimer / 16f));
                        Vector2 distnorm = (player.Center + rotationVector * 360) - NPC.Center;
                        distnorm.Normalize();
                        distnorm *= 16;
                        if (NPC.velocity != Vector2.Zero)
                        {
                            if (NPC.velocity.X < distnorm.X)
                            {
                                NPC.velocity.X += acceleration * 6;
                            }
                            else if (NPC.velocity.X > distnorm.X)
                            {
                                NPC.velocity.X -= acceleration * 6;
                            }
                            if (NPC.velocity.Y < distnorm.Y)
                            {
                                NPC.velocity.Y += acceleration * 2;
                            }
                            else if (NPC.velocity.Y > distnorm.Y)
                            {
                                NPC.velocity.Y -= acceleration * 2;
                            }
                        }
                        else
                            NPC.velocity = -Vector2.UnitY;
                    }
                }
                else
                {
                    Vector2 rotationVector = new Vector2((float)Math.Cos(phaseTimer / 16f), (float)Math.Sin(phaseTimer / 16f));
                    Vector2 distnorm = (player.Center + rotationVector * 360) - NPC.Center;
                    distnorm.Normalize();
                    distnorm *= 16;
                    NPC.velocity.Y = distnorm.Y;
                    NPC.velocity.X = (Math.Sign(player.Center.X - NPC.Center.X) * (Math.Abs(player.velocity.X) + 5f));
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
            NPC.rotation = Main.npc[firstSegment].rotation;
            if (NPC.ai[1] == 3) //player is dead
            {
                NPC.velocity.X *= 0.99f;
                NPC.velocity.Y = -20;
                NPC.localAI[1]++;
                if (NPC.localAI[1] > 260)
                    NPC.active = false;
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return true;       //this make that the npc does not have a health bar
        }
    }
}
