using AerovelenceMod.Content.Biomes;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using static AerovelenceMod.Content.Projectiles.LightningUtility;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class Condurtle : ModNPC
    {
        private const int WALK_FRAMES = 6; //1-6
        private const int IDLE_FRAMES = 4; //7-10
        private const int SHELL_ENTER_FRAMES = 8; //11-18
        private const int ATTACK_FRAMES = 2; //19-20
        private const int SHELL_EXIT_FRAMES = 6; //21-26

        private const int WALK_START = 0;
        private const int IDLE_START = WALK_FRAMES;
        private const int SHELL_ENTER_START = IDLE_START + IDLE_FRAMES;
        private const int ATTACK_START = SHELL_ENTER_START + SHELL_ENTER_FRAMES;
        private const int SHELL_EXIT_START = ATTACK_START + ATTACK_FRAMES;

        private enum AIState
        {
            Idle,
            Walking,
            EnteringShell,
            InShell,
            AttackingInShell,
            ExitingShell,
            PostAttackWalking
        }

        private AIState currentState = AIState.Walking;
        private int frameCounter;
        private int stateTimer;
        private int attackCooldown;
        private bool initialized;
        private bool facingRight = true;

        private const float DETECTION_RANGE = 250f;
        private const int ATTACK_COOLDOWN = 240;
        private const int POST_ATTACK_WALK_TIME = 60;
        private const int SHELL_MAX_TIME = 300;
        private int conductorID = -1;
        private int pylonID = -1;
        private bool needsNewConductor = false;
        private Vector2 lastShellPosition;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = WALK_FRAMES + IDLE_FRAMES + SHELL_ENTER_FRAMES + ATTACK_FRAMES + SHELL_EXIT_FRAMES;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Position = new Vector2(0f, 8f),
                PortraitPositionXOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 72;
            NPC.height = 40;
            NPC.damage = 15;
            NPC.defense = 12;
            NPC.lifeMax = 120;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 150f;
            NPC.knockBackResist = 0.2f;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.rotation = 0f;

            SpawnModBiomes = new int[] { ModContent.GetInstance<CrystalCavernsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("A curious turtle with conductive properties. It can generate electrical currents when threatened, using crystals to channel its energy.")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsBiome>()))
            {
                return SpawnCondition.OverworldNightMonster.Chance * 0.3f;
            }
            return 0f;
        }

        public override void AI()
        {
            if (!initialized)
            {
                attackCooldown = Main.rand.Next(ATTACK_COOLDOWN, ATTACK_COOLDOWN * 2);
                facingRight = Main.rand.NextBool();
                initialized = true;
            }

            NPC.velocity.Y += 0.3f;
            if (NPC.velocity.Y > 8f)
                NPC.velocity.Y = 8f;
            Player target = FindTarget();
            UpdateProjectileReferences();
            switch (currentState)
            {
                case AIState.Walking:
                    UpdateWalkingState(target);
                    break;
                case AIState.Idle:
                    UpdateIdleState(target);
                    break;
                case AIState.EnteringShell:
                    UpdateEnteringShellState();
                    break;
                case AIState.InShell:
                    UpdateInShellState();
                    break;
                case AIState.AttackingInShell:
                    UpdateAttackingInShellState(target);
                    break;
                case AIState.ExitingShell:
                    UpdateExitingShellState();
                    break;
                case AIState.PostAttackWalking:
                    UpdatePostAttackWalkingState();
                    break;
            }
            if (attackCooldown > 0)
                attackCooldown--;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/CrystalCaverns/Condurtle_Glow").Value;

            SpriteEffects effects = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 origin = new Vector2(NPC.frame.Width / 2, NPC.frame.Height / 2);

            spriteBatch.Draw(glowmask, drawPos, NPC.frame, Color.White, NPC.rotation, origin, NPC.scale, effects, 0f);
        }


        private void UpdateProjectileReferences()
        {
            if (conductorID != -1)
            {
                if (!Main.projectile[conductorID].active || Main.projectile[conductorID].type != ModContent.ProjectileType<CondurtleConductor>())
                {
                    conductorID = -1;
                    if (currentState == AIState.InShell && pylonID != -1 &&
                        Main.projectile[pylonID].active && Main.projectile[pylonID].type == ModContent.ProjectileType<CondurtlePylon>())
                    {
                        needsNewConductor = true;
                    }
                }
            }
            if (pylonID != -1)
            {
                if (!Main.projectile[pylonID].active || Main.projectile[pylonID].type != ModContent.ProjectileType<CondurtlePylon>())
                {
                    pylonID = -1;
                    if (currentState == AIState.InShell || currentState == AIState.AttackingInShell)
                    {
                        currentState = AIState.ExitingShell;
                        frameCounter = 0;
                    }
                }
            }

            if (needsNewConductor && currentState == AIState.InShell && pylonID != -1 && Main.projectile[pylonID].active && Main.projectile[pylonID].type == ModContent.ProjectileType<CondurtlePylon>())
            {
                ShootConductor(FindTarget());
                needsNewConductor = false;
                stateTimer = 0;
            }
        }

        private Player FindTarget()
        {
            Player target = null;
            float closestDistance = DETECTION_RANGE;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    float distance = Vector2.Distance(player.Center, NPC.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        target = player;
                    }
                }
            }

            return target;
        }

        private void StepUp()
        {
            if (NPC.velocity.Y >= 0f)
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, true, 1);
        }



        #region State Updates
        private void UpdateWalkingState(Player target)
        {
            float moveSpeed = 0.6f;
            NPC.velocity.X = facingRight ? moveSpeed : -moveSpeed;
            int direction = facingRight ? 1 : -1;
            bool obstacleAhead = false;
            StepUp();

            if (NPC.velocity.Y == 0f)
            {
                Vector2 bottomCenter = NPC.Bottom - new Vector2(0, 2);
                Vector2 checkPosition = bottomCenter + new Vector2(direction * (NPC.width / 2 + 2), 0);
                Point tileCoords = checkPosition.ToTileCoordinates();

                bool wallAtFeet = WorldGen.SolidTile(tileCoords.X, tileCoords.Y);
                bool wallAboveFeet = WorldGen.SolidTile(tileCoords.X, tileCoords.Y - 1);
                bool wallTwoAbove = WorldGen.SolidTile(tileCoords.X, tileCoords.Y - 2);
                Point groundCheckPos = new Point(tileCoords.X, tileCoords.Y + 1);
                bool groundBelowNext = WorldGen.SolidTile(groundCheckPos.X, groundCheckPos.Y);
                /*if (Main.netMode != NetmodeID.Server && Main.GameUpdateCount % 5 == 0)
                {
                    Dust.NewDustPerfect(
                        new Vector2(tileCoords.X * 16 + 8, tileCoords.Y * 16 + 8),
                        DustID.BlueTorch, Vector2.Zero, 0, default, 0.7f);
                    Dust.NewDustPerfect(
                        new Vector2(tileCoords.X * 16 + 8, (tileCoords.Y - 1) * 16 + 8),
                        DustID.RedTorch, Vector2.Zero, 0, default, 0.7f);
                }*/
                if (wallAtFeet && (wallAboveFeet || wallTwoAbove) || (!groundBelowNext && NPC.velocity.Y == 0))
                {
                    obstacleAhead = true;
                }
                if (obstacleAhead && Main.GameUpdateCount % 5 == 0)
                {
                    facingRight = !facingRight;
                    NPC.velocity.X = facingRight ? moveSpeed : -moveSpeed;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                NPC.velocity.X *= 0.99f;

                if (Main.GameUpdateCount % 5 == 0)
                {
                    bool collision = false;
                    Vector2 positionAhead = NPC.Bottom + new Vector2(direction * (NPC.width / 2), -6);
                    Point tilePos = positionAhead.ToTileCoordinates();

                    if (WorldGen.SolidTile(tilePos.X, tilePos.Y - 1))
                    {
                        collision = true;
                    }

                    if (Main.rand.NextBool(500))
                    {
                        collision = true;
                    }

                    if (collision)
                    {
                        facingRight = !facingRight;
                        NPC.velocity.X = facingRight ? moveSpeed : -moveSpeed;
                        NPC.netUpdate = true;
                    }
                }
            }
            if (Main.rand.NextBool(500))
            {
                currentState = AIState.Idle;
                stateTimer = Main.rand.Next(60, 120);
                NPC.velocity.X = 0;
                frameCounter = 0;
                NPC.frame.Y = NPC.frame.Height * IDLE_START;
            }
            if (target != null && attackCooldown <= 0)
            {
                currentState = AIState.EnteringShell;
                frameCounter = 0;
                stateTimer = 0;
                NPC.velocity.X = 0;
            }
            if (Math.Abs(NPC.velocity.X) > 0.1f)
            {
                frameCounter++;
                if (frameCounter >= 8)
                {
                    frameCounter = 0;
                    NPC.frame.Y += NPC.frame.Height;
                    if (NPC.frame.Y >= NPC.frame.Height * (WALK_START + WALK_FRAMES))
                        NPC.frame.Y = NPC.frame.Height * WALK_START;
                }
            }
            else
            {
                NPC.frame.Y = NPC.frame.Height * IDLE_START;
            }
        }

        private void UpdateIdleState(Player target)
        {
            NPC.velocity.X = 0;
            stateTimer--;
            if (stateTimer <= 0)
            {
                currentState = AIState.Walking;
                frameCounter = 0;
                return;
            }
            if (target != null && attackCooldown <= 0)
            {
                currentState = AIState.EnteringShell;
                frameCounter = 0;
                stateTimer = 0;
                return;
            }
            frameCounter++;
            if (frameCounter >= 40)
            {
                frameCounter = 0;
                NPC.frame.Y += NPC.frame.Height;
                if (NPC.frame.Y >= NPC.frame.Height * (IDLE_START + IDLE_FRAMES))
                    NPC.frame.Y = NPC.frame.Height * IDLE_START;
            }
        }

        private void UpdateEnteringShellState()
        {
            NPC.velocity.X = 0;
            frameCounter++;
            if (frameCounter >= 6)
            {
                frameCounter = 0;
                NPC.frame.Y += NPC.frame.Height;
                if (NPC.frame.Y >= NPC.frame.Height * (SHELL_ENTER_START + SHELL_ENTER_FRAMES))
                {
                    NPC.frame.Y = NPC.frame.Height * (SHELL_ENTER_START + SHELL_ENTER_FRAMES - 1);
                    lastShellPosition = NPC.Center;
                    ShootPylon();
                    currentState = AIState.AttackingInShell;
                    stateTimer = 0;
                    frameCounter = 0;
                }
            }
        }

        private void UpdateInShellState()
        {
            NPC.velocity.X = 0;
            NPC.frame.Y = NPC.frame.Height * ATTACK_START;
            stateTimer++;
            if ((conductorID == -1 && pylonID == -1) || stateTimer >= SHELL_MAX_TIME)
            {
                currentState = AIState.ExitingShell;
                frameCounter = 0;
                return;
            }
        }

        private void UpdateAttackingInShellState(Player target)
        {
            NPC.velocity.X = 0;
            frameCounter++;
            if (frameCounter >= 8)
            {
                frameCounter = 0;
                NPC.frame.Y += NPC.frame.Height;
                if (NPC.frame.Y > NPC.frame.Height * (ATTACK_START + ATTACK_FRAMES - 1))
                    NPC.frame.Y = NPC.frame.Height * ATTACK_START;
            }
            stateTimer++;
            if (stateTimer == 30)
                ShootConductor(target);
            if (stateTimer >= 60)
            {
                currentState = AIState.InShell;
                stateTimer = 0;
                frameCounter = 0;
                attackCooldown = ATTACK_COOLDOWN;
                NPC.frame.Y = NPC.frame.Height * ATTACK_START;
            }
        }

        private void UpdateExitingShellState()
        {
            NPC.velocity.X = 0;
            frameCounter++;
            if (frameCounter >= 6)
            {
                frameCounter = 0;
                NPC.frame.Y += NPC.frame.Height;
                if (NPC.frame.Y >= NPC.frame.Height * (SHELL_EXIT_START + SHELL_EXIT_FRAMES))
                {
                    currentState = AIState.PostAttackWalking;
                    stateTimer = POST_ATTACK_WALK_TIME;
                    float moveSpeed = 1.2f;
                    NPC.velocity.X = facingRight ? moveSpeed : -moveSpeed;
                    NPC.frame.Y = NPC.frame.Height * WALK_START;
                }
            }
        }

        private void UpdatePostAttackWalkingState()
        {
            float moveSpeed = 1.2f;
            NPC.velocity.X = facingRight ? moveSpeed : -moveSpeed;
            bool canStepUp = false;

            if (NPC.velocity.Y == 0)
            {
                Vector2 position = NPC.position;
                position.X += facingRight ? NPC.width : -2;
                Point tileCoords = position.ToTileCoordinates();
                bool solidAhead = Main.tile[tileCoords.X, tileCoords.Y].HasTile && Main.tileSolid[Main.tile[tileCoords.X, tileCoords.Y].TileType];
                bool emptyAbove = !Main.tile[tileCoords.X, tileCoords.Y - 1].HasTile || !Main.tileSolid[Main.tile[tileCoords.X, tileCoords.Y - 1].TileType];
                if (solidAhead && emptyAbove)
                    canStepUp = true;
            }

            if (canStepUp)
            {

            }
            else
            {
                if (Main.GameUpdateCount % 5 == 0)
                {
                    bool collision = false;
                    int direction = facingRight ? 1 : -1;
                    Vector2 positionAhead = NPC.Bottom + new Vector2(direction * (NPC.width / 2), -6);
                    Point tilePos = positionAhead.ToTileCoordinates();
                    if (WorldGen.SolidTile(tilePos.X, tilePos.Y - 1))
                        collision = true;
                    if (NPC.velocity.Y == 0 && !canStepUp)
                    {
                        Point ledgeCheckPos = (NPC.Bottom + new Vector2(direction * (NPC.width / 2 + 4), 4)).ToTileCoordinates();
                        bool groundAhead = WorldGen.SolidTile(ledgeCheckPos.X, ledgeCheckPos.Y);
                        if (!groundAhead)
                            collision = true;
                    }
                    if (Main.rand.NextBool(300))
                    {
                        collision = true;
                    }

                    if (collision)
                    {
                        facingRight = !facingRight;
                        NPC.velocity.X = facingRight ? moveSpeed : -moveSpeed;
                        NPC.netUpdate = true;
                    }
                }
            }
            stateTimer--;
            if (stateTimer <= 0)
            {
                currentState = AIState.Walking;
            }

            if (Math.Abs(NPC.velocity.X) > 0.1f)
            {
                frameCounter++;
                if (frameCounter >= 6)
                {
                    frameCounter = 0;
                    NPC.frame.Y += NPC.frame.Height;
                    if (NPC.frame.Y >= NPC.frame.Height * (WALK_START + WALK_FRAMES))
                        NPC.frame.Y = NPC.frame.Height * WALK_START;
                }
            }
            else
            {
                NPC.frame.Y = NPC.frame.Height * IDLE_START;
            }
        }
        #endregion

        #region Attack Methods
        private void ShootPylon()
        {
            Vector2 velocity = new(0, -3f);
            int projType = ModContent.ProjectileType<CondurtlePylon>();
            int damage = NPC.damage / 3;
            Vector2 spawnPos = NPC.Center - new Vector2(0, NPC.height / 2 - 4);
            int pylonIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, velocity, projType, damage, 1f, Main.myPlayer);
            pylonID = pylonIndex;
            SoundEngine.PlaySound(SoundID.Item8 with { Volume = 0.5f, Pitch = 0.2f }, NPC.Center);
        }

        private void ShootConductor(Player target)
        {
            if (target == null || pylonID == -1 || !Main.projectile[pylonID].active)
                return;
            Vector2 playerDirection = target.Center - NPC.Center;
            playerDirection.Normalize();
            playerDirection.Y -= 0.5f;
            playerDirection.Normalize();
            Vector2 velocity = playerDirection * 2f;

            int projType = ModContent.ProjectileType<CondurtleConductor>();
            int damage = NPC.damage / 2;
            Vector2 spawnPos = NPC.Center - new Vector2(0, NPC.height / 2 - 4);
            int conductorIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, velocity, projType, damage, 1f, Main.myPlayer);

            Projectile proj = Main.projectile[conductorIndex];
            proj.ai[0] = pylonID;
            proj.ai[1] = NPC.whoAmI;
            conductorID = conductorIndex;
            SoundEngine.PlaySound(SoundID.Item93 with { Volume = 0.7f, Pitch = 0.3f }, NPC.Center);
        }
        #endregion

        public override void FindFrame(int frameHeight)
        {
            if (NPC.frame.Y < frameHeight)
            {
                NPC.frame.Y = frameHeight * WALK_START;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation,
                new Vector2(NPC.frame.Width / 2, NPC.frame.Height / 2), NPC.scale, effects, 0f);

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric,
                        hit.HitDirection * 2f, -2f, 0, default, 1f);
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric,
                        hit.HitDirection, -1f, 0, default, 0.8f);
                }
            }
        }
    }

    public class CondurtlePylon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.aiStyle = -1;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y < 0)
            {
                Projectile.velocity.Y *= 0.98f;
                if (Math.Abs(Projectile.velocity.Y) < 0.5f)
                    Projectile.velocity.Y = 0;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 dustPos = Projectile.Center + new Vector2(Main.rand.NextFloat(-15, 15), Main.rand.NextFloat(-15, 15));
                int dustIndex = Dust.NewDust(dustPos, 1, 1, DustID.BlueCrystalShard, 0f, 0f, 0, default, 0.5f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 0.3f;
            }
            if (Projectile.velocity.Y == 0)
                Projectile.position.Y += (float)Math.Sin(Main.GameUpdateCount * 0.05f) * 0.3f;
            Projectile.light = 0.5f + (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.2f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);
            float scale = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.05f;
            Color glowColor = new Color(100, 200, 255, 100) * 0.5f;
            Main.spriteBatch.Draw(texture, drawPos, null, glowColor, Projectile.rotation, origin, scale * 1.2f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPos, null, lightColor, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric,
                    Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 0, default, 1.2f);
            }
            SoundEngine.PlaySound(SoundID.NPCDeath3 with { Volume = 0.6f, Pitch = 0.2f }, Projectile.position);
        }
    }

    public class CondurtleConductor : ModProjectile
    {
        private LightningData shellToConduitLightning;
        private LightningData conduitToPylonLightning;
        private bool lightningInitialized = false;
        private int lightningTimer = 0;
        private const int SHELL_LIGHTNING_DURATION = 30;
        private const int PYLON_LIGHTNING_DELAY = 60;
        private const int PYLON_LIGHTNING_DURATION = 45;
        private bool pylonLightningActive = false;

        private int pylonID = -1;
        private int turtleID = -1;
        private Vector2 turtleShellPosition;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.light = 0.5f;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (pylonID == -1 && Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxProjectiles)
                pylonID = (int)Projectile.ai[0];

            if (turtleID == -1 && Projectile.ai[1] >= 0 && Projectile.ai[1] < Main.maxNPCs)
            {
                turtleID = (int)Projectile.ai[1];
                if (Main.npc[turtleID].active && Main.npc[turtleID].type == ModContent.NPCType<Condurtle>())
                    turtleShellPosition = Main.npc[turtleID].Center + new Vector2(0, -4);
            }

            if (!lightningInitialized && turtleShellPosition != Vector2.Zero)
            {
                shellToConduitLightning = new LightningData(Projectile, LightningStyle.Jagged);
                InitializeBetweenPoints(shellToConduitLightning, turtleShellPosition, Projectile.Center, LightningStyle.Jagged);
                SoundEngine.PlaySound(SoundID.NPCHit53 with { Volume = 0.5f, Pitch = 0.3f });
                lightningInitialized = true;
            }

            if (lightningTimer < SHELL_LIGHTNING_DURATION + PYLON_LIGHTNING_DELAY)
            {
                Projectile.velocity *= 0.97f;
                if (Projectile.velocity.Length() < 0.5f)
                {
                    Projectile.velocity = Vector2.Zero;
                }
            }
            else if (!pylonLightningActive && pylonID != -1 && Main.projectile[pylonID].active)
            {
                if (pylonID == -1 || !Main.projectile[pylonID].active)
                    return;
                conduitToPylonLightning = new LightningData(Projectile, LightningStyle.Default);
               InitializeBetweenPoints(conduitToPylonLightning, Projectile.Center, Main.projectile[pylonID].Center, LightningStyle.Default);
                SoundEngine.PlaySound(SoundID.NPCHit53 with { Volume = 0.5f, Pitch = 0.2f });
                pylonLightningActive = true;
            }

            if (Main.rand.NextBool(3))
            {
                Vector2 dustPos = Projectile.Center + new Vector2(Main.rand.NextFloat(-15, 15), Main.rand.NextFloat(-15, 15));
                int dustIndex = Dust.NewDust(dustPos, 1, 1, DustID.BlueCrystalShard, 0f, 0f, 0, default, 0.5f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 0.3f;
            }

            Projectile.rotation += 0.03f;
            Projectile.light = 0.5f + (float)Math.Sin(Main.GameUpdateCount * 0.15f) * 0.3f;
            UpdateLightning();

            if (lightningTimer > SHELL_LIGHTNING_DURATION + PYLON_LIGHTNING_DELAY + PYLON_LIGHTNING_DURATION)
            {
                if (pylonID != -1 && Main.projectile[pylonID].active)
                {
                    Vector2 direction = Main.projectile[pylonID].Center - Projectile.Center;
                    direction.Normalize();
                    Projectile.velocity += direction * 0.1f;
                    if (Projectile.velocity.Length() > 5f)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 5f;
                    }
                }
                else
                    Projectile.Kill();
            }
            lightningTimer++;
        }

        private void UpdateLightning()
        {
            if (shellToConduitLightning != null && lightningTimer < SHELL_LIGHTNING_DURATION)
            {
                LightningUtility.InitializeBetweenPoints(
                    shellToConduitLightning,
                    turtleShellPosition,
                    Projectile.Center,
                    LightningUtility.LightningStyle.Jagged
                );

                LightningUtility.UpdateSegments(shellToConduitLightning);
                LightningUtility.UpdateBranches(shellToConduitLightning);
                if (Main.rand.NextBool(3))
                    LightningUtility.SpawnDust(shellToConduitLightning);
                if (lightningTimer > SHELL_LIGHTNING_DURATION - 10)
                {
                    shellToConduitLightning.Alpha *= 0.9f;
                }
            }
            if (conduitToPylonLightning != null && pylonLightningActive &&
                pylonID != -1 && Main.projectile[pylonID].active)
            {
                int pylonLightningTime = lightningTimer - (SHELL_LIGHTNING_DURATION + PYLON_LIGHTNING_DELAY);

                if (pylonLightningTime >= 0 && pylonLightningTime < PYLON_LIGHTNING_DURATION)
                {
                    LightningUtility.InitializeBetweenPoints(conduitToPylonLightning, Projectile.Center, Main.projectile[pylonID].Center, LightningUtility.LightningStyle.Default);
                    LightningUtility.UpdateSegments(conduitToPylonLightning);
                    LightningUtility.UpdateBranches(conduitToPylonLightning);
                    if (Main.rand.NextBool(2))
                        LightningUtility.SpawnDust(conduitToPylonLightning);
                    if (pylonLightningTime > PYLON_LIGHTNING_DURATION - 10)
                        conduitToPylonLightning.Alpha *= 0.9f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (shellToConduitLightning != null && shellToConduitLightning.Initialized && lightningTimer < SHELL_LIGHTNING_DURATION)
                LightningUtility.DrawLightning(shellToConduitLightning, Main.spriteBatch);
            if (conduitToPylonLightning != null && conduitToPylonLightning.Initialized && pylonLightningActive)
            {
                int pylonLightningTime = lightningTimer - (SHELL_LIGHTNING_DURATION + PYLON_LIGHTNING_DELAY);
                if (pylonLightningTime >= 0 && pylonLightningTime < PYLON_LIGHTNING_DURATION)
                    LightningUtility.DrawLightning(conduitToPylonLightning, Main.spriteBatch);
            }
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);
            float scale = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.05f;
            Color glowColor = new Color(50, 150, 255, 100) * 0.6f;
            Main.spriteBatch.Draw(texture, drawPos, null, glowColor, Projectile.rotation + MathHelper.PiOver4, origin, scale * 1.3f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPos, null, lightColor, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}