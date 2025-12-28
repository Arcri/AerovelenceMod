using AerovelenceMod.Common;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.Weapons.CrystalCaverns.GaussShotgun;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns.BooyahBomb
{

    //TODO:
    //- Balance
    //- Skill Strike
    //- Optainability
    //- Sprite
    //- Tooltip
    public class BooyahBomb : ModItem
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.knockBack = KnockbackTiers.Weak;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 23; //DO NOT CHANGE THIS. It fucks up the arm at higher useTime for some reason
            Item.shootSpeed = 12f;

            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<BooyahHeldProj>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.rare = ItemRarities.EarlyPHM;
            Item.value = Item.buyPrice(0, 0, 50, 0);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.channel = true;
        }

    }

    //This projectile is the bomb while it is held in the player's hand
    public class BooyahHeldProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanDamage() => false;


        int chargeTime = 75;

        int timer = 0;
        public override void AI()
        {
            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            Player owner = Main.player[Projectile.owner];

            //Face the direction of the mouse
            if (owner.whoAmI == Main.myPlayer)
                owner.direction = Main.MouseWorld.X > owner.Center.X ? 1 : -1;

            Vector2 orbPosOffset = new Vector2(-11f * owner.direction, -30f + owner.gfxOffY);

            Projectile.velocity = Vector2.Zero;
            Projectile.Center = owner.Center + orbPosOffset;


            float chargeProg = Utils.GetLerpValue(0f, 1f, (float)timer / (float)chargeTime, true);

            if (owner.channel || chargeProg < 1f)
            {
                //Prevent the projectile from despawing while being channeled or not at full charge
                owner.itemAnimation = 20;
                owner.itemTime = 20;
                Projectile.timeLeft = 20;

                //Release some electric dust
                if (timer % 4 == 0)
                {
                    Dust dp = Dust.NewDustPerfect(owner.Center + orbPosOffset, ModContent.DustType<ElectricSparkGlow>(), newColor: Color.DeepSkyBlue, Scale: Main.rand.NextFloat(0.75f, 1f) + (chargeProg * 0.5f));
                    dp.velocity *= 1f + chargeProg;

                    ElectricSparkBehavior esb = new ElectricSparkBehavior(FadeAlphaPower: 0.89f, FadeScalePower: 0.91f, FadeVelPower: 0.92f, Pixelize: true, XScale: 1f, YScale: 1f); //0.91
                    esb.killEarlyTime = 20;
                    dp.customData = esb;
                }
            }
            else
            {
                //Shoot the bomb | (Projectile.ai[0] tracks whether the bomb has been shot yet or not)
                if (Projectile.ai[0] == 0 && Main.myPlayer == Projectile.owner)
                {
                    Vector2 toMouse = (Main.MouseWorld - owner.Center).SafeNormalize(Vector2.UnitX);
                    Vector2 shotVel = toMouse * 12f;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.Center + orbPosOffset, shotVel, ModContent.ProjectileType<BooyahBombProj>(), Projectile.damage, Projectile.knockBack, owner.whoAmI);

                    //Apply recoil, but more in the vertical direction than horizontal
                    if (owner.velocity.Y != 0)
                        owner.velocity.X += toMouse.X * -6f;

                    owner.velocity.Y *= 0.25f;
                    owner.velocity.Y += toMouse.Y * -10f;

                    //Reset player staring fall pos so they don't explode from fall damage even if they significantly slow their fall with this weapon
                    if (toMouse.Y > 0.25f)
                        owner.fallStart = owner.position.ToTileCoordinates().Y;

                    Projectile.ai[0]++;
                }
            }

            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3() * chargeProg);

            timer++;
        }

        Effect myEffect = null;
        public List<float> previousRotations = new List<float>();
        public List<Vector2> previousPositions = new List<Vector2>();
        public override bool PreDraw(ref Color lightColor)
        {
            //Stop drawing if we've released the shot
            if (timer > chargeTime && !Main.player[Projectile.owner].channel)
                return false;

            float postFullChargeProg = Utils.GetLerpValue(chargeTime, chargeTime + 15, timer, true);

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D gash = CommonTextures.SoulSpike.Value;

            float sineScale1 = 1f + (float)Math.Sin(Main.timeForVisualEffects * 0.12f) * 0.1f;
            float sineScale2 = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.22f) * 0.06f;

            Color between = Color.Lerp(Color.DodgerBlue, Color.DeepSkyBlue, 0.5f);

            Vector2 gashScale = new Vector2(1f * Easings.easeOutCubic(postFullChargeProg) * sineScale2, 0.45f * sineScale1) * Projectile.scale;
            Main.EntitySpriteDraw(gash, drawPos, null, between with { A = 0 } * 0.35f, 0f, gash.Size() / 2f, gashScale * 2f, SpriteEffects.None);
            Main.EntitySpriteDraw(gash, drawPos, null, Color.White with { A = 0 } * 0.35f, 0f, gash.Size() / 2f, gashScale * 1f, SpriteEffects.None);

            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.Dusts, () =>
            {
                DrawBasicBall(false);
            });

            ModContent.GetInstance<NewAdditivePixelationSystem>().QueueRenderAction(RenderLayer.Dusts, () =>
            {
                DrawBall(false);
            });

            return false;
        }

        //This is the non-shader part of the orb, just some stacked bloom orbs
        public void DrawBasicBall(bool giveUp)
        {
            if (giveUp)
                return;

            float chargeProg = Utils.GetLerpValue(0f, 1f, (float)timer / (float)chargeTime, true);
            float postFullChargeProg = Utils.GetLerpValue(chargeTime, chargeTime + 15, timer, true);

            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            //Draw Orb
            Texture2D Orb = CommonTextures.feather_circle128PMA.Value;

            Color[] cols = { Color.White * 1f, Color.DeepSkyBlue * 0.525f, Color.DodgerBlue * 0.375f };
            float[] scales = { 0.85f, 1.45f, 2.5f };

            float orbAlpha = 1f;
            float totalScale = Projectile.scale * 0.45f * Easings.easeInOutQuad(chargeProg);

            float sineScale1 = 1f + (float)Math.Sin(Main.timeForVisualEffects * 0.12f) * 0.1f;
            float sineScale2 = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.22f) * 0.06f;

            Main.EntitySpriteDraw(Orb, drawPos, null, Color.DodgerBlue * orbAlpha * 0.35f, 0f, Orb.Size() / 2f, scales[2] * totalScale, SpriteEffects.None);

            Main.EntitySpriteDraw(Orb, drawPos, null, cols[0] with { A = 0 } * orbAlpha, 0f, Orb.Size() / 2f, scales[0] * totalScale, SpriteEffects.None);
            Main.EntitySpriteDraw(Orb, drawPos, null, cols[1] with { A = 0 } * orbAlpha, 0f, Orb.Size() / 2f, scales[1] * totalScale * sineScale1, SpriteEffects.None);
            Main.EntitySpriteDraw(Orb, drawPos, null, cols[2] with { A = 0 } * orbAlpha, 0f, Orb.Size() / 2f, scales[2] * totalScale * sineScale2, SpriteEffects.None);
        }

        //This is the shader part of the orb | Might be nice to replace this shader with the version that doesn't use a texture for gradients once I actually write that
        public void DrawBall(bool giveUp)
        {
            if (giveUp)
                return;

            Texture2D ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/bigCircle2").Value;
            Texture2D ball2 = CommonTextures.feather_circle128PMA.Value;

            float chargeProg = Utils.GetLerpValue(0f, 1f, (float)timer / chargeTime, true);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            float drawScale = Projectile.scale * Easings.easeInQuad(chargeProg) * 0.3f;

            float sineScale1 = 1f + (float)Math.Sin(Main.timeForVisualEffects * 0.055f) * 0.07f;
            float sineScale2 = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.1f) * 0.07f;
            float sineScale3 = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.2f + timer * 0.05f) * 0.03f;
            float sineColor = (float)Math.Sin(Main.timeForVisualEffects * 0.08f) * 0.2f;

            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/NewRadialScroll", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/WaterEnergyNoise").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/SofterBlueGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/sparkNoiseloop").Value);
            myEffect.Parameters["flowSpeed"].SetValue(1f);
            myEffect.Parameters["distortStrength"].SetValue(0.06f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);

            myEffect.Parameters["vignetteSize"].SetValue(1f);
            myEffect.Parameters["vignetteBlend"].SetValue(0.5f);
            myEffect.Parameters["colorIntensity"].SetValue(2f * chargeProg);

            //Main shader
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.EffectMatrix);

            float rot1 = (float)Main.timeForVisualEffects * 0.01f;
            Main.spriteBatch.Draw(ball, drawPos, null, Color.White with { A = 0 }, rot1, ball.Size() / 2, drawScale * sineScale3, 0f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }




    }

    public class BooyahBombProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";


        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public bool isHeld = true;

        float justShotVal = 1f;

        int timer = 0;
        public override void AI()
        {
            Projectile.velocity.Y += 0.25f;

            int trailCount = 30;
            previousRotations.Add(Projectile.velocity.ToRotation());
            previousPositions.Add(Projectile.Center + Projectile.velocity);

            if (previousRotations.Count > trailCount)
                previousRotations.RemoveAt(0);

            if (previousPositions.Count > trailCount)
                previousPositions.RemoveAt(0);


            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3());

            if (timer > 5)
                justShotVal = Math.Clamp(MathHelper.Lerp(justShotVal, -0.35f, 0.04f), 0f, 1f);

            if (timer % 4 == 0)
            {
                Dust dp = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ElectricSparkGlow>(), newColor: Color.DeepSkyBlue, Scale: Main.rand.NextFloat(0.85f, 1f));
                dp.velocity += Projectile.velocity * 0.5f;

                ElectricSparkBehavior esb = new ElectricSparkBehavior(FadeAlphaPower: 0.8f, FadeScalePower: 0.91f, FadeVelPower: 0.92f, Pixelize: false, XScale: 1f, YScale: 1f); //0.91
                esb.killEarlyTime = 20;
                dp.customData = esb;
            }

            timer++;
        }

        public override bool PreKill(int timeLeft)
        {
            SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/ElectricExplode") with { Volume = 0.05f, Pitch = 0.35f, PitchVariance = 0.15f, MaxInstances = -1, };
            SoundEngine.PlaySound(style, Projectile.Center);

            Color between2 = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.15f);
            Dust d1 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStarSharp>(), Velocity: Vector2.Zero, newColor: between2, Scale: 2.4f);
            Dust d2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStarSharp>(), Velocity: Vector2.Zero, newColor: Color.White, Scale: 1.2f);

            d1.customData = DustBehaviorUtil.AssignBehavior_GSSBase(fadePower: 0.85f, shouldFadeColor: true);
            d2.customData = DustBehaviorUtil.AssignBehavior_GSSBase(fadePower: 0.85f, shouldFadeColor: true);
            
            //VFX Projectiles
            int gaussImpact = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GaussExplosionVFX>(), 0, 0, Main.myPlayer);
            
            int booyahImpact = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BooyahImpactVFX>(), 0, 0, Main.myPlayer);
            Main.projectile[booyahImpact].spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;

            //Hit all enemies in a radius
            GeneralUtils.strikeNPCsInRadius(Projectile.Center, 100f, Projectile.damage * 0.5f, Projectile.knockBack * 0.5f);


            return base.PreKill(timeLeft);
        }

        int initialDir = 1;

        float overallAlpha = 1f;
        float overallScale = 1f;

        //The orb draws identically to the held projectile (except for the horizontal gash)
        Effect myEffect = null;
        public List<float> previousRotations = new List<float>();
        public List<Vector2> previousPositions = new List<Vector2>();
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D gash = CommonTextures.SoulSpike.Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            float sineScale1 = 1f + (float)Math.Sin(Main.timeForVisualEffects * 0.12f) * 0.1f;
            float sineScale2 = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.22f) * 0.06f;

            Color between = Color.Lerp(Color.DodgerBlue, Color.DeepSkyBlue, 0.5f);

            Vector2 gashScale = new Vector2(2.5f * Easings.easeOutCubic(1f - justShotVal) * sineScale2, 0.45f * sineScale1) * Projectile.scale;
            Main.EntitySpriteDraw(gash, drawPos, null, between with { A = 0 } * Easings.easeInQuad(justShotVal) * 1f, 0f, gash.Size() / 2f, gashScale * 2f, SpriteEffects.None);
            Main.EntitySpriteDraw(gash, drawPos, null, Color.White with { A = 0 } * Easings.easeInQuad(justShotVal) * 1f, 0f, gash.Size() / 2f, gashScale * 1f, SpriteEffects.None);

            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.OverPlayers, () =>
            {
                DrawTrail(false);
                DrawBasicBall(false);
            });

            ModContent.GetInstance<NewAdditivePixelationSystem>().QueueRenderAction(RenderLayer.OverPlayers, () =>
            {
                DrawBall(false);
            });

            return false;
        }

        public void DrawBasicBall(bool giveUp)
        {
            if (giveUp)
                return;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            //Draw Orb
            Texture2D Orb = CommonTextures.feather_circle128PMA.Value;

            Color[] cols = { Color.White * 1f, Color.DeepSkyBlue * 0.525f, Color.DodgerBlue * 0.375f };
            float[] scales = { 0.85f, 1.45f, 2.5f };

            float orbAlpha = 1f;
            float totalScale = Projectile.scale * 0.45f * Easings.easeInOutQuad(overallScale);

            float sineScale1 = 1f + (float)Math.Sin(Main.timeForVisualEffects * 0.12f) * 0.1f;
            float sineScale2 = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.22f) * 0.06f;

            Main.EntitySpriteDraw(Orb, drawPos, null, Color.DodgerBlue * orbAlpha * 0.35f, 0f, Orb.Size() / 2f, scales[2] * totalScale, SpriteEffects.None);

            Main.EntitySpriteDraw(Orb, drawPos, null, cols[0] with { A = 0 } * orbAlpha, 0f, Orb.Size() / 2f, scales[0] * totalScale, SpriteEffects.None);
            Main.EntitySpriteDraw(Orb, drawPos, null, cols[1] with { A = 0 } * orbAlpha, 0f, Orb.Size() / 2f, scales[1] * totalScale * sineScale1, SpriteEffects.None);
            Main.EntitySpriteDraw(Orb, drawPos, null, cols[2] with { A = 0 } * orbAlpha, 0f, Orb.Size() / 2f, scales[2] * totalScale * sineScale2, SpriteEffects.None);
        }

        public void DrawBall(bool giveUp)
        {
            if (giveUp)
                return;

            Texture2D ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/bigCircle2").Value;
            Texture2D ball2 = CommonTextures.feather_circle128PMA.Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            float drawScale = Projectile.scale * Easings.easeOutCirc(1f) * 0.3f;

            float sineScale1 = 1f + (float)Math.Sin(Main.timeForVisualEffects * 0.055f) * 0.07f;
            float sineScale2 = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.1f) * 0.07f;
            float sineScale3 = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.2f + timer * 0.05f) * 0.03f;
            float sineColor = (float)Math.Sin(Main.timeForVisualEffects * 0.08f) * 0.2f;

            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("VFXPlus/Effects/Radial/NewRadialScroll", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("VFXPlus/Assets/Noise/WaterEnergyNoise").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("VFXPlus/Assets/Gradients/SofterBlueGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("VFXPlus/Assets/Noise/sparkNoiseloop").Value);
            myEffect.Parameters["flowSpeed"].SetValue(1f);
            myEffect.Parameters["distortStrength"].SetValue(0.06f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);

            myEffect.Parameters["vignetteSize"].SetValue(1f);
            myEffect.Parameters["vignetteBlend"].SetValue(0.5f);
            myEffect.Parameters["colorIntensity"].SetValue(2f * overallAlpha);

            //Main shader
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.EffectMatrix);

            float rot1 = (float)Main.timeForVisualEffects * 0.01f;
            Main.spriteBatch.Draw(ball, drawPos, null, Color.White with { A = 0 }, rot1, ball.Size() / 2, drawScale * sineScale3, 0f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        //Basic vertex trail using TendrilShader with two layers
        Effect trailEffect = null;
        public void DrawTrail(bool giveUp)
        {
            if (giveUp)
                return;

            Texture2D trailTexture = Mod.Assets.Request<Texture2D>("Assets/Trails/EvenThinnerGlowLine").Value; 
            Texture2D trailTexture2 = Mod.Assets.Request<Texture2D>("Assets/Trails/Trail5Loop").Value;

            if (trailEffect == null)
                trailEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/TrailShaders/TendrilShader", AssetRequestMode.ImmediateLoad).Value;


            //Convert lists to arrays for use in vertex strip
            Vector2[] pos_arr = previousPositions.ToArray();
            float[] rot_arr = previousRotations.ToArray();


            Color StripColor(float progress) => Color.White * Easings.easeInCubic(progress * progress) * overallAlpha * (1f - justShotVal);

            float StripWidth(float progress)
            {
                float toReturn = 0f;
                if (progress < 0.95f) //back half
                {
                    float LV = Utils.GetLerpValue(0f, 0.95f, progress, true);
                    toReturn = Easings.easeOutSine(LV);
                }
                else //Front half
                {
                    float LV = Utils.GetLerpValue(0.5f, 1f, progress, true);
                    toReturn = 1f;
                }

                return toReturn * overallScale * 102;
            }

            float StripWidth2(float progress)
            {
                float toReturn = 0f;
                if (progress < 0.95f) //back half
                {
                    float LV = Utils.GetLerpValue(0f, 0.95f, progress, true);
                    toReturn = Easings.easeOutSine(LV);
                }
                else //Front half
                {
                    float LV = Utils.GetLerpValue(0.5f, 1f, progress, true);
                    toReturn = 1f;
                }

                return toReturn * overallScale * 70f;
            }


            VertexStrip vertexStrip = new VertexStrip();
            vertexStrip.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth, -Main.screenPosition, includeBacksides: true);

            VertexStrip vertexStrip2 = new VertexStrip();
            vertexStrip2.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth2, -Main.screenPosition, includeBacksides: true);


            trailEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            trailEffect.Parameters["progress"].SetValue((float)Main.timeForVisualEffects * 0.02f); //0.02

            //Make the repitions of the texture be based on texture length so it doesn't look different as the number of points increases
            float repPercent = (float)previousPositions.Count / 30f;
            trailEffect.Parameters["reps"].SetValue(0.5f * repPercent);

            //Under layer
            trailEffect.Parameters["TrailTexture"].SetValue(trailTexture);
            trailEffect.Parameters["ColorOne"].SetValue(Color.DeepSkyBlue.ToVector3() * 1f);
            trailEffect.Parameters["glowThreshold"].SetValue(1f);
            trailEffect.Parameters["glowIntensity"].SetValue(1.2f);
            trailEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip.DrawTrail();

            Color between = Color.Lerp(Color.SkyBlue, Color.DeepSkyBlue, 0.4f);
            //Over 
            trailEffect.Parameters["TrailTexture"].SetValue(trailTexture2);
            trailEffect.Parameters["glowThreshold"].SetValue(1f);
            trailEffect.Parameters["glowIntensity"].SetValue(1f);
            trailEffect.Parameters["ColorOne"].SetValue(between.ToVector3() * 2.5f); //Hotpink4.5
            trailEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip2.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

    }


    public class BooyahImpactVFX : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 22900;
        }

        public override bool? CanDamage() => false;

        float overallAlpha = 1f;
        float overallScale = 1f;

        int timer = 0;

        public override void AI()
        {

            if (timer == 0)
                Projectile.rotation = Main.rand.NextFloat(6.28f);

            int timeForPulse = 15;
            if (timer <= timeForPulse)
                overallScale = MathHelper.Lerp(0.1f, 0.75f, Easings.easeOutSine((float)timer / (float)timeForPulse)); //.1 .75 outCubic

            if (timer >= 0)
            {
                if (timer >= (timeForPulse * 0.75f))
                    overallAlpha -= 0.08f;

                if (timer > timeForPulse + 5) //
                    overallScale = Math.Clamp(MathHelper.Lerp(overallScale, -0.25f, 0.01f), 0f, 1f); //

                if (overallAlpha <= 0)
                    Projectile.active = false;
            }

            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3() * overallScale);

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.UnderProjectiles, () =>
            {
                DrawCrack(false);
            });

            DrawCrack(true);

            return false;
        }

        Effect myEffect = null;
        public void DrawCrack(bool giveUp = false)
        {
            if (giveUp)
                return;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Texture2D ball = Mod.Assets.Request<Texture2D>("Assets/Ring/ThunderRing4").Value;
            Texture2D ring = Mod.Assets.Request<Texture2D>("Assets/Ring/GlowRing2").Value;

            Texture2D orb = Mod.Assets.Request<Texture2D>("Assets/Flare/flare_4Black").Value;
            float rot2 = Projectile.rotation + (float)(Main.timeForVisualEffects * 0.075f) * Projectile.spriteDirection;
            Main.EntitySpriteDraw(orb, drawPos, null, Color.DodgerBlue with { A = 0 } * overallAlpha * 0.5f, rot2 * 1.5f, orb.Size() / 2f, 1.5f * overallScale, SpriteEffects.None);
            Main.EntitySpriteDraw(orb, drawPos, null, Color.SkyBlue with { A = 0 } * overallAlpha * 0.5f, -rot2, orb.Size() / 2f, 0.8f * overallScale * overallAlpha, SpriteEffects.None);
            Main.EntitySpriteDraw(orb, drawPos, null, Color.White with { A = 0 } * overallAlpha * 0.5f, rot2 * 0.5f, orb.Size() / 2f, 0.45f * overallScale * overallAlpha, SpriteEffects.None);


            Main.EntitySpriteDraw(ring, drawPos, null, Color.DodgerBlue with { A = 0 } * overallAlpha * 1f, 0f, ring.Size() / 2f, 0.26f * overallScale, SpriteEffects.None);

            float rot = Projectile.rotation + (float)(Main.timeForVisualEffects * 0.1f) * Projectile.spriteDirection;
            Main.EntitySpriteDraw(ball, drawPos, null, Color.LightSkyBlue with { A = 0 } * overallAlpha * 1.5f, rot * 1.5f, ball.Size() / 2f, 0.5f * overallScale, SpriteEffects.None);
            Main.EntitySpriteDraw(ball, drawPos, null, Color.LightSkyBlue with { A = 0 } * overallAlpha * 2f, -rot, ball.Size() / 2f, 0.25f * overallScale, SpriteEffects.None);
        }

    }
}
