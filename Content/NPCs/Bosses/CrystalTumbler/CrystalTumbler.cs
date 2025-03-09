using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.DataStructures;
using AerovelenceMod.Content.Tiles.Citadel;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Building;
using System;
using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System.Linq;
using static AerovelenceMod.Content.Items.BossSummons.LargeGeode;
using ReLogic.Content;
using Terraria.Audio;
using AerovelenceMod.Content.Buffs;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{

    /* The Lord bless you and keep you; the Lord make his face shine on you and be gracious to you; the Lord turn his face toward you and give you peace
     * Numbers 6:24-26
     * Godspeed to whoever tries to touch anything in this amazing fantastic class
     */

    [AutoloadBossHead]
    public class CrystalTumbler : ModNPC
    {
        private int phase = 1;
        private int attackTimer = 0;
        private int attackCooldown = 500;

        private NPC healerDrone;
        private List<Vector2> HealerBeamTrailPositions;
        private List<float> HealerBeamTrailRotations;
        private float rotationOffset = 1f;

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 128;
            NPC.damage = 0;
            NPC.defense = 15;
            NPC.lifeMax = 5000;
            NPC.boss = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = Item.buyPrice(0, 5, 50, 0);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;

            telegraphLines = [];

            NPC.HitSound = new SoundStyle("AerovelenceMod/Sounds/Effects/RockHit")
            {
                Volume = 0.75f,
                Pitch = 0f,
                PitchVariance = 0.4f,
            };

            HealerBeamTrailPositions = [];
            HealerBeamTrailRotations = [];

            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CrystalTumbler");
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.65f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.75f);
        }

        public override void OnKill()
        {
            RemoveArenaBoundaries();
        }

        private float initialStunRotation = 0f;
        private int orbSpawnTimer;

        private bool isStunned = false;
        private int stunTimer = 0;
        private int stunDuration = 100;

        private bool spinDash = false;
        private bool irritatingToggleFix = false;
        private bool hasSpawnedHealerDrone = false;
        private int initializationDelay = 0;

        private Vector2 lastStartPoint;
        private Vector2 lastEndPoint;

        private bool startGroundSpikes = false;
        private int spikeSpawnTimer = 0;
        private int spikesSpawned = 0;
        public override void AI()
        {
            if (startGroundSpikes)
            {
                spikeSpawnTimer++;
                float delayBetweenSpikes = 12;
                if (spikeSpawnTimer >= delayBetweenSpikes && spikesSpawned < 10)
                {
                    Vector2 spikePosition = new(Main.rand.Next((int)ArenaBoundaries.leftBoundary.X, (int)ArenaBoundaries.rightBoundary.X), Main.maxTilesY * 16);
                    if (spikePosition.X <= ArenaBoundaries.leftBoundary.X)
                    {
                        startGroundSpikes = false;
                        return;
                    }
                    if (spikePosition.X >= ArenaBoundaries.rightBoundary.X)
                    {
                        startGroundSpikes = false;
                        return;
                    }
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spikePosition, Vector2.Zero, ModContent.ProjectileType<GroundSpike>(), damage, knockback);
                    spikeSpawnTimer = 0;
                    spikesSpawned++;
                    if (spikesSpawned >= 10)
                    {
                        startGroundSpikes = false;
                    }
                }
            }

        
            if (!hasSpawnedHealerDrone && NPC.life <= NPC.lifeMax * 0.33f)
            {
                if(!isAttacking && !isTelegraphing && NPC.velocity.Y == 0)
                {
                    healerDrone = Main.npc[NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<HealerDrone>(), 0)];
                    hasSpawnedHealerDrone = true;

                    isInPlatformPhase = true;
                }

            }

            if (healerDrone != null && healerDrone.active)
            {
                if (initializationDelay < 10)
                {
                    initializationDelay++;
                    return;
                }

                Vector2 startPoint = healerDrone.Center;
                Vector2 endPoint = NPC.Center;

                if (startPoint != lastStartPoint || endPoint != lastEndPoint)
                {
                    HealerBeamTrailPositions.Clear();
                    HealerBeamTrailRotations.Clear();

                    int numPoints = 100;
                    HealerBeamTrailPositions.Add(startPoint);
                    HealerBeamTrailRotations.Add(rotationOffset);

                    for (int i = 1; i <= numPoints; i++)
                    {
                        float t = i / (float)numPoints;
                        Vector2 midPoint = (startPoint + endPoint) / 2 + new Vector2(0, -100);
                        Vector2 point = Vector2.Lerp(Vector2.Lerp(startPoint, midPoint, t), Vector2.Lerp(midPoint, endPoint, t), t);

                        HealerBeamTrailPositions.Add(point);
                        HealerBeamTrailRotations.Add(rotationOffset + i * 0.1f);
                    }

                    lastStartPoint = startPoint;
                    lastEndPoint = endPoint;
                }

                rotationOffset -= 0.05f;

                for (int i = 0; i < HealerBeamTrailRotations.Count; i++)
                {
                    float speedMultiplier = 1f + (i / (float)HealerBeamTrailRotations.Count) * 2f;
                    HealerBeamTrailRotations[i] -= 0.05f * speedMultiplier;
                }
            }

            //Main.NewText(isSlamming);
            Player player = Main.player[NPC.target];

            player.AddBuff(ModContent.BuffType<FearsomeFoe>(), 1);

            if ((float)NPC.life / NPC.lifeMax <= 0.5)
            {
                phase = 2;
            }
            if (!isInPlatformPhase)
            {
                if (!phase2cinematic)
                {
                    if (!doingGiganticMegaLightningSlamOfMuchDestruction)
                    {
                        if (isStunned)
                        {
                            HandleStunState();
                        }
                        else
                        {
                            if (shouldPerformRollingSlam)
                            {
                                NPC.TargetClosest(false);
                            }

                            if (isBombPhase)
                            {
                                isTelegraphing = false;
                                HandleBombPhase();
                                NPC.TargetClosest(false);
                            }
                            else if ((shouldPerformRollingSlam && !completedSlamToBounce) || performingSlamBounce)
                            {
                                HandleRollingToSlam(player);
                                irritatingToggleFix = false;
                            }
                            else if (spinDash)
                            {
                                HandleSpinDash();
                            }
                            else
                            {
                                if (phase == 1)
                                {
                                    Phase1AI(player);
                                }
                                else
                                {
                                    Phase2AI(player);
                                }
                            }
                        }
                        if (performingSlamBounce)
                        {
                            NPC.rotation += NPC.velocity.X * 0.025f;
                            Vector2 arenaCenterW = (leftBoundary + rightBoundary) / 2;
                            if (NPC.Center.X < arenaCenterW.X)
                            {
                                NPC.velocity.X = Math.Abs(NPC.velocity.X);
                            }
                            else
                            {
                                NPC.velocity.X = -Math.Abs(NPC.velocity.X);
                            }

                            float maxHorizontalSpeed = 8f;
                            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -maxHorizontalSpeed, maxHorizontalSpeed);

                            if (NPC.velocity.Y > -0.2f && NPC.velocity.Y < 0.2f)
                            {
                                isDescending = true;

                                for (int num325 = 0; num325 < 20; num325++)
                                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X, NPC.velocity.Y, 0, default, 1);
                            }

                            if (isDescending)
                            {
                                NPC.rotation += NPC.velocity.Y * 0.025f;

                                int checkX = (int)(NPC.Center.X / 16);
                                for (int offset = 0; offset <= 7; offset++)
                                {
                                    int checkY = (int)(NPC.Bottom.Y / 16) + offset;

                                    Tile tileBelow = Framing.GetTileSafely(checkX, checkY);
                                    if (tileBelow.HasTile && (tileBelow.TileType == ModContent.TileType<SmoothCavernStoneTile>() || tileBelow.TileType == ModContent.TileType<CitadelBrickTile>() || tileBelow.TileType == ModContent.TileType<ChargedStoneTile>()))
                                    {
                                        SoundStyle stylea = new("AerovelenceMod/Sounds/Effects/CrystalSlam")
                                        {
                                            Volume = .85f,
                                            Pitch = 0f,
                                            PitchVariance = 0f,
                                        };
                                        SoundEngine.PlaySound(stylea, NPC.Center);
                                        NPC.noTileCollide = false;
                                        EyeGlow = false;
                                        isSlamming = false;
                                        isDescending = false;
                                        NPC.noGravity = false;
                                        reachedBoundary = false;
                                        performingSlamBounce = false;
                                        Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 30;
                                        spinDash = true;
                                        isStunned = true;
                                        stunTimer = 0;
                                        isAttacking = false;

                                        if (Math.Abs(player.velocity.Y) < 0.1f)
                                        {
                                            player.velocity.Y = -10f;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        if (completedSlamToBounce)
                        {
                            PerformSlam(player);
                        }
                    }
                    else
                    {
                        PerformSlam(player);
                    }

                }
                else
                {
                    if (!doingGiganticMegaLightningSlamOfMuchDestruction)
                    {
                        NPC.TargetClosest(false);

                        if (!isMovingToCenter)
                        {
                            float distanceToCenter = Vector2.Distance(NPC.Center, arenaCenter);
                            Vector2 directionToCenter = Vector2.Normalize(arenaCenter - NPC.Center);
                            float currentSpeed = NPC.velocity.Length();
                            float minSpeed = 5f;
                            float decelerationFactor = MathHelper.Clamp(distanceToCenter / 500f, 0.05f, 1f);
                            float newSpeed = MathHelper.Lerp(currentSpeed, minSpeed, decelerationFactor);

                            NPC.velocity = directionToCenter * newSpeed;
                            NPC.rotation += NPC.velocity.X * 0.025f;

                            if (distanceToCenter < 65f)
                            {
                                NPC.velocity = Vector2.Zero;
                                isMovingToCenter = true;
                                zapBoss = true;
                                Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 30;

                                zapTimer = 120;
                            }
                        }

                        else if (zapTimer > 0)
                        {
                            zapTimer--;
                            if (zapTimer == 0)
                            {
                                phase2cinematic = false;
                                doingGiganticMegaLightningSlamOfMuchDestruction = true;
                            }
                        }
                    }
                }

                if (phase == 2 && lightningStrikePositionsInitialized)
                {
                    startGroundSpikes = true;
                    timeInPhase2++;
                    if (timeInPhase2 % 15 == 0 && lightningStrikeIndex < lightningStrikePositions.Length)
                    {
                        Vector2 spawnPosition = lightningStrikePositions[lightningStrikeIndex];
                        Vector2 spawnOffset = spawnPosition - new Vector2(0, 500f);
                        Vector2 downwardVelocity = new(0, 15f);

                        Projectile.NewProjectile(spawnSource: NPC.GetSource_FromAI(), spawnOffset, downwardVelocity, ModContent.ProjectileType<ElectricBolt>(), damage, knockback, Main.myPlayer, ai0: 1);

                        lightningStrikeIndex++;
                    }

                    if (lightningStrikeIndex >= lightningStrikePositions.Length)
                    {
                        lightningStrikePositionsInitialized = false;
                        readyToSpawnTelegraphStrikes = false;
                    }
                }
            }
            else
            {
                HandlePlatformPhase();
                NPC.TargetClosest(false);
                NPC.damage = 0;

                int healingDroneType = ModContent.NPCType<HealerDrone>();

                bool healingDroneActive = false;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == healingDroneType)
                    {
                        healingDroneActive = true;
                        break;
                    }
                }

                if (!healingDroneActive)
                {
                    isInPlatformPhase = false;
                    NPC.dontTakeDamage = false;
                    NPC.damage = 10;
                    return;
                }
            }
        }

        private int lightningStrikeIndex = 0;

        private int timeInPhase2 = 0;
        private bool isSpinning = false;
        private bool isSpinDashing = false;
        private float maxRotationSpeed = 2f;
        private float spinAcceleration = 0.11f;
        private float currentRotationSpeed = 0f;
        private float maxDashSpeed = 20f;
        private float dashAcceleration = 0.5f;
        private float currentDashSpeed = 0f;
        private bool dashDirectionRight = true;

        private void HandleSpinDash()
        {
            if (spinDash)
            {
                if (!isSpinning && !isSpinDashing)
                {
                    NPC.velocity.X *= 0.9f;
                    if (Math.Abs(NPC.velocity.X) < 0.1f)
                    {
                        NPC.velocity.X = 0f;
                        isSpinning = true;
                        Vector2 arenaCenterW = (leftBoundary + rightBoundary) / 2;
                        dashDirectionRight = NPC.Center.X < arenaCenterW.X;
                    }
                }

                if (isSpinning)
                {
                    currentRotationSpeed += spinAcceleration;
                    NPC.rotation += currentRotationSpeed * (dashDirectionRight ? -1 : 1);
                    if (currentRotationSpeed >= maxRotationSpeed)
                    {
                        currentRotationSpeed = maxRotationSpeed;
                        isSpinning = false;
                        isSpinDashing = true;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 dustVel = Vector2.UnitX * (dashDirectionRight ? -5f : 5f);
                        dustVel += Main.rand.NextVector2Circular(2f, 2f);
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Cloud, dustVel.X, dustVel.Y, 100, default, 1.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.7f;
                    }
                }

                if (isSpinDashing)
                {
                    // Instead of setting rotation to NPC.velocity.X, set it based on the velocity vector.
                    NPC.rotation = NPC.velocity.ToRotation();

                    if (currentDashSpeed < maxDashSpeed)
                    {
                        currentDashSpeed += dashAcceleration;
                    }
                    if (phase == 2)
                    {
                        Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 5;
                        EyeGlow = true;
                    }
                    NPC.velocity.X = dashDirectionRight ? currentDashSpeed : -currentDashSpeed;
                    if ((dashDirectionRight && NPC.Center.X >= rightBoundary.X) || (!dashDirectionRight && NPC.Center.X <= leftBoundary.X))
                    {
                        NPC.velocity.X = 0f;
                        isSpinDashing = false;
                        currentRotationSpeed = 0f;
                        currentDashSpeed = 0f;
                        if (phase == 2)
                        {
                            EyeGlow = false;
                        }
                        Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 30;
                        SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/HardRockSlam")
                        {
                            Volume = .75f,
                            Pitch = 1f,
                            PitchVariance = 0f,
                        };
                        SoundEngine.PlaySound(stylea, NPC.Center);
                        spinDash = false;
                    }
                }
            }
        }


        public static bool isAttacking = false;
        private int lastAttack = -1;
        private bool bombPhaseTriggered = false;
        private bool shouldPerformRollingSlam;

        private void Phase1AI(Player player)
        {
            NPC.TargetClosest(true);

            if (NPC.life <= NPC.lifeMax * 0.75f && !bombPhaseTriggered && !isBombPhase)
            {
                StartBombPhase();
                bombPhaseTriggered = true;
                return;
            }
            if (isBombPhase)
            {
                return;
            }
            lineExtraPower = Math.Clamp(MathHelper.Lerp(lineExtraPower, -0.25f, 0.1f), 0f, 1f);
            attackTimer++;
            if (isAttacking)
            {
                if (isSlamming)
                {
                    PerformSlam(player);
                    if (NPC.velocity.Y > -0.2f && NPC.velocity.Y < 0.2f)
                    {
                        isDescending = true;
                        for (int num325 = 0; num325 < 20; num325++) Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X, NPC.velocity.Y, 0, default, 1);
                    }
                    if (isDescending)
                    {
                        NPC.rotation += NPC.velocity.Y * 0.025f;
                    }
                    return;
                }
                if (aboutToDash)
                {
                    PerformSuperDash(player);
                    return;
                }
                if (isDashing)
                {
                    float rotationAmount = NPC.velocity.X * 0.025f;
                    NPC.rotation += isDashingLeft ? rotationAmount : rotationAmount;
                    SimpleDash();
                    return;
                }
                if (isSpawningGeysers)
                {
                    SpawnGeysers();
                    return;
                }
                isAttacking = false;
            }

            if (!isStunned && !isAttacking)
            {
                RollTowardsPlayer(player);
                if (attackTimer > attackCooldown)
                {
                    attackTimer = 0;
                    int attackChoice;
                    do
                    {
                        attackChoice = Main.rand.Next(5);
                    } while (attackChoice == lastAttack);
                    lastAttack = attackChoice;
                    isAttacking = true;
                    irritatingToggleFix = false;
                    switch (attackChoice)
                    {
                        case 0:
                            PerformRockThrow();
                            break;
                        case 1:
                            aboutToDash = true;
                            PerformSuperDash(player);
                            break;
                        case 2:
                            PerformSlam(player);
                            break;
                        case 3:
                            if (!irritatingToggleFix && isDescending)
                            {
                                isDescending = false;
                                irritatingToggleFix = true;
                            }
                            else
                            {
                                shouldPerformRollingSlam = true;
                            }
                            break;
                        case 4:
                            Vector2 directionToPlayer = Vector2.Normalize(player.Center - NPC.Center);
                            float dashSpeed = 12f;
                            float jumpHeight = -5f;
                            NPC.velocity.X = directionToPlayer.X * dashSpeed;

                            if (NPC.velocity.Y == 0)
                            {
                                NPC.velocity.Y = jumpHeight;
                            }
                            break;
                        default:
                            RollTowardsPlayer(player);
                            isAttacking = false;
                            break;
                    }
                }
            }
        }

        /*private bool isMovingToCenterForSpiral = false;
        private bool isPerformingSpiralAttack = false;
        private int spiralAttackTimer = 0;
        private int spiralWaveDelay = 200;
        private int spiralWaveCount = 0;

        private void PerformSpiralAttack(Player player)
        {
            isAttacking = true;
            if (!isMovingToCenterForSpiral)
            {
                float distanceToCenter = Vector2.Distance(NPC.Center, arenaCenter);
                float speedFactor = MathHelper.Clamp(distanceToCenter / 100f, 0.2f, 1f);

                NPC.rotation += NPC.velocity.Y * 0.025f;
                NPC.velocity = Vector2.Normalize(arenaCenter - NPC.Center) * 10f * speedFactor;

                if (distanceToCenter < 65f)
                {
                    NPC.velocity = Vector2.Zero;
                    isMovingToCenterForSpiral = true;
                    isPerformingSpiralAttack = true;
                    NPC.dontTakeDamage = true;
                    spiralAttackTimer = spiralWaveDelay;
                }
            }
            else if (isPerformingSpiralAttack)
            {
                if (spiralAttackTimer > 0)
                {
                    spiralAttackTimer--;

                    if (spiralAttackTimer == 0)
                    {
                        SpawnSpiralProjectiles();

                        spiralWaveCount++;
                        if (spiralWaveCount < 3)
                        {
                            spiralAttackTimer = spiralWaveDelay;
                        }
                        else
                        {
                            isPerformingSpiralAttack = false;
                            NPC.dontTakeDamage = false;
                            isAttacking = false;
                        }
                    }
                }
            }
        }

        private void SpawnSpiralProjectiles()
        {
            int numberOfProjectiles = 10;
            float spiralSpeed = 10f;

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                Vector2 direction = Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numberOfProjectiles * i));
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction * spiralSpeed, ModContent.ProjectileType<SpiralProjectile>(), NPC.damage / 2, 0f, Main.myPlayer);

                Main.projectile[proj].ai[0] = NPC.whoAmI;
            }
        }*/

        private bool phase2Initialized = false;
        public bool zapBoss = false;
        private bool phase2cinematic = false;
        private bool isMovingToCenter = false;
        private int zapTimer = 0;
        Vector2 arenaCenter = (ArenaBoundaries.leftBoundary + ArenaBoundaries.rightBoundary) / 2;

        private bool isMagneticPullActive = false;
        private int magneticPullTimer = 0;
        private int debrisSpawnTimer = 0;
        private float magneticForce = 0.5f;
        private int debrisSpawnInterval = 30;
        private bool shouldPerformMagneticPull = false;

        /*private void MagneticPull(Player player)
        {
            isAttacking = true;
            NPC.velocity *= 0.95f;
            if (!isMagneticPullActive)
            {
                isMagneticPullActive = true;
                magneticPullTimer = 600;
                debrisSpawnTimer = debrisSpawnInterval;
            }
            if (isMagneticPullActive)
            {
                magneticPullTimer--;
                Vector2 pullDirection = NPC.Center - player.Center;
                pullDirection.Normalize();
                player.velocity += pullDirection * magneticForce;

                if (debrisSpawnTimer <= 0)
                {
                    SpawnDebris(player);
                    debrisSpawnTimer = debrisSpawnInterval;
                }
                else
                {
                    debrisSpawnTimer--;
                }
                if (magneticPullTimer <= 0)
                {
                    isMagneticPullActive = false;
                    isAttacking = false;
                }
            }
        }

        private void SpawnDebris(Player player)
        {
            int numberOfDebris = 6;
            for (int i = 0; i < numberOfDebris; i++)
            {
                Vector2 spawnPosition = NPC.Center + Main.rand.NextVector2CircularEdge(200, 200);
                Vector2 velocity = Vector2.Normalize(player.Center - spawnPosition) * Main.rand.NextFloat(5f, 10f);

                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, velocity, ModContent.ProjectileType<CrystalShard>(), NPC.damage / 4, 0f, Main.myPlayer);
            }
        }*/

        private bool doingGiganticMegaLightningSlamOfMuchDestruction = false;
        private bool hasCompletedGiganticMegaLightningSlamOfMuchDestruction = false;
        private int shardAttackTimer = 0;
        private int electricBoltCount = 0;
        private int shardAttackPhase = 0;

        private void Phase2AI(Player player)
        {
            NPC.TargetClosest(true);
            if (actuallyAboutToSlam)
            {
                isAttacking = true;
                PerformSlam(player);
            }
            if(isRadialAttackActive && phase2Initialized)
            {
                HandleRadialAttack();
            }
            if (isDoingShardAttack && phase2Initialized)
            {
                shardAttackTimer++;
                if (shardAttackTimer % 15 == 0)
                {
                    if (shardAttackPhase == 0)
                    {
                        float rotationOffset = MathHelper.ToRadians(25);
                        for (int i = -1; i <= 1; i++)
                        {
                            Vector2 velocity = Vector2.Normalize(player.Center - NPC.Center).RotatedBy(rotationOffset * i) * 10f;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<SharpCrystalShard>(), 15, 0f, Main.myPlayer);
                        }
                        shardAttackPhase++;
                    }
                    else if (shardAttackPhase == 1)
                    {
                        if (electricBoltCount < Main.rand.Next(1, 4))
                        {
                            Vector2 velocity = Vector2.Normalize(player.Center - NPC.Center) * 10f;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<HomingSharpCrystalShard>(), 15, 0f, Main.myPlayer);
                            electricBoltCount++;
                        }
                        else
                        {
                            electricBoltCount = 0;
                            shardAttackPhase++;
                        }
                    }
                    else if (shardAttackPhase == 2)
                    {
                        float rotationOffset = MathHelper.ToRadians(25);

                        for (int i = -1; i <= 1; i++)
                        {
                            Vector2 velocity = Vector2.Normalize(player.Center - NPC.Center).RotatedBy(rotationOffset * i) * 10f;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<SharpCrystalShard>(), 15, 0f, Main.myPlayer);
                        }
                        shardAttackPhase++;
                    }
                    else if (shardAttackPhase == 3)
                    {
                        if (electricBoltCount < Main.rand.Next(3, 5))
                        {
                            Vector2 velocity = Vector2.Normalize(player.Center - NPC.Center) * 10f;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<HomingSharpCrystalShard>(), 15, 0f, Main.myPlayer);
                            electricBoltCount++;
                        }
                        else
                        {
                            shardAttackPhase = 0;
                            electricBoltCount = 0;
                            isDoingShardAttack = false;
                        }
                    }
                }
            }
            if (!phase2Initialized && !isAttacking)
            {
                phase2Initialized = true;
                zapBoss = true;
                foreach (var crystalPos in crystalPositions)
                {
                    if (crystalPos != Vector2.Zero)
                    {
                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(), crystalPos, Vector2.Zero, ModContent.ProjectileType<SmallTumblerOrbVFX>(), 0, 0f, Main.myPlayer);
                        Main.projectile[projID].scale = 0.5f;

                        int projID2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), crystalPos, Vector2.Zero, ModContent.ProjectileType<LightningLaser>(), 0, 0f, Main.myPlayer);
                        Main.projectile[projID2].scale = 0.5f;
                    }
                }
                phase2cinematic = true;
            }
            lineExtraPower = Math.Clamp(MathHelper.Lerp(lineExtraPower, -0.25f, 0.1f), 0f, 1f);
            attackTimer++;
            if (isAttacking)
            {
                if (isSlamming)
                {
                    PerformSlam(player);
                    if (NPC.velocity.Y > -0.2f && NPC.velocity.Y < 0.2f)
                    {
                        isDescending = true;

                        for (int num325 = 0; num325 < 20; num325++)
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X, NPC.velocity.Y, 0, default, 1);
                    }
                    if (isDescending)
                    {
                        NPC.rotation += NPC.velocity.Y * 0.025f;
                    }
                    return;
                }
                if (aboutToDash)
                {
                    PerformSuperDash(player);
                    return;
                }
                if (isDashing)
                {
                    float rotationAmount = NPC.velocity.X * 0.025f;
                    NPC.rotation += isDashingLeft ? rotationAmount : rotationAmount;
                    SimpleDash();
                    return;
                }
                if (isSpawningGeysers)
                {
                    SpawnGeysers();
                    return;
                }
                isAttacking = false;
            }

            if (!isStunned && !isAttacking)
            {
                RollTowardsPlayer(player);
                if (attackTimer > attackCooldown)
                {
                    attackTimer = 0;
                    int attackChoice;
                    do
                    {
                      attackChoice = Main.rand.Next(5);
                    } while (attackChoice == lastAttack);
                    lastAttack = attackChoice;
                    isAttacking = true;
                    switch (attackChoice)
                    {
                        case 0:
                            PerformRockThrow();
                            break;
                        case 1:
                            aboutToDash = true;
                            PerformSuperDash(player);
                            break;
                        case 2:
                            PerformSlam(player);
                            break;
                        case 3:
                            if (!irritatingToggleFix && isDescending)
                            {
                                isDescending = false;
                                irritatingToggleFix = true;
                            }
                            else
                            {
                                shouldPerformRollingSlam = true;
                            }
                            break;
                        case 4:
                            Vector2 directionToPlayer = Vector2.Normalize(player.Center - NPC.Center);
                            float dashSpeed = 12f;
                            float jumpHeight = -5f;
                            NPC.velocity.X = directionToPlayer.X * dashSpeed;
                            if (NPC.velocity.Y == 0)
                            {
                                NPC.velocity.Y = jumpHeight;
                            }
                            break;

                        case 5:
                            break;
                        default:
                            RollTowardsPlayer(player);
                            isAttacking = false;
                            break;
                    }
                }
            }
        }

        private bool isRolling = false;
        private bool hasBounced = false;
        private bool isModifiedSlam = false;
        private bool reachedBoundary = false;
        private bool completedSlamToBounce = false;
        private bool irritatingToggleFix2 = false;

        private void HandleRollingToSlam(Player player)
        {
            if (!irritatingToggleFix2)
            {
                isDescending = false;
                irritatingToggleFix2 = true;
            }
            isAttacking = true;
            NPC.TargetClosest(false);
            float slowdownDistance = 240f;
            float maxSpeed = 5f;
            float accelerationRate = 0.1f;
            float decelerationRate = 0.2f;
            float distanceToLeftBoundary = NPC.position.X - leftBoundary.X;
            float distanceToRightBoundary = rightBoundary.X - NPC.position.X;
            bool isNearBoundary = (NPC.direction < 0 && distanceToLeftBoundary < slowdownDistance) || (NPC.direction > 0 && distanceToRightBoundary < slowdownDistance);
            if (!isRolling)
            {
                isRolling = true;
                hasBounced = false;
                completedSlamToBounce = false;
                if (player.Center.X > NPC.Center.X)
                {
                    NPC.velocity.X = 8f;
                }
                else
                {
                    NPC.velocity.X = -8f;
                }
            }
            if (reachedBoundary && !performingSlamBounce)
            {
                NPC.velocity.X = 0f;
            }
            if (isRolling)
            {
                NPC.rotation += NPC.velocity.X * 0.025f;

                if (isNearBoundary)
                {
                    NPC.velocity.X -= decelerationRate * NPC.direction;

                    if (Math.Abs(NPC.velocity.X) < 0.5f)
                    {
                        NPC.velocity.X = 0f;
                        NPC.direction *= -1;
                        completedSlamToBounce = true;
                        reachedBoundary = true;
                        isRolling = false;
                    }
                }
                else
                {
                    NPC.velocity.X += accelerationRate * NPC.direction;
                    NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -maxSpeed, maxSpeed);
                }
            }
            if (isDescending)
            {
                NPC.noGravity = true;
                NPC.velocity.Y += 1.1f;
                int checkX = (int)(NPC.Center.X / 16);
                for (int offset = 0; offset <= 7; offset++)
                {
                    int checkY = (int)(NPC.Bottom.Y / 16) + offset;
                    Tile tileBelow = Framing.GetTileSafely(checkX, checkY);
                    if (tileBelow.HasTile && (tileBelow.TileType == ModContent.TileType<SmoothCavernStoneTile>() || tileBelow.TileType == ModContent.TileType<CitadelBrickTile>() || tileBelow.TileType == ModContent.TileType<ChargedStoneTile>()))
                    {
                        NPC.velocity.Y = 0;
                        NPC.position.Y = checkY * 16 - NPC.height;
                        NPC.noTileCollide = false;
                        EyeGlow = false;
                        isSlamming = false;
                        isDescending = false;
                        NPC.noGravity = false;
                        Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 30;
                        isStunned = true;
                        stunTimer = 0;
                        isAttacking = false;
                        reachedBoundary = false;
                        break;
                    }
                }
            }
        }

        private bool performingSlamBounce = false;

        private void PerformSlamBounce()
        {
            if (!isSlamming && !performingSlamBounce)
            {
                isSlamming = true;
                EyeGlow = true;
                NPC.noTileCollide = true;
                isDescending = false;
                NPC.velocity.Y = -15f;
            }

            if (!isDescending && !hasBounced)
            {
                hasBounced = true;
                performingSlamBounce = true;
                NPC.velocity.Y = -17f;
                NPC.velocity.X = (NPC.velocity.X > 0) ? -12f : 12f;
            }
        }

        private void HandleStunState()
        {
            if (stunTimer == 0)
            {
                initialStunRotation = NPC.rotation;
            }
            stunTimer++;
            NPC.velocity *= 0.91f;
            float rockingIntensity = 0.4f * (1f - (stunTimer / (float)stunDuration));
            float rockingOffset = (float)Math.Sin(stunTimer * 0.15f) * rockingIntensity;
            NPC.rotation = initialStunRotation + rockingOffset;
            if (stunTimer >= stunDuration)
            {
                isStunned = false;
                stunTimer = 0;
                NPC.rotation -= rockingOffset;
                int actionChoice = Main.rand.Next(3);
                if (actionChoice == 0)
                {
                    aboutToDash = true;
                }
                else
                {
                    RollTowardsPlayer(Main.player[NPC.target]);
                }
            }
        }

        #region quick attacks

        public void SpawnTumblerOrb()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 spawnPosition = NPC.Center - new Vector2(0, -500);
                int orbIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, Vector2.Zero, ModContent.ProjectileType<TumblerOrb>(), 0, 0f, Main.myPlayer);

                SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/lightning_flash_01") with { Volume = .65f, Pitch = 1f, PitchVariance = 0f, };
                SoundEngine.PlaySound(stylea, NPC.Center);
                Main.projectile[orbIndex].ai[1] = NPC.whoAmI;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, orbIndex);
                }
            }
        }

        #endregion

        #region bomb attack phase

        private int bombTimer = 0;
        private int bombsSpawned = 0;
        private int bombSpawnInterval = 15;

        private bool isBombPhase = false;
        private const int bombPhaseDuration = 600;

        private void StartBombPhase()
        {
            isBombPhase = true;
            isSlamming = false;
            isDashing = false;
            telegraphLaserSpawned = false;
            bombHitGround = false;
            bombsSpawned = 0;
            bombTimer = 0;
            beginShake = false;
            screenShakePower = 15f;
            laserCountdownTimer = 0;
            NPC.dontTakeDamage = true;
        }

        private bool activateShieldVFX = false;
        private void HandleBombPhase()
        {
            NPC.rotation += NPC.velocity.X * 0.025f;
            bombTimer++;
            float slowdownDistance = 240f;
            float maxSpeed = 5f;
            float accelerationRate = 0.1f;
            float decelerationRate = 0.2f;
            if (bombTimer == 1)
            {
                NPC.velocity.X = 0f;
                activateShieldVFX = true;
                NPC.dontTakeDamage = true;
            }
            if (bombTimer > 60)
            {
                float distanceToLeftBoundary = NPC.position.X - leftBoundary.X;
                float distanceToRightBoundary = rightBoundary.X - NPC.position.X;
                bool isNearBoundary = (NPC.direction < 0 && distanceToLeftBoundary < slowdownDistance) || (NPC.direction > 0 && distanceToRightBoundary < slowdownDistance);
                if (isNearBoundary)
                {
                    NPC.velocity.X -= decelerationRate * NPC.direction;
                    if (Math.Abs(NPC.velocity.X) < 0.5f)
                    {
                        NPC.velocity.X = 0f;
                        NPC.direction *= -1;
                    }
                }
                else
                {
                    NPC.velocity.X += accelerationRate * NPC.direction;
                    NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -maxSpeed, maxSpeed);
                }
            }
            HandleBombSpawning();
            NPC.velocity.Y = 0;
        }

        private bool bombHitGround = false;
        private bool beginShake = false;
        private bool telegraphLaserSpawned = false;
        private float screenShakePower = 15f;
        private float screenShakeDecayRate = 0.2f;
        private int laserCountdownTimer = 0;

        private void HandleBombSpawning()
        {
            int numberOfBombs = 10;
            float arenaWidth = ArenaBoundaries.rightBoundary.X - ArenaBoundaries.leftBoundary.X;
            float segmentWidth = arenaWidth / numberOfBombs;
            float targetHeightAboveBoss = 500f;
            bombTimer++;
            if (bombTimer >= bombSpawnInterval && bombsSpawned < numberOfBombs)
            {
                Vector2 launchPosition = NPC.Center;
                float segmentStartX = ArenaBoundaries.leftBoundary.X + bombsSpawned * segmentWidth;
                float randomX = segmentStartX + Main.rand.NextFloat(0, segmentWidth);
                int bombIndex = NPC.NewNPC(NPC.GetSource_FromAI(), (int)launchPosition.X, (int)launchPosition.Y, ModContent.NPCType<CrystalBomb>(), 0);
                NPC bombNPC = Main.npc[bombIndex];
                Vector2 directionToTarget = new Vector2(randomX, NPC.Center.Y - targetHeightAboveBoss) - launchPosition;
                directionToTarget.Normalize();
                bombNPC.velocity = directionToTarget * 5f;
                bombNPC.ai[1] = NPC.Center.Y - targetHeightAboveBoss;
                bombNPC.ai[2] = randomX;
                bombNPC.ai[0] = NPC.whoAmI;
                bombNPC.noTileCollide = true;
                bombTimer = 0;
                bombsSpawned++;
            }

            if (bombHitGround)
            {
                if (!telegraphLaserSpawned)
                {
                    Vector2 telegraphPosition = ArenaBoundaries.leftBoundary - new Vector2(500, 20);
                    Vector2 direction = Vector2.UnitX;
                    int projIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), telegraphPosition, direction, ModContent.ProjectileType<HugeLaserTelegraph>(), 0, 0, Main.myPlayer);
                    Main.projectile[projIndex].ai[1] = direction == Vector2.UnitX ? 0 : 1;
                    telegraphLaserSpawned = true;
                }
                if (laserCountdownTimer > 0)
                {
                    laserCountdownTimer--;
                    if (laserCountdownTimer == 0)
                    {
                        Vector2 spawnPosition;
                        Vector2 laserDirection;
                        if (Main.rand.NextBool())
                        {
                            spawnPosition = new Vector2(ArenaBoundaries.leftBoundary.X - 200, NPC.Center.Y + 50);
                            laserDirection = Vector2.UnitX;
                        }
                        else
                        {
                            spawnPosition = new Vector2(ArenaBoundaries.rightBoundary.X + 200, NPC.Center.Y + 50);
                            laserDirection = -Vector2.UnitX;
                        }
                        Main.NewText("edifdshfg factual laser");
                        beginShake = true;
                        int projID = Projectile.NewProjectile(NPC.GetSource_FromAI(),
                            spawnPosition,
                            laserDirection * 50f,
                            ModContent.ProjectileType<HugeLaserProjectile>(),
                            NPC.damage, 0, Main.myPlayer);

                        if (projID >= 0)
                        {
                            SoundStyle stylea = new("AerovelenceMod/Sounds/Effects/GiantElectricityShot")
                            {
                                Volume = .80f,
                                Pitch = 1f,
                                PitchVariance = 0f,
                            };
                            SoundEngine.PlaySound(stylea, NPC.Center);
                        }
                        NPC.dontTakeDamage = false;
                        activateShieldVFX = false;
                        isBombPhase = false;
                        bombHitGround = false;
                        telegraphLaserSpawned = false;
                        bombsSpawned = 0;
                        bombTimer = 0;
                        beginShake = false;
                        screenShakePower = 15f;
                        laserCountdownTimer = 0;
                        isCountingDown = false;
                        NPC.velocity.X = 0f;
                        NPC.direction = 1;
                    }
                }
            }
            bool allBombsDead = true;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC otherBomb = Main.npc[i];
                if (otherBomb.active && otherBomb.type == ModContent.NPCType<CrystalBomb>())
                {
                    allBombsDead = false;
                    break;
                }
            }
            if (allBombsDead && bombsSpawned == numberOfBombs)
            {
                if (!bombHitGround)
                {
                    bombHitGround = true;
                    laserCountdownTimer = 200;
                    isCountingDown = true;
                }
            }
        }

        private bool isCountingDown = false;

        public void StartLaserCountdown()
        {
            if (!isCountingDown)
            {
                Main.NewText($"3called");
                bombHitGround = true;
                laserCountdownTimer = 200;
                isCountingDown = true;
            }
        }

        #endregion

        private int platformTimer = 0;
        private bool isInPlatformPhase = false;
        private void HandlePlatformPhase()
        {
            NPC.rotation += NPC.velocity.X * 0.025f;
            platformTimer++;
            float slowdownDistance = 240f;
            float maxSpeed = 5f;
            float accelerationRate = 0.1f;
            float decelerationRate = 0.2f;
            if (platformTimer == 1)
            {
                NPC.velocity.X = 0f;
                activateShieldVFX = true;
                NPC.dontTakeDamage = true;
            }
            if (platformTimer > 60)
            {
                float distanceToLeftBoundary = NPC.position.X - leftBoundary.X;
                float distanceToRightBoundary = rightBoundary.X - NPC.position.X;
                bool isNearBoundary = (NPC.direction < 0 && distanceToLeftBoundary < slowdownDistance) || (NPC.direction > 0 && distanceToRightBoundary < slowdownDistance);
                if (isNearBoundary)
                {
                    NPC.velocity.X -= decelerationRate * NPC.direction;

                    if (Math.Abs(NPC.velocity.X) < 0.5f)
                    {
                        NPC.velocity.X = 0f;
                        NPC.direction *= -1;
                    }
                }
                else
                {
                    NPC.velocity.X += accelerationRate * NPC.direction;
                    NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -maxSpeed, maxSpeed);
                }
            }
            NPC.velocity.Y = 0;
            if (platformTimer % 300 == 0)
            {
                SpawnPlatform(Main.player[NPC.target]);
            }
        }

        private void SpawnPlatform(Player player)
        {
            Main.NewText("plat");
            int platformLength = 56;
            int gapSize = Main.rand.Next(5, 8);
            int gapPosition = Main.rand.Next(0, platformLength - gapSize);
            float arenaCenterX = (ArenaBoundaries.leftBoundary.X + ArenaBoundaries.rightBoundary.X) / 2f;
            Vector2 platformStartPosition = new(arenaCenterX - (platformLength * 16) / 2f, player.Center.Y - 100f);
            int actualSpawnedTiles = 0;
            for (int i = 0; i < platformLength; i++)
            {
                if (i >= gapPosition && i < gapPosition + gapSize)
                {
                    continue;
                }
                Vector2 spawnPosition = platformStartPosition + new Vector2(actualSpawnedTiles * 32, 0);
                int frame;
                if (actualSpawnedTiles == 0)
                {
                    frame = 0;
                }
                else if (i == platformLength - 1 - gapSize || actualSpawnedTiles == platformLength - gapSize - 1)
                {
                    frame = 2;
                }
                else
                {
                    frame = 1;
                }
                int projectileIndex = Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPosition, Vector2.Zero, ModContent.ProjectileType<PlatformProjectile>(), 50, 0f, player.whoAmI, actualSpawnedTiles);
                Main.projectile[projectileIndex].ai[0] = actualSpawnedTiles;
                Main.projectile[projectileIndex].ai[1] = frame;

                actualSpawnedTiles++;
            }
            platformLength -= gapSize;
            if (actualSpawnedTiles > 0)
            {
                Projectile lastProjectile = Main.projectile[^1];
                if (lastProjectile.active && lastProjectile.type == ModContent.ProjectileType<PlatformProjectile>())
                {
                    lastProjectile.ai[1] = 2;
                }
            }
        }

        #region drawing code
        private bool SetAfterglow = true;

        public List<float> previousRotations;
        public List<Vector2> previousPostions;

        private bool EyeGlow = false;

        private float eyeGlowAlpha = 1f;
        float lineExtraPower = 0f;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D npcTexture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalTumbler");
            if (phase == 2 && !phase2cinematic && (isDashing || isSpinDashing || isSlamming || isDescending))
            {
                #region after image
                if (previousRotations != null && previousPostions != null)
                {
                    for (int i = 0; i < previousRotations.Count; i++)
                    {
                        float progress = (float)i / previousRotations.Count;
                        Color col = Color.Azure * Easings.easeOutCirc(progress);
                        float scale = 2.05f;
                        Main.EntitySpriteDraw(npcTexture, previousPostions[i] - Main.screenPosition + new Vector2(0, 4), null, col with { A = 0 } * progress * 0.9f,
                            previousRotations[i], npcTexture.Size() / 2f, scale, SpriteEffects.None);
                    }
                }
                #endregion
                for (int i = 0; i < 8; i++)
                {
                    Color col = i == 0 ? Color.SkyBlue with { A = 0 } : Color.DeepSkyBlue with { A = 0 };

                    Main.EntitySpriteDraw(npcTexture, NPC.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f) + new Vector2(0, 4), null, col * 1f, NPC.rotation, npcTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
                }
                Main.EntitySpriteDraw(npcTexture, NPC.Center - Main.screenPosition + new Vector2(0, 4), null, drawColor, NPC.rotation, npcTexture.Size() / 2, 1f, SpriteEffects.None, 0f);
                //Main.EntitySpriteDraw(npcTexture, NPC.Center - Main.screenPosition + new Vector2(0, 4), null, Color.White with { A = 0 } * 0.25f, NPC.rotation, npcTexture.Size() / 2, 1.1f, SpriteEffects.None, 0f);
            }
            else
            {
                Main.EntitySpriteDraw(npcTexture, NPC.Center - Main.screenPosition + new Vector2(0, 4), null, drawColor, NPC.rotation, npcTexture.Size() / 2, 1f, SpriteEffects.None, 0f);
            }
            return false;
        }

        private List<Tuple<Vector2, Vector2, float, float>> telegraphLines = [];
        private int telegraphTimer2 = 0;

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/CrystalTumbler/Glowmask").Value;
            Texture2D laserTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/RotatingThing").Value;
            Texture2D gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/EosGrad").Value;
            Color[] gradientColors = new Color[gradientTexture.Width * gradientTexture.Height];
            gradientTexture.GetData(gradientColors);
            Vector2 drawPosition = NPC.Center - Main.screenPosition;
            Color[] horizontalGradientColors = new Color[gradientTexture.Width];
            for (int i = 0; i < gradientTexture.Width; i++)
            {
                horizontalGradientColors[i] = gradientColors[i];
            }
            Vector2 offset = new(0, 4);
            drawPosition += offset;
            Vector2 origin = NPC.frame.Size() / 2f;
            spriteBatch.Draw(texture, drawPosition, NPC.frame, Color.White, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0);
            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            if (healerDrone != null && healerDrone.active)
            {
                for (int i = 0; i < HealerBeamTrailPositions.Count - 1; i++)
                {
                    Vector2 startPosition = HealerBeamTrailPositions[i];
                    Vector2 endPosition = HealerBeamTrailPositions[i + 1];
                    Vector2 direction = endPosition - startPosition;
                    float length = direction.Length();
                    if (length > 0f)
                    {
                        direction.Normalize();
                        for (float j = 0; j <= length; j += 10f)
                        {
                            Vector2 drawPosition2 = startPosition + direction * j;
                            float rotation = direction.ToRotation() + HealerBeamTrailRotations[i];
                            float gradientPosition = i / (float)(HealerBeamTrailPositions.Count - 1);
                            int gradientX = Math.Clamp((int)(gradientPosition * (gradientTexture.Width - 1)), 0, gradientTexture.Width - 1);
                            Color gradientColor = horizontalGradientColors[gradientX];
                            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/RotatingThing").Value, drawPosition2 - Main.screenPosition, null, gradientColor * 0.75f, rotation, new Vector2(16, 16), 1f, SpriteEffects.None, 0f);
                        }
                    }
                }
            }
            float glowIntensity = 0f;
            if (isAttacking && (isDashing || isSpinDashing || isDescending))
            {
                glowIntensity = 1f;
            }
            else if (!isDescending)
            {
                glowIntensity = 0.1f;
            }
            else
            {
                glowIntensity = 0.1f;
            }
            Texture2D Bloommy = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/Bloommy");
            Main.EntitySpriteDraw(Bloommy, drawPosition, NPC.frame, Color.White * glowIntensity, NPC.rotation, NPC.frame.Size() / 2f, 1, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(Bloommy, drawPosition, NPC.frame, Color.White * glowIntensity, NPC.rotation, NPC.frame.Size() / 2f, 1, SpriteEffects.None, 0);
            if (activateShieldVFX)
            {
                Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/Orbs/whiteFireEye").Value;
                Texture2D Flare2 = Mod.Assets.Request<Texture2D>("Assets/Orbs/spiky_20fade").Value;
                Texture2D Flare3 = Mod.Assets.Request<Texture2D>("Assets/Slash/pixelKennySlash").Value;
                Texture2D Ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;
                Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;
                myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1").Value);
                myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/SofterBlueGrad").Value);
                myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);
                myEffect.Parameters["flowSpeed"].SetValue(0.3f);
                myEffect.Parameters["vignetteSize"].SetValue(1f);
                myEffect.Parameters["vignetteBlend"].SetValue(0.8f);
                myEffect.Parameters["distortStrength"].SetValue(0.02f);
                myEffect.Parameters["xOffset"].SetValue(0.0f);
                myEffect.Parameters["uTime"].SetValue(Main.GameUpdateCount * 0.015f);
                myEffect.Parameters["colorIntensity"].SetValue(0.5f);
                Main.spriteBatch.Draw(Ball, NPC.Center - Main.screenPosition, null, Color.Black * 0.3f, NPC.rotation, Ball.Size() / 2, 0.5f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Ball, NPC.Center - Main.screenPosition, null, Color.DeepSkyBlue * 0.2f, NPC.rotation, Ball.Size() / 2, 2f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare, NPC.Center - Main.screenPosition, null, Color.DodgerBlue * 0.2f, NPC.rotation * 0.8f, Flare.Size() / 2, 0.75f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare, NPC.Center - Main.screenPosition, null, Color.SkyBlue * 0.2f, NPC.rotation * -0.8f, Flare.Size() / 2, 0.75f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare, NPC.Center - Main.screenPosition, null, Color.White * 0.15f, NPC.rotation * 0.8f, Flare.Size() / 2, 0.35f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare, NPC.Center - Main.screenPosition, null, Color.White * 0.15f, NPC.rotation * -0.8f, Flare.Size() / 2, 0.35f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(Ball, NPC.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * 0.2f, NPC.rotation, Ball.Size() / 2, 0.45f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare3, NPC.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * 0.2f, NPC.rotation, Flare3.Size() / 2, 0.6f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare3, NPC.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * 0.2f, NPC.rotation * -1, Flare3.Size() / 2, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare2, NPC.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * 0.2f, NPC.rotation + 1, Flare2.Size() / 2, 0.5f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare2, NPC.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * 0.2f, NPC.rotation * -1 + 1, Flare2.Size() / 2, 0.7f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            Texture2D lineTexture2 = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/Medusa_Gray").Value;
            float telegraphLength2 = 500f;
            for (int i = telegraphLines.Count - 1; i >= 0; i--)
            {
                var line = telegraphLines[i];
                float newLifetime = line.Item3 - 1f;
                float newOpacity = line.Item4 * (newLifetime / 60f);
                DrawTelegraphLine(spriteBatch, line.Item1, line.Item2, lineTexture2, Color.Blue, newOpacity, telegraphLength2);
                telegraphLines[i] = new Tuple<Vector2, Vector2, float, float>(line.Item1, line.Item2, newLifetime, newOpacity);
                if (newLifetime <= 0f)
                {
                    telegraphLines.RemoveAt(i);
                }
            }
            if (isTelegraphing)
            {
                int numberOfLines = 8;
                Texture2D lineTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/Medusa_Gray").Value;
                float telegraphLength = 200f;
                for (int i = 0; i < numberOfLines; i++)
                {
                    Vector2 direction = Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numberOfLines * i));
                    DrawTelegraphLine(spriteBatch, NPC.Center, direction, lineTexture, Color.Blue, 0.4f, telegraphLength);
                    if (NPC.life <= NPC.lifeMax / 2)
                    {
                        Vector2 rotatedDirection = Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numberOfLines * i + 25f));
                        DrawTelegraphLine(spriteBatch, NPC.Center, rotatedDirection, lineTexture, Color.Blue, 0.4f, telegraphLength);
                    }
                }
            }
            if (readyToSpawnTelegraphStrikes)
            {
                Texture2D lineTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/Medusa_Gray").Value;
                Color telegraphColor = Color.Blue;
                float telegraphLength = 500f;
                foreach (var position in lightningStrikePositions)
                {
                    Vector2 directionDown = -Vector2.UnitY;
                    Vector2 positionBelow = position - new Vector2(0, -264);
                    DrawTelegraphLine(spriteBatch, positionBelow, directionDown, lineTexture, telegraphColor, 1f, telegraphLength);
                }
            }
            if (EyeGlow)
            {
                Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/Pixel/GlowStar").Value;
                Vector2 eyeStarDrawPos = NPC.Center - Main.screenPosition;
                float eyeStarRotation = NPC.rotation;
                float eyeStarValue = 0.5f;
                eyeGlowAlpha -= 0.02f;
                if (eyeGlowAlpha < 0f)
                {
                    eyeGlowAlpha = 0f;
                }
                for (int al = 0; al < 2; al++)
                {
                    Color fadeColor = Color.SkyBlue * eyeGlowAlpha;
                    Main.spriteBatch.Draw(Flare, eyeStarDrawPos, Flare.Frame(1, 1, 0, 0), fadeColor, eyeStarRotation, Flare.Size() / 2, eyeStarValue * 2f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(Flare, eyeStarDrawPos, Flare.Frame(1, 1, 0, 0), fadeColor * 0.4f, eyeStarRotation, Flare.Size() / 2, eyeStarValue * 2.5f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(Flare, eyeStarDrawPos, Flare.Frame(1, 1, 0, 0), Color.White * eyeGlowAlpha, eyeStarRotation * -1, Flare.Size() / 2, eyeStarValue * 0.8f, SpriteEffects.None, 0f);
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
        #endregion

        #region dash attack

        private bool aboutToDash = false;
        private bool isDashing = false;
        private bool doingDash = false;
        private int dashPhase = 0;
        private float dashSpeedMultiplier = 1f;
        private int dashPreparationTimer = 0;
        private int dashDuration = 180;

        private void PerformSuperDash(Player player)
        {
            if (dashPhase == 0)
            {
                NPC.velocity.X *= 0.95f;
                NPC.rotation += NPC.velocity.X * 0.025f;
                dashPreparationTimer++;

                if (dashPreparationTimer > 60)
                {
                    dashPhase++;
                    dashPreparationTimer = 0;
                }
            }
            else if (dashPhase == 1)
            {
                NPC.velocity.X *= 0.95f;
                NPC.rotation += NPC.velocity.X * 0.025f;
                dashPreparationTimer++;
                NPC.rotation += 2 / 180 * NPC.direction;
                int spikeCount = 3 + Main.rand.Next(3);
                if (dashPreparationTimer % 40 == 0 && dashPreparationTimer < 180)
                {
                    SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/Flail1") with { Volume = .30f, Pitch = .56f, PitchVariance = .27f, };
                    SoundEngine.PlaySound(stylea, NPC.Center);
                    for (int i = 0; i < spikeCount; i++)
                    {
                        Vector2 velocity = Vector2.Normalize(player.Center - NPC.Center).RotatedByRandom(MathHelper.ToRadians(10)) * 10f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<CrystalShard>(), 15, 0f, Main.myPlayer);
                    }
                }
                if (dashPreparationTimer >= 180)
                {
                    dashPhase++;
                    dashPreparationTimer = 0;
                }
            }
            else if (dashPhase == 2)
            {
                dashPreparationTimer++;
                float rotationIncrement = 0.2f;
                float timeFactor = dashPreparationTimer / 60f;
                NPC.rotation += rotationIncrement * timeFactor * NPC.direction;
                bool willDashRight = player.Center.X > NPC.Center.X;
                if (NPC.velocity.Y == 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 dustPos = new Vector2(
                            NPC.position.X + Main.rand.Next(NPC.width),
                            NPC.position.Y + NPC.height - 8
                        );
                        float dustSpeed = Main.rand.NextFloat(3f, 6f);
                        Vector2 dustVel = Vector2.UnitX * (willDashRight ? -dustSpeed : dustSpeed);
                        dustVel.Y = Main.rand.NextFloat(-1f, -0.2f);
                        int dust = Dust.NewDust(dustPos, 8, 8, DustID.Cloud,
                            dustVel.X, dustVel.Y, 100, default, Main.rand.NextFloat(1.2f, 1.8f));
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.7f;
                        if (Main.rand.NextBool())
                        {
                            dust = Dust.NewDust(dustPos, 4, 4, DustID.Cloud,
                                dustVel.X * 0.8f, dustVel.Y, 100, default, Main.rand.NextFloat(0.8f, 1.2f));
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].velocity *= 0.5f;
                        }
                    }
                }
                if (dashPreparationTimer % 20 == 0 && dashPreparationTimer < 120)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 velocity = Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / 8 * i)) * 10f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<CrystalShard>(), 15, 0f, Main.myPlayer);
                    }
                }
                if (dashPreparationTimer >= 120)
                {
                    dashPhase++;
                    dashPreparationTimer = 0;
                    NPC.velocity.Y = -5f;
                    isDashingLeft = Main.rand.NextBool();
                    isDashing = true;
                    Vector2 initialDirection = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center);
                    NPC.velocity = initialDirection * 15f;
                    SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/StampAirSwing2") with { Volume = .70f, Pitch = 0f, PitchVariance = 0f, };
                    SoundEngine.PlaySound(styleb, NPC.Center);
                    hasInitialBoost = true;
                }
            }
            else if (dashPhase == 3)
            {
                SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/SwooshySwoosh") with { Volume = .55f, Pitch = 0.7f, PitchVariance = 0f, };
                SoundEngine.PlaySound(stylea, NPC.Center);
                isDashing = true;
                aboutToDash = false;
                dashPhase = 0;
                if (!isDashing)
                {
                    isAttacking = false;
                }
            }
        }

        Vector2 leftBoundary = ArenaBoundaries.leftBoundary + new Vector2(10 * 16, 0);
        Vector2 rightBoundary = ArenaBoundaries.rightBoundary - new Vector2(10 * 16, 0);

        private bool isDashingLeft;
        private bool hasInitialBoost = false;

        public void AddTelegraphLine(Vector2 start, Vector2 direction, float length, float lifetime)
        {
            telegraphLines.Add(new Tuple<Vector2, Vector2, float, float>(start, direction, lifetime, 1f)); 
        }


        private void DrawTelegraphLine(SpriteBatch spriteBatch, Vector2 start, Vector2 direction, Texture2D lineTexture, Color color, float opacity, float length)
        {
            Vector2 lineScale = new Vector2(length / lineTexture.Width, 0.2f + (lineExtraPower * 0.1f)) * 1.5f;
            float rotation = direction.ToRotation();
            spriteBatch.Draw(lineTexture, start - Main.screenPosition, null, color * opacity * 1.5f, rotation, new Vector2(0, lineTexture.Height / 2), lineScale * 1.25f, SpriteEffects.None, 0f);
            spriteBatch.Draw(lineTexture, start - Main.screenPosition, null, Color.Aqua * opacity * 1.6f, rotation, new Vector2(0, lineTexture.Height / 2), lineScale * 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(lineTexture, start - Main.screenPosition, null, color * opacity * 1.7f, rotation, new Vector2(0, lineTexture.Height / 2), lineScale * 0.75f, SpriteEffects.None, 0f);
        }

        private int telegraphTimer = 0;
        private bool isTelegraphing = false;

        private void SimpleDash()
        {
            if (!isTelegraphing)
            {
                isTelegraphing = true;
                telegraphTimer = 60;
            }
            NPC.velocity *= 0.96f;
            if (Math.Abs(NPC.velocity.X) < 0.2f)
            {
                NPC.velocity.X = 0f;
                isDashing = false;
                hasInitialBoost = false;
                isStunned = true;
                stunTimer = 0;
                stunDuration = 60;
                if (telegraphTimer <= 0)
                {
                    SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/ElectricExplode") with { Volume = .15f, Pitch = 0f, PitchVariance = 0f, };
                    SoundEngine.PlaySound(stylea, NPC.Center);
                    int numberOfProjectiles = 8;
                    for (int i = 0; i < numberOfProjectiles; i++)
                    {
                        Vector2 velocity = Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numberOfProjectiles * i)) * 10f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<ElectricBolt>(), 40, 1f, Main.myPlayer, ai0: 0);
                    }
                    if (NPC.life <= NPC.lifeMax / 2)
                    {
                        for (int i = 0; i < numberOfProjectiles; i++)
                        {
                            Vector2 velocity = Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numberOfProjectiles * i + 25f)) * 10f;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<ElectricBolt>(), 40, 1f, Main.myPlayer, ai0: 0);
                        }
                    }
                    isTelegraphing = false;
                }
            }
            if (NPC.life <= NPC.lifeMax / 2 && isDashing)
            {
                if (NPC.collideX || NPC.collideY)
                {
                    if (Main.rand.NextBool(8))
                    {
                        Vector2 spawnPosition = new(NPC.Center.X, NPC.Bottom.Y - 100);
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, Vector2.Zero, ModContent.ProjectileType<TallCrystalSpike>(), NPC.damage / 2, 0f, Main.myPlayer);

                        if (Main.rand.NextBool())
                        {
                            Main.projectile[proj].spriteDirection = -1;
                        }
                        Main.projectile[proj].ai[0] = 1;
                    }
                }
            }
            if (telegraphTimer > 0)
            {
                telegraphTimer--;
            }
            NPC.noTileCollide = false;
            if (!isDashing)
            {
                isAttacking = false;
                isTelegraphing = false;
            }
        }

        #endregion
        private bool isDoingShardAttack = false;
        #region basic movement

        private int lastAttackType = -1;
        private int shardSpawnTimer = 0;
        private void RollTowardsPlayer(Player player)
        {
            orbSpawnTimer++;
            if (orbSpawnTimer >= 200)
            {
                if (phase == 1)
                {
                    int attackType;
                    do
                    {
                        attackType = Main.rand.Next(3);
                    } while (attackType == lastAttackType);
                    lastAttackType = attackType;
                    switch (attackType)
                    {
                        case 0:
                            SpawnTumblerOrb();
                            break;

                        case 1:
                            isRadialAttackActive = true;
                            radialAttackPhase = 0;
                            radialAttackTimer = 0;
                            break;

                        case 2:
                            SoundStyle stylea = new("AerovelenceMod/Sounds/Effects/Flail1")
                            {
                                Volume = 0.30f,
                                Pitch = 0.56f,
                                PitchVariance = 0.27f
                            };
                            SoundEngine.PlaySound(stylea, NPC.Center);

                            int spikeCount = 5;

                            for (int i = 0; i < spikeCount; i++)
                            {
                                Vector2 velocity = Vector2.Normalize(player.Center - NPC.Center).RotatedByRandom(MathHelper.ToRadians(10)) * 10f;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<CrystalShard>(), 15, 0f, Main.myPlayer);
                            }
                            break;
                    }
                    orbSpawnTimer = 0;
                }
                else if (phase == 2)
                {
                    int attackType;
                    do
                    {
                        attackType = Main.rand.Next(3);
                    } while (attackType == lastAttackType);
                    lastAttackType = attackType;
                    switch (attackType)
                    {

                        case 0:
                            isRadialAttackActive = true;
                            radialAttackPhase = 0;
                            radialAttackTimer = 0;
                            break;
                        case 1:
                            SoundStyle stylea = new("AerovelenceMod/Sounds/Effects/Flail1")
                            {
                                Volume = 0.30f,
                                Pitch = 0.56f,
                                PitchVariance = 0.27f
                            };
                            SoundEngine.PlaySound(stylea, NPC.Center);
                            int spikeCount = 5;
                            for (int i = 0; i < spikeCount; i++)
                            {
                                Vector2 velocity = Vector2.Normalize(player.Center - NPC.Center).RotatedByRandom(MathHelper.ToRadians(10)) * 10f;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<CrystalShard>(), 15, 0f, Main.myPlayer);
                            }
                            break;
                        case 2:
                            isDoingShardAttack = true;
                            break;
                    }
                    orbSpawnTimer = 0;
                }
            }
            float lifePercentLeft = (float)NPC.life / NPC.lifeMax;
            float desiredSpeed = 2 + 12 * (1 - lifePercentLeft);

            if (phase == 2)
            {
                desiredSpeed *= 0.5f;

                if (Main.rand.NextBool(120) && NPC.velocity.Y == 0)
                {
                    NPC.velocity.Y = -6f;
                }
            }
            if (player.Center.X > NPC.Center.X)
            {
                if (NPC.velocity.X < desiredSpeed)
                {
                    NPC.velocity.X += (0.1f * (1 - lifePercentLeft + 1));
                }
            }
            else if (player.Center.X < NPC.Center.X)
            {
                if (NPC.velocity.X > -desiredSpeed)
                {
                    NPC.velocity.X -= (0.1f * (1 - lifePercentLeft + 1));
                }
            }
            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -6, 6);

            if (!isSlamming)
            {
                NPC.rotation += NPC.velocity.X * 0.025f;
            }
        }

        #endregion

        #region main slam

        private bool isSlamming = false;
        private bool isDescending = false;
        private bool hasSpawnedIndicator = false;
        private Vector2 slamStartPosition;
        private int timeInAir = 0;
        private bool hasDoneGiganticMegaLightningSlamOfMuchDestruction = false;
        private bool readyToSpawnTelegraphStrikes = false;
        private bool actuallyAboutToSlam = false;
        private bool lightningStrikePositionsInitialized = false;

        private Vector2[] lightningStrikePositions = new Vector2[10];

        private float currentAngle = 0f;

        private void PerformSlam(Player player)
        {
            if (!doingGiganticMegaLightningSlamOfMuchDestruction)
            {
                if (!isSlamming)
                {
                    isSlamming = true;
                    EyeGlow = true;
                    NPC.velocity.X = 0f;
                    NPC.noTileCollide = true;
                    isDescending = false;
                    NPC.velocity.Y = -15f;
                }
                if (!isDescending && !hasSpawnedIndicator)
                {
                    hasSpawnedIndicator = true;
                    SpawnIndicatorProjectile();
                }
                if (isDescending)
                {
                    NPC.noGravity = true;
                    NPC.velocity.Y += 1.1f;
                    int checkX = (int)(NPC.Center.X / 16);
                    for (int offset = 0; offset <= 7; offset++)
                    {
                        int checkY = (int)(NPC.Bottom.Y / 16) + offset;
                        Tile tileBelow = Framing.GetTileSafely(checkX, checkY);
                        if (tileBelow.HasTile &&
                            (tileBelow.TileType == ModContent.TileType<SmoothCavernStoneTile>() ||
                             tileBelow.TileType == ModContent.TileType<CitadelBrickTile>() ||
                             tileBelow.TileType == ModContent.TileType<ChargedStoneTile>()))
                        {
                            NPC.velocity.Y = 0;
                            NPC.position.Y = checkY * 16 - NPC.height;
                            NPC.noTileCollide = false;
                            EyeGlow = false;
                            for (int i = 0; i < 20; i++)
                            {
                                Vector2 direction = (i % 2 == 0) ? new Vector2(-1, 0) : new Vector2(1, 0);
                                direction += Main.rand.NextVector2Circular(0.25f, 0.25f);
                                Vector2 dustVel = direction * Main.rand.NextFloat(2f, 3f);
                                dustVel += NPC.velocity * 0.3f;
                                Dust gd = Dust.NewDustPerfect(NPC.Bottom, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.SkyBlue, Scale: Main.rand.NextFloat(0.2f, 0.35f));
                                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.3f, timeBeforeSlow: 20,
                                    preSlowPower: 0.94f, postSlowPower: 0.90f, velToBeginShrink: 1f, fadePower: 0.92f, shouldFadeColor: false);
                            }
                            isSlamming = false;
                            isDescending = false;
                            NPC.noGravity = false;
                            Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 30;
                            int slamType = Main.rand.Next(3);
                            SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/CrystalSlam") with { Volume = .85f, Pitch = 0f, PitchVariance = 0f, };
                            SoundEngine.PlaySound(stylea, NPC.Center);
                            int dustCount = 12;
                            float baseSpeed = 5f;
                            float verticalSpread = 1f;
                            float horizontalSpread = 0.5f;
                            for (int i = 0; i < dustCount; i++)
                            {
                                Vector2 dustVel = new(-baseSpeed, 0);
                                dustVel += Main.rand.NextVector2Circular(horizontalSpread, verticalSpread);
                                Vector2 dustPos = NPC.Bottom + new Vector2(-10, -4);
                                int dust = Dust.NewDust(dustPos, 8, 4, DustID.Cloud,
                                    dustVel.X, dustVel.Y, 100, default, 2f);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].fadeIn = 1.1f;
                            }
                            for (int i = 0; i < dustCount; i++)
                            {
                                Vector2 dustVel = new(baseSpeed, 0);
                                dustVel += Main.rand.NextVector2Circular(horizontalSpread, verticalSpread);
                                Vector2 dustPos = NPC.Bottom + new Vector2(10, -4);
                                int dust = Dust.NewDust(dustPos, 8, 4, DustID.Cloud, dustVel.X, dustVel.Y, 100, default, 2f);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].fadeIn = 1.1f;
                            }
                            if (completedSlamToBounce)
                            {
                                completedSlamToBounce = false;
                                shouldPerformRollingSlam = false;
                                PerformSlamBounce();
                            }
                            else
                            {
                                isStunned = true;
                                stunTimer = 0;
                                stunDuration = 60;
                            }
                            if (phase == 2 && !hasDoneGiganticMegaLightningSlamOfMuchDestruction)
                            {
                                isDescending = false;
                            }
                            else
                            {
                                switch (slamType)
                                {
                                    case 0:
                                        SpikeSlam();
                                        break;
                                    case 1:
                                        SpikeSlam();
                                        break;
                                    case 2:
                                        SpikeSlam();
                                        break;
                                }
                                isDescending = false;
                            }
                            actuallyAboutToSlam = false;
                            if (phase == 2 && !lightningStrikePositionsInitialized)
                            {
                                if (lightningStrikePositions == null || lightningStrikePositions.Length < 10)
                                {
                                    //Main.NewText("not properly initialized dumbass", Color.Red);
                                }
                                else
                                {
                                    for (int i = 0; i < 10; i++)
                                    {
                                        float xPosition = ArenaBoundaries.leftBoundary.X + i * (ArenaBoundaries.rightBoundary.X - ArenaBoundaries.leftBoundary.X) / 9;
                                        float yPosition = NPC.Center.Y - 200;
                                        lightningStrikePositions[i] = new Vector2(xPosition, yPosition);
                                    }
                                    readyToSpawnTelegraphStrikes = true;
                                    lightningStrikePositionsInitialized = true;
                                    zapBoss = false;
                                }
                            }
                            isAttacking = false;
                            break;
                        }
                    }
                }
            }
            else if (doingGiganticMegaLightningSlamOfMuchDestruction)
            {
                if (timeInAir <= 1)
                {
                    NPC.velocity.Y -= 5f;
                }
                timeInAir++;
                NPC.noGravity = true;
                if (timeInAir >= 5 && timeInAir <= 20)
                {
                    NPC.velocity.Y *= 0.95f;
                }
                if (timeInAir > 20)
                {
                    NPC.velocity.Y = 0;
                }
                if (timeInAir >= 20 && timeInAir <= 90)
                {
                    if (NPC.velocity.Y == 0)
                    {
                        NPC.rotation += 0.15f;
                        NPC.rotation *= 1.15f;
                        isDescending = true;
                        shardSpawnTimer++;
                        telegraphTimer2++;
                        if (shardSpawnTimer == 1)
                        {
                            float angle = currentAngle + MathHelper.ToRadians(-40);
                            Vector2 direction = new((float)Math.Cos(angle), (float)Math.Sin(angle));
                            Vector2 telegraphPosition = NPC.Center;
                            telegraphLines.Add(new Tuple<Vector2, Vector2, float, float>(telegraphPosition, direction, 60f, 30f));
                            telegraphLines = telegraphLines
                                .Where(t => t.Item3 > 1)
                                .Select(t => new Tuple<Vector2, Vector2, float, float>(t.Item1, t.Item2, t.Item3 - 1, t.Item4 > 0 ? t.Item4 - 1 : t.Item4))
                                .ToList();
                        }
                        if (shardSpawnTimer >= 3)
                        {
                            shardSpawnTimer = 0;
                            Vector2 velocity = new Vector2((float)Math.Cos(currentAngle), (float)Math.Sin(currentAngle)) * 16f;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<SharpCrystalShard>(), 15, 0f, Main.myPlayer);
                            float rotationStep = MathHelper.ToRadians(20);
                            currentAngle += rotationStep;
                            if (currentAngle > MathHelper.TwoPi)
                            {
                                currentAngle -= MathHelper.TwoPi;
                            }
                        }
                    }
                }
                if (timeInAir >= 100 && !actuallyAboutToSlam)
                {
                    doingGiganticMegaLightningSlamOfMuchDestruction = false;
                    actuallyAboutToSlam = true;
                    NPC.noGravity = false;
                    
                    PerformSlam(player);
                }
            }
        }

        #endregion

        #region spike slam

        private int damage = 1;
        private int knockback = 1;
        private void SpikeSlam()
        {
            IEntitySource entitySource = NPC.GetSource_FromAI();
            Vector2 spikePositionRight = new Vector2(NPC.position.X + NPC.width + 40, NPC.position.Y + NPC.height - 20);
            Vector2 spikePositionLeft = new Vector2(NPC.position.X - 40, NPC.position.Y + NPC.height - 20);
            int rightFrameStart = Main.rand.NextBool() ? 0 : 6;
            int leftFrameStart = rightFrameStart == 0 ? 6 : 0;
            int spikeRight = Projectile.NewProjectile(entitySource, spikePositionRight, Vector2.Zero, ModContent.ProjectileType<CrystalSpike>(), damage, knockback, Main.myPlayer);
            Main.projectile[spikeRight].spriteDirection = 1;
            Main.projectile[spikeRight].frame = rightFrameStart;
            int spikeLeft = Projectile.NewProjectile(entitySource, spikePositionLeft, Vector2.Zero, ModContent.ProjectileType<CrystalSpike>(), damage, knockback, Main.myPlayer);
            Main.projectile[spikeLeft].spriteDirection = -1;
            Main.projectile[spikeLeft].frame = leftFrameStart;
        }


        #endregion

        #region water geyser slam
        private bool isSpawningGeysers = false;
        private int geyserSpawnTimer = 0;
        private int geyserInterval = 15 * 16;
        private int currentGeyserX = 0;
        private int endGeyserX = 0;
        private int geyserDirection = 1;
        private int geyserSpawnSpeed = 5;

        private void WaterSlam()
        {
            int radius = 50;
            int bossX = (int)(NPC.Center.X / 16);
            int bossY = (int)(NPC.Center.Y / 16);
            int leftmostX = bossX - radius;
            int rightmostX = bossX + radius;
            int bottomY = bossY + radius;
            List<int> waterColumns = [];
            for (int x = leftmostX; x <= rightmostX; x++)
            {
                for (int y = bossY; y <= bottomY; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile.LiquidType == LiquidID.Water && tile.LiquidAmount > 0)
                    {
                        waterColumns.Add(x);
                        break;
                    }
                }
            }
            if (waterColumns.Count > 0)
            {
                int leftX = waterColumns.Min();
                int rightX = waterColumns.Max();
                StartGeyserSpawning(leftX, rightX);
            }
        }

        private void StartGeyserSpawning(int leftX, int rightX)
        {
            isSpawningGeysers = true;
            geyserSpawnTimer = 0;
            if (Main.rand.NextBool())
            {
                currentGeyserX = leftX;
                endGeyserX = rightX;
                geyserDirection = 1;

                SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/WaterImpact") with { Volume = .55f, Pitch = 1f, PitchVariance = .30f, };
                SoundEngine.PlaySound(stylea, NPC.Center);
            }
            else
            {
                currentGeyserX = rightX;
                endGeyserX = leftX;
                geyserDirection = -1;
            }
        }


        private void SpawnGeysers()
        {
            if (geyserSpawnTimer % geyserSpawnSpeed == 0)
            {
                Vector2 spawnPosition = new(currentGeyserX * 16, NPC.Bottom.Y + 150f);
                CreateWaterGeyser(spawnPosition, 0);
                currentGeyserX += geyserDirection * geyserInterval / 16;
                if ((geyserDirection == 1 && currentGeyserX > endGeyserX) ||
                (geyserDirection == -1 && currentGeyserX < endGeyserX))
                {
                    isSpawningGeysers = false;
                }
            }
            geyserSpawnTimer++;
        }


        private void CreateWaterGeyser(Vector2 position, int delay)
        {
            IEntitySource source = NPC.GetSource_FromAI();
            Vector2 velocity = new Vector2(0, -5);
            int proj = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<WaterGeyserProjectile>(), 0, 0, Main.myPlayer);
            Main.projectile[proj].timeLeft -= delay;
        }

        #endregion

        #region indicator
        private void SpawnIndicatorProjectile()
        {
            int checkX = (int)(NPC.position.X / 16);
            int lowestY = -1;
            for (int offsetY = 0; offsetY < 50; offsetY++)
            {
                int checkY = (int)(NPC.position.Y / 16) + offsetY;
                Tile tile = Framing.GetTileSafely(checkX, checkY);

                if (tile.HasTile && tile.TileType == ModContent.TileType<SmoothCavernStoneTile>())
                {
                    lowestY = checkY;
                }
            }
            if (lowestY != -1)
            {
                Vector2 indicatorPosition = new(NPC.Center.X, (lowestY - 3) * 16);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), indicatorPosition, Vector2.Zero, ModContent.ProjectileType<Indicator>(), 0, 0, Main.myPlayer);
            }
        }
        #endregion


        #region rock throwing attack

        private void PerformRockThrow()
        {
            isAttacking = true;
            List<int> rockProjectiles = [];
            List<Vector2> rockPositions = FindRockPositions();

            if (rockPositions.Count < 3)
            {
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                int rockType = ModContent.ProjectileType<RockProjectile>();
                int delay = i * 30;
                int rockIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), rockPositions[i], Vector2.Zero, rockType, 0, 0f, Main.myPlayer, NPC.whoAmI, i);

                if (rockIndex >= 0 && rockIndex < Main.maxProjectiles)
                {
                    Projectile proj = Main.projectile[rockIndex];
                    proj.frame = i;
                    proj.timeLeft += delay;
                    proj.localAI[0] = delay;
                    rockProjectiles.Add(rockIndex);
                }
            }
        }

        private List<Vector2> FindRockPositions()
        {
            List<Vector2> validPositions = [];
            int arenaWidth = 200 * 16;
            int arenaHeight = 100 * 16;

            for (int x = (int)(NPC.position.X - arenaWidth / 2); x < NPC.position.X + arenaWidth / 2; x += 16)
            {
                for (int y = (int)(NPC.position.Y - arenaHeight / 2); y < NPC.position.Y + arenaHeight / 2; y += 16)
                {
                    Tile tile = Framing.GetTileSafely(x / 16, y / 16);
                    if (tile.TileType == ModContent.TileType<CavernStoneTile>() && IsExposed(tile))
                    {
                        Vector2 position = new(x, y);
                        if (!IsNearOtherPositions(position, validPositions))
                        {
                            validPositions.Add(position);
                            if (validPositions.Count >= 3) return validPositions;
                        }
                    }
                }
            }

            return validPositions;
        }

        private static bool IsExposed(Tile tile)
        {
            Tile aboveTile = Framing.GetTileSafely(tile.TileFrameX, tile.TileFrameY - 1);
            return !aboveTile.HasTile;
        }

        private static bool IsNearOtherPositions(Vector2 position, List<Vector2> otherPositions)
        {
            foreach (var pos in otherPositions)
            {
                if (Vector2.Distance(position, pos) < 20 * 16)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        private int radialAttackPhase = 0;
        private int radialAttackTimer = 0;
        private bool isRadialAttackActive = false;

        private void HandleRadialAttack()
        {
            isAttacking = true;
            if (!isRadialAttackActive) return;

            radialAttackTimer++;

            if (radialAttackTimer == 0)
            {
                SpawnRadialProjectiles(3, 300f);
            }
            else if (radialAttackTimer == 150)
            {
                SpawnRadialProjectiles(5, 300f);
            }
            else if (radialAttackTimer == 300)
            {
                SpawnRadialProjectiles(7, 300f);

                isRadialAttackActive = false;
                isAttacking = false;
                radialAttackTimer = 0;
            }
        }

        private void SpawnRadialProjectiles(int numProjectiles, float radius)
        {
            Vector2 playerPosition = Main.player[NPC.target].Center;

            for (int i = 0; i < numProjectiles; i++)
            {
                float angle = MathHelper.ToRadians(360f / numProjectiles * i);
                Vector2 position = playerPosition + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                Vector2 direction = Vector2.Normalize(playerPosition - position);

                int projectileID = Projectile.NewProjectile(NPC.GetSource_FromAI(), position, Vector2.Zero, ModContent.ProjectileType<EnchantedEye>(), damage, knockback, Main.myPlayer);

                Main.projectile[projectileID].ai[1] = playerPosition.X;
                Main.projectile[projectileID].ai[2] = playerPosition.Y;

                Main.projectile[projectileID].rotation = direction.ToRotation() + MathHelper.PiOver2;
            }
        }
    }
}