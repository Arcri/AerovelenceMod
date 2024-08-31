using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Graphics;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Utilities;
using Microsoft.CodeAnalysis;

namespace AerovelenceMod.Content.NPCs.Bosses.FeatheredFoe
{
    public class OrbitingFeather : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        int timer = 0;
        public int advancer = 0;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.ignoreWater = false;
            Projectile.hostile = true;
            Projectile.friendly = false;

            Projectile.tileCollide = true;
            Projectile.timeLeft = 1800;
            Projectile.scale = 1.15f;
        }

        public float orbitVal = 500f;
        Vector2 storedPlayerCenter = Vector2.Zero;

        public float rotGoal = 0f;
        float dashVal = 0f;

        float pulseIntensity = 0f;
        float progress = 0f;

        float alpha = 0f;

        public Vector2 orbitVector = new Vector2(500f, 0f);
        public float rotSpeed = 1f;
        public int timeToOrbit = 130;
        public float spinOutVel = 10f;
        public override void AI()
        {
            Player targetPlayer = Main.player[Main.myPlayer];
            
            //Orbit around player
            if (advancer == 0)
            {
                //Initialize Lists
                if (timer == 0)
                {
                    previousRotations = new List<float>();
                    previousPostions = new List<Vector2>();
                    storedPlayerCenter = targetPlayer.Center;
                }

                //Lerp position to target Vector
                float lerpToPointProg = Math.Clamp(timer / 80f, 0f, 1f);
                Projectile.Center = Vector2.Lerp(Projectile.Center, storedPlayerCenter + orbitVector, lerpToPointProg);


                //float sinMult = (MathF.Sin(timer / 60f * rotSpeed) / 10f) + 1;
                orbitVector = orbitVector.RotatedBy(0.03f * rotSpeed) * 1f;

                float rotProg = Math.Clamp(timer / 25f, 0f, 1f);
                float rot = MathHelper.Lerp(orbitVector.ToRotation(), orbitVector.ToRotation() + MathHelper.PiOver2 * (rotSpeed > 0 ? 1f : -1), Easings.easeInSine(rotProg));
                Projectile.rotation = rot;
                //if (lerpToPointProg < 0.75f)
                  //  Projectile.rotation = ((targetPlayer.Center + orbitVector) - Projectile.Center).ToRotation();
                //else
                  //  Projectile.rotation = orbitVector.ToRotation() + MathHelper.PiOver2 * (rotSpeed > 0 ? 1f : -1);

                if (timer == timeToOrbit)
                {
                    //SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/trident_twirl_01") with { Pitch = .75f, PitchVariance = 0.2f, MaxInstances = -1, Volume = 0.25f }; //0.2f
                    //SoundEngine.PlaySound(style, Projectile.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_66") with { Pitch = .60f, MaxInstances = -1, Volume = 0.35f, PitchVariance = 0.2f }; 
                    SoundEngine.PlaySound(style2, Projectile.Center);

                    //SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/glaive_shot_01") with { Volume = .35f, Pitch = .75f, PitchVariance = 0.25f, MaxInstances = -1 }; 
                    //SoundEngine.PlaySound(style3, Projectile.Center);


                    Projectile.velocity = orbitVector.SafeNormalize(Vector2.UnitX) * 10f;

                    for (int i = 0; i < 4 + Main.rand.Next(0, 3); i++)
                    {
                        Vector2 randomStart = Main.rand.NextVector2Circular(2f, 2f) * 1f;
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), Projectile.velocity * 0.5f + randomStart, newColor: new Color(40, 125, 255), Scale: Main.rand.NextFloat(0.35f, 0.65f));
                        //dust.velocity += Projectile.velocity * 0.25f;

                        dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                            rotPower: 0.15f, preSlowPower: 0.95f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 3f, fadePower: 0.88f, shouldFadeColor: false);
                    }

                    advancer++;
                    timer = -1;
                }
            }

            //Spin out and aim at player
            else if (advancer == 1)
            {
                //Fade velocity
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * spinOutVel;
                spinOutVel *= 0.93f;

                rotGoal = (targetPlayer.Center - Projectile.Center).ToRotation();
                Projectile.rotation = MathHelper.Lerp(rotGoal + (MathF.PI * 12), rotGoal, Easings.easeOutCirc(timer / 45f));

                if (timer == 45)
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_ogre_spit") with { Pitch = 1f, PitchVariance = 0.33f, MaxInstances = -1 }; //0.33
                    SoundEngine.PlaySound(style, Projectile.Center);

                    SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_ballista_tower_shot_1") with { Pitch = .54f, PitchVariance = 0.2f, Volume = 0.3f, MaxInstances = -1 }; //0.2
                    SoundEngine.PlaySound(style3, Projectile.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_42") with { Pitch = .2f, PitchVariance = 0.2f, Volume = 0.55f, MaxInstances = -1 }; //0.2f
                    SoundEngine.PlaySound(style2, Projectile.Center);

                    Projectile.rotation = rotGoal;

                    pulseIntensity = 1f;

                    timer = -1;
                    advancer++;
                }
            }
            
            // Dash 
            else if (advancer == 2)
            {
                float prog = Math.Clamp(timer / 20f, 0f, 1f);
                dashVal = MathHelper.Lerp(0f, 21f, Easings.easeOutQuad(prog)); //Math.Clamp(MathHelper.Lerp(dashVal, 20f, 0.1f), 0f, 19f);

                Projectile.velocity = Projectile.rotation.ToRotationVector2() * dashVal;

                if (timer == 30 && false)
                {
                    for (int i = 0; i < 4; i++) 
                    {
                        Projectile.NewProjectile(null, Projectile.Center, new Vector2(21f, 0f).RotatedBy(MathHelper.PiOver2 * i), ModContent.ProjectileType<StraightFeather>(),
                            Projectile.damage, 2f, Main.myPlayer);
                    }
                }
            }
            
            int trailCount = 10;
            previousRotations.Add(Projectile.rotation);
            previousPostions.Add(Projectile.Center);

            if (previousRotations.Count > trailCount)
                previousRotations.RemoveAt(0);

            if (previousPostions.Count > trailCount)
                previousPostions.RemoveAt(0);

            pulseIntensity = Math.Clamp(MathHelper.Lerp(pulseIntensity, -0.25f, 0.03f), 0f, 2f);
            alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.25f, 0.03f), 0f, 1f);

            timer++;
        }

        public List<float> previousRotations;
        public List<Vector2> previousPostions;

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer <= 0 && advancer == 0) return false;
            Texture2D Feather = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/FeatheredFoe/Assets/Feather").Value;
            Texture2D FeatherGray = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/FeatheredFoe/Assets/FeatherGray").Value;
            Texture2D FeatherWhite = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/FeatheredFoe/Assets/FeatherWhite").Value;

            Texture2D Twirl = Mod.Assets.Request<Texture2D>("Assets/PixelSwirl").Value;

            #region after image
            if (previousRotations != null && previousPostions != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;

                    float size = (0.75f + (progress * 0.25f)) * Projectile.scale;


                    Color col = Color.Lerp(Color.Blue, Color.DeepSkyBlue, progress) * progress;

                    float size2 = (1f + (progress * 0.25f)) * Projectile.scale;
                    Main.EntitySpriteDraw(FeatherGray, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 0.55f * alpha,
                            previousRotations[i], FeatherGray.Size() / 2f, size2, SpriteEffects.None);

                    Vector2 vec2Scale = new Vector2(1.5f, 0.25f) * size;
                    if (advancer != 1)
                        Main.EntitySpriteDraw(FeatherWhite, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 0.85f * alpha,
                            previousRotations[i], FeatherGray.Size() / 2f, vec2Scale, SpriteEffects.None);
                }

            }
            #endregion

            float twirlAlpha = 1f - Easings.easeOutCirc((float)(timer / 40f));

            if (advancer == 1)
                Main.EntitySpriteDraw(Twirl, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * twirlAlpha * 1.5f, Projectile.rotation, Twirl.Size() / 2f, Projectile.scale * 0.65f, SpriteEffects.None);

            Color outerCol = Color.Lerp(Color.DeepSkyBlue * 0.5f, Color.SkyBlue with { A = 0 } * 0.8f, pulseIntensity);
            float scale = MathHelper.Lerp(1f, 1.25f, pulseIntensity);
            for (int i = 0; i < 3; i++)
            {
                Main.EntitySpriteDraw(FeatherWhite, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(2f, 2f), null, outerCol * alpha, Projectile.rotation, Feather.Size() / 2f, Projectile.scale * 1.05f * scale, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(Feather, Projectile.Center - Main.screenPosition, null, lightColor * alpha, Projectile.rotation, Feather.Size() / 2f, Projectile.scale, SpriteEffects.None);

            Main.EntitySpriteDraw(Feather, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 0.4f * alpha, Projectile.rotation, Feather.Size() / 2f, Projectile.scale * 1f, SpriteEffects.None);


            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 randomStart = Main.rand.NextVector2Circular(1.5f, 1.5f) * 1f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: new Color(40, 125, 255), Scale: Main.rand.NextFloat(0.35f, 0.45f));
                dust.velocity += Projectile.velocity * 0.25f;

                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    rotPower: 0.15f, preSlowPower: 0.99f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.88f, shouldFadeColor: false);
            }

            base.OnKill(timeLeft);
        }
    }

    public class StraightFeather : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        int timer = 0;
        public int advancer = 0;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.ignoreWater = false;
            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.tileCollide = true;
            Projectile.timeLeft = 170;

        }

        public float accelTime = 40f;

        float alpha = 0f;
        float dashVal = 0f;
        float pulseIntensity = 0f;

        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.ai[0] = Projectile.velocity.Length();
                previousRotations = new List<float>();
                previousPostions = new List<Vector2>();

                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_ogre_spit") with { Pitch = 1f, PitchVariance = .33f, MaxInstances = -1 };
                SoundEngine.PlaySound(style, Projectile.Center);

                SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_ballista_tower_shot_1") with { Pitch = .54f, PitchVariance = 0.2f, Volume = 0.3f, MaxInstances = -1 };
                SoundEngine.PlaySound(style3, Projectile.Center);

                SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_42") with { Pitch = .2f, PitchVariance = .2f, Volume = 0.55f, MaxInstances = -1 };
                SoundEngine.PlaySound(style2, Projectile.Center);

                Main.NewText(Projectile.velocity.ToRotation());

                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 1f;
                Projectile.rotation = Projectile.velocity.ToRotation();

                pulseIntensity = 1f;
            }

            if (timer > 0)
            {
                float prog = Math.Clamp(timer / accelTime, 0f, 1f);
                dashVal = MathHelper.Lerp(0f, Projectile.ai[0], Easings.easeInExpo(prog));

                Projectile.velocity = Projectile.rotation.ToRotationVector2() * dashVal;

                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            int trailCount = 10;
            previousRotations.Add(Projectile.rotation);
            previousPostions.Add(Projectile.Center);

            if (previousRotations.Count > trailCount)
                previousRotations.RemoveAt(0);

            if (previousPostions.Count > trailCount)
                previousPostions.RemoveAt(0);

            pulseIntensity = Math.Clamp(MathHelper.Lerp(pulseIntensity, -0.25f, 0.03f), 0f, 2f);
            alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.25f, 0.03f), 0f, 1f);

            timer++;
        }

        public List<float> previousRotations;
        public List<Vector2> previousPostions;

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer <= 0) return false;
            Texture2D Feather = Mod.Assets.Request<Texture2D>("Content/Projectiles/TempVFX/Feather").Value;
            Texture2D FeatherGray = Mod.Assets.Request<Texture2D>("Content/Projectiles/TempVFX/FeatherGray").Value;
            Texture2D FeatherWhite = Mod.Assets.Request<Texture2D>("Content/Projectiles/TempVFX/FeatherWhite").Value;

            #region after image
            if (previousRotations != null && previousPostions != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;

                    float size = (0.75f + (progress * 0.25f)) * Projectile.scale;


                    Color col = Color.Lerp(Color.Blue, Color.DeepSkyBlue, progress) * progress;

                    float size2 = (1f + (progress * 0.25f)) * Projectile.scale;
                    Main.EntitySpriteDraw(FeatherGray, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 0.55f * alpha,
                            previousRotations[i], FeatherGray.Size() / 2f, size2, SpriteEffects.None);

                    Vector2 vec2Scale = new Vector2(1.5f, 0.25f) * size;
                    
                    Main.EntitySpriteDraw(FeatherWhite, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 0.85f * alpha,
                            previousRotations[i], FeatherGray.Size() / 2f, vec2Scale, SpriteEffects.None);
                }

            }
            #endregion

            Color outerCol = Color.Lerp(Color.DeepSkyBlue * 0.5f, Color.SkyBlue with { A = 0 } * 0.8f, pulseIntensity);
            float scale = MathHelper.Lerp(1f, 1.25f, pulseIntensity);
            for (int i = 0; i < 3; i++)
            {
                Main.EntitySpriteDraw(FeatherWhite, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(2f, 2f), null, outerCol * alpha, Projectile.rotation, Feather.Size() / 2f, Projectile.scale * 1.05f * scale, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(Feather, Projectile.Center - Main.screenPosition, null, lightColor * alpha, Projectile.rotation, Feather.Size() / 2f, Projectile.scale, SpriteEffects.None);

            Main.EntitySpriteDraw(Feather, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 0.4f * alpha, Projectile.rotation, Feather.Size() / 2f, Projectile.scale * 1f, SpriteEffects.None);


            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 randomStart = Main.rand.NextVector2Circular(1.5f, 1.5f) * 1f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: new Color(40, 125, 255), Scale: Main.rand.NextFloat(0.35f, 0.45f));
                dust.velocity += Projectile.velocity * 0.25f;

                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    rotPower: 0.15f, preSlowPower: 0.99f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.88f, shouldFadeColor: false);
            }

            base.OnKill(timeLeft);
        }
    }

    public class CurvingFeather : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        int timer = 0;
        public int advancer = 0;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.ignoreWater = false;
            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.tileCollide = true;
            Projectile.timeLeft = 170;

        }

        public float accelTime = 15f;

        float alpha = 0f;
        float dashVal = 0f;
        float pulseIntensity = 0f;

        public float curveValue = 0f;

        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.ai[0] = Projectile.velocity.Length();
                previousRotations = new List<float>();
                previousPostions = new List<Vector2>();

                //SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_ogre_spit") with { Pitch = 1f, PitchVariance = .33f, MaxInstances = -1 };
                //SoundEngine.PlaySound(style, Projectile.Center);

                //SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_ballista_tower_shot_1") with { Pitch = .54f, PitchVariance = 0.2f, Volume = 0.3f, MaxInstances = -1 };
                //SoundEngine.PlaySound(style3, Projectile.Center);

                SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_42") with { Pitch = .2f, PitchVariance = .2f, Volume = 0.55f, MaxInstances = -1 };
                SoundEngine.PlaySound(style2, Projectile.Center);

                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 1f;
                Projectile.rotation = Projectile.velocity.ToRotation();

                pulseIntensity = 1f;
            }

            if (timer > 0)
            {
                float prog = Math.Clamp(timer / accelTime, 0f, 1f);
                dashVal = MathHelper.Lerp(0f, Projectile.ai[0], Easings.easeInExpo(prog));

                //Projectile.velocity = Projectile.rotation.ToRotationVector2() * dashVal;

                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            int trailCount = 10;
            previousRotations.Add(Projectile.rotation);
            previousPostions.Add(Projectile.Center);

            if (previousRotations.Count > trailCount)
                previousRotations.RemoveAt(0);

            if (previousPostions.Count > trailCount)
                previousPostions.RemoveAt(0);


            if (timer < 90)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(-curveValue);

                if (timer < 50)
                    Projectile.velocity *= 1.05f;
            }


            pulseIntensity = Math.Clamp(MathHelper.Lerp(pulseIntensity, -0.25f, 0.03f), 0f, 2f);
            alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.25f, 0.03f), 0f, 1f);

            timer++;
        }

        public List<float> previousRotations;
        public List<Vector2> previousPostions;

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer <= 0) return false;
            Texture2D Feather = Mod.Assets.Request<Texture2D>("Content/Projectiles/TempVFX/Feather").Value;
            Texture2D FeatherGray = Mod.Assets.Request<Texture2D>("Content/Projectiles/TempVFX/FeatherGray").Value;
            Texture2D FeatherWhite = Mod.Assets.Request<Texture2D>("Content/Projectiles/TempVFX/FeatherWhite").Value;

            #region after image
            if (previousRotations != null && previousPostions != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;

                    float size = (0.75f + (progress * 0.25f)) * Projectile.scale;


                    Color col = Color.Lerp(Color.Blue, Color.DeepSkyBlue, progress) * progress;

                    float size2 = (1f + (progress * 0.25f)) * Projectile.scale;
                    Main.EntitySpriteDraw(FeatherGray, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 0.55f * alpha,
                            previousRotations[i], FeatherGray.Size() / 2f, size2, SpriteEffects.None);

                    Vector2 vec2Scale = new Vector2(1.5f, 0.25f) * size;

                    Main.EntitySpriteDraw(FeatherWhite, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 0.85f * alpha,
                            previousRotations[i], FeatherGray.Size() / 2f, vec2Scale, SpriteEffects.None);
                }

            }
            #endregion

            Color outerCol = Color.Lerp(Color.DeepSkyBlue * 0.5f, Color.SkyBlue with { A = 0 } * 0.8f, pulseIntensity);
            float scale = MathHelper.Lerp(1f, 1.25f, pulseIntensity);
            for (int i = 0; i < 3; i++)
            {
                Main.EntitySpriteDraw(FeatherWhite, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(2f, 2f), null, outerCol * alpha, Projectile.rotation, Feather.Size() / 2f, Projectile.scale * 1.05f * scale, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(Feather, Projectile.Center - Main.screenPosition, null, lightColor * alpha, Projectile.rotation, Feather.Size() / 2f, Projectile.scale, SpriteEffects.None);

            Main.EntitySpriteDraw(Feather, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 0.4f * alpha, Projectile.rotation, Feather.Size() / 2f, Projectile.scale * 1f, SpriteEffects.None);


            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 randomStart = Main.rand.NextVector2Circular(1.5f, 1.5f) * 1f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: new Color(40, 125, 255), Scale: Main.rand.NextFloat(0.35f, 0.45f));
                dust.velocity += Projectile.velocity * 0.25f;

                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    rotPower: 0.15f, preSlowPower: 0.99f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.88f, shouldFadeColor: false);
            }

            base.OnKill(timeLeft);
        }
    }


    public class StopAndStartFeather : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.ignoreWater = false;
            Projectile.hostile = true;
            Projectile.friendly = false;

            Projectile.tileCollide = true;
            Projectile.timeLeft = 170;

        }

        int timer = 0;
        float alpha = 0f;
        float pulseIntensity = 0f;

        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.ai[0] = Projectile.velocity.Length();
                previousRotations = new List<float>();
                previousPostions = new List<Vector2>();

                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_ogre_spit") with { Pitch = 1f, PitchVariance = .33f, MaxInstances = -1 };
                SoundEngine.PlaySound(style, Projectile.Center);

                SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_ballista_tower_shot_1") with { Pitch = .54f, PitchVariance = 0.2f, Volume = 0.3f, MaxInstances = -1 };
                SoundEngine.PlaySound(style3, Projectile.Center);

                SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_42") with { Pitch = .2f, PitchVariance = .2f, Volume = 0.55f, MaxInstances = -1 };
                SoundEngine.PlaySound(style2, Projectile.Center);

                Projectile.rotation = Projectile.velocity.ToRotation();

                pulseIntensity = 1f;
            }

            if (timer <= 35)
                Projectile.velocity *= 0.94f;
            else if (timer < 60)
                Projectile.velocity *= 1.13f;
            
            int trailCount = 10;
            previousRotations.Add(Projectile.rotation);
            previousPostions.Add(Projectile.Center);

            if (previousRotations.Count > trailCount)
                previousRotations.RemoveAt(0);

            if (previousPostions.Count > trailCount)
                previousPostions.RemoveAt(0);

            pulseIntensity = Math.Clamp(MathHelper.Lerp(pulseIntensity, -0.25f, 0.03f), 0f, 2f);
            alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.25f, 0.03f), 0f, 1f);

            timer++;
        }

        public List<float> previousRotations;
        public List<Vector2> previousPostions;

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer <= 0) return false;
            Texture2D Feather = Mod.Assets.Request<Texture2D>("Content/Projectiles/TempVFX/Feather").Value;
            Texture2D FeatherGray = Mod.Assets.Request<Texture2D>("Content/Projectiles/TempVFX/FeatherGray").Value;
            Texture2D FeatherWhite = Mod.Assets.Request<Texture2D>("Content/Projectiles/TempVFX/FeatherWhite").Value;

            Vector2 vec2MainScale = new Vector2(1f, 0.25f + (alpha * 0.75f)) * Projectile.scale;

            #region after image
            if (previousRotations != null && previousPostions != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;

                    float size = (0.75f + (progress * 0.25f)) * Projectile.scale;


                    Color col = Color.Lerp(Color.Blue, Color.DeepSkyBlue, progress) * progress;

                    float size2 = (1f + (progress * 0.25f)) * Projectile.scale;
                    Main.EntitySpriteDraw(FeatherGray, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 0.55f * alpha,
                            previousRotations[i], FeatherGray.Size() / 2f, size2, SpriteEffects.None);

                    Vector2 vec2Scale = new Vector2(1.5f, 0.25f) * size;

                    Main.EntitySpriteDraw(FeatherWhite, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 0.85f * alpha,
                            previousRotations[i], FeatherGray.Size() / 2f, vec2Scale, SpriteEffects.None);
                }

            }
            #endregion

            Color outerCol = Color.Lerp(Color.DeepSkyBlue * 0.5f, Color.SkyBlue with { A = 0 } * 0.8f, pulseIntensity);
            float scale = MathHelper.Lerp(1f, 1.25f, pulseIntensity);
            for (int i = 0; i < 3; i++)
            {
                Main.EntitySpriteDraw(FeatherWhite, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(2f, 2f), null, outerCol * alpha, Projectile.rotation, Feather.Size() / 2f, vec2MainScale * 1.05f * scale, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(Feather, Projectile.Center - Main.screenPosition, null, lightColor * alpha, Projectile.rotation, Feather.Size() / 2f, vec2MainScale, SpriteEffects.None);

            Main.EntitySpriteDraw(Feather, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 0.4f * alpha, Projectile.rotation, Feather.Size() / 2f, vec2MainScale * 1f, SpriteEffects.None);


            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 randomStart = Main.rand.NextVector2Circular(1.5f, 1.5f) * 1f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: new Color(40, 125, 255), Scale: Main.rand.NextFloat(0.35f, 0.45f));
                dust.velocity += Projectile.velocity * 0.25f;

                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    rotPower: 0.15f, preSlowPower: 0.99f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.88f, shouldFadeColor: false);
            }

            base.OnKill(timeLeft);
        }
    }
}