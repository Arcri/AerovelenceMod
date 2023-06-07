using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics;
using AerovelenceMod.Content.Projectiles;
using MonoMod.Utils;
using AerovelenceMod.Common.Utilities;
using static Humanizer.In;
using ReLogic.Content;
using Terraria.Map;
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.Items.Weapons.Caverns.ThunderLance
{
    public class ThunderLanceProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        private enum AttackType 
        {
            StrongStab,
            QuickStab,
            Spin,
            Special,
        }
        private AttackType CurrentAttack
        {
            get => (AttackType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }


        public override void SetDefaults()
        {
            Projectile.width = 35;
            Projectile.height = 35;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
        }

        public override bool? CanDamage()
        {
            return hitPause <= 0 && thrusting;
        }

        public ref float timer => ref Projectile.ai[1];
        int vfxTimer = 0;

        public Player owner => Main.player[Projectile.owner];

        Vector2 offset = Vector2.Zero;

        float angleOffsetBigStab = 0f;

        bool thrusting = false;

        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();
        public override void AI()
        {
            Trail();
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;

            if (!owner.channel)
            {
                Projectile.active = false;
            }


            Projectile.velocity = Vector2.Normalize(Projectile.velocity);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (hitPause <= 0)
            {
                if (timer < 20)
                {
                    float lerpVal = Easings.easeOutSine(Math.Clamp(timer / 17f, 0, 1));
                    offset = Vector2.Lerp(new Vector2(70, 0), new Vector2(40, 0), lerpVal);

                    if (timer < 15)
                    {
                        float progress = timer / 15f;
                        angleOffsetBigStab = MathHelper.Lerp(angleOffsetBigStab, MathHelper.ToRadians(10 * Projectile.direction), progress);
                    }

                }
                else if (timer >= 25 && timer <= 40)
                {
                    if (timer == 25)
                    {
                        SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/lightning_flash_01") with { Pitch = 1f, PitchVariance = 0.2f, Volume = 0.7f };
                        SoundEngine.PlaySound(style, Projectile.Center);

                        SoundStyle style2 = new SoundStyle("Terraria/Sounds/Thunder_0") with { Volume = .4f, Pitch = .6f, PitchVariance = .22f, }; 
                        SoundEngine.PlaySound(style2, Projectile.Center);

                        owner.GetModPlayer<AeroPlayer>().ScreenShakePower = 6;

                        thrusting = true;

                        
                    }
                    else if (timer == 29)
                    {
                        for (int i = 0; i < 6; i++)
                        {

                            Vector2 trueVel = owner.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX);

                            Dust a = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, ModContent.DustType<GlowStrong>(),
                                trueVel.RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f)) * Main.rand.NextFloat(5.0f, 7.5f),
                                newColor: Color.DodgerBlue, Scale: Main.rand.NextFloat(0.4f, 0.45f));

                            //a.fadeIn = 2;
                        }

                    }

                    float progress = (timer - 25f) / 5f;
                    angleOffsetBigStab = MathHelper.Lerp(angleOffsetBigStab, 0, progress);

                    if (timer < 33)
                        extraVFXIntensity = 2.1f;

                    if (timer == 45)
                        thrusting = false;

                    float lerpVal = Easings.easeOutQuint(Math.Clamp((timer - 25f) / 10f, 0, 1));
                    offset = Vector2.Lerp(new Vector2(40, 0), new Vector2(120, 0), lerpVal);
                }
                else if (timer > 40)
                {
                    float progress = (timer - 40f) / 10f;
                    angleOffsetBigStab = MathHelper.Lerp(angleOffsetBigStab, MathHelper.ToRadians(0), progress);


                    float lerpVal = Easings.easeInOutSine(Math.Clamp((timer - 40f) / 30f, 0, 1));
                    offset = Vector2.Lerp(new Vector2(120, 0), new Vector2(40, 0), lerpVal);

                    thrusting = false;
                }

                timer++;

                extraVFXIntensity = Math.Clamp(MathHelper.Lerp(extraVFXIntensity, -0.5f, 0.06f), 1, 10);

            }





            Projectile.velocity = owner.DirectionTo(Main.MouseWorld).RotatedBy(angleOffsetBigStab);

            Projectile.timeLeft = 2;

            Projectile.Center = owner.MountedCenter + offset.RotatedBy(Projectile.rotation - MathHelper.PiOver2);

            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.Pi);
            owner.ChangeDir(Projectile.direction);

            hitPause--;
            vfxTimer++;

        }


        public void Trail()
        {
            //Trail1 Info Dump
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlamesTextureButBlack").Value;
            trail1.trailColor = Color.White * 0.7f;
            trail1.trailPointLimit = 100;
            trail1.trailWidth = 11;
            trail1.trailMaxLength = 100;
            trail1.timesToDraw = 2;

            trail1.trailTime = vfxTimer * 0.02f;
            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center;
            trail1.TrailLogic();

            //Trail2 Info Dump
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail5Loop").Value;
            trail2.trailColor = Color.DeepSkyBlue;
            trail2.trailPointLimit = 100;
            trail2.trailWidth = 40;
            trail2.trailMaxLength = 100;
            trail2.timesToDraw = 2;

            //trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/LoopingThunderGrad").Value;
            trail2.shouldScrollColor = true;
            trail2.gradientTime = vfxTimer * 0.03f;

            trail2.trailTime = vfxTimer * 0.03f;
            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center;
            trail2.TrailLogic();
        }

        Effect myEffect = null;
        float extraVFXIntensity = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            //trail1.TrailDrawing(Main.spriteBatch);
            //trail2.TrailDrawing(Main.spriteBatch);

            if (myEffect == null)
            {
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;
            }

            spearBackGlow();


            Texture2D Spear = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Caverns/ThunderLance/ThunderLanceProjectile");
            Texture2D SpearGlowMask = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Caverns/ThunderLance/ThunderLanceGlowMask");

            Vector2 extraOffset = new Vector2(-55, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2);

            Main.spriteBatch.Draw(Spear, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + extraOffset, null, lightColor, Projectile.rotation - MathHelper.PiOver4, Spear.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(SpearGlowMask, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + extraOffset, null, Color.White, Projectile.rotation - MathHelper.PiOver4, SpearGlowMask.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);


            spearTop();

            return false;
        }

        #region DrawMethods
        public void spearBackGlow()
        {
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Caverns/ThunderLance/ThunderLanceBackGlow");
            Texture2D DiamondGlow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/DiamondGlow");
            Texture2D TipGlow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Caverns/ThunderLance/ThunderLanceTipGlow");

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 vec2Scale1 = new Vector2(0.55f, 1f) * Projectile.scale * 0.7f * (extraVFXIntensity * 0.85f);
            Vector2 vec2Scale2 = new Vector2(0.55f, 1f) * Projectile.scale * 0.7f * (extraVFXIntensity * 0.85f);
            Vector2 vec2Scale3 = new Vector2(0.25f, 1f) * Projectile.scale * 0.7f * (extraVFXIntensity * 0.85f);

            Main.spriteBatch.Draw(DiamondGlow, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3, 3), null, Color.DeepSkyBlue * 0.3f, Projectile.rotation, DiamondGlow.Size() / 2, vec2Scale1 * 0.6f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(DiamondGlow, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3, 3), null, Color.DeepSkyBlue * 0.6f, Projectile.rotation, DiamondGlow.Size() / 2, vec2Scale2 * 0.45f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(DiamondGlow, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3, 3), null, Color.White * 0.8f, Projectile.rotation, DiamondGlow.Size() / 2, vec2Scale3 * 0.45f, SpriteEffects.None, 0f);

            Vector2 tipGlowOffset = new Vector2(0, 10f).RotatedBy(Projectile.rotation);
            Main.spriteBatch.Draw(TipGlow, Projectile.Center - Main.screenPosition + tipGlowOffset, null, Color.SkyBlue * 0.8f, Projectile.rotation - MathHelper.PiOver4, TipGlow.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(TipGlow, Projectile.Center - Main.screenPosition + tipGlowOffset, null, Color.DeepSkyBlue * 0.3f, Projectile.rotation - MathHelper.PiOver4, TipGlow.Size() / 2, Projectile.scale * 1.25f, SpriteEffects.None, 0f);


            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/ThunderGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);

            myEffect.Parameters["flowSpeed"].SetValue(0.3f);
            myEffect.Parameters["vignetteSize"].SetValue(0f);
            myEffect.Parameters["vignetteBlend"].SetValue(1f);
            myEffect.Parameters["distortStrength"].SetValue(0.01f);
            myEffect.Parameters["xOffset"].SetValue(0.0f);
            myEffect.Parameters["uTime"].SetValue(vfxTimer * 0.02f);

            Vector2 scaleV = new Vector2(1f, 1f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);



            myEffect.CurrentTechnique.Passes[0].Apply();


            Vector2 origin = new Vector2(Glow.Width * 0.5f, Glow.Height * 0.5f); //.95, 0.05
            Vector2 extraOffset = new Vector2(-55, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2);

            Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + extraOffset, null, Color.White, Projectile.rotation - MathHelper.PiOver4, origin, Projectile.scale * 0.95f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

        }

        public void spearTop()
        {
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Caverns/ThunderLance/ThunderLanceTipGlow");
            Texture2D Star = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/flare_1");
            Texture2D Tip = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Caverns/ThunderLance/ThunderLanceTip");

            Vector2 tipGlowOffset = new Vector2(0, 10f).RotatedBy(Projectile.rotation);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 extraOffset = new Vector2(-55, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2);
            Main.spriteBatch.Draw(Tip, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + extraOffset, null, Color.SkyBlue * 1f * (extraVFXIntensity - 1), Projectile.rotation - MathHelper.PiOver4, Tip.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);


            Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition + tipGlowOffset, null, Color.DeepSkyBlue * 0.8f * (extraVFXIntensity - 1), Projectile.rotation - MathHelper.PiOver4, Glow.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            
            Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + tipGlowOffset + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 14, null, Color.DeepSkyBlue * 1f * (extraVFXIntensity - 1), Projectile.rotation - MathHelper.PiOver4 + (vfxTimer * 0.15f), Star.Size() / 2, Projectile.scale * 0.27f * (1 - extraVFXIntensity), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + tipGlowOffset + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 14, null, Color.White * 1f * (extraVFXIntensity - 1), Projectile.rotation - MathHelper.PiOver4 + (vfxTimer * -0.15f), Star.Size() / 2, Projectile.scale * 0.2f * (1 - extraVFXIntensity), SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            if (extraVFXIntensity > 1)
                Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition + tipGlowOffset, null, Color.SkyBlue * 0.3f * extraVFXIntensity, Projectile.rotation - MathHelper.PiOver4, Glow.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


        }
        #endregion

        public int hitPause = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = 0.5f, Pitch = 0.77f, MaxInstances = 2, PitchVariance = 0.3f };
            SoundEngine.PlaySound(style2, target.Center);

            if (thrusting)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 direction = Projectile.velocity.RotateRandom(0.8f);
                    Vector2 ai = PolarVector(10, direction.ToRotation());
                    float ai2 = Main.rand.Next(100);

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity * 10, PolarVector(10, direction.ToRotation()) * 0.5f, ModContent.ProjectileType<LightningHitFX>(), 0, 0, Main.myPlayer, ai.ToRotation(), ai2);

                }

                for (int i = 0; i < 7; i++)
                {

                    Dust a = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowStrong>(),
                        Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-3, 3)) * Main.rand.NextFloat(2, 4),
                        newColor: Color.DodgerBlue, Scale: Main.rand.NextFloat(0.35f, 0.42f));

                    a.fadeIn = 2;
                }

            }

            hitPause = 5;
        }

        public static Vector2 PolarVector(float radius, float theta) => new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;

    }

    /*
    public class ThunderLanceSpin : ModProjectile
    {

    }
    */
}