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
using AerovelenceMod.Content.Items.Weapons.Caverns.ThunderLance;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using AerovelenceMod.Content.NPCs.TownNPC.RockCollector;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System.Net;
using Terraria.Graphics.Effects;
using AerovelenceMod.Content.Projectiles;
using static AerovelenceMod.Content.Projectiles.LightningUtility;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AerovelenceMod.Content.NPCs.CrystalCaverns;

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
        RockThrow
    }

    [AutoloadBossHead]
    public class CrystalTumbler2 : ModNPC
    {
        private TumblerAttackState currentAttack = TumblerAttackState.Idle;
        private int attackTimer = 0;
        private int idleDuration = 180;
        private int phase = 1;

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

        private static readonly TumblerAttackState[][] combos =
        {
            new TumblerAttackState[]
            {
                TumblerAttackState.CrystalBarrage,
                TumblerAttackState.RollToDash,
                TumblerAttackState.DashOuterToOuter,
                TumblerAttackState.RollToSideAndSlam,
                TumblerAttackState.DashSideToSide,
                TumblerAttackState.RollToSideAndSlam
            },

            new TumblerAttackState[]
            {
                TumblerAttackState.WaterLightning,
                TumblerAttackState.RollToDash,
                TumblerAttackState.DashOuterToOuter,
                TumblerAttackState.Idle
            },

            new TumblerAttackState[]
            {
                TumblerAttackState.RollToSideAndSlam,
                TumblerAttackState.CrystalBarrage,
                TumblerAttackState.RollToSideAndSlam
            }
        };

        private void StartCombo(int comboIndex)
        {
            comboIndex = Math.Clamp(comboIndex, 0, combos.Length - 1);

            isInCombo = true;
            currentComboIndex = comboIndex;
            currentComboStep = 0;
            currentAttack = combos[currentComboIndex][currentComboStep];
            attackTimer = 0;

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
            Main.NewText("Attack finished");
            attackTimer = 0;
            idleTimer = 0;
            SpawnedOrbs = false;
            NPC.noTileCollide = false;
            NPC.noGravity = false;

            if (isInCombo)
            {
                currentComboStep++;
                if (currentComboStep >= combos[currentComboIndex].Length)
                {
                    isInCombo = false;
                    SelectNextAttack();
                    return;
                }
                else
                {
                    currentAttack = combos[currentComboIndex][currentComboStep];
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
        private Vector2[] lightningStrikePositions = new Vector2[10];
        private int lightningStrikeIndex = 0;
        private int anotherTimer = 0;
        private bool readyToSpawnTelegraphStrikes = false;
        private float lineExtraPower = 0;

        private void DrawTelegraphLine(SpriteBatch spriteBatch, Vector2 start, Vector2 direction, Texture2D lineTexture, Color color, float opacity, float length)
        {
            Vector2 lineScale = new Vector2(length / lineTexture.Width, 0.2f + (lineExtraPower * 0.1f)) * 1.5f;
            float rotation = direction.ToRotation();
            spriteBatch.Draw(lineTexture, start - Main.screenPosition, null, color * opacity * 1.5f, rotation, new Vector2(0, lineTexture.Height / 2), lineScale * 1.25f, SpriteEffects.None, 0f);
            spriteBatch.Draw(lineTexture, start - Main.screenPosition, null, Color.Aqua * opacity * 1.6f, rotation, new Vector2(0, lineTexture.Height / 2), lineScale * 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(lineTexture, start - Main.screenPosition, null, color * opacity * 1.7f, rotation, new Vector2(0, lineTexture.Height / 2), lineScale * 0.75f, SpriteEffects.None, 0f);
        }

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


        public override void AI()
        {
            if (firstFrame)
            {
                firstFrame = false;
            }

            lineExtraPower = Math.Clamp(MathHelper.Lerp(lineExtraPower, -0.25f, 0.1f), 0f, 1f);

            if (doLightning && !lightningStrikePositionsInitialized)
            {
                if (lightningStrikePositions == null || lightningStrikePositions.Length < 10)
                {
                    lightningStrikePositions = new Vector2[10];
                }
                else
                {
                    bool bossOnLeft = NPC.Center.X < ArenaData.ArenaCenter.X;

                    for (int i = 0; i < 10; i++)
                    {
                        float factor = (float)i / 9f;
                        if (!bossOnLeft)
                            factor = 1f - factor;
                        float xPosition = MathHelper.Lerp(ArenaData.OuterArenaBoundaryLeft.X, ArenaData.OuterArenaBoundaryRight.X, factor);
                        float yPosition = NPC.Center.Y - 200;
                        lightningStrikePositions[i] = new Vector2(xPosition, yPosition);
                        Vector2 telegraphStart = lightningStrikePositions[i];
                        Vector2 telegraphEnd = telegraphStart + new Vector2(0, 500);
                        TelegraphLightningDust(new Vector2(telegraphStart.X, telegraphStart.Y -500), telegraphEnd, segmentCount: 12, maxDisplacement: 5f);
                    }
                    //readyToSpawnTelegraphStrikes = true;
                    lightningStrikePositionsInitialized = true;
                }
            }

            if (doLightning)
            {
                if (lightningStrikePositionsInitialized)
                {
                    anotherTimer++;
                    if (anotherTimer > 60)
                    {
                        if (anotherTimer % 10 == 0 && lightningStrikeIndex < lightningStrikePositions.Length)
                        {
                            Vector2 spawnPosition = lightningStrikePositions[lightningStrikeIndex];
                            Vector2 spawnOffset = spawnPosition - new Vector2(0, 500f);
                            Vector2 downwardVelocity = new Vector2(0, 15f);

                            Projectile.NewProjectile(spawnSource: NPC.GetSource_FromAI(), spawnOffset, downwardVelocity,
                                ModContent.ProjectileType<LightningBolt>(), damage, 2, Main.myPlayer, ai0: 1);

                            lightningStrikeIndex++;
                        }

                        if (lightningStrikeIndex >= lightningStrikePositions.Length)
                        {
                            lightningStrikePositionsInitialized = false;
                            //readyToSpawnTelegraphStrikes = false;
                            doLightning = false;
                            lightningStrikeIndex = 0;
                            anotherTimer = 0;
                        }
                    }
                }
            }


            if (isExecutingWaterLightning)
            {
                UpdateLightningWaterAttack();
            }
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

            float healthFactor = 1f - (NPC.life / (float)NPC.lifeMax);
            float rotationFactor = 2f + healthFactor * 2f;
            NPC.rotation += NPC.velocity.X / NPC.width * rotationFactor;

            if (currentAttack == TumblerAttackState.Idle)
            {
                Main.NewText("Idle");
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

                attackTimer++;
                if (attackTimer >= idleDuration)
                {
                    SelectNextAttack();
                    attackTimer = 0;
                }
            }
            else if (currentAttack == TumblerAttackState.RockingBackAndForth)
            {
                Main.NewText("Rocking");
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
                
                Main.NewText("Roll to dash");
                RollToDashAttack();
            }
            else if (currentAttack == TumblerAttackState.RollToSideAndSlam)
            {
                Main.NewText("Roll to side");
                RollToSideAndSlamAttack(player);
            }
            else if (currentAttack == TumblerAttackState.DashOuterToOuter)
            {
                
                Main.NewText("Dash Outer to Outer");
                DashOuterToOuterSequence(player);
            }
            if (currentAttack == TumblerAttackState.DashSideToSide)
            {
                Main.NewText("dash side to side");
                DashSideToSideSequence(Main.player[NPC.target]);
            }
            if (currentAttack == TumblerAttackState.WaterLightning)
            {

                Main.NewText("Water lightning");
                WaterLightningAttack();
            }
            else
            {
                Vector2 directionToPlayer = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                float desiredSpeed = MathHelper.Lerp(3f, 6f, healthFactor);
                float acceleration = 0.1f;
                switch (currentAttack)
                {
                    case TumblerAttackState.CrystalBarrage:
                        Main.NewText("Crystal Barrage");
                        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, -directionToPlayer.X * desiredSpeed, acceleration);
                        CrystalBarrageAttack();
                        break;
                    case TumblerAttackState.CrystalLightning:
                        if (!crystalElectrocutePhaseActive)
                        {
                            StartArenaCrystalElectrocution();
                            Main.NewText("Electric phase");
                        }
                        ArenaCrystalElectrocutionSequence(player);
                        break;
                    default:
                        break;
                }
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
                Main.EntitySpriteDraw(npcTexture, NPC.Center - Main.screenPosition + new Vector2(0, 4), null, drawColor, NPC.rotation, npcTexture.Size() / 2, 1f, SpriteEffects.None, 0f);
                Main.EntitySpriteDraw(npcTexture, NPC.Center - Main.screenPosition + new Vector2(0, 4), null, Color.White with { A = 0 } * 0.25f, NPC.rotation, npcTexture.Size() / 2, 1, SpriteEffects.None, 0f);
            }
            else
            {
                Main.EntitySpriteDraw(npcTexture, NPC.Center - Main.screenPosition + new Vector2(0, 4), null, drawColor, NPC.rotation, npcTexture.Size() / 2, 1f, SpriteEffects.None, 0f);
            }
            return false;
        }

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

            Main.EntitySpriteDraw(Dash, drawPosition, null, Color.Aquamarine * dashIntensity, NPC.rotation / 2, new Vector2(Dash.Width / 2f, Dash.Height / 2f), 0.4f, flipEffect, 0);
            Main.EntitySpriteDraw(WaveGlow, drawPosition, null, Color.DeepSkyBlue * dashIntensity, 0, new Vector2(WaveGlow.Width / 2f, WaveGlow.Height / 2f), 0.58f, SpriteEffects.None, 0);

            /*if (activateShieldVFX)
            {
                Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/Orbs/whiteFireEye").Value;
                Texture2D Flare2 = Mod.Assets.Request<Texture2D>("Assets/Orbs/spiky_20fade").Value;
                Texture2D Flare3 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/pixelKennySlash").Value;
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
            
            if (EyeGlow)
            {
                Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/TrailImages/GlowStar").Value;
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
            }*/
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            if (readyToSpawnTelegraphStrikes)
            {
                Texture2D lineTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Medusa_Gray").Value;
                Color telegraphColor = Color.Blue;
                float telegraphLength = 500f;
                foreach (var position in lightningStrikePositions)
                {
                    Vector2 directionDown = -Vector2.UnitY;
                    Vector2 positionBelow = position - new Vector2(0, -264);
                    DrawTelegraphLine(spriteBatch, positionBelow, directionDown, lineTexture, telegraphColor, 1f, telegraphLength);
                }
            }

        }

        private void SelectNextAttack()
        {
            comboCycleIndex++;
            if (comboCycleIndex >= combos.Length)
                comboCycleIndex = 0;
            StartCombo(comboCycleIndex);
        }



        private void PerformRockThrow()
        {
            Main.NewText("Performing rock throw");
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
            Main.NewText("Spawn Wall");
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
            Main.NewText("Performing rock slam");

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


        private int lastSpawnDirection = 0; //0 = top, 1 = right, 2 = bottom, 3 = left


        private void SpawnRadialProjectiles()
        {
            Main.NewText("Spawning radial projectiles");

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



        private void UpdatePhase()
        {
            float lifePercent = (float)NPC.life / NPC.lifeMax;
            if (phase == 1 && lifePercent < 0.5f)
            {
                phase = 2;
            }
            else if (phase == 2 && lifePercent < 0.25f)
            {
                phase = 3;
            }
        }

        private void CrystalBarrageAttack()
        {
            StartAttackVFX();
            Main.NewText("Crystal Barrage");
            attackTimer++;
            if (attackTimer == 10)
            {
                SpawnRadialProjectiles();
            }

            if(attackTimer == 20)
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
            Main.NewText("Water Lightning");
            waterLightningTimer++;

            if (waterLightningTimer == 0)
            {
                pending.Clear();
                Vector2 innerArenaLeft = ArenaData.InnerArenaBoundaryLeft;
                Vector2 innerArenaRight = ArenaData.InnerArenaBoundaryRight;
                int waterLayerTile = ArenaData.WaterLayer;

                int lightningCount = Main.rand.Next(8, 11);
                float arenaWidth = innerArenaRight.X - innerArenaLeft.X;
                float spacing = arenaWidth / lightningCount;
                float randomOffset = Main.rand.NextFloat(0, spacing);
                pending = [];
                if (spawnFromLeft)
                {
                    for (int i = 0; i < lightningCount; i++)
                    {
                        float lightningX = innerArenaLeft.X + randomOffset + (i * spacing);
                        pending.Add(new Vector2(lightningX, waterLayerTile * 16));
                    }
                }
                else
                {
                    for (int i = 0; i < lightningCount; i++)
                    {
                        float lightningX = innerArenaRight.X - randomOffset - (i * spacing);
                        pending.Add(new Vector2(lightningX, waterLayerTile * 16));
                    }
                }

                spawnFromLeft = !spawnFromLeft;
            }

            if (waterLightningTimer < pending.Count * 30)
            {
                int currentStrikeIndex = waterLightningTimer / 30;
                if (currentStrikeIndex < pending.Count)
                {
                    TelegraphDustLine(pending[currentStrikeIndex], 150f);
                }
            }
            if (waterLightningTimer == pending.Count * 30)
            {
                foreach (Vector2 position in pending)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), position, Vector2.Zero,
                        ModContent.ProjectileType<LightningHitFX>(), NPC.damage / 4, 0f, Main.myPlayer);
                }
                pending.Clear();
            }

            waterLightningTimer++;

            if (waterLightningTimer > pending.Count * 30 + 30)
            {
                isExecutingWaterLightning = false;
                OnAttackFinished();
            }
        }

        private void WaterLightningAttack()
        {
            Main.NewText("Water Lightning");
            attackTimer++;

            if (attackTimer == 20)
            {
                Vector2 innerArenaLeft = ArenaData.InnerArenaBoundaryLeft;
                Vector2 innerArenaRight = ArenaData.InnerArenaBoundaryRight;
                int waterLayerTile = ArenaData.WaterLayer;

                int lightningCount = Main.rand.Next(8, 11);
                float arenaWidth = innerArenaRight.X - innerArenaLeft.X;
                float spacing = arenaWidth / lightningCount;
                float randomOffset = Main.rand.NextFloat(0, spacing);

                for (int i = 0; i < lightningCount; i++)
                {
                    float lightningX = innerArenaLeft.X + randomOffset + (i * spacing);
                    Vector2 lightningSpawnPosition = new Vector2(lightningX, waterLayerTile * 16);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), lightningSpawnPosition, Vector2.Zero,
                        ModContent.ProjectileType<LightningHitFX>(), NPC.damage / 4, 0f, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), lightningSpawnPosition, Vector2.Zero,
                        ModContent.ProjectileType<LightningStar>(), NPC.damage / 4, 0f, Main.myPlayer);
                    Vector2 orbVelocity = new(0, -20f);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), lightningSpawnPosition, orbVelocity, ModContent.ProjectileType<MagneticOrb>(), NPC.damage / 4, 0f, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), lightningSpawnPosition, new Vector2(0, 1f), ModContent.ProjectileType<LightningStar>(), NPC.damage / 4, 0f, Main.myPlayer);
                    TelegraphDustLine(lightningSpawnPosition, 144f);
                }
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

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                        ModContent.ProjectileType<CrystalShard>(), NPC.damage / 4, 0f, Main.myPlayer);
                }
            }
        }

        private void TelegraphDustLine(Vector2 position, float lineHeight)
        {
            int dustCount = 1;
            for (int i = 0; i < dustCount; i++)
            {
                float yOffset = Main.rand.NextFloat(0, lineHeight);
                Vector2 dustPosition = position - new Vector2(0, yOffset);

                int dust = Dust.NewDust(dustPosition, 4, 4, DustID.GemSapphire, 0f, -0.5f, 100, default, 1.2f);
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
            Main.NewText("Roll To Dash, Phase: " + rollDashPhase);
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
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * projectileSpeed,
                                    ModContent.ProjectileType<CrystalShard>(), NPC.damage / 4, 0f, Main.myPlayer);
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

        private void RollToSideAndSlamAttack(Player player)
        {
            Main.NewText("Roll to Side and Slam");
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

                        if (Math.Abs(distance) > 160f)
                        {
                            NPC.velocity.X += accel * Math.Sign(distance);
                            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -desiredSpeed, desiredSpeed);
                            
                        }
                        else
                        {
                            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0f, 0.1f);
                            NPC.rotation += rollSlamHorizontalDirection * Math.Abs(NPC.velocity.Y) * 0.01f;
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
                                if (Math.Abs(player.velocity.Y) < 0.1f)
                                {
                                    player.velocity.Y = -10f;
                                }
                                break;

                            }
                        }

                        if (groundFound)
                        {
                            doLightning = true;
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
                            SoundEngine.PlaySound(style, NPC.Center);
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
            Main.NewText("Spawn Moth");

            int moth = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
                ModContent.NPCType<Charger>());
        }
        private void TriggerBoundarySlamEffects()
        {
            Main.NewText("Trigger Boundary Slam");
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

        private void DashSideToSideSequence(Player player)
        {
            Main.NewText("Dash side to side");
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
                        dashSideTimer++;
                        if (dashSideTimer >= 60)
                        {
                            shouldPerformJump = !shouldPerformJump;
                            if (shouldPerformJump)
                            {
                                Main.NewText("Switching to Phase 8");
                                dashSidePhase = 8;
                                SetupBezierJump();
                            }
                            else
                            {
                                Main.NewText("Switching to Phase 2");
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
                        doLightning = true;
                        KickRocks();
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
            Main.NewText("Spawn Orbs");
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
            if (!dashVariantIsPlayerTargeted)
            {
                StartAttackVFX();
                Main.NewText("Dash outer to outer (Original)");
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
                            KickRocks();
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
            else 
            {
                StartAttackVFX();
                Main.NewText("Dash outer to outer (Player-Targeted)");
                switch (dashPlayerPhase)
                {
                    case 0:
                        {
                            if (dashPlayerTimer == 0)
                            {
                                playerDashTarget = player.Center;
                                dashSideDirection = (playerDashTarget.X > NPC.Center.X) ? 1f : -1f;
                            }
                            float targetSpinRate = 1.2f;
                            float spinIncrement = targetSpinRate / 300f;
                            storedExtraSpin = Math.Min(storedExtraSpin + spinIncrement, targetSpinRate);
                            SpawnOrbProjectiles();
                            NPC.rotation += dashSideDirection * storedExtraSpin;
                            dashPlayerTimer++;
                            if (dashPlayerTimer >= 60)
                            {
                                preStunRotation = NPC.rotation;
                                dashPlayerPhase = 1;
                                dashPlayerTimer = 0;
                                storedExtraSpin = 0f;
                            }
                        }
                        break;
                    case 1:
                        {
                            isDashing = true;
                            float dashSpeed = 20f;
                            Vector2 toTarget = playerDashTarget - NPC.Center;
                            float distance = toTarget.Length();
                            if (distance > 0)
                            {
                                Vector2 dashDirection = toTarget / distance;
                                NPC.velocity += Vector2.Lerp(NPC.velocity, dashDirection * dashSpeed, 0.1f);
                            }
                            dashPlayerTimer++;
                            if (distance < 10f || dashPlayerTimer >= 5)
                            {
                                NPC.velocity = Vector2.Zero;
                                NPC.noGravity = false;
                                NPC.noTileCollide = false;
                                dashPlayerPhase = 2;
                                dashPlayerTimer = 0;
                                preStunRotation = NPC.rotation;
                                isDashing = false;
                            }
                        }
                        break;
                    case 2:
                        {
                            dashPlayerTimer++;
                            float rockAmplitude = MathHelper.Lerp(0.1f, 0f, dashPlayerTimer / 30f);
                            NPC.rotation = preStunRotation + (float)Math.Sin(dashPlayerTimer * 0.2f) * rockAmplitude;
                            if (dashPlayerTimer >= 30)
                            {
                                dashPlayerPhase = 3;
                                dashPlayerTimer = 0;
                            }
                            if (dashPlayerTimer == 1)
                            {
                                PerformWallSlam(NPC.Center, 15);
                            }
                            StopAttackVFX();
                        }
                        break;
                    case 3:
                        {
                            dashVariantIsPlayerTargeted = false;
                            dashPlayerPhase = 0;
                            dashPlayerTimer = 0;
                            OnAttackFinished();
                        }
                        break;
                }
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
                float kickSpeed = Main.rand.Next(8);
                Vector2 velocity = kickDirection * kickSpeed;
                Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, velocity,
                    ModContent.ProjectileType<RockShard>(), NPC.damage / 2, 0f, Main.myPlayer);
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
            List<int> order = new List<int> { 0, 1, 2 };
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
                lightningData = new LightningData(Projectile, LightningStyle.Jagged);
                Vector2 startPos = Projectile.Center;
                Vector2 endPos = Projectile.Center + new Vector2(0f, 760f);
                LightningUtility.InitializeBetweenPoints(lightningData, startPos, endPos, LightningStyle.Jagged);
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
            Texture2D spotTex = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/CrispStarPMA").Value;
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
            Main.spriteBatch.Draw(spotTex, pos , spotTex.Frame(1, 1, 0, 0), Color.Aqua * 1.5f * starAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(timer * 0.15f), spotTex.Size() / 2, 0.75f, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.Blue * 2f * starAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(-1 * timer * 0.3f), spotTex.Size() / 2, 0.75f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.White * starAlpha * ((255 - Projectile.alpha) / 255f), Projectile.rotation + MathHelper.ToRadians(timer * 0.15f), spotTex.Size() / 2, 0.4f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}