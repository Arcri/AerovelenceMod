using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using AerovelenceMod.Content.Items.BossSummons;
using AerovelenceMod.Content.Tiles.Citadel;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Building;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Terraria.Audio;
using AerovelenceMod.Content.Items.Weapons.Misc.Magic.WandOfExploding;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Buffs;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles;
using static AerovelenceMod.Content.Projectiles.LightningUtility;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture;
using AerovelenceMod.Content.NPCs.CrystalCaverns;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public enum TumblerAttackState
    {
        Idle,
        CrystalBarrage,
        WaterLightning,
        RockingBackAndForth,
        RollToDash,
        RollToSideAndSlam,
        DashSideToSide,
        DashOuterToOuter,
        CrystalLightning,
        SpawnRadialProjectiles,
        RockThrow,
        DoubleBounce,
        CrystalDash,
        DoubleDash,
        SingleDash
    }

    [AutoloadBossHead]
    public class CrystalTumbler2 : ModNPC
    {
        private TumblerAttackState currentAttack = TumblerAttackState.Idle;
        private int attackTimer = 0;
        private int idleDuration = 400;
        private int phase = 1;

        private bool shouldSpawnHorde = false;
        private int idleAttackType = -1;
        private int idleAttackTimer = 0;
        private int idleAttackDelay = 100;
        private bool idleAttackExecuted = false;

        private bool rockingMovingRight = true;

        private int rollDashPhase = 0;
        private int rollDashTimer = 0;

        private int rollSlamPhase = 0;
        private int rollSlamTimer = 0;
        private Vector2 rollSlamStartPosition;
        private Vector2 rollSlamTargetPosition;
        private Vector2 rollSlamControlPoint;

        private int dashSidePhase = 0;
        private int dashSideTimer = 0;
        private int dashSideIteration = 0;
        private float dashSideDirection = 0f;

        private bool firstFrame = true;

        private bool isInCombo = false;
        private int currentComboIndex = 0;
        private int currentComboStep = 0;

        private int comboCycleIndex = -1;

        private static readonly TumblerAttackState[][] phase1Combos =
        {
            new TumblerAttackState[]
            {
                TumblerAttackState.RollToDash,
                TumblerAttackState.DashOuterToOuter,
                TumblerAttackState.RollToSideAndSlam,
                TumblerAttackState.DashSideToSide,
                TumblerAttackState.RollToSideAndSlam
            },

            new TumblerAttackState[]
            {
                TumblerAttackState.RollToDash,
                TumblerAttackState.DashSideToSide,
                TumblerAttackState.Idle
            },

            new TumblerAttackState[]
            {
                TumblerAttackState.RollToSideAndSlam,
                TumblerAttackState.DashSideToSide
            }
        };


        private static readonly TumblerAttackState[][] phase2Combos =
        {
            new TumblerAttackState[]
            {
                TumblerAttackState.RollToDash,
                TumblerAttackState.DoubleBounce,
                TumblerAttackState.Idle,
                TumblerAttackState.DashOuterToOuter,
                TumblerAttackState.Idle,
                TumblerAttackState.CrystalDash,
                TumblerAttackState.RollToDash,
                TumblerAttackState.DoubleBounce,
                TumblerAttackState.SingleDash
            },

            new TumblerAttackState[]
            {
                TumblerAttackState.DoubleBounce,
                TumblerAttackState.Idle,
                TumblerAttackState.RollToSideAndSlam,
                TumblerAttackState.DashSideToSide,
                TumblerAttackState.DoubleBounce,
                TumblerAttackState.Idle,
                TumblerAttackState.CrystalDash,
                TumblerAttackState.SingleDash,
                TumblerAttackState.SingleDash,
                TumblerAttackState.Idle
            },

            new TumblerAttackState[]
            {
                TumblerAttackState.DashOuterToOuter,
                TumblerAttackState.CrystalDash,
                TumblerAttackState.RollToDash,
                TumblerAttackState.DoubleBounce,
                TumblerAttackState.Idle,
                TumblerAttackState.CrystalDash,
                TumblerAttackState.DashSideToSide,
                TumblerAttackState.Idle,
                TumblerAttackState.DoubleBounce
            }
        };

        private static readonly TumblerAttackState[][] phase3Combos =
        {
            new TumblerAttackState[]
            {
                TumblerAttackState.DashOuterToOuter,
                TumblerAttackState.RollToSideAndSlam,
                TumblerAttackState.DashSideToSide
            },

            new TumblerAttackState[]
            {
                TumblerAttackState.DashOuterToOuter,
                TumblerAttackState.RollToDash,
                TumblerAttackState.DashSideToSide
            }
        };


        private string currentComboName = "";
        private string nextAttackName = "";
        private int remainingStepsInCombo = 0;

        private void StartCombo(int comboIndex)
        {
            TumblerAttackState[][] currentPhaseCombo;

            if (phase == 3 && phase2TransitionComplete)
            {
                currentPhaseCombo = phase3Combos;
                currentComboName = $"Phase 3 Combo {comboIndex + 1}";
            }
            else if (phase == 2 && phase2TransitionComplete)
            {
                currentPhaseCombo = phase2Combos;
                currentComboName = $"Phase 2 Combo {comboIndex + 1}";
            }
            else
            {
                currentPhaseCombo = phase1Combos;
                currentComboName = $"Phase 1 Combo {comboIndex + 1}";
            }

            comboIndex = Math.Clamp(comboIndex, 0, currentPhaseCombo.Length - 1);

            isInCombo = true;
            currentComboIndex = comboIndex;
            currentComboStep = 0;
            currentAttack = currentPhaseCombo[currentComboIndex][currentComboStep];
            attackTimer = 0;

            remainingStepsInCombo = currentPhaseCombo[currentComboIndex].Length;
            Main.NewText($"Starting {currentComboName} ({remainingStepsInCombo} steps)", Color.Yellow);
            nextAttackName = GetAttackName(currentAttack);
            Main.NewText($"Current attack: {nextAttackName} (Step 1/{remainingStepsInCombo})", Color.LightBlue);

            dashOuterPhase = 0;
            dashOuterTimer = 0;
            dashPlayerPhase = 0;
            dashPlayerTimer = 0;
            usePlayerDashThisTime = !usePlayerDashThisTime;
            dashVariantIsPlayerTargeted = usePlayerDashThisTime;
        }

        private void OnAttackFinished()
        {
            StopAttackVFX();
            Main.NewText($"Attack finished: {GetAttackName(currentAttack)}", Color.Orange);
            attackTimer = 0;
            idleTimer = 0;
            SpawnedOrbs = false;
            NPC.noTileCollide = false;
            NPC.noGravity = false;

            if (shouldStartPhase2Transition && !isInPhase2Transition && !phase2TransitionComplete)
            {
                Main.NewText("Attack ended, starting phase 2 transition", Color.HotPink);
                isInPhase2Transition = true;
                shouldStartPhase2Transition = false;
                phase2TransitionTimer = 0;
                isInCombo = false;
                currentAttack = TumblerAttackState.Idle;
                return;
            }

            TumblerAttackState[][] currentPhaseCombo;
            if (phase == 3 && phase2TransitionComplete)
            {
                currentPhaseCombo = phase3Combos;
            }
            else if (phase == 2 && phase2TransitionComplete)
            {
                currentPhaseCombo = phase2Combos;
            }
            else
            {
                currentPhaseCombo = phase1Combos;
            }

            if (isInCombo)
            {
                currentComboStep++;

                if (currentComboStep >= currentPhaseCombo[currentComboIndex].Length)
                {
                    Main.NewText($"Completed {currentComboName}", Color.Green);
                    isInCombo = false;
                    SelectNextAttack();
                    return;
                }
                else
                {
                    currentAttack = currentPhaseCombo[currentComboIndex][currentComboStep];
                    nextAttackName = GetAttackName(currentAttack);
                    Main.NewText($"Next attack: {nextAttackName} (Step {currentComboStep + 1}/{remainingStepsInCombo})", Color.LightBlue);
                    attackTimer = 0;
                }
            }
            else
            {
                SelectNextAttack();
            }
        }


        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return doBossAttackVFX;
        }


        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (!doBossAttackVFX)
            {
                modifiers.FinalDamage = modifiers.FinalDamage * 0f;
            }
            else
            {
                modifiers.FinalDamage = modifiers.FinalDamage * 15f;
            }
        }




        public override void SetDefaults()
        {
            NPC.damage = 5;
            NPC.width = 120;
            NPC.height = 128;
            NPC.lifeMax = 3100;
            NPC.defense = 13;
            NPC.boss = true;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.knockBackResist = 0f;

            NPC.HitSound = new SoundStyle("AerovelenceMod/Sounds/Effects/RockHit")
            {
                Volume = 0.75f,
                Pitch = 0f,
                PitchVariance = 0.4f,
            };

            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CrystalTumbler");
            }

        }

        private int rockingVariantIndex = 0;

        private int idleTimer = 0;

        private bool doAttackDrawing = false;
        private bool doBossAttackVFX = false;
        private float afterImageScale = 0.8f;
        private float glowIntensity = 0f;
        private float scaleIncreaseRate = 0.005f;
        private float intensityIncreaseRate = 0.05f;
        private float scaleResetRate = 0.006f;
        private float intensityResetRate = 0.1f;
        private int timer = 0;

        private float dashIntensity = 0f;
        private bool isDashing = false;

        private bool lightningStrikePositionsInitialized = false;
        private Vector2[] lightningStrikePositions;
        private int lightningStrikeIndex = 0;
        private int anotherTimer = 0;
        private int numberOfLightningStrikes = 7;
        private float lineExtraPower = 0;

        private void TelegraphLightningDust(Vector2 start, Vector2 end, int segmentCount = 30, float maxDisplacement = 5f)
        {
            Vector2 direction = end - start;
            float length = direction.Length();
            if (length <= 0f)
                return;
            direction.Normalize();
            Vector2 normal = new Vector2(-direction.Y, direction.X);
            float segmentLength = length / (segmentCount - 1);

            for (int i = 0; i < segmentCount; i++)
            {
                Vector2 pos = start + direction * segmentLength * i;
                float displacement = (Main.rand.NextFloat() - 0.5f) * 2f * maxDisplacement;
                pos += normal * displacement;
                int dustIndex = Dust.NewDust(pos, 4, 4, DustID.Electric, 0f, 0f, 100, Color.Cyan, 1f);
                Main.dust[dustIndex].noGravity = true;
            }
        }

        public bool doWaterRocks = false;
        int waterRockTimer = 0;
        public bool waterPositionsInitialized = false;

        private bool positionsInitialized = false;
        private Vector2[] lightningPositions;
        private int lightningCount;
        private float spacing;
        private float randomOffset;
        private Vector2 innerArenaLeft;
        private Vector2 innerArenaRight;
        private int waterLayerTile;

        private bool isPlatformDisableActive = false;
        private int platformDisableState = 0;
        private int platformDisableTimer = 0;

        private Vector2 chosenPlatformCenter;
        private Vector2 chosenPlatformCenterOffset;
        private float chosenPlatformWidthPixels;

        private int LightningTelegraphTime = 250;

        private int autoTriggerTimer = 0;
        private int tDiddy = 0;

        private int lightningTelegraphId = -1;

        public override void AI()
        {

            if (firstFrame)
            {
                firstFrame = false;
            }

            TelegraphUtility.UpdateTelegraphs();

            CheckStartPhase2Transition();

            if (timer == 0)
            {
                previousRotations = new List<float>();
                previousPostions = new List<Vector2>();
            }
            if (timer % 2 == 0)
            {
                int trailCount = 10;
                previousRotations.Add(NPC.rotation);
                previousPostions.Add(NPC.Center);

                if (previousRotations.Count > trailCount)
                    previousRotations.RemoveAt(0);

                if (previousPostions.Count > trailCount)
                    previousPostions.RemoveAt(0);
            }

            timer++;

            if (isInPhase2Transition)
            {
                ExecutePhase2Transition();
                return;
            }

            UpdateEyeEffects();

            autoTriggerTimer++;

            if (autoTriggerTimer >= 1000 && currentAttack != TumblerAttackState.DashOuterToOuter)
            {
                autoTriggerTimer = 0;
                DisableRandomPlatform();
            }

            if (isPlatformDisableActive)
            {
                platformDisableTimer++;
                switch (platformDisableState)
                {
                    case 0:
                        if (platformDisableTimer == 1)
                        {
                            SpawnLightningTelegraphGlow(chosenPlatformCenter, chosenPlatformWidthPixels);
                        }

                        if (platformDisableTimer >= 90)
                        {
                            bool pickLeftSide = Main.rand.NextBool();
                            float offsetX = Main.rand.NextFloat(300f, 500f) * (pickLeftSide ? -1f : 1f);
                            Vector2 startPos = new Vector2(chosenPlatformCenter.X + offsetX, chosenPlatformCenter.Y - 600f);
                            LightningManager.StrikeLightning(startPos, new Vector2(chosenPlatformCenter.X + 8, chosenPlatformCenter.Y), 0, telegraphTime: LightningTelegraphTime);
                            TelegraphLightningDust(startPos, new Vector2(chosenPlatformCenter.X + 8, chosenPlatformCenter.Y), segmentCount: 22, maxDisplacement: 5f);
                            platformDisableState = 1;
                            platformDisableTimer = 0;
                        }
                        break;

                    case 1: //actual field + strike spawn
                        if (platformDisableTimer >= LightningTelegraphTime)
                        {
                            if (lightningTelegraphId != -1 && lightningTelegraphId < Main.maxProjectiles && Main.projectile[lightningTelegraphId].active && Main.projectile[lightningTelegraphId].ModProjectile is LightningTelegraphGlow glow)
                                glow.StartFadeOut();

                            SpawnElectricFieldOnPlatform(chosenPlatformCenter, chosenPlatformWidthPixels);
                            isPlatformDisableActive = false;
                            platformDisableState = 0;
                            platformDisableTimer = 0;
                            lightningTelegraphId = -1;
                        }
                        break;
                }
            }

            if ((doWaterRocks && currentAttack != TumblerAttackState.DashOuterToOuter) && !doLightning)
            {
                //Main.NewText("doWaterRocks is TRUE!");
                waterRockTimer++;
                if (!positionsInitialized)
                {
                    //Main.NewText("init positions");

                    waterPositionsInitialized = true;
                    positionsInitialized = true;

                    lightningCount = Main.rand.Next(4, 7);
                    innerArenaLeft = ArenaData.InnerArenaBoundaryLeft;
                    innerArenaRight = ArenaData.InnerArenaBoundaryRight;
                    waterLayerTile = ArenaData.WaterLayer;

                    float arenaWidth = innerArenaRight.X - innerArenaLeft.X;
                    spacing = arenaWidth / lightningCount;
                    randomOffset = Main.rand.NextFloat(0, spacing);

                    lightningPositions = new Vector2[lightningCount];
                    for (int i = 0; i < lightningCount; i++)
                    {
                        float lightningX = innerArenaLeft.X + randomOffset + (i * spacing);
                        lightningPositions[i] = new Vector2(lightningX, waterLayerTile * 16);
                        Main.NewText($"position {i}: X={lightningX}, Y={waterLayerTile * 16}");
                    }
                }

                if ((positionsInitialized && lightningPositions != null) && !doWaterRocks)
                {
                    for (int i = 0; i < lightningPositions.Length; i++)
                    {
                        Vector2 lightningSpawnPosition = lightningPositions[i];
                        if (waterRockTimer % 10 == 0)
                            TelegraphDustLine(lightningSpawnPosition, 500f, 0f);
                        if (waterRockTimer % 30 == 0)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), lightningSpawnPosition, Vector2.Zero, ModContent.ProjectileType<LightningStar>(), NPC.damage / 4, 0f, Main.myPlayer);
                        if (waterRockTimer == 120)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), lightningSpawnPosition, Vector2.Zero, ModContent.ProjectileType<LightningStar>(), NPC.damage / 4, 0f, Main.myPlayer);
                            Vector2 orbVelocity = new(0, Main.rand.NextFloat(-20f, -22f));
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), lightningSpawnPosition, orbVelocity, ModContent.ProjectileType<MagneticOrb>(), NPC.damage / 4, 0f, Main.myPlayer);
                        }
                    }
                }

                if (waterRockTimer >= 120)
                {
                    Main.NewText("Resetting Water Rock Timer...");
                    waterRockTimer = 0;
                    doWaterRocks = false;
                    positionsInitialized = false;
                    waterPositionsInitialized = false;
                }
            }

            lineExtraPower = Math.Clamp(MathHelper.Lerp(lineExtraPower, -0.25f, 0.1f), 0f, 1f);

            if ((doLightning && !lightningStrikePositionsInitialized) && !doWaterRocks)
            {
                lightningStrikePositions = new Vector2[numberOfLightningStrikes];
                float arenaWidth = ArenaData.OuterArenaBoundaryRight.X - ArenaData.OuterArenaBoundaryLeft.X;
                bool bossOnLeft = NPC.Center.X < ArenaData.ArenaCenter.X;
                for (int i = 0; i < numberOfLightningStrikes; i++)
                {
                    float factor = (float)i / (numberOfLightningStrikes - 1);
                    float xPosition = ArenaData.OuterArenaBoundaryLeft.X + (arenaWidth * factor);
                    float yPosition = NPC.Center.Y - 200;
                    lightningStrikePositions[i] = new Vector2(xPosition, yPosition);
                    Main.NewText($"Lightning position {i} set at X: {xPosition}, Y: {yPosition}");
                }
    
                lightningStrikePositionsInitialized = true;
                Main.NewText($"Initialized {numberOfLightningStrikes} lightning strikes across the arena");
            }

            if (doLightning)
            {
                if (lightningStrikePositionsInitialized)
                {
                    anotherTimer++;
                    if (anotherTimer > 60)
                    {
                        if (anotherTimer % 20 == 0 && lightningStrikeIndex < numberOfLightningStrikes)
                        {
                            Vector2 strikeStart = lightningStrikePositions[lightningStrikeIndex];
                            Vector2 strikeStartOffset = strikeStart - new Vector2(0, 500f);
                            Vector2 strikeEnd = new Vector2(strikeStart.X, strikeStart.Y + 250);
                            LightningManager.StrikeLightning(strikeStartOffset, strikeEnd, LightningTelegraphTime);
                            TelegraphLightningDust(strikeStartOffset, strikeEnd, segmentCount: 22, maxDisplacement: 5f);
                            Main.NewText($"Creating lightning strike {lightningStrikeIndex} at X: {strikeStart.X}");
                            lightningStrikeIndex++;
                        }
                        if (lightningStrikeIndex >= numberOfLightningStrikes)
                        {
                            lightningStrikePositionsInitialized = false;
                            doLightning = false;
                            lightningStrikeIndex = 0;
                            anotherTimer = 0;
                            Main.NewText("All lightning strikes complete");
                        }
                    }
                }
            }


            if (isExecutingWaterLightning)
            {
                UpdateLightningWaterAttack();
            }


            if (doBossAttackVFX)
            {
                afterImageScale = MathHelper.Clamp(afterImageScale + scaleIncreaseRate, 0.8f, 1.1f);
                glowIntensity = MathHelper.Clamp(glowIntensity + intensityIncreaseRate, 0f, 1f);
            }
            else
            {
                afterImageScale = MathHelper.Clamp(afterImageScale - scaleResetRate, 0.8f, 1.1f);
                glowIntensity = MathHelper.Clamp(glowIntensity - intensityResetRate, 0f, 1f);
            }

            if (isDashing)
            {
                dashIntensity = MathHelper.Clamp(dashIntensity + 0.1f, 0f, 1f);
            }
            else
            {
                dashIntensity = MathHelper.Clamp(dashIntensity - 0.1f, 0f, 1f);
            }


            if (currentAttack != TumblerAttackState.RockingBackAndForth)
            {
                NPC.TargetClosest(true);
            }
            Player player = Main.player[NPC.target];



            player.AddBuff(ModContent.BuffType<FearsomeFoe>(), 1);

            UpdatePhase();

            float radius = NPC.width / 2f;
            float rotationPer = NPC.velocity.X / radius;



            float healthFactor = 1f - (NPC.life / (float)NPC.lifeMax);
            NPC.rotation += rotationPer * (1f + healthFactor * 0.5f);
            //float rotationFactor = 1.7f + healthFactor * 2f;
            //NPC.rotation += NPC.velocity.X / NPC.width * rotationFactor;

            if (currentAttack == TumblerAttackState.Idle)
            {
                //Main.NewText("Idle");
                Vector2 directionToPlayer = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                float desiredSpeed = MathHelper.Lerp(3f, 6f, healthFactor);
                float acceleration = 0.1f;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, directionToPlayer.X * desiredSpeed, acceleration);
                idleTimer++;
                if (Main.rand.NextBool(20))
                {
                    Vector2 dustPos = NPC.Center + new Vector2(Main.rand.NextFloat(-NPC.width / 2, NPC.width / 2),
                                                           Main.rand.NextFloat(-NPC.height / 2, NPC.height / 2));
                    Dust.NewDustPerfect(dustPos, DustID.Electric, new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)),
                                         0, Color.Cyan, 1f).noGravity = true;
                }
                if (!shouldSpawnHorde && idleTimer >= idleAttackDelay && !idleAttackExecuted)
                {
                    idleAttackType = 0;
                    shouldSpawnHorde = true;
                    idleAttackTimer = 0;
                }
                if (shouldSpawnHorde)
                {
                    idleAttackTimer++;
                    switch (idleAttackType)
                    {
                        case 0:
                            PerformSpawnTheHorde();
                            break;
                        case 1:
                            StartLightningPoints();
                            break;

                    }

                    if (!shouldSpawnHorde)
                    {
                        idleAttackExecuted = true;
                    }
                }

                attackTimer++;
                if (attackTimer >= idleDuration)
                {
                    shouldSpawnHorde = false;
                    idleAttackType = -1;
                    idleAttackTimer = 0;
                    idleAttackExecuted = false;
                    if (!isInCombo)
                    {
                        SelectNextAttack();
                    }
                    else
                    {
                        OnAttackFinished();
                    }

                    attackTimer = 0;
                    idleTimer = 0;
                }
            }
            else if (currentAttack == TumblerAttackState.RockingBackAndForth)
            {
                //Main.NewText("Rocking");
                bool useMagnetRocks = false;
                bool useArenaCrystalZappers = false;
                bool usePhase3 = false;

                if (rockingVariantIndex == 0)
                {
                    useMagnetRocks = true;
                }
                else if (rockingVariantIndex == 1)
                {
                    useArenaCrystalZappers = true;
                }

                RockingBackAndForthAttack(useMagnetRocks, useArenaCrystalZappers, usePhase3, player);

                rockingVariantIndex = (rockingVariantIndex + 1) % 2;
            }
            else if (currentAttack == TumblerAttackState.RollToDash)
            {

                //Main.NewText("Roll to dash");
                RollToDashAttack();
            }
            else if (currentAttack == TumblerAttackState.RollToSideAndSlam)
            {
                //Main.NewText("Roll to side");
                RollToSideAndSlamAttack(player);
            }
            else if (currentAttack == TumblerAttackState.DashOuterToOuter)
            {

                //Main.NewText("Dash Outer to Outer");
                DashOuterToOuterSequence(player);
            }
            if (currentAttack == TumblerAttackState.DashSideToSide)
            {
                //Main.NewText("dash side to side");
                DashSideToSideSequence(Main.player[NPC.target]);
            }
            if (currentAttack == TumblerAttackState.WaterLightning)
            {

                //Main.NewText("Water lightning");
                WaterLightningAttack();
            }
            else if (currentAttack == TumblerAttackState.DoubleBounce)
            {
                DoubleBounceAttack(Main.player[NPC.target]);
            }
            else if (currentAttack == TumblerAttackState.CrystalDash)
            {
                //Main.NewText("Crystal Dash");
                CrystalDashAttack(Main.player[NPC.target]);
            }
            else if (currentAttack == TumblerAttackState.DoubleDash)
            {
                //Main.NewText("Double Dash");
                DoubleDashAttack(Main.player[NPC.target]);
            }
            else if (currentAttack == TumblerAttackState.SingleDash)
            {
                //Main.NewText("Single Dash");
                SingleDashAttack(Main.player[NPC.target]);
            }
            else
            {
                Vector2 directionToPlayer = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                float desiredSpeed = MathHelper.Lerp(3f, 6f, healthFactor);
                float acceleration = 0.1f;
                switch (currentAttack)
                {
                    case TumblerAttackState.CrystalBarrage:
                        //Main.NewText("Crystal Barrage");
                        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, -directionToPlayer.X * desiredSpeed, acceleration);
                        CrystalBarrageAttack();
                        break;
                    case TumblerAttackState.CrystalLightning:
                        if (!crystalElectrocutePhaseActive)
                        {
                            StartArenaCrystalElectrocution();
                            //Main.NewText("Electric phase");
                        }
                        ArenaCrystalElectrocutionSequence(player);
                        break;
                    default:
                        break;
                }
            }
        }


        private void ExecutePhase2Transition()
        {
            phase2TransitionTimer++;
            float progress = (float)phase2TransitionTimer / phase2TransitionLength;
            float halfwayPoint = 0.5f;
            if (progress < 0.2f)
            {
                MoveTowardsCenter(progress);
            }
            if (progress > 0.15f && progress < 0.3f && eyeActive)
            {
                StopEye();
            }
            if (progress >= halfwayPoint && progress < 0.8f)
            {
                shieldVfxAlpha = MathHelper.Lerp(0f, 1f, (progress - halfwayPoint) * 3.33f);
                activateShieldVFX = true;
            }
            if (progress < 0.8f)
            {
                SpawnEnergyAbsorptionOrbs();
            }
            float defenseBoostFactor = progress < 0.5f ? progress * 2 : 1f;
            tempDefenseBoost = (int)(75 * defenseBoostFactor);
            if (progress > 0.85f && !eyeActive)
            {
                StartEye();
            }
            if (phase2TransitionTimer >= phase2TransitionLength)
            {
                CompletePhase2Transition();
            }
        }

        private void MoveTowardsCenter(float progress)
        {
            Vector2 centerPosition = ArenaData.ArenaCenter;
            Vector2 directionToCenter = centerPosition - NPC.Center;
            float distanceToCenter = directionToCenter.Length();
            if (distanceToCenter > 70f)
            {
                float speedFactor = 1f - (progress * 5f);
                speedFactor = MathHelper.Clamp(speedFactor, 0.1f, 1f);
                float maxSpeed = 10f;
                float speed = MathHelper.Lerp(maxSpeed * speedFactor, 0.5f, Math.Max(0, 1f - distanceToCenter / 300f));
                directionToCenter.Normalize();
                float acceleration = 0.15f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, directionToCenter * speed, acceleration);

                float radius = NPC.width / 2f;
                float rotationPerFrame = NPC.velocity.X / radius;
                NPC.rotation += rotationPerFrame;
            }
            else
            {
                float radius = NPC.width / 2f;
                float rotationPerFrame = NPC.velocity.X / radius;
                NPC.rotation += rotationPerFrame;
                NPC.velocity *= 0.95f;
                if (NPC.velocity.Length() < 0.1f)
                    NPC.velocity = Vector2.Zero;
            }
        }


        private List<Vector2> recentOrbPositions = [];
        private float MIN_ORB_DISTANCE = 50f;

        private void SpawnEnergyAbsorptionOrbs()
        {
            orbSpawnCounter++;
            if (orbSpawnCounter >= 35)
            {
                orbSpawnCounter = 0;
                for (int i = 0; i < 5; i++)
                {
                    int edge = Main.rand.Next(4);
                    float angle = 0;

                    switch (edge)
                    {
                        case 0: angle = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4); break; //top
                        case 1: angle = Main.rand.NextFloat(MathHelper.PiOver4, 3 * MathHelper.PiOver4); break; //right
                        case 2: angle = Main.rand.NextFloat(3 * MathHelper.PiOver4, 5 * MathHelper.PiOver4); break; //bottom
                        case 3: angle = Main.rand.NextFloat(5 * MathHelper.PiOver4, 7 * MathHelper.PiOver4); break; //left
                    }
                    float distance = 800f;
                    Vector2 spawnPos = NPC.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * distance;

                    bool tooClose = false;
                    foreach (Vector2 pos in recentOrbPositions)
                    {
                        if (Vector2.Distance(pos, spawnPos) < MIN_ORB_DISTANCE)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (!tooClose)
                    {
                        int projIndex = Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero,
                            ModContent.ProjectileType<EnergyAbsorptionOrb>(), 0, 0f, Main.myPlayer, ai0: NPC.whoAmI);

                        if (projIndex >= 0)
                        {
                            recentOrbPositions.Add(spawnPos);
                            if (recentOrbPositions.Count > 10)
                                recentOrbPositions.RemoveAt(0);
                        }
                    }
                }
            }
        }

        private void ExecuteLightningStrikes()
        {
            if (lightningOrbPositions.Count > 0 && phase2TransitionTimer % 15 == 0)
            {
                int index = phase2TransitionTimer % lightningOrbPositions.Count;
                Vector2 strikeStart = lightningOrbPositions[index];
                Vector2 strikeEnd = new Vector2(strikeStart.X, ArenaData.WaterLayer * 16);
                LightningManager.StrikeLightning(strikeStart, strikeEnd, 60);
                Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 10;
            }
        }

        private void UpdateEyeEffects()
        {
            if (eyeActive && eyeAlpha < 1f)
            {
                eyeAlpha = MathHelper.Lerp(eyeAlpha, 1.1f, 0.05f);
                if (eyeAlpha > 1f) eyeAlpha = 1f;
            }
            else if (!eyeActive && eyeAlpha > 0f)
            {
                eyeAlpha = MathHelper.Lerp(eyeAlpha, -0.1f, 0.05f);
                if (eyeAlpha < 0f) eyeAlpha = 0f;
            }
            if (EyeGlow)
            {
                eyeGlowAlpha = MathHelper.Lerp(eyeGlowAlpha, 1.1f, 0.1f);
                if (eyeGlowAlpha > 1f) eyeGlowAlpha = 1f;
            }
            else if (eyeGlowAlpha > 0f)
            {
                eyeGlowAlpha -= 0.02f;
                if (eyeGlowAlpha < 0f) eyeGlowAlpha = 0f;
            }
        }

        private void StartEye()
        {
            eyeActive = true;
            EyeGlow = true;
        }

        private void StopEye()
        {
            eyeActive = false;
            EyeGlow = false;
        }

        private void CompletePhase2Transition()
        {
            isInPhase2Transition = false;
            phase2TransitionComplete = true;
            tempDefenseBoost = 0;
            lightningOrbPositions.Clear();
            currentAttack = TumblerAttackState.Idle;
            attackTimer = 0;
            idleTimer = 0;
            comboCycleIndex = 0;
            StartCombo(0);

            /*SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/CrystalChime")
            {
                Volume = 0.85f,
                Pitch = -0.2f,
                PitchVariance = 0.1f,
            };
            SoundEngine.PlaySound(style, NPC.Center);*/

            Main.NewText("phase 2!? On my BOULDER BOSS!?", Color.DeepSkyBlue);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (isInPhase2Transition)
            {
                modifiers.Defense.Flat += tempDefenseBoost;
            }
            base.ModifyHitByProjectile(projectile, ref modifiers);
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (isInPhase2Transition)
            {
                modifiers.Defense.Flat += tempDefenseBoost;
            }
            base.ModifyHitByItem(player, item, ref modifiers);
        }

        private void PerformSpawnTheHorde()
        {
            if (idleAttackTimer == 1)
            {
                int mothCount = 5;
                float spawnHeight = NPC.Center.Y - 500f;

                for (int i = 0; i < mothCount; i++)
                {
                    float progressAcross = (float)i / (mothCount - 1);
                    float spawnX = MathHelper.Lerp(
                        ArenaData.InnerArenaBoundaryLeft.X + 100f,
                        ArenaData.InnerArenaBoundaryRight.X - 100f,
                        progressAcross
                    );
                    spawnX += Main.rand.NextFloat(-50f, 50f);
                    float spawnY = spawnHeight + Main.rand.NextFloat(-30f, 30f);
                    int mothIndex = NPC.NewNPC(
                        NPC.GetSource_FromThis(),
                        (int)spawnX,
                        (int)spawnY,
                        ModContent.NPCType<Bomber>()
                    );
                    if (mothIndex >= 0 && mothIndex < Main.maxNPCs)
                    {
                        Main.npc[mothIndex].ai[0] = Main.rand.NextFloat(0, MathHelper.TwoPi);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, mothIndex);
                        }
                    }
                }
                /*SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/CrystalChime")
                {
                    Volume = 0.8f,
                    Pitch = -0.2f,
                    PitchVariance = 0.2f,
                };
                SoundEngine.PlaySound(style, NPC.Center);*/
            }
            if (idleAttackTimer >= 60)
            {
                shouldSpawnHorde = false;
            }
        }

        private bool orbitersActive = false;
        private int orbiterTimer = 0;
        private int[] orbiterProjectileIds = new int[3];
        private float orbiterRotation = 0f;
        private float orbiterRadius = 150f;

        private void StartLightningPoints()
        {
            Main.NewText("Lightning Points");
            orbitersActive = true;
            orbiterTimer = 0;
            orbiterRotation = 0f;
            /*SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/CrystalChime")
            {
                Volume = 0.8f,
                Pitch = 0.1f,
                PitchVariance = 0.2f,
            };
            SoundEngine.PlaySound(style, NPC.Center);*/
            NPC.velocity *= 0.5f;
            for (int i = 0; i < 3; i++)
            {
                float angle = i * MathHelper.TwoPi / 3f;
                Vector2 offset = new Vector2(
                    (float)Math.Cos(angle) * orbiterRadius,
                    (float)Math.Sin(angle) * orbiterRadius
                );

                Vector2 spawnPos = NPC.Center + offset;
                int projId = Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<LightningStar>(), NPC.damage / 3, 0f, Main.myPlayer);
                orbiterProjectileIds[i] = projId;
            }
        }

        private bool showAfterImage = true;
        public List<float> previousRotations;
        public List<Vector2> previousPostions;

        private void StartAttackVFX()
        {
            if (!doBossAttackVFX)
            {
                doBossAttackVFX = true;
                doAttackDrawing = true;
            }
        }

        private void StopAttackVFX()
        {
            doBossAttackVFX = false;
            doAttackDrawing = false;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D npcTexture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalTumbler");
            Texture2D npcTextureGlow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalTumblerFuzzy");
            Texture2D eyeTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalTumbler2Eye").Value;
            if (showAfterImage)
            {
                #region after image
                if (previousRotations != null && previousPostions != null)
                {
                    for (int i = 0; i < previousRotations.Count; i++)
                    {
                        float progress = (float)i / previousRotations.Count;
                        Color col = Color.DeepSkyBlue * Easings.easeOutCirc(progress) * 0.5f;
                        Main.EntitySpriteDraw(npcTexture, previousPostions[i] - Main.screenPosition + new Vector2(0, 4), null, col with { A = 0 } * progress * 0.9f, previousRotations[i], npcTexture.Size() / 2f, afterImageScale, SpriteEffects.None);
                    }
                }
                #endregion
                for (int i = 0; i < 8; i++)
                {
                    Color col = i == 0 ? Color.SkyBlue with { A = 0 } : Color.DeepSkyBlue with { A = 0 };

                    Main.EntitySpriteDraw(npcTexture, NPC.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f) + new Vector2(0, 4), null, col * 1f, NPC.rotation, npcTexture.Size() / 2f, afterImageScale, SpriteEffects.None, 0f);
                }

                if (phase == 2)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                    for (int i = 0; i < 8; i++)
                    {
                        Color col = i == 0 ? Color.Purple with { A = 0 } : Color.Aqua with { A = 0 };

                        Main.EntitySpriteDraw(npcTextureGlow, NPC.Center - Main.screenPosition + new Vector2(0, 6), null, Color.White, NPC.rotation, npcTextureGlow.Size() / 2f, 1.65f, SpriteEffects.None, 0f);
                    }
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                    
                }
                Main.EntitySpriteDraw(npcTexture, NPC.Center - Main.screenPosition + new Vector2(0, 4), null, drawColor, NPC.rotation, npcTexture.Size() / 2, 1f, SpriteEffects.None, 0f);
                Main.EntitySpriteDraw(npcTexture, NPC.Center - Main.screenPosition + new Vector2(0, 4), null, Color.White with { A = 0 } * 0.25f, NPC.rotation, npcTexture.Size() / 2, 1, SpriteEffects.None, 0f);
            }
            else
            {
                Main.EntitySpriteDraw(npcTexture, NPC.Center - Main.screenPosition + new Vector2(0, 4), null, drawColor, NPC.rotation, npcTexture.Size() / 2, 1f, SpriteEffects.None, 0f);
            }

            if(phase == 2)
            {
                for (int i = 0; i < 8; i++)
                {
                    Color col = i == 0 ? Color.SkyBlue with { A = 0 } : Color.DeepSkyBlue with { A = 0 };

                    Main.EntitySpriteDraw(eyeTexture, NPC.Center - Main.screenPosition + Main.rand.NextVector2Circular(5f, 5f) + new Vector2(0, 4), null, col * 1f, NPC.rotation, npcTexture.Size() / 2f, afterImageScale, SpriteEffects.None, 0f);
                }
            }
            return false;
        }


        int funTimer = 0;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            funTimer++;
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
            Vector2 drawPosition2 = NPC.Center - Main.screenPosition;
            Vector2 origin = NPC.frame.Size() / 2f;
            Texture2D Dash = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/zFadeCircle");
            Texture2D WaveGlow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/whiteFireEye");
            spriteBatch.Draw(texture, drawPosition, NPC.frame, Color.White, NPC.rotation, origin, 1f, SpriteEffects.None, 0);
            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D Bloommy = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/Bloommy");

            Main.EntitySpriteDraw(Bloommy, drawPosition, NPC.frame, Color.White * glowIntensity, NPC.rotation, NPC.frame.Size() / 2f, 1f, SpriteEffects.None, 0);
            SpriteEffects flipEffect = (dashSideDirection == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            //Main.EntitySpriteDraw(Dash, drawPosition, null, Color.Aquamarine * dashIntensity, NPC.rotation / 2, new Vector2(Dash.Width / 2f, Dash.Height / 2f), 0.4f, flipEffect, 0);
            Main.EntitySpriteDraw(WaveGlow, drawPosition, null, Color.DeepSkyBlue * dashIntensity, 0, new Vector2(WaveGlow.Width / 2f, WaveGlow.Height / 2f), 0.58f, SpriteEffects.None, 0);

            TelegraphUtility.DrawAllTelegraphs(spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            if (eyeAlpha > 0f || eyeGlowAlpha > 0f)
            {
                Texture2D eyeTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalTumbler2Eye").Value;
                spriteBatch.Draw(eyeTexture, drawPosition, null, Color.White * eyeAlpha, NPC.rotation, eyeTexture.Size() / 2, 1f, SpriteEffects.None, 0);
                if (eyeGlowAlpha > 0f)
                {
                    Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/Pixel/GlowStar").Value;
                    Vector2 eyeStarDrawPos = NPC.Center - Main.screenPosition;
                    float eyeStarRotation = NPC.rotation;
                    float eyeStarValue = 0.5f;

                    for (int al = 0; al < 2; al++)
                    {
                        Color fadeColor = (Color.Aqua * eyeGlowAlpha) * 0.2f;
                        Main.spriteBatch.Draw(Flare, eyeStarDrawPos, Flare.Frame(1, 1, 0, 0), fadeColor, eyeStarRotation, Flare.Size() / 2, eyeStarValue * 2f, SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(Flare, eyeStarDrawPos, Flare.Frame(1, 1, 0, 0), fadeColor * 0.4f, eyeStarRotation, Flare.Size() / 2, eyeStarValue * 2.5f, SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(Flare, eyeStarDrawPos, Flare.Frame(1, 1, 0, 0), (Color.Aqua * eyeGlowAlpha) * 0.2f, eyeStarRotation * -1, Flare.Size() / 2, eyeStarValue * 0.8f, SpriteEffects.None, 0f);
                    }
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            if (activateShieldVFX && shieldVfxAlpha > 0f)
            {
                Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/Orbs/whiteFireEye").Value;
                Texture2D Flare2 = Mod.Assets.Request<Texture2D>("Assets/Orbs/spiky_20fade").Value;
                Texture2D Flare3 = Mod.Assets.Request<Texture2D>("Assets/Flare/pixelKennySlash").Value;
                Texture2D Ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;


                Color shieldColor = Color.DeepSkyBlue * 0.2f * shieldVfxAlpha;
                Color dodgerColor = Color.DodgerBlue * 0.2f * shieldVfxAlpha;
                Color skyColor = Color.SkyBlue * 0.2f * shieldVfxAlpha;
                Color whiteColor = Color.White * 0.15f * shieldVfxAlpha;

                Main.spriteBatch.End();

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
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
                myEffect.Parameters["colorIntensity"].SetValue(0.5f * shieldVfxAlpha);
                Main.spriteBatch.Draw(Ball, NPC.Center - Main.screenPosition, null, (Color.Black * 0.3f) * shieldVfxAlpha, NPC.rotation, Ball.Size() / 2, 0.5f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Ball, NPC.Center - Main.screenPosition, null, (Color.DeepSkyBlue * 0.2f) * shieldVfxAlpha, NPC.rotation, Ball.Size() / 2, 2f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare, NPC.Center - Main.screenPosition, null, (Color.DodgerBlue * 0.2f) * shieldVfxAlpha, 0.2f, Flare.Size() / 2, 0.75f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare, NPC.Center - Main.screenPosition, null, (Color.SkyBlue * 0.2f) * shieldVfxAlpha, 0.2f, Flare.Size() / 2, 0.75f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare, NPC.Center - Main.screenPosition, null, (Color.White * 0.15f) * shieldVfxAlpha, 0.2f, Flare.Size() / 2, 0.35f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare, NPC.Center - Main.screenPosition, null, (Color.White * 0.15f) * shieldVfxAlpha, 0.2f, Flare.Size() / 2, 0.35f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(Ball, NPC.Center - Main.screenPosition, null, (new Color(255, 255, 255, 0) * 0.2f) * shieldVfxAlpha, 0.2f, Ball.Size() / 2, 0.45f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare3, NPC.Center - Main.screenPosition, null, (new Color(255, 255, 255, 0) * 0.2f) * shieldVfxAlpha, 0.2f, Flare3.Size() / 2, 0.4f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare3, NPC.Center - Main.screenPosition, null, (new Color(255, 255, 255, 0) * 0.2f) * shieldVfxAlpha, 0.2f, Flare3.Size() / 2, 0.7f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare2, NPC.Center - Main.screenPosition, null, (new Color(255, 255, 255, 0) * 0.2f) * shieldVfxAlpha, 0.2f, Flare2.Size() / 2, 0.3f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare2, NPC.Center - Main.screenPosition, null, (new Color(255, 255, 255, 0) * 0.2f) * shieldVfxAlpha, 0.2f, Flare2.Size() / 2, 0.5f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                if (EyeGlow)
                {
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
                        //Main.spriteBatch.Draw(Flare, eyeStarDrawPos, Flare.Frame(1, 1, 0, 0), fadeColor, eyeStarRotation, Flare.Size() / 2, eyeStarValue * 2f, SpriteEffects.None, 0f);
                        //Main.spriteBatch.Draw(Flare, eyeStarDrawPos, Flare.Frame(1, 1, 0, 0), fadeColor * 0.4f, eyeStarRotation, Flare.Size() / 2, eyeStarValue * 2.5f, SpriteEffects.None, 0f);
                        //Main.spriteBatch.Draw(Flare, eyeStarDrawPos, Flare.Frame(1, 1, 0, 0), Color.White * eyeGlowAlpha, eyeStarRotation * -1, Flare.Size() / 2, eyeStarValue * 0.8f, SpriteEffects.None, 0f);
                    }
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

        }

        private void SelectNextAttack()
        {
            TumblerAttackState[][] currentPhaseCombo;
            string phaseText = "";

            if (phase == 3 && phase2TransitionComplete)
            {
                currentPhaseCombo = phase3Combos;
                phaseText = "Phase 3";
            }
            else if (phase == 2 && phase2TransitionComplete)
            {
                currentPhaseCombo = phase2Combos;
                phaseText = "Phase 2";
            }
            else
            {
                currentPhaseCombo = phase1Combos;
                phaseText = "Phase 1";
            }

            comboCycleIndex++;
            if (comboCycleIndex >= currentPhaseCombo.Length)
            {
                comboCycleIndex = 0;
                Main.NewText($"Cycling back to first {phaseText} combo", Color.Purple);
            }
            else
            {
                Main.NewText($"Moving to next {phaseText} combo ({comboCycleIndex + 1}/{currentPhaseCombo.Length})", Color.Purple);
            }

            StartCombo(comboCycleIndex);
        }

        private string GetAttackName(TumblerAttackState state)
        {
            return state switch
            {
                TumblerAttackState.Idle => "Idle",
                TumblerAttackState.CrystalBarrage => "Crystal Barrage",
                TumblerAttackState.WaterLightning => "Water Lightning",
                TumblerAttackState.RockingBackAndForth => "Rocking Back And Forth",
                TumblerAttackState.RollToDash => "Roll To Dash",
                TumblerAttackState.RollToSideAndSlam => "Roll To Side And Slam",
                TumblerAttackState.DashSideToSide => "Dash Side To Side",
                TumblerAttackState.DashOuterToOuter => "Dash Outer To Outer",
                TumblerAttackState.CrystalLightning => "Crystal Lightning",
                TumblerAttackState.SpawnRadialProjectiles => "Spawn Radial Projectiles",
                TumblerAttackState.RockThrow => "Rock Throw",
                TumblerAttackState.DoubleBounce => "Double Bounce",
                TumblerAttackState.CrystalDash => "Crystal Dash",
                TumblerAttackState.DoubleDash => "Double Dash",
                TumblerAttackState.SingleDash => "Single Dash",
                _ => state.ToString()
            };
        }

        private void PerformRockThrow()
        {
            //Main.NewText("Performing rock throw");
            attackTimer++;
            int rockType = ModContent.ProjectileType<RockProjectile>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == rockType && Main.projectile[i].ai[0] == NPC.whoAmI)
                {
                    OnAttackFinished();
                    return;
                }
            }
            if (attackTimer == 1)
            {
                List<int> rockProjectiles = [];
                for (int i = 0; i < 3; i++)
                {
                    int delay = i * 30;
                    int rockIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, rockType, 0, 0f, Main.myPlayer, NPC.whoAmI, i);

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
            if (attackTimer > 120)
            {
                OnAttackFinished();
            }
        }



        private void SpawnCrystalSpikeWall(Vector2 position)
        {
            //Main.NewText("Spawn Wall");
            int spikeCount = 10;
            float radius = 64f;

            for (int i = 0; i < spikeCount; i++)
            {
                float angle = MathHelper.TwoPi * i / spikeCount;
                Vector2 spawnOffset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                Vector2 spawnPosition = position + spawnOffset;

                Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPosition, Vector2.Zero,
                    ModContent.ProjectileType<CrystalSpike>(), NPC.damage / 2, 0f, Main.myPlayer);
            }
        }

        private void PerformWallSlam(Vector2 position, int screenShakePower)
        {
            StopAttackVFX();
            //Main.NewText("Performing rock slam");

            SoundStyle style = new("AerovelenceMod/Sounds/Effects/HardRockSlam")
            {
                Volume = 0.75f,
                Pitch = 1f,
                PitchVariance = 0f,
            };
            SoundEngine.PlaySound(style, NPC.Center);

            Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = screenShakePower;
            SpawnStalactiteProjectiles();

            int spikeCount = 10;
            float radius = 64f;
            float shardSpeed = 8f;

            for (int i = 0; i < spikeCount; i++)
            {
                float angle = MathHelper.TwoPi * i / spikeCount;
                Vector2 spawnOffset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                Vector2 spawnPosition = position + spawnOffset;
                Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPosition, Vector2.Zero,
                    ModContent.ProjectileType<LightningStar>(), NPC.damage / 2, 0f, Main.myPlayer);
                /*Vector2 shardVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * shardSpeed;
                Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPosition, shardVelocity,
                    ModContent.ProjectileType<SharpCrystalShard>(), NPC.damage / 2, 0f, Main.myPlayer);*/
            }
        }


        private int lastSpawnDirection = 0;


        private void SpawnRadialProjectiles()
        {
            //Main.NewText("Spawning radial projectiles");

            Vector2 playerPosition = Main.player[NPC.target].Center;
            float spawnDistance = 200f;
            int numProjectiles = 3;
            lastSpawnDirection = (lastSpawnDirection + 1) % 4;
            float baseAngle = 0f;

            switch (lastSpawnDirection)
            {
                case 0: baseAngle = -MathHelper.PiOver2; break; //top
                case 1: baseAngle = 0; break; //right
                case 2: baseAngle = MathHelper.PiOver2; break; //bottom
                case 3: baseAngle = MathHelper.Pi; break; //left
            }

            for (int i = 0; i < numProjectiles; i++)
            {
                float angleOffset = 25f * (MathHelper.Pi / 180f);

                float angle = baseAngle - angleOffset + (i * angleOffset * 2);
                Vector2 position = playerPosition + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * spawnDistance;
                Vector2 direction = Vector2.Normalize(playerPosition - position);

                int projectileID = Projectile.NewProjectile(NPC.GetSource_FromAI(), position, Vector2.Zero,
                    ModContent.ProjectileType<EnchantedEye>(), damage, 1, Main.myPlayer);

                Main.projectile[projectileID].ai[1] = playerPosition.X;
                Main.projectile[projectileID].ai[2] = playerPosition.Y;
                Main.projectile[projectileID].localAI[0] = 0f;
                Main.projectile[projectileID].rotation = direction.ToRotation() + MathHelper.PiOver2;
            }
        }

        private bool isInPhase2Transition = false;
        private int phase2TransitionTimer = 0;
        private int phase2TransitionLength = 500;
        private bool phase2TransitionComplete = false;
        private bool shouldStartPhase2Transition = false;
        private bool eyeActive = true;
        private float eyeAlpha = 1f;
        private float shieldVfxAlpha = 0f;
        private bool activateShieldVFX = false;
        private int tempDefenseBoost = 0;
        private bool hasSpawnedLightningOrbs = false;
        private List<Vector2> lightningOrbPositions = new List<Vector2>();
        private int orbSpawnCounter = 0;
        private float eyeGlowAlpha = 0f;
        private bool EyeGlow = false;


        private void UpdatePhase()
        {
            float lifePercent = (float)NPC.life / NPC.lifeMax;
            if (phase == 1 && lifePercent <= 0.5f && !shouldStartPhase2Transition && !isInPhase2Transition && !phase2TransitionComplete)
            {
                Main.NewText("Phase 2 transition queued");
                shouldStartPhase2Transition = true;
                phase = 2;
            }
            else if (phase == 2 && lifePercent <= 0.25f && phase2TransitionComplete)
            {
                phase = 3;
                /*SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/CrystalChime")
                {
                    Volume = 0.95f,
                    Pitch = 0.1f,
                    PitchVariance = 0.1f,
                };
                SoundEngine.PlaySound(style, NPC.Center);*/

                Main.NewText("phase 3 ON MY BOULDER BOSS?!?!", Color.HotPink);
                comboCycleIndex = 0;
                StartCombo(0);
            }
        }

        private void CheckStartPhase2Transition()
        {
            if (shouldStartPhase2Transition && !isInCombo)
            {
                if (currentAttack != TumblerAttackState.Idle)
                {
                    return;
                }

                Main.NewText("Beginning Phase 2 transition");
                isInPhase2Transition = true;
                shouldStartPhase2Transition = false;
                phase2TransitionTimer = 0;

                currentAttack = TumblerAttackState.Idle;
                attackTimer = 0;
                idleTimer = 0;

                StopAttackVFX();
                NPC.noTileCollide = false;
                NPC.noGravity = false;

                if (!eyeActive)
                {
                    eyeActive = true;
                    eyeAlpha = 1f;
                }

                rollDashPhase = 0;
                rollSlamPhase = 0;
                dashSidePhase = 0;
                dashOuterPhase = 0;
            }
        }


        private void CrystalBarrageAttack()
        {
            StartAttackVFX();
            //Main.NewText("Crystal Barrage");
            attackTimer++;
            if (attackTimer == 10)
            {
                //SpawnRadialProjectiles();
            }

            if (attackTimer == 20)
            {
                //SpawnMoth();
            }

            if (attackTimer > 60)
            {
                OnAttackFinished();
            }
        }

        private List<Vector2> pending = new List<Vector2>();
        private bool isExecutingWaterLightning = false;
        private int waterLightningTimer = 0;
        private bool spawnFromLeft = true;

        private void UpdateLightningWaterAttack()
        {
            //Main.NewText("Water Lightning");
            waterLightningTimer++;

            if (waterLightningTimer == 0)
            {

            }

            if (waterLightningTimer < pending.Count * 30)
            {
                int currentStrikeIndex = waterLightningTimer / 30;
                if (currentStrikeIndex < pending.Count)
                {
                    TelegraphDustLine(pending[currentStrikeIndex], 150f, 0);
                }
            }
            if (waterLightningTimer == pending.Count * 30)
            {

            }

            if (waterLightningTimer > pending.Count * 30 + 30)
            {
                isExecutingWaterLightning = false;
                OnAttackFinished();
            }
        }

        private void WaterLightningAttack()
        {
            //Main.NewText("Water Lightning");
            attackTimer++;

            if (attackTimer == 20)
            {

            }

            if (attackTimer > 60)
            {
                OnAttackFinished();
            }
        }


        private void ShootCrystals()
        {
            if (attackTimer % 20 == 0)
            {
                float spread = MathHelper.ToRadians(15);
                int numProjectiles = 3;
                float baseSpeed = 8f;

                for (int i = 0; i < numProjectiles; i++)
                {
                    float angle = MathHelper.Lerp(-spread, spread, i / (float)(numProjectiles - 1));
                    Vector2 velocity = new Vector2(baseSpeed, 0).RotatedBy(angle) * (NPC.velocity.X > 0 ? 1 : -1);

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity, ModContent.ProjectileType<CrystalShard>(), NPC.damage / 4, 0f, Main.myPlayer);
                }
            }
        }

        private void TelegraphDustLine(Vector2 position, float lineHeight, float angle)
        {
            int dustCount = Math.Max(1, (int)(lineHeight / 24));

            for (int i = 0; i < dustCount; i++)
            {
                float yOffset = (i / (float)(dustCount - 1)) * lineHeight;
                Vector2 offsetDirection = new Vector2(0, -yOffset).RotatedBy(angle);
                Vector2 dustPosition = position + offsetDirection;

                int dust = Dust.NewDust(dustPosition, 4, 4, DustID.BlueCrystalShard, 0f, -0.5f, 100, default, 1.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.2f;
            }
        }




        private float chosenDashDirection = 0f;
        private float storedExtraSpin = 0f;
        private bool rollDashStunned = false;
        private bool dashNearWall = false;

        private void RollToDashAttack()
        {
            //Main.NewText("Roll To Dash, Phase: " + rollDashPhase);
            switch (rollDashPhase)
            {
                case 0:
                    {
                        NPC.velocity = new Vector2(NPC.velocity.X * 0.95f, NPC.velocity.Y * 0.95f);
                        if (Math.Abs(NPC.velocity.X) < 0.2f && Math.Abs(NPC.velocity.Y) < 0.2f)
                        {
                            NPC.velocity = Vector2.Zero;
                            rollDashPhase = 1;
                            rollDashTimer = 0;
                            storedExtraSpin = 0f;
                        }
                    }
                    break;

                case 1:
                    {
                        Player target = Main.player[NPC.target];
                        float spinDirection = Math.Sign(target.Center.X - NPC.Center.X);
                        float targetSpinRate = 1.01f;
                        float spinIncrement = targetSpinRate / 300f;
                        storedExtraSpin = Math.Min(storedExtraSpin + spinIncrement, targetSpinRate);
                        SpawnOrbProjectiles();
                        NPC.rotation += spinDirection * storedExtraSpin;

                        if (rollDashTimer <= 0)
                        {
                            int telegraphID = TelegraphUtility.DrawTelegraph(() => NPC.Center, 500f, new Vector2(target.position.X, target.position.Y), 1f, 10f, focus: true, aimAtTarget: true, () => new Vector2(target.position.X, target.position.Y), Color.Aqua, true, false, 60f);
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 dustPos = new Vector2(
                                NPC.position.X + Main.rand.Next(NPC.width),
                                NPC.position.Y + NPC.height - 8
                            );
                            float dustSpeed = Main.rand.NextFloat(3f, 6f);
                            Vector2 dustVel = new Vector2(dashSideDirection > 0 ? -dustSpeed : dustSpeed, Main.rand.NextFloat(-1f, -0.2f));
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

                        if (rollDashTimer % 15 == 0)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                float randomAngle = Main.rand.NextFloat(0, MathHelper.TwoPi);
                                Vector2 direction = new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle));
                                float projectileSpeed = 10f;
                               // Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * projectileSpeed, ModContent.ProjectileType<CrystalShard>(), NPC.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                        rollDashTimer++;
                        if (rollDashTimer >= 90)
                        {
                            float diff = target.Center.X - NPC.Center.X;
                            chosenDashDirection = Math.Sign(diff);
                            float outerLeft = ArenaData.OuterArenaBoundaryLeft.X;
                            float outerRight = ArenaData.OuterArenaBoundaryRight.X;
                            float currentX = NPC.Center.X;
                            if (chosenDashDirection < 0 && (currentX - outerLeft <= 20 * 16))
                                dashNearWall = true;
                            else if (chosenDashDirection > 0 && (outerRight - currentX <= 20 * 16))
                                dashNearWall = true;
                            else
                                dashNearWall = false;
                            rollDashPhase = 2;
                            StartAttackVFX();
                            rollDashTimer = 0;
                        }
                    }
                    break;

                case 2:
                    {
                        if (dashNearWall)
                        {
                            NPC.velocity = new Vector2(chosenDashDirection * 14f, 0f);
                            rollDashTimer++;
                            storedExtraSpin *= 0.95f;
                            float currentX = NPC.Center.X;
                            float outerLeft = ArenaData.OuterArenaBoundaryLeft.X;
                            float outerRight = ArenaData.OuterArenaBoundaryRight.X;
                            if ((chosenDashDirection < 0 && currentX - outerLeft <= 5 * 16) ||
                                (chosenDashDirection > 0 && outerRight - currentX <= 5 * 16))
                            {
                                SpawnStalactiteProjectiles();
                                rollDashPhase = 3;
                                rollDashTimer = 0;
                            }
                        }
                        else
                        {
                            if (rollDashTimer == 0)
                            {
                                if (NPC.velocity.Y == 0)
                                {
                                    float dashSpeed = 12f;
                                    float jumpHeight = -6f;
                                    NPC.velocity = new Vector2(chosenDashDirection * dashSpeed, jumpHeight);
                                }
                                else
                                {
                                    NPC.velocity = new Vector2(chosenDashDirection * 12f, NPC.velocity.Y);
                                }
                            }
                            else
                            {
                                NPC.velocity.X *= 0.95f;
                            }
                            storedExtraSpin *= 0.95f;
                            rollDashTimer++;
                            if (Math.Abs(NPC.velocity.X) < 0.5f)
                            {
                                Vector2 lightningSpawnPosition = new Vector2(NPC.Center.X, ArenaData.WaterLayer * 16);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                                    ModContent.ProjectileType<LightningStar>(), NPC.damage / 4, 0f, Main.myPlayer);
                                rollDashPhase = 3;
                                rollDashTimer = 0;
                            }
                        }
                    }
                    break;

                case 3:
                    {
                        OnAttackFinished();
                        rollDashPhase = 0;
                        rollDashTimer = 0;
                        storedExtraSpin = 0f;
                        dashNearWall = false;
                    }
                    break;
            }
        }

        private Vector2 rollSlamStartPos;
        private float initialRotation = 0f;
        private float targetRotation = 0f;
        private int damage = 10;
        private float rollSlamHorizontalDirection = 1f;

        private int mothSpawnTimer;
        private int mothSpawnInterval;
        public int activeMothCount = 0;
        private const int maxMoths = 3;

        private bool doLightning = false;
        private bool didFirstTelegraph = false;

        private Vector2 GetUpdatedSlamDirection()
        {
            Vector2 finalSlamPosition = GetFinalSlamPosition();
            return (finalSlamPosition - NPC.Center).SafeNormalize(Vector2.UnitY);
        }

        int descendTimer = 0;

        private Vector2 GetFinalSlamPosition()
        {
            descendTimer++;

            float descentMultiplier = (NPC.velocity.Y > 0) ? 10f : 1f;
            float dynamicY = ArenaData.ArenaCenter.Y - 500f;
            if (descendTimer > 20)
            {
                dynamicY += 200;
                //Main.NewText("Diabolical!");
            }
            return new Vector2(
                ArenaData.ArenaCenter.X,
                dynamicY
            );
        }




        private void RollToSideAndSlamAttack(Player player)
        {
            //Main.NewText("Roll to Side and Slam");
            switch (rollSlamPhase)
            {
                case 0:
                    {
                        float targetX = (NPC.Center.X < ArenaData.ArenaCenter.X)
                            ? ArenaData.InnerArenaBoundaryLeft.X
                            : ArenaData.InnerArenaBoundaryRight.X;
                        float distance = targetX - NPC.Center.X;
                        float desiredSpeed = 8f;
                        float accel = 0.2f;
                        if (!didFirstTelegraph)
                        {
                            Main.NewText("cringus");

                            float rollSlamHorizontalDirection1 = (NPC.Center.X < ArenaData.ArenaCenter.X) ? 1f : -1f;
                            int telegraphID = TelegraphUtility.DrawTelegraph(() => NPC.Center, 500f, GetUpdatedSlamDirection(), 1f, 10f, true, true, () => GetFinalSlamPosition(), Color.Blue, true, false, 160f);

                            didFirstTelegraph = true;
                        }




                        if (Math.Abs(distance) > 160f)
                        {
                            NPC.velocity.X += accel * Math.Sign(distance);
                            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -desiredSpeed, desiredSpeed);
                        }
                        else
                        {
                            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0f, 0.1f);
                            NPC.rotation += rollSlamHorizontalDirection / 3;
                            if (Math.Abs(NPC.velocity.X) < 0.2f)
                            {
                                rollSlamHorizontalDirection = (NPC.Center.X < ArenaData.ArenaCenter.X) ? 1f : -1f;
                                NPC.velocity.X = 0f;
                                NPC.noTileCollide = true;
                                rollSlamPhase = 1;
                                StartAttackVFX();
                                rollSlamTimer = 0;
                            }
                        }



                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 dustPos = new Vector2(
                                NPC.position.X + Main.rand.Next(NPC.width),
                                NPC.position.Y + NPC.height - 8
                            );
                            float dustSpeed = Main.rand.NextFloat(3f, 6f);
                            Vector2 dustVel = new Vector2(dashSideDirection > 0 ? -dustSpeed : dustSpeed, Main.rand.NextFloat(-1f, -0.2f));
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
                    break;

                case 1:
                    {
                        didFirstTelegraph = false;
                        descendTimer = 0;
                        if (rollSlamTimer == 0)
                        {
                            NPC.velocity.Y = -15f;
                        }
                        rollSlamTimer++;
                        NPC.rotation += rollSlamHorizontalDirection * 2;

                        if (NPC.velocity.Y > 0)
                        {
                            NPC.noGravity = true;
                            NPC.velocity.Y += 1.1f;

                            int checkX = (int)(NPC.Center.X / 16);
                            bool groundFound = false;
                            for (int offset = 0; offset <= 7; offset++)
                            {
                                int checkY = (int)(NPC.Bottom.Y / 16) + offset;
                                Tile tileBelow = Framing.GetTileSafely(checkX, checkY);

                                bool nearLeftBoundary = NPC.Center.X / 16 <= ArenaData.OuterArenaBoundaryLeft.X + 10;
                                bool nearRightBoundary = NPC.Center.X / 16 >= ArenaData.OuterArenaBoundaryRight.X - 10;

                                if (tileBelow.HasTile)
                                {
                                    if (tileBelow.TileType == ModContent.TileType<SmoothCavernStoneTile>() ||
                                        tileBelow.TileType == ModContent.TileType<CitadelBrickTile>() ||
                                        tileBelow.TileType == ModContent.TileType<ChargedStoneTile>())
                                    {
                                        groundFound = true;
                                        NPC.velocity.Y = 0;
                                        NPC.position.Y = checkY * 16 - NPC.height;
                                        NPC.noTileCollide = false;
                                        NPC.noGravity = false;
                                        break;
                                    }
                                    else if ((nearLeftBoundary || nearRightBoundary) &&
                                             tileBelow.TileType == ModContent.TileType<GlimmerwoodPlatformTile>())
                                    {
                                        groundFound = true;
                                        NPC.velocity.Y = 0;
                                        NPC.position.Y = checkY * 16 - NPC.height;
                                        NPC.noTileCollide = false;
                                        NPC.noGravity = false;
                                        break;
                                    }
                                }
                            }


                            if (groundFound)
                            {
                                TriggerBoundarySlamEffects();
                                if (rollSlamTimer > 60)
                                {
                                    rollSlamPhase = 2;
                                    rollSlamTimer = 0;
                                    rollSlamStartPos = NPC.Center;
                                    NPC.velocity.X = 0f;
                                }
                            }
                        }
                    }
                    break;

                case 2:
                    {
                        NPC.noTileCollide = true;

                        if (rollSlamTimer == 0)
                        {
                            NPC.velocity.Y = -20f;
                            initialRotation = NPC.rotation;
                            targetRotation = MathHelper.ToRadians(360);
                            float minInterval, maxInterval;
                            if (Main.masterMode)
                            {
                                minInterval = 0.8f;
                                maxInterval = 1.1f;
                            }
                            else if (Main.expertMode)
                            {
                                minInterval = 1f;
                                maxInterval = 1.2f;
                            }
                            else
                            {
                                minInterval = 1.5f;
                                maxInterval = 1.7f;
                            }
                            mothSpawnInterval = (int)(Main.rand.NextFloat(minInterval, maxInterval) * 60f);
                            mothSpawnTimer = 0;
                        }

                        float totalArcDuration = 60f;
                        float t = MathHelper.Clamp(rollSlamTimer / totalArcDuration, 0f, 1f);
                        Vector2 startPos = rollSlamStartPos;
                        Vector2 endPos = ArenaData.ArenaCenter - new Vector2(0, 64f);
                        Vector2 controlPoint = new((startPos.X + endPos.X) / 2, startPos.Y - 700f);
                        Vector2 bezierPos = (1 - t) * (1 - t) * startPos + 2 * (1 - t) * t * controlPoint + t * t * endPos;
                        NPC.Center = bezierPos;
                        NPC.rotation = MathHelper.Lerp(initialRotation, targetRotation, t);

                        rollSlamTimer++;

                        if (mothSpawnTimer >= mothSpawnInterval)
                        {
                            //SpawnMoth();
                            mothSpawnTimer = 0;
                        }
                        else
                        {
                            mothSpawnTimer++;
                        }

                        if (Math.Abs(ArenaData.ArenaCenter.X - NPC.Center.X) < 5f)
                        {
                            NPC.velocity.X = 0f;
                            rollSlamPhase = 3;
                            rollSlamTimer = 0;
                        }
                    }
                    break;


                case 3:
                    {
                        NPC.noGravity = true;
                        NPC.velocity.Y += 1.1f;
                        int checkX = (int)(NPC.Center.X / 16);
                        bool groundFound = false;
                        for (int offset = 0; offset <= 7; offset++)
                        {
                            int checkY = (int)(NPC.Bottom.Y / 16) + offset;
                            Tile tileBelow = Framing.GetTileSafely(checkX, checkY);
                            if (tileBelow.HasTile && (tileBelow.TileType == ModContent.TileType<SmoothCavernStoneTile>() || tileBelow.TileType == ModContent.TileType<CitadelBrickTile>() || tileBelow.TileType == ModContent.TileType<ChargedStoneTile>()))
                            {
                                groundFound = true;
                                NPC.velocity.Y = 0;
                                NPC.position.Y = checkY * 16 - NPC.height;
                                NPC.noTileCollide = false;
                                NPC.noGravity = false;

                                Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 30;
                                break;

                            }
                        }

                        if (groundFound)
                        {
                            Vector2 leftSpawnPosition = new Vector2(NPC.position.X - 40, NPC.position.Y + NPC.height - 20);
                            Vector2 rightSpawnPosition = new Vector2(NPC.position.X + NPC.width + 40, NPC.position.Y + NPC.height - 20);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), leftSpawnPosition, Vector2.Zero,
                                ModContent.ProjectileType<CrystalSpike>(), NPC.damage / 3, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), rightSpawnPosition, Vector2.Zero,
                                ModContent.ProjectileType<CrystalSpike>(), NPC.damage / 3, 0f, Main.myPlayer);
                            Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 30;
                            SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/CrystalSlam")
                            {
                                Volume = 0.85f,
                                Pitch = 0f,
                                PitchVariance = 0f,
                            };

                            SpawnOrbProjectiles();
                            SoundEngine.PlaySound(style, NPC.Center);
                            doWaterRocks = true;
                            OnAttackFinished();
                            //PerformRockThrow();
                            rollSlamPhase = 0;
                            rollSlamTimer = 0;
                        }
                        else
                        {
                            rollSlamTimer++;
                            if (rollSlamTimer > 30)
                            {
                                OnAttackFinished();
                                rollSlamPhase = 0;
                                rollSlamTimer = 0;
                            }
                        }
                    }
                    break;
            }
        }
        private void SpawnMoth()
        {
            //Main.NewText("Spawn Moth");

            int moth = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
                ModContent.NPCType<Charger>());
        }
        private void TriggerBoundarySlamEffects()
        {
            //Main.NewText("Trigger Boundary Slam");
            Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 30;
            SoundStyle style = new("AerovelenceMod/Sounds/Effects/CrystalSlam")
            {
                Volume = 0.85f,
                Pitch = 0f,
                PitchVariance = 0f,
            };
            SoundEngine.PlaySound(style, NPC.Center);
            SpawnStalactiteProjectiles();
        }


        private float preStunRotation = 0f;

        private int dashSideLaserInterval;

        private bool shouldPerformJump = false;
        private Vector2 bezierStart, bezierControl, bezierEnd;
        private float bezierProgress = 0f;

        private float dashTexScale = 0;

        private Vector2 GetBezierHighestPointDirection()
        {
            return new Vector2(dashSideDirection, -1).SafeNormalize(Vector2.UnitY) * 200f;
        }


        private void DashSideToSideSequence(Player player)
        {
            //Main.NewText("Dash side to side");
            switch (dashSidePhase)
            {
                case 0:
                    {
                        bool useLeftBoundary = (Math.Abs(NPC.Center.X - ArenaData.InnerArenaBoundaryLeft.X) <
                                                 Math.Abs(NPC.Center.X - ArenaData.InnerArenaBoundaryRight.X));
                        float targetX = useLeftBoundary ? ArenaData.InnerArenaBoundaryLeft.X : ArenaData.InnerArenaBoundaryRight.X;
                        dashSideDirection = useLeftBoundary ? 1f : -1f;

                        float distance = targetX - NPC.Center.X;
                        float desiredSpeed = 8f;
                        float accel = 0.2f;
                        float threshold = 10 * 16f;

                        if (Math.Abs(distance) > threshold)
                        {
                            NPC.velocity.X += accel * Math.Sign(distance);
                            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -desiredSpeed, desiredSpeed);
                        }
                        else
                        {
                            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0f, 0.1f);
                            if (Math.Abs(NPC.velocity.X) < 0.2f)
                            {
                                NPC.velocity.X = 0f;
                                dashSidePhase = 1;
                                dashSideTimer = 0;
                                storedExtraSpin = 0f;
                            }
                        }
                    }
                    break;

                case 1:
                    {
                        isDashing = true;
                        StartAttackVFX();
                        float targetSpinRate = 1.01f;
                        float spinIncrement = targetSpinRate / 300f;
                        storedExtraSpin = Math.Min(storedExtraSpin + spinIncrement, targetSpinRate);
                        SpawnOrbProjectiles();
                        NPC.rotation += dashSideDirection * storedExtraSpin;


                        if (dashSideTimer == 0)
                        {
                            Vector2 telegraphDirection = Vector2.Zero;
                            if (shouldPerformJump)
                            {
                                telegraphDirection = new Vector2(dashSideDirection, 0);
                                int telegraphID = TelegraphUtility.DrawTelegraph(() => NPC.Center, 500f, telegraphDirection, 1f, 0f, false, false, null, Color.Blue, true, false, 60f);
                            }
                            else if (!shouldPerformJump)
                            {
                                telegraphDirection = GetBezierHighestPointDirection();
                                int telegraphID = TelegraphUtility.DrawTelegraph(() => NPC.Center, 500f, telegraphDirection, 1f, 0f, false, false, null, Color.Blue, true, false, 60f);
                            }
                        }


                        dashSideTimer++;
                        if (dashSideTimer >= 60)
                        {
                            shouldPerformJump = !shouldPerformJump;
                            if (shouldPerformJump)
                            {
                                //Main.NewText("Switching to Phase 8");
                                dashSidePhase = 8;
                                SetupBezierJump();

                            }
                            else
                            {
                                //Main.NewText("Switching to Phase 2");
                                dashSidePhase = 2;
                                isDashing = false;
                            }
                            dashSideTimer = 0;
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 dustPos = new Vector2(
                                NPC.position.X + Main.rand.Next(NPC.width),
                                NPC.position.Y + NPC.height - 8
                            );
                            float dustSpeed = Main.rand.NextFloat(3f, 6f);
                            Vector2 dustVel = new Vector2(dashSideDirection > 0 ? -dustSpeed : dustSpeed, Main.rand.NextFloat(-1f, -0.2f));
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
                    break;

                case 2:
                    {
                        isDashing = true;
                        float dashSpeed = 16f;
                        NPC.velocity = new Vector2(dashSideDirection * dashSpeed, 0f);
                        dashSideTimer++;

                        float outerTargetX = (dashSideDirection > 0) ? ArenaData.OuterArenaBoundaryRight.X : ArenaData.OuterArenaBoundaryLeft.X;
                        if ((dashSideDirection > 0 && NPC.Center.X >= outerTargetX - 5 * 16f) ||
                            (dashSideDirection < 0 && NPC.Center.X <= outerTargetX + 5 * 16f))
                        {
                            NPC.velocity = Vector2.Zero;
                            preStunRotation = NPC.rotation;
                            dashSidePhase = 3;
                            dashSideTimer = 0;
                            isDashing = false;
                        }
                    }
                    break;
                case 3:
                    {
                        StopAttackVFX();
                        dashSideTimer++;
                        float rockAmplitude = MathHelper.Lerp(0.2f, 0f, dashSideTimer / 60f);
                        NPC.rotation = preStunRotation + (float)Math.Sin(dashSideTimer * 0.1f) * rockAmplitude;
                        if (dashSideTimer >= 60)
                        {
                            dashSidePhase = 4;
                            dashSideTimer = 0;
                        }
                        if (dashSideTimer == 1)
                        {
                            PerformWallSlam(NPC.Center, 15);
                        }
                    }
                    break;

                case 4:
                    {
                        float slowdownOffset = 10 * 16f;
                        bool useLeftBoundary = (Math.Abs(NPC.Center.X - ArenaData.InnerArenaBoundaryLeft.X) < Math.Abs(NPC.Center.X - ArenaData.InnerArenaBoundaryRight.X));
                        float targetInnerX = useLeftBoundary
                            ? ArenaData.InnerArenaBoundaryLeft.X + slowdownOffset
                            : ArenaData.InnerArenaBoundaryRight.X - slowdownOffset;

                        float diff = targetInnerX - NPC.Center.X;
                        float desiredSpeed = 8f;
                        float accel = 0.2f;
                        float threshold = 10 * 16f;
                        if (Math.Abs(diff) > threshold)
                        {
                            NPC.velocity.X += accel * Math.Sign(diff);
                            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -desiredSpeed, desiredSpeed);
                        }
                        else
                        {
                            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0f, 0.1f);
                            if (Math.Abs(NPC.velocity.X) < 0.2f)
                            {
                                NPC.velocity.X = 0f;
                                dashSidePhase = 5;
                                dashSideTimer = 0;
                                storedExtraSpin = 0f;
                            }
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 dustPos = new Vector2(
                                NPC.position.X + Main.rand.Next(NPC.width),
                                NPC.position.Y + NPC.height - 8
                            );
                            float dustSpeed = Main.rand.NextFloat(3f, 6f);
                            Vector2 dustVel = new Vector2(dashSideDirection > 0 ? -dustSpeed : dustSpeed, Main.rand.NextFloat(-1f, -0.2f));
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
                    break;
                case 5:
                    {
                        isDashing = true;
                        StartAttackVFX();
                        dashSideDirection = (NPC.Center.X > ArenaData.ArenaCenter.X) ? -1f : 1f;

                        float targetSpinRate = 1.01f;
                        float spinIncrement = targetSpinRate / 300f;
                        storedExtraSpin = Math.Min(storedExtraSpin + spinIncrement, targetSpinRate);
                        SpawnOrbProjectiles();
                        NPC.rotation += dashSideDirection * storedExtraSpin;

                        if (dashSideTimer <= 0)
                        {
                            int telegraphID = TelegraphUtility.DrawTelegraph(() => NPC.Center, 500f, new Vector2(dashSideDirection, 0), 1f, 0f, false, false, null, Color.Blue, true, false, 60f);
                        }

                        dashSideTimer++;
                        if (dashSideTimer >= 60)
                        {
                            dashSidePhase = 6;
                            dashSideTimer = 0;
                            isDashing = false;
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 dustPos = new Vector2(
                                NPC.position.X + Main.rand.Next(NPC.width),
                                NPC.position.Y + NPC.height - 8
                            );
                            float dustSpeed = Main.rand.NextFloat(3f, 6f);
                            Vector2 dustVel = new Vector2(dashSideDirection > 0 ? -dustSpeed : dustSpeed, Main.rand.NextFloat(-1f, -0.2f));
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
                    break;

                case 6:
                    {
                        isDashing = true;
                        dashSideTimer++;
                        NPC.noTileCollide = true;
                        NPC.noGravity = true;

                        if (dashSideTimer == 1)
                        {
                            float minInterval, maxInterval;
                            if (Main.masterMode)
                            {
                                minInterval = 0.8f;
                                maxInterval = 1.1f;
                            }
                            else if (Main.expertMode)
                            {
                                minInterval = 1f;
                                maxInterval = 1.2f;
                            }
                            else
                            {
                                minInterval = 1.5f;
                                maxInterval = 1.7f;
                            }
                            dashSideLaserInterval = (int)(Main.rand.NextFloat(minInterval, maxInterval) * 60f);
                        }
                        StartAttackVFX();
                        //KickRocks();
                        float finalDashSpeed = (dashSideIteration >= 1) ? 20f : 16f;
                        NPC.velocity = new Vector2(dashSideDirection * finalDashSpeed, 0f);

                        if (dashSideIteration >= 1 && dashSideTimer % dashSideLaserInterval == 0)
                        {
                            Vector2 laserSpawn = NPC.Center + new Vector2(0, -32);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), laserSpawn, Vector2.Zero,
                                ModContent.ProjectileType<Stalactite>(), NPC.damage / 2, 0f, Main.myPlayer);
                            //SpawnMoth();
                        }

                        float outerTarget = (dashSideDirection > 0)
                            ? ArenaData.OuterArenaBoundaryRight.X
                            : ArenaData.OuterArenaBoundaryLeft.X;
                        bool reachedBoundary = (dashSideDirection > 0 && NPC.Center.X >= outerTarget - 5 * 16f) ||
                                               (dashSideDirection < 0 && NPC.Center.X <= outerTarget + 5 * 16f);
                        if (reachedBoundary || dashSideTimer >= 120)
                        {
                            NPC.velocity = Vector2.Zero;
                            preStunRotation = NPC.rotation;
                            dashSidePhase = 7;
                            dashSideTimer = 0;
                            dashSideIteration++;
                            NPC.noTileCollide = false;
                            NPC.noGravity = false;
                        }
                    }
                    break;
                case 7:
                    {
                        StopAttackVFX();
                        isDashing = false;
                        dashSideTimer++;
                        float finalRockAmplitude = MathHelper.Lerp(0.3f, 0f, dashSideTimer / 120f);
                        NPC.rotation = preStunRotation + (float)Math.Sin(dashSideTimer * 0.1f) * finalRockAmplitude;
                        if (dashSideTimer >= 120)
                        {
                            dashSidePhase = 0;
                            dashSideTimer = 0;
                            dashSideIteration = 0;
                            NPC.velocity = Vector2.Zero;
                            OnAttackFinished();
                        }
                        if (dashSideTimer == 1)
                        {
                            PerformWallSlam(NPC.Center, 15);
                        }
                    }
                    break;
                case 8:
                    {
                        dashSideTimer++;
                        bezierProgress += 0.015f;
                        bezierProgress = MathHelper.Clamp(bezierProgress, 0f, 1f);
                        float npcBottomOffset = NPC.height / 2;
                        Vector2 adjustedEnd = bezierEnd - new Vector2(0, npcBottomOffset);
                        float jumpPeakHeight = Math.Abs(bezierStart.X - bezierEnd.X) * 0.4f + 100f;
                        Vector2 adjustedControl = new((bezierStart.X + adjustedEnd.X) / 2, bezierStart.Y - jumpPeakHeight);
                        NPC.Center = BezierCurve(bezierStart, adjustedControl, adjustedEnd, bezierProgress);

                        NPC.rotation += 1f;
                        if (dashSideTimer % 15 == 0)
                        {
                            Vector2 sparkSpawn = NPC.Center + new Vector2(Main.rand.NextFloat(-10, 10), 10);
                            Vector2 sparkVelocity = new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.2f, 0.4f));
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), sparkSpawn, sparkVelocity,
                                ModContent.ProjectileType<LightningStar>(), NPC.damage / 3, 0f, Main.myPlayer);
                        }
                        if (bezierProgress >= 1f)
                        {
                            StopAttackVFX();
                            dashSidePhase = 3;
                            dashSideTimer = 0;
                        }
                    }
                    break;
            }
        }

        private bool SpawnedOrbs = false;

        private void SpawnOrbProjectiles()
        {
            //Main.NewText("Spawn Orbs");
            if (!SpawnedOrbs)
            {
                int projectileCount = 10;
                float spawnRadius = 24f;
                float initialSpeed = 0.5f;

                for (int i = 0; i < projectileCount; i++)
                {
                    float angle = MathHelper.TwoPi * i / projectileCount;
                    Vector2 spawnOffset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * spawnRadius;
                    Vector2 spawnPosition = NPC.Center + spawnOffset;
                    Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * initialSpeed;

                    int proj = Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPosition, velocity,
                        ModContent.ProjectileType<LightningStar>(), NPC.damage / 2, 0f, Main.myPlayer);

                    Main.projectile[proj].timeLeft = 90;
                }
                SpawnedOrbs = true;
            }
        }

        private void SetupBezierJump()
        {
            Main.NewText("Start bezier");
            bezierStart = NPC.Center;
            bool isCloserToLeft = Math.Abs(NPC.Center.X - ArenaData.InnerArenaBoundaryLeft.X) <
                                  Math.Abs(NPC.Center.X - ArenaData.InnerArenaBoundaryRight.X);

            bezierEnd = isCloserToLeft ? ArenaData.InnerArenaBoundaryRight : ArenaData.InnerArenaBoundaryLeft;
            float peakHeight = 80f;
            bezierControl = new Vector2((bezierStart.X + bezierEnd.X) / 2, bezierStart.Y - peakHeight);
            bezierProgress = 0f;
            dashSideDirection = (bezierEnd.X > bezierStart.X) ? 1f : -1f;
        }


        private Vector2 BezierCurve(Vector2 start, Vector2 control, Vector2 end, float t)
        {
            float u = 1 - t;
            return (u * u) * start + (2 * u * t) * control + (t * t) * end;
        }


        private int dashOuterPhase = 0;
        private int dashOuterTimer = 0;
        private int dashOuterIteration = 0;
        private int maxDashIterations = 3;
        private float dashOuterDirection = 0f;
        private bool usePlayerDashThisTime = false;
        private bool dashVariantIsPlayerTargeted = false;

        private int dashPlayerPhase = 0;
        private int dashPlayerTimer = 0;
        private Vector2 playerDashTarget = Vector2.Zero;

        private void DashOuterToOuterSequence(Player player)
        {
            StartAttackVFX();
            //Main.NewText("Dash outer to outer (Original)");
            switch (dashOuterPhase)
            {
                case 0:
                    {
                        dashOuterDirection = (NPC.Center.X < ArenaData.ArenaCenter.X) ? 1f : -1f;
                        float targetSpinRate = 1.2f;
                        float spinIncrement = targetSpinRate / 300f;
                        storedExtraSpin = Math.Min(storedExtraSpin + spinIncrement, targetSpinRate);
                        SpawnOrbProjectiles();
                        NPC.rotation += dashOuterDirection * storedExtraSpin;
                        if (dashOuterTimer <= 0)
                        {
                            NPC.velocity.Y = -4f;
                            int telegraphID = TelegraphUtility.DrawTelegraph(() => NPC.Center, 500f, new Vector2(dashOuterDirection, 0), 1f, 0f, false, false, null, Color.Blue, true, false, 60f);
                        }

                        dashOuterTimer++;
                        if (dashOuterTimer >= 60)
                        {
                            preStunRotation = NPC.rotation;
                            dashOuterPhase = 1;
                            dashOuterTimer = 0;
                            storedExtraSpin = 0f;
                        }


                    }
                    break;

                case 1:
                    {
                        isDashing = true;
                        float dashSpeed = 20f;
                        if (dashOuterTimer % 10 == 0 && NPC.velocity.Y == 0)
                        {
                            NPC.velocity.Y -= Main.rand.NextFloat(3, 5);
                        }
                        NPC.velocity.X = dashOuterDirection * dashSpeed;
                        //KickRocks();
                        dashOuterTimer++;

                        float outerTarget = (dashOuterDirection > 0)
                            ? ArenaData.OuterArenaBoundaryRight.X
                            : ArenaData.OuterArenaBoundaryLeft.X;
                        bool reachedBoundary = (dashOuterDirection > 0 && NPC.Center.X >= outerTarget - 5 * 16f) ||
                                               (dashOuterDirection < 0 && NPC.Center.X <= outerTarget + 5 * 16f);
                        if (reachedBoundary || dashOuterTimer >= 120)
                        {
                            NPC.velocity = Vector2.Zero;
                            dashOuterPhase = 2;
                            dashOuterTimer = 0;
                            preStunRotation = NPC.rotation;
                            isDashing = false;
                        }
                    }
                    break;

                case 2:
                    {
                        dashOuterTimer++;
                        float rockAmplitude = MathHelper.Lerp(0.1f, 0f, dashOuterTimer / 30f);
                        NPC.rotation = preStunRotation + (float)Math.Sin(dashOuterTimer * 0.2f) * rockAmplitude;
                        if (dashOuterTimer >= 30)
                        {
                            dashOuterPhase = 3;
                            dashOuterTimer = 0;
                        }
                        if (dashOuterTimer == 1)
                        {
                            PerformWallSlam(NPC.Center, 15);
                        }
                        StopAttackVFX();
                    }
                    break;

                case 3:
                    {
                        doLightning = true;
                        dashOuterDirection = -dashOuterDirection;
                        dashOuterPhase = 0;
                        dashOuterTimer = 0;
                        dashOuterIteration++;
                        if (dashOuterIteration >= maxDashIterations)
                        {
                            dashOuterPhase = 0;
                            dashOuterTimer = 0;
                            dashOuterIteration = 0;
                            dashVariantIsPlayerTargeted = false;
                            OnAttackFinished();
                        }
                    }
                    break;
            }
        }

        private int doubleBouncePhase = 0;
        private int doubleBounceTimer = 0;
        private float doubleBounceDirection = 0f;
        private bool doubleBounceFirstSlam = false;
        private bool doubleBounceSecondSlam = false;
        private bool doubleBounceResetTileCollide = false;

        private int finalDoubleAttackTimer = 0;
        private float bounceAccelerationFactor = 0;
        private void DoubleBounceAttack(Player player)
        {
            Main.NewText(finalDoubleAttackTimer);
            switch (doubleBouncePhase)
            {
                case 0:
                    {
                        NPC.velocity *= 0.85f;

                        if (NPC.velocity.Length() < 0.5f)
                        {
                            doubleBounceDirection = Math.Sign(player.Center.X - NPC.Center.X);
                            NPC.noTileCollide = true;
                            doubleBouncePhase = 1;
                            doubleBounceTimer = 0;
                            StartAttackVFX();
                        }
                    }
                    break;

                case 1: //first jump
                    {
                        Main.NewText("1");
                        if (doubleBounceTimer == 0)
                        {
                            NPC.velocity.Y = -12f;
                            NPC.velocity.X = doubleBounceDirection * 2f;
                            int telegraphID = TelegraphUtility.DrawTelegraph(
                                () => NPC.Center,
                                300f,
                                new Vector2(doubleBounceDirection, 1f).SafeNormalize(Vector2.UnitY),
                                1f,
                                0f,
                                false,
                                false,
                                null,
                                Color.Blue,
                                true,
                                false,
                                45f
                            );
                        }

                        NPC.rotation += doubleBounceDirection * 0.15f;
                        if (NPC.velocity.Y > 0 && !doubleBounceFirstSlam)
                        {
                            NPC.velocity.Y *= 1.9f;
                            if (Main.GameUpdateCount % 2 == 0)
                            {
                                float dustScale = 0.8f;
                                for (int i = 0; i < 3; i++)
                                {
                                    Vector2 dustPos = NPC.Center + new Vector2(Main.rand.NextFloat(-NPC.width * 0.3f, NPC.width * 0.3f), Main.rand.NextFloat(-5f, 5f));

                                    int dust = Dust.NewDust(dustPos, 4, 4, DustID.Electric, 0, 0, 100, Color.Cyan, dustScale);
                                    Main.dust[dust].noGravity = true;
                                    Main.dust[dust].velocity = new Vector2(0, Main.rand.NextFloat(-1, 1));
                                }
                            }
                            if (Main.GameUpdateCount % 15 == 0)
                            {
                                //Main.NewText($"Velocity: {NPC.velocity.Y:F2}, Factor: {bounceAccelerationFactor:F2}");
                            }
                            if (NPC.position.Y + NPC.height >= ArenaData.ArenaCenter.Y)
                            {
                                
                                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/CrystalSlam")
                                {
                                    Volume = 0.75f,
                                    Pitch = 0.2f,
                                    PitchVariance = 0.1f,
                                };
                                NPC.noGravity = false;
                                NPC.noTileCollide = false;
                                SoundEngine.PlaySound(style, NPC.Center);
                                Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 15;
                                Vector2 lightningStarPos = NPC.Center + new Vector2(doubleBounceDirection * 60f, 0);
                                Projectile.NewProjectile(
                                    NPC.GetSource_FromThis(),
                                    lightningStarPos,
                                    Vector2.Zero,
                                    ModContent.ProjectileType<LightningStar>(),
                                    NPC.damage / 3,
                                    0f,
                                    Main.myPlayer
                                );
                                doubleBounceDirection = Math.Sign(player.Center.X - NPC.Center.X);
                                doubleBouncePhase = 2;
                                doubleBounceTimer = 0;
                                doubleBounceFirstSlam = true;
                            }
                        }
                        else
                        {
                            bounceAccelerationFactor = 0f;
                        }

                        doubleBounceTimer++;
                    }
                    break;

                case 2: //second jump
                    {
                        //Main.NewText($"Phase 2 - Timer: {doubleBounceTimer}, Velocity: {NPC.velocity}");

                        if (doubleBounceTimer == 0)
                        {
                            NPC.velocity = Vector2.Zero;
                        }
                        if (doubleBounceTimer == 3)
                        {
                            NPC.noTileCollide = true;
                            NPC.velocity.Y = -10f;
                            NPC.velocity.X = doubleBounceDirection * 2f;
                            doubleBounceResetTileCollide = false;

                            int telegraphID = TelegraphUtility.DrawTelegraph(
                                () => NPC.Center,
                                300f,
                                new Vector2(doubleBounceDirection, 1f).SafeNormalize(Vector2.UnitY),
                                1f,
                                0f,
                                false,
                                false,
                                null,
                                Color.Cyan,
                                true,
                                false,
                                45f
                            );
                        }

                        NPC.rotation += doubleBounceDirection * 0.2f;
                        if (doubleBounceTimer >= 10 && NPC.velocity.Y > 0 && !doubleBounceSecondSlam)
                        {
                            Main.NewText("Greg");
                            NPC.velocity.Y *= 1.9f;
                            if (NPC.position.Y + NPC.height >= ArenaData.ArenaCenter.Y)
                            {
                                Main.NewText("Greg tortugle");
                                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/CrystalSlam")
                                {
                                    Volume = 0.9f,
                                    Pitch = 0f,
                                    PitchVariance = 0.1f,
                                };
                                SoundEngine.PlaySound(style, NPC.Center);
                                Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 25;
                                NPC.noTileCollide = false;
                                int starCount = 3;
                                for (int i = 0; i < starCount; i++)
                                {
                                    float angle = MathHelper.TwoPi * i / starCount;
                                    Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 50f;
                                    Vector2 starPos = NPC.Center + offset;

                                    Projectile.NewProjectile(
                                        NPC.GetSource_FromThis(),
                                        starPos,
                                        Vector2.Zero,
                                        ModContent.ProjectileType<LightningStar>(),
                                        NPC.damage / 3,
                                        0f,
                                        Main.myPlayer
                                    );
                                }

                                doubleBouncePhase = 3;
                                doubleBounceTimer = 0;
                                doubleBounceSecondSlam = true;
                            }
                        }

                        doubleBounceTimer++;
                    }
                    break;




                case 3: //cleanup
                    {
                        NPC.position.Y -= 2;
                        Main.NewText("3");
                        finalDoubleAttackTimer++;
                        StopAttackVFX();
                        
                        doubleBounceFirstSlam = false;
                        doubleBounceSecondSlam = false;

                        doubleBounceResetTileCollide = false;
                        NPC.noTileCollide = false;
                        NPC.velocity *= 0.97f;
                        if (finalDoubleAttackTimer >= 60)
                        {
                            doubleBouncePhase = 0;
                            doubleBounceTimer = 0;
                            Main.NewText("3");

                            OnAttackFinished();
                            finalDoubleAttackTimer = 0;
                        }
                    }
                    break;
            }
        }

        private int crystalDashPhase = 0;
        private int crystalDashTimer = 0;
        private bool leftCrystalDestroyed = false;
        private bool rightCrystalDestroyed = false;
        private int leftCrystalId = -1;
        private int rightCrystalId = -1;
        private Vector2 leftCrystalPosition;
        private Vector2 rightCrystalPosition;
        private float crystalDashDirection = 0f;
        private bool readyToDestroyCrystals = false;
        private bool isSpinningUp = false;
        private float crystalRotationSpeed = 0f;
        private float crystalDashSpeed = 0f;

        private void CrystalDashAttack(Player player)
        {
            switch (crystalDashPhase)
            {
                case 0: //initial rolling and crystal spawning
                    {
                        if (crystalDashTimer == 0)
                        {
                            Main.NewText("Crystal Dash: Initializing");
                            leftCrystalPosition = new Vector2(
                                ArenaData.OuterArenaBoundaryLeft.X + 150,
                                ArenaData.ArenaCenter.Y + 158);

                            rightCrystalPosition = new Vector2(
                                ArenaData.OuterArenaBoundaryRight.X - 150,
                                ArenaData.ArenaCenter.Y + 158);

                            leftCrystalId = Projectile.NewProjectile(
                                NPC.GetSource_FromThis(),
                                leftCrystalPosition,
                                Vector2.Zero,
                                ModContent.ProjectileType<CrystalCastle>(),
                                0,
                                0f,
                                Main.myPlayer);

                            rightCrystalId = Projectile.NewProjectile(
                                NPC.GetSource_FromThis(),
                                rightCrystalPosition,
                                Vector2.Zero,
                                ModContent.ProjectileType<CrystalCastle>(),
                                0,
                                0f,
                                Main.myPlayer);
                            leftCrystalDestroyed = false;
                            rightCrystalDestroyed = false;
                            readyToDestroyCrystals = false;
                        }

                        Vector2 directionToPlayer = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                        float healthFactor = 1f - (NPC.life / (float)NPC.lifeMax);
                        float desiredSpeed = MathHelper.Lerp(3f, 6f, healthFactor);
                        float acceleration = 0.1f;
                        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, directionToPlayer.X * desiredSpeed, acceleration);

                        if (crystalDashTimer >= 120)
                        {
                            Main.NewText("Crystal Dash: Crystals fully grown");
                            crystalDashPhase = 1;
                            crystalDashTimer = 0;
                            storedExtraSpin = 0f;
                            NPC.velocity = Vector2.Zero;
                            bool attackLeftFirst = Math.Abs(NPC.Center.X - leftCrystalPosition.X) >
                                                 Math.Abs(NPC.Center.X - rightCrystalPosition.X);

                            crystalDashDirection = attackLeftFirst ? -1f : 1f;
                        }

                        crystalDashTimer++;
                    }
                    break;

                case 1: //spin up and prepare for dash to first crystal
                    {
                        if (crystalDashTimer == 0)
                        {
                            isSpinningUp = true;
                            StartAttackVFX();
                        }
                        //crystalRotationSpeed = Math.Min(crystalRotationSpeed + 0.020f, 1.0f);
                        float targetSpinRate = 1.2f;
                        float spinIncrement = targetSpinRate / 300f;
                        storedExtraSpin = Math.Min(storedExtraSpin + spinIncrement, targetSpinRate);
                        NPC.rotation += crystalDashDirection * storedExtraSpin;
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 dustPos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2);
                            int dust = Dust.NewDust(dustPos, 4, 4, DustID.Electric, 0f, 0f, 100, Color.Cyan, 1.5f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].velocity = Vector2.Zero;
                        }

                        if (crystalDashTimer == 60)
                        {
                            Vector2 targetCrystal = GetCurrentCrystalPosition(crystalDashDirection < 0);

                            int telegraphID = TelegraphUtility.DrawTelegraph(
                                () => NPC.Center,
                                500f,
                                new Vector2(targetCrystal.X - NPC.Center.X, 0).SafeNormalize(Vector2.UnitX),
                                1f,
                                0f,
                                false,
                                false,
                                null,
                                Color.DeepSkyBlue,
                                true,
                                false,
                                30f
                            );
                        }

                        if (crystalDashTimer >= 90)
                        {
                            crystalDashPhase = 2;
                            crystalDashTimer = 0;
                            crystalDashSpeed = 0f;
                            isDashing = true;
                            Main.NewText("Crystal Dash: Dashing to first crystal");
                        }

                        crystalDashTimer++;
                    }
                    break;

                case 2: //dash to first crystal
                    {
                        Vector2 targetCrystal = GetCurrentCrystalPosition(crystalDashDirection < 0);
                        Vector2 dashDir = new Vector2(targetCrystal.X - NPC.Center.X, 0).SafeNormalize(Vector2.UnitX);
                        crystalDashSpeed = Math.Min(crystalDashSpeed + 0.5f, 25f);
                        NPC.velocity = dashDir * 15;
                        float radius = NPC.width / 2f;
                        float rotationPer = NPC.velocity.X / radius;



                        float healthFactor = 1f - (NPC.life / (float)NPC.lifeMax);
                        //NPC.rotation += rotationPer * (1f + healthFactor * 0.5f);

                        float distToTarget = Math.Abs(NPC.Center.X - targetCrystal.X);
                        if (distToTarget < 50f)
                        {
                            //hit the crystal!
                            Main.NewText("Crystal Dash: First crystal destroyed");
                            NPC.velocity = Vector2.Zero;
                            SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/CrystalSlam")
                            {
                                Volume = 0.9f,
                                Pitch = -0.2f,
                                PitchVariance = 0.1f,
                            };
                            SoundEngine.PlaySound(style, NPC.Center);
                            Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 20;

                            //"destroy" the crystal
                            if (crystalDashDirection < 0)
                            {
                                leftCrystalDestroyed = true;
                                if (leftCrystalId >= 0 && leftCrystalId < Main.maxProjectiles && Main.projectile[leftCrystalId].active)
                                {
                                    //this would trigger the crystal explosion
                                    Main.projectile[leftCrystalId].Kill();
                                    leftCrystalId = -1;

                                    //spawn crystal shards effect
                                    for (int i = 0; i < 20; i++)
                                    {
                                        Vector2 velocity = Main.rand.NextVector2CircularEdge(5f, 5f);
                                        int dust = Dust.NewDust(targetCrystal, 10, 10, DustID.BlueCrystalShard, velocity.X, velocity.Y, 100, default, 1.5f);
                                        Main.dust[dust].noGravity = true;
                                    }
                                }
                            }
                            else
                            {
                                rightCrystalDestroyed = true;
                                if (rightCrystalId >= 0 && rightCrystalId < Main.maxProjectiles && Main.projectile[rightCrystalId].active)
                                {
                                    Main.projectile[rightCrystalId].Kill();
                                    rightCrystalId = -1;

                                    //spawn crystal shards effect
                                    for (int i = 0; i < 20; i++)
                                    {
                                        Vector2 velocity = Main.rand.NextVector2CircularEdge(5f, 5f);
                                        int dust = Dust.NewDust(targetCrystal, 10, 10, DustID.BlueCrystalShard, velocity.X, velocity.Y, 100, default, 1.5f);
                                        Main.dust[dust].noGravity = true;
                                    }
                                }
                            }
                            crystalDashPhase = 3;
                            crystalDashTimer = 0;
                        }

                        crystalDashTimer++;
                    }
                    break;

                case 3: //short recovery after first crystal impact
                    {
                        isDashing = false;

                        if (crystalDashTimer == 0)
                        {
                            crystalRotationSpeed = 0.2f;
                        }
                        crystalRotationSpeed = Math.Max(crystalRotationSpeed - 0.01f, 0.1f);
                        NPC.rotation += crystalDashDirection * crystalRotationSpeed;

                        if (crystalDashTimer >= 10)
                        {
                            crystalDashPhase = 4;
                            crystalDashTimer = 0;
                            Vector2 directionToWall = new(crystalDashDirection, 0);
                            NPC.velocity = directionToWall * 8f;
                            Main.NewText("moving to wall");
                        }

                        crystalDashTimer++;
                    }
                    break;

                case 4: //move until hitting wall
                    {
                        float wallPosition = (crystalDashDirection > 0)
                            ? ArenaData.OuterArenaBoundaryRight.X - 80
                            : ArenaData.OuterArenaBoundaryLeft.X + 80;

                        bool reachedWall = (crystalDashDirection > 0 && NPC.Center.X >= wallPosition) ||
                                           (crystalDashDirection < 0 && NPC.Center.X <= wallPosition);

                        if (reachedWall)
                        {
                            Main.NewText("reached wall, preparing second dash");
                            crystalDashPhase = 5;
                            crystalDashTimer = 0;
                            NPC.velocity = Vector2.Zero;
                            crystalDashDirection = -crystalDashDirection;
                        }

                        crystalDashTimer++;
                    }
                    break;

                case 5: //spin up and preparation for second crystal dash
                    {
                        if (crystalDashTimer == 0)
                        {
                            isSpinningUp = true;
                            StartAttackVFX();
                        }
                        //crystalRotationSpeed = Math.Min(crystalRotationSpeed + 0.02f, 1.0f);
                        //NPC.rotation += crystalDashDirection * crystalRotationSpeed;
                        float targetSpinRate = 1.2f;
                        float spinIncrement = targetSpinRate / 300f;
                        storedExtraSpin = Math.Min(storedExtraSpin + spinIncrement, targetSpinRate);
                        NPC.rotation += crystalDashDirection * storedExtraSpin;
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 dustPos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2);
                            int dust = Dust.NewDust(dustPos, 4, 4, DustID.Electric, 0f, 0f, 100, Color.Cyan, 1.5f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].velocity = Vector2.Zero;
                        }

                        if (crystalDashTimer == 60)
                        {
                            Vector2 targetCrystal = GetCurrentCrystalPosition(crystalDashDirection < 0);

                            int telegraphID = TelegraphUtility.DrawTelegraph(
                                () => NPC.Center,
                                500f,
                                new Vector2(targetCrystal.X - NPC.Center.X, 0).SafeNormalize(Vector2.UnitX),
                                1f,
                                0f,
                                false,
                                false,
                                null,
                                Color.DeepSkyBlue,
                                true,
                                false,
                                30f
                            );
                        }

                        if (crystalDashTimer >= 90)
                        {
                            crystalDashPhase = 6;
                            crystalDashTimer = 0;
                            crystalDashSpeed = 0f;
                            storedExtraSpin = 0;
                            isDashing = true;
                            Main.NewText("dashing to second crystal");
                        }

                        crystalDashTimer++;
                    }
                    break;

                case 6: //dash to second crystal
                    {
                        Vector2 targetCrystal = GetCurrentCrystalPosition(crystalDashDirection < 0);
                        Vector2 dashDir = new Vector2(targetCrystal.X - NPC.Center.X, 0).SafeNormalize(Vector2.UnitX);
                        crystalDashSpeed = Math.Min(crystalDashSpeed + 0.5f, 25f);
                        NPC.velocity = dashDir * 15;
                        float radius = NPC.width / 2f;
                        float rotationPer = NPC.velocity.X / radius;



                        float healthFactor = 1f - (NPC.life / (float)NPC.lifeMax);
                        //NPC.rotation += rotationPer * (1f + healthFactor * 0.5f);
                        float distToTarget = Math.Abs(NPC.Center.X - targetCrystal.X);
                        if (distToTarget < 50f)
                        {
                            //hit the crystal
                            Main.NewText("Second crystal destroyed");
                            NPC.velocity = Vector2.Zero;
                            SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/CrystalSlam")
                            {
                                Volume = 0.9f,
                                Pitch = -0.2f,
                                PitchVariance = 0.1f,
                            };
                            SoundEngine.PlaySound(style, NPC.Center);
                            Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 20;

                            if (crystalDashDirection < 0)
                            {
                                leftCrystalDestroyed = true;
                                if (leftCrystalId >= 0 && leftCrystalId < Main.maxProjectiles && Main.projectile[leftCrystalId].active)
                                {
                                    Main.projectile[leftCrystalId].Kill();
                                    leftCrystalId = -1;
                                    for (int i = 0; i < 20; i++)
                                    {
                                        Vector2 velocity = Main.rand.NextVector2CircularEdge(5f, 5f);
                                        int dust = Dust.NewDust(targetCrystal, 10, 10, DustID.BlueCrystalShard, velocity.X, velocity.Y, 100, default, 1.5f);
                                        Main.dust[dust].noGravity = true;
                                    }
                                }
                            }
                            else
                            {
                                rightCrystalDestroyed = true;
                                if (rightCrystalId >= 0 && rightCrystalId < Main.maxProjectiles && Main.projectile[rightCrystalId].active)
                                {
                                    Main.projectile[rightCrystalId].Kill();
                                    rightCrystalId = -1;
                                    for (int i = 0; i < 20; i++)
                                    {
                                        Vector2 velocity = Main.rand.NextVector2CircularEdge(5f, 5f);
                                        int dust = Dust.NewDust(targetCrystal, 10, 10, DustID.BlueCrystalShard, velocity.X, velocity.Y, 100, default, 1.5f);
                                        Main.dust[dust].noGravity = true;
                                    }
                                }

                                for (int j = 0; j < (6 + Main.rand.Next(0, 2)) * 25; j++)
                                {
                                    Dust star = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowPixelCross>(),
                                    Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1.5f, 3.25f), newColor: new Color(255, 180, 60), Scale: Main.rand.NextFloat(0.35f, 0.5f) * 1f);

                                    star.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                                                    rotPower: 0.15f, preSlowPower: 0.91f, timeBeforeSlow: 15, postSlowPower: 0.90f, velToBeginShrink: 2f, fadePower: 0.93f, shouldFadeColor: false);
                                }
                                if (1 > 0f)
                                {
                                    SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_death_1") with { Pitch = .46f, PitchVariance = .12f, MaxInstances = -1, Volume = 0.5f * 1 };
                                    SoundEngine.PlaySound(style, NPC.Center);
                                }
                                for (int j = 0; j < (6 + Main.rand.Next(0, 2)) * 25; j++)
                                {
                                    Dust star = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowPixelCross>(),
                                    Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(2f, 4f), newColor: new Color(100, 100, 255), Scale: Main.rand.NextFloat(0.4f, 0.6f) * 1f);

                                    star.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                                                    rotPower: 0.2f, preSlowPower: 0.93f, timeBeforeSlow: 12, postSlowPower: 0.91f, velToBeginShrink: 2.2f, fadePower: 0.94f, shouldFadeColor: false);
                                }
                                if (1 > 0f)
                                {
                                    //SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_lightning_aura_zap") with { Pitch = .2f, PitchVariance = .15f, MaxInstances = -1, Volume = 0.4f * 1 };
                                   // SoundEngine.PlaySound(style2, NPC.Center);
                                }

                            }

                            if (leftCrystalDestroyed && rightCrystalDestroyed)
                            {
                                crystalDashPhase = 7;
                                crystalDashTimer = 0;
                            }
                        }

                        crystalDashTimer++;
                    }
                    break;

                case 7: //short recovery after second crystal impact
                    {
                        isDashing = false;

                        if (crystalDashTimer == 0)
                        {
                            crystalRotationSpeed = 0.2f;
                        }

                        //crystalRotationSpeed = Math.Max(crystalRotationSpeed - 0.01f, 0.1f);
                        //NPC.rotation += crystalDashDirection * crystalRotationSpeed;
                        float targetSpinRate = 1.2f;
                        float spinIncrement = targetSpinRate / 300f;
                        storedExtraSpin = Math.Min(storedExtraSpin + spinIncrement, targetSpinRate);
                        NPC.rotation += crystalDashDirection * storedExtraSpin;

                        if (crystalDashTimer >= 10)
                        {
                            crystalDashPhase = 8;
                            crystalDashTimer = 0;
                            storedExtraSpin = 0;
                            Vector2 directionToWall = new(crystalDashDirection, 0);
                            NPC.velocity = directionToWall * 8f;
                            Main.NewText("Moving to wall after second crystal impact");
                        }

                        crystalDashTimer++;
                    }
                    break;

                case 8: //move to boundary
                    {
                        float wallPosition = (crystalDashDirection > 0)
                            ? ArenaData.OuterArenaBoundaryRight.X - 80
                            : ArenaData.OuterArenaBoundaryLeft.X + 80;

                        bool reachedWall = (crystalDashDirection > 0 && NPC.Center.X >= wallPosition) ||
                                           (crystalDashDirection < 0 && NPC.Center.X <= wallPosition);

                        if (reachedWall)
                        {
                            Main.NewText("Reached wall, boss stunned");
                            preStunRotation = NPC.rotation;
                            crystalDashPhase = 9;
                            crystalDashTimer = 0;
                            NPC.velocity = Vector2.Zero;
                        }

                        crystalDashTimer++;
                    }
                    break;

                case 9: //stun and rock back and forth
                    {
                        float rockAmplitude = MathHelper.Lerp(0.2f, 0f, crystalDashTimer / 60f);
                        NPC.rotation = preStunRotation + (float)Math.Sin(crystalDashTimer * 0.1f) * rockAmplitude;

                        if (crystalDashTimer >= 60)
                        {
                            Main.NewText("Boss recovering from stun");
                            crystalDashPhase = 10;
                            crystalDashTimer = 0;
                        }

                        crystalDashTimer++;
                    }
                    break;

                case 10: //recovery phase before moving to the next attack
                    {
                        StopAttackVFX();
                        isSpinningUp = false;
                        crystalDashPhase = 0;
                        crystalDashTimer = 0;
                        leftCrystalDestroyed = false;
                        rightCrystalDestroyed = false;
                        leftCrystalId = -1;
                        rightCrystalId = -1;
                        crystalRotationSpeed = 0f;
                        OnAttackFinished();
                    }
                    break;
            }
        }

        private Vector2 GetCurrentCrystalPosition(bool leftCrystal)
        {
            int crystalId = leftCrystal ? leftCrystalId : rightCrystalId;
            Vector2 storedPosition = leftCrystal ? leftCrystalPosition : rightCrystalPosition;
            if (crystalId >= 0 && crystalId < Main.maxProjectiles && Main.projectile[crystalId].active)
                return Main.projectile[crystalId].Center;
            return storedPosition;
        }

        private int doubleDashPhase = 0;
        private int doubleDashTimer = 0;
        private float doubleDashDirection = 0f;
        private float initialSpeed = 0f;

        private void DoubleDashAttack(Player player)
        {
            switch (doubleDashPhase)
            {
                case 0: //deceleration
                    {
                        if (doubleDashTimer == 0)
                        {
                            initialSpeed = NPC.velocity.Length();
                        }
                        NPC.velocity *= 0.9f;
                        if (doubleDashTimer % 2 == 0)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 dustPos = NPC.Center + new Vector2(-Math.Sign(NPC.velocity.X) * Main.rand.NextFloat(10f, 20f),
                                                                      Main.rand.NextFloat(-10f, 10f));
                                int dust = Dust.NewDust(dustPos, 4, 4, DustID.Electric, 0f, 0f, 100, Color.Cyan, 1f);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].velocity = Vector2.Zero;
                            }
                        }
                        if (NPC.velocity.Length() < 0.5f || doubleDashTimer >= 30)
                        {
                            doubleDashPhase = 1;
                            doubleDashTimer = 0;
                            doubleDashDirection = Math.Sign(player.Center.X - NPC.Center.X);
                            int telegraphID = TelegraphUtility.DrawTelegraph(
                                () => NPC.Center,
                                300f,
                                new Vector2(doubleDashDirection, 0),
                                1f,
                                0f,
                                false,
                                false,
                                null,
                                Color.Blue,
                                true,
                                false,
                                20f
                            );
                        }

                        doubleDashTimer++;
                    }
                    break;

                case 1: //first dash toward player
                    {
                        if (doubleDashTimer == 0)
                        {
                            StartAttackVFX();
                            isDashing = true;
                            float dashSpeed = 15f;
                            NPC.velocity = new Vector2(doubleDashDirection * dashSpeed, 0);
                        }
                        NPC.rotation += doubleDashDirection * 0.2f;
                        if (doubleDashTimer >= 30)
                        {
                            doubleDashPhase = 2;
                            doubleDashTimer = 0;
                            isDashing = false;
                        }

                        doubleDashTimer++;
                    }
                    break;

                case 2: //second deceleration
                    {
                        NPC.velocity *= 0.85f;
                        NPC.rotation += doubleDashDirection * 0.1f;
                        if (doubleDashTimer % 2 == 0)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 dustPos = NPC.Center + new Vector2(-Math.Sign(NPC.velocity.X) * Main.rand.NextFloat(10f, 20f),
                                                                      Main.rand.NextFloat(-10f, 10f));
                                int dust = Dust.NewDust(dustPos, 4, 4, DustID.Electric, 0f, 0f, 100, Color.Cyan, 1f);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].velocity = Vector2.Zero;
                            }
                        }
                        if (doubleDashTimer == 10)
                        {
                            doubleDashDirection = Math.Sign(player.Center.X - NPC.Center.X);
                            int telegraphID = TelegraphUtility.DrawTelegraph(
                                () => NPC.Center,
                                300f,
                                new Vector2(doubleDashDirection, 0),
                                1f,
                                0f,
                                false,
                                false,
                                null,
                                Color.DeepSkyBlue,
                                true,
                                false,
                                15f
                            );
                        }
                        if (NPC.velocity.Length() < 0.3f || doubleDashTimer >= 20)
                        {
                            doubleDashPhase = 3;
                            doubleDashTimer = 0;
                        }

                        doubleDashTimer++;
                    }
                    break;

                case 3: //second quicker dash
                    {
                        if (doubleDashTimer == 0)
                        {
                            StartAttackVFX();
                            isDashing = true;
                            float dashSpeed = 20f;
                            NPC.velocity = new Vector2(doubleDashDirection * dashSpeed, 0);
                        }
                        NPC.rotation += doubleDashDirection * 0.3f;
                        if (doubleDashTimer >= 25)
                        {
                            doubleDashPhase = 4;
                            doubleDashTimer = 0;
                            isDashing = false;
                        }

                        doubleDashTimer++;
                    }
                    break;

                case 4: //final deceleration and spawn lightning TODO not lightning!!!
                    {
                        NPC.velocity *= 0.9f;
                        NPC.rotation += doubleDashDirection * 0.05f;

                        if (doubleDashTimer == 10)
                        {
                            Vector2 lightningPos = NPC.Center + new Vector2(doubleDashDirection * 12, 0);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), lightningPos, Vector2.Zero, ModContent.ProjectileType<CrystalCastle>(), NPC.damage / 3, 0f, Main.myPlayer);

                            /*SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/CrystalChime")
                            {
                                Volume = 0.75f,
                                Pitch = 0.2f,
                                PitchVariance = 0.1f,
                            };
                            SoundEngine.PlaySound(style, NPC.Center);*/
                        }
                        if (doubleDashTimer >= 30)
                        {
                            doubleDashPhase = 0;
                            doubleDashTimer = 0;
                            StopAttackVFX();
                            OnAttackFinished();
                        }

                        doubleDashTimer++;
                    }
                    break;
            }
        }

        private int dashPhase = 0;
        private int dashTimer = 0;
        private int dashDir = 0;
        private bool dashActive = false;

        private Vector2 dashStartPos;
        private Vector2 dashTargetPos;
        private float initialRotationS;
        private float savedRotation;
        private int stunTimer;

        private void SingleDashAttack(Player player)
        {
            float healthFactor = (float)NPC.life / NPC.lifeMax;

            switch (dashPhase)
            {

                    case 0:
                    {
                        dashOuterDirection = (NPC.Center.X < ArenaData.ArenaCenter.X) ? 1f : -1f;
                        float targetSpinRate = 1.2f;
                        float spinIncrement = targetSpinRate / 300f;
                        storedExtraSpin = Math.Min(storedExtraSpin + spinIncrement, targetSpinRate);
                        SpawnOrbProjectiles();
                        NPC.rotation += dashOuterDirection * storedExtraSpin;
                        if (dashOuterTimer <= 0)
                        {
                            NPC.velocity.Y = -4f;
                            int telegraphID = TelegraphUtility.DrawTelegraph(() => NPC.Center, 500f, new Vector2(dashOuterDirection, 0), 1f, 0f, false, false, null, Color.Blue, true, false, 60f);
                        }

                        dashOuterTimer++;
                        if (dashOuterTimer >= 60)
                        {
                            preStunRotation = NPC.rotation;
                            dashPhase = 1;
                            dashOuterTimer = 0;
                            storedExtraSpin = 0f;
                        }
                    }
                    break;

                case 1:
                    {
                        isDashing = true;
                        float dashSpeed = 20f;
                        if (dashOuterTimer % 10 == 0 && NPC.velocity.Y == 0)
                        {
                            NPC.velocity.Y -= Main.rand.NextFloat(3, 5);
                        }
                        NPC.velocity.X = dashOuterDirection * dashSpeed;
                        //KickRocks();
                        dashOuterTimer++;

                        float outerTarget = (dashOuterDirection > 0)
                            ? ArenaData.OuterArenaBoundaryRight.X
                            : ArenaData.OuterArenaBoundaryLeft.X;
                        bool reachedBoundary = (dashOuterDirection > 0 && NPC.Center.X >= outerTarget - 5 * 16f) ||
                                               (dashOuterDirection < 0 && NPC.Center.X <= outerTarget + 5 * 16f);
                        if (reachedBoundary || dashOuterTimer >= 120)
                        {
                            NPC.velocity = Vector2.Zero;
                            dashPhase = 2;
                            dashOuterTimer = 0;
                            preStunRotation = NPC.rotation;
                            isDashing = false;
                        }
                    }
                    break;

                case 2:
                    {
                        dashOuterTimer++;
                        float rockAmplitude = MathHelper.Lerp(0.1f, 0f, dashOuterTimer / 30f);
                        NPC.rotation = preStunRotation + (float)Math.Sin(dashOuterTimer * 0.2f) * rockAmplitude;
                        if (dashOuterTimer >= 30)
                        {
                            SelectNextAttack();
                            dashOuterTimer = 0;
                        }
                        if (dashOuterTimer == 1)
                        {
                            PerformWallSlam(NPC.Center, 15);
                        }
                        StopAttackVFX();
                    }
                    break;
            }
        }

        private int kickRocksTimer = 0;

        private void KickRocks()
        {
            kickRocksTimer++;
            if (kickRocksTimer >= 10)
            {
                kickRocksTimer = 0;
                Vector2 spawnPos = NPC.Bottom;
                Vector2 kickDirection = new Vector2(-dashSideDirection * 0.5f, -1f);
                kickDirection.Normalize();
                float kickSpeed = Main.rand.Next(10, 12);
                Vector2 velocity = kickDirection * kickSpeed;
                Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, velocity,
                    ModContent.ProjectileType<RockShard>(), NPC.damage / 2, 0f, Main.myPlayer);
            }
        }

        private int lastDisabledPlatformIndex = -1;
        private bool lastDisabledSide = false;

        private void DisableRandomPlatform()
        {
            if (isPlatformDisableActive)
                return;

            isPlatformDisableActive = true;
            platformDisableState = 0;
            platformDisableTimer = 0;

            int[] leftPlatforms = { 1, 2, 3 };
            int[] rightPlatforms = { 4, 5, 6 };

            int[] validPlatforms = lastDisabledSide ? leftPlatforms : rightPlatforms;
            int randomPlatformIndex;
            do
            {
                randomPlatformIndex = validPlatforms[Main.rand.Next(validPlatforms.Length)];
            }
            while (randomPlatformIndex == lastDisabledPlatformIndex);

            lastDisabledPlatformIndex = randomPlatformIndex;
            lastDisabledSide = !lastDisabledSide;
            chosenPlatformCenter = LargeGeode.GetPlatformCenter(randomPlatformIndex);
            float platformWidthTiles = LargeGeode.GetPlatformWidth(randomPlatformIndex);
            chosenPlatformWidthPixels = platformWidthTiles * 16f;
            /*if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                IEntitySource source = NPC.GetSource_FromAI();
                int projIndex = Projectile.NewProjectile(source, new Vector2(chosenPlatformCenter.X + 8, chosenPlatformCenter.Y - 16), Vector2.Zero, ModContent.ProjectileType<TelegraphX>(), 0, 0f, Main.myPlayer);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncProjectile, number: projIndex);
            }*/
        }

        private void SpawnLightningTelegraphGlow(Vector2 platformCenter, float widthPixels)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            IEntitySource source = NPC.GetSource_FromAI();

            lightningTelegraphId = Projectile.NewProjectile(
                source,
                platformCenter,
                Vector2.Zero,
                ModContent.ProjectileType<LightningTelegraphGlow>(),
                0,
                0f,
                Main.myPlayer
            );

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncProjectile, number: lightningTelegraphId);

            if (lightningTelegraphId >= 0 && lightningTelegraphId < Main.maxProjectiles &&
                Main.projectile[lightningTelegraphId].ModProjectile is LightningTelegraphGlow telegraph)
            {
                telegraph.SetWidth(widthPixels);
                Main.projectile[lightningTelegraphId].position = platformCenter - new Vector2((Main.projectile[lightningTelegraphId].width / 2f) - 8, Main.projectile[lightningTelegraphId].height / 2f + 10);
            }
        }

        private void SpawnElectricFieldOnPlatform(Vector2 platformCenter, float widthPixels)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            IEntitySource source = NPC.GetSource_FromAI();

            float halfWidth = widthPixels / 2f;
            Vector2 leftEdgePos = new(platformCenter.X - (halfWidth - 16), platformCenter.Y - 2);

            int projIndex = Projectile.NewProjectile(
                source,
                new Vector2(leftEdgePos.X - 20, leftEdgePos.Y),
                Vector2.Zero,
                ModContent.ProjectileType<ElectricSpikeField>(),
                50,
                0f,
                Main.myPlayer
            );

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncProjectile, number: projIndex);

            if (projIndex >= 0 && projIndex < Main.maxProjectiles &&
                Main.projectile[projIndex].ModProjectile is ElectricSpikeField field)
            {
                field.SetWidth(widthPixels);
                Main.projectile[projIndex].position =
                    platformCenter - new Vector2(field.Projectile.width / 2f - 8,
                                                 field.Projectile.height / 2f + 10);
            }
        }


        private void SpawnStalactiteProjectiles()
        {
            /*float baseCeilingY = ArenaData.ArenaCenter.Y - 300;
            float leftX = ArenaData.OuterArenaBoundaryLeft.X;
            float rightX = ArenaData.OuterArenaBoundaryRight.X;
            int numStalactites = 8;

            for (int i = 0; i < numStalactites; i++)
            {
                float x = Main.rand.NextFloat(leftX, rightX);
                float delay = Main.rand.Next(0, 61);
                int tileX = (int)(x / 16);
                int startTileY = (int)(baseCeilingY / 16);
                int foundTileY = startTileY;
                for (int y = startTileY; y >= 0; y--)
                {
                    Tile tile = Framing.GetTileSafely(tileX, y);
                    if (tile.HasTile && Main.tileSolid[tile.TileType])
                    {
                        foundTileY = y;
                        break;
                    }
                }
                Vector2 spawnPos = new Vector2(x, foundTileY * 16);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero,
                    ModContent.ProjectileType<Stalactite>(), NPC.damage / 2, 0f, Main.myPlayer, delay);
            }*/
        }

        private void ResetAttack()
        {
            StopAttackVFX();
            Main.NewText("Reset attack");
            OnAttackFinished();
            attackTimer = 0;
            SpawnedOrbs = false;
        }

        private int rockingTimer = 0;

        private void RockingBackAndForthAttack(bool useMagnetRocks, bool useArenaCrystalZappers, bool usePhase3, Player player)
        {
            Main.NewText("Rocking");
            NPC.noGravity = false;
            float targetX = rockingMovingRight
                ? ArenaData.InnerArenaBoundaryRight.X
                : ArenaData.InnerArenaBoundaryLeft.X;

            float healthFactor = 1f - (NPC.life / (float)NPC.lifeMax);
            float desiredSpeed = MathHelper.Lerp(3f, 6f, healthFactor);
            float acceleration = 0.02f;
            float direction = Math.Sign(targetX - NPC.Center.X);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, direction * desiredSpeed, acceleration);
            if (Math.Abs(NPC.Center.X - targetX) < 160)
            {
                rockingMovingRight = !rockingMovingRight;
            }

            if (useMagnetRocks)
            {
                if (!magnetSequenceActive)
                {
                    StartMagnetRockSequence();
                }
                UpdateMagnetRockSequence();
            }
            if (useArenaCrystalZappers)
            {

            }
            if (usePhase3)
            {
                PhaseThreeSequence();
            }

            if (++rockingTimer >= 600)
            {
                OnAttackFinished();
                rockingTimer = 0;
            }
        }

        private int magnetSequenceTimer = 0;
        private int magnetOrb1 = -1;
        private int magnetOrb2 = -1;
        private int magnetOrb3 = -1;
        private bool magnetSequenceActive = false;


        private void StartMagnetRockSequence()
        {
            Main.NewText("Magnet rocks");
            if (!magnetSequenceActive)
            {
                magnetSequenceActive = true;
                magnetSequenceTimer = 0;
                magnetOrb1 = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                    ModContent.ProjectileType<MagneticOrb>(), NPC.damage, 0f, Main.myPlayer, ai0: 1);
                magnetOrb2 = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                    ModContent.ProjectileType<MagneticOrb>(), NPC.damage, 0f, Main.myPlayer, ai0: 2);
                if (Main.expertMode)
                {
                    magnetOrb3 = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<MagneticOrb>(), NPC.damage, 0f, Main.myPlayer, ai0: 3);
                }
            }
        }

        private void UpdateMagnetRockSequence()
        {
            Main.NewText("updating magnet rocks");
            if (!magnetSequenceActive)
                return;
            magnetSequenceTimer++;
            float orbitSpeed = 2f;
            float baseAngle = (magnetSequenceTimer / 60f) * orbitSpeed;
            float amplitude = 40f;
            float oscillation = (float)Math.Sin(magnetSequenceTimer / 60f) * amplitude;
            float baseRadius = 125f;
            float innerRadius = baseRadius + oscillation;
            float outerRadius = baseRadius + oscillation + 20f;

            if (!Main.expertMode)
            {
                Vector2 orbPos1 = NPC.Center + new Vector2((float)Math.Cos(baseAngle) * innerRadius, (float)Math.Sin(baseAngle) * innerRadius);
                Vector2 orbPos2 = NPC.Center + new Vector2((float)Math.Cos(baseAngle + MathHelper.Pi) * outerRadius, (float)Math.Sin(baseAngle + MathHelper.Pi) * outerRadius);
                if (magnetOrb1 >= 0 && Main.projectile[magnetOrb1].active)
                {
                    Main.projectile[magnetOrb1].Center = orbPos1;
                }
                if (magnetOrb2 >= 0 && Main.projectile[magnetOrb2].active)
                {
                    Main.projectile[magnetOrb2].Center = orbPos2;
                }
            }
            else
            {
                float radius = baseRadius + oscillation;
                Vector2 orbPos1 = NPC.Center + new Vector2((float)Math.Cos(baseAngle) * radius, (float)Math.Sin(baseAngle) * radius);
                Vector2 orbPos2 = NPC.Center + new Vector2((float)Math.Cos(baseAngle + 2 * MathHelper.Pi / 3) * radius,
                                                            (float)Math.Sin(baseAngle + 2 * MathHelper.Pi / 3) * radius);
                Vector2 orbPos3 = NPC.Center + new Vector2((float)Math.Cos(baseAngle + 4 * MathHelper.Pi / 3) * radius,
                                                            (float)Math.Sin(baseAngle + 4 * MathHelper.Pi / 3) * radius);

                if (magnetOrb1 >= 0 && Main.projectile[magnetOrb1].active)
                {
                    Main.projectile[magnetOrb1].Center = orbPos1;
                }
                if (magnetOrb2 >= 0 && Main.projectile[magnetOrb2].active)
                {
                    Main.projectile[magnetOrb2].Center = orbPos2;
                }
                if (magnetOrb3 >= 0 && Main.projectile[magnetOrb3].active)
                {
                    Main.projectile[magnetOrb3].Center = orbPos3;
                }
            }
            if (magnetSequenceTimer >= 600)
            {
                if (magnetOrb1 >= 0 && Main.projectile[magnetOrb1].active)
                    Main.projectile[magnetOrb1].Kill();
                if (magnetOrb2 >= 0 && Main.projectile[magnetOrb2].active)
                    Main.projectile[magnetOrb2].Kill();
                if (Main.expertMode && magnetOrb3 >= 0 && Main.projectile[magnetOrb3].active)
                    Main.projectile[magnetOrb3].Kill();
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                    ModContent.ProjectileType<WandOfExplodingExplosion>(), NPC.damage * 2, 0f, Main.myPlayer);
                magnetSequenceActive = false;
                magnetSequenceTimer = 0;
                magnetOrb1 = -1;
                magnetOrb2 = -1;
                magnetOrb3 = -1;
            }
        }


        private int arenaElectrocutionTimer = 0;
        private int arenaElectrocutionPhase = 0;
        private int[] crystalOrder = new int[3];
        private int crystalsFired = 0;
        private bool crystalElectrocutePhaseActive = false;

        private void StartArenaCrystalElectrocution()
        {
            Main.NewText("Starting arena crystal electrocution");
            crystalElectrocutePhaseActive = true;
            arenaElectrocutionPhase = 0;
            arenaElectrocutionTimer = 0;
            crystalsFired = 0;
            List<int> order = [0, 1, 2];
            for (int i = 0; i < 3; i++)
            {
                int index = Main.rand.Next(order.Count);
                crystalOrder[i] = order[index];
                order.RemoveAt(index);
            }
        }

        private void ArenaCrystalElectrocutionSequence(Player player)
        {
            Main.NewText("Electric execution");
            arenaElectrocutionTimer++;

            switch (arenaElectrocutionPhase)
            {
                case 0:
                    if (arenaElectrocutionTimer >= 30)
                    {
                        arenaElectrocutionPhase = 1;
                        arenaElectrocutionTimer = 0;
                    }
                    break;

                case 1:
                    if (arenaElectrocutionTimer >= 180 && crystalsFired < crystalOrder.Length)
                    {
                        int crystalIndex = crystalOrder[crystalsFired];
                        FireCrystalElectrocution(crystalIndex, player);
                        crystalsFired++;
                        arenaElectrocutionTimer = 0;

                        if (crystalsFired >= 3)
                        {
                            arenaElectrocutionPhase = 2;
                            arenaElectrocutionTimer = 0;
                        }
                    }
                    break;

                case 2:
                    if (arenaElectrocutionTimer == 30)
                    {
                        Vector2 finalLightningPos = ArenaData.ArenaCenter + new Vector2(0, -10 * 16f);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), finalLightningPos, Vector2.Zero,
                            ModContent.ProjectileType<LightningStar>(), NPC.damage / 2, 0f, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), finalLightningPos, Vector2.Zero,
                            ModContent.ProjectileType<TumblerOrb>(), NPC.damage, 0f, Main.myPlayer);
                    }
                    if (arenaElectrocutionTimer >= 60)
                    {
                        OnAttackFinished();
                        crystalElectrocutePhaseActive = false;

                        arenaElectrocutionPhase = 0;
                        arenaElectrocutionTimer = 0;
                    }
                    break;
            }
        }

        private void FireCrystalElectrocution(int crystalIndex, Player player)
        {
            Main.NewText("Shoot electric");
            float floorY = ArenaData.ArenaCenter.Y;
            int tileRange = 10;
            int rangePixels = tileRange * 16;

            Vector2 shootPos1, shootPos2, shootPos3;

            if (crystalIndex == 0)
            {
                shootPos1 = new Vector2(ArenaData.InnerArenaBoundaryLeft.X + Main.rand.NextFloat(0, rangePixels), floorY);
                shootPos2 = new Vector2(ArenaData.ArenaCenter.X + Main.rand.NextFloat(-rangePixels, rangePixels), floorY);
                shootPos3 = player.Center;
            }
            else if (crystalIndex == 1)
            {
                shootPos1 = new Vector2(ArenaData.ArenaCenter.X + Main.rand.NextFloat(-rangePixels, rangePixels), floorY);
                bool chooseLeft = Main.rand.NextBool();
                if (chooseLeft)
                {
                    shootPos2 = new Vector2(ArenaData.InnerArenaBoundaryLeft.X + rangePixels, floorY);
                    shootPos3 = new Vector2(ArenaData.InnerArenaBoundaryRight.X - rangePixels, floorY);
                }
                else
                {
                    shootPos2 = new Vector2(ArenaData.InnerArenaBoundaryRight.X - rangePixels, floorY);
                    shootPos3 = new Vector2(ArenaData.InnerArenaBoundaryLeft.X + rangePixels, floorY);
                }
            }
            else if (crystalIndex == 2)
            {
                shootPos1 = new Vector2(ArenaData.InnerArenaBoundaryRight.X - Main.rand.NextFloat(0, rangePixels), floorY);
                shootPos2 = new Vector2(ArenaData.ArenaCenter.X + Main.rand.NextFloat(-rangePixels, rangePixels), floorY);
                shootPos3 = player.Center;
            }
            else
            {
                shootPos1 = shootPos2 = shootPos3 = new Vector2(ArenaData.ArenaCenter.X, floorY);
            }
            if (LargeGeode.crystalPositions != null && LargeGeode.crystalPositions.Length >= 3)
            {
                shootPos1 = LargeGeode.crystalPositions[crystalIndex];
            }

            Projectile.NewProjectile(NPC.GetSource_FromThis(), shootPos1, Vector2.Zero,
                ModContent.ProjectileType<LightningStar>(), NPC.damage / 4, 0f, Main.myPlayer);
            Projectile.NewProjectile(NPC.GetSource_FromThis(), shootPos2, Vector2.Zero,
                ModContent.ProjectileType<LightningStar>(), NPC.damage / 4, 0f, Main.myPlayer);
            Projectile.NewProjectile(NPC.GetSource_FromThis(), shootPos3, Vector2.Zero,
                ModContent.ProjectileType<LightningStar>(), NPC.damage / 4, 0f, Main.myPlayer);
        }

        private void PhaseThreeSequence()
        {
            //TODO this
        }
    }

    public class LightningBolt : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        private LightningData lightningData;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.damage = 10;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Rectangle lightningHitbox = new(
                (int)(Projectile.Center.X - Projectile.width / 2f),
                (int)(Projectile.Center.Y),
                Projectile.width,
                760
            );

            return lightningHitbox.Intersects(targetHitbox);
        }


        public override void AI()
        {
            if (lightningData == null || !lightningData.Initialized)
            {
                lightningData = new LightningData(Projectile, LightningStyle.Static);
                Vector2 startPos = Projectile.Center;
                Vector2 endPos = Projectile.Center + new Vector2(0f, 760f);
                LightningUtility.InitializeBetweenPoints(lightningData, startPos, endPos, LightningStyle.Static);
            }
            LightningUtility.UpdateSegments(lightningData);
            LightningUtility.UpdateBranches(lightningData);
            LightningUtility.SpawnDust(lightningData);
            if (Projectile.timeLeft < 10)
                lightningData.Alpha *= 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (lightningData == null || !lightningData.Initialized)
                return false;
            LightningUtility.DrawLightning(lightningData, Main.spriteBatch);
            return false;
        }
    }

    public class LightningStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        private float fadeOutRate = 0.02f;
        private float starAlpha = 0;
        int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 5;
            Projectile.scale = 1f;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            timer++;
            if (timer < 120)
            {
                Projectile.alpha = (int)MathHelper.Lerp(255, 0, timer / 120f);
            }
            else
            {
                Projectile.alpha += (int)(fadeOutRate * 255);
                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                    return;
                }
            }
            starAlpha = MathHelper.Clamp(MathHelper.Lerp(starAlpha, 1.25f, 0.02f), 0f, 1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer > 0)
            {

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
                Texture2D glowTex = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition;
                Color glowColor = Color.Aqua * ((255 - Projectile.alpha) / 255f);
                Main.spriteBatch.Draw(glowTex, drawPosition, null, glowColor, 0f, glowTex.Size() / 2, 0.5f, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                return false;
            }
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D spotTex = Mod.Assets.Request<Texture2D>("Assets/Flare/CrispStarPMA").Value;
            Texture2D glowTex = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;
            Color adjustedColor = Color.Black * 0.5f * starAlpha * ((255 - Projectile.alpha) / 255f);
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(glowTex, pos, glowTex.Frame(1, 1, 0, 0), adjustedColor, Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), glowTex.Size() / 2, 0.1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), adjustedColor, Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), spotTex.Size() / 2, 1.2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), adjustedColor, Projectile.rotation + MathHelper.ToRadians(timer * 0.15f), spotTex.Size() / 2, 0.75f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(glowTex, pos, glowTex.Frame(1, 1, 0, 0), Color.Blue * 0.3f * starAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), glowTex.Size() / 2, 0.2f, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.Blue * 2f * starAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), spotTex.Size() / 2, 1.2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.Aqua * 1.5f * starAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(timer * 0.15f), spotTex.Size() / 2, 0.75f, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.Blue * 2f * starAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), spotTex.Size() / 2, 0.75f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.White * starAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(timer * 0.15f), spotTex.Size() / 2, 0.4f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
    public class LightningTelegraphGlow : ModProjectile
    {
        private float fieldWidth = 0f;
        private float fadeInProgress = 0f;
        private float fadeOutProgress = 0f;
        private float pulsateTimer = 0f;
        private bool startFadeOut = false;
        private float fadeSpeed = 0.02f;
        private float pulseSpeed = 0.05f;

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public void SetWidth(float width)
        {
            fieldWidth = width;
            Projectile.width = (int)width;
            Projectile.height = 24;
        }

        public override void AI()
        {
            if (!startFadeOut && fadeInProgress < 1f)
            {
                fadeInProgress += fadeSpeed;
                if (fadeInProgress >= 1f)
                {
                    fadeInProgress = 1f;
                }
            }
            pulsateTimer += pulseSpeed;
            if (startFadeOut || Projectile.timeLeft < 60)
            {
                startFadeOut = true;
                fadeOutProgress += fadeSpeed;

                if (fadeOutProgress >= 1f)
                {
                    Projectile.Kill();
                }
            }
        }

        public void StartFadeOut()
        {
            startFadeOut = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            DrawSmoothGlowStrip();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        private void DrawSmoothGlowStrip(float layerDepth = 0f)
        {
            Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/GlowTrailSlice").Value;
            Rectangle glowSourceRect = new Rectangle(0, 0, 1, 100);
            float scaleY = Projectile.height / 50f;
            int fadeRange = 6;
            float time = Main.GameUpdateCount * 0.04f;
            Color[] warningGradientColors =
            [
                Color.Red,
            Color.OrangeRed,
            Color.Crimson,
            Color.DarkRed,
            Color.IndianRed
            ];
            float alpha = fadeInProgress;
            if (startFadeOut)
            {
                alpha *= (1f - fadeOutProgress);
            }
            float warningPulse = (float)Math.Sin(pulsateTimer) * 0.5f + 0.5f;
            if (Projectile.timeLeft < 120)
            {
                warningPulse = Math.Max(warningPulse, 1f - (Projectile.timeLeft / 120f));
            }
            for (int i = 0; i < Projectile.width; i++)
            {
                float position = (i / (float)Projectile.width) + time;
                position %= 1f;
                float scaledPos = position * warningGradientColors.Length;
                int colorIndex1 = (int)scaledPos % warningGradientColors.Length;
                int colorIndex2 = (colorIndex1 + 1) % warningGradientColors.Length;
                float lerpFactor = scaledPos - (int)scaledPos;
                Color baseColor = Color.Lerp(warningGradientColors[colorIndex1], warningGradientColors[colorIndex2], lerpFactor);
                Color warningColor = warningGradientColors[colorIndex1];
                if (warningPulse > 0)
                    baseColor = Color.Lerp(baseColor, warningColor, warningPulse);
                float brightness = 0.8f + 0.2f * (float)Math.Sin(time * 2f + i * 0.05f);
                baseColor = baseColor * brightness;
                float fadeFactor = 1f;
                if (i < fadeRange)
                    fadeFactor = i / (float)fadeRange;
                else if (i > Projectile.width - fadeRange)
                    fadeFactor = (Projectile.width - i) / (float)fadeRange;
                Color finalColor = baseColor * fadeFactor * alpha;
                Vector2 pos = new(Projectile.position.X + i, Projectile.position.Y - 24);
                Main.spriteBatch.Draw(glowTexture, pos - Main.screenPosition, glowSourceRect, finalColor, 0f, Vector2.Zero, new Vector2(1f, scaleY), SpriteEffects.None, layerDepth);
            }
        }
    }

    public class ElectricSpikeField : ModProjectile
    {
        private float fieldWidth = 0f;
        private int tileCount = 0;

        private int animFrame = 0;
        private int animFrameCounter = 0;
        private int maxAnimFrames = 3;
        private int animFrameSpeed = 6;

        private int cellWidth = 16;
        private int cellHeight = 24;
        private int cellPadding = 2;

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.hide = true;
            Projectile.penetrate = -1;
        }

        public void SetWidth(float width)
        {
            fieldWidth = width;
            tileCount = (int)(fieldWidth / cellWidth);
            if (tileCount < 2)
                tileCount = 2;
            Projectile.width = tileCount * cellWidth;
            Projectile.height = cellHeight;
        }

        public override void AI()
        {
            animFrameCounter++;
            if (animFrameCounter >= animFrameSpeed)
            {
                animFrameCounter = 0;
                animFrame = (animFrame + 1) % maxAnimFrames;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.position - Main.screenPosition;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            DrawSmoothGlowStrip(0.9f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < tileCount; i++)
            {
                int col = 1;
                if (i == 0)
                    col = 0;
                else if (i == tileCount - 1)
                    col = 3;
                else
                    col = (i % 2 == 0) ? 1 : 2;
                int row = animFrame;
                Rectangle sourceRect = new(
                    col * (cellWidth + cellPadding),
                    row * (cellHeight + cellPadding), cellWidth, cellHeight);
                for (int t = 0; t < 8; t++)
                {
                    Color color = t == 0 ? Color.SkyBlue with { A = 0 } : Color.DeepSkyBlue with { A = 0 };
                    Main.spriteBatch.Draw(mainTexture, drawPos + Main.rand.NextVector2Circular(0, 5f), sourceRect, color * 0.3f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                }

                Main.spriteBatch.Draw(mainTexture, drawPos, sourceRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);

                drawPos.X += cellWidth;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GenerateGlowDust();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        private void DrawSmoothGlowStrip(float layerDepth = 0f)
        {
            Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/GlowTrailSlice").Value;
            Rectangle glowSourceRect = new Rectangle(0, 0, 1, 100);
            float scaleY = Projectile.height / 50f;
            int fadeRange = 6;
            float time = Main.GameUpdateCount * 0.04f;

            Color[] gradientColors =
            [
                Color.Aqua,
                Color.Blue,
                Color.SkyBlue,
                Color.MediumPurple,
                Color.RoyalBlue
            ];

            for (int i = 0; i < Projectile.width; i++)
            {
                float position = (i / (float)Projectile.width) + time;
                position %= 1f;
                float scaledPos = position * gradientColors.Length;
                int colorIndex1 = (int)scaledPos % gradientColors.Length;
                int colorIndex2 = (colorIndex1 + 1) % gradientColors.Length;
                float lerpFactor = scaledPos - (int)scaledPos;
                Color baseColor = Color.Lerp(gradientColors[colorIndex1], gradientColors[colorIndex2], lerpFactor);
                float brightness = 0.8f + 0.2f * (float)Math.Sin(time * 2f + i * 0.05f);
                baseColor = baseColor * brightness;
                float fadeFactor = 1f;
                if (i < fadeRange)
                    fadeFactor = i / (float)fadeRange;
                else if (i > Projectile.width - fadeRange)
                    fadeFactor = (Projectile.width - i) / (float)fadeRange;
                Color finalColor = baseColor * fadeFactor;
                Vector2 pos = new(Projectile.position.X + i, Projectile.position.Y - 24);
                Main.spriteBatch.Draw(glowTexture, pos - Main.screenPosition, glowSourceRect, finalColor, 0f, Vector2.Zero, new Vector2(1f, scaleY), SpriteEffects.None, layerDepth);
            }
        }

        private void GenerateGlowDust()
        {
            int dustCount = 2;
            for (int j = 0; j < dustCount; j++)
            {
                float randX = Main.rand.NextFloat(Projectile.width);
                Vector2 dustPos = new Vector2(Projectile.position.X + randX, (Projectile.position.Y + Projectile.height) - 2);
                Dust gd = Dust.NewDustPerfect(dustPos, ModContent.DustType<GlowPixelCross>(), new Vector2(0f, -Main.rand.NextFloat(1f, 2f)), newColor: Color.Aqua, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5, preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }
        }
    }

    public class TelegraphX : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Assets/Telegraph/X";

        private float flashAlpha = 1f;
        private float scaleSineTimer = 0f;
        private float baseScale = 1f;
        private float scaleAmplitude = 0.2f;

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 58;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 405;
            Projectile.alpha = 0;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Main.NewText(Projectile.timeLeft);

            if (flashAlpha > 0f)
            {
                flashAlpha -= 0.1f;
                if (flashAlpha < 0f)
                    flashAlpha = 0f;
            }
            scaleSineTimer += 0.1f;
            float sineValue = (float)Math.Sin(scaleSineTimer);
            Projectile.scale = baseScale + scaleAmplitude * sineValue;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Telegraph/X_Glow").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = glowTexture.Size() / 2f;
            float scale = Projectile.scale;
            Color glowColor = Color.Cyan * (0.5f + 0.5f * flashAlpha);
            Main.spriteBatch.Draw(glowTexture, drawPosition, null, glowColor, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D xTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Telegraph/X").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 origin = xTexture.Size() / 2f;
            float overallAlpha = 1f;
            Color xColor = Color.White * overallAlpha * (1f + flashAlpha * 0.5f);
            float scale = Projectile.scale;
            Main.spriteBatch.Draw(xTexture, drawPos, null, xColor, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
        }
    }

    public class EnergyAbsorptionOrb : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        private float orbAlpha = 0f;
        private float rotationSpeed;
        private float scaleMultiplier;
        private Color orbColor;

        private bool initialized = false;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
        }

        public override void OnSpawn(IEntitySource source)
        {
            rotationSpeed = Main.rand.NextFloat(-0.1f, 0.1f);
            scaleMultiplier = Main.rand.NextFloat(0.7f, 1.3f);
            orbColor = Main.rand.NextBool() ? Color.DeepSkyBlue : Color.Cyan;
        }

        private int timer = 0;

        public override void AI()
        {
            timer++;
            if (!initialized)
            {
                initialized = true;
                int targetNPC = (int)Projectile.ai[0];
                if (targetNPC >= 0 && targetNPC < Main.maxNPCs && Main.npc[targetNPC].active)
                {
                    Vector2 toBoss = Main.npc[targetNPC].Center - Projectile.Center;
                    float dist = toBoss.Length();
                    if (dist > 0)
                    {
                        toBoss.Normalize();
                        Projectile.velocity = Vector2.Zero;
                        Projectile.ai[1] = toBoss.X;
                        Projectile.ai[2] = toBoss.Y;
                    }
                }
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            orbAlpha = 1f - (Projectile.alpha / 255f);
            Projectile.rotation += 0.05f;
            int npcIndex = (int)Projectile.ai[0];
            if (npcIndex >= 0 && npcIndex < Main.maxNPCs && Main.npc[npcIndex].active)
            {
                NPC target = Main.npc[npcIndex];
                if (timer > 60)
                {
                    Vector2 direction = target.Center - Projectile.Center;
                    float distance = direction.Length();

                    if (distance > 65f)
                    {
                        direction.Normalize();

                        float acceleration = 0.2f;
                        float maxSpeed = 5f;
                        float speedIncrease = MathHelper.Lerp(0.1f, 4f, (timer - 60) / 100f);
                        speedIncrease = Math.Min(speedIncrease, maxSpeed);

                        Projectile.velocity += direction * acceleration * speedIncrease;
                        Projectile.velocity = Vector2.Clamp(Projectile.velocity, -Vector2.One * maxSpeed, Vector2.One * maxSpeed);
                    }
                    else
                    {
                        if (Main.rand.NextBool(3))
                        {
                            Vector2 dustPos = Projectile.Center + Main.rand.NextVector2Circular(10f, 10f);
                            Dust dust = Dust.NewDustPerfect(dustPos, DustID.IceTorch, Vector2.Zero, 0, orbColor * 0.7f, Main.rand.NextFloat(0.5f, 0.8f));
                            dust.noGravity = true;
                            dust.fadeIn = 1.2f;
                        }
                        Projectile.Kill();
                    }
                }
                else
                    Projectile.scale = 1f + 0.1f * (float)Math.Sin(timer * 0.2f);
            }
            else
            {
                Projectile.Kill();
                return;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Glow").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() / 2f;
            Main.spriteBatch.Draw(glowTexture, drawPosition, null, orbColor * 0.1f * orbAlpha, Projectile.rotation, glowTexture.Size() / 2, 0.3f * scaleMultiplier, SpriteEffects.None, 0f);
            float scale = 0.2f * scaleMultiplier * (1f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f) * 0.1f);
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White * 0.7f * orbAlpha, -Projectile.rotation * 0.2f, origin, scale * 0.6f, SpriteEffects.None, 0f);
            Texture2D spotTex = Mod.Assets.Request<Texture2D>("Assets/Flare/CrispStarPMA").Value;
            Texture2D glowTex = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;
            Color adjustedColor = Color.Black * 0.5f * orbAlpha * ((255 - Projectile.alpha) / 255f);
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(glowTex, pos, glowTex.Frame(1, 1, 0, 0), adjustedColor, Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), glowTex.Size() / 2, 0.1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), adjustedColor, Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), spotTex.Size() / 2, 1.2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), adjustedColor, Projectile.rotation + MathHelper.ToRadians(timer * 0.15f), spotTex.Size() / 2, 0.75f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(glowTex, pos, glowTex.Frame(1, 1, 0, 0), Color.Blue * 0.3f * orbAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), glowTex.Size() / 2, 0.2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.Blue * 2f * orbAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), spotTex.Size() / 2, 1.2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.Aqua * 1.5f * orbAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(timer * 0.15f), spotTex.Size() / 2, 0.75f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.Blue * 2f * orbAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), spotTex.Size() / 2, 0.75f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.White * orbAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(timer * 0.15f), spotTex.Size() / 2, 0.4f, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }

    public class LightningOrbStar : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Assets/Orbs/feather_circle";

        private float orbAlpha = 0f;
        private float rotationSpeed;
        private float pulseRate;
        private bool isHugeVariant = false;
        private int lightningCooldown = 0;
        private int riseTimer = 0;
        private const int MAX_RISE_TIME = 120;
        private bool hasReachedTarget = false;
        private Vector2 targetPosition;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 900;
            Projectile.alpha = 255;
            Projectile.scale = 1f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            rotationSpeed = Main.rand.NextFloat(0.01f, 0.03f);
            pulseRate = Main.rand.NextFloat(0.03f, 0.06f);
            isHugeVariant = Projectile.ai[0] == 1f;

            if (isHugeVariant)
            {
                Projectile.scale = 2f;
                Projectile.width = 64;
                Projectile.height = 64;
                Projectile.damage *= 2;
            }
            targetPosition = new Vector2(Projectile.position.X, ArenaData.WaterLayer * 16);
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 10;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            orbAlpha = 1f - (Projectile.alpha / 255f);
            Projectile.rotation += rotationSpeed;
            if (!hasReachedTarget)
            {
                riseTimer++;
                float progress = (float)riseTimer / MAX_RISE_TIME;

                if (progress >= 1f)
                {
                    hasReachedTarget = true;
                    Projectile.position.Y = targetPosition.Y;
                    Projectile.velocity = Vector2.Zero;
                }
                else
                {
                    float easedProgress = EaseInOutQuad(progress);
                    float targetY = MathHelper.Lerp(Projectile.position.Y, targetPosition.Y, easedProgress);
                    Projectile.position.Y = targetY;
                }
            }
            if (hasReachedTarget)
            {
                if (lightningCooldown <= 0)
                {
                    FireLightning();
                    if (isHugeVariant)
                    {
                        lightningCooldown = Main.rand.Next(40, 80);
                        if (Main.expertMode)
                        {
                            FireRadialLightning();
                        }
                    }
                    else
                    {
                        lightningCooldown = Main.rand.Next(60, 120);
                    }
                }
                else
                {
                    lightningCooldown--;
                }
            }
            SpawnElectricDust();
        }

        private float EaseInOutQuad(float t)
        {
            return (float)(t < 0.5 ? 2 * t * t : 1 - Math.Pow(-2 * t + 2, 2) / 2);
        }

        private void FireLightning()
        {
            Vector2 lightningStart = Projectile.Center;
            Vector2 lightningEnd = new(lightningStart.X, ArenaData.ArenaCenter.Y);
            LightningManager.StrikeLightning(lightningStart, lightningEnd, 60);
            TelegraphLightningDust(lightningStart, lightningEnd, 30, 5f);
            /*SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/Thunder")
            {
                Volume = 0.5f,
                Pitch = Main.rand.NextFloat(-0.2f, 0.2f),
                PitchVariance = 0.2f,
            };
            SoundEngine.PlaySound(style, Projectile.Center);*/
            Player player = Main.LocalPlayer;
            player.GetModPlayer<AeroPlayer>().ScreenShakePower = isHugeVariant ? 15 : 5;
        }

        private void FireRadialLightning()
        {
            int boltCount = Main.rand.Next(3, 6);

            for (int i = 0; i < boltCount; i++)
            {
                float angle = Main.rand.NextFloat() * MathHelper.TwoPi;
                float distance = Main.rand.Next(200, 400);
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 lightningEnd = Projectile.Center + direction * distance;
                LightningManager.StrikeLightning(Projectile.Center, lightningEnd, 40);
                TelegraphLightningDust(Projectile.Center, lightningEnd, 20, 3f);
            }
        }

        private void TelegraphLightningDust(Vector2 start, Vector2 end, int segmentCount = 30, float maxDisplacement = 5f)
        {
            Vector2 direction = end - start;
            float length = direction.Length();
            if (length <= 0f)
                return;
            direction.Normalize();
            Vector2 normal = new(-direction.Y, direction.X);
            float segmentLength = length / (segmentCount - 1);
            for (int i = 0; i < segmentCount; i++)
            {
                Vector2 pos = start + direction * segmentLength * i;
                float displacement = (Main.rand.NextFloat() - 0.5f) * 2f * maxDisplacement;
                pos += normal * displacement;
                int dustIndex = Dust.NewDust(pos, 4, 4, DustID.Electric, 0f, 0f, 100, Color.Cyan, 1f);
                Main.dust[dustIndex].noGravity = true;
            }
        }

        private void SpawnElectricDust()
        {
            int dustCount = isHugeVariant ? 3 : 1;
            float dustScale = isHugeVariant ? 1.5f : 1f;

            for (int i = 0; i < dustCount; i++)
            {
                Vector2 dustOffset = Main.rand.NextVector2CircularEdge(Projectile.width / 2f, Projectile.height / 2f);
                Vector2 dustPos = Projectile.Center + dustOffset;
                int dust = Dust.NewDust(dustPos, 4, 4, DustID.Electric, 0f, 0f, 0, Color.Cyan, Main.rand.NextFloat(0.8f, 1.2f) * dustScale);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = dustOffset * 0.05f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Glow").Value;
            Texture2D starTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Flare/CrispStarPMA").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() / 2f;
            float pulseScale = 1f + 0.2f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6.2f * pulseRate);
            float finalScale = Projectile.scale * pulseScale;
            float glowScale = isHugeVariant ? 2.5f : 1.5f;
            Main.spriteBatch.Draw(glowTexture, drawPosition, null, Color.Cyan * 0.7f * orbAlpha, 0f, glowTexture.Size() / 2, 0.5f * finalScale * glowScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(starTexture, drawPosition, null, Color.DeepSkyBlue * 0.8f * orbAlpha, Projectile.rotation * 1.5f, starTexture.Size() / 2, 0.5f * finalScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(starTexture, drawPosition, null, Color.White * 0.6f * orbAlpha, -Projectile.rotation, starTexture.Size() / 2, 0.3f * finalScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.Blue * orbAlpha, Projectile.rotation, origin, 0.3f * finalScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White * 0.8f * orbAlpha, -Projectile.rotation * 0.5f, origin, 0.15f * finalScale, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(5f, 5f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, velocity, 0, Color.Cyan, 1.5f);
                dust.noGravity = true;
            }
            if (isHugeVariant)
            {
                for (int i = 0; i < 8; i++)
                {
                    float angle = MathHelper.TwoPi * i / 8;
                    Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                    Vector2 lightningEnd = Projectile.Center + direction * 200;

                    LightningManager.StrikeLightning(Projectile.Center, lightningEnd, 30);
                }
                Player player = Main.LocalPlayer;
                player.GetModPlayer<AeroPlayer>().ScreenShakePower = 20;
            }
        }
    }
    public class CrystalCastle : ModProjectile
    {
        private bool initialized = false;
        private float growthProgress = 0f;
        private float growthRate = 0.005f;
        private Vector2 basePosition;
        private int dustSpawnRate = 2;

        private int textureWidth = 198;
        private int textureHeight = 300;

        private bool fullyGrown = false;
        private bool destroyed = false;
        private int destroyAnimTimer = 0;
        private int destroyAnimDuration = 30;

        private float sineWaveTimer = 0f;
        private float sineWaveSpeed = 0.1f;
        private float sineWaveAmplitude = 1f;
        private float sineWaveDustRange;
        private const float initialSineWaveDustRange = 100f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = textureWidth;
            Projectile.height = textureHeight;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 3600;
            Projectile.alpha = 0;
            Projectile.scale = 1f;
            Projectile.light = 0.5f;
            sineWaveDustRange = initialSineWaveDustRange;
        }

        public override void AI()
        {
            if (!initialized)
            {
                basePosition = Projectile.position;
                initialized = true;
                SoundEngine.PlaySound(SoundID.Item70, Projectile.position);
            }
            if (growthProgress < 1f)
                GrowCrystal();
            else if (!fullyGrown)
                OnFullyGrown();
            if (destroyed)
                UpdateDestroyAnimation();
            else
            {
                if (fullyGrown && Main.rand.NextBool(30))
                    SpawnAmbientDust();
                UpdateSineWaveDust();
            }
        }

        private void UpdateSineWaveDust()
        {
            sineWaveTimer += sineWaveSpeed;
            sineWaveDustRange = initialSineWaveDustRange * (1f - (growthProgress * 0.6f));
            float visibleHeight = growthProgress * textureHeight;
            float currentGrowingPoint = Projectile.position.Y;
            if (Main.GameUpdateCount % 3 == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    float offset = i == 0 ? 0 : MathHelper.Pi;
                    float xPos = (float)Math.Sin(sineWaveTimer + offset) * sineWaveDustRange;

                    Vector2 dustPos = new(Projectile.Center.X + xPos, currentGrowingPoint + (float)Math.Sin(sineWaveTimer * 0.5f) * sineWaveAmplitude * 10);
                    Vector2 dustVel = new(0, -0.2f);
                    Color dustColor = i == 0 ? Color.SkyBlue : Color.Yellow;
                    Dust gd = Dust.NewDustPerfect(dustPos, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: dustColor, Scale: Main.rand.NextFloat(0.2f, 0.35f));
                    gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                        rotPower: 0.3f,
                        timeBeforeSlow: 20,
                        preSlowPower: 0.94f,
                        postSlowPower: 0.90f,
                        velToBeginShrink: 1f,
                        fadePower: 0.92f,
                        shouldFadeColor: false
                    );
                }
            }
        }

        private void GrowCrystal()
        {
            growthProgress = Math.Min(growthProgress + growthRate, 1f);
            float visibleHeight = growthProgress * textureHeight;
            Projectile.position.Y = basePosition.Y - textureHeight + (1f - growthProgress) * textureHeight;
            if (Main.rand.NextBool(dustSpawnRate))
            {
                float currentGrowingPoint = Projectile.position.Y + visibleHeight - 5;
                Vector2 dustPos = new(Projectile.Center.X + Main.rand.Next(-30, 31), currentGrowingPoint);
                Dust dust = Dust.NewDustDirect(dustPos, 10, 10, DustID.BlueCrystalShard);
                dust.noGravity = true;
                dust.scale = 1.2f;
                dust.velocity.Y = -Math.Abs(dust.velocity.Y) * 0.5f;
                dust.velocity.X *= 0.3f;
            }
        }

        private void OnFullyGrown()
        {
            fullyGrown = true;
            SoundEngine.PlaySound(SoundID.Item70 with { Volume = 0.8f, Pitch = 0.2f }, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                Vector2 dustPos = new(Projectile.Center.X + Main.rand.Next(-40, 41), Projectile.position.Y + Main.rand.Next(0, textureHeight));

                Dust dust = Dust.NewDustDirect(dustPos, 10, 10, DustID.BlueCrystalShard);
                dust.noGravity = true;
                dust.scale = 1.5f;
                dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
            }
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (!destroyed)
            {
                destroyed = true;
                Projectile.timeLeft = destroyAnimDuration;
                SoundStyle style = new("AerovelenceMod/Sounds/Effects/CrystalSlam")
                {
                    Volume = 0.9f,
                    Pitch = -0.2f,
                    PitchVariance = 0.1f,
                };
                SoundEngine.PlaySound(style, Projectile.Center);
                if (Main.player[Projectile.owner] != null && Main.player[Projectile.owner].active)
                {
                    Player player = Main.player[Projectile.owner];
                    if (player.GetModPlayer<AeroPlayer>() != null)
                    {
                        player.GetModPlayer<AeroPlayer>().ScreenShakePower = 10;
                    }
                }
            }
        }

        private void UpdateDestroyAnimation()
        {
            destroyAnimTimer++;
            Projectile.alpha = (int)MathHelper.Lerp(0, 255, destroyAnimTimer / (float)destroyAnimDuration);
            if (destroyAnimTimer % 2 == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 position = new(Projectile.Center.X + Main.rand.Next(-40, 41), Projectile.position.Y + Main.rand.Next(0, textureHeight));

                    Vector2 velocity = Main.rand.NextVector2CircularEdge(5f, 5f);
                    int dust = Dust.NewDust(position, 10, 10, DustID.BlueCrystalShard, velocity.X, velocity.Y, 100, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                }
            }
        }

        private void SpawnAmbientDust()
        {
            Vector2 position = new Vector2(
                Projectile.Center.X + Main.rand.Next(-30, 31),
                Projectile.position.Y + Main.rand.Next(0, textureHeight)
            );

            int dust = Dust.NewDust(position, 4, 4, DustID.Electric, 0f, 0f, 100, Color.DeepSkyBlue, 1.2f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-1f, -0.2f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle sourceRectangle = new(0, 0, texture.Width, (int)(texture.Height * growthProgress));
            Vector2 origin = new(texture.Width / 2, 0);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            drawPosition.Y = Projectile.position.Y - Main.screenPosition.Y;
            Main.spriteBatch.Draw(texture, drawPosition, sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}