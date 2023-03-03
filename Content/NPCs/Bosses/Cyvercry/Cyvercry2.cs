using AerovelenceMod.Common.Globals.Worlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
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

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry 
{
    [AutoloadBossHead]
    public class Cyvercry2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyvercry");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            
            //Immune to fire debuffs because they make him ugly :(
            NPCDebuffImmunityData debuffData = new()
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    BuffID.Bleeding,
                    ModContent.BuffType<AuroraFire>(),
                    BuffID.OnFire,
                    BuffID.OnFire3,
                    BuffID.CursedInferno,
                    BuffID.Ichor,
                    BuffID.Frostburn,
                    BuffID.Frostburn2,
                }
            };
        }
        ArmorShaderData dustShader = null;
        ArmorShaderData dustShader2 = null;

        public int ContactDamage = 0;

        public bool Phase2 = false;
        public bool Phase3 = false;

        public override bool CheckActive()
        {
            return false;
        }
        public override void SetDefaults()
        {
            ContactDamage = 80;
            if (isExpert) ContactDamage = 100;
            if (isMaster) ContactDamage = 120;

            NPC.lifeMax = 37500;
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
            NPC.value = Item.buyPrice(0, 22, 11, 5);
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Cyvercry");
            }
            dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 43110;
            NPC.damage = 125;
            NPC.defense = 10;
        }

        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (item.DamageType == DamageClass.Melee && (NPC.velocity.Length() > 17 || whatAttack == 3))
            {
                if ((NPC.Center - Main.player[NPC.target].Center).Length() < 300)
                    damage = (int)(damage * (whatAttack == 3 && !spammingLaser ? 2 : 3.5f));

            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.DamageType == DamageClass.Melee && (NPC.velocity.Length() > 17 || whatAttack == 3))
            {
                if ((NPC.Center - Main.player[NPC.target].Center).Length() < 400)
                {
                    damage = (int)(damage * (whatAttack == 3 && !spammingLaser ? 2 : 3.5f));
                }
            }


            //Shhhh 
            if (projectile.type == ProjectileID.ChlorophyteBullet)
                damage = (int)(damage * 0.75f);
            if (projectile.type == ProjectileID.ChlorophyteArrow)
                damage = (int)(damage * 0.8f);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;

            DownedWorld.DownedCyvercry = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        #region Drawing
        float pinkGlowMaskTimer = 0;
        bool fadeDashing = false;
        float phase3Intensity = 0f;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            
            //PinkGlow
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            //TODO make this run only when needed
            Vector2 from = NPC.Center + new Vector2(-98, 0).RotatedBy(NPC.rotation);
            Texture2D Ball = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/circle_05");
            Main.EntitySpriteDraw(Ball, from - Main.screenPosition, Ball.Frame(), Color.HotPink * 2, NPC.rotation, Ball.Frame().Size() / 2f, (ballScale / 150) * 0.08f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(Ball, from - Main.screenPosition, Ball.Frame(), Color.HotPink, NPC.rotation, Ball.Frame().Size() / 2f, (ballScale / 150) * 0.13f, SpriteEffects.None, 0);

            float glowIntensity = fadeDashing ? 0.25f : 1f;
            Texture2D Bloommy = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/RegreGlowCyvercry");
            Main.EntitySpriteDraw(Bloommy, NPC.Center - Main.screenPosition, NPC.frame, Color.White * glowIntensity, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(Bloommy, NPC.Center - Main.screenPosition, NPC.frame, Color.White * glowIntensity, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 drawOffset = new Vector2(0, 0).RotatedBy(NPC.rotation);

            if (!fadeDashing)
            {
                Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/CyverGlowMaskPink");
                Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition + drawOffset, NPC.frame, Color.White * (0.5f * (float)Math.Sin(pinkGlowMaskTimer / 60f) + 0.5f), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            }

            float intensity = fadeDashing ? 0.6f : 1;
            //Blue Glow
            Texture2D texture3 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/CyverGlowMaskBlue");
            Main.EntitySpriteDraw(texture3, NPC.Center - Main.screenPosition + drawOffset, NPC.frame, Color.White * intensity, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

            if (Phase3 && !fadeDashing)
            {
                Vector2 random = new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3));
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Texture2D pixelStar = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Starlight").Value;
                Vector2 scale1 = new Vector2(0.9f, 0.9f);
                Vector2 scale2 = new Vector2(2.2f, 1.3f);

                spriteBatch.Draw(pixelStar, NPC.Center - Main.screenPosition + new Vector2(-70, 0).RotatedBy(NPC.rotation) + random, pixelStar.Frame(1, 1, 0, 0), Color.DeepPink * 0.6f * phase3Intensity, NPC.rotation + MathHelper.PiOver2, pixelStar.Size() / 2, scale2 * 1.2f, SpriteEffects.None, 0);
                spriteBatch.Draw(pixelStar, NPC.Center - Main.screenPosition + new Vector2(-70, 0).RotatedBy(NPC.rotation) + random, pixelStar.Frame(1, 1, 0, 0), Color.HotPink * 0.75f * phase3Intensity, NPC.rotation + MathHelper.PiOver2, pixelStar.Size() / 2, scale2 * 0.7f, SpriteEffects.None, 0);
                spriteBatch.Draw(pixelStar, NPC.Center - Main.screenPosition + new Vector2(-70, 0).RotatedBy(NPC.rotation), pixelStar.Frame(1, 1, 0, 0), Color.White * phase3Intensity, NPC.rotation + MathHelper.PiOver2, pixelStar.Size() / 2, scale2 * 0.3f, SpriteEffects.None, 0);

                spriteBatch.Draw(pixelStar, NPC.Center - Main.screenPosition + new Vector2(-75, 0).RotatedBy(NPC.rotation) + random, pixelStar.Frame(1, 1, 0, 0), Color.DeepPink * 0.6f * phase3Intensity, NPC.rotation, pixelStar.Size() / 2, scale1 * 1.2f, SpriteEffects.None, 0);
                spriteBatch.Draw(pixelStar, NPC.Center - Main.screenPosition + new Vector2(-75, 0).RotatedBy(NPC.rotation) + random, pixelStar.Frame(1, 1, 0, 0), Color.HotPink * 0.75f * phase3Intensity, NPC.rotation, pixelStar.Size() / 2, scale1 * 0.7f, SpriteEffects.None, 0);
                spriteBatch.Draw(pixelStar, NPC.Center - Main.screenPosition + new Vector2(-70, 0).RotatedBy(NPC.rotation), pixelStar.Frame(1, 1, 0, 0), Color.White * phase3Intensity, NPC.rotation, pixelStar.Size() / 2, scale2 * 0.3f, SpriteEffects.None, 0);


                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            }

        }

        bool ThrusterRotaion = Main.rand.NextBool();
        float thrusterValue = 0f;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (firstFrame)
                return false;
            //FX for death Anim
            if (DrawDeathOrb)
            {
                Texture2D starTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/scorch_01").Value;
                Texture2D starTex2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;


                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

                Vector2 starPos = NPC.Center + (NPC.rotation.ToRotationVector2() * -25);
                for (int i = 0; i < 3; i++)
                {
                    Main.spriteBatch.Draw(starTex, starPos - Main.screenPosition, starTex.Frame(1, 1, 0, 0), Color.HotPink, NPC.rotation + MathHelper.ToRadians(-2 * timer), starTex.Size() / 2, deathOrbScale * 0.65f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(starTex2, starPos - Main.screenPosition, starTex2.Frame(1, 1, 0, 0), Color.Pink, NPC.rotation + MathHelper.ToRadians(timer * 0.7f), starTex2.Size() / 2, deathOrbScale, SpriteEffects.None, 0);

                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            }

            //Phase 2 Glow
            if ((Phase2 || Phase3) && !fadeDashing)
            {
                if (Phase3)
                {
                    //Texture2D pixelStar = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Starlight").Value;
                    Texture2D auraTex = Mod.Assets.Request<Texture2D>("Assets/Glorb").Value;
                    Vector2 scale = new Vector2(1.2f, 0.8f);
                    //Vector2 scale2 = new Vector2(3f, 1.2f);

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

                    //Main.spriteBatch.Draw(eBall, NPC.Center - Main.screenPosition + new Vector2(-20, 0).RotatedBy(NPC.rotation), eBall.Frame(1, 1, 0, 0), Color.HotPink, NPC.rotation, eBall.Size() / 2, scale, SpriteEffects.None, 0.0f);

                    for (int i = 0; i <= 4; i++)
                    {
                        spriteBatch.Draw(auraTex, NPC.Center + new Vector2(Main.rand.Next(-6, 8), Main.rand.Next(-6, 8)) - Main.screenPosition, auraTex.Frame(1, 1, 0, 0), Color.DeepPink, NPC.rotation, auraTex.Size() / 2, scale * (i * 1.1f), SpriteEffects.None, 0);
                    }
                    //spriteBatch.Draw(pixelStar, NPC.Center - Main.screenPosition + new Vector2(-80, 0).RotatedBy(NPC.rotation), pixelStar.Frame(1, 1, 0, 0), Color.DeepPink, NPC.rotation + MathHelper.PiOver2, pixelStar.Size() / 2, scale2, SpriteEffects.None, 0);
                }


                Texture2D textureA = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/CyvercryNoThruster").Value;

                Effect myEffectA = ModContent.Request<Effect>("AerovelenceMod/Effects/CyverAura", AssetRequestMode.ImmediateLoad).Value;

                myEffectA.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3() * 1f);
                myEffectA.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/VoroNoise").Value);
                myEffectA.Parameters["uTime"].SetValue(timer * 0.2f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffectA, Main.GameViewMatrix.TransformationMatrix);
                myEffectA.CurrentTechnique.Passes[0].Apply();

                int height1 = textureA.Height;
                Vector2 origin1 = new Vector2((float)textureA.Width / 2f, (float)height1 / 2f);

                Main.spriteBatch.Draw(textureA, NPC.Center - Main.screenPosition + new Vector2(2, 215).RotatedBy(NPC.rotation), new Rectangle(0, 0, 180, 100), Color.White, NPC.rotation, origin1, 1.075f, SpriteEffects.None, 0.0f);
                //Main.spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, Color.White, NPC.rotation, origin1, 1f, SpriteEffects.None, 0.0f);
                //Main.spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, Color.White, NPC.rotation, origin1, 1f, SpriteEffects.None, 0.0f);


                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            }

            Vector2 from = NPC.Center - new Vector2(75, 0).RotatedBy(NPC.rotation);

            #region DrawTelegraphLines
            //AZZY LASER DRAWING
            if (drawAzzyLaser == 1 && whatAttack != -1) //5 shot
            {
                Player target = Main.player[NPC.target];
                for (int i = -1; i < 2; i++)
                {
                    Vector2 dirToTarget = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                    //Projectile.NewProjectile(NPC.GetSource_FromAI(), from, velocity.RotatedBy(MathHelper.ToRadians(17.5f * i)), ModContent.ProjectileType<CyverLaser>(), 30, 3, Main.myPlayer);
                    Utils.DrawLine(spriteBatch, from, NPC.Center + dirToTarget.RotatedBy(MathHelper.ToRadians(17.5f * i)) * 1100, Color.DeepPink * 0.6f, Color.DeepSkyBlue * 0.4f, 1);
                    Utils.DrawLine(spriteBatch, NPC.Center + dirToTarget.RotatedBy(MathHelper.ToRadians(17.5f * i)) * 1100, NPC.Center + dirToTarget.RotatedBy(MathHelper.ToRadians(17.5f * i)) * 1600, Color.DeepSkyBlue * 0.4f, Color.DeepSkyBlue * 0f, 1);

                }

            } else if (drawAzzyLaser == 2 && whatAttack != -1) //4 shot
            {
                Player target = Main.player[NPC.target];

                for (int i = -1; i < 2; i++)
                {
                    Vector2 dirToTarget = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                    if (i != 0)
                    {
                        Utils.DrawLine(spriteBatch, from, NPC.Center + dirToTarget.RotatedBy(MathHelper.ToRadians(10f * i)) * 1100, Color.DeepPink * 0.6f, Color.DeepSkyBlue * 0.4f, 1);
                        Utils.DrawLine(spriteBatch, NPC.Center + dirToTarget.RotatedBy(MathHelper.ToRadians(10f * i)) * 1100, NPC.Center + dirToTarget.RotatedBy(MathHelper.ToRadians(10f * i)) * 1600, Color.DeepSkyBlue * 0.4f, Color.DeepSkyBlue * 0f, 1);

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
                Utils.DrawLine(spriteBatch, from, NPC.Center + dirToTarget * -1900, Color.DeepPink * intensity, Color.HotPink * intensity, 2);


            }
            #endregion

            //Draw After Image
            Vector2 drawOriginAI = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Cyvercry").Value;
            if (NPC.velocity.Length() > 18f)
            {
                Color drawingCol = drawColor;
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOriginAI + new Vector2(0f, NPC.gfxOffY);
                    drawingCol = NPC.GetAlpha(drawColor) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                    Main.EntitySpriteDraw((Texture2D)TextureAssets.Npc[NPC.type], drawPos + new Vector2(-40, 0), NPC.frame, drawingCol * 0.5f, NPC.rotation, drawOriginAI, NPC.scale, SpriteEffects.None, 0);
                }
            }

            //Draw Blue Afterimage
            if (fadeDashing)
            { 
                Texture2D CyverPink = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/CyverGlowMaskBlue");
                Vector2 drawOrigin2 = new Vector2(CyverPink.Width * 0.5f, NPC.height * 0.5f);
                Color color = Color.White;
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin2 + new Vector2(0f, NPC.gfxOffY);
                    color = NPC.GetAlpha(drawColor) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                    Main.EntitySpriteDraw(CyverPink, drawPos + new Vector2(-40, 0), NPC.frame, color, NPC.rotation, drawOrigin2, NPC.scale, SpriteEffects.None, 0);
                }
            }



            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            //Texture2D texture3 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/CyverGlowMaskBlue");
            //Main.EntitySpriteDraw(texture3, NPC.Center - Main.screenPosition + drawOffset, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

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
                spriteBatch.Draw(texture2, NPC.Center - Main.screenPosition + new Vector2(66, 0).RotatedBy(NPC.rotation), null, Color.Black, NPC.rotation + MathHelper.PiOver2, texture2.Size() / 2, 0.3f, ThrusterRotaion ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);

            Texture2D CyverTexture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/CyvercryNoThruster").Value;

            if (!fadeDashing)
                Main.EntitySpriteDraw(CyverTexture, NPC.Center - Main.screenPosition, NPC.frame, drawColor * 1f, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);

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
                    if ((!fadeDashing || !NPC.hide) && whatAttack != 6) //!= clones
                    {
                        for (int i = 0; i < 5; i++) //4 //2,2
                        {
                            Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.Next(5, 10) * -1f;

                            Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + vel * -5, ModContent.DustType<GlowCircleQuadStar>(), vel * -1f,
                                Color.DeepSkyBlue, Main.rand.NextFloat(0.6f, 1.2f), 0.4f, 0f, dustShader);
                            p.noLight = true;
                            p.velocity += NPC.velocity * (0.4f + Main.rand.NextFloat(-0.1f, -0.2f));
                            //p.rotation = Main.rand.NextFloat(6.28f);
                        }
                    }

                    /*
                    for (int i = 0; i < 360; i += 20)
                    {
                        Vector2 circular = new Vector2(24, 0).RotatedBy(MathHelper.ToRadians(i));
                        circular.Y *= 0.7f;
                        circular = circular.RotatedBy(NPC.rotation);
                        Vector2 dustVelo = new Vector2(12, 0).RotatedBy(NPC.rotation);


                        Dust d = GlowDustHelper.DrawGlowDustPerfect(from - new Vector2(5) + circular, ModContent.DustType<GlowCircleSpinner>(), dustVelo, new Color(67, 215, 209), 0.4f, 0.6f, 0f, dustShader);
                        d.scale = 0.4f;
                    }
                    */
                    thrusterValue = 0;
                    ThrusterRotaion = Main.rand.NextBool();
                }
            }
        }
        #endregion

        float bonusSpinCharge = 120;
        int whatAttack = -2;
        int timer = 0;
        int advancer = 0;
        float accelFloat = 0;

        //Doing this and not Main.masterMode so I can override the difficulty for both testing and in a config
        bool isExpert = true;
        bool isMaster = true;

        bool firstFrame = true;

        public override void AI()
        {
            if (firstFrame)
            {
                firstFrame = false;
                SkyManager.Instance.Activate("AerovelenceMod:Cyvercry2");
            }

            isExpert = true;
            isMaster = true;
            Phase3 = true;
            Main.dayTime = false;
            Main.time = 12600 + 3598; //midnight - 2 cause we don't want to keep activating stuff that happens at midnight

            //Phase2 = true;
            //Phase3 = true;
            NPC.damage = 0;

            //Main.NewText(whatAttack);
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Player myPlayer = Main.player[NPC.target];

            if (myPlayer.active == false || myPlayer.dead == true)
            {
                NPC.active = false;
            }

            whatAttack = 69;
            //ClonesP3(myPlayer);
            switch (whatAttack)
            {
                case -1:
                    DeathAnimation(myPlayer);
                    break;
                case -2:
                    IntroAnimation(myPlayer);
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
                case 69:
                    SweepLaser(myPlayer);
                    break;

            }

            phase3Intensity = 0.8f + (0.25f * (float)Math.Sin(pinkGlowMaskTimer / 25f) + 0.25f);
            thrusterValue = Math.Clamp(MathHelper.Lerp(thrusterValue, 3, 0.06f), 0, 2);
            pinkGlowMaskTimer++;

        }

        #region Attacks

        #region Laser Attacks

        float startingAngBonus = 0;
        float startingQuadrant = 3;
        float dashQuadrant = 1;
        bool advanceNegative = false;
        public void IdleLaser(Player myPlayer)
        {
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

            int shotDelay = Phase3 ? 25 : 30;
            float shotSpeed = isMaster ? 8 : 6;

            Vector2 goalPoint = new Vector2(550, 0).RotatedBy(MathHelper.ToRadians(advancer * 0.2f + startingAngBonus)); //advancer * 0.4

            //Vector2 goalPoint = new Vector2(550, 0).RotatedBy(MathHelper.ToRadians(advancer * 0.2f - 40)); //advancer * 0.4

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

                SoundStyle stylec = new SoundStyle("Terraria/Sounds/Item_67") with { Pitch = .38f, Volume = 0.7f }; //1f
                SoundEngine.PlaySound(stylec, NPC.Center);

                Dust a = GlowDustHelper.DrawGlowDustPerfect(NPC.Center - new Vector2(70, 0).RotatedBy(NPC.rotation), ModContent.DustType<GlowCircleQuadStar>(),
                (NPC.rotation + MathHelper.Pi).ToRotationVector2() * 12, Color.HotPink, 0.7f, 0.2f, 0f, dustShader2);

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
                FireLaser(ModContent.ProjectileType<StretchLaser>(), 1.5f, 8f);
                //FireLaser(ModContent.ProjectileType<CyverLaser>(), speed: shotSpeed); //Death Laser
            }

            if (timer == 220)
            {
                advancer = 0;
                timer = -1;
                whatAttack = 2;
                //whatAttack = 7; //2
            }
            if (advanceNegative)
                advancer = (timer > 140 ? --advancer : advancer - 2); //++ 2
            else
                advancer = (timer > 140 ? ++advancer : advancer + 2); //++ 2
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
                    //FireLaser(ModContent.ProjectileType<CyverLaser>(), 13f, 0.7f);
                    FireLaser(ModContent.ProjectileType<StretchLaser>(), 2f, 5f);

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
            float shotDelay = MathHelper.Clamp(currentShot, 0, 13) * 5;

            if (currentShot == 0)
            {
                //Main.NewText("Telegraph Lines maybe WIP");
                goalLocation = new Vector2(600, 0).RotatedByRandom(6.28);
                currentShot++;
            }

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
                    Vector2 from = NPC.Center - new Vector2(96, 0).RotatedBy(NPC.rotation);

                    if (currentShot % 2 == 0)
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            Vector2 velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 13;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), from, velocity.RotatedBy(MathHelper.ToRadians(18.5f * i)), ModContent.ProjectileType<CyverLaser>(), 30, 3, Main.myPlayer);

                        }
                    }
                    else
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            Vector2 velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 13;
                            if (i != 0)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), from, velocity.RotatedBy(MathHelper.ToRadians(11 * i)), ModContent.ProjectileType<CyverLaser>(), 30, 3, Main.myPlayer);

                        }
                    }

                    ShotDust();

                    SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_158") with { Pitch = .56f, PitchVariance = .27f, MaxInstances = -1, Volume = 0.75f };
                    SoundEngine.PlaySound(stylea, NPC.Center);
                    SoundEngine.PlaySound(stylea, NPC.Center);

                    SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .22f, Pitch = .73f, PitchVariance = .27f, MaxInstances = -1 };
                    SoundEngine.PlaySound(styleb, NPC.Center);
                    SoundEngine.PlaySound(styleb, NPC.Center);

                    SoundStyle stylec = new SoundStyle("Terraria/Sounds/Item_67") with { Pitch = .38f, Volume = 0.5f }; //1f
                    SoundEngine.PlaySound(stylec, NPC.Center);

                }

            }

            if (timer == 60 + 45 - (shotDelay))
            {
                timer = -1;
                currentShot++;
                goalLocation = goalLocation.RotatedBy(Main.rand.NextBool() ? -1 : 1);
                drawAzzyLaser = 0;
                if (currentShot == 14 + bonusShots)
                {
                    currentShot = 0;
                    whatAttack = 5;
                    isExpert = true;
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
                    SoundStyle style = new SoundStyle("Terraria/Sounds/Zombie_66");
                    SoundEngine.PlaySound(style, NPC.Center);
                }
                float scale = MathHelper.Clamp(ballScale / 2, 0, 100);
                Vector2 ballSpawnVec = NPC.Center + new Vector2(-98, 0).RotatedBy(NPC.rotation);
                Vector2 spawnVecOutSet = Main.rand.NextVector2CircularEdge(scale / 2, scale / 2);
                GlowDustHelper.DrawGlowDustPerfect(ballSpawnVec + spawnVecOutSet, ModContent.DustType<GlowCircleQuadStar>(), spawnVecOutSet.SafeNormalize(Vector2.UnitX) * -2, Color.HotPink, Main.rand.NextFloat(.2f, .3f), dustShader2);
                ballScale += 4;
                NPC.rotation = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();
            }

            if (timer >= 85)
                NPC.velocity = Vector2.Zero;

            if (timer == 85)
            {
                NPC.Center -= NPC.rotation.ToRotationVector2() * -30;
                myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 60;

                ballScale = 0;
                bigShotTimer = 20;
                Vector2 from = NPC.Center - new Vector2(80, 0).RotatedBy(NPC.rotation);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), from, NPC.rotation.ToRotationVector2() * -1, ModContent.ProjectileType<CyverHyperBeam>(), 30, 2, Main.myPlayer);
            }

            if (timer == 130)
            {
                timer = -1;
                //whatAttack = 0;
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
            } else if (timer >= 100)
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

        public void GigaLaserSpam(Player myPlayer)
        {
            if (advancer == 0)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Zombie_66");
                SoundEngine.PlaySound(style, NPC.Center);
                goalLocation = new Vector2(530, 0).RotatedByRandom(6.28f);
                advancer++;
            }

            if (timer < 40)
            {
                if (timer < 30)
                {
                    Vector2 move = (goalLocation + myPlayer.Center) - NPC.Center;

                    float scalespeed = 0.6f * 3f;

                    NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
                    NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;
                }
                else
                {
                    NPC.velocity = Vector2.Zero;
                }


                if (timer == 5)
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/Zombie_66");
                    SoundEngine.PlaySound(style, NPC.Center);
                }
                float scale = MathHelper.Clamp(ballScale / 2, 0, 100);
                Vector2 ballSpawnVec = NPC.Center + new Vector2(-98, 0).RotatedBy(NPC.rotation);
                Vector2 spawnVecOutSet = Main.rand.NextVector2CircularEdge(scale / 2, scale / 2);
                GlowDustHelper.DrawGlowDustPerfect(ballSpawnVec + spawnVecOutSet, ModContent.DustType<GlowCircleQuadStar>(), spawnVecOutSet.SafeNormalize(Vector2.UnitX) * -2, Color.HotPink, Main.rand.NextFloat(.2f, .3f), dustShader2);
                ballScale += 10;

                float goalRot = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();
                NPC.rotation = Utils.AngleTowards(NPC.rotation, goalRot, 0.4f);
            } else ballScale += 5;

            if (timer >= 55) NPC.velocity = Vector2.Zero;

            if (timer == 55)
            {
                NPC.Center -= NPC.rotation.ToRotationVector2() * -30;
                myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 60;

                ballScale = 0;
                bigShotTimer = 20;
                Vector2 from = NPC.Center - new Vector2(80, 0).RotatedBy(NPC.rotation);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), from, NPC.rotation.ToRotationVector2() * -1, ModContent.ProjectileType<CyverHyperBeam>(), 30, 2, Main.myPlayer);
            }

            if (timer == 70)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Zombie_66");
                SoundEngine.PlaySound(style, NPC.Center);
                timer = 4;
                int side = Main.rand.NextBool() ? -1 : 1;
                float rot = Main.rand.NextFloat(0.9f, 1.7f) * side;
                goalLocation = goalLocation.RotatedBy(rot);

            }

            bigShotTimer--;
            timer++;
        }

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
                    sweepLaserDir = Main.rand.NextBool();

                    int telegraphLine = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center , Vector2.Zero, ModContent.ProjectileType<TelegraphLineCyver>(), 0, 0);
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
                    Main.projectile[a].timeLeft = 70;
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
                NPC.rotation += (0.0025f * timer + 0.007f) * (sweepLaserDir ? 1 : -1);

                //Main.NewText((0.0017f * timer + 0.015f));

                if (timer % 5 == 0)
                {
                    ShotDust();
                    //SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = 1.5f, PitchVariance = .47f, MaxInstances = 0, Volume = 0.3f };
                    //SoundEngine.PlaySound(style, NPC.Center);

                    //SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .12f, Pitch = .4f, PitchVariance = .2f, MaxInstances = 1 };
                    //SoundEngine.PlaySound(style2, NPC.Center);
                    //Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.rotation.ToRotationVector2() * -40, NPC.rotation.ToRotationVector2() * -0.1f, ModContent.ProjectileType<CyverBeam>(), 0, 0);
                }

                //turn desiredDirection
                //Shootlasers

                if (timer == 70)
                {
                    timer = -1;
                    advancer = -0;
                }
            }

            //Turn and move towards player
            //Stop and swoop lasers
            //eyeFlareSize = Math.Clamp(MathHelper.Lerp(eyeFlareSize, -0.1f, 0.02f), 0, 1);
            timer++;
        }

        #endregion

        #region Dash Attacks
        //Dash Attacks

        float storedRotaion = 0;
        Vector2 storedVec2 = Vector2.Zero;
        public void IdleDash(Player myPlayer)
        {
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
                    NPC.velocity = Vector2.Zero;

                #region daggers
                if (timer == 80 && Phase2)
                {
                    for (int i = 0; i < 3 + (isExpert ? 1 : 0); i++)
                    {
                        Vector2 toLocation = myPlayer.Center + new Vector2(Main.rand.NextFloat(240, 600), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(360)));
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int damage = 20;
                            /*if (Main.expertMode)
                            {
                                damage = (int)(damage / Main.expertDamage);
                            }*/
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), toLocation, Vector2.Zero, ModContent.ProjectileType<ShadowBlade>(), damage, 0, Main.myPlayer);
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
                                Dust d = GlowDustHelper.DrawGlowDustPerfect(from + circularLocation, ModContent.DustType<GlowCircleQuadStar>(), Vector2.Zero, Color.DeepPink, 0.6f, 0.6f, 0f, dustShader2);
                            }


                            //int dust = Dust.NewDust(from + new Vector2(-4, -4) + circularLocation, 0, 0, 235, 0, 0, 0, default, 1.25f);
                            //Main.dust[dust].noGravity = true;
                            //Main.dust[dust].velocity *= 0.1f;
                            //Main.dust[dust].scale = 1.8f;

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
            else if (timer >= 105)
            {
                NPC.damage = ContactDamage;
                if (isMaster && timer == 105)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<LaserExplosionBall>(), 20, 0, Main.myPlayer);
                }

                if ((timer == 115 || timer == 119 || timer == 123) && Phase2)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<EnergyBall>(), 20, 0, Main.myPlayer);
                }

                if (NPC.velocity.Length() > 20)
                    Dust.NewDust(NPC.Center, 12, 12, ModContent.DustType<DashTrailDust>(), NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f, 0, new Color(0, 255, 255), 1f);
                if (timer == 107) //113
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = .15f, MaxInstances = -1, };
                    SoundEngine.PlaySound(style, NPC.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = .59f, Pitch = 1f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/flame_thrower_airblast_rocket_redirect") with { Volume = .2f, Pitch = .42f };
                    SoundEngine.PlaySound(style3, NPC.Center);
                }
                accelFloat = MathHelper.SmoothStep(accelFloat, 80, 0.2f);  //MathHelper.Clamp(MathHelper.Lerp(accelFloat, 60f, 0.1f), 0, 50f);
                NPC.rotation = storedRotaion;
                NPC.velocity = storedRotaion.ToRotationVector2() * accelFloat * -1; // 4--> accelFloat
            }

            if (timer == 69)
            {
                NPC.velocity = Vector2.Zero;
                storedVec2 = myPlayer.Center;
                storedRotaion = NPC.rotation;
            }

            if (timer == 130)
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
                    //whatAttack = 8;
                    SetNextAttack("Laser");
                }
            }
            if (advanceNegative && timer < 69)
                advancer--;
            else if (!advanceNegative && timer < 69)
                advancer++;
            timer++;
        }

        public void ChaseDash(Player myPlayer)
        {
            //55 to 60 for faster dashes
            //velocity from 35 to 55

            float minusTime = (isExpert || isMaster) ? 2 : 0;

            NPC.damage = 0;
            NPC.hide = false;
            NPC.dontTakeDamage = true;
            fadeDashing = true;
            //Cyver spawns Target Reticle
            if (timer == 0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Items.Weapons.Misc.Ranged.GaussExplosion>(), 0, 0, myPlayer.whoAmI);
                //int retIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CyverReticle>(), 0, 0, myPlayer.whoAmI);
                //Projectile Reticle = Main.projectile[retIndex];
                //if (Reticle.ModProjectile is CyverReticle target)
                //{
                //target.ParentIndex = NPC.whoAmI;
                //}
            }


            if (timer < 40 && timer > 10)
            {
                Vector2 toPlayer = Vector2.Lerp(NPC.rotation.ToRotationVector2(), (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX), 0.2f);// (NPC.rotation.ToRotationVector2() )

                NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 2.5f;
                NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;
                //NPC.velocity = NPC.rotation.ToRotationVector2() * -7.5f;
                storedRotaion = NPC.velocity.ToRotation();
            }

            if (timer >= 40)
            {
                NPC.rotation = storedRotaion + MathHelper.Pi;

                //Spawn Cyver2EnergyBall
                if (timer == 40)
                {
                    for (int i = 0; i < 360; i += 20)
                    {
                        Vector2 circular = new Vector2(Phase3 ? 40 : 32, 0).RotatedBy(MathHelper.ToRadians(i));
                        //circular.X *= 0.6f;
                        circular = circular.RotatedBy(NPC.rotation);
                        Vector2 dustVelo = -circular * 0.1f;

                        Dust b = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + circular, ModContent.DustType<GlowCircleDust>(), Vector2.Zero, Color.DeepPink, 0.3f, 0.6f, 0f, dustShader2);
                    }
                    int type = Phase3 ? ModContent.ProjectileType<CyverLaserBomb>() : ModContent.ProjectileType<Cyver2EnergyBall>();
                    int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, type, 20, 0, Main.myPlayer);

                    if (Main.projectile[a].ModProjectile is LaserExplosionBall ball)
                    {
                        ball.numberOfLasers = 6;
                        Main.projectile[a].timeLeft = 50;
                    }

                    Main.projectile[a].rotation = storedRotaion;

                    storedVec2 = storedRotaion.ToRotationVector2() * (Phase3 ? 28 : 35);
                    NPC.velocity = Vector2.Zero;
                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = 0.5f, Pitch = 0.77f, MaxInstances = -1, PitchVariance = 0.1f };
                    SoundEngine.PlaySound(style2, NPC.Center);
                    NPC.velocity = storedRotaion.ToRotationVector2() * 55;

                    /*
                    for (int i = -1; i < 2; i++)
                    {
                        Vector2 offset = storedRotaion.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * i) * 40;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, storedRotaion.ToRotationVector2() * -15, ModContent.ProjectileType<Cyver2EnergyBall>(), 20, 0, Main.myPlayer);
                    }
                    */
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
                //Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PinkExplosion>(), 0, 0, myPlayer.whoAmI);
                fadeDashing = false;
                NPC.width = 100;
                NPC.height = 100;
                NPC.damage = ContactDamage;
                startingQuadrant = 4;
                SetNextAttack("Summon");
                //whatAttack = 4;
                timer = -1;
                advancer = 0;
                storedRotaion = 0;
                NPC.damage = 0;
                NPC.dontTakeDamage = false;

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

            if (timer == 0)
            {
                accelFloat = 35;
                if (NPC.Center.X > myPlayer.Center.X)
                    vecOut = new Vector2(500, 0);
                else
                    vecOut = new Vector2(500, 0);
                //vecOut = new Vector2(470, 0).RotatedBy(Main.rand.NextFloat(6.28f));
                //NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * -25;
                //NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation();
            }

            //Cyver Moves to a random VectorOut
            if (timer < 95)
            {
                Vector2 move = (vecOut + myPlayer.Center) - NPC.Center;

                float scalespeed = advancer == 0 ? 1.15f : 1.15f; //2

                NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
                NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;

                NPC.rotation = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();



            }

            //Cyver Dashes with 2 energyBalls
            if (timer >= 95)
            {
                if (timer == 95)
                {
                    if (advancer == 0)
                        NPC.velocity = Vector2.Zero;
                    storedRotaion = NPC.rotation;
                }

                NPC.damage = ContactDamage;

                if (NPC.velocity.Length() > 20)
                    Dust.NewDust(NPC.Center, 12, 12, ModContent.DustType<DashTrailDust>(), NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f, 0, new Color(0, 255, 255), 1f);

                accelFloat = MathHelper.SmoothStep(accelFloat, 50, 0.1f);  //MathHelper.Clamp(MathHelper.Lerp(accelFloat, 60f, 0.1f), 0, 50f);
                NPC.rotation = storedRotaion;
                NPC.velocity = storedRotaion.ToRotationVector2() * accelFloat * -1;

                if (timer == 98)
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = .15f, MaxInstances = -1, };
                    SoundEngine.PlaySound(style, NPC.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = .59f, Pitch = 1f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style2, NPC.Center);

                    SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/flame_thrower_airblast_rocket_redirect") with { Volume = .12f, Pitch = .42f }; 
                    SoundEngine.PlaySound(style3, NPC.Center);

                    Vector2 vel = (NPC.rotation + 2f).ToRotationVector2() * 6;
                    Vector2 vel2 = (NPC.rotation - 2f).ToRotationVector2() * 6;
                    if (isExpert || isMaster)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<EnergyBall>(), 20, 1);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel2, ModContent.ProjectileType<EnergyBall>(), 20, 1);
                    }

                }
            }

            //Reset

            if (timer == 140)
            {
                timer = 39;
                vecOut = new Vector2(470, 0).RotatedBy(Main.rand.NextFloat(6.28f));
                NPC.Center = myPlayer.Center + (vecOut * 2.3f);

                if (advancer == 8)
                {
                    NPC.velocity = Vector2.Zero;
                    timer = -1;
                    advancer = -1;
                    startingQuadrant = 2;
                    SetNextAttack("Summon");
                }
                advancer++;
            }

            timer++;
        }

        float numberOfDashes = 10;
        bool side = false;
        public void OrbDashP3(Player myPlayer)
        {
            //move to top 


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
                float rotDirection = Main.rand.NextBool() ? 1: -1;
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
            //Phase2 = true;
            //Phase3 = false;

            float delay = (isExpert || isMaster) ? 0 : 10; 

            if (timer == 0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PinkExplosion>(), 0, 0, Main.myPlayer);
            }
            else
            {

                if (timer == 60 + delay)
                {

                    float value = 350;
                    float secondValue = 100;

                    
                    float extraSpacing = Phase3 ? 0 : 0;
                    
                    float forValue = Phase2 ? -1 : 0;
                    forValue = Phase3 ? -1 : forValue;


                    if (advancer % 2 == 0)
                    {
                        //Right
                        for (int i = (int)forValue; i < forValue * -1 + 1; i++)
                        {
                            Vector2 goalLocation = new Vector2(350, (Phase3 ? 300 : 150) * i);

                            if (!(Phase3 && i == 100))
                            {
                                int cloneIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), myPlayer.Center + goalLocation * 2, Vector2.Zero, ModContent.ProjectileType<ShadowClone>(), ContactDamage / 4, 2, Main.myPlayer);
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

                            Vector2 goalLocation = new Vector2(-350, (Phase3 ? 300 : 150) * i);

                            if (!(Phase3 && i == 100))
                            {
                                int cloneIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), myPlayer.Center + goalLocation * 2, Vector2.Zero, ModContent.ProjectileType<ShadowClone>(), ContactDamage / 4, 2, Main.myPlayer);
                                Projectile Clone = Main.projectile[cloneIndex];

                                if (Clone.ModProjectile is ShadowClone dashers)
                                {
                                    dashers.SetGoalPoint(goalLocation);
                                }
                            }

                        }


                        /*
                        //Bottom
                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 goalLocation = new Vector2(350 - (700 * i), 0);
                            int cloneIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), myPlayer.Center + goalLocation * 2, Vector2.Zero, ModContent.ProjectileType<ShadowClone>(), ContactDamage / 4, 2, Main.myPlayer);
                            Projectile Clone = Main.projectile[cloneIndex];

                            if (Clone.ModProjectile is ShadowClone dashers)
                            {
                                dashers.SetGoalPoint(goalLocation);
                            }

                        }
                        */
                    } else
                    {
                        //Top
                        for (int i = (int)forValue; i < forValue * -1 + 1; i++)
                        {
                            Vector2 goalLocation = new Vector2((Phase3 ? 300 : 150) * i, -350);

                            if (!(Phase3 && i == 100))
                            {
                                int cloneIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), myPlayer.Center + goalLocation * 2, Vector2.Zero, ModContent.ProjectileType<ShadowClone>(), ContactDamage / 4, 2, Main.myPlayer);
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
                            Vector2 goalLocation = new Vector2((Phase3 ? 300 : 150) * i, 350);

                            if (!(Phase3 && i == 100))
                            {

                                int cloneIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), myPlayer.Center + goalLocation * 2, Vector2.Zero, ModContent.ProjectileType<ShadowClone>(), ContactDamage / 4, 2, Main.myPlayer);
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

            if (advancer == 5)
            {
                NPC.alpha = 0;
                timer = -1;
                startingQuadrant = 3;
                SetNextAttack("Barrage");
                //whatAttack = 0;
                NPC.dontTakeDamage = false;
                NPC.hide = false;
                advancer = 0;
            }

            timer++;
        }

        public void ClonesP3(Player myPlayer)
        {
            if (timer == 0)
            {
                //goalLocation = new Vector2(0, 500).RotatedBy(MathHelper.ToRadians(20 * i * (whichSide ? -1 : 1)));
                //Vector2 goalLocation = new Vector2(0, 500).RotatedBy(20 * i * (whichSide ? -1 : 1));
                float startingRotation = Main.rand.NextFloat(6.28f);
                bool whichSide = Main.rand.NextBool();
                bool topOrBottom = Main.rand.NextBool();

                for (int i = 0; i < 7; i++)
                {
                    Vector2 goalLocation = new Vector2(0,500 * (topOrBottom ? -1 : 1)).RotatedBy(startingRotation + MathHelper.ToRadians(30 * i * (whichSide ? -1 : 1)));
                    int cloneIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), myPlayer.Center + goalLocation * 2, Vector2.Zero, ModContent.ProjectileType<ShadowClone>(), ContactDamage / 4, 2, Main.myPlayer);
                    Projectile Clone = Main.projectile[cloneIndex];

                    if (Clone.ModProjectile is ShadowClone dashers)
                    {
                        dashers.dashSpeed = 16;
                        dashers.SetGoalPoint(goalLocation);
                    }
                }

            }

            if (timer == 90)
            {
                timer = -1;
                advancer++;
            }

            //
            NPC.dontTakeDamage = true;
            NPC.hide = true;
            NPC.Center = myPlayer.Center + new Vector2(0, 500);
            
            timer++;
        }

        #endregion

        #endregion

        bool cutsceneDeathFinished = false;
        bool DrawDeathOrb = false;
        float deathOrbScale = 0f;

        bool dead = false;
        public override void HitEffect(int hitDirection, double damage)
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
        }

        public void DeathAnimation(Player myPlayer) 
        {
            if (timer == 0)
            {
                ballScale = 0f;
                spammingLaser = true;

                //Set hitsound volume to 0f so we don't hear it when we strike later
                NPC.HitSound = SoundID.NPCHit4 with { Volume = 0f };

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
            if (timer < 150)
            {
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                if (Main.rand.NextBool(3))
                {
                    int m = GlowDustHelper.DrawGlowDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowCircleRise>(), 0, 0,
                        Color.Gray * 0.7f, Main.rand.NextFloat(0.9f, 1f), 0.8f, 0f, dustShader);
                    Main.dust[m].velocity.Y = Math.Abs(Main.dust[m].velocity.Y) * -1;
                }
                else if (Main.rand.NextBool(1))
                {
                    int p = GlowDustHelper.DrawGlowDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowCircleRiseFlare>(), 0, 0,
                        Color.HotPink, Main.rand.NextFloat(0.5f, 0.8f), 0.4f, 0f, dustShader2);
                    Main.dust[p].velocity.Y = Math.Abs(Main.dust[p].velocity.Y) * -1;
                }
            }

            //Explosion fx, sound fx
            if (timer == 150)
            {
                deathOrbScale = 0f;
                SoundStyle styleba = new SoundStyle("AerovelenceMod/Sounds/Effects/fireLoopBad") with { Volume = .12f, PitchVariance = .11f, MaxInstances = -1 };
                SoundEngine.PlaySound(styleba, NPC.Center);

                SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .45f, Pitch = .93f, PitchVariance = .11f, MaxInstances = -1 };
                SoundEngine.PlaySound(styleb, NPC.Center);

                //SoundStyle styla = new SoundStyle("Terraria/Sounds/Item_122") with { Pitch = .44f, Volume = 0.9f, PitchVariance = 0.11f };
                //SoundEngine.PlaySound(styla, NPC.Center);

                //SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_109") with { Volume = .51f, Pitch = -.55f, PitchVariance = 0.2f };
                //SoundEngine.PlaySound(stylea, NPC.Center);

                //SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion with { Pitch = 0.6f, Volume = 0.5f }, NPC.Center);
                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_2") with { Pitch = -.63f, PitchVariance = 0.25f, MaxInstances = -1, Volume = 1f };
                SoundEngine.PlaySound(style, NPC.Center);

                //SoundEngine.PlaySound(SoundID.Item70 with { Pitch = -0.8f, Volume = 0.87f, MaxInstances = -1, PitchVariance = 0.25f }, NPC.Center);

                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Pitch = 0.7f, PitchVariance = 0.2f, Volume = 0.8f }, NPC.Center);


                SoundStyle style3 = new SoundStyle("Terraria/Sounds/Item_45") with { Pitch = -.88f, Volume = 0.3f };
                SoundEngine.PlaySound(style3, NPC.Center);

                NPC.DeathSound = SoundID.ScaryScream with { Pitch = 1f, Volume = 0f };

                //explosion
                for (int i = 0; i < 10; i++)
                {
                    int a = Projectile.NewProjectile(null, NPC.Center, new Vector2(2f, 0).RotatedByRandom(6) * Main.rand.NextFloat(0.7f, 2f), ModContent.ProjectileType<FadeExplosionHighRes>(), 0, 0);
                    Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);
                    if (Main.projectile[a].ModProjectile is FadeExplosionHighRes explo)
                    {
                        explo.rise = true;
                        explo.color = Main.rand.NextBool() ? Color.DeepPink : Color.DeepSkyBlue;
                        explo.size = 0.8f;
                        explo.colorIntensity = 0.75f; //0.5
                    }
                }
                NPC.hide = true;

            } else if (timer == 160)
            {
                cutsceneDeathFinished = true;
            }

            if (timer <= 150)
            {
                Main.GameZoomTarget = MathHelper.Lerp(Main.GameZoomTarget, 1.15f, 0.03f);
            } else
            {
                Main.GameZoomTarget = Math.Clamp(MathHelper.Lerp(Main.GameZoomTarget, 0.8f, 0.05f), 1, 2);
            }

            if (cutsceneDeathFinished)
            {
                myPlayer.GetModPlayer<ScreenPlayer>().lerpBackToPlayer = true;
                myPlayer.GetModPlayer<ScreenPlayer>().cutscene = false;
                NPC.StrikeNPC(NPC.lifeMax, 0f, 0, false, noEffect: true);
                spammingLaser = false;
                Main.GameZoomTarget = 1;
            }
            deathOrbScale += 0.008f;
            timer++;
        }

        Vector2 introCenter = Vector2.Zero;
        public void IntroAnimation(Player myPlayer)
        {
            if (timer == 0)
            {
                introCenter = myPlayer.Center + new Vector2(-600, -200);
                NPC.Center = new Vector2(-600, -1900) + myPlayer.Center;

                
                myPlayer.GetModPlayer<ScreenPlayer>().cutscene = true;
                myPlayer.GetModPlayer<ScreenPlayer>().ScreenGoalPos = introCenter;
            }
            Main.GameZoomTarget = MathHelper.Lerp(Main.GameZoomTarget, 1.15f, 0.03f);
            NPC.dontTakeDamage = true;

            NPC.rotation = MathHelper.ToRadians(180) + new Vector2(0,1).ToRotation();

            if (timer > 50 && timer < 150)
            {
                if (timer == 56)
                {
                    SoundStyle styleas = new SoundStyle("Terraria/Sounds/Item_131") with { Pitch = -.2f, PitchVariance = .13f, Volume = 0.4f};
                    SoundEngine.PlaySound(styleas, NPC.Center);
                }
                //move to overshoot
                Vector2 move = (introCenter + new Vector2(0,1400)) - NPC.Center;

                float scalespeed = 0.8f; //2

                NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
                NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;

                NPC.rotation = MathHelper.ToRadians(180) + new Vector2(0, 1).ToRotation();


            }


            if (timer >= 125)
            {
                if (timer == 125)
                {
                    NPC.Center = introCenter + new Vector2(0, -900);
                }
                NPC.rotation = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();

                Vector2 move = (introCenter) - NPC.Center;

                float scalespeed = 0.7f; //2

                NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
                NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;

                if (timer > 250  && timer < 300)
                {

                    if (timer == 251)
                    {
                        SoundStyle style2 = new SoundStyle("Terraria/Sounds/Zombie_66") with { Pitch = -.47f, PitchVariance = 0f, MaxInstances = -1, Volume = 0.2f };
                        SoundEngine.PlaySound(style2, NPC.Center);

                        //SoundStyle style3 = new SoundStyle("Terraria/Sounds/Zombie_68") with { Pitch = .77f, PitchVariance = 0f, MaxInstances = -1, Volume = 0.7f };
                        //SoundEngine.PlaySound(style3, NPC.Center);

                        SoundStyle style3 = new SoundStyle("Terraria/Sounds/Zombie_68") with { Pitch = .77f, PitchVariance = 0f, MaxInstances = -1, Volume = 0.7f };
                        SoundEngine.PlaySound(style3, NPC.Center);

                        //SoundStyle style = new SoundStyle("Terraria/Sounds/Zombie_68") with { Pitch = -0.3f, PitchVariance = 0f, MaxInstances = 1, Volume = 0.5f };
                        //SoundEngine.PlaySound(style, NPC.Center);

                        //SoundEngine.PlaySound(SoundID.Roar with { Pitch = 1f, Volume = 0.05f }, NPC.Center);
                        //SoundEngine.PlaySound(SoundID.ScaryScream with { Pitch = 1f, Volume = 0.07f }, NPC.Center);

                        int b = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-68, 0).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<PinkExplosion>(), 0, 0f);
                        Projectile explo = Main.projectile[b];
                        if (explo.ModProjectile is PinkExplosion pinkExplo)
                        {
                            pinkExplo.exploScale = 4.5f;
                            pinkExplo.numberOfDraws = 2;
                        }

                        
                        for (int i = 0; i < 0; i++)
                        {
                            int a = Projectile.NewProjectile(null, NPC.Center + new Vector2(-78, 0).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<HollowPulse>(), 0, 0, Main.myPlayer);
                            if (Main.projectile[a].ModProjectile is HollowPulse pulse)
                            {
                                pulse.color = Color.HotPink * 0.9f;
                                pulse.oval = false;
                                pulse.size = 2f + (i * 0.05f);
                            }
                        }
                        for (int i = 0; i < 1; i++)
                        {
                            int a = Projectile.NewProjectile(null, NPC.Center + new Vector2(-78, 0).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<HollowPulse>(), 0, 0, Main.myPlayer);
                            if (Main.projectile[a].ModProjectile is HollowPulse pulse)
                            {
                                pulse.color = Color.DeepSkyBlue * 1f;
                                pulse.oval = false;
                                pulse.size = 15;
                            }
                        }
                        

                        ArmorShaderData dustShadera = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                        ArmorShaderData dustShaderb = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                        for (int j = 0; j < 10; j++)
                        {
                            Dust d = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + new Vector2(-80, 0).RotatedBy(NPC.rotation), ModContent.DustType<GlowLine1Fast>(), Vector2.One.RotatedByRandom(j * 2) * (j + 1f),
                                Color.DeepSkyBlue, 0.3f, 0.6f, 0f, dustShadera);
                            d.velocity *= 0.4f;
                            d.fadeIn = 45;

                        }
                        for (int j = 0; j < 10; j++)
                        {
                            Dust d = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + new Vector2(-80, 0).RotatedBy(NPC.rotation), ModContent.DustType<GlowLine1Fast>(), Vector2.One.RotatedByRandom(j * 2) * (j + 1f),
                                Color.DeepPink, 0.3f, 0.6f, 0f, dustShaderb);
                            d.velocity *= 0.4f;

                            d.fadeIn = 45;
                        }
                    }

                }
            }
            if (timer == 300)
            {
                myPlayer.GetModPlayer<ScreenPlayer>().lerpBackToPlayer = true;
                myPlayer.GetModPlayer<ScreenPlayer>().cutscene = false;
            }

            if (timer >= 300)
            {
                Main.GameZoomTarget = Math.Clamp(MathHelper.Lerp(Main.GameZoomTarget, 0.8f, 0.05f), 1, 2);
            }

            if (timer == 350)
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

        float barrageCount = 1;
        bool trueSpinFalseAzzy = true;
        bool trueWrapFalseChase = true;
        bool trueCloneFalseBot = true;

        public void SetNextAttack(String nextAttackName)
        {
            if (nextAttackName == "Barrage")
            {
                whatAttack = 1;
                barrageCount = 1;
            } else if (nextAttackName == "Laser")
            {
                if (trueSpinFalseAzzy)
                {
                    whatAttack = 3;
                    trueSpinFalseAzzy = !trueSpinFalseAzzy;
                } else
                {
                    whatAttack = 4;
                    trueSpinFalseAzzy = !trueSpinFalseAzzy;
                }

                if (!Phase2) //omg recurrsice fi=unction
                    whatAttack = 3;
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

                if (!Phase2) //omg recurrsice fi=unction
                    SetNextAttack("Summon");
            }
            else if (nextAttackName == "Summon")
            {
                if (trueCloneFalseBot)
                {
                    whatAttack = 6;
                    trueCloneFalseBot = !trueCloneFalseBot;
                }
                else
                {
                    whatAttack = 8;
                    trueCloneFalseBot = !trueCloneFalseBot;
                }

                if (!Phase2)
                    whatAttack = 6;
            }
            else
            {
                Main.NewText("very bad");
            }

            float phase2TransValue = isExpert ? (NPC.lifeMax - (NPC.lifeMax / 4)) : (NPC.lifeMax - (NPC.lifeMax / 3));

            if (NPC.life < phase2TransValue && !Phase2)
            {
                SoundStyle style3 = new SoundStyle("Terraria/Sounds/Zombie_68") with { Pitch = .77f, PitchVariance = 0f, MaxInstances = -1, Volume = 0.7f };
                SoundEngine.PlaySound(style3, NPC.Center);
                Phase2 = true;

                trueSpinFalseAzzy = true;
                trueWrapFalseChase = true;
                trueCloneFalseBot = true;

                whatAttack = 1;
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

  
        public void FireLaser(int type, float speed = 6f, float recoilMult = 2f, float ai1 = 0, float ai2 = 0)
        {
            var entitySource = NPC.GetSource_FromAI();
            Player player = Main.player[NPC.target];
            Vector2 toPlayer = player.Center - NPC.Center;
            toPlayer = toPlayer.SafeNormalize(new Vector2(1, 0));
            toPlayer *= speed;
            Vector2 from = NPC.Center - new Vector2(96, 0).RotatedBy(NPC.rotation);
            int damage = 75 / 4;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Projectile.NewProjectile(entitySource, from, toPlayer, type, damage, 3, Main.myPlayer, ai1, ai2);
            }
            NPC.velocity -= toPlayer * recoilMult;

            if (type == ModContent.ProjectileType<CyverLaser>() || type == ModContent.ProjectileType<StretchLaser>())
            {

                float addition = (whatAttack == 0 ? -1 : 0);
                for (int i = 0; i < 4 + (addition * 2); i++) //4 //2,2
                {

                    Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f)) * Main.rand.Next(5, 10) * -1f;

                    Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -50, ModContent.DustType<GlowLine1Fast>(), vel * 3f,
                        Color.HotPink, Main.rand.NextFloat(0.1f, 0.2f), 0.6f, 0f, dustShader2);
                    p.noLight = false;
                    //p.velocity += NPC.velocity * (0.8f + Main.rand.NextFloat(-0.1f, -0.2f));
                    p.fadeIn = (whatAttack == 0 ? 30 : 40) + Main.rand.NextFloat(-5, 10);
                    p.velocity *= 0.4f;
                }
                for (int i = 0; i < 3 - addition; i++) //4 //2,2
                {

                    Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.Next(5, 10) * -1f;

                    Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -50, ModContent.DustType<GlowLine1Fast>(), vel * 3f,
                        Color.HotPink, Main.rand.NextFloat(0.1f, 0.2f), 0.6f, 0f, dustShader2);
                    p.noLight = false;
                    //p.velocity += NPC.velocity * (0.8f + Main.rand.NextFloat(-0.1f, -0.2f));
                    p.fadeIn = (whatAttack == 0 ? 30 : 40) + Main.rand.NextFloat(-5, 10);
                    p.velocity *= 0.2f;
                }


            }

        }

        public void ShotDust()
        {
            for (int i = 0; i < 5; i++) //4 //2,2
            {

                Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f)) * Main.rand.Next(5, 10) * -1f;

                Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -50, ModContent.DustType<GlowLine1Fast>(), vel * 3f,
                    Color.HotPink, Main.rand.NextFloat(0.1f, 0.2f), 0.6f, 0f, dustShader2);
                p.noLight = false;
                //p.velocity += NPC.velocity * (0.8f + Main.rand.NextFloat(-0.1f, -0.2f));
                p.fadeIn = (whatAttack == 0 ? 30 : 40) + Main.rand.NextFloat(-5, 10);
                p.velocity *= 0.4f;
            }
            for (int i = 0; i < 2; i++) //4 //2,2
            {

                Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.Next(5, 10) * -1f;

                Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -50, ModContent.DustType<GlowLine1Fast>(), vel * 3f,
                    Color.HotPink, Main.rand.NextFloat(0.1f, 0.2f), 0.6f, 0f, dustShader2);
                p.noLight = false;
                //p.velocity += NPC.velocity * (0.8f + Main.rand.NextFloat(-0.1f, -0.2f));
                p.fadeIn = (whatAttack == 0 ? 30 : 40) + Main.rand.NextFloat(-5, 10);
                p.velocity *= 0.2f;
            }
        }

        //This is for the custom sky being reactive
        public float bigShotTimer = 0;
        public int getAttack()
        {
            if (spammingLaser)
                return -1;
            else
                return whatAttack;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {

            float newScale = 0.75f;

            return base.DrawHealthBar(hbPosition, ref newScale, ref position);
        }
    }
}
