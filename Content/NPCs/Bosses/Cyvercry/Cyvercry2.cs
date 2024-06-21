using AerovelenceMod.Common.Globals.Worlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Utilities;
using System;
using Terraria.Graphics.Effects;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry;
using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Projectiles.Other;
using Terraria.DataStructures;
using AerovelenceMod.Content.Buffs.PlayerInflictedDebuffs;
using AerovelenceMod.Content.Projectiles;
using rail;
using static Terraria.ModLoader.PlayerDrawLayer;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using Terraria.GameContent.Bestiary;
using static AerovelenceMod.Common.Utilities.DustBehaviorUtil;
using System.Security.Policy;
using AerovelenceMod.Content.Buffs.FlareDebuffs;
using AerovelenceMod.Content.Buffs;
using Terraria.Map;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry 
{
    [AutoloadBossHead]
    public class Cyvercry2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 3;

            //Immune to fire debuffs because they make him ugly :(
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<EmberFire>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<AuroraFire>()] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            database.Entries.Remove(bestiaryEntry);
        }

        ArmorShaderData dustShader = null;
        ArmorShaderData dustShader2 = null;

        public int ContactDamage = 0;

        public bool Phase2 = false;
        public bool Phase3 = false;

        public override bool CheckActive() { return false; }

        public override void SetDefaults()
        {
            ContactDamage = 60;
            if (isExpert) ContactDamage = 90;
            if (isMaster) ContactDamage = 120;

            NPC.lifeMax = 43110;
            NPC.damage = 105;
            NPC.defense = 30;
            NPC.knockBackResist = 0f;
            NPC.width = 100;
            NPC.height = 100;

            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.HitSound = SoundID.NPCHit4 with { Pitch = -0.5f, PitchVariance = 0.14f };
            NPC.DeathSound = SoundID.NPCDeath14;

            NPC.value = Item.buyPrice(0, 25, 50, 0);
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Cyvercry");
            }
            dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

        }

        float baseDamageMult = 1f;
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.65f * balance * bossAdjustment); //0.75
            NPC.damage = (int)(NPC.damage * 0.75f);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            //Shhhh  | REMEMBER TO AXE THIS AFTER BULLET REWORK
            if (projectile.type == ProjectileID.ChlorophyteBullet)
                projectile.damage = (int)(projectile.damage * 0.7f); 
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;

            DownedWorld.DownedCyvercry = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        #region Drawing
        public List<Vector2> previousPositions;
        public List<float> previousRotations;


        bool hideRegreGlow = false;

        float pinkGlowMaskTimer = 0;
        bool fadeDashing = false;
        float phase3Intensity = 0f;
        float phase3PulseValue = 0f;
        Color phase3PulseColor = Color.White;
        float eyeStarRotation = 0f;
        float eyeStarValue = 0f;
        float fadeDashValue = 0f;
        float justDashValue = 0f;
        float overlayPower = 0f;
        float curveDashTelegraphValue = 0f;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 squashScale = new Vector2(1f, 1f - (0.5f * squashPower)) * NPC.scale;
            float deathRotOffset = Main.rand.NextFloat(-0.115f, 0.115f) * deathOrbScale;
            float adjustedRot = NPC.rotation + deathRotOffset;

            //PinkGlow
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            //TODO make this run only when needed
            Vector2 from = NPC.Center + new Vector2(-98, 0).RotatedBy(NPC.rotation);
            Texture2D Ball = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/flare_4");
            Texture2D Ball2 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/circle_05");

            Main.EntitySpriteDraw(Ball2, from - Main.screenPosition, Ball2.Frame(), Color.DeepPink * 1.5f, NPC.rotation - ((float)Main.timeForVisualEffects * 0.045f), Ball2.Frame().Size() / 2f, (ballScale / 130) * 0.11f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(Ball2, from - Main.screenPosition, Ball2.Frame(), Color.HotPink * 2f, NPC.rotation - ((float)Main.timeForVisualEffects * 0.045f), Ball2.Frame().Size() / 2f, (ballScale / 130) * 0.095f, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(Ball, from - Main.screenPosition + Main.rand.NextVector2Circular(3, 3), Ball.Frame(), Color.Pink * 2f, NPC.rotation - ((float)Main.timeForVisualEffects * 0.065f), Ball.Frame().Size() / 2f, (ballScale / 130) * 0.17f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(Ball, from - Main.screenPosition + Main.rand.NextVector2Circular(3, 3), Ball.Frame(), Color.HotPink * 1f, NPC.rotation + ((float)Main.timeForVisualEffects * 0.04f), Ball.Frame().Size() / 2f, (ballScale / 130) * 0.15f, SpriteEffects.None, 0);

            float glowIntensity = fadeDashing ? 0.25f : 1f;
            if (!hideRegreGlow)
            {
                Texture2D Bloommy = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/RegreGlowCyvercry");
                Main.EntitySpriteDraw(Bloommy, NPC.Center - Main.screenPosition, NPC.frame, Color.White * glowIntensity, adjustedRot, NPC.frame.Size() / 2f, squashScale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(Bloommy, NPC.Center - Main.screenPosition, NPC.frame, Color.White * glowIntensity, adjustedRot, NPC.frame.Size() / 2f, squashScale, SpriteEffects.None, 0);

            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 drawOffset = new Vector2(0, 0).RotatedBy(NPC.rotation);

            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/CyverGlowMaskPink");

            if (!fadeDashing)
            {
                Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition + drawOffset, NPC.frame, Color.White * (0.5f * (float)Math.Sin(pinkGlowMaskTimer / 60f) + 0.5f), adjustedRot, NPC.frame.Size() / 2f, squashScale, SpriteEffects.None, 0);
            }

            float intensity = fadeDashing ? 0.6f : 1;
            //Blue Glow
            Texture2D texture3 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/CyverGlowMaskBlue");
            if (!hideRegreGlow)
                Main.EntitySpriteDraw(texture3, NPC.Center - Main.screenPosition + drawOffset, NPC.frame, Color.White * intensity, adjustedRot, NPC.frame.Size() / 2f, squashScale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture3, NPC.Center - Main.screenPosition + drawOffset, NPC.frame, Color.White with { A = 0 } * justDashValue * 2f, adjustedRot, NPC.frame.Size() / 2f, squashScale, SpriteEffects.None, 0);

            if (overlayPower > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = Main.rand.NextVector2Circular(6f, 6f);
                    Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition + offset, NPC.frame, Color.HotPink with { A = 0 } * overlayPower * 0.25f, NPC.rotation, NPC.frame.Size() / 2f, squashScale, SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(texture3, NPC.Center - Main.screenPosition + offset, NPC.frame, Color.SkyBlue with { A = 0 } * overlayPower * 0.25f, NPC.rotation, NPC.frame.Size() / 2f, squashScale, SpriteEffects.None, 0);
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            //FadeDashBlue
            Texture2D BlueFuzzyGlow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/CyverBlueFuzzy");
            Vector2 drawOrigin2 = new Vector2(BlueFuzzyGlow.Width * 0.5f, BlueFuzzyGlow.Height * 0.5f);
            if (fadeDashing && !hideRegreGlow)
            {
                Color color = Color.SkyBlue;
                float fadeVal = (hideRegreGlow ? (fadeDashValue * 0.65f) : (fadeDashValue));

                if (fadeVal == (0.5f * 0.65f))
                    fadeVal = 0;

                for (int k = 0; k < previousPositions.Count; k++)
                {
                    Vector2 drawPos = previousPositions[k] - Main.screenPosition + drawOrigin2 + new Vector2(0f, NPC.gfxOffY);
                    color = Color.White * (float)(k / 10f);
                    Main.EntitySpriteDraw(BlueFuzzyGlow, drawPos + new Vector2(-40, 0) + Main.rand.NextVector2Circular(2f, 2f), BlueFuzzyGlow.Frame(1, 1, 0, 0), color * fadeVal * 0.5f, previousRotations[k], drawOrigin2, squashScale, SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(BlueFuzzyGlow, drawPos + new Vector2(-40, 0) + Main.rand.NextVector2Circular(2f, 2f), BlueFuzzyGlow.Frame(1, 1, 0, 0), color * fadeVal * 0.5f, previousRotations[k], drawOrigin2, squashScale, SpriteEffects.None, 0);

                }

                //Blue spinny sine layer
                if (!hideRegreGlow)
                {
                    float rot = (float)Main.timeForVisualEffects * 0.12f;
                    float offsetVal = MathF.Sin((float)Main.timeForVisualEffects * 0.14f) * 0.5f;

                    Vector2 v = new Vector2(2f + offsetVal * 2, 0) + Main.rand.NextVector2Circular(2f, 2f);
                    Vector2 trueDrawPos = NPC.position + new Vector2(-40, 0) + Main.rand.NextVector2Circular(2f, 2f) - Main.screenPosition + drawOrigin2;

                    float scale1 = (1f + (MathF.Sin((float)Main.timeForVisualEffects * 0.08f) * 0.025f));

                    Main.spriteBatch.Draw(BlueFuzzyGlow, trueDrawPos + v.RotatedBy(rot - MathHelper.PiOver2), BlueFuzzyGlow.Frame(1, 1, 0, 0), Color.SkyBlue * 0.75f * fadeVal, NPC.rotation, drawOrigin2, squashScale * scale1, 0, 0f);
                    Main.spriteBatch.Draw(BlueFuzzyGlow, trueDrawPos + v.RotatedBy(rot) - Main.screenPosition, BlueFuzzyGlow.Frame(1, 1, 0, 0), Color.DeepSkyBlue * 0.75f * fadeVal, NPC.rotation, drawOrigin2, squashScale * scale1, 0, 0f);
                    Main.spriteBatch.Draw(BlueFuzzyGlow, trueDrawPos + v.RotatedBy(rot + MathHelper.PiOver2), BlueFuzzyGlow.Frame(1, 1, 0, 0), Color.SkyBlue * 0.75f * fadeVal, NPC.rotation, drawOrigin2, squashScale * scale1, 0, 0f);
                    Main.spriteBatch.Draw(BlueFuzzyGlow, trueDrawPos + v.RotatedBy(rot + MathHelper.Pi), BlueFuzzyGlow.Frame(1, 1, 0, 0), Color.DeepSkyBlue * 0.75f * fadeVal, NPC.rotation, drawOrigin2, squashScale * scale1, 0, 0f);

                    //Eye
                    Texture2D justEye = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/CyverEye");
                    Main.EntitySpriteDraw(justEye, NPC.Center - Main.screenPosition + NPC.rotation.ToRotationVector2() * -9f, null, Color.White with { A = 0 } * fadeVal, NPC.rotation, justEye.Size() / 2, squashScale, SpriteEffects.None, 0);
                }

            }
            else //Blue Dash AfterImage
            {
                for (int k = 0; k < previousPositions.Count; k++)
                {
                    Vector2 drawPos = previousPositions[k] - Main.screenPosition + drawOrigin2 + new Vector2(0f, NPC.gfxOffY);
                    Color col = Color.White * (float)(k / 10f);
                    Main.EntitySpriteDraw(BlueFuzzyGlow, drawPos + new Vector2(-40, 0) + Main.rand.NextVector2Circular(2f, 2f), BlueFuzzyGlow.Frame(1, 1, 0, 0), col * justDashValue, previousRotations[k], drawOrigin2, squashScale, SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(BlueFuzzyGlow, drawPos + new Vector2(-40, 0) + Main.rand.NextVector2Circular(2f, 2f), BlueFuzzyGlow.Frame(1, 1, 0, 0), col * justDashValue * 1f, previousRotations[k], drawOrigin2, squashScale * 1.05f, SpriteEffects.None, 0);
                }
                //if (justDashValue > 0 && false) //remove if ugly
                //Main.EntitySpriteDraw(BlueFuzzyGlow, NPC.position + new Vector2(-40, 0) + Main.rand.NextVector2Circular(2f, 2f) - Main.screenPosition + drawOrigin2, BlueFuzzyGlow.Frame(1, 1, 0, 0), Color.White * justDashValue, NPC.rotation, drawOrigin2, squashScale, SpriteEffects.None, 0);

            }

            //Phase 3 eye effect
            if (!fadeDashing)
            {
                Vector2 random = new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3));
                Vector2 random2 = new Vector2(Main.rand.Next(-1, 2), Main.rand.Next(-1, 2));


                Texture2D pixelStar = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Starlight").Value;
                Vector2 scale1 = new Vector2(0.9f, 0.9f);
                Vector2 scale2 = new Vector2(2.2f, 1.3f);

                Color col1 = Color.Lerp(Color.DeepPink * 0.60f, phase3PulseColor, phase3PulseValue);
                Color col2 = Color.Lerp(Color.HotPink * 0.75f, phase3PulseColor, phase3PulseValue);
                Color col3 = phase3PulseColor;

                spriteBatch.Draw(pixelStar, NPC.Center - Main.screenPosition + new Vector2(-70, 0).RotatedBy(NPC.rotation) + random, pixelStar.Frame(1, 1, 0, 0), col1 * phase3Intensity, NPC.rotation + MathHelper.PiOver2, pixelStar.Size() / 2, scale2 * 1.2f, SpriteEffects.None, 0);
                spriteBatch.Draw(pixelStar, NPC.Center - Main.screenPosition + new Vector2(-70, 0).RotatedBy(NPC.rotation) + random, pixelStar.Frame(1, 1, 0, 0), col2 * phase3Intensity, NPC.rotation + MathHelper.PiOver2, pixelStar.Size() / 2, scale2 * 0.7f, SpriteEffects.None, 0);
                spriteBatch.Draw(pixelStar, NPC.Center - Main.screenPosition + new Vector2(-70, 0).RotatedBy(NPC.rotation) + random2, pixelStar.Frame(1, 1, 0, 0), col3 * phase3Intensity, NPC.rotation + MathHelper.PiOver2, pixelStar.Size() / 2, scale2 * 0.3f, SpriteEffects.None, 0);

                spriteBatch.Draw(pixelStar, NPC.Center - Main.screenPosition + new Vector2(-75, 0).RotatedBy(NPC.rotation) + random, pixelStar.Frame(1, 1, 0, 0), col1 * phase3Intensity, NPC.rotation, pixelStar.Size() / 2, scale1 * 1.2f, SpriteEffects.None, 0);
                spriteBatch.Draw(pixelStar, NPC.Center - Main.screenPosition + new Vector2(-75, 0).RotatedBy(NPC.rotation) + random, pixelStar.Frame(1, 1, 0, 0), col2 * phase3Intensity, NPC.rotation, pixelStar.Size() / 2, scale1 * 0.7f, SpriteEffects.None, 0);
                spriteBatch.Draw(pixelStar, NPC.Center - Main.screenPosition + new Vector2(-70, 0).RotatedBy(NPC.rotation) + random2, pixelStar.Frame(1, 1, 0, 0), col3 * phase3Intensity, NPC.rotation, pixelStar.Size() / 2, scale2 * 0.3f, SpriteEffects.None, 0);

                if (phase3PulseColor == Color.White)
                {
                    Texture2D glowStrong = Mod.Assets.Request<Texture2D>("Assets/TrailImages/GlowStar").Value;
                    spriteBatch.Draw(glowStrong, NPC.Center - Main.screenPosition + new Vector2(-70, 0).RotatedBy(NPC.rotation), glowStrong.Frame(1, 1, 0, 0), Color.HotPink * phase3PulseValue, NPC.rotation + MathHelper.PiOver4, glowStrong.Size() / 2, 1.25f, SpriteEffects.None, 0);
                    spriteBatch.Draw(glowStrong, NPC.Center - Main.screenPosition + new Vector2(-70, 0).RotatedBy(NPC.rotation), glowStrong.Frame(1, 1, 0, 0), Color.HotPink * phase3PulseValue, NPC.rotation + MathHelper.PiOver4, glowStrong.Size() / 2, 1f, SpriteEffects.None, 0);

                }

            }

            Texture2D EyeGlow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/CvyerEyeGlow");
            spriteBatch.Draw(EyeGlow, NPC.Center - Main.screenPosition, null, Color.White * phase3PulseValue, NPC.rotation, EyeGlow.Size() / 2, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(EyeGlow, NPC.Center - Main.screenPosition, null, Color.White * phase3PulseValue, NPC.rotation, EyeGlow.Size() / 2, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(EyeGlow, NPC.Center - Main.screenPosition, null, Color.White * phase3PulseValue * 0.65f, NPC.rotation, EyeGlow.Size() / 2, 1.05f, SpriteEffects.None, 0);

            //spriteBatch.Draw(glowStrong, NPC.Center - Main.screenPosition + new Vector2(-70, 0).RotatedBy(NPC.rotation), glowStrong.Frame(1, 1, 0, 0), Color.HotPink * phase3PulseValue, NPC.rotation, glowStrong.Size() / 2, new Vector2(2.2f, 1.3f) * 0.09f, SpriteEffects.None, 0);
            //spriteBatch.Draw(glowStrong, NPC.Center - Main.screenPosition + new Vector2(-70, 0).RotatedBy(NPC.rotation), glowStrong.Frame(1, 1, 0, 0), Color.HotPink * phase3PulseValue, NPC.rotation, glowStrong.Size() / 2, new Vector2(2.2f, 1.3f) * 0.06f, SpriteEffects.None, 0);

            //Eye Star
            Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/TrailImages/GlowStar").Value;
            Vector2 eyeStarDrawPos = NPC.Center - Main.screenPosition + new Vector2(-70, 0).RotatedBy(NPC.rotation);
            for (int al = 0; al < 2; al++)
            {
                Main.spriteBatch.Draw(Flare, eyeStarDrawPos, Flare.Frame(1, 1, 0, 0), Color.SkyBlue, eyeStarRotation, Flare.Size() / 2, eyeStarValue * 2f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare, eyeStarDrawPos, Flare.Frame(1, 1, 0, 0), Color.SkyBlue * 0.4f, eyeStarRotation * -1, Flare.Size() / 2, eyeStarValue * 2.5f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Flare, eyeStarDrawPos, Flare.Frame(1, 1, 0, 0), Color.White, eyeStarRotation * -1, Flare.Size() / 2, eyeStarValue * 0.8f, SpriteEffects.None, 0f);
            }

            //Curve Dash Telegraph
            Texture2D Ray = Mod.Assets.Request<Texture2D>("Assets/MuzzleFlashes/EasyLightray").Value;
            Main.spriteBatch.Draw(Ray, eyeStarDrawPos, Ray.Frame(1, 1, 0, 0), new Color(255, 20, 100) * curveDashTelegraphValue * 0.65f, NPC.rotation + MathF.PI, new Vector2(0, Ray.Height / 2f), 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Ray, eyeStarDrawPos, Ray.Frame(1, 1, 0, 0), Color.DeepPink * curveDashTelegraphValue * 0.35f, NPC.rotation + MathF.PI, new Vector2(0, Ray.Height / 2f), 0.6f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

        }

        float thrusterValue = 0f;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)

        {
            Vector2 squashScale = new Vector2(1f, 1f - (0.5f * squashPower)) * NPC.scale;

            if (firstFrame)
                return false;

            Color pinkToUse = new Color(255, 25, 155);

            //FX for death Anim
            if (DrawDeathOrb)
            {
                Texture2D starTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/scorch_01").Value;
                Texture2D starTex2 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_4").Value;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

                Vector2 starPos = NPC.Center + (NPC.rotation.ToRotationVector2() * -25);
                for (int i = 0; i < 3; i++)
                {
                    Main.spriteBatch.Draw(starTex, starPos - Main.screenPosition, starTex.Frame(1, 1, 0, 0), Color.HotPink, NPC.rotation + MathHelper.ToRadians(-3.2f * timer), starTex.Size() / 2, deathOrbScale * 0.65f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(starTex, starPos - Main.screenPosition, starTex.Frame(1, 1, 0, 0), pinkToUse * 0.5f, NPC.rotation + MathHelper.ToRadians(-6.1f * timer), starTex.Size() / 2, deathOrbScale * 0.65f, SpriteEffects.None, 0);

                    Main.spriteBatch.Draw(starTex2, starPos - Main.screenPosition, starTex2.Frame(1, 1, 0, 0), Color.HotPink, NPC.rotation + MathHelper.ToRadians(timer * 3.7f), starTex2.Size() / 2, deathOrbScale, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(starTex2, starPos - Main.screenPosition, starTex2.Frame(1, 1, 0, 0), pinkToUse, NPC.rotation + MathHelper.ToRadians(timer * -2.52f), starTex2.Size() / 2, deathOrbScale * 0.8f, SpriteEffects.None, 0);

                    Main.spriteBatch.Draw(starTex2, starPos - Main.screenPosition, starTex2.Frame(1, 1, 0, 0), Color.DeepSkyBlue, NPC.rotation + MathHelper.ToRadians(timer * 4.5f) + 1.57f, starTex2.Size() / 2, deathOrbScale, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(starTex2, starPos - Main.screenPosition, starTex2.Frame(1, 1, 0, 0), Color.DeepSkyBlue, NPC.rotation + MathHelper.ToRadians(timer * -3f) + 1.57f, starTex2.Size() / 2, deathOrbScale * 0.8f, SpriteEffects.None, 0);


                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            }

            //Phase 2 Glow
            if (!fadeDashing)
            {
                Texture2D auraTex = Mod.Assets.Request<Texture2D>("Assets/Glorb").Value;
                Vector2 scale = new Vector2(1.2f, 0.8f - (0.4f * squashPower));

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

                for (int i = 0; i <= 4; i++)
                {
                    spriteBatch.Draw(auraTex, NPC.Center + new Vector2(Main.rand.Next(-6, 8), Main.rand.Next(-6, 8)) - Main.screenPosition, auraTex.Frame(1, 1, 0, 0), Color.DeepPink, NPC.rotation, auraTex.Size() / 2, scale * (i * 1.1f), SpriteEffects.None, 0);
                }

                Texture2D textureA = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/CyvercryNoThruster").Value;

                Effect myEffectA = ModContent.Request<Effect>("AerovelenceMod/Effects/CyverAura", AssetRequestMode.ImmediateLoad).Value;

                myEffectA.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3());
                myEffectA.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/VoroNoise").Value);
                myEffectA.Parameters["uTime"].SetValue(timer * 0.2f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffectA, Main.GameViewMatrix.TransformationMatrix);
                myEffectA.CurrentTechnique.Passes[0].Apply();

                Vector2 origin1 = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);

                Main.spriteBatch.Draw(textureA, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, origin1, squashScale * 1.075f, SpriteEffects.None, 0.0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            }

            Vector2 from = NPC.Center - new Vector2(75, 0).RotatedBy(NPC.rotation);

            #region DrawTelegraphLines
            //AZZY LASER DRAWING
            Color a = Color.DeepPink with { A = 0 } * 0.65f; //4
            Color b = Color.DeepSkyBlue with { A = 0 } * 0.3f; //2
            Color c = Color.DeepSkyBlue with { A = 0 } * 0.05f; //0


            if (drawAzzyLaser == 1 && whatAttack != -1) //5 shot
            {
                Player target = Main.player[NPC.target];
                for (int i = -1; i < 2; i++)
                {
                    Vector2 dirToTarget = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                    Utils.DrawLine(spriteBatch, from, NPC.Center + dirToTarget.RotatedBy(MathHelper.ToRadians(17.5f * i)) * 1100, a, b, 1);
                    Utils.DrawLine(spriteBatch, NPC.Center + dirToTarget.RotatedBy(MathHelper.ToRadians(17.5f * i)) * 1100, NPC.Center + dirToTarget.RotatedBy(MathHelper.ToRadians(17.5f * i)) * 1600, b, c, 1.5f);
                }
            }
            else if (drawAzzyLaser == 2 && whatAttack != -1) //4 shot
            {
                Player target = Main.player[NPC.target];

                for (int i = -1; i < 2; i++)
                {
                    Vector2 dirToTarget = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                    if (i != 0)
                    {
                        Utils.DrawLine(spriteBatch, from, NPC.Center + dirToTarget.RotatedBy(MathHelper.ToRadians(10f * i)) * 1100, a, b, 1.5f);
                        Utils.DrawLine(spriteBatch, NPC.Center + dirToTarget.RotatedBy(MathHelper.ToRadians(10f * i)) * 1100, NPC.Center + dirToTarget.RotatedBy(MathHelper.ToRadians(10f * i)) * 1600, b, c, 1.5f);

                    }
                }
            }

            //TRACK DASH DRAWING
            if (whatAttack == 6 && timer < 60)
            {
                Player target = Main.player[NPC.target];

                Vector2 vec2TrackPoint = (trackPoint - NPC.Center).SafeNormalize(Vector2.UnitX);
                Utils.DrawLine(spriteBatch, from, NPC.Center + vec2TrackPoint * 750, Color.DeepSkyBlue, Color.HotPink * 0.5f, 3);

            }

            //GigaBeam
            if ((whatAttack == 5 && timer < 95) || (whatAttack == 13 && timer < 50 && timer > 7))
            {
                float intensity = whatAttack == 13 ? 0.1f : 0.2f;

                Vector2 dirToTarget = NPC.rotation.ToRotationVector2();
                Utils.DrawLine(spriteBatch, from, NPC.Center + dirToTarget * -1900, Color.DeepPink * intensity, Color.HotPink * intensity, 3);


            }
            #endregion

            //Draw After Image
            Vector2 drawOriginAI = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);

            if (!fadeDashing)
            {
                Color col = Color.White * ((NPC.velocity.Length() > 18f || whatAttack == 13 || whatAttack == 15) ? 0.5f : 0.25f); // 0.25f | 0.1f
                col = (NPC.velocity.Length() > 18 ? Color.White * 0.75f : col);
                for (int k = 0; k < previousPositions.Count; k++)
                {
                    Vector2 drawPos = previousPositions[k] - Main.screenPosition + drawOriginAI + new Vector2(0f, NPC.gfxOffY);
                    Color col2 = col * (k / 10f);
                    Main.EntitySpriteDraw((Texture2D)TextureAssets.Npc[NPC.type], drawPos + new Vector2(-40, 0), NPC.frame, col2 with { A = 0 } * 0.5f, previousRotations[k], drawOriginAI, squashScale, SpriteEffects.None, 0);
                }
            }

            //Thruster Flash
            Texture2D texture2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/muzzle_05").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(new Color(0, 255, 255).ToVector3() * (2 - thrusterValue));
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.4f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            if (!fadeDashing)
            {
                spriteBatch.Draw(texture2, NPC.Center - Main.screenPosition + new Vector2(70, 0).RotatedBy(NPC.rotation) + Main.rand.NextVector2Circular(3f, 3f), null, Color.Black, NPC.rotation + MathHelper.PiOver2, texture2.Size() / 2, squashScale * 0.3f, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture2, NPC.Center - Main.screenPosition + new Vector2(70, 0).RotatedBy(NPC.rotation) + Main.rand.NextVector2Circular(3f, 3f), null, Color.Black, NPC.rotation + MathHelper.PiOver2, texture2.Size() / 2, squashScale * 0.3f, SpriteEffects.FlipHorizontally, 0f);

            }


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);

            Texture2D CyverTexture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/CyvercryNoThruster").Value;

            Color extraDrawCol = Color.Lerp(Color.DeepPink, Color.HotPink, deathOrbScale);

            float rotOffset = Main.rand.NextFloat(-0.1f, 0.1f) * deathOrbScale; //.05
            if (!fadeDashing)
                Main.EntitySpriteDraw(CyverTexture, NPC.Center - Main.screenPosition, NPC.frame, drawColor * 1f, NPC.rotation + rotOffset, drawOrigin, squashScale, SpriteEffects.None, 0);

            if (DrawDeathOrb)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 offset = new Vector2(20, 0).RotatedBy(MathHelper.ToRadians((120 * i) + timer * 2f)) + Main.rand.NextVector2Circular(3, 3);
                    Main.EntitySpriteDraw(CyverTexture, NPC.Center - Main.screenPosition + offset, NPC.frame, extraDrawCol with { A = 0 } * Math.Clamp(deathOrbScale, 0, 3), NPC.rotation, drawOrigin, squashScale, SpriteEffects.None, 0);
                    //Main.EntitySpriteDraw(CyverTexture, NPC.Center - Main.screenPosition + (offset * 0.3f), NPC.frame, Color.Black * Math.Clamp(deathOrbScale, 0, 3) * 0.3f, NPC.rotation, drawOrigin, NPC.scale * 0.85f, SpriteEffects.None, 0);

                }
            }

            Texture2D gmask = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/CyverGlowMaskWhiteFuzzy").Value;

            if (overlayPower > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = Main.rand.NextVector2Circular(5f, 5f);
                    Main.EntitySpriteDraw(gmask, NPC.Center - Main.screenPosition + offset, NPC.frame, Color.HotPink with { A = 0 } * overlayPower * 0.15f, NPC.rotation, drawOrigin, squashScale, SpriteEffects.None, 0);
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 5;
                if (NPC.frame.Y > 4 * frameHeight)
                {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y == 3 * frameHeight) //booster frame
                {
                    if ((!fadeDashing || !NPC.hide) && !NPC.hide && whatAttack != 24) //!= BallDash
                    {
                        for (int i = 0; i < 0; i++) //5
                        {
                            Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.Next(5, 10) * -1f;

                            Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + vel * -5, ModContent.DustType<GlowCircleQuadStar>(), vel * -1f,
                                Color.DeepSkyBlue, Main.rand.NextFloat(0.6f, 1.2f), 0.4f, 0f, dustShader);
                            p.noLight = true;
                            p.velocity += NPC.velocity * (0.4f + Main.rand.NextFloat(-0.1f, -0.2f));
                            //p.rotation = Main.rand.NextFloat(6.28f);
                        }
                    }

                    thrusterValue = 0;
                }
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            float newScale = 0.75f;

            //Hide healthbar when Cyver is on top of pink clone
            if (whatAttack == 19 && timer < 120)
                return false;

            return base.DrawHealthBar(hbPosition, ref newScale, ref position);
        }
        #endregion

        public bool hasDoneMusicSync = false;

        float bonusSpinCharge = 120;
        public int whatAttack = -2;
        int timer = 0;
        int advancer = 0;
        int totalTime = 0;
        float accelFloat = 0;

        //Doing this and not Main.masterMode so I can override the difficulty for both testing and in a config
        bool isExpert = true;
        bool isMaster = true;

        bool firstFrame = true;
        float moonStartPos = 0f; //need to store this for easing function

        float squashPower = 0f;
        public override void AI()
        {
            //whatAttack = 24;


            if (firstFrame)
            {
                firstFrame = false;
                SkyManager.Instance.Activate("AerovelenceMod:Cyvercry2");
                moonStartPos = (float)Main.time;

                previousRotations = new List<float>();
                previousPositions = new List<Vector2>();

                if (Main.masterMode)
                {
                    isExpert = true;
                    isMaster = true;
                }
                else if (Main.expertMode)
                {
                    isExpert = true;
                    isMaster = false;
                }
                else
                {
                    isExpert = false;
                    isMaster = false;
                }

                //Override difficulty if set in the config
                string DifOverride = ModContent.GetInstance<AeroClientConfig>().CyvercryAIOverride;
                if (DifOverride == "Master")
                {
                    Main.NewText("Difficulty Override: Master", Color.HotPink);
                    isExpert = true;
                    isMaster = true;
                }
                else if (DifOverride == "Expert")
                {
                    Main.NewText("Difficulty Override: Expert", Color.HotPink);
                    isExpert = true;
                    isMaster = false;
                }
                else if (DifOverride == "Normal")
                {
                    Main.NewText("Difficulty Override: Normal", Color.HotPink);
                    isExpert = false;
                    isMaster = false;
                }
            }

            //These should always be true due to bad code
            spammingLaser = true;
            Phase2 = true;
            Phase3 = true;


            ///Main.time = 12600 + 3598; | midnight - 2 cause we don't want to keep activating stuff that happens at midnight probably
            Main.time = MathHelper.Lerp(moonStartPos, 12600 + 3598, Easings.easeInOutQuad(Math.Clamp((totalTime / 90f), 0f, 1f)));
            NPC.damage = 0;

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Player myPlayer = Main.player[NPC.target];

            if (myPlayer.active == false || myPlayer.dead == true)
            {
                NPC.active = false;
                OnDespawnCleanup(myPlayer);
            }

            myPlayer.AddBuff(ModContent.BuffType<FearsomeFoe>(), 1);

            switch (whatAttack)
            {
                case -1:
                    DeathAnimation(myPlayer);
                    break;
                case -2:
                    IntroAnimation(myPlayer);
                    break;
                case -3:
                    PhaseTransition(myPlayer);
                    break;
                case -4:
                    SitStill(myPlayer);
                    break;

                case 1:
                    IdleLaser(myPlayer);
                    break;
                case 2:
                    IdleDash(myPlayer);
                    break;
                case 3:
                    Spin(myPlayer);
                    break;
                case 4:
                    AzzyLaser(myPlayer);
                    break;
                case 5:
                    GigaBeam(myPlayer);
                    break;
                case 6:
                    Clones(myPlayer);
                    break;
                case 7:
                    ChaseDash(myPlayer);
                    break;
                case 8:
                    Bots(myPlayer);
                    break;
                case 9:
                    WrapDash(myPlayer);
                    break;
                case 10:
                    ClonesP3(myPlayer);
                    break;
                case 11:
                    ExplodeBallSpam(myPlayer);
                    break;
                case 12:
                    FocusLaser(myPlayer);
                    break;
                case 13:
                    GigaLaserSpam(myPlayer);
                    break;
                case 14:
                    SweepLaser(myPlayer);
                    break;
                case 15:
                    thinkOfANameLater(myPlayer);
                    break;
                case 16:
                    SpinPhase3(myPlayer);
                    break;
                case 17:
                    CurvedDash(myPlayer);
                    break;
                case 18:
                    CCPhantomDash(myPlayer);
                    break;
                case 19:
                    PinkCloneP3(myPlayer);
                    break;
                case 20:
                    FunnelLaser(myPlayer);
                    break;
                case 21:
                    PhantomDash1(myPlayer);
                    break;
                case 22:
                    NewPhantomDash1(myPlayer);
                    break;
                case 23:
                    NewBots(myPlayer);
                    break;
                case 24:
                    BallDash(myPlayer);
                    break;
                case 33:
                    EyeSword(myPlayer);
                    break;
            }

            int listLength = 10;
            previousRotations.Add(NPC.rotation);
            previousPositions.Add(NPC.position);
            if (previousRotations.Count > listLength) { previousRotations.RemoveAt(0); }
            if (previousPositions.Count > listLength) { previousPositions.RemoveAt(0); }

            phase3Intensity = 0.8f + (0.25f * (float)Math.Sin(pinkGlowMaskTimer / 25f) + 0.25f);
            phase3PulseValue = Math.Clamp(MathHelper.Lerp(phase3PulseValue, -0.75f, 0.05f), 0f, 1f);

            eyeStarValue = Math.Clamp(MathHelper.Lerp(eyeStarValue, -0.2f, 0.06f), 0, 1f);
            eyeStarRotation += 0.08f;

            thrusterValue = Math.Clamp(MathHelper.Lerp(thrusterValue, 3, 0.06f), 0, 2);
            fadeDashValue = Math.Clamp(MathHelper.Lerp(fadeDashValue, 0.3f, 0.06f), 0.5f, 1);

            justDashValue = Math.Clamp(MathHelper.Lerp(justDashValue, -0.2f, 0.06f), 0f, 1f);
            extraBoost = Math.Clamp(MathHelper.Lerp(extraBoost, -0.4f, 0.2f), 0f, 3f); //1f cap prev
            overlayPower = Math.Clamp(MathHelper.Lerp(overlayPower, -0.25f, 0.03f), 0f, 4f);
            whiteBackgroundPower = Math.Clamp(MathHelper.Lerp(whiteBackgroundPower, -0.1f, 0.02f), 0f, 1f);
            squashPower = Math.Clamp(MathHelper.Lerp(squashPower, -0.5f, 0.1f), 0f, 2f);
            lineBonusSpeed = Math.Clamp(MathHelper.Lerp(lineBonusSpeed, -0.15f, 0.025f), 0f, 10f);
            swordDangerTelegraphPower = Math.Clamp(MathHelper.Lerp(swordDangerTelegraphPower, -0.25f, 0.02f), 0f, 1f);

            pinkGlowMaskTimer++;
            totalTime++;
        }

        #region Attacks

        #region Laser Attacks

        float startingAngBonus = 0;
        float startingQuadrant = 3;
        float dashQuadrant = 1;
        bool advanceNegative = false;
        public void IdleLaser(Player myPlayer)
        {
            int extraTime = hasDoneMusicSync ? 0 : 19;

            //Have a bit more delay on shots when it is first cycle
            int extraShotDelay = hasDoneMusicSync ? 0 : 3;

            NPC.hide = false;
            NPC.dontTakeDamage = false;
            //startingQuadrant = 2;
            if (timer == 0)
            {
                switch (startingQuadrant)
                {
                    case 1:
                        startingAngBonus = -40;
                        dashQuadrant = 4;
                        advanceNegative = false;
                        break;
                    case 2:
                        startingAngBonus = -140;
                        dashQuadrant = 3;
                        advanceNegative = true;
                        break;
                    case 3:
                        startingAngBonus = 140;
                        dashQuadrant = 2;
                        advanceNegative = false;
                        break;
                    case 4:
                        startingAngBonus = 40;
                        dashQuadrant = 1;
                        advanceNegative = true;
                        break;
                }
            }

            int shotDelay = isExpert ? 25 : 30;
            float shotSpeed = isMaster ? 8 : 6;

            shotDelay += extraShotDelay;

            Vector2 goalPoint = new Vector2(550, 0).RotatedBy(MathHelper.ToRadians(advancer * 0.2f + startingAngBonus)); //advancer * 0.4

            Vector2 move = (goalPoint + myPlayer.Center) - NPC.Center;

            float scalespeed = 0.6f * 2f;

            NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
            NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;

            NPC.rotation = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();


            if (timer % shotDelay == 0 && timer > 40)
            {
                SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_158") with { Pitch = .56f, PitchVariance = .27f, MaxInstances = -1 };
                SoundEngine.PlaySound(stylea, NPC.Center);
                SoundEngine.PlaySound(stylea, NPC.Center);

                SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .33f, Pitch = .73f, PitchVariance = .27f, MaxInstances = -1 };
                SoundEngine.PlaySound(styleb, NPC.Center);
                SoundEngine.PlaySound(styleb, NPC.Center);

                SoundStyle stylec = new SoundStyle("Terraria/Sounds/Item_67") with { Pitch = .38f, Volume = 0.7f, MaxInstances = -1 }; //1f
                SoundEngine.PlaySound(stylec, NPC.Center);

                Dust a = GlowDustHelper.DrawGlowDustPerfect(NPC.Center - new Vector2(70, 0).RotatedBy(NPC.rotation), ModContent.DustType<GlowCircleQuadStar>(),
                (NPC.rotation + MathHelper.Pi).ToRotationVector2() * 12, Color.HotPink, 0.7f, 0.2f, 0f, dustShader2);

                phase3PulseValue = 1;

                /*
                for (int i = 0; i < 360; i += 20)
                {
                    Vector2 circular = new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(i));
                    Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center - new Vector2(70, 0).RotatedBy(NPC.rotation), ModContent.DustType<LineGlow>(),
                        circular * 2, Color.DeepPink, Main.rand.NextFloat(0.2f, 0.2f), 0.2f, 0f, dustShader2);
                    p.fadeIn = 50;

                }
                */

                /*
                int pulseIndex = Projectile.NewProjectile(null, NPC.Center - new Vector2(80, 0).RotatedBy(NPC.rotation), (NPC.rotation + MathHelper.Pi).ToRotationVector2() * 3, ModContent.ProjectileType<CyverLaserPulse>(), 0, 0, Main.myPlayer);
                Projectile pulse = Main.projectile[pulseIndex];
                if (pulse.ModProjectile is CyverLaserPulse pulse2)
                {
                    pulse2.ParentIndex = NPC.whoAmI;
                }
                */
                //ShotDust();

                FireLaser(ModContent.ProjectileType<StretchLaser>(), 1.5f, 8f, damage: GetDamage("IdleShot"));
            }

            if (timer == 220 + extraTime)
            {
                timer = -1;
                whatAttack = 2;
                //whatAttack = -3;
                advancer = 0;
            }
            if (advanceNegative)
                advancer = (timer > 140 ? --advancer : advancer - 2);
            else
                advancer = (timer > 140 ? ++advancer : advancer + 2);


            timer++;
        }

        float spinTimer = 0;
        bool spammingLaser = false;
        float ballScale = 0;
        public void Spin(Player myPlayer)
        {
            if (timer == 20)
            {
                int retIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CyverReticle>(), 0, 0, myPlayer.whoAmI);
                Projectile Reticle = Main.projectile[retIndex];
                if (Reticle.ModProjectile is CyverReticle target)
                {
                    target.ParentIndex = NPC.whoAmI;
                    //Main.instance.DrawCacheProjsBehindNPCs.Add(sawIndex);
                }

                Vector2 from = NPC.Center + new Vector2(-102, 0).RotatedBy(NPC.rotation);

                for (int i = 0; i < 360; i += 20)
                {
                    Vector2 circular = new Vector2(32, 0).RotatedBy(MathHelper.ToRadians(i));
                    //circular.X *= 0.6f;
                    circular = circular.RotatedBy(NPC.rotation);
                    Vector2 dustVelo = -circular * 0.1f;

                    Dust b = GlowDustHelper.DrawGlowDustPerfect(from + circular, ModContent.DustType<GlowCircleDust>(), Vector2.Zero, Color.DeepPink, 0.3f, 0.6f, 0f, dustShader2);
                }

                SoundStyle style = new SoundStyle("Terraria/Sounds/Zombie_68");
                SoundEngine.PlaySound(style, NPC.Center);
            }
            if (timer <= 150 + bonusSpinCharge)
            {
                ballScale += 1;
                if (timer >= 20)
                {
                    Vector2 from = NPC.Center + new Vector2(-102, 0).RotatedBy(NPC.rotation);
                    for (int j = 0; j < 3; j++)
                    {
                        Vector2 circular = new Vector2(32, 0).RotatedBy(MathHelper.ToRadians(j * 120 + timer * 4));
                        //circular.X *= 0.6f;
                        circular = circular.RotatedBy(NPC.rotation);
                        Vector2 dustVelo = -circular * 0.09f;
                        Dust b = GlowDustHelper.DrawGlowDustPerfect(from + circular, ModContent.DustType<GlowCircleQuadStar>(), Vector2.Zero, Color.DeepPink, 0.3f, 0.6f, 0f, dustShader2);
                    }
                }

                NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * (Phase2 ? (isMaster ? 6.5f : 5.5f) : 4);
            }
            else
                NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 0.5f;

            NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;

            if (timer > 150 + bonusSpinCharge)
            {
                if (timer == 151 + bonusSpinCharge)
                {
                    ballScale = 0;
                    Vector2 from = NPC.Center + new Vector2(-102, 0).RotatedBy(NPC.rotation);

                    //SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .12f, Pitch = .8f, MaxInstances = 1 };
                    //SoundEngine.PlaySound(style);

                    //SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = 1.5f, PitchVariance = .47f, MaxInstances = 0, Volume = 0.3f };
                    //SoundEngine.PlaySound(style);

                    //SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .12f, Pitch = .4f, PitchVariance = .2f, MaxInstances = 1 };
                    //SoundEngine.PlaySound(style2);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), from, Vector2.Zero, ModContent.ProjectileType<PinkExplosion>(), 0, 0, Main.myPlayer);

                    /*
                    for (int i = 0; i < 360; i += 20)
                    {
                        Vector2 circular = new Vector2(32, 0).RotatedBy(MathHelper.ToRadians(i));
                        //circular.X *= 0.6f;
                        circular = circular.RotatedBy(NPC.rotation);
                        Vector2 dustVelo = -circular * 0.1f;

                        Dust b = GlowDustHelper.DrawGlowDustPerfect(from + circular, ModContent.DustType<GlowCircleDust>(), Vector2.Zero, Color.DeepPink, 0.5f, 0.6f, 0f, dustShader2);
                    }
                    */
                }

                if (Phase2 && timer % 100 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-102, 0).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<LaserExplosionBall>(), 0, 0, Main.myPlayer);
                }

                spammingLaser = true;
                if (timer % 5 == 0)
                {
                    SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_158") with { Pitch = .56f, PitchVariance = .27f, };
                    SoundEngine.PlaySound(stylea, NPC.Center);
                    SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .33f, Pitch = .73f, PitchVariance = .27f, };
                    SoundEngine.PlaySound(styleb, NPC.Center);
                    FireLaser(ModContent.ProjectileType<CyverLaser>(), 13f, 0.7f);
                    //FireLaser(ModContent.ProjectileType<StretchLaser>(), 2f, 5f);

                    phase3PulseValue = 1f;

                }
            }
            if (timer == 400 + bonusSpinCharge)
            {
                bonusSpinCharge = 0;
                ballScale = 0;
                spammingLaser = false;
                timer = -1;

                if (Phase2)
                    SetNextAttack("Dash");
                else
                    SetNextAttack("Summon");
                //whatAttack = 6; //6
            }
            timer++;
        }

        int currentShot = 0;
        Vector2 goalLocation = Vector2.Zero;

        int drawAzzyLaser = 0;
        public void AzzyLaser(Player myPlayer)
        {
            float bonusShots = isMaster ? 3 : isExpert ? 0 : -1;

            float shotDelay = MathHelper.Clamp(currentShot, 0, Phase3 ? 11f : 13f) * (Phase3 ? 6f : 5f);

            if (currentShot == 0)
            {
                goalLocation = new Vector2(600, 0).RotatedByRandom(6.28);
                currentShot++;
            }

            //Large delay on first shot so idle dash stuff doesn't overlap
            if (currentShot == 1)
                shotDelay = -15;

            if (timer < 1000) //????
            {
                if (timer > 5)
                {
                    if (currentShot % 2 == 0)
                        drawAzzyLaser = 1;
                    else
                        drawAzzyLaser = 2;
                }

                //go there

                if (timer == 35 + 45 - (shotDelay))
                    NPC.velocity = Vector2.Zero;

                if (timer < 35 + 45 - (shotDelay))
                {
                    Vector2 move = (goalLocation + myPlayer.Center) - NPC.Center;

                    float scalespeed = 0.6f * 3f;//2

                    NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
                    NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;

                }

                NPC.rotation = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();

                if (timer == 40 + 45 - (shotDelay) || timer == 50 + 45 - (shotDelay) || timer == 60 + 45 - (shotDelay))
                {
                    Vector2 from = NPC.Center - new Vector2(75, 0).RotatedBy(NPC.rotation);

                    if (currentShot % 2 == 0)
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            Vector2 velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 13;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), from, velocity.RotatedBy(MathHelper.ToRadians(18.5f * i)), ModContent.ProjectileType<CyverLaser>(), GetDamage("AzzyLaser"), 2, Main.myPlayer);

                        }
                    }
                    else
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            Vector2 velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 13;
                            if (i != 0)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), from, velocity.RotatedBy(MathHelper.ToRadians(11 * i)), ModContent.ProjectileType<CyverLaser>(), GetDamage("AzzyLaser"), 2, Main.myPlayer);

                        }
                    }
                    ShotDust();

                    SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_158") with { Pitch = .56f, PitchVariance = .27f, MaxInstances = -1, Volume = 0.65f };
                    SoundEngine.PlaySound(stylea, NPC.Center);
                    SoundEngine.PlaySound(stylea, NPC.Center);

                    SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .2f, Pitch = .73f, PitchVariance = .27f, MaxInstances = -1 };
                    SoundEngine.PlaySound(styleb, NPC.Center);
                    SoundEngine.PlaySound(styleb, NPC.Center);

                    SoundStyle stylec = new SoundStyle("Terraria/Sounds/Item_67") with { Pitch = .38f, Volume = 0.45f }; //1f
                    SoundEngine.PlaySound(stylec, NPC.Center);

                    phase3PulseValue = 0.5f;
                    eyeStarValue = 0.5f;

                }

            }

            if (timer == 60 + 45 - (shotDelay))
            {
                timer = -1;
                currentShot++;

                previousPositions.Clear();
                previousRotations.Clear();

                float rotAmount = currentShot < 14 ? 1f : 0.2f;
                goalLocation = goalLocation.RotatedBy(rotAmount * (Main.rand.NextBool() ? -1 : 1));


                drawAzzyLaser = 0;
                if (currentShot == 14 + bonusShots)
                {
                    currentShot = 0;
                    whatAttack = 5;
                }
            }

            //Cyver goes to a random point on circle
            //Shoots v v or v  v  v
            //Point gets rotated by random 60 degrees
            //ShootsOpposite
            timer++;
        }

        public void GigaBeam(Player myPlayer)
        {
            if (timer < 75)
            {
                if (timer < 30)
                {
                    Vector2 move = (goalLocation + myPlayer.Center) - NPC.Center;

                    float scalespeed = 0.6f * 3f;//2

                    NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
                    NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;
                }
                else
                {
                    NPC.velocity = Vector2.Zero;
                }

                if (timer == 0)
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/Zombie_66") with { PitchVariance = 0.1f, Pitch = 0f };
                    SoundEngine.PlaySound(style, NPC.Center);

                    SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorCharge") with { Volume = .4f, Pitch = 0.8f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    int telegraphLine = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TelegraphLineCyver>(), 0, 0);
                    Main.projectile[telegraphLine].timeLeft = 85;
                    telegraphLineProj = Main.projectile[telegraphLine];
                    if (Main.projectile[telegraphLine].ModProjectile is TelegraphLineCyver line)
                    {
                        line.uColorIntensity = 0.5f;
                        line.NPCTetheredTo = NPC;
                    }
                }
                float scale = MathHelper.Clamp(ballScale / 2, 0, 100);
                Vector2 ballSpawnVec = NPC.Center + new Vector2(-98, 0).RotatedBy(NPC.rotation);
                Vector2 spawnVecOutSet = Main.rand.NextVector2CircularEdge(scale / 2, scale / 2);
                float dustBonusScale = (timer / 60f) * 0.1f;
                float dustBonusVel = -2.5f * (1f + (timer / 60f));

                Dust p = Dust.NewDustPerfect(ballSpawnVec + spawnVecOutSet, ModContent.DustType<GlowStrong>(), spawnVecOutSet.SafeNormalize(Vector2.UnitX) * dustBonusVel, 2,
                    newColor: Color.HotPink, Scale: Main.rand.NextFloat(0.15f, 0.2f) + dustBonusScale);

                ballScale += 4;
                NPC.rotation = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();
            }

            if (timer >= 85)
            {
                NPC.velocity.X *= 0.9f;
                NPC.velocity.Y *= 0.9f;
            }

            if (timer == 85)
            {
                NPC.velocity = Vector2.Zero;


                NPC.Center -= NPC.rotation.ToRotationVector2() * -30;
                myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 70;

                ballScale = 0;
                bigShotTimer = 10;
                Vector2 from = NPC.Center - new Vector2(80, 0).RotatedBy(NPC.rotation);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), from, NPC.rotation.ToRotationVector2() * -1, ModContent.ProjectileType<CyverHyperBeam>(), GetDamage("SoloGiga"), 2, Main.myPlayer);

                //SoundStyle style32 = new SoundStyle("AerovelenceMod/Sounds/Effects/laser_fire") with { Volume = 0.5f, Pitch = 0f, MaxInstances = -1, PitchVariance = 0.1f };
                //SoundEngine.PlaySound(style32, NPC.Center);

                int afg = Projectile.NewProjectile(null, NPC.Center - new Vector2(120, 0).RotatedBy(NPC.rotation), NPC.velocity.SafeNormalize(Vector2.UnitX) * 1f, ModContent.ProjectileType<DistortProj>(), 0, 0);
                Main.projectile[afg].rotation = Main.rand.NextFloat(6.28f);
                Main.projectile[afg].timeLeft = 6;
                Main.projectile[afg].scale = 0.5f;

                if (Main.projectile[afg].ModProjectile is DistortProj distort)
                {
                    distort.tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/MagmaBall");
                    distort.implode = false;
                    distort.scale = 0.65f;
                }

                Common.Systems.FlashSystem.SetCAFlashEffect(0.45f, 30, 1f, 0.5f, false, true); //0.25f, 30 1f, 0.5f
                whiteBackgroundPower = 0.05f;
                lineBonusSpeed = 3f;

                eyeStarValue = 1;
                Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.Pi);
                NPC.velocity = vel * -12f; //-10f

                backVelRot = vel.ToRotation();
                backVelVal = -12f;

                phase3PulseValue = 1;
                extraBoost = 1;
                squashPower = 1.25f;
                overlayPower = 1.5f;
                justDashValue = 1;
            }

            if (timer == 130)
            {
                timer = -1;

                if (Phase3)
                    whatAttack = 13;
                else
                    SetNextAttack("Dash");
            }

            bigShotTimer--;
            timer++;
        }

        public void ExplodeBallSpam(Player myPlayer)
        {
            //NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 4f;

            if (timer % 20 == 0 && timer != 0 && timer < 100)
            {
                ShotDust();
                ShotDust();

                Vector2 from = NPC.Center - new Vector2(96, 0).RotatedBy(NPC.rotation);
                int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), from, NPC.rotation.ToRotationVector2() * -40, ModContent.ProjectileType<LaserExplosionBall>(),
                    ContactDamage / 10, 2, Main.myPlayer);
                Projectile p = Main.projectile[a];
                p.timeLeft = 50;

                if (p.ModProjectile is LaserExplosionBall ball)
                {
                    ball.numberOfLasers = 8;
                    //ball.projType = ModContent.ProjectileType<StretchLaser>();
                    ball.vel = 1f;
                }
                //justShotEBall = 10;
                NPC.Center -= NPC.rotation.ToRotationVector2() * -10;
            }
            else if (timer >= 100)
            {
                NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 4f;
                NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;
            }
            else
            {
                NPC.rotation = timer * 0.025f;

            }
            //justShotEBall = Math.Clamp(justShotEBall *= 0.9f, -10, 100);
            if (timer == 200)
                timer = -1;
            timer++;
        }

        public void FocusLaser(Player myPlayer)
        {
            //Move towards Player and charge dust
            //Try to rotate
            //Spawn laser

            if (timer < 60)
            {
                float scale = MathHelper.Clamp(ballScale / 2, 0, 100);
                Vector2 ballSpawnVec = NPC.Center + new Vector2(-98, 0).RotatedBy(NPC.rotation);
                Vector2 spawnVecOutSet = Main.rand.NextVector2CircularEdge(scale / 2, scale / 2);
                GlowDustHelper.DrawGlowDustPerfect(ballSpawnVec + spawnVecOutSet, ModContent.DustType<GlowCircleQuadStar>(), spawnVecOutSet.SafeNormalize(Vector2.UnitX) * -2, Color.HotPink, Main.rand.NextFloat(.2f, .3f), dustShader2);
                ballScale += 6;

                float goalRot = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();
                NPC.rotation = Utils.AngleLerp(NPC.rotation, goalRot, 0.0f);
                //NPC.rotation = Utils.AngleLerp(NPC.rotation, goalRot, 0.9f);


            }
            else if (timer >= 60)
            {
                lineBonusSpeed = 1f;

                if (timer == 60)
                {
                    int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.rotation.ToRotationVector2() * -96, Vector2.Zero, ModContent.ProjectileType<FocusedLaser>(), 15, 2);
                    if (Main.projectile[a].ModProjectile is FocusedLaser laser)
                    {
                        laser.parentIndex = NPC.whoAmI;
                    }
                }

                if (timer % 60 == 0 && timer != 60)
                {
                    SoundStyle stylees = new SoundStyle("Terraria/Sounds/Item_117") with { Pitch = .72f, PitchVariance = .41f, Volume = 0.5f };
                    SoundEngine.PlaySound(stylees, NPC.Center);

                    float shotRot = Utils.AngleLerp(NPC.rotation, MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation(), 0.2f);
                    Vector2 from = NPC.Center - new Vector2(96, 0).RotatedBy(NPC.rotation);
                    int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), from, shotRot.ToRotationVector2() * -40, ModContent.ProjectileType<LaserExplosionBall>(),
                        ContactDamage / 10, 2);
                    Projectile p = Main.projectile[a];
                    p.timeLeft = 45;

                    if (p.ModProjectile is LaserExplosionBall ball)
                    {
                        ball.numberOfLasers = 8;
                        ball.projType = ModContent.ProjectileType<StretchLaser>();
                        ball.vel = 1f;
                    }
                }

                if (timer % 15 == 0)
                {
                    SoundStyle stylees = new SoundStyle("Terraria/Sounds/Item_117") with { Pitch = .32f, PitchVariance = .71f, Volume = 0.2f, MaxInstances = -1 };
                    SoundEngine.PlaySound(stylees, NPC.Center);
                    //SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/movingshield_sound") { MaxInstances = 1, Pitch = 0.2f, Volume = 1f };
                    //SoundEngine.PlaySound(style2, NPC.Center);
                    //SoundEngine.PlaySound(style2, NPC.Center);
                    //SoundEngine.PlaySound(style2, NPC.Center);
                }

                float goalRot = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();
                //NPC.rotation = Utils.AngleLerp(NPC.rotation, goalRot, 0.02f + Math.Abs((float)Math.Sin(timer / 60) * 0.01f));

                float intensity = (Math.Abs(NPC.rotation - goalRot) > 0.28f && Math.Abs(NPC.rotation - goalRot) < 6f ? 0.025f : 0.03f);
                NPC.rotation = Utils.AngleLerp(NPC.rotation, goalRot, intensity);

                NPC.velocity = NPC.rotation.ToRotationVector2() * -7.5f;
            }

            timer++;
        }

        int gigaLaserCount = 0;
        Vector2 originalPosForLerp = Vector2.Zero;
        float backVelVal = 0f;
        float backVelRot = 0f;
        public void GigaLaserSpam(Player myPlayer)
        {
            int shotCount = isExpert ? 7 : 4;

            if (advancer == 0)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Zombie_66");
                SoundEngine.PlaySound(style, NPC.Center);
                originalPosForLerp = NPC.Center;

                goalLocation = (NPC.Center - myPlayer.Center).SafeNormalize(Vector2.UnitX) * 530f;
                int side = Main.rand.NextBool() ? -1 : 1;
                float rot = Main.rand.NextFloat(0.85f, 1.4f) * side;
                goalLocation = goalLocation.RotatedBy(rot * 1.5f);

                goalLocation = new Vector2(530, 0).RotatedByRandom(6.28f);
                advancer++;

                int telegraphLine = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TelegraphLineCyver>(), 0, 0);
                Main.projectile[telegraphLine].timeLeft = 45;
                telegraphLineProj = Main.projectile[telegraphLine];
                if (Main.projectile[telegraphLine].ModProjectile is TelegraphLineCyver line)
                {
                    line.uColorIntensity = 0.75f;
                    line.NPCTetheredTo = NPC;
                }
            }

            if (timer > 5 && timer < 45)
            {
                overlayPower = 0.15f;
                justDashValue = 0.25f;
            }
            if (timer < 40) //35
            {
                if (timer > 5)
                {
                    /*
                    Vector2 move = (goalLocation + myPlayer.Center) - NPC.Center;

                    float scalespeed = 0.6f * 3f;

                    NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
                    NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;
                    */
                    NPC.Center = Vector2.Lerp(originalPosForLerp, myPlayer.Center + goalLocation, Easings.easeOutQuint(Math.Clamp((timer - 5f) / 55f, 0f, 1f)));
                }
                else
                {
                    NPC.velocity = Vector2.Zero;
                }


                if (timer == 5)
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/Zombie_66") with { PitchVariance = 0.1f, Pitch = 0f };
                    SoundEngine.PlaySound(style, NPC.Center);

                    SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorCharge") with { Volume = .4f, Pitch = 0.8f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style2, NPC.Center);
                }
                float scale = MathHelper.Clamp(ballScale / 2, 0, 100);
                Vector2 ballSpawnVec = NPC.Center + new Vector2(-98, 0).RotatedBy(NPC.rotation);
                Vector2 spawnVecOutSet = Main.rand.NextVector2CircularEdge(scale / 2, scale / 2);
                float dustBonusScale = (timer / 40f) * 0.15f;
                float dustBonusVel = -2.5f * (1f + (timer / 40f));
                //GlowDustHelper.DrawGlowDustPerfect(ballSpawnVec + spawnVecOutSet, ModContent.DustType<GlowCircleFlare>(), spawnVecOutSet.SafeNormalize(Vector2.UnitX) * dustBonusVel, Color.HotPink, Main.rand.NextFloat(.3f, .4f) + dustBonusScale, 0.6f, 0f, dustShader2);

                Dust p = Dust.NewDustPerfect(ballSpawnVec + spawnVecOutSet, ModContent.DustType<GlowStrong>(), spawnVecOutSet.SafeNormalize(Vector2.UnitX) * dustBonusVel, 2,
                    newColor: Color.HotPink, Scale: Main.rand.NextFloat(0.15f, 0.2f) + dustBonusScale);

                //p.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                //rotPower: 0.05f, timeBeforeSlow: 0, postSlowPower: 0.94f, velToBeginShrink: 3f, fadePower: 0.9f, shouldFadeColor: false);

                ballScale += 10;

                float goalRot = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();
                NPC.rotation = Utils.AngleTowards(NPC.rotation, goalRot, 0.4f);
            }
            else ballScale += 5;

            if (timer > 45)
            {
                backVelVal *= 0.87f;
                NPC.velocity = backVelRot.ToRotationVector2() * backVelVal;

                float bgLineProgress = (timer - 45f) / 10f;
                if (timer < 55)
                    extraBoost = 1f - (0.5f * bgLineProgress);
            }

            if (timer == 45)
            {
                NPC.velocity = Vector2.Zero;


                NPC.Center -= NPC.rotation.ToRotationVector2() * -30;
                myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 60; //70

                ballScale = 0;
                bigShotTimer = 10;
                Vector2 from = NPC.Center - new Vector2(80, 0).RotatedBy(NPC.rotation);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), from, NPC.rotation.ToRotationVector2() * -1, ModContent.ProjectileType<CyverHyperBeam>(), GetDamage("GigaSpam"), 2, Main.myPlayer);

                int afg = Projectile.NewProjectile(null, NPC.Center - new Vector2(120, 0).RotatedBy(NPC.rotation), NPC.velocity.SafeNormalize(Vector2.UnitX) * 1f, ModContent.ProjectileType<DistortProj>(), 0, 0);
                Main.projectile[afg].rotation = Main.rand.NextFloat(6.28f);
                Main.projectile[afg].timeLeft = 6;
                Main.projectile[afg].scale = 0.5f;

                if (Main.projectile[afg].ModProjectile is DistortProj distort)
                {
                    distort.tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/MagmaBall");
                    distort.implode = false;
                    distort.scale = 0.65f;
                }

                Common.Systems.FlashSystem.SetCAFlashEffect(0.35f, 30, 1f, 0.5f, false, true); //0.45 intensity
                whiteBackgroundPower = 0.05f;
                lineBonusSpeed = 3f;

                eyeStarValue = 1;
                Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.Pi);
                NPC.velocity = vel * -12f; 

                backVelRot = vel.ToRotation();
                backVelVal = -12f;

                phase3PulseValue = 1;
                extraBoost = 1;
                squashPower = 1.25f;
                overlayPower = 1.5f;
                justDashValue = 1;
            }

            if (timer == 65)
            {
                if (gigaLaserCount == 7)
                {
                    timer = -1;
                    advancer = 0;
                    gigaLaserCount = 0;
                    ballScale = 0;

                    SetNextAttack("Dash");
                }
                else
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/Zombie_66");
                    SoundEngine.PlaySound(style, NPC.Center);
                    timer = 4;
                    int side = Main.rand.NextBool() ? -1 : 1;
                    float rot = Main.rand.NextFloat(0.85f, 1.4f) * side;
                    goalLocation = goalLocation.RotatedBy(rot * 1.5f);

                    originalPosForLerp = NPC.Center;

                    previousPositions.Clear();
                    previousRotations.Clear();


                    int telegraphLine = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TelegraphLineCyver>(), 0, 0);
                    Main.projectile[telegraphLine].timeLeft = 45;
                    telegraphLineProj = Main.projectile[telegraphLine];
                    if (Main.projectile[telegraphLine].ModProjectile is TelegraphLineCyver line)
                    {
                        line.uColorIntensity = 0.75f;
                        line.NPCTetheredTo = NPC;
                    }
                }

                gigaLaserCount++;
            }

            bigShotTimer--;
            timer++;
        }

        int sweepLaserReps = 0;
        public bool sweepLaserDir = false;
        public float eyeFlareSize = 0;
        public void SweepLaser(Player myPlayer)
        {
            //Move towards player and telegraph
            if (advancer == 0)
            {
                Vector2 vecToPlayer = (NPC.Center - myPlayer.Center).SafeNormalize(Vector2.UnitX) * (550 * (1 - (timer * 0.0025f)));
                NPC.Center = Vector2.Lerp(NPC.Center, myPlayer.Center + vecToPlayer, Math.Clamp(timer * 0.005f, 0, 0.8f));
                //NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 4f;
                NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;

                //spawn telegraph line
                if (timer == 0)
                {
                    if (sweepLaserReps == 0)
                        CombatText.NewText(new Rectangle((int)myPlayer.Center.X, (int)myPlayer.Center.Y, 1, 1), Color.White with { A = 0 }, "this attack is getting deleted lol", dramatic: true);

                    sweepLaserDir = Main.rand.NextBool();

                    int telegraphLine = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TelegraphLineCyver>(), 0, 0);
                    if (Main.projectile[telegraphLine].ModProjectile is TelegraphLineCyver line)
                    {
                        line.NPCTetheredTo = NPC;
                    }
                }

                if (timer == 50)
                {
                    int telegraph = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<SweepLaserTell>(), 0, 0);
                    if (Main.projectile[telegraph].ModProjectile is SweepLaserTell tell)
                    {
                        tell.NPCTetheredTo = NPC;
                        tell.sweepDir = sweepLaserDir;
                    }

                    //int telegraphLine = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(0,1).RotatedBy(NPC.rotation + MathHelper.PiOver2), ModContent.ProjectileType<TelegraphLineCyver>(), 0, 0);
                    //Main.projectile[telegraphLine].timeLeft = 20;
                    //if (Main.projectile[telegraphLine].ModProjectile is TelegraphLineCyver line)
                    //{
                    //line.NPCTetheredTo = NPC;
                    //line.sweepDir = sweepLaserDir;
                    //line.sweepTell = true;
                    //}
                }

                if (timer == 100)
                {
                    timer = -1;
                    advancer++;
                    NPC.velocity = Vector2.Zero;
                }
            }
            else if (advancer == 1)
            {

                if (timer == 25)
                {
                    SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorCharge") with { Volume = .62f, Pitch = .64f, };
                    SoundEngine.PlaySound(style, NPC.Center);

                    SoundStyle style3 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = 1.5f, PitchVariance = .47f, MaxInstances = 0, Volume = 0.3f };
                    SoundEngine.PlaySound(style3, NPC.Center);

                    SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .16f, Pitch = .6f, PitchVariance = .2f, MaxInstances = 1 };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.rotation.ToRotationVector2() * -96, Vector2.Zero, ModContent.ProjectileType<FocusedLaser>(), 15, 2);
                    Main.projectile[a].timeLeft = 119;
                    if (Main.projectile[a].ModProjectile is FocusedLaser laser)
                    {
                        laser.parentIndex = NPC.whoAmI;
                    }


                    advancer++;
                    timer = -1;
                }
            }
            else if (advancer == 2)
            {
                lineBonusSpeed = 1f;
                NPC.rotation += MathHelper.Clamp((0.0013f * timer), 0, 0.07f) * (sweepLaserDir ? 1 : -1);

                if (timer % 2 == 0)
                {
                    phase3PulseValue = 0.75f;
                }

                if (timer % 50 == 0 && timer != 0)
                {

                    //LOVEEE this laser pulse and should use it with the balls, but just feels wrong here
                    /*

                    Vector2 from = NPC.Center - new Vector2(96, 0).RotatedBy(NPC.rotation);
                    int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), from, NPC.rotation.ToRotationVector2() * -10, ModContent.ProjectileType<LaserExplosionBall>(),
                        ContactDamage / 10, 2, Main.myPlayer);
                    Projectile p = Main.projectile[a];
                    p.timeLeft = 1;

                    if (p.ModProjectile is LaserExplosionBall ball)
                    {
                        ball.projType = ModContent.ProjectileType<EnergyBall>();


                        ball.numberOfLasers = 3;
                        //ball.projType = ModContent.ProjectileType<StretchLaser>();
                        //ball.vel = 1f;
                    }
                    */


                    SoundEngine.PlaySound(SoundID.Item91 with { Pitch = 0.4f, Volume = 0.6f }, NPC.Center);
                    SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_explosive_trap_explode_1") with { PitchVariance = .16f, Volume = 0.8f, Pitch = 0.7f };
                    SoundEngine.PlaySound(style, NPC.Center);


                    Vector2 from = NPC.Center - new Vector2(96, 0).RotatedBy(NPC.rotation);

                    for (int i = -4; i < 5; i++)
                    {
                        int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), from, NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver4 * i) * -0.1f, ModContent.ProjectileType<StretchLaser>(),
                        ContactDamage / 7, 2);
                        Main.projectile[a].timeLeft = 400;
                        if (Main.projectile[a].ModProjectile is StretchLaser laser)
                        {
                            laser.accelerateTime = 300;
                            laser.accelerateStrength = 1.025f; //1.025
                        }
                    }

                    eyeStarValue = 1;

                }

                if (timer % 5 == 0)
                {
                    ShotDust();
                }

                if (timer == 120) //86
                {
                    timer = -1;
                    advancer = 0;

                    sweepLaserReps++;

                    if (sweepLaserReps == 3)
                    {
                        SetNextAttack("Dash");
                        sweepLaserReps = 0;
                    }
                }
            }

            timer++;
        }

        float splitLaserRot = 0;
        Projectile telegraphLineProj = null;
        Vector2 splitLaserVectoGoal = new Vector2(570, 0f);
        bool trueXfalseY = true;
        int splitLaserCount = 0;
        public void thinkOfANameLater(Player myPlayer)
        {
            float reAdjustAmount = isExpert ? 0.5f : 0.25f; 
            
            //Move to player side
            if (advancer == 0)
            {
                NPC.Center = Vector2.Lerp(NPC.Center, myPlayer.Center + splitLaserVectoGoal, Math.Clamp(timer * 0.005f, 0, 0.3f));
                NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;

                if (timer == 40)
                {
                    int telegraphLine = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TelegraphLineCyver>(), 0, 0);

                    telegraphLineProj = Main.projectile[telegraphLine];
                    if (Main.projectile[telegraphLine].ModProjectile is TelegraphLineCyver line)
                    {
                        line.NPCTetheredTo = NPC;
                    }
                }

                if (timer == 50)
                {
                    splitLaserRot = Main.rand.NextFloat(-0.2f, 0.21f) + (Main.rand.NextBool() ? reAdjustAmount : -reAdjustAmount);
                    timer = -1;
                    advancer++;
                }
            }

            //Re-adjust
            else if (advancer == 1)
            {

                if (timer <= 20)
                {
                    Vector2 vecToGoal = (splitLaserVectoGoal * 1.2f).RotatedBy(splitLaserRot);
                    NPC.Center = Vector2.Lerp(NPC.Center, myPlayer.Center + vecToGoal, Math.Clamp(timer * 0.02f, 0, 0.4f));
                    NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;

                    if (timer == 20)
                    {
                        if (telegraphLineProj != null)
                        {
                            telegraphLineProj.active = false;
                        }
                        NPC.velocity = Vector2.Zero;
                    }
                }
                else
                {
                    NPC.velocity *= 0.95f;
                }



                if (timer == 30)
                {
                    Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.Pi);
                    int a = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + vel * 100, vel * 4, ModContent.ProjectileType<SplittingLaser>(), GetDamage("SplitLaser"), 2);

                    if (Main.projectile[a].ModProjectile is SplittingLaser laser)
                        laser.CyverIndex = NPC.whoAmI;

                    ShotDust();
                    ShotDust();

                    SoundStyle style23 = new SoundStyle("Terraria/Sounds/Custom/dd2_sky_dragons_fury_shot_0") with { Pitch = .20f, PitchVariance = 0.4f, Volume = 0.5f };
                    SoundEngine.PlaySound(style23, NPC.Center);

                    SoundStyle style32 = new SoundStyle("AerovelenceMod/Sounds/Effects/laser_fire") with { Volume = 0.5f, Pitch = 0f, MaxInstances = -1, PitchVariance = 0.1f };
                    SoundEngine.PlaySound(style32, NPC.Center);

                    myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 8;
                    extraBoost = 0.65f;
                    lineBonusSpeed = 1.15f;

                    phase3PulseValue = 1;
                    eyeStarValue = 0.8f;
                    NPC.velocity = vel * -5; //5
                }

                if (timer > 30 && timer <= 41)
                    extraBoost = 0.5f;
                if (timer == 45)
                {

                    if (trueXfalseY)
                    {
                        trueXfalseY = false;

                        if (NPC.Center.Y > myPlayer.Center.Y)
                            splitLaserVectoGoal = new Vector2(0, 540);
                        else
                            splitLaserVectoGoal = new Vector2(0, -540);
                    }
                    else
                    {
                        trueXfalseY = true;

                        if (NPC.Center.X > myPlayer.Center.X)
                            splitLaserVectoGoal = new Vector2(540, 0);
                        else
                            splitLaserVectoGoal = new Vector2(-540, 0);
                    }

                    splitLaserCount++;

                    if (splitLaserCount == 6)
                    {
                        splitLaserCount = 0;
                        SetNextAttack("Dash");
                    }

                    advancer = 0;
                    timer = -1;
                }

            }


            //Fire Projectile

            timer++;
        }

        public bool spinningClockwise = false;
        float approachAccelValue = 0f;
        bool firstSpin = true;
        public void SpinPhase3(Player myPlayer)
        {
            int extraStartupTime = firstSpin ? 40 : 0;

            float toPlayerVelMult = isExpert ? 1f : 0.75f;
            int shotModulo = isExpert ? 5 : 6;


            if (!hasDoneMusicSync)
                lineAlpha = Math.Clamp(lineAlpha + 0.0045f, 0f, 1f);

            //Do the normal startup
            phase3PulseColor = Color.White;
            if (timer <= 150 + extraStartupTime)
            {
                if (timer == 0)
                {
                    storedRotaion = (myPlayer.Center - NPC.Center).ToRotation();
                    approachAccelValue = 0f;
                }

                ballScale += 2;
                if (timer >= 20)
                {
                    //Spinning chrage-up dust
                    Vector2 from = NPC.Center + new Vector2(-102, 0).RotatedBy(NPC.rotation);
                    for (int j = 0; j < 3; j++)
                    {
                        Vector2 circular = new Vector2(32, 0).RotatedBy(MathHelper.ToRadians(j * 120 + timer * 4));
                        circular = circular.RotatedBy(NPC.rotation);
                        Dust b = GlowDustHelper.DrawGlowDustPerfect(from + circular, ModContent.DustType<GlowCircleQuadStar>(), Vector2.Zero, Color.DeepPink, 0.3f, 0.6f, 0f, dustShader2);
                    }
                }

                //If further away from player, move in quicker
                if (Math.Abs(NPC.Center.Distance(myPlayer.Center)) > 450)
                {
                    approachAccelValue = Math.Clamp(approachAccelValue + 8, 10, 100);
                    NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * (7f + (approachAccelValue * 0.12f)) * toPlayerVelMult; //0.075
                }
                else
                {
                    approachAccelValue = Math.Clamp(approachAccelValue - 3, 10, 100);
                    NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * (4f + (approachAccelValue * 0.08f)) * toPlayerVelMult; //0.075
                }

                NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;
            }

            //Turn towards player 
            else if (timer > 150 + extraStartupTime)
            {
                if (timer == 151 + extraStartupTime + 5 && !hasDoneMusicSync)
                {
                    lineAlpha = 1f;
                    overlayPower = 1.5f;
                    hasDoneMusicSync = true;
                }

                ballScale = 0;
                spammingLaser = true;
                lineBonusSpeed = 0.75f;

                bool farFromPlayer = Math.Abs(NPC.Center.Distance(myPlayer.Center)) > 975; //1150

                NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 1f;

                float newRotation = 0;
                if (timer > 300 + extraStartupTime && timer < 315 + extraStartupTime)
                    newRotation = Utils.AngleTowards(NPC.rotation, (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi + (spinningClockwise ? 1.2f : -1.2f), 0.2f);
                else
                    newRotation = Utils.AngleTowards(NPC.rotation, (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi, 0.04f); //25


                if (timer == 300 + extraStartupTime)
                {

                    //Teleport nearby if the player is too far
                    if (farFromPlayer)
                    {
                        Vector2 teleportDir = (NPC.Center - myPlayer.Center).SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver4 * 1.5f * (Main.rand.NextBool() ? 1f : -1f));

                        NPC.Center = myPlayer.Center + (teleportDir * 550f);

                        for (int i = 0; i < 22; i++) //4 //2,2
                        {
                            Dust p = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowStarSharp>(),
                                Main.rand.NextVector2Circular(12f, 12f), newColor: Color.DeepPink, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 0.75f);

                            StarDustDrawInfo info = new StarDustDrawInfo(true, false, true, true, false, 1f);
                            p.customData = AssignBehavior_GSSBase(rotPower: 0.04f, timeBeforeSlow: 5, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.8f, shouldFadeColor: false, sdci: info);
                        }

                        for (int y = 0; y < 15; y++)
                        {
                            Dust p = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LineSpark>(),
                                Main.rand.NextVector2CircularEdge(10f, 10f) * Main.rand.NextFloat(1f, 2.5f), newColor: Color.DeepPink, Scale: Main.rand.NextFloat(0.65f, 0.85f) * 2f);

                            p.customData = AssignBehavior_LSBase(velFadePower: 0.9f, preShrinkPower: 0.99f, postShrinkPower: 0.9f, timeToStartShrink: Main.rand.Next(0, 4), killEarlyTime: 80,
                                1f, 0.35f);
                        }
                    }

                    SoundStyle style = new SoundStyle("Terraria/Sounds/Research_1") with { Pitch = .65f, PitchVariance = .2f, Volume = 1f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style, NPC.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_68") with { Pitch = .54f, Volume = 0.8f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    SoundStyle BlasterDirect = new SoundStyle("AerovelenceMod/Sounds/Effects/SplatoonDirect") with { Pitch = -0.1f, PitchVariance = .1f, Volume = 0.11f, MaxInstances = -1 };
                    SoundEngine.PlaySound(BlasterDirect, NPC.Center);

                    eyeStarValue = 1f;
                    extraBoost = 1f;
                    lineBonusSpeed = 2f;

                    if (newRotation > NPC.rotation)
                        spinningClockwise = true;
                    else
                        spinningClockwise = false;

                }

                NPC.rotation = newRotation;

                if (timer % shotModulo == 0 && !(timer >= 300 + extraStartupTime && timer < 315 + extraStartupTime))
                {
                    overlayPower = Math.Clamp(overlayPower + 0.2f, 0f, 1f);
                    SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_158") with { Pitch = .56f, PitchVariance = .27f, };
                    SoundEngine.PlaySound(stylea, NPC.Center);
                    SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .33f, Pitch = .73f, PitchVariance = .27f, };
                    SoundEngine.PlaySound(styleb, NPC.Center);

                    Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.Pi);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + vel * 90, vel * 2, ModContent.ProjectileType<StretchLaser>(), GetDamage("SpinLaser"), 2);

                    NPC.velocity *= vel * -1f;

                    for (int i = 0; i < 3 + Main.rand.Next(3); i++)
                    {
                        Dust d = Dust.NewDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -50, ModContent.DustType<MuraLineBasic>(),
                            Velocity: NPC.rotation.ToRotationVector2().RotatedByRandom(0.2f) * -8f * Main.rand.NextFloat(0.7f, 1.3f), Alpha: 12, Color.HotPink * 1f, 0.45f);
                    }
                    ShotDust();

                    phase3PulseValue = 0.3f;
                    eyeStarValue = eyeStarValue > 0.35f ? eyeStarValue : 0.35f;
                    eyeStarRotation = Main.rand.NextFloat(6.28f);
                }

                if (timer == 400 + extraStartupTime)
                {
                    timer = 255 + extraStartupTime;
                    advancer++;

                    if (advancer == 3)
                    {
                        phase3PulseColor = Color.White;

                        hasDoneMusicSync = true;
                        firstSpin = false;

                        whatAttack = 15;
                        timer = -1;
                        advancer = 0;
                        spammingLaser = false;
                    }
                }
            }
            timer++;
        }

        float offsetAngle = 0;
        public void FunnelLaser(Player myPlayer)
        {

            if (timer == 0) { offsetAngle = 1.5f; }


            if (timer <= 40)
            {
                if (timer <= 30)
                    NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;
                if (timer == 30)
                {
                    storedRotaion = NPC.rotation;
                }
                NPC.velocity = Vector2.Zero;
            }

            if (timer > 40 && timer < 120)
            {

                if (timer % 5 == 0)
                {
                    Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.Pi);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + vel * 90, vel.RotatedBy(-offsetAngle) * 2, ModContent.ProjectileType<StretchLaser>(), 0, 2);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + vel * 90, vel.RotatedBy(offsetAngle) * 2, ModContent.ProjectileType<StretchLaser>(), 0, 2);
                }

                offsetAngle = MathHelper.Clamp(offsetAngle - 0.025f, 0.25f, 2f);
            }

            if (timer == 120)
            {
                timer = -1;
                offsetAngle = 1.5f;
            }

            //Store Rotation
            //Fire Lasers outward by an offset that shrinks over time
            //Stop near end and wait
            //shoot big ball

            timer++;
        }

        Projectile eyeSwordInstance = null;
        float swordEasingProgress = 0f;
        bool hasDoneSwordSound = false;
        bool hasDoneSwordShot = false;
        int swordJustHitTime = 0;
        float swordDangerTelegraphPower = 0f;
        int swordSwingsCount = 0;
        public void EyeSword(Player myPlayer)
        {
            NPC.hide = false;

            //The total distance (radians) of the swing
            float swingDistance = MathHelper.ToRadians(180);

            float additionAmount = 0.025f;
            float startingProgress = 0.1f;
            float dashTime = 30f;
            float dashDistance = 800f;

            //Master: 8 | Expert: 12 | Classic 16 
            int frameToStartSwing = (isMaster ? 8 : (isExpert ? 12 : 16));

            //Master: 10 | Expert: 14 | Classic  16
            int frameToStartDash = (isMaster ? 10 : (isExpert ? 14 : 16));

            int timeToAim = 40;

            if (swordSwingsCount == 0)
            {
                timeToAim += 30;
                frameToStartSwing += 4;
            }

            // 6 in all difficulties
            int swordTotalReps = 6;

            //Move towards player and telegraph
            if (advancer == 0)
            {
                if (timer == 0)
                {
                    storedVec2 = (NPC.Center - myPlayer.Center).SafeNormalize(Vector2.UnitX).RotateRandom(3.5f);
                    if (swordSwingsCount == 0)
                    {
                        NPC.Center = myPlayer.Center + Main.rand.NextVector2CircularEdge(1100, 1100);
                        storedVec2 = (NPC.Center - myPlayer.Center).SafeNormalize(Vector2.UnitX).RotateRandom(0.5f);

                        SoundStyle style2 = new SoundStyle("Terraria/Sounds/Zombie_67") with { Pitch = -0.1f, PitchVariance = 0f, MaxInstances = -1, Volume = 1f };
                        SoundEngine.PlaySound(style2, NPC.Center);

                        SoundStyle style3 = new SoundStyle("Terraria/Sounds/Zombie_68") with { Pitch = -0.2f, PitchVariance = 0f, MaxInstances = -1, Volume = 1f };
                        SoundEngine.PlaySound(style3, NPC.Center);

                        SoundStyle style4 = new SoundStyle("AerovelenceMod/Sounds/Effects/EvilEnergy") with { Volume = 0.4f, Pitch = 0.15f, MaxInstances = -1 };
                        SoundEngine.PlaySound(style4, NPC.Center);

                        int b = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-68, 0).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<CyverRoarPulse>(), 0, 0f);
                        Main.projectile[b].scale = 14f;

                        (Main.projectile[b].ModProjectile as CyverRoarPulse).intensity = 0.35f;

                        foreach (Projectile p in Main.projectile)
                        {
                            if (p.type == ModContent.ProjectileType<EnergyBall>())
                                p.Kill();
                        }

                    }
                }

                //NPC position lerps to a distance that shrinks with time
                Vector2 vecToPlayer = storedVec2 * (550 * (1 - (timer * 0.0025f)));

                NPC.Center = Vector2.Lerp(NPC.Center, myPlayer.Center + vecToPlayer, Math.Clamp(timer * 0.005f, 0, 0.8f));
                NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;

                //Telegraph Line
                if (timer == 0)
                {
                    sweepLaserDir = true;

                    int telegraphLine = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TelegraphLineCyver>(), 0, 0);
                    Main.projectile[telegraphLine].timeLeft = timeToAim; //40 or 70 on first

                    if (Main.projectile[telegraphLine].ModProjectile is TelegraphLineCyver line)
                    {
                        line.NPCTetheredTo = NPC;
                    }
                }

                //Reset and move on
                if (timer == timeToAim) //40 or 70 on first
                {
                    timer = -1;
                    advancer++;
                    NPC.velocity = Vector2.Zero;
                }
            }

            //Create Sword after a bit
            else if (advancer == 1)
            {
                if (timer == frameToStartDash)
                {
                    SoundStyle style3 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = 1.5f, PitchVariance = .47f, MaxInstances = 0, Volume = 0.3f };
                    SoundEngine.PlaySound(style3, NPC.Center);

                    SoundStyle style4 = new SoundStyle("AerovelenceMod/Sounds/Effects/EvilEnergy") with { Volume = .3f, Pitch = 1f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style4, NPC.Center);

                    SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .16f, Pitch = 0f, PitchVariance = .2f, MaxInstances = 1 };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    //Distortion
                    int afg = Projectile.NewProjectile(null, NPC.Center, NPC.velocity.SafeNormalize(Vector2.UnitX) * 1f, ModContent.ProjectileType<DistortProj>(), 0, 0);
                    Main.projectile[afg].rotation = Main.rand.NextFloat(6.28f);
                    Main.projectile[afg].timeLeft = 12;
                    if (Main.projectile[afg].ModProjectile is DistortProj distort)
                    {
                        distort.tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/MagmaBall");
                        distort.implode = false;
                        distort.scale = 0.4f;
                    }

                    //Sword
                    int sword = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.rotation.ToRotationVector2() * -65f, Vector2.Zero, ModContent.ProjectileType<EyeSword>(), GetDamage("EyeSword"), 2, ai0: NPC.rotation);
                    eyeSwordInstance = Main.projectile[sword];
                    Main.projectile[sword].timeLeft = 100000;
                    if (Main.projectile[sword].ModProjectile is FocusedLaser laser)
                    {
                        laser.parentIndex = NPC.whoAmI;
                    }

                    swordEasingProgress = startingProgress;
                    storedRotaion = NPC.rotation;
                    originalPosForLerp = NPC.Center;

                    advancer++;
                    timer = -1;
                }
            }

            //Dash
            else if (advancer == 2)
            {
                if (timer == 0)
                {
                    Common.Systems.FlashSystem.SetCAFlashEffect(0.5f, 35, 1f, 0.5f, false, true);
                    squashPower = 1f;
                    myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 15;
                    justDashValue = 1f;
                    lineBonusSpeed = 1f;
                }

                float progress = timer / dashTime;

                NPC.Center = Vector2.Lerp(originalPosForLerp, originalPosForLerp + storedRotaion.ToRotationVector2() * -dashDistance, Easings.easeOutExpo(progress));


                thrusterValue = 0f;
                phase3PulseValue = 1f;
                extraBoost = 1f - progress;
                eyeStarValue = 1f - Easings.easeOutQuad(progress);
                phase3PulseColor = Color.HotPink * 1f;

                eyeSwordInstance.Center = NPC.Center + NPC.rotation.ToRotationVector2() * -60f;
                eyeSwordInstance.rotation = NPC.rotation + MathHelper.Pi;

                if (timer == dashTime - 8f)
                {
                    swordDangerTelegraphPower = 1f;

                    timer = -1;
                    advancer++;
                    sweepLaserReps++;

                    originalPosForLerp = NPC.Center;
                    storedRotaion = NPC.rotation + MathHelper.Pi;

                    if (advancer == 5)
                    {
                        advancer = 0;
                        (eyeSwordInstance.ModProjectile as EyeSword).fade = true;
                    }

                }
            }

            //Speeen
            else if (advancer == 3)
            {
                if (timer >= frameToStartSwing && swordJustHitTime <= 0)
                {
                    (eyeSwordInstance.ModProjectile as EyeSword).start = true;

                    swordEasingProgress = Math.Clamp(swordEasingProgress + additionAmount, startingProgress, 1f);

                    float rotationValue = MathHelper.Lerp(-swingDistance, swingDistance, Easings.easeInOutExpo(swordEasingProgress));

                    NPC.rotation = (storedRotaion + 0f) + (rotationValue * (sweepLaserDir ? 1f : -1f));

                }

                phase3PulseValue = 1f;
                extraBoost = (1f - swordEasingProgress);
                eyeStarValue = 1f - Easings.easeOutQuad(swordEasingProgress);
                phase3PulseColor = Color.HotPink * 1f;

                if (timer % 5 == 0 && swordEasingProgress < 0.65f) ShotDust(1.5f);

                if (timer % 1 == 0 && swordEasingProgress > 0.45f && swordEasingProgress < 0.55f && false) //0.4 0.6
                {
                    //int shot = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.rotation.ToRotationVector2() * -120f, NPC.rotation.ToRotationVector2() * -13f, ModContent.ProjectileType<Cyver2EnergyBall>(), 15, 2, ai0: myPlayer.whoAmI);
                    //Main.projectile[shot].extraUpdates = 0;

                    /*
                    for (int i = -1; i < 2; i += 1)
                    {
                        int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.rotation.ToRotationVector2() * -220f, NPC.rotation.ToRotationVector2().RotatedBy(i * 0.15f) * -0.3f, ModContent.ProjectileType<StretchLaser>(), 15, 2, ai0: myPlayer.whoAmI);
                        Main.projectile[a].timeLeft = 400;
                        if (Main.projectile[a].ModProjectile is StretchLaser laser)
                        {
                            laser.accelerateTime = 300;
                            laser.accelerateStrength = 1.025f; //1.025
                        }
                    }
                    */
                }

                if (swordEasingProgress >= 0.5f && !hasDoneSwordShot)
                {
                    for (int j = -4; j < 5; j++)
                    {
                        float rotVal = Math.Abs(j) * 0.4f * (j >= 0 ? 1f : -1f);

                        Vector2 pos = NPC.Center + (storedRotaion.ToRotationVector2() * -160f).RotatedBy(rotVal);
                        Vector2 vel = storedRotaion.ToRotationVector2().RotatedBy(rotVal);

                        int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, vel * -0.2f, ModContent.ProjectileType<StretchLaser>(), GetDamage("EyeSwordShot"), 2, ai0: myPlayer.whoAmI);
                        Main.projectile[a].timeLeft = 400;
                        if (Main.projectile[a].ModProjectile is StretchLaser laser)
                        {
                            laser.accelerateTime = 180;
                            laser.accelerateStrength = 1.03f; 
                        }

                    }
                    swordJustHitTime = 1;
                    hasDoneSwordShot = true;
                }

                if (swordEasingProgress >= 0.35f && !hasDoneSwordSound)
                {
                    Common.Systems.FlashSystem.SetCAFlashEffect(0.75f, 25, 1f, 0.45f, false, true);
                    myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 10;


                    lineBonusSpeed = 4f; //2.5
                    whiteBackgroundPower = 0.05f;

                    phase3PulseValue = 1;
                    extraBoost = 1;
                    overlayPower = 1.5f;

                    SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Killed_56") with { Pitch = 0.65f, PitchVariance = .11f, Volume = 0.5f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style, NPC.Center);
                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Killed_55") with { Pitch = 0.36f, PitchVariance = .15f, Volume = 0.5f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    hasDoneSwordSound = true;
                }


                eyeSwordInstance.Center = NPC.Center + NPC.rotation.ToRotationVector2() * -60f;
                eyeSwordInstance.rotation = NPC.rotation + MathHelper.Pi;
                (eyeSwordInstance.ModProjectile as EyeSword).progress = swordEasingProgress;
                (eyeSwordInstance.ModProjectile as EyeSword).centerAngle = storedRotaion;

                if (swordEasingProgress >= 0.8f)
                    (eyeSwordInstance.ModProjectile as EyeSword).fade = true;

                //Reset
                if (swordEasingProgress >= 0.85f /*timer == timeToLast*/)
                {
                    timer = -1;
                    advancer = 0;
                    swordEasingProgress = 0f;
                    swordSwingsCount++;

                    hasDoneSwordSound = false;
                    hasDoneSwordShot = false;

                    (eyeSwordInstance.ModProjectile as EyeSword).fade = true;


                    if (swordSwingsCount >= swordTotalReps)
                    {
                        SetNextAttack("Summon");
                        swordSwingsCount = 0;
                    }

                }
            }

            swordJustHitTime--;
            timer++;
        }

        #endregion

        #region Dash Attacks 
        //Dash Attacks

        float storedRotaion = 0;
        Vector2 storedVec2 = Vector2.Zero;
        public void IdleDash(Player myPlayer)
        {
            int extraTime = hasDoneMusicSync ? 0 : 15;

            if (timer == 0)
            {
                switch (dashQuadrant)
                {
                    case 1:
                        startingAngBonus = 160;
                        advanceNegative = true;
                        break;
                    case 2:
                        startingAngBonus = 20;
                        advanceNegative = false;
                        break;
                    case 3:
                        startingAngBonus = -20;
                        advanceNegative = true;
                        break;
                    case 4:
                        startingAngBonus = 160; //160
                        advanceNegative = true;
                        break;
                }
            }

            if (timer < 105)
            {
                if (timer == 70)
                {
                    NPC.velocity = Vector2.Zero;
                    eyeStarValue = 1;
                    SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Custom/dd2_phantom_phoenix_shot_2") with { Pitch = .96f, Volume = 0.8f }, NPC.Center);

                    SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/katana_06") with { Pitch = .7f, Volume = 0.1f };
                    SoundEngine.PlaySound(style, NPC.Center);
                }

                #region daggers
                if (timer == 80 && Phase2)
                {
                    int daggerCount = 3 + (isExpert ? 1 : 0);
                    for (int i = 0; i < daggerCount; i++)
                    {
                        Vector2 outVec = new Vector2(300f, 0f).RotatedBy((MathHelper.TwoPi / daggerCount) * i);

                        Vector2 toLocation = myPlayer.Center + outVec;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), toLocation, Vector2.Zero, ModContent.ProjectileType<ShadowBlade>(), GetDamage("IdleDaggers"), 2, Main.myPlayer);
                        }
                        Vector2 toLocationVelo = toLocation - NPC.Center;
                        Vector2 from = NPC.Center;
                        for (int j = 0; j < 300; j++)
                        {
                            Vector2 velo = toLocationVelo.SafeNormalize(Vector2.Zero);
                            from += velo * 12;
                            Vector2 circularLocation = new Vector2(10, 0).RotatedBy(MathHelper.ToRadians(j * 12 + (timer * 1)));

                            if (j % 1 == 0)
                            {
                                Dust star = Dust.NewDustPerfect(from + circularLocation, ModContent.DustType<GlowPixelCross>(), Vector2.Zero, newColor: Color.HotPink, Scale: 0.45f);

                                star.rotation = Main.rand.NextFloat(6.28f);

                                star.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                                    rotPower: 0.05f, preSlowPower: 0.95f, timeBeforeSlow: 20, postSlowPower: 0.86f, velToBeginShrink: 4f, fadePower: 0.89f, shouldFadeColor: false);

                                //Dust d = GlowDustHelper.DrawGlowDustPerfect(from + circularLocation, ModContent.DustType<GlowCircleQuadStar>(), Vector2.Zero, Color.DeepPink, 0.6f, 0.6f, 0f, dustShader2);
                            }
                            if ((from - toLocation).Length() < 24)
                            {
                                break;
                            }
                        }
                    }
                }
                #endregion

                Vector2 goalPoint;
                if (timer < 70)
                    goalPoint = new Vector2(-350, 0).RotatedBy(MathHelper.ToRadians(advancer * -0.6f + startingAngBonus)); //250 || 0.4
                else
                    goalPoint = new Vector2(-500, 0).RotatedBy(MathHelper.ToRadians(advancer * -0.6f + startingAngBonus)); //250 || 0.4

                Vector2 move = (goalPoint + (timer < 70 ? myPlayer.Center : storedVec2)) - NPC.Center;

                float scalespeed = (timer < 70 ? 0.6f * 4f : 0.6f * 2f);//2

                NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
                NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;

                if (timer < 70)
                    NPC.rotation = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();
                else
                    NPC.rotation = storedRotaion;//(float)(storedRotaion + MathHelper.ToRadians(3f * (float)Math.Sin(timer)) );

            }
            else if (timer >= 105 && timer < 130)
            {
                NPC.damage = ContactDamage;
                thrusterValue = 0;

                if (timer == 105)
                {
                    //Laser Pulse
                    if (isMaster)
                    {
                        for (int i = 0; i < 360; i += 360 / 12)
                        {
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(2f, 0).RotatedBy(MathHelper.ToRadians(i)), ModContent.ProjectileType<CyverLaser>(), GetDamage("IdleBurst"), 0, Main.myPlayer);
                            if (Main.projectile[proj].ModProjectile is CyverLaser laser)
                            {
                                laser.accelerate = true;
                                laser.accelerateTime = 95;
                                laser.accelerateAmount = 1.015f;
                            }

                        }
                    }

                    for (int m = 0; m < 12 + Main.rand.Next(0, 4); m++)
                    {
                        Color col = new Color(78, 225, 245);
                        Dust d = Dust.NewDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -10, ModContent.DustType<MuraLineBasic>(),
                            NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(0.75f, 3.5f) * 5f, newColor: col, Scale: Main.rand.NextFloat(0.6f, 0.9f));
                        d.fadeIn = 1f + Main.rand.NextFloat(-0.4f, 0f);
                        d.alpha = 15 + Main.rand.Next(-2, 1);
                    }
                    squashPower = 1f;
                    justDashValue = 1f;
                }

                if ((timer == 115 || timer == 119 || timer == 123) && Phase2)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<EnergyBall>(), GetDamage("IdleEnergyBall"), 2, -1);
                }

                if (NPC.velocity.Length() > 20)
                    Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(5, 5), ModContent.DustType<MuraLineBasic>(), NPC.velocity.RotateRandom(0.25f) * 0.3f, 13, new Color(0, 255, 255), 0.45f);

                if (timer == 107) //113
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = .15f, MaxInstances = -1, };
                    SoundEngine.PlaySound(style, NPC.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = .59f, Pitch = 1f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/flame_thrower_airblast_rocket_redirect") with { Volume = .2f, Pitch = .42f };
                    SoundEngine.PlaySound(style3, NPC.Center);

                    myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 18;

                    int afg = Projectile.NewProjectile(null, NPC.Center, NPC.velocity.SafeNormalize(Vector2.UnitX) * 1f, ModContent.ProjectileType<DistortProj>(), 0, 0);
                    Main.projectile[afg].rotation = Main.rand.NextFloat(6.28f);
                    Main.projectile[afg].timeLeft = 10;

                    if (Main.projectile[afg].ModProjectile is DistortProj distort)
                    {
                        distort.tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/MagmaBall");
                        distort.implode = false;
                        distort.scale = 0.4f;
                    }
                }
                accelFloat = MathHelper.SmoothStep(accelFloat, 80, 0.3f);  //MathHelper.Clamp(MathHelper.Lerp(accelFloat, 60f, 0.1f), 0, 50f);
                NPC.rotation = storedRotaion;
                NPC.velocity = storedRotaion.ToRotationVector2() * accelFloat * -1; // 4--> accelFloat
            }

            if (timer == 69)
            {
                NPC.velocity = Vector2.Zero;
                storedVec2 = myPlayer.Center;
                storedRotaion = NPC.rotation;
            }

            if (advanceNegative && timer < 69)
                advancer--;
            else if (!advanceNegative && timer < 69)
                advancer++;

            //Have him slow if the attack has more endlag
            if (timer > 132)
                NPC.velocity *= 0.9f;

            if (timer == 140 + extraTime)
            {
                startingQuadrant = dashQuadrant;
                accelFloat = 0f;
                advancer = 0;
                timer = -1;

                if (barrageCount > 0)
                {
                    whatAttack = 1;
                    switch (dashQuadrant)
                    {
                        case 1:
                            startingQuadrant = 3;
                            break;
                        case 2:
                            startingQuadrant = 4;
                            break;
                        case 3:
                            startingQuadrant = 1;
                            break;
                        case 4:
                            startingQuadrant = 2;
                            break;
                    }

                    barrageCount--;
                }
                else
                {
                    whatAttack = 1;
                    SetNextAttack("Laser");
                }
            }

            timer++;
        }

        public void ChaseDash(Player myPlayer)
        {
            //55 to 60 for faster dashes
            //velocity from 35 to 55

            //This bullshit makes it so that the player can dash with the SoC, but cannot bonk off anything 
            if (myPlayer.timeSinceLastDashStarted == 0 && myPlayer.dashType == 2)
            {
                myPlayer.eocDash = 0;
            }

            float minusTime = (isExpert || isMaster) ? 8 : 3; //8:0

            NPC.damage = 0;
            NPC.hide = false;
            NPC.dontTakeDamage = false;
            fadeDashing = true;
            if (timer == 0)
            {
                int FX = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<TeleportFXCyver>(), 0, 0, Main.myPlayer);
                Main.projectile[FX].rotation = NPC.rotation;
            }


            if (timer < 40 && timer > 10)
            {
                Vector2 toPlayer = Vector2.Lerp(NPC.rotation.ToRotationVector2(), (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX), 0.2f);// (NPC.rotation.ToRotationVector2() )

                NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 2.5f;
                NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;
                storedRotaion = NPC.velocity.ToRotation();
            }

            if (timer >= 40)
            {
                NPC.rotation = storedRotaion + MathHelper.Pi;

                //Spawn Cyver2EnergyBall
                if (timer == 40)
                {
                    fadeDashValue = 1;

                    int type = ModContent.ProjectileType<CyverLaserBomb>();
                    int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, type, GetDamage("ChaseShard"), 2, -1);

                    if (Main.projectile[a].ModProjectile is CyverLaserBomb clb)
                    {
                        clb.CyverIndex = NPC.whoAmI;
                        clb.longTelegraph = true;
                    }

                    Main.projectile[a].rotation = storedRotaion;


                    storedVec2 = storedRotaion.ToRotationVector2().RotatedByRandom(0f) * (Phase3 ? 25 : 35); //28 35
                    NPC.velocity = Vector2.Zero;

                    NPC.velocity = storedRotaion.ToRotationVector2() * 175; //55

                    SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/GloogaSlide") with { Volume = 0.15f, Pitch = 0.6f, MaxInstances = 2 }, NPC.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = 0.25f, Pitch = 0.85f, MaxInstances = -1, PitchVariance = 0.1f };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/ElectricExplode") with { Volume = .05f, Pitch = 1f, };
                    SoundEngine.PlaySound(style3, NPC.Center);

                    squashPower = 1.5f;
                    lineBonusSpeed = 1f;

                    //Dust
                    for (int m = 0; m < 9 + Main.rand.Next(0, 4); m++)
                    {
                        Color col = new Color(78, 225, 245);
                        Dust d = Dust.NewDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -10, ModContent.DustType<MuraLineBasic>(),
                            NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(0.75f, 3.5f) * 4f, newColor: col, Scale: Main.rand.NextFloat(0.6f, 0.9f));
                        d.fadeIn = 1f + Main.rand.NextFloat(-0.4f, 0f);
                        d.alpha = 15 + Main.rand.Next(-2, 1);
                    }

                }
                storedVec2 *= 0.95f;
                NPC.velocity = storedVec2;

            }

            if (timer > 60 - minusTime)
            {
                timer = 38;
                advancer++;
            }

            if (advancer == 20 + minusTime) //25
            {
                fadeDashing = false;
                startingQuadrant = 4;

                if (!Phase3)
                    SetNextAttack("Summon");
                else
                {
                    whatAttack = 24;
                    //Hide because next attack is ball dash
                    NPC.hide = true;
                }


                timer = -1;
                advancer = 0;
                storedRotaion = 0;
                //NPC.damage = 0;


                NPC.dontTakeDamage = false;

                previousPositions.Clear();
                previousRotations.Clear();
            }
            timer++;
        }

        Vector2 trackPoint = Vector2.Zero;
        public void TrackDash(Player myPlayer)
        {
            if (timer < 60)
            {
                trackPoint = Vector2.Lerp(trackPoint, myPlayer.Center + myPlayer.velocity.SafeNormalize(Vector2.UnitX) * 200, 0.2f);

                Dust.NewDustPerfect(trackPoint, DustID.AmberBolt, Velocity: Vector2.Zero);

                NPC.velocity = (trackPoint - NPC.Center).SafeNormalize(Vector2.UnitX) * 4.5f;
                NPC.rotation = (trackPoint - NPC.Center).ToRotation() + MathHelper.Pi;

            }
            if (timer >= 60)
            {
                if (timer == 60)
                {
                    storedRotaion = NPC.rotation;
                }

                NPC.velocity = Vector2.SmoothStep(NPC.velocity, storedRotaion.ToRotationVector2() * -50, 0.2f);
                if (timer == 70 || timer == 75 || timer == 80)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ShadowBlade>(), ContactDamage / 4, 1, Main.myPlayer);
                }
            }
            //move towards player
            if (timer == 90)
            {
                timer = -1;
                advancer++;
                NPC.velocity = Vector2.Zero;
                if (advancer == 7)
                {
                    whatAttack = 5;
                    advancer = 0;
                }
            }

            timer++;
        }

        Vector2 vecOut = Vector2.Zero;
        public void WrapDash(Player myPlayer)
        {
            int repsVal = 5;

            float velMax = 0f;

            if (isExpert)
                velMax = 43;
            else if (isMaster)
                velMax = Phase3 ? 50 : 45;
            else
                velMax = 40;

            if (timer == 0)
            {
                accelFloat = 35;
                if (NPC.Center.X > myPlayer.Center.X)
                    vecOut = new Vector2(500, 0);
                else
                    vecOut = new Vector2(-500, 0);

            }

            //Cyver Moves to a random VectorOut
            if (timer < 95)
            {
                Vector2 move = (vecOut + myPlayer.Center) - NPC.Center;

                float scalespeed = advancer == 0 ? 1.15f : 1.15f; //2

                NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
                NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;

                NPC.rotation = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();

                if (timer == 35)
                    eyeStarValue = 1;

            }

            //Cyver Dashes with 2 energyBalls
            if (timer >= 95)
            {
                thrusterValue = 0;

                if (timer <= 101)
                {
                    lineBonusSpeed *= 0.9f;
                    extraBoost = 1f;
                }

                if (timer == 95)
                {
                    eyeStarValue = 0.75f;
                    justDashValue = 1f;

                    //extraBoost = 1.25f;
                    overlayPower = 0.75f;

                    if (advancer == 0)
                        NPC.velocity = Vector2.Zero;

                    lineBonusSpeed = 1.75f; //1f
                    myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 15;
                    squashPower = 1f;

                    int afg = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<DistortProj>(), 0, 0);
                    Main.projectile[afg].rotation = Main.rand.NextFloat(6.28f);
                    Main.projectile[afg].timeLeft = 10;

                    if (Main.projectile[afg].ModProjectile is DistortProj distort)
                    {
                        distort.tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/MagmaBall");
                        distort.implode = false;
                        distort.scale = 0.4f;
                    }

                    storedRotaion = NPC.rotation;


                    for (int m = 0; m < 12 + Main.rand.Next(0, 4); m++)
                    {
                        Color col = new Color(78, 225, 245);
                        Dust d = Dust.NewDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -10, ModContent.DustType<MuraLineBasic>(),
                            NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(0.75f, 3.5f) * 5f, newColor: col, Scale: Main.rand.NextFloat(0.6f, 0.9f));
                        d.fadeIn = 1f + Main.rand.NextFloat(-0.4f, 0f);
                        d.alpha = 15 + Main.rand.Next(-2, 1);
                    }

                }

                NPC.damage = ContactDamage;

                if (NPC.velocity.Length() > 20)
                    Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(10, 10), ModContent.DustType<MuraLineBasic>(), NPC.velocity.RotateRandom(0.25f) * 0.3f, 13, new Color(0, 255, 255), 0.45f);

                accelFloat = MathHelper.SmoothStep(accelFloat, velMax, 0.1f);  //MathHelper.Clamp(MathHelper.Lerp(accelFloat, 60f, 0.1f), 0, 50f);
                NPC.rotation = storedRotaion;
                NPC.velocity = storedRotaion.ToRotationVector2() * accelFloat * -1;

                if (timer == 98)
                {

                    SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = .15f, MaxInstances = -1, };
                    SoundEngine.PlaySound(style, NPC.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = .59f, Pitch = 1f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/flame_thrower_airblast_rocket_redirect") with { Volume = .16f, Pitch = .42f, PitchVariance = 0.1f };
                    SoundEngine.PlaySound(style3, NPC.Center);

                    Vector2 vel = (NPC.rotation + 2.25f).ToRotationVector2() * 6.5f;
                    Vector2 vel2 = (NPC.rotation - 2.25f).ToRotationVector2() * 6.5f;


                    int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<EnergyBall>(), GetDamage("WrapEnergyBall"), 1);
                    int b = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel2, ModContent.ProjectileType<EnergyBall>(), 20, 1);

                    if (isExpert)
                    {
                        int c = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2() * 6, ModContent.ProjectileType<EnergyBall>(), GetDamage("WrapEnergyBall"), 1);
                        Main.projectile[c].extraUpdates = 1;

                        if (isMaster)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2() * -6, ModContent.ProjectileType<EnergyBall>(), GetDamage("WrapEnergyBall"), 1);
                    }
                }
            }

            //Reset
            if (timer == 140)
            {
                timer = 39;
                vecOut = new Vector2(470, 0).RotatedBy(Main.rand.NextFloat(6.28f));
                NPC.Center = myPlayer.Center + (vecOut * 2.3f);

                if (advancer == repsVal)
                {
                    NPC.velocity = Vector2.Zero;
                    timer = -1;
                    advancer = -1;
                    startingQuadrant = 2;

                    if (Phase3)
                        whatAttack = 17;
                    else
                        SetNextAttack("Summon");
                }
                advancer++;
            }

            timer++;
        }

        float wrapRotAmount = 0;
        bool wrapDir = false;
        int wrap2side = 1;
        public void WrapDashParaffin(Player myPlayer)
        {
            if (timer == 0)
            {
                CombatText.NewText(new Rectangle((int)myPlayer.Center.X, (int)myPlayer.Center.Y, 1, 1), Color.White, "THIS ATTACK IS PENDING PSEUDO-REWORK", dramatic: true);

                accelFloat = 35;
                vecOut = new Vector2(-350, 0).RotatedBy(MathHelper.PiOver4 + ((wrap2side - 1) * MathHelper.PiOver2));
                NPC.Center = myPlayer.Center + vecOut * 2f;
            }

            if (timer < 35)
            {

                //vecOut = vecOut.RotatedBy(wrapRotAmount);

                Vector2 move = (vecOut + myPlayer.Center) - NPC.Center;

                float scalespeed = 1.3f;

                NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
                NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;

                NPC.rotation = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();
                //NPC.rotation = MathHelper.Pi;


                //wrapRotAmount = wrapDir ? 0.02f : -0.02f;

            }

            if (timer >= 40)
            {
                if (timer == 40)
                {
                    storedRotaion = NPC.rotation;
                }

                NPC.damage = ContactDamage;

                if (NPC.velocity.Length() > 20)
                    Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(10, 10), ModContent.DustType<MuraLineBasic>(), NPC.velocity.RotateRandom(0.25f) * 0.3f, 13, new Color(0, 255, 255), 0.45f);

                accelFloat = Math.Clamp(MathHelper.Lerp(accelFloat, 60, 0.1f), 0, 50);  //50 0.1
                NPC.rotation = storedRotaion;
                NPC.velocity = storedRotaion.ToRotationVector2() * accelFloat * -1;

                thrusterValue = 0;


                if (timer == 43)
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = .15f, MaxInstances = -1, };
                    SoundEngine.PlaySound(style, NPC.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = .29f, Pitch = 1f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/flame_thrower_airblast_rocket_redirect") with { Volume = .16f, Pitch = .42f };
                    SoundEngine.PlaySound(style3, NPC.Center);

                    myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 15;
                    squashPower = 1f;

                    int afg = Projectile.NewProjectile(null, NPC.Center, NPC.velocity.SafeNormalize(Vector2.UnitX) * 1f, ModContent.ProjectileType<DistortProj>(), 0, 0);
                    Main.projectile[afg].rotation = Main.rand.NextFloat(6.28f);
                    Main.projectile[afg].timeLeft = 10;

                    if (Main.projectile[afg].ModProjectile is DistortProj distort)
                    {
                        distort.tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/MagmaBall");
                        distort.implode = false;
                        distort.scale = 0.6f;
                    }

                    for (int i = -4; i < 5; i++)
                    {
                        int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2().RotatedBy(0.3f * i) * -0.4f, ModContent.ProjectileType<StretchLaser>(),
                        ContactDamage / 7, 2);
                        Main.projectile[a].timeLeft = 400;
                        if (Main.projectile[a].ModProjectile is StretchLaser laser)
                        {
                            laser.accelerateTime = 200;
                            laser.accelerateStrength = 1.02f; //1.025
                        }
                    }

                    /*
                    int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<LaserExplosionBall>(),
                        ContactDamage / 7, 2, Main.myPlayer);
                    Projectile p = Main.projectile[a];
                    p.timeLeft = 1;

                    if (p.ModProjectile is LaserExplosionBall ball)
                    {
                        ball.numberOfLasers = 12;
                        //ball.projType = ModContent.ProjectileType<StretchLaser>();
                        ball.vel = 7f;
                        
                    }
                    */
                }

                if (timer > 38 && timer % 6 == 0 && timer < 60)
                {
                    //int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<EnergyBall>(), ContactDamage / 7, 2, Main.myPlayer);
                }

                /*
                if (timer == 44 || timer == 50 || timer == 55)
                {
                    int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<LaserExplosionBall>(),
                        ContactDamage / 7, 2, Main.myPlayer);
                    Projectile p = Main.projectile[a];
                    p.timeLeft = 50;

                    if (p.ModProjectile is LaserExplosionBall ball)
                    {
                        ball.numberOfLasers = 4;
                        ball.projType = ModContent.ProjectileType<StretchLaser>();
                        ball.vel = 2f;
                    }
                }
                */
            }


            if (timer == 90)
            {
                timer = 1;

                vecOut = new Vector2(-350, 0).RotatedBy(MathHelper.PiOver4 + ((wrap2side - 1) * MathHelper.PiOver2));
                wrapDir = !wrapDir;
                NPC.Center = myPlayer.Center + (vecOut * 3f);
                accelFloat = 10;


                wrap2side++;
                if (wrap2side == 5)
                    wrap2side = 1;

                if (advancer == 6)
                {
                    SetNextAttack("Summon");
                    timer = -1;
                    advancer = -1;
                    wrap2side = 1;
                }

                advancer++;
            }
            timer++;
        }

        int ballDashCount = 0;
        public void BallDash(Player myPlayer)
        {
            int reps = 9; //8

            //This bullshit makes it so that the player can dash with the SoC, but cannot bonk off anything 
            if (myPlayer.timeSinceLastDashStarted == 0 && myPlayer.dashType == 2)
            {
                myPlayer.eocDash = 0;
            }

            NPC.damage = 0;
            if (timer < 75 && timer != 39) //62
                NPC.dontTakeDamage = false;
            else if (timer >= 75 && timer < 100)
                NPC.dontTakeDamage = true;
            fadeDashing = true;

            if (timer == 0)
            {
                previousPositions.Clear();
                previousRotations.Clear();

                hideRegreGlow = true;
                NPC.hide = true;
                int FX = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<TeleportFXCyver>(), 0, 0, Main.myPlayer);
                Main.projectile[FX].rotation = NPC.rotation;

                if (Main.projectile[FX].ModProjectile is TeleportFXCyver tfxc)
                    tfxc.blue = true;

                NPC.velocity = Vector2.Zero;
                NPC.dontTakeDamage = false;
            }

            if (timer >= 40)
            {
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi;


                if (timer == 40)
                {
                    if (ballDashCount == 0)
                    {
                        previousPositions.Clear();
                        previousRotations.Clear();
                    }

                    NPC.hide = false;
                    hideRegreGlow = false;

                    goalLocation = myPlayer.velocity.ToRotation().ToRotationVector2().RotatedByRandom(1f) * 700f;
                    NPC.Center = goalLocation + myPlayer.Center;

                    //Dash
                    fadeDashValue = 1;

                    NPC.velocity = Vector2.Zero;
                    NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * -30;

                    NPC.rotation = NPC.velocity.ToRotation();

                    SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/GloogaSlide") with { Volume = 0.15f, Pitch = 0.6f, MaxInstances = 2 }, NPC.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = 0.25f, Pitch = 0.85f, MaxInstances = -1, PitchVariance = 0.1f };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/ElectricExplode") with { Volume = .05f, Pitch = 1f, };
                    SoundEngine.PlaySound(style3, NPC.Center);
                    squashPower = 1.5f;
                    //Dust
                    for (int m = 0; m < 9 + Main.rand.Next(0, 4); m++)
                    {
                        Color col = new Color(78, 225, 245);
                        Dust d = Dust.NewDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -10, ModContent.DustType<MuraLineBasic>(),
                            NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.35f, 0.35f)) * Main.rand.NextFloat(0.75f, 3.5f) * 4f, newColor: col, Scale: Main.rand.NextFloat(0.6f, 0.9f));
                        d.fadeIn = 1f + Main.rand.NextFloat(-0.4f, 0f);
                        d.alpha = 15 + Main.rand.Next(-2, 1);
                    }

                    int FX = Projectile.NewProjectile(null, NPC.Center, NPC.rotation.ToRotationVector2() * -10, ModContent.ProjectileType<TeleportFXCyver>(), 0, 0, Main.myPlayer);
                    Main.projectile[FX].rotation = NPC.rotation;

                    if (Main.projectile[FX].ModProjectile is TeleportFXCyver tfxc)
                    {
                        tfxc.reverse = true;
                        tfxc.blue = true;
                    }

                    storedVec2 = NPC.rotation.ToRotationVector2() * 25;

                    for (int afterImage = 0; afterImage < NPC.oldPos.Length; afterImage++)
                    {
                        NPC.oldPos[afterImage] = NPC.position;
                        NPC.oldRot[afterImage] = NPC.rotation;
                    }
                }

                if (timer == 55)
                {
                    //Dash Again but drop a bomb

                    fadeDashValue = 1;

                    NPC.velocity = Vector2.Zero;
                    NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 30;
                    NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi;


                    SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/GloogaSlide") with { Volume = 0.15f, Pitch = 0.6f, MaxInstances = 2 }, NPC.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = 0.25f, Pitch = 0.85f, MaxInstances = -1, PitchVariance = 0.1f };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/ElectricExplode") with { Volume = .05f, Pitch = 1f, };
                    SoundEngine.PlaySound(style3, NPC.Center);
                    squashPower = 1.5f;
                    //Dust
                    for (int m = 0; m < 9 + Main.rand.Next(0, 4); m++)
                    {
                        Color col = new Color(78, 225, 245);
                        Dust d = Dust.NewDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -10, ModContent.DustType<MuraLineBasic>(),
                            NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.35f, 0.35f)) * Main.rand.NextFloat(0.75f, 3.5f) * 4f, newColor: col, Scale: Main.rand.NextFloat(0.6f, 0.9f));
                        d.fadeIn = 1f + Main.rand.NextFloat(-0.4f, 0f);
                        d.alpha = 15 + Main.rand.Next(-2, 1);
                    }

                    int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2() * 0, ModContent.ProjectileType<LaserExplosionBall>(), GetDamage("BallDash"), 2, Main.myPlayer);
                    Projectile p = Main.projectile[a];
                    p.timeLeft = 30;

                    if (p.ModProjectile is LaserExplosionBall ball)
                    {
                        ball.projType = ModContent.ProjectileType<EnergyBall>();
                        ball.numberOfLasers = 3;
                        ball.vel = isExpert ? 5f : 6f;
                        ball.CyverIndex = NPC.whoAmI;
                    }
                    storedVec2 = NPC.rotation.ToRotationVector2() * 25;
                }

                if (timer == 62)
                {
                    hideRegreGlow = true;
                    NPC.dontTakeDamage = true;

                    NPC.hide = true;
                    int FX = Projectile.NewProjectile(null, NPC.Center, storedVec2 * -1f, ModContent.ProjectileType<TeleportFXCyver>(), 0, 0, Main.myPlayer);
                    Main.projectile[FX].rotation = NPC.rotation;

                    if (Main.projectile[FX].ModProjectile is TeleportFXCyver tfxc)
                        tfxc.blue = true;

                }

                if (timer >= 62)
                    storedVec2 *= 0.85f;
                else
                    storedVec2 *= 0.95f;
                NPC.velocity = storedVec2 * -1;
            }

            if (timer == 100)
            {
                ballDashCount++;
                timer = 38;

                previousPositions.Clear();
                previousRotations.Clear();

                NPC.velocity = Vector2.Zero;
                NPC.dontTakeDamage = true;
                NPC.hide = true;

                if (ballDashCount >= reps)
                {
                    timer = -1;
                    //SetNextAttack("Summon");
                    whatAttack = 33;
                    ballDashCount = 0;


                    //Hide because next attack is ball dash
                    NPC.hide = true;

                    NPC.dontTakeDamage = false;
                    fadeDashing = false;
                    hideRegreGlow = false;

                }
            }

            timer++;
        }

        Vector2 storedToPlayer = Vector2.Zero;
        public void CurvedDash(Player myPlayer)
        {
            int reps = 5; //6

            float spreadShotSpread = isExpert ? 0.10f : 0.075f;
            int shardModulo = isMaster ? 5 : 6;

            //This bullshit makes it so that the player can dash with the SoC, but cannot bonk off anything 
            if (myPlayer.timeSinceLastDashStarted == 0 && myPlayer.dashType == 2)
            {
                myPlayer.eocDash = 0;
            }

            if (timer == 0)
            {
                fadeDashing = false;

                accelFloat = 10;
                vecOut = new Vector2(-400, 0).RotatedBy(MathHelper.PiOver4 + ((wrap2side - 1) * MathHelper.PiOver2));
                NPC.Center = myPlayer.Center + vecOut * 2f;
            }

            if (timer < 35 + 10)
            {
                fadeDashing = false;
                curveDashTelegraphValue = 1.5f;
                justDashValue = 0.35f;
                phase3Intensity = 0f;

                Vector2 move = (vecOut + myPlayer.Center) - NPC.Center;

                float scalespeed = 1.3f;

                NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
                NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;

                NPC.rotation = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();
            }

            int extra = 10;
            if (timer >= 40 + extra && timer <= 100 + extra)
            {
                curveDashTelegraphValue = 0f;
                if (timer == 40 + extra)
                {
                    storedRotaion = (myPlayer.Center - NPC.Center).ToRotation();// NPC.rotation;
                    storedToPlayer = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                }

                //NPC.damage = ContactDamage;
                fadeDashing = true;
                fadeDashValue = 1f;
                justDashValue = 1f;

                if (NPC.velocity.Length() > 20)
                    Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(10, 10), ModContent.DustType<MuraLineBasic>(), NPC.velocity.RotateRandom(0.25f) * 0.3f, 13, new Color(0, 255, 255), 0.45f);

                accelFloat = Math.Clamp(MathHelper.Lerp(accelFloat, 60f, 0.02f), 0, 50f);  //50 0.1

                storedRotaion = storedRotaion.AngleTowards((myPlayer.Center - NPC.Center).ToRotation(), (0.001f * timer) + 0.01f); //(0.001f * timer) + 0.01f

                NPC.velocity = storedRotaion.ToRotationVector2() * accelFloat;
                NPC.rotation = storedRotaion + MathF.PI;

                //NPC.rotation = storedRotaion;
                //NPC.velocity = storedRotaion.ToRotationVector2() * accelFloat * -1;

                thrusterValue = 0;

                if (timer == 43 + extra)
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = .15f, MaxInstances = -1, };
                    SoundEngine.PlaySound(style, NPC.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = .29f, Pitch = 1f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/flame_thrower_airblast_rocket_redirect") with { Volume = .16f, Pitch = .42f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style3, NPC.Center);

                    SoundStyle style4 = new SoundStyle("AerovelenceMod/Sounds/Effects/lightning_flash_01") with { Volume = .64f, Pitch = .29f, PitchVariance = .22f, };
                    SoundEngine.PlaySound(style4, NPC.Center);

                    myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 15;
                    squashPower = 1f;

                    extraBoost = 1f;
                    Common.Systems.FlashSystem.SetCAFlashEffect(0.4f, 20, 1f, 0.5f, false, true);

                    int afg = Projectile.NewProjectile(null, NPC.Center, NPC.velocity.SafeNormalize(Vector2.UnitX) * 1f, ModContent.ProjectileType<DistortProj>(), 0, 0);
                    Main.projectile[afg].rotation = Main.rand.NextFloat(6.28f);
                    Main.projectile[afg].timeLeft = 20;

                    if (Main.projectile[afg].ModProjectile is DistortProj distort)
                    {
                        distort.tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/MagmaBall");
                        distort.implode = false;
                        distort.scale = 0.6f;
                    }

                    //int chaser = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2() * -10.25f, ModContent.ProjectileType<Cyver2EnergyBall>(), ContactDamage / 7, 2);


                    for (int i = -2; i < 3; i++)
                    {
                        int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2().RotatedBy(spreadShotSpread * i) * -1.25f, ModContent.ProjectileType<StretchLaser>(),
                        GetDamage("CurveSpreadShot"), 2);

                        Main.projectile[a].timeLeft = 200;
                        if (Main.projectile[a].ModProjectile is StretchLaser laser)
                        {
                            laser.accelerateTime = 100;
                            laser.accelerateStrength = 1.03f; //1.025
                        }
                    }

                }

                if (timer > 42 + extra && timer % shardModulo == 0 && timer <= 90 + extra)
                {
                    int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CyverLaserBomb>(), GetDamage("CurveShard"), 2, Main.myPlayer);
                    Main.projectile[a].rotation = storedRotaion + MathHelper.PiOver2;

                    if (Main.projectile[a].ModProjectile is CyverLaserBomb bomb)
                        bomb.CyverIndex = NPC.whoAmI;
                }

                //FadeFX
                if (timer == 100 + extra)
                {
                    int FX = Projectile.NewProjectile(null, NPC.Center, NPC.velocity * 0.5f, ModContent.ProjectileType<TeleportFXCyver>(), 0, 0, Main.myPlayer);
                    Main.projectile[FX].rotation = NPC.rotation;
                    if (Main.projectile[FX].ModProjectile is TeleportFXCyver tfxc) tfxc.blue = true;

                    NPC.velocity = Vector2.Zero;
                    NPC.hide = true;
                }

            }


            if (timer == 110 + extra)
            {
                previousPositions.Clear();
                previousRotations.Clear();

                timer = 1; //1
                NPC.hide = false;
                vecOut = new Vector2(-350, 0).RotatedByRandom(6.28f);

                //vecOut = new Vector2(-350, 0).RotatedBy(MathHelper.PiOver4 + ((wrap2side - 1) * MathHelper.PiOver2));
                wrapDir = !wrapDir;
                NPC.Center = myPlayer.Center + (vecOut * 3f);
                accelFloat = 5;


                wrap2side++;
                if (wrap2side == 5)
                    wrap2side = 1;

                if (advancer == reps)
                {
                    NPC.hide = false;
                    NPC.dontTakeDamage = false;
                    fadeDashing = false;
                    hideRegreGlow = false;

                    SetNextAttack("Summon");
                    timer = -1;
                    advancer = -1;
                    wrap2side = 1;
                }

                advancer++;
            }
            timer++;
        }


        Vector2 areaCenter = new Vector2(0, 0);
        Projectile stuckLaser = null;
        int penisCounter = 0;

        int phantomDash1Counter = 0;
        Projectile border = null;

        public void PhantomDash1(Player myPlayer)
        {
            //Give player infinite wing time to be nice (shhh)
            myPlayer.wingTime = myPlayer.wingTimeMax;

            if (advancer == 0)
            {
                NPC.velocity = Vector2.Zero;

                if (timer == 5)
                {
                    NPC.hide = true;
                    NPC.dontTakeDamage = true;

                    //SOUND EFFECT

                    bool a = Main.rand.NextBool(); // straight or diagonal
                    bool b = Main.rand.NextBool(); // default or rotated
                    bool c = Main.rand.NextBool(); // Other side

                    goalLocation = new Vector2(610, 0).RotatedBy((c ? 0 : MathHelper.Pi) + (b ? 0 : MathHelper.PiOver2) + (a ? 0 : MathHelper.PiOver4));


                    int FX = Projectile.NewProjectile(null, NPC.Center, NPC.velocity, ModContent.ProjectileType<TeleportFXCyver>(), 0, 0, Main.myPlayer);
                    Main.projectile[FX].rotation = NPC.rotation;

                    NPC.rotation = goalLocation.ToRotation();


                }

                if (timer == 20)
                {
                    //Spawn Border
                    int a = Projectile.NewProjectile(null, myPlayer.Center, Vector2.Zero, ModContent.ProjectileType<LightningBorder>(), 0, 0);
                    border = Main.projectile[a];


                    areaCenter = myPlayer.Center;

                }

                if (timer == 40)
                {
                    advancer++;
                    timer = 0;
                }
            }

            if (advancer == 1)
            {

                //telegraph
                if (timer == 30)
                {
                    int tele = Projectile.NewProjectile(null, areaCenter, Vector2.Zero, ModContent.ProjectileType<DookieTelegraph>(), 0, 0);
                    Main.projectile[tele].rotation = goalLocation.ToRotation();
                }

                //Put Fadein FX early
                if (timer == 60)
                {

                    int FX = Projectile.NewProjectile(null, (goalLocation * 1.10f) + areaCenter, goalLocation.SafeNormalize(Vector2.UnitX) * -10f, ModContent.ProjectileType<TeleportFXCyver>(), 10, 0, Main.myPlayer);
                    Main.projectile[FX].rotation = goalLocation.ToRotation();

                    if (Main.projectile[FX].ModProjectile is TeleportFXCyver tp) tp.reverse = true;


                }

                //Dash
                if (timer >= 75)
                {
                    NPC.damage = ContactDamage;

                    //Teleport back in
                    if (timer == 75)
                    {
                        NPC.dontTakeDamage = false;
                        NPC.hide = false;

                        NPC.Center = goalLocation + areaCenter;

                        NPC.damage = ContactDamage;

                        //int a = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<PhantomLaserTelegraph>(), 2, 0);
                        //if (Main.projectile[a].type == ModContent.ProjectileType<PhantomLaserTelegraph>()) 
                        //stuckLaser = Main.projectile[a];

                    }

                    if (timer == 78)
                    {
                        //SOUND EFFECT
                        SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = .15f, MaxInstances = -1, };
                        SoundEngine.PlaySound(style, NPC.Center);

                        SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = .29f, Pitch = 1f, MaxInstances = -1 };
                        SoundEngine.PlaySound(style2, NPC.Center);

                        SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/flame_thrower_airblast_rocket_redirect") with { Volume = .16f, Pitch = .42f };
                        SoundEngine.PlaySound(style3, NPC.Center);
                    }

                    Vector2 start = goalLocation + areaCenter;
                    Vector2 end = (goalLocation.RotatedBy(MathHelper.Pi) * 1f) + areaCenter;

                    float rawProgress = 0f;
                    if (timer > 75f)
                        rawProgress = (timer - 75f) / 75f;

                    float easingProgress = 1f - (float)Math.Pow(1f - rawProgress, 6f); //4f

                    NPC.Center = Vector2.SmoothStep(start, end, easingProgress);

                    NPC.rotation = goalLocation.ToRotation();

                    thrusterValue = 0;

                    if (timer == 85)
                    {
                        //int ball = Projectile.NewProjectile(null, areaCenter, (NPC.rotation + MathHelper.PiOver2 + (Main.rand.NextBool() ? 0 : MathF.PI)).ToRotationVector2() * 12f, ModContent.ProjectileType<DifferentExplodeBall>(), ContactDamage / 4, 0, Main.myPlayer);

                        for (int i = -2; i < 3; i++)
                        {
                            int ball = Projectile.NewProjectile(null, areaCenter + (goalLocation * i * 0.25f), Vector2.Zero, ModContent.ProjectileType<Cyver2EnergyBall>(), ContactDamage / 4, 0, Main.myPlayer);

                            if (Main.projectile[ball].ModProjectile is Cyver2EnergyBall c2)
                                c2.timer = 30;

                        }

                        //int ball1 = Projectile.NewProjectile(null, areaCenter, (NPC.rotation + MathHelper.PiOver2 + 0).ToRotationVector2() * 22f, ModContent.ProjectileType<DifferentExplodeBall>(), ContactDamage / 4, 0, Main.myPlayer);
                        //int ball2 = Projectile.NewProjectile(null, areaCenter, (NPC.rotation + MathHelper.PiOver2 + MathF.PI).ToRotationVector2() * 22f, ModContent.ProjectileType<DifferentExplodeBall>(), ContactDamage / 4, 0, Main.myPlayer);

                    }

                }

                //Reset
                if (timer == 110)
                {
                    bool a = Main.rand.NextBool(); // straight or diagonal
                    bool b = Main.rand.NextBool(); // default or rotated
                    bool c = Main.rand.NextBool(); //Other side

                    goalLocation = new Vector2(610, 0).RotatedBy((c ? 0 : MathHelper.Pi) + (b ? 0 : MathHelper.PiOver2) + (a ? 0 : MathHelper.PiOver4));


                    NPC.velocity = Vector2.Zero;

                    int FX = Projectile.NewProjectile(null, NPC.Center, NPC.velocity, ModContent.ProjectileType<TeleportFXCyver>(), 0, 0, Main.myPlayer);
                    Main.projectile[FX].rotation = NPC.rotation;

                    NPC.rotation = goalLocation.ToRotation();

                    timer = 10; //-15

                    phantomDash1Counter++;

                    if (phantomDash1Counter == 6)
                        advancer++;

                    NPC.dontTakeDamage = true;
                    NPC.hide = true;

                }
            }

            if (advancer == 2)
            {

                if (timer == 100)
                {
                    if (border != null)
                    {
                        if (border.ModProjectile is LightningBorder barrier)
                            barrier.fade = true;
                    }
                }


                if (timer == 140)
                {
                    advancer = 0;
                    timer = -1;
                    phantomDash1Counter = 0;
                    whatAttack = 18;
                }

            }


            timer++;

            #region old
            /*
            /*
                if (timer == 0)
                {
                    //Spawn Border
                    ///Projectile.NewProjectile(null, myPlayer.Center, Vector2.Zero, ModContent.ProjectileType<LightningBorder>(), 0, 0);

                    //int a = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<PhantomLaserTelegraph>(), 2, 0);
                    //if (Main.projectile[a].type == ModContent.ProjectileType<PhantomLaserTelegraph>()) 
                    //stuckLaser = Main.projectile[a];

                    ///areaCenter = myPlayer.Center;
                    //goalLocation = Main.rand.NextVector2CircularEdge(610, 610);

                    bool a = Main.rand.NextBool(); // straight or diagonal
                    bool b = Main.rand.NextBool(); // default or rotated
                    bool c = Main.rand.NextBool(); //Other side

                    goalLocation = new Vector2(610, 0).RotatedBy((c ? 0 : MathHelper.Pi) + (b ? 0 : MathHelper.PiOver2) + (a ? 0 : MathHelper.PiOver4));


                    NPC.velocity = Vector2.Zero;
                }
                

            /*
            //fade
            if (timer == 5)
            {
                NPC.hide = true;
                NPC.dontTakeDamage = true;

                //SOUND EFFECT

                int FX = Projectile.NewProjectile(null, NPC.Center, NPC.velocity, ModContent.ProjectileType<TeleportFXCyver>(), 0, 0, Main.myPlayer);
                Main.projectile[FX].rotation = NPC.rotation;

                NPC.rotation = goalLocation.ToRotation();

            }
            
            //telegraph
            if (timer == 40)
            {
                int tele = Projectile.NewProjectile(null, areaCenter, Vector2.Zero, ModContent.ProjectileType<DookieTelegraph>(), 0, 0);
                Main.projectile[tele].rotation = goalLocation.ToRotation();
            }

            //Put Fadein FX early
            if (timer == 60)
            {

                int FX = Projectile.NewProjectile(null, (goalLocation * 1.10f) + areaCenter, goalLocation.SafeNormalize(Vector2.UnitX) * -10f, ModContent.ProjectileType<TeleportFXCyver>(), 10, 0, Main.myPlayer);
                Main.projectile[FX].rotation = goalLocation.ToRotation();

                if (Main.projectile[FX].ModProjectile is TeleportFXCyver tp) tp.reverse = true;
            }

            //Dash
            if (timer >= 75)
            {
                //Teleport back in
                if (timer == 75)
                {
                    NPC.dontTakeDamage = false;
                    NPC.hide = false;



                    NPC.Center = goalLocation + areaCenter;

                    //int FX = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<TeleportFXCyver>(), 10, 0, Main.myPlayer);
                    //Main.projectile[FX].rotation = goalLocation.ToRotation();

                    //if (Main.projectile[FX].ModProjectile is TeleportFXCyver tp) tp.reverse = true;

                }

                if (timer == 78)
                {
                    //SOUND EFFECT
                    SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = .15f, MaxInstances = -1, };
                    SoundEngine.PlaySound(style, NPC.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = .29f, Pitch = 1f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/flame_thrower_airblast_rocket_redirect") with { Volume = .16f, Pitch = .42f };
                    SoundEngine.PlaySound(style3, NPC.Center);
                }

                Vector2 start = goalLocation + areaCenter;
                Vector2 end = (goalLocation.RotatedBy(MathHelper.Pi) * 1f) + areaCenter;

                float rawProgress = 0f;
                if (timer > 75f)
                    rawProgress = (timer - 75f) / 75f;

                float easingProgress = 1f - (float)Math.Pow(1f - rawProgress, 6f); //4f

                NPC.Center = Vector2.SmoothStep(start, end, easingProgress);

                NPC.rotation = goalLocation.ToRotation();

                thrusterValue = 0;

                if (timer == 85)
                {
                    //int ball = Projectile.NewProjectile(null, areaCenter, (NPC.rotation + MathHelper.PiOver2 + (Main.rand.NextBool() ? 0 : MathF.PI)).ToRotationVector2() * 12f, ModContent.ProjectileType<DifferentExplodeBall>(), ContactDamage / 4, 0, Main.myPlayer);

                    int ball1 = Projectile.NewProjectile(null, areaCenter, (NPC.rotation + MathHelper.PiOver2 + 0).ToRotationVector2() * 22f, ModContent.ProjectileType<DifferentExplodeBall>(), ContactDamage / 4, 0, Main.myPlayer);
                    int ball2 = Projectile.NewProjectile(null, areaCenter, (NPC.rotation + MathHelper.PiOver2 + MathF.PI).ToRotationVector2() * 22f, ModContent.ProjectileType<DifferentExplodeBall>(), ContactDamage / 4, 0, Main.myPlayer);

                }

            }

            //Reset
            if (timer == 110)
            {
                //goalLocation = Main.rand.NextVector2CircularEdge(610, 610);

                bool a = Main.rand.NextBool(); // straight or diagonal
                bool b = Main.rand.NextBool(); // default or rotated
                bool c = Main.rand.NextBool(); //Other side

                goalLocation = new Vector2(610, 0).RotatedBy((c ? 0 : MathHelper.Pi) + (b ? 0 : MathHelper.PiOver2) + (a ? 0 : MathHelper.PiOver4));


                NPC.velocity = Vector2.Zero;

                //will rerun timer == 5 stuff next frame
                timer = 4;


            }

            */

            /*
            //spawn area
            if (timer == 0)
            { 
                //Spawn Border
                Projectile.NewProjectile(null, myPlayer.Center, Vector2.Zero, ModContent.ProjectileType<LightningBorder>(), 0, 0);

                //int a = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<PhantomLaserTelegraph>(), 2, 0);
                //if (Main.projectile[a].type == ModContent.ProjectileType<PhantomLaserTelegraph>()) 
                    //stuckLaser = Main.projectile[a];

                areaCenter = myPlayer.Center;
                goalLocation = Main.rand.NextVector2CircularEdge(510, 510);
            }

            if (timer < 60)
            {
                Vector2 trueGoal = goalLocation.RotatedBy(timer * 0.02f * (rotDir ? 1 : -1));

                //Dust d = Dust.NewDustPerfect(trueGoal + areaCenter, DustID.Torch, Scale: 3);
                //d.noGravity = true;

                NPC.Center = Vector2.Lerp(NPC.Center, areaCenter + trueGoal, 0.025f + (timer * 0.005f)); //0.05

                NPC.Center += Main.rand.NextVector2Circular(1, 1) * (timer * 0.02f);

                NPC.rotation = Utils.AngleLerp(NPC.rotation, trueGoal.ToRotation(), Math.Clamp(timer * 0.015f, 0f, 1f));

                //NPC.rotation = trueGoal.ToRotation(); //(myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;
                NPC.velocity = Vector2.Zero;

                if (timer == 40)
                {
                    eyeStarValue = 1;
                }

            }

            if (timer >= 60)
            {
                thrusterValue = 0;

                if (timer == 60)
                {
                    //NPC.velocity = goalLocation.RotatedBy(timer * 0.02f * (rotDir ? 1 : -1) + MathHelper.Pi).SafeNormalize(Vector2.UnitX) * 25;

                    int a = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<PhantomLaserTelegraph>(), 2, 0);
                    if (Main.projectile[a].type == ModContent.ProjectileType<PhantomLaserTelegraph>())
                        stuckLaser = Main.projectile[a];

                    Vector2 thisTrueGoal = goalLocation.RotatedBy(60 * 0.02f * (rotDir ? 1 : -1));


                    //accelFloat = Math.Clamp(MathHelper.Lerp(accelFloat, 60, 0.1f), 0, 50);  //50 0.1
                    //NPC.rotation = thisTrueGoal.ToRotation();

                    goalLocation = thisTrueGoal.RotatedBy(MathHelper.Pi);

                    //NPC.velocity = storedRotaion.ToRotationVector2() * accelFloat * -1;

                }

                NPC.Center = Vector2.SmoothStep(NPC.Center, (goalLocation * 1.3f) + areaCenter, easeOutQuint((timer - 60f) * 0.004f));

                if (stuckLaser != null && stuckLaser.type == ModContent.ProjectileType<PhantomLaserTelegraph>())
                    stuckLaser.Center = NPC.Center;

                //Main.NewText((timer - 100f) * 0.03f);

                
                if (timer % 7 == 0)
                {
                    int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CyverLaserBomb>(), 20, 0);
                    Main.projectile[a].rotation = NPC.rotation;

                    ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                    for (int i = 0; i < 360; i += 20)
                    {
                        Vector2 circular = new Vector2(40, 0).RotatedBy(MathHelper.ToRadians(i));
                        Vector2 dustVelo = -circular * 0.1f;

                        Dust b = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + circular, ModContent.DustType<GlowCircleDust>(), Vector2.Zero, Color.DeepPink, 0.3f, 0.6f, 0f, dustShader);
                    }
                }
                
                //accelFloat = Math.Clamp(MathHelper.Lerp(accelFloat, 50, 0.1f), 0, 40);  //50 0.1
                //NPC.velocity = NPC.rotation.ToRotationVector2() * accelFloat * -1;
            }

            if (timer == 85)
            {
                int a = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<PhantomLaserTelegraph>(), 2, 0);
                if (Main.projectile[a].type == ModContent.ProjectileType<PhantomLaserTelegraph>())
                    stuckLaser = Main.projectile[a];

                goalLocation = (NPC.Center - areaCenter).SafeNormalize(Vector2.UnitX) * 510;
                //rotDir = !rotDir;//Main.rand.NextVector2CircularEdge(480, 480);
                timer = 2;
            }

            timer++;
            */
            #endregion
        }

        float[] PhantomAngles = new float[6];
        float PhantomOffset = 0f;
        public void NewPhantomDash1(Player myPlayer)
        {
            //LOGIC:
            //Choose point protruding from border
            //Be at that point
            //Dash Towards Center
            //Spawn Laser at border and drop at other end 
            //Drop a dookie 

            //myPlayer.wingTime = myPlayer.wingTimeMax;

            if (timer == 0)
            {
                //Spawn Border
                Projectile.NewProjectile(null, myPlayer.Center, Vector2.Zero, ModContent.ProjectileType<LightningBorder>(), 0, 0);

                //int a = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<PhantomLaserTelegraph>(), 2, 0);

                //if (Main.projectile[a].type == ModContent.ProjectileType<PhantomLaserTelegraph>())
                //stuckLaser = Main.projectile[a];

                areaCenter = myPlayer.Center;
                goalLocation = Main.rand.NextVector2CircularEdge(480, 480);

                NPC.Center = areaCenter + (goalLocation * 3f);

            }

            if (timer >= 10 && timer < 35)
            {
                NPC.velocity = Vector2.Zero;
                if (timer == 10)
                {
                    int tele = Projectile.NewProjectile(null, areaCenter, Vector2.Zero, ModContent.ProjectileType<DookieTelegraph>(), 0, 0);
                    Main.projectile[tele].rotation = goalLocation.ToRotation();
                }
            }

            if (timer >= 35)
            {
                thrusterValue = 0;


                float motherFuck = (timer - 35f) / 55f;

                Main.NewText(motherFuck);

                //Vector2 additional = Vector2.Lerp(new Vector2(-480f, 0f))

                //NPC.Center = Vector2.Lerp(areaCenter + new Vector2(-480f, 0f), areaCenter + new Vector2(480f, 0f), (timer - 35f) / 55f);

                NPC.Center = Vector2.SmoothStep((goalLocation * 3f) + areaCenter, areaCenter + goalLocation.RotatedBy(MathF.PI) * 2f, motherFuck);


                //NPC.Center = Vector2.Lerp(NPC.Center, areaCenter + goalLocation.RotatedBy(MathF.PI) * 2f, motherFuck);
                //NPC.velocity = goalLocation.RotatedBy(MathHelper.Pi).SafeNormalize(Vector2.UnitX) * 30;

                NPC.rotation = goalLocation.ToRotation();
            }

            if (timer == 90)
            {
                NPC.velocity = Vector2.Zero;

                timer = 2;
                goalLocation = Main.rand.NextVector2CircularEdge(480, 480);
                NPC.Center = areaCenter + (goalLocation * 3f);
            }

            timer++;
        }

        public void CCPhantomDash(Player myPlayer)
        {
            //Spawn Area
            fadeDashing = false;
            if (timer == 0)
            {
                //Spawn Border
                //Projectile.NewProjectile(null, myPlayer.Center, Vector2.Zero, ModContent.ProjectileType<LightningBorder>(), 0, 0);

                int FX = Projectile.NewProjectile(null, NPC.Center, NPC.rotation.ToRotationVector2(), ModContent.ProjectileType<TeleportFXCyver>(), 0, 0, Main.myPlayer);
                Main.projectile[FX].rotation = NPC.rotation;

                if (Main.projectile[FX].ModProjectile is TeleportFXCyver tp) tp.reverse = true;

                int a = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<PhantomLaserTelegraph>(), 2, 0);

                if (Main.projectile[a].type == ModContent.ProjectileType<PhantomLaserTelegraph>())
                    stuckLaser = Main.projectile[a];

                //areaCenter = myPlayer.Center;
                goalLocation = Main.rand.NextVector2CircularEdge(480, 480);
            }

            if (advancer == 0)
            {

                //SFX
                if (timer == 2 && penisCounter != 6)
                {

                    if (penisCounter == 5)
                    {
                        SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/SwooshySwoosh") with { Volume = .58f, PitchVariance = .18f, };
                        SoundEngine.PlaySound(style, NPC.Center);
                    }


                    SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/katana_05") with { Volume = .38f, Pitch = .66f, PitchVariance = .18f, };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/StampAirSwing2") with { Volume = .58f, Pitch = .65f, PitchVariance = .18f, };
                    SoundEngine.PlaySound(style3, NPC.Center);

                }

                NPC.Center = Vector2.Lerp(NPC.Center, goalLocation + areaCenter, Math.Clamp(timer * 0.08f, 0, 1f)); //0.01 //4

                thrusterValue = 0;

                Vector2 truGoal = goalLocation + areaCenter;
                NPC.rotation = (truGoal - NPC.Center).ToRotation() + MathHelper.Pi;
                //NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;

                if (stuckLaser.type == ModContent.ProjectileType<PhantomLaserTelegraph>())
                    stuckLaser.Center = NPC.Center;


                if (penisCounter == 6)
                {
                    int FX = Projectile.NewProjectile(null, NPC.Center, NPC.rotation.ToRotationVector2() * -10, ModContent.ProjectileType<TeleportFXCyver>(), 0, 0, Main.myPlayer);
                    Main.projectile[FX].rotation = NPC.rotation;

                    /*
                    foreach (Projectile p in Main.projectile)
                    {
                        if (p.active == true)
                        {
                            if (p.ModProjectile is PhantomLaserTelegraph tele)
                            {
                                tele.Release();
                            }
                        }

                    }
                    */
                    penisCounter = 0;
                    advancer = 1;
                }

                if (timer == 9) //40 //21 //15
                {

                    timer = 1;
                    penisCounter++;
                    //I cant just undo the previous rot
                    goalLocation = goalLocation.RotatedBy(MathHelper.TwoPi * 0.40f); //2.51

                    int a = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<PhantomLaserTelegraph>(), 2, 0);
                    stuckLaser = Main.projectile[a];
                }


            }
            else if (advancer == 1)
            {

                if (timer == 20)
                {
                    foreach (Projectile p in Main.projectile)
                    {
                        if (p.active == true)
                        {
                            if (p.ModProjectile is PhantomLaserTelegraph tele)
                            {
                                tele.Release();
                                SoundStyle style32 = new SoundStyle("AerovelenceMod/Sounds/Effects/laser_line") with { Volume = .2f, Pitch = -.22f, MaxInstances = -1 };
                                SoundEngine.PlaySound(style32, p.Center);

                                //SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/EvilEnergy") with { Pitch = 1f, MaxInstances = -1, Volume = 0.15f }; 
                                //SoundEngine.PlaySound(style, p.Center);
                            }
                        }

                    }



                    //int afg = Projectile.NewProjectile(null, areaCenter, Vector2.Zero, ModContent.ProjectileType<DistortProj>(), 0, 0);
                    //Main.projectile[afg].rotation = Main.rand.NextFloat(6.28f);

                    /*
                    if (Main.projectile[afg].ModProjectile is DistortProj distort)
                    {
                        distort.tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/MagmaBall");
                        distort.implode = true;
                        distort.scale = 2;
                        //distort.
                    }
                    */
                }

                if (timer == 105)
                {
                    eyeStarValue = 1;
                    SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Custom/dd2_phantom_phoenix_shot_2") with { Pitch = .96f, Volume = 0.8f }, NPC.Center);

                    SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/katana_06") with { Pitch = .7f, Volume = 0.15f };
                    SoundEngine.PlaySound(style, NPC.Center);

                    SoundStyle style4 = new SoundStyle("AerovelenceMod/Sounds/Effects/laser_line") with { Volume = .51f, Pitch = .78f, };
                    SoundEngine.PlaySound(style4, NPC.Center);
                }

                if (timer == 120)
                {
                    advancer = 0;
                    timer = -1;
                    whatAttack = 1;

                    if (border != null)
                    {
                        if (border.ModProjectile is LightningBorder barrier)
                            barrier.fade = true;
                    }
                }
            }
            //Go to edge
            //Identify Angle
            // - Get angle through area center
            // - Rotate said angle by amount
            // - Get point to go to 
            //Move to new Angle
            //Repeat x times
            //Fire All Lasers pew pew

            timer++;
        }
        #endregion

        #region Summon Attacks
        //Special Attacks

        //BOT ATTACKS
        public void Bots(Player myPlayer)
        {
            NPC.dontTakeDamage = true;
            //isExpert = true;
            //isMaster = true;

            float bonusBots = isMaster ? 2 : 1;
            float delay = (isExpert || isMaster) ? -10 : 10;
            float bonusBarrages = (isExpert || isMaster) ? 0 : 0;

            if (timer < 100)
            {
                NPC.Center = new Vector2(0, -900) + myPlayer.Center;
                NPC.hide = true;
            }

            if (timer == 50)
            {
                Vector2 randomOut = new Vector2(425, 0).RotatedBy(Main.rand.NextFloat(6.28f));
                float rotDirection = Main.rand.NextBool() ? 1 : -1;
                for (int i = 0; i < 5 + bonusBots; i++)
                {
                    Vector2 spawnPos = randomOut.RotatedBy(MathHelper.ToRadians(360 / (5 + bonusBots)) * i);
                    int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(myPlayer.Center.X + (spawnPos.X * 2)), (int)(myPlayer.Center.Y + (spawnPos.Y * 2)), ModContent.NPCType<CyverBotOrbiter>());
                    NPC laser = Main.npc[index];
                    laser.damage = 0;
                    if (laser.ModNPC is CyverBotOrbiter bot)
                    {
                        bot.rotDir = rotDirection;
                        bot.State = (int)CyverBotOrbiter.Behavior.StarStrikeP1;
                        bot.GoalPoint = spawnPos;
                    }
                }
            }

            if (timer == 120 + delay)
            {
                timer = 49;
                if (advancer == 5 + bonusBarrages)
                {
                    NPC.Center = myPlayer.Center - new Vector2(-1000, 400);
                    timer = -1;
                    advancer = -1;
                    startingQuadrant = 1;
                    SetNextAttack("Barrage");
                    NPC.dontTakeDamage = false;
                    NPC.hide = false;
                }
                advancer++;
            }




            timer++;
        }

        public void Clones(Player myPlayer)
        {
            NPC.dontTakeDamage = true;

            float delay = (isExpert || isMaster) ? 0 : 5;

            if (timer == 0)
            {
                int FX = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<TeleportFXCyver>(), 0, 0, Main.myPlayer);
                Main.projectile[FX].rotation = NPC.rotation;
            }
            else
            {

                if (timer == 60 + delay)
                {
                    float extraSpacing = Phase3 ? 0 : 0;

                    float forValue = isExpert ? -1 : 0;
                    forValue = isMaster ? -1 : forValue;


                    if (advancer % 2 == 0)
                    {
                        //Right
                        for (int i = (int)forValue; i < forValue * -1 + 1; i++)
                        {
                            Vector2 goalLocation = new Vector2(350, (isMaster ? 200 : 150) * i);

                            if (!(Phase3 && i == 100))
                            {
                                int cloneIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), myPlayer.Center + goalLocation * 2, Vector2.Zero, ModContent.ProjectileType<ShadowClone>(), GetDamage("CloneP3"), 2, -1);
                                Projectile Clone = Main.projectile[cloneIndex];

                                if (Clone.ModProjectile is ShadowClone dashers)
                                {
                                    dashers.SetGoalPoint(goalLocation);
                                }
                            }

                        }
                        //Left
                        for (int i = (int)forValue; i < forValue * -1 + 1; i++)
                        {

                            Vector2 goalLocation = new Vector2(-350, (isMaster ? 200 : 150) * i);

                            if (!(Phase3 && i == 100))
                            {
                                int cloneIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), myPlayer.Center + goalLocation * 2, Vector2.Zero, ModContent.ProjectileType<ShadowClone>(), GetDamage("CloneP3"), 2, -1);
                                Projectile Clone = Main.projectile[cloneIndex];

                                if (Clone.ModProjectile is ShadowClone dashers)
                                {
                                    dashers.SetGoalPoint(goalLocation);
                                }
                            }

                        }

                    }
                    else
                    {
                        //Top
                        for (int i = (int)forValue; i < forValue * -1 + 1; i++)
                        {
                            Vector2 goalLocation = new Vector2((isMaster ? 200 : 150) * i, -350);

                            if (!(Phase3 && i == 100))
                            {
                                int cloneIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), myPlayer.Center + goalLocation * 2, Vector2.Zero, ModContent.ProjectileType<ShadowClone>(), GetDamage("CloneP3"), 2, -1);
                                Projectile Clone = Main.projectile[cloneIndex];

                                if (Clone.ModProjectile is ShadowClone dashers)
                                {
                                    dashers.SetGoalPoint(goalLocation);
                                }
                            }

                        }

                        //Bottom
                        for (int i = (int)forValue; i < forValue * -1 + 1; i++)
                        {
                            Vector2 goalLocation = new Vector2((isMaster ? 200 : 150) * i, 350);

                            if (!(Phase3 && i == 100))
                            {

                                int cloneIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), myPlayer.Center + goalLocation * 2, Vector2.Zero, ModContent.ProjectileType<ShadowClone>(), GetDamage("CloneP3"), 2, -1);
                                Projectile Clone = Main.projectile[cloneIndex];

                                if (Clone.ModProjectile is ShadowClone dashers)
                                {
                                    dashers.SetGoalPoint(goalLocation);
                                }
                            }

                        }
                        /*
                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 goalLocation = new Vector2(0, 350 - (700 * i));
                            int cloneIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), myPlayer.Center + goalLocation * 2, Vector2.Zero, ModContent.ProjectileType<ShadowClone>(), ContactDamage / 4, 2, Main.myPlayer);
                            Projectile Clone = Main.projectile[cloneIndex];

                            if (Clone.ModProjectile is ShadowClone dashers)
                            {
                                dashers.SetGoalPoint(goalLocation);
                            }

                        }
                        */
                    }


                    advancer++;
                }

                NPC.hide = true;
                NPC.Center = myPlayer.Center + new Vector2(0, 500);

                if (timer == 75 + delay)
                {
                    timer = 30;
                }
            }

            if (advancer == 5 && timer == 50)
            {
                timer = -1;
                whatAttack = 19;
                advancer = 0;

                //Want Cyver to stay hidden after this attack
                //NPC.dontTakeDamage = false;
                //NPC.hide = false;
                
            }

            timer++;
        }

        public void ClonesP3(Player myPlayer)
        {
            if (timer == 0)
            {
                if (advancer == 0)
                {
                    int FX = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<TeleportFXCyver>(), 0, 0, Main.myPlayer);
                    Main.projectile[FX].rotation = NPC.rotation;
                }

                float startingRotation = Main.rand.NextFloat(6.28f);
                bool whichSide = Main.rand.NextBool();
                bool topOrBottom = Main.rand.NextBool();

                for (int i = -2; i < 3; i++)
                {
                    Vector2 goalLocation = new Vector2(0, 440f).RotatedBy(startingRotation + MathHelper.ToRadians(45 * i));

                    //Vector2 goalLocation = new Vector2(0, 470 * (topOrBottom ? -1 : 1)).RotatedBy(startingRotation + MathHelper.ToRadians(36 * i * (whichSide ? -1 : 1)));
                    int cloneIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), myPlayer.Center + goalLocation * 2, Vector2.Zero, ModContent.ProjectileType<ShadowClone>(), ContactDamage / 4, 2, Main.myPlayer);
                    Projectile Clone = Main.projectile[cloneIndex];

                    if (Clone.ModProjectile is ShadowClone dashers)
                    {
                        dashers.dashSpeed = 18.5f; //18.5f
                        dashers.SetGoalPoint(goalLocation);
                    }
                }
            }

            if (timer == 90)
            {
                timer = -1;
                advancer++;

                if (advancer == 6)
                {
                    whatAttack = 19;
                    advancer = 0;

                }
            }

            NPC.dontTakeDamage = true;
            NPC.hide = true;
            NPC.Center = myPlayer.Center + new Vector2(-500, 1000);

            timer++;
        }

        float pinkCloneDashValue = 4;
        Projectile pinkProj = null;
        public void PinkCloneP3(Player myPlayer)
        {

            int startingDelay = advancer == 0 ? 50 : 0;

            int cloneDashSpeed = isExpert ? 20 : 18;

            if (timer == 0 + startingDelay)
            {
                float startingRotation = Main.rand.NextFloat(6.28f);

                for (int i = 0; i < 5; i++)
                {
                    Vector2 goalLocation = new Vector2(0, 450).RotatedBy(startingRotation + ((MathHelper.TwoPi / 5) * i));

                    if (i != 0)
                    {
                        int cloneIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), myPlayer.Center + goalLocation * 2, Vector2.Zero, ModContent.ProjectileType<ShadowClone>(), DamageValues["PinkCloneClone"], 2, Main.myPlayer);
                        Projectile Clone = Main.projectile[cloneIndex];

                        if (Clone.ModProjectile is ShadowClone dashers)
                        {
                            dashers.dashSpeed = 20;
                            dashers.SetGoalPoint(goalLocation);
                        }
                    }
                    else
                    {
                        int cloneIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), myPlayer.Center + goalLocation * 2, Vector2.Zero, ModContent.ProjectileType<ShadowClonePink>(), 0, 0, Main.myPlayer);
                        Projectile Clone = Main.projectile[cloneIndex];

                        pinkProj = Clone;

                        if (Clone.ModProjectile is ShadowClonePink dashers)
                        {
                            dashers.SetGoalPoint(goalLocation);
                        }
                    }

                }

            }

            if (timer >= 115 + startingDelay && timer < 175 + startingDelay)
            {
                if (timer == 115 + startingDelay)
                {
                    NPC.dontTakeDamage = false;
                    NPC.hide = false;

                    previousPositions.Clear();
                    previousRotations.Clear();

                    if (pinkProj != null && pinkProj.active == true && pinkProj.type == ModContent.ProjectileType<ShadowClonePink>())
                    {
                        NPC.Center = pinkProj.Center;
                        NPC.rotation = pinkProj.rotation;
                        pinkProj.active = false;
                    }
                    else Main.NewText("Very bad... (PinkProjIssue)");

                    pinkProj.active = false;

                    //int FX = Projectile.NewProjectile(null, NPC.Center, NPC.rotation.ToRotationVector2() * -16, ModContent.ProjectileType<TeleportFXCyver>(), 0, 0, Main.myPlayer);
                    //Main.projectile[FX].rotation = NPC.rotation;
                    //if (Main.projectile[FX].ModProjectile is TeleportFXCyver tp) tp.reverse = true;

                    SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = .2f, MaxInstances = -1, };
                    SoundEngine.PlaySound(style, NPC.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = .5f, Pitch = 1f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/flame_thrower_airblast_rocket_redirect") with { Volume = .12f, Pitch = .3f };
                    SoundEngine.PlaySound(style3, NPC.Center);

                    extraBoost = 1f;
                    Common.Systems.FlashSystem.SetCAFlashEffect(0.5f, 40, 1f, 0.5f, false, true);
                    squashPower = 1f;

                    myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 15;
                    eyeStarValue = 1;

                    int afg = Projectile.NewProjectile(null, NPC.Center, NPC.velocity.SafeNormalize(Vector2.UnitX) * 1f, ModContent.ProjectileType<DistortProj>(), 0, 0);
                    Main.projectile[afg].rotation = Main.rand.NextFloat(6.28f);
                    Main.projectile[afg].timeLeft = 10;

                    if (Main.projectile[afg].ModProjectile is DistortProj distort)
                    {
                        distort.tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/MagmaBall");
                        distort.implode = false;
                        distort.scale = 0.6f;
                    }

                    for (int i = -10; i < 11; i++)
                    {
                        int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2().RotatedBy(0.3f * i) * -0.7f, ModContent.ProjectileType<StretchLaser>(),
                        DamageValues["PinkCloneLaser"], 2);
                        Main.projectile[a].timeLeft = 400;
                        if (Main.projectile[a].ModProjectile is StretchLaser laser)
                        {
                            laser.accelerateTime = 400;
                            laser.accelerateStrength = 1.017f; //1.025
                        }
                    }

                    for (int m = 0; m < 12 + Main.rand.Next(0, 4); m++)
                    {
                        Color col = new Color(78, 225, 245);
                        Dust d = Dust.NewDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -10, ModContent.DustType<MuraLineBasic>(),
                            NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(0.75f, 3.5f) * 5f, newColor: col, Scale: Main.rand.NextFloat(0.6f, 0.9f));
                        d.fadeIn = 1f + Main.rand.NextFloat(-0.4f, 0f);
                        d.alpha = 15 + Main.rand.Next(-2, 1);
                    }
                    justDashValue = 1f;

                    overlayPower = 1f;
                    lineBonusSpeed = 1.5f;
                    thrusterValue = 0;
                }


                if (timer < 125 + startingDelay)
                    extraBoost = 1f;

                if (timer < 132 + startingDelay)
                    pinkCloneDashValue *= 1.17f; //1.13f

                NPC.velocity = -pinkCloneDashValue * NPC.rotation.ToRotationVector2();

                if (NPC.velocity.Length() > 20)
                    Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(10, 10), ModContent.DustType<MuraLineBasic>(), NPC.velocity.RotateRandom(0.25f) * 0.3f, 13, new Color(0, 255, 255), 0.45f);

                //Main.NewText((float)((timer - 120f) / 40f));
                NPC.damage = ContactDamage;
            }

            if (timer == 170 + startingDelay) //180
            {
                pinkCloneDashValue = 4; //2
                timer = -1;
                advancer++;

                if (advancer == 4)
                {
                    SetNextAttack("Barrage");
                    timer = -1;
                    advancer = 0;

                    NPC.dontTakeDamage = false;
                    NPC.hide = false;

                    NPC.Center = myPlayer.Center + new Vector2(-500f, 1000f);
                }
            }
            else if (timer < 115 + startingDelay && timer >= 1)
            {
                NPC.velocity = Vector2.Zero;

                //Put NPC on the pink clone
                NPC.dontTakeDamage = false;
                NPC.hide = true;

                if (pinkProj != null && pinkProj.active == true && pinkProj.type == ModContent.ProjectileType<ShadowClonePink>())
                {
                    NPC.Center = pinkProj.Center;
                    NPC.rotation = pinkProj.rotation;
                }
            }

            timer++;

            //Spawn 5 Clones
            //Set one of them to be pink

            //After a while, make cyver dash from pinks position
        }

        int newBotsReps = 0;
        public void NewBots(Player myPlayer)
        {
            //Values

            //Classic -> 2 bots, longer time, 1 less rep
            //Expert -> 4 bots, longer time
            //Master -> 4 bots, -5 frame time between waves, MathHelper.PiOver2 + 1.75f

            //Phase 3 --> 4 bots

            int numberOfBots = Phase3 ? 4 : 2;
            float rotationSpeed = isMaster ? 0.032f : (isExpert ? 0.028f : 0.026f); //35 : 30

            int totalReps = 5;
            int timeBetweenWaves = isMaster ? 140 : 145; //180
            int startTime = 30;

            //Have Cyver disappear 
            NPC.hide = true;
            NPC.dontTakeDamage = true;
            if (timer == 0 && newBotsReps == 0)
            {
                //FadeFX
                int FX = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<TeleportFXCyver>(), 0, 0, Main.myPlayer);
                Main.projectile[FX].rotation = NPC.rotation;
            }

            //Spawn Bots
            if (timer == startTime)
            {
                bool randomDir = Main.rand.NextBool();

                float rotationOffset = numberOfBots > 0 ? Main.rand.NextFloat(6.28f) : 0;
                for (int i = 0; i < numberOfBots; i++)
                {
                    //Assign where bots will start at
                    Vector2 spawnPos = new Vector2(400, 0).RotatedBy(rotationOffset + MathHelper.ToRadians(360 / numberOfBots) * i);

                    Vector2 trueSpawnPos = (spawnPos * 2.5f) + myPlayer.Center;

                    int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)trueSpawnPos.X, (int)trueSpawnPos.Y, ModContent.NPCType<CyverBot>(), myPlayer.whoAmI);
                    NPC thisBot = Main.npc[index];
                    thisBot.ai[1] = randomDir ? 1f : -1f;
                    thisBot.damage = 0;
                    if (thisBot.ModNPC is CyverBot bot)
                    {
                        bot.CyverIndex = NPC.whoAmI;

                        int version = (newBotsReps == totalReps) ? (int)(CyverBot.Behavior.PrimeLaserLong) : (int)(CyverBot.Behavior.PrimeLaser);

                        bot.State = version;
                        bot.rotIntensity = rotationSpeed;
                        bot.setGoalLocation(spawnPos);

                        if (i == 0 && (newBotsReps == totalReps)) //Makes the ball occur so only do so on the last wave
                            bot.Leader = true;
                    }
                }
            }


            //Reset normal
            if (timer == startTime + timeBetweenWaves && newBotsReps != totalReps)
            {
                timer = startTime - 1;
                newBotsReps++;


                //Next Attack
                if (newBotsReps >= totalReps)
                {
                    timer = -1;
                    newBotsReps = 0;
                    SetNextAttack("Barrage");

                    previousPositions.Clear();
                    previousRotations.Clear();
                    NPC.dontTakeDamage = false;
                    NPC.hide = false;

                    NPC.Center = myPlayer.Center + new Vector2(-300, 700);
                }
            }

            if (timer == 700)
            {
                //Next Attack
                if (newBotsReps == totalReps)
                {
                    timer = -1;
                    newBotsReps = 0;
                    SetNextAttack("Barrage");

                    previousPositions.Clear();
                    previousRotations.Clear();
                }
            }

            timer++;
        }
        #endregion

        #endregion

        bool cutsceneDeathFinished = false;
        bool DrawDeathOrb = false;
        float deathOrbScale = 0f;

        bool dead = false;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (!dead && NPC.life < 1)
            {
                if (NPC.velocity.Length() > 100f)
                {
                    NPC.velocity = NPC.velocity.ToRotation().ToRotationVector2() * 10f;
                }
                NPC.life = 1;
                dead = true;
                NPC.dontTakeDamage = true;
                whatAttack = -1;
                timer = -1;
                advancer = 0;
                NPC.hide = false;
                fadeDashing = false;
            }

            //SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/MetalClank") with { Pitch = -.19f, PitchVariance = .22f, MaxInstances = 1, Volume = 0.3f }; 
            //SoundEngine.PlaySound(style, NPC.Center);
        }

        //Turns things like the Eye sword projectile off
        public void OnDespawnCleanup(Player myPlayer) 
        {
            if (eyeSwordInstance != null)
                eyeSwordInstance.active = false;
            if (pinkProj != null)
                pinkProj.active = false;

            //Fade out Proj
            int FX = Projectile.NewProjectile(null, NPC.Center, NPC.velocity * 0.25f, ModContent.ProjectileType<TeleportFXCyver>(), 0, 0, Main.myPlayer);
            Main.projectile[FX].rotation = NPC.rotation;

            //Safety Check
            myPlayer.GetModPlayer<ScreenPlayer>().lerpBackToPlayer = true;
            myPlayer.GetModPlayer<ScreenPlayer>().cutscene = false;
        }

        public void DeathAnimation(Player myPlayer)
        {
            if (timer == 0)
            {
                lineBonusSpeed = 0f;
                Common.Systems.FlashSystem.SetFlashEffect(2f, 20);

                myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 400;


                Vector2 from = NPC.Center - new Vector2(80, 0).RotatedBy(NPC.rotation);

                int hitFlare = Projectile.NewProjectile(null, from, NPC.velocity * 0.25f, ModContent.ProjectileType<Items.Weapons.Aurora.Eos.EosHitFlare>(), 0, 0, Main.myPlayer);
                Main.projectile[hitFlare].scale = 1.5f;
                Main.projectile[hitFlare].rotation = NPC.rotation + MathHelper.PiOver4;


                SoundStyle slash = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Impact_Sword_L_a") with { Pitch = 1f, PitchVariance = .1f, Volume = 0.2f };
                SoundEngine.PlaySound(slash, NPC.Center);

                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/katana_impact_object_03") with { Volume = .4f, Pitch = .84f, };
                SoundEngine.PlaySound(style2, NPC.Center);

                ballScale = 0f;
                spammingLaser = true;

                //Set hitsound volume to 0f so we don't hear it when we strike later
                NPC.HitSound = SoundID.NPCHit4 with { Volume = 0f };

                if (eyeSwordInstance != null)
                    eyeSwordInstance.active = false;
                if (pinkProj != null)
                    pinkProj.active = false;

                DrawDeathOrb = true;
                NPC.hide = false;
                fadeDashing = false;
            }

            NPC.dontTakeDamage = true;
            myPlayer.GetModPlayer<ScreenPlayer>().cutscene = true;
            myPlayer.GetModPlayer<ScreenPlayer>().ScreenGoalPos = NPC.Center;

            //Slow Down NPC vel to 0, first check is for if you caught it mid dash or something
            if (NPC.velocity.Length() > 30)
                NPC.velocity *= 0.5f;
            NPC.velocity *= 0.98f;

            //Smoke dust
            if (timer < 219)
            {
                eyeStarValue = MathF.Abs(MathF.Cos(timer / 11f)) * 0.5f; ;
                phase3PulseValue = MathF.Abs(MathF.Sin(timer / 8f)) * 0.5f;
                lineBonusSpeed += 0.15f;

                //lol lmao too lazy to be smart rn
                // 70 | 120 | 145 | 160 | 
                // 85 | 120 | 140 | 155 |
                if (timer == 80 || timer == 118 || timer == 140 || timer == 155 || timer == 168 || timer == 175 || timer == 182 || timer == 190 || timer == 197 || timer == 204 || timer == 211)
                {
                    Vector2 randomPos = new Vector2(Main.rand.NextFloat(-60, 60), Main.rand.NextFloat(-40, 40)).RotatedBy(NPC.rotation) + NPC.Center;
                    //Vector2 randomPos = new Vector2(0f, 40f).RotatedBy(NPC.rotation) + NPC.Center;

                    for (int i = 0; i < 7; i++)
                    {
                        var v = Main.rand.NextVector2Unit() * 1f; //3
                        Dust portalGunTrail = Dust.NewDustPerfect(randomPos, DustID.PortalBoltTrail, v * Main.rand.NextFloat(1f, 6f), 0,
                            Color.DeepPink, Main.rand.NextFloat(0.2f, 0.5f) * 1.75f);
                        portalGunTrail.alpha = 50;

                        if (portalGunTrail.velocity.Y > 4)
                            portalGunTrail.velocity.Y *= -1f;

                        if (Main.rand.NextBool())
                            portalGunTrail.velocity.Y = MathF.Abs(portalGunTrail.velocity.Y) * -1;

                    }

                    int numberOfSmoke = timer > 195 ? 25 : 15;
                    int explosion = Projectile.NewProjectile(null, randomPos, Vector2.Zero, ModContent.ProjectileType<FadeExplosionHandler>(), 0, 0, Main.myPlayer);
                    if (Main.projectile[explosion].ModProjectile is FadeExplosionHandler feh)
                    {
                        feh.color = Color.HotPink;
                        feh.colorIntensity = 0.75f;
                        feh.fadeSpeed = 0.04f;

                        for (int m = 0; m < numberOfSmoke; m++)
                        {
                            FadeExplosionClass newSmoke = new FadeExplosionClass(Main.projectile[explosion].Center, new Vector2(5f, 0).RotatedByRandom(6.28f) * Main.rand.NextFloat(0.5f, 2f));

                            newSmoke.shouldSlow = true;
                            newSmoke.slowAmount = 0.85f;
                            newSmoke.size = 0.3f + Main.rand.NextFloat(-0.15f, 0.15f);
                            feh.Smokes.Add(newSmoke);

                        }
                    }

                    for (int fg = 0; fg < 10; fg++)
                    {
                        Vector2 randomStart = Main.rand.NextVector2CircularEdge(4, 4);
                        Dust gd = Dust.NewDustPerfect(randomPos, ModContent.DustType<GlowPixelAlts>(), randomStart * Main.rand.NextFloat(0.3f, 1.35f) * 1.5f, newColor: Color.DeepPink, Scale: Main.rand.NextFloat(1f, 1.4f) * 0.6f);
                        gd.alpha = 2;
                    }

                    SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Pitch = 0.7f, PitchVariance = 0.2f, Volume = 0.8f }, NPC.Center);
                    SoundEngine.PlaySound(SoundID.NPCHit4 with { Pitch = -1f, PitchVariance = 0.2f, Volume = 0.2f }, NPC.Center);

                }
            }

            //Explosion fx, sound fx
            if (timer == 219)
            {
                deathOrbScale = 0f;

                //lmao
                SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .45f, Pitch = .93f, PitchVariance = .11f, MaxInstances = -1 };
                SoundEngine.PlaySound(styleb, NPC.Center);

                SoundStyle styla = new SoundStyle("Terraria/Sounds/Item_122") with { Pitch = .44f, Volume = 0.9f, PitchVariance = 0.11f };
                SoundEngine.PlaySound(styla, NPC.Center);

                SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_109") with { Volume = .51f, Pitch = -.55f, PitchVariance = 0.2f };
                SoundEngine.PlaySound(stylea, NPC.Center);

                SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion with { Pitch = 0.6f, Volume = 0.5f }, NPC.Center);

                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Pitch = 0.7f, PitchVariance = 0.2f, Volume = 0.8f }, NPC.Center);

                SoundStyle electric = new SoundStyle("AerovelenceMod/Sounds/Effects/ElectricExplode") with { Volume = .21f, Pitch = .61f, };
                SoundEngine.PlaySound(electric, NPC.Center);

                SoundStyle styleMetal = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/demo_charge_hit_world1") with { Volume = .25f, Pitch = 0, };
                SoundEngine.PlaySound(styleMetal, NPC.Center);

                NPC.DeathSound = SoundID.ScaryScream with { Pitch = 1f, Volume = 0f };

                int explosion = Projectile.NewProjectile(null, NPC.Center + new Vector2(5f, 0).RotatedByRandom(6.28f) * Main.rand.NextFloat(0.7f, 1.5f), Vector2.Zero,
                    ModContent.ProjectileType<FadeExplosionHandler>(), 0, 0, Main.myPlayer);

                if (Main.projectile[explosion].ModProjectile is FadeExplosionHandler feh)
                {
                    feh.color = Color.DeepPink;
                    feh.colorIntensity = 0.75f;
                    feh.fadeSpeed = 0.035f;
                    for (int m = 0; m < 10; m++)
                    {
                        FadeExplosionClass newSmoke = new FadeExplosionClass(Main.projectile[explosion].Center, new Vector2(2f, 0).RotatedByRandom(6.28f) * Main.rand.NextFloat(0.7f, 1.5f));

                        newSmoke.shouldSlow = true;
                        newSmoke.slowAmount = 0.95f;
                        newSmoke.size = 0.8f;
                        feh.Smokes.Add(newSmoke);

                    }
                }

                for (int fg = 0; fg < 60; fg++)
                {
                    Vector2 randomStart = Main.rand.NextVector2CircularEdge(3, 3);
                    Dust gd = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowPixelAlts>(), randomStart * Main.rand.NextFloat(0.3f, 3.35f) * 4f, newColor: Color.HotPink, Scale: Main.rand.NextFloat(1f, 1.6f) * 2f);
                }

                for (int i = 0; i < 15; i++)
                {
                    var v = Main.rand.NextVector2Unit() * 3;
                    Dust sa = Dust.NewDustPerfect(NPC.Center, DustID.PortalBoltTrail, v * Main.rand.NextFloat(1f, 6f), 0,
                        Color.DeepPink, Main.rand.NextFloat(0.4f, 0.9f) * 3);
                }

                Common.Systems.FlashSystem.SetFlashEffect(4f, 60);
                int Explo = Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<CyverDeathExplosion>(), 0, 0, Main.myPlayer);

                myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 70;

                NPC.hide = true;

                lineBonusSpeed = -1.5f;

            }
            else if (timer == 239) //160
            {
                cutsceneDeathFinished = true;
            }

            if (timer <= 219)
            {
                Main.GameZoomTarget = MathHelper.Lerp(Main.GameZoomTarget, 1.3f, 0.02f); 

                //Lmao
                if (timer == 85 || timer == 118 || timer == 140 || timer == 155 || timer == 168 || timer == 175 || timer == 182 || timer == 190 || timer == 197 || timer == 204 || timer == 211)
                    Main.GameZoomTarget += 0.02f;

                if (timer == 219)
                {
                    Main.GameZoomTarget -= 0.04f;
                }
            }
            else
            {
                Main.GameZoomTarget = Math.Clamp(MathHelper.Lerp(Main.GameZoomTarget, 0.8f, 0.05f), 1, 2);
            }

            if (cutsceneDeathFinished)
            {
                myPlayer.GetModPlayer<ScreenPlayer>().lerpBackToPlayer = true;
                myPlayer.GetModPlayer<ScreenPlayer>().cutscene = false;

                NPC.StrikeInstantKill();

                //NPC.StrikeNPC(NPC.lifeMax, 0f, 0, false, noEffect: true);
                spammingLaser = false;
                Main.GameZoomTarget = 1;

                timer = 2;
            }
            deathOrbScale += 0.008f;
            timer++;
        }

        Vector2 introCenter = Vector2.Zero;
        public void IntroAnimation(Player myPlayer)
        {
            int timeHelper = 10;

            if (timer == 0)
            {
                introCenter = myPlayer.Center + new Vector2(-600, -200);
                NPC.Center = new Vector2(-600, -1900) + myPlayer.Center;

                myPlayer.GetModPlayer<ScreenPlayer>().cutscene = true;
                myPlayer.GetModPlayer<ScreenPlayer>().ScreenGoalPos = introCenter;
            }

            Vector2 eyeOff = new Vector2(-50, 0).RotatedBy(NPC.rotation);
            if (timer < 250 + timeHelper)
                myPlayer.GetModPlayer<ScreenPlayer>().ScreenGoalPos = Vector2.Lerp(myPlayer.GetModPlayer<ScreenPlayer>().ScreenGoalPos, introCenter, 0.2f);
            else
                myPlayer.GetModPlayer<ScreenPlayer>().ScreenGoalPos = Vector2.Lerp(myPlayer.GetModPlayer<ScreenPlayer>().ScreenGoalPos, introCenter + eyeOff, 0.05f);

            Main.GameZoomTarget = MathHelper.Lerp(Main.GameZoomTarget, 1.15f, 0.03f);
            NPC.dontTakeDamage = true;

            NPC.rotation = MathHelper.ToRadians(180) + new Vector2(0, 1).ToRotation();

            if (timer > 50 + timeHelper && timer < 150 + timeHelper)
            {
                if (timer == 60 + timeHelper)
                {
                    SoundStyle styleas = new SoundStyle("Terraria/Sounds/Item_131") with { Pitch = -.2f, PitchVariance = .13f, Volume = 0.4f };
                    SoundEngine.PlaySound(styleas, introCenter);

                    SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/flame_thrower_airblast_rocket_redirect") with { Volume = .05f, Pitch = 0.25f, };
                    SoundEngine.PlaySound(style2, introCenter);

                    myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 10;

                }
                //move to overshoot
                Vector2 move = (introCenter + new Vector2(0, 1600)) - NPC.Center;

                float scalespeed = 0.8f; //2

                NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
                NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;

                NPC.rotation = MathHelper.ToRadians(180) + new Vector2(0, 1).ToRotation();

                thrusterValue = 0f;
            }


            if (timer >= 125 + timeHelper)
            {
                if (timer == 125 + timeHelper)
                {
                    NPC.Center = introCenter + new Vector2(0, -900);
                }

                //NPC.rotation = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();
                NPC.rotation = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();
                Vector2 move = (introCenter) - NPC.Center;

                float scalespeed = 0.75f; //0.7

                NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
                NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;

                if (timer > 230 + timeHelper && timer < 280 + timeHelper)
                {
                    if (timer < 250 + timeHelper)
                    {
                        for (int a = 0; a < 2 + Main.rand.Next(0, 2); a++)
                        {
                            Vector2 vel = Main.rand.NextVector2CircularEdge(20f, 20f) * Main.rand.NextFloat(1f, 2.2f);
                            float dustScale = Main.rand.NextFloat(0.5f, 0.8f);
                            Color col = a == 2 ? Color.SkyBlue : Color.HotPink;
                            Dust d = Dust.NewDustPerfect(NPC.Center + new Vector2(-80, 0).RotatedBy(NPC.rotation), ModContent.DustType<MuraLineBasic>(), vel, 10, col, dustScale);
                            //d.fadeIn = 10;
                        }

                        myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 30f;


                        if (timer % 4 == 0)
                        {
                            eyeStarValue = 0.4f;
                            eyeStarRotation = Main.rand.NextFloat(6.28f);

                        }

                        justDashValue = 1f;
                        phase3PulseValue = 1f;
                    }


                    if (timer == 231 + timeHelper)
                    {
                        SoundStyle style2 = new SoundStyle("Terraria/Sounds/Zombie_67") with { Pitch = -.5f, PitchVariance = 0f, MaxInstances = -1, Volume = 0.5f };
                        SoundEngine.PlaySound(style2, NPC.Center);

                        SoundStyle style3 = new SoundStyle("Terraria/Sounds/Zombie_68") with { Pitch = .47f, PitchVariance = 0f, MaxInstances = -1, Volume = 0.65f };
                        SoundEngine.PlaySound(style3, NPC.Center);

                        SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/EvilEnergy") with { Volume = .3f, Pitch = -.21f, MaxInstances = -1 };
                        SoundEngine.PlaySound(style, NPC.Center);

                        SoundStyle style4 = new SoundStyle("AerovelenceMod/Sounds/Effects/EvilEnergy") with { Volume = .4f, Pitch = .6f, MaxInstances = -1 };
                        SoundEngine.PlaySound(style4, NPC.Center);

                        int b = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-68, 0).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<CyverRoarPulse>(), 0, 0f);
                        int b2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-68, 0).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<CyverRoarPulse>(), 0, 0f);
                        Main.projectile[b].scale = 4f;
                        Main.projectile[b].ai[0] = 1f;
                        Main.projectile[b2].scale = 3f;
                        Main.projectile[b2].ai[0] = -1f;

                        if (Main.projectile[b].ModProjectile is CyverRoarPulse crp1)
                        {
                            crp1.pixel = false;
                            crp1.forRoar = false;
                        }
                        if (Main.projectile[b2].ModProjectile is CyverRoarPulse crp2) crp2.pixel = false;

                        for (int a = 0; a < 10 + Main.rand.Next(0, 2); a++)
                        {
                            Vector2 vel = Main.rand.NextVector2CircularEdge(20f, 20f) * Main.rand.NextFloat(1f, 2.2f);
                            float dustScale = Main.rand.NextFloat(0.5f, 0.8f);
                            Color col = a >= 8 ? Color.SkyBlue : Color.HotPink;
                            Dust d = Dust.NewDustPerfect(NPC.Center + new Vector2(-80, 0).RotatedBy(NPC.rotation), ModContent.DustType<MuraLineBasic>(), vel, 10, col, dustScale);
                            //d.fadeIn = 10;
                        }

                        //myPlayer.GetModPlayer<ScreenPlayer>().ScreenGoalPos = NPC.Center + new Vector2(95, 0).RotatedBy(NPC.rotation);
                        Main.GameZoomTarget = 1.65f; //1.75
                        phase3PulseValue = 1f;
                    }

                }
            }
            if (timer == 280 + timeHelper)
            {
                myPlayer.GetModPlayer<ScreenPlayer>().lerpBackToPlayer = true;
                myPlayer.GetModPlayer<ScreenPlayer>().cutscene = false;
            }

            if (timer >= 280 + timeHelper)
            {
                Main.GameZoomTarget = Math.Clamp(MathHelper.Lerp(Main.GameZoomTarget, 0.8f, 0.05f), 1, 2);
            }

            if (timer == 330 + timeHelper)
            {
                whatAttack = 1;
                myPlayer.GetModPlayer<ScreenPlayer>().lerpBackToPlayer = true;
                myPlayer.GetModPlayer<ScreenPlayer>().cutscene = false;
                timer = -1;
                startingAngBonus = 0;
                startingQuadrant = 3;
                NPC.dontTakeDamage = false;
            }

            timer++;
        }

        public void PhaseTransition(Player myPlayer)
        {
            if (timer == 0)
            {
                Common.Systems.FlashSystem.SetCAFlashEffect(1f, 40, 1f, 1f, false, true);

                myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 100;

                SoundStyle slash = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Impact_Sword_L_a") with { Pitch = 1f, PitchVariance = .1f, Volume = 0.2f };
                SoundEngine.PlaySound(slash, NPC.Center);

                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/katana_impact_object_03") with { Volume = .4f, Pitch = 1f, };
                SoundEngine.PlaySound(style2, NPC.Center);

                Vector2 towardPlayer = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX);

                NPC.velocity = towardPlayer * -18f; //20

                NPC.hide = false;
                fadeDashing = false;
            }

            NPC.dontTakeDamage = true;
            myPlayer.GetModPlayer<ScreenPlayer>().cutscene = true;
            myPlayer.GetModPlayer<ScreenPlayer>().ScreenGoalPos = NPC.Center;

            if (timer == 85)
                NPC.velocity *= 0.5f;

            if (NPC.velocity.Length() > 2f)
                NPC.velocity *= 0.96f;
            else
                NPC.velocity *= 0.96f;

            if (timer > 5 && timer < 110)
            {
                Main.GameZoomTarget = MathHelper.Lerp(Main.GameZoomTarget, 1.10f, 0.03f);

                float rotGoal = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;
                NPC.rotation = MathHelper.Lerp(rotGoal + (MathF.PI * 5), rotGoal, Easings.easeOutExpo((timer - 5) / 110f));
            }
            else if (timer >= 110)
            {
                NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;
            }

            //Dust
            if (timer > 110 && timer < 150)
            {
                for (int a = 0; a < 3 + Main.rand.Next(0, 3); a++)
                {
                    Vector2 vel = Main.rand.NextVector2CircularEdge(30f, 30f) * Main.rand.NextFloat(1f, 2.2f);
                    float dustScale = Main.rand.NextFloat(0.5f, 0.8f);
                    Color col = Color.HotPink;
                    Dust d = Dust.NewDustPerfect(NPC.Center + new Vector2(-80, 0).RotatedBy(NPC.rotation), ModContent.DustType<MuraLineBasic>(), vel, Main.rand.Next(10, 16), col, dustScale);
                }
                myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 35;
                phase3PulseValue = 0.75f + (Main.rand.Next(-1, 2) * 0.15f);

                thrusterValue = 0f;
                Main.GameZoomTarget *= Main.rand.NextFloat(0.97f, 1.05f);
            }

            //Pulse
            if (timer == 110)
            {
                SoundStyle style2 = new SoundStyle("Terraria/Sounds/Zombie_67") with { Pitch = -0.35f, PitchVariance = 0f, MaxInstances = -1, Volume = 0.5f };
                SoundEngine.PlaySound(style2, NPC.Center);

                SoundStyle style3 = new SoundStyle("Terraria/Sounds/Zombie_68") with { Pitch = .45f, PitchVariance = 0f, MaxInstances = -1, Volume = 0.7f };
                SoundEngine.PlaySound(style3, NPC.Center);

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/EvilEnergy") with { Volume = .35f, Pitch = -.2f, MaxInstances = -1 };
                SoundEngine.PlaySound(style, NPC.Center);

                SoundStyle style4 = new SoundStyle("AerovelenceMod/Sounds/Effects/EvilEnergy") with { Volume = .45f, Pitch = .55f, MaxInstances = -1 };
                SoundEngine.PlaySound(style4, NPC.Center);

                int b = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-68, 0).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<CyverRoarPulse>(), 0, 0f);
                //int b2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-68, 0).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<CyverRoarPulse>(), 0, 0f);
                Main.projectile[b].scale = 4f;
                Main.projectile[b].ai[0] = 1f;
                //Main.projectile[b2].scale = 6f;
                //Main.projectile[b2].ai[0] = -1f;

                if (Main.projectile[b].ModProjectile is CyverRoarPulse crp1)
                {
                    crp1.pixel = false;
                    crp1.forRoar = true;
                }
                //if (Main.projectile[b2].ModProjectile is CyverRoarPulse crp2) crp2.pixel = false;

                for (int a = 0; a < 10 + Main.rand.Next(0, 2); a++)
                {
                    Vector2 vel = Main.rand.NextVector2CircularEdge(30f, 30f) * Main.rand.NextFloat(1f, 2.2f);
                    float dustScale = Main.rand.NextFloat(0.5f, 0.8f);
                    Color col = a >= 8 ? Color.SkyBlue : Color.HotPink;
                    Dust d = Dust.NewDustPerfect(NPC.Center + new Vector2(-80, 0).RotatedBy(NPC.rotation), ModContent.DustType<MuraLineBasic>(), vel, 10, col, dustScale);
                    //d.fadeIn = 10;
                }

                //myPlayer.GetModPlayer<ScreenPlayer>().ScreenGoalPos = NPC.Center + new Vector2(95, 0).RotatedBy(NPC.rotation);
                myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 125;

                eyeStarValue = 1f;
                justDashValue = 1f;
                overlayPower = 1f;
                Main.GameZoomTarget = 1.95f; //1.75
                phase3PulseValue = 1f;
            }

            if (timer == 125)
            {
                myPlayer.GetModPlayer<ScreenPlayer>().lerpBackToPlayer = true;
                myPlayer.GetModPlayer<ScreenPlayer>().cutscene = false;
            }

            if (timer >= 135)
            {
                Main.GameZoomTarget = Math.Clamp(MathHelper.Lerp(Main.GameZoomTarget, 0.8f, 0.1f), 1, 2);
            }

            if (timer == 170)
            {
                Main.GameZoomTarget = 1;
                whatAttack = 1;
                myPlayer.GetModPlayer<ScreenPlayer>().lerpBackToPlayer = true;
                myPlayer.GetModPlayer<ScreenPlayer>().cutscene = false;
                timer = -1;
                startingAngBonus = 0;
                startingQuadrant = 3;
                NPC.dontTakeDamage = false;
            }

            timer++;
        }

        public void PowerDownAttack(Player myPlayer)
        {
            //Do shite
        }
        public void SitStill(Player myPlayer)
        {
            NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;

            NPC.dontTakeDamage = true;
            NPC.hide = false;

            timer++;
        }

        float barrageCount = 1;
        bool trueSpinFalseAzzy = true;
        bool trueWrapFalseChase = true;
        bool trueCloneFalseBot = true;

        public void SetNextAttack(String nextAttackName)
        {
            //Idle | Spam | Wrap Dash | Clones | Bots
            //Idle | Azzy + Single Giga | Bots | Clones
            if (!Phase3)
            {
                if (nextAttackName == "Barrage")
                {
                    whatAttack = 1;
                    barrageCount = 1;
                }
                else if (nextAttackName == "Laser")
                {
                    if (trueSpinFalseAzzy)
                    {
                        whatAttack = 16; //SpinP3
                        trueSpinFalseAzzy = !trueSpinFalseAzzy;
                    }
                    else
                    {
                        whatAttack = 4; //Azzy
                        trueSpinFalseAzzy = !trueSpinFalseAzzy;
                    }
                }
                else if (nextAttackName == "Dash")
                {
                    whatAttack = 9;
                }
                else if (nextAttackName == "Summon")
                {
                    //Clone/Bot order is reversed in P3
                    if (trueCloneFalseBot)
                    {
                        whatAttack = 6;
                        trueCloneFalseBot = !trueCloneFalseBot;
                    }
                    else
                    {
                        whatAttack = 23;
                        trueCloneFalseBot = !trueCloneFalseBot;
                    }
                }
                else
                {
                    Main.NewText("very bad");
                }
            }
            //Idle | Spam | toanftl | Less Wraps | Curved | Bots
            //Idle | Azzy | Sword | Chase | Ball Dash | Clones | Pink Clone
            else if (Phase3)
            {
                if (nextAttackName == "Barrage")
                {
                    whatAttack = 1;
                    barrageCount = 1;
                }
                else if (nextAttackName == "Laser")
                {
                    if (trueSpinFalseAzzy)
                    {
                        whatAttack = 16; //SpinPhase3
                        trueSpinFalseAzzy = !trueSpinFalseAzzy;
                    }
                    else
                    {
                        whatAttack = 4; //Azzy
                        trueSpinFalseAzzy = !trueSpinFalseAzzy;
                    }
                }
                else if (nextAttackName == "Dash")
                {
                    if (trueWrapFalseChase)
                    {
                        whatAttack = 9;
                        trueWrapFalseChase = !trueWrapFalseChase;
                    }
                    else
                    {
                        whatAttack = 7;
                        trueWrapFalseChase = !trueWrapFalseChase;
                    }
                }
                else if (nextAttackName == "Summon")
                {
                    //Clone/Bot order is reversed in P3
                    if (trueCloneFalseBot)
                    {
                        whatAttack = 23;
                        trueCloneFalseBot = !trueCloneFalseBot;
                    }
                    else
                    {
                        whatAttack = 6;
                        trueCloneFalseBot = !trueCloneFalseBot;
                    }
                }
                else
                {
                    Main.NewText("very bad");
                }
            }


            float phase3TransValue = isExpert ? (NPC.lifeMax - (NPC.lifeMax / 3)) : (NPC.lifeMax - (NPC.lifeMax / 3));

            if (NPC.life < phase3TransValue && !Phase3)
            {
                SoundStyle style3 = new SoundStyle("Terraria/Sounds/Zombie_68") with { Pitch = .77f, PitchVariance = 0f, MaxInstances = -1, Volume = 0.7f };
                SoundEngine.PlaySound(style3, NPC.Center);
                Phase2 = true;
                Phase3 = true;

                trueSpinFalseAzzy = true;
                trueWrapFalseChase = true;
                trueCloneFalseBot = false;

                whatAttack = -3;
                barrageCount = 1;
            }

        }
        // -1 - Death
        // 0 - Intro
        // 1 - IdleLaser
        // 2 - IdleDash
        // 3 - Spin
        // 4 - AzzyLaser
        // 5 - GigaBeam
        // 6 - Clones
        // 7 - ChaseDash
        // 8 - Bots
        // 9 - Wrap Dash


        public void FireLaser(int type, float speed = 6f, float recoilMult = 2f, float ai1 = 0, float ai2 = 0, int damage = 50)
        {
            var entitySource = NPC.GetSource_FromAI();
            Player player = Main.player[NPC.target];
            Vector2 toPlayer = player.Center - NPC.Center;
            toPlayer = toPlayer.SafeNormalize(new Vector2(1, 0));
            toPlayer *= speed;
            Vector2 from = NPC.Center - new Vector2(96, 0).RotatedBy(NPC.rotation);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Projectile.NewProjectile(entitySource, from, toPlayer, type, damage, 3, Main.myPlayer, ai1, ai2);
            }
            NPC.velocity -= toPlayer * (recoilMult * (Phase3 ? 1.25f : 1f));

            if (type == ModContent.ProjectileType<CyverLaser>() || type == ModContent.ProjectileType<StretchLaser>())
            {

                float addition = (whatAttack == 0 ? -1 : 0);
                for (int i = 0; i < 7 + (addition * 2); i++) //4 //2,2
                {

                    Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f)) * Main.rand.Next(5, 10) * -1f;

                    Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -50, ModContent.DustType<GlowLine1Fast>(), vel * 3f,
                        Color.HotPink, Main.rand.NextFloat(0.13f, 0.23f), 0.6f, 0f, dustShader2);
                    p.noLight = false;
                    //p.velocity += NPC.velocity * (0.8f + Main.rand.NextFloat(-0.1f, -0.2f));
                    p.fadeIn = (whatAttack == 0 ? 36 : 46) + Main.rand.NextFloat(-5, 10);
                    p.velocity *= 0.4f;
                }
                for (int i = 0; i < 8 - addition; i++) //4 //2,2
                {

                    Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.Next(5, 10) * -1f;

                    Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -50, ModContent.DustType<GlowLine1Fast>(), vel * 3f,
                        Color.HotPink, Main.rand.NextFloat(0.13f, 0.23f), 0.6f, 0f, dustShader2);
                    p.noLight = false;
                    //p.velocity += NPC.velocity * (0.8f + Main.rand.NextFloat(-0.1f, -0.2f));
                    p.fadeIn = (whatAttack == 0 ? 36 : 46) + Main.rand.NextFloat(-5, 10);
                    p.velocity *= 0.2f;
                }

                for (int i = 0; i < 7 + Main.rand.Next(4); i++)
                {
                    Dust d = Dust.NewDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -50, ModContent.DustType<MuraLineBasic>(),
                        Velocity: NPC.rotation.ToRotationVector2().RotatedByRandom(0.2f) * -8f * Main.rand.NextFloat(0.7f, 1.3f), Alpha: 10, Color.HotPink * 1f, 0.3f);
                }

            }

        }

        public void ShotDust(float velMult = 1f)
        {
            Color pinkToUse = new Color(255, 70, 170);

            for (int i = 0; i < 9; i++) //4 //2,2
            {

                Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f)) * Main.rand.Next(5, 10) * -1f;

                Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -50, ModContent.DustType<GlowLine1Fast>(), vel * 4f * velMult,
                    pinkToUse, Main.rand.NextFloat(0.125f, 0.225f), 0.6f, 0f, dustShader2);
                p.noLight = false;
                //p.velocity += NPC.velocity * (0.8f + Main.rand.NextFloat(-0.1f, -0.2f));
                p.fadeIn = (whatAttack == 0 ? 38 : 48) + Main.rand.NextFloat(-5, 10);
                p.velocity *= 0.4f;
            }
            for (int i = 0; i < 5 + Main.rand.Next(0, 2); i++) //4 //2,2
            {

                Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.Next(5, 10) * -1f;

                Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -50, ModContent.DustType<GlowLine1Fast>(), vel * 4f * velMult,
                    pinkToUse, Main.rand.NextFloat(0.13f, 0.23f), 0.6f, 0f, dustShader2);
                p.noLight = false;
                //p.velocity += NPC.velocity * (0.8f + Main.rand.NextFloat(-0.1f, -0.2f));
                p.fadeIn = (whatAttack == 0 ? 38 : 48) + Main.rand.NextFloat(-5, 10);
                p.velocity *= 0.2f;
            }
        }

        //This is for the custom sky being reactive
        public float bigShotTimer = 0;
        public float extraBoost = 0f;
        public float whiteBackgroundPower = 0f;
        public float lineBonusSpeed = 0f;
        public float lineAlpha = 0f;
        public int getAttack()
        {
            if (spammingLaser)
                return -1;
            else
                return whatAttack;
        }

        #region DamageValues
        private enum AtkVals
        {
            //50 60 80 100 110
            VeryLight = 40, 
            Light = 45,
            Medium = 50,
            Heavy = 65,
            VeryHeavy = 75
        }

        //Idle Spin toanl Wrap Curve Bots | Azzy Giga Chase Sword Clone1 PinkClone
        public Dictionary<string, int> DamageValues = new Dictionary<string, int>
        {
            //Idle Attack
            ["IdleShot"] = (int)AtkVals.Medium,
            ["IdleEnergyBall"] = (int)AtkVals.Medium,
            ["IdleDaggers"] = (int)AtkVals.Light,
            ["IdleBurst"] = (int)AtkVals.Light,

            //Spin
            ["SpinLaser"] = (int)AtkVals.Medium,

            //Toanl
            ["SplitLaser"] = (int)AtkVals.Medium,
            ["SplitLaserShard"] = (int)AtkVals.Medium,

            //WrapDash
            ["WrapEnergyBall"] = (int)AtkVals.Light,

            //CurveDash
            ["CurveShard"] = (int)AtkVals.Medium,
            ["CurveSpreadShot"] = (int)AtkVals.Medium,

            //Bots
            ["Bots"] = (int)AtkVals.Medium,

            //AzzyLaser
            ["AzzyLaser"] = (int)AtkVals.VeryLight,

            //GigaBeam
            ["SoloGiga"] = (int)AtkVals.VeryHeavy,
            ["GigaSpam"] = (int)AtkVals.Heavy,

            //Chase Dash
            ["ChaseShard"] = (int)AtkVals.Medium,

            //Ball Dash
            ["BallDash"] = (int)AtkVals.Medium,

            //EyeSword
            ["EyeSword"] = (int)AtkVals.Light,
            ["EyeSwordShot"] = (int)AtkVals.Light,

            //CloneP3
            ["CloneP3"] = (int)AtkVals.Medium,

            //Pink Clone
            ["PinkCloneLaser"] = (int)AtkVals.Medium,
            ["PinkCloneClone"] = (int)AtkVals.Medium,
        };


        public int GetDamage(string type)
        {
            float normalizationValue = 2f;
            baseDamageMult = 1.25f;

            if (Main.masterMode)
            {
                normalizationValue = 6f;
                baseDamageMult = 2.45f;
            }
            else if (Main.expertMode)
            {
                normalizationValue = 4f;
                baseDamageMult = 1.9f; //1.9
            }

            return (int)((DamageValues[type] / normalizationValue) * baseDamageMult);
        }
        #endregion
    }
}
