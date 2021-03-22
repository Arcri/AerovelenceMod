using System;
using System.Collections.Generic;
using AerovelenceMod.Content.Items.Weapons.Thrown;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.VoidReaver
{
    [AutoloadBossHead]
    public class VoidReaverHead : ModNPC
    {

        public bool phaseTwo = false;
        private bool Spawned;
        public int damage;
        public int i;
        float dynamicCounter = 0;
        private enum VoidReaverState
        {
            IdleFly = 0,
            TimeAuraSpawn = 1,
            SuperDash = 2,
            BodyLasers = 3,
            TimeBlades = 4,
            EnergyProjectiles = 5
        }

        /// <summary>
        /// Manages the current AI state of the Crystal Tumbler.
        /// Gets and sets npc.ai[0] as tracker.
        /// </summary>
        private VoidReaverState State
        {
            get => (VoidReaverState)npc.ai[0];
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
            DisplayName.SetDefault("Sky Wraith"); //DONT Change me
            Main.npcFrameCount[npc.type] = 1;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 122663;        //this is the npc health
            npc.damage = 100;    //this is the npc damage
            npc.defense = 15;         //this is the npc defense
            npc.knockBackResist = 0f;
            npc.width = 114; //this is where you put the npc sprite width.     important
            npc.height = 180; //this is where you put the npc sprite height.   important
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
            i++;
            npc.rotation = Main.npc[firstSegment].rotation;
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            if (npc.life <= npc.lifeMax / 2)
            {
                phaseTwo = true;
            }

            if (State == VoidReaverState.IdleFly)
            {
                Main.NewText("Idle Fly");
                WormMovement(player);
                if (++AttackTimer >= 200)
                {
                    AttackTimer = 0;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        State = (VoidReaverState)Main.rand.Next(1, 6);
                    }

                    npc.netUpdate = true;
                }
            }
            else if (State == VoidReaverState.TimeAuraSpawn)
            {
                Main.NewText("Time Aura");
                for (int i = 0; i < 3 + (Main.expertMode ? 1 : 0); i++)
                {
                    Vector2 toLocation = player.Center + new Vector2(Main.rand.NextFloat(300, 600), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(360)));
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        damage = 0;
                        Projectile.NewProjectile(toLocation, Vector2.Zero, ModContent.ProjectileType<DarkDaggerProjectile>(), damage, 0, Main.myPlayer, player.whoAmI);
                    }
                    Vector2 toLocationVelo = toLocation - player.Center;
                    Vector2 from = npc.Center;
                    for (int j = 0; j < 300; j++)
                    {
                        Vector2 velo = toLocationVelo.SafeNormalize(Vector2.Zero);
                        from += velo * 12;
                        Vector2 circularLocation = new Vector2(10, 0).RotatedBy(MathHelper.ToRadians(j * 12 + dynamicCounter));

                        int dust = Dust.NewDust(from + new Vector2(-4, -4) + circularLocation, 0, 0, 164, 0, 0, 0, default, 1.25f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.1f;
                        Main.dust[dust].scale = 1.8f;

                        if ((from - toLocation).Length() < 24)
                        {
                            break;
                        }
                    }
                }
                State = VoidReaverState.IdleFly;
            }
            else if (State == VoidReaverState.SuperDash)
            {
                Main.NewText("Super Dash");
                Vector2 moveTo = player.Center;
                float speed = 10f;
                Vector2 move = moveTo - npc.Center;
                float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
                move *= speed / magnitude;
                if (i % 100 == 0)
                {
                    npc.velocity = move;
                }
                if (i == 200)
                {
                    State = VoidReaverState.IdleFly;
                }
            }
            else if (State == VoidReaverState.BodyLasers)
            {
                State = VoidReaverState.IdleFly;
            }
            else if (State == VoidReaverState.TimeBlades)
            {
                State = VoidReaverState.IdleFly;
            }
            else if (State == VoidReaverState.EnergyProjectiles)
            {
                State = VoidReaverState.IdleFly;
            }
        }

        private void WormMovement(Player player)
        {
            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 1.57f;
            npc.TargetClosest(false);
            if (Main.netMode != NetmodeID.MultiplayerClient && !segmentsCreated)
            {
                npc.ai[3] = npc.whoAmI;
                npc.realLife = npc.whoAmI;

                if (!Spawned && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
                    for (int segmentSpawn = 0; segmentSpawn < 60; segmentSpawn++)
                    {
                        int segment = (int)((segmentSpawn < 0 || segmentSpawn >= 59) ? (float)NPC.NewNPC((int)(npc.position.X + npc.width / 2), (int)(npc.position.Y + npc.height), ModContent.NPCType<VoidReaverTail>(), npc.whoAmI) : NPC.NewNPC((int)(npc.position.X + npc.width / 2), (int)(npc.position.Y + npc.height), ModContent.NPCType<VoidReaverBody>(), npc.whoAmI));
                        Main.npc[segment].realLife = npc.whoAmI;
                        Main.npc[segment].ai[2] = npc.whoAmI;
                        Main.npc[segment].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = segment;
                        npc.netUpdate = true;
                        Previous = segment;
                    }
                    Spawned = true;
                }
            }
            float acceleration = 0.30f;
            if (player.dead)
            {
                npc.ai[1] = 3;
            }
            else
            {
                if (npc.ai[1] >= 1) //max of 3 phases
                {
                    npc.ai[1] = 0;
                }
                if (npc.life < npc.lifeMax * 0.4 & npc.ai[1] == 0)
                {
                    npc.ai[1] = 1;
                }
                if (player.velocity.Y != 0)
                {
                        Vector2 rotationVector = new Vector2((float)Math.Cos(AttackTimer / 16f), (float)Math.Sin(AttackTimer / 16f));
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
                else
                {
                    Vector2 rotationVector = new Vector2((float)Math.Cos(AttackTimer / 16f), (float)Math.Sin(AttackTimer / 16f));
                    Vector2 distnorm = (player.Center + rotationVector * 360) - npc.Center;
                    distnorm.Normalize();
                    distnorm *= 16;
                    npc.velocity.Y = distnorm.Y;
                    npc.velocity.X = (Math.Sign(player.Center.X - npc.Center.X) * (Math.Abs(player.velocity.X) + 5f));
                }
            }
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
