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

        public float glowIntensity = 0f;

        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.width = 120;
            NPC.height = 128;
            NPC.lifeMax = 5500;
            NPC.damage = 5;
            NPC.defense = 15;
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

        public override void AI()
        {
            
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
                if (idleTimer >= idleDuration)
                {
                    if (Math.Abs(NPC.Center.X - player.Center.X) > 300f)
                    {
                        float dashSpeedMultiplier = 2f;
                        NPC.velocity.X = directionToPlayer.X * desiredSpeed * dashSpeedMultiplier;
                    }
                    else
                    {
                        NPC.velocity *= 1.5f;
                        NPC.rotation += Main.rand.NextFloat(-0.2f, 0.2f);
                    }
                    idleTimer = 0;
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
                RollToDashAttack();
            }
            else if (currentAttack == TumblerAttackState.RollToSideAndSlam)
            {
                RollToSideAndSlamAttack(player);
            }
            else if (currentAttack == TumblerAttackState.DashOuterToOuter)
            {
                DashOuterToOuterSequence(player);
            }
            if (currentAttack == TumblerAttackState.DashSideToSide)
            {
                DashSideToSideSequence(Main.player[NPC.target]);
            }
            else
            {
                switch (currentAttack)
                {
                    case TumblerAttackState.CrystalBarrage:
                        CrystalBarrageAttack();
                        break;
                    case TumblerAttackState.RockThrow:
                        PerformRockThrow();
                        break;
                    case TumblerAttackState.WaterLightning:
                        WaterLightningAttack();
                        break;
                    case TumblerAttackState.CrystalLightning:
                        if (!crystalElectrocutePhaseActive)
                        {
                            StartArenaCrystalElectrocution();
                        }
                        ArenaCrystalElectrocutionSequence(player);
                        break;
                    case TumblerAttackState.SpawnRadialProjectiles:
                        SpawnRadialProjectiles(7, 300f);
                        break;
                    default:
                        break;
                }
            }
        }

        private bool showAfterImage = false;
        public List<float> previousRotations;
        public List<Vector2> previousPostions;

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

                    Main.EntitySpriteDraw(npcTexture, NPC.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f) + new Vector2(0, 4), null, col * 1f, NPC.rotation, npcTexture.Size() / 2f, 1.1f, SpriteEffects.None, 0f);
                }
                Main.EntitySpriteDraw(npcTexture, NPC.Center - Main.screenPosition + new Vector2(0, 4), null, drawColor, NPC.rotation, npcTexture.Size() / 2, 1f, SpriteEffects.None, 0f);
                Main.EntitySpriteDraw(npcTexture, NPC.Center - Main.screenPosition + new Vector2(0, 4), null, Color.White with { A = 0 } * 0.25f, NPC.rotation, npcTexture.Size() / 2, 1f, SpriteEffects.None, 0f);
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
            Vector2 origin = NPC.frame.Size() / 2f;
            spriteBatch.Draw(texture, drawPosition, NPC.frame, Color.White, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0);
            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D Bloommy = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/Bloommy");
            Main.EntitySpriteDraw(Bloommy, drawPosition, NPC.frame, Color.White * glowIntensity, NPC.rotation, NPC.frame.Size() / 2f, 1, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(Bloommy, drawPosition, NPC.frame, Color.White * glowIntensity, NPC.rotation, NPC.frame.Size() / 2f, 1, SpriteEffects.None, 0);
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
        }

        private void SelectNextAttack()
        {
            int choice = Main.rand.Next(6);
            if (choice == 0)
                currentAttack = TumblerAttackState.CrystalBarrage;
            else if (choice == 1)
                currentAttack = TumblerAttackState.WaterLightning;
            else if (choice == 2)
                currentAttack = TumblerAttackState.RollToDash;
            else if (choice == 3)
                currentAttack = TumblerAttackState.SpawnRadialProjectiles;
            else if (choice == 4)
                currentAttack = TumblerAttackState.RollToSideAndSlam;
            else if (choice == 5)
                currentAttack = TumblerAttackState.DashSideToSide;
            else
                currentAttack = TumblerAttackState.DashOuterToOuter;
            attackTimer = 0;

            if (currentAttack == TumblerAttackState.RollToDash)
            {
                rollDashPhase = 0;
                rollDashTimer = 0;
            }
        }

        private void PerformRockThrow()
        {
            attackTimer++;
            if(attackTimer == 0)
            {
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
            

            if (attackTimer > 120)
            {
                ResetAttack();
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

        private void SpawnRadialProjectiles(int numProjectiles, float radius)
        {
            attackTimer++;
            if (attackTimer == 1)
            {
                Vector2 playerPosition = Main.player[NPC.target].Center;

                for (int i = 0; i < numProjectiles; i++)
                {
                    float angle = MathHelper.ToRadians(360f / numProjectiles * i);
                    Vector2 position = playerPosition + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                    Vector2 direction = Vector2.Normalize(playerPosition - position);

                    int projectileID = Projectile.NewProjectile(NPC.GetSource_FromAI(), position, Vector2.Zero, ModContent.ProjectileType<EnchantedEye>(), damage, 1, Main.myPlayer);

                    Main.projectile[projectileID].ai[1] = playerPosition.X;
                    Main.projectile[projectileID].ai[2] = playerPosition.Y;

                    Main.projectile[projectileID].rotation = direction.ToRotation() + MathHelper.PiOver2;
                }
            }

            if (attackTimer > 120)
            {
                ResetAttack();
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
            Main.NewText("Crystal Barrage");
            attackTimer++;
            if (attackTimer == 10)
            {
                Player target = Main.player[NPC.target];
                for (int i = 0; i < 3; i++)
                {
                    Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                    float angleOffset = MathHelper.ToRadians(-10 + (10 * i));
                    Vector2 shootVelocity = direction.RotatedBy(angleOffset) * 10f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootVelocity,
                        ModContent.ProjectileType<CrystalShard>(),
                        15, 0f, Main.myPlayer);
                }
            }

            if (attackTimer > 60)
            {
                ResetAttack();
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
                float lightningX = Main.rand.NextFloat(innerArenaLeft.X, innerArenaRight.X);
                Vector2 lightningSpawnPosition = new Vector2(lightningX, waterLayerTile * 16);

                Projectile.NewProjectile(NPC.GetSource_FromThis(), lightningSpawnPosition, Vector2.Zero,
                    ModContent.ProjectileType<LightningHitFX>(),
                    NPC.damage / 4, 0f, Main.myPlayer);
            }

            if (attackTimer > 60)
            {
                ResetAttack();
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
                        glowIntensity += 0.1f;
                    }
                    break;

                case 1:
                    {
                        Player target = Main.player[NPC.target];
                        float spinDirection = Math.Sign(target.Center.X - NPC.Center.X);
                        float targetSpinRate = 1.01f;
                        float spinIncrement = targetSpinRate / 300f;
                        storedExtraSpin = Math.Min(storedExtraSpin + spinIncrement, targetSpinRate);
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
                            rollDashTimer = 0;
                        }
                    }
                    break;

                case 2:
                    {
                        if (dashNearWall)
                        {
                            NPC.velocity = new Vector2(chosenDashDirection * 14f, 0f);
                            showAfterImage = true;
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
                                    showAfterImage = true;
                                }
                            }
                            else
                            {
                                NPC.velocity.X *= 0.95f;
                            }
                            storedExtraSpin *= 0.95f;
                            glowIntensity -= 0.1f;
                            rollDashTimer++;
                            if (Math.Abs(NPC.velocity.X) < 0.5f)
                            {
                                Vector2 lightningSpawnPosition = new Vector2(NPC.Center.X, ArenaData.WaterLayer * 16);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                                    ModContent.ProjectileType<LightningLaser>(), NPC.damage / 4, 0f, Main.myPlayer);
                                showAfterImage = false;
                                rollDashPhase = 3;
                                rollDashTimer = 0;
                            }
                        }
                    }
                    break;

                case 3:
                    {
                        ResetAttack();
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
        private int damage = 50;
        private float rollSlamHorizontalDirection = 1f;

        private void RollToSideAndSlamAttack(Player player)
        {
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
                                rollSlamTimer = 0;
                            }
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            glowIntensity *= 1.1f;
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
                        showAfterImage = true;
                        NPC.rotation += rollSlamHorizontalDirection * Math.Abs(NPC.velocity.Y) * 0.01f;

                        if (NPC.velocity.Y > 0)
                        {
                            NPC.noGravity = true;
                            NPC.velocity.Y += 1.1f;
                            glowIntensity *= 1.1f;

                            int checkX = (int)(NPC.Center.X / 16);
                            bool groundFound = false;
                            for (int offset = 0; offset <= 7; offset++)
                            {
                                glowIntensity *= 0.98f;
                                int checkY = (int)(NPC.Bottom.Y / 16) + offset;
                                Tile tileBelow = Framing.GetTileSafely(checkX, checkY);
                                if (tileBelow.HasTile &&
                                   (tileBelow.TileType == ModContent.TileType<SmoothCavernStoneTile>() ||
                                    tileBelow.TileType == ModContent.TileType<CitadelBrickTile>() ||
                                    tileBelow.TileType == ModContent.TileType<ChargedStoneTile>()))
                                {
                                    groundFound = true;
                                    NPC.velocity.Y = 0;
                                    NPC.position.Y = checkY * 16 - NPC.height;
                                    NPC.noTileCollide = false;
                                    NPC.noGravity = false;
                                    glowIntensity = 0f;
                                    break;
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
                        glowIntensity += 1.01f;
                        if (rollSlamTimer == 0)
                        {
                            NPC.velocity.Y = -20f;
                            initialRotation = NPC.rotation;
                            targetRotation = MathHelper.ToRadians(15);
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
                        glowIntensity *= 0.98f;
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

                                glowIntensity = 0f;
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
                            SoundEngine.PlaySound(style, NPC.Center);
                            showAfterImage = false;
                            ResetAttack();
                            rollSlamPhase = 0;
                            rollSlamTimer = 0;
                        }
                        else
                        {
                            rollSlamTimer++;
                            if (rollSlamTimer > 30)
                            {
                                ResetAttack();
                                rollSlamPhase = 0;
                                rollSlamTimer = 0;
                            }
                        }
                    }
                    break;
            }
        }

        private void TriggerBoundarySlamEffects()
        {
            Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 30;
            SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/CrystalSlam")
            {
                Volume = 0.85f,
                Pitch = 0f,
                PitchVariance = 0f,
            };
            SoundEngine.PlaySound(style, NPC.Center);
            SpawnStalactiteProjectiles();
        }


        private float preStunRotation = 0f;

        private void DashSideToSideSequence(Player player)
        {
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
                        float targetSpinRate = 1.01f;
                        float spinIncrement = targetSpinRate / 300f;
                        storedExtraSpin = Math.Min(storedExtraSpin + spinIncrement, targetSpinRate);
                        NPC.rotation += dashSideDirection * storedExtraSpin;
                        dashSideTimer++;
                        if (dashSideTimer >= 60)
                        {
                            dashSidePhase = 2;
                            dashSideTimer = 0;
                        }

                        showAfterImage = true;

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
                        }
                    }
                    break;

                case 3:
                    {
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
                            SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/HardRockSlam")
                            {
                                Volume = 0.75f,
                                Pitch = 1f,
                                PitchVariance = 0f,
                            };
                            SoundEngine.PlaySound(stylea, NPC.Center);
                            Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 15;
                            SpawnStalactiteProjectiles();
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
                        dashSideDirection = (NPC.Center.X > ArenaData.ArenaCenter.X) ? -1f : 1f;
                        float targetSpinRate = 1.01f;
                        float spinIncrement = targetSpinRate / 300f;
                        storedExtraSpin = Math.Min(storedExtraSpin + spinIncrement, targetSpinRate);
                        NPC.rotation += dashSideDirection * storedExtraSpin;
                        dashSideTimer++;
                        if (dashSideTimer >= 60)
                        {
                            dashSidePhase = 6;
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

                case 6:
                    {
                        dashSideTimer++;
                        NPC.noTileCollide = true;
                        NPC.noGravity = true;
                        float finalDashSpeed = 16f;
                        NPC.velocity = new Vector2(dashSideDirection * finalDashSpeed, 0f);
                        if (dashSideIteration >= 1 && dashSideTimer % 30 == 0)
                        {
                            Vector2 lightningSpawn = new Vector2(player.Center.X, player.Center.Y - 50 * 16f);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), lightningSpawn, Vector2.Zero,
                                ModContent.ProjectileType<LightningLaser>(), NPC.damage / 2, 0f, Main.myPlayer);
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
                        dashSideTimer++;
                        float finalRockAmplitude = MathHelper.Lerp(0.3f, 0f, dashSideTimer / 120f);
                        NPC.rotation = preStunRotation + (float)Math.Sin(dashSideTimer * 0.1f) * finalRockAmplitude;
                        if (dashSideTimer >= 120)
                        {
                            dashSidePhase = 0;
                            dashSideTimer = 0;
                            dashSideIteration = 0;
                            NPC.velocity = Vector2.Zero;
                            ResetAttack();
                        }
                        if (dashSideTimer == 1)
                        {
                            SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/HardRockSlam")
                            {
                                Volume = 0.75f,
                                Pitch = 1f,
                                PitchVariance = 0f,
                            };
                            SoundEngine.PlaySound(stylea, NPC.Center);
                            Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 15;
                            SpawnStalactiteProjectiles();
                            showAfterImage = false;
                        }
                    }
                    break;
            }
        }

        private int dashOuterPhase = 0;
        private int dashOuterTimer = 0;
        private int dashOuterIteration = 0;
        private int maxDashIterations = 3;
        private float dashOuterDirection = 0f;

        private void DashOuterToOuterSequence(Player player)
        {
            switch (dashOuterPhase)
            {
                case 0:
                    {
                        dashOuterDirection = (NPC.Center.X < ArenaData.ArenaCenter.X) ? 1f : -1f;
                        float targetSpinRate = 1.2f;
                        float spinIncrement = targetSpinRate / 300f;
                        storedExtraSpin = Math.Min(storedExtraSpin + spinIncrement, targetSpinRate);
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
                        float dashSpeed = 20f;
                        glowIntensity += 0.1f;
                        showAfterImage = true;
                        if (dashOuterTimer % 10 == 0 && NPC.velocity.Y == 0)
                        {
                            NPC.velocity.Y -= Main.rand.NextFloat(3, 5);
                        }
                        NPC.velocity.X = dashOuterDirection * dashSpeed;

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
                            SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/HardRockSlam")
                            {
                                Volume = 0.75f,
                                Pitch = 1f,
                                PitchVariance = 0f,
                            };
                            SoundEngine.PlaySound(stylea, NPC.Center);
                            Main.player[NPC.target].GetModPlayer<AeroPlayer>().ScreenShakePower = 15;
                            SpawnStalactiteProjectiles();
                        }
                    }
                    break;

                case 3:
                    {
                        dashOuterDirection = -dashOuterDirection;
                        dashOuterPhase = 0;
                        dashOuterTimer = 0;
                        dashOuterIteration++;
                        if (dashOuterIteration >= maxDashIterations)
                        {
                            showAfterImage = false;
                            dashOuterPhase = 0;
                            dashOuterTimer = 0;
                            dashOuterIteration = 0;
                            ResetAttack();
                        }
                    }
                    break;
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
            glowIntensity = 0f;
            currentAttack = TumblerAttackState.Idle;
            attackTimer = 0;
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
                ResetAttack();
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
                            ModContent.ProjectileType<LightningLaser>(), NPC.damage / 2, 0f, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), finalLightningPos, Vector2.Zero,
                            ModContent.ProjectileType<TumblerOrb>(), NPC.damage, 0f, Main.myPlayer);
                    }
                    if (arenaElectrocutionTimer >= 60)
                    {
                        ResetAttack();
                        arenaElectrocutionPhase = 0;
                        arenaElectrocutionTimer = 0;
                    }
                    break;
            }
        }

        private void FireCrystalElectrocution(int crystalIndex, Player player)
        {
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
                ModContent.ProjectileType<LightningLaser>(), NPC.damage / 4, 0f, Main.myPlayer);
            Projectile.NewProjectile(NPC.GetSource_FromThis(), shootPos2, Vector2.Zero,
                ModContent.ProjectileType<LightningLaser>(), NPC.damage / 4, 0f, Main.myPlayer);
            Projectile.NewProjectile(NPC.GetSource_FromThis(), shootPos3, Vector2.Zero,
                ModContent.ProjectileType<LightningLaser>(), NPC.damage / 4, 0f, Main.myPlayer);
        }


        private void PhaseThreeSequence()
        {
            //TODO this
        }
    }

    public class LightningBolt : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.light = 0.8f;
        }
    }
}
