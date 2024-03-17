using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using System;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.GameContent;
using Terraria.Audio;
using AerovelenceMod.Content.Projectiles.Other;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{   
    public class CyverLaserBomb : ModProjectile
    {
        float whiteIntensity = 0f;
        int timer = 0;

        public bool telegraphLong = true;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laser Bomb");
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 200;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override bool? CanDamage()
        {
            return false;
        }

        public bool longTelegraph = false;

        public float overallScale = 0f;
        public override void AI()
        {
            if (timer > 32)
            {
                teleScale = MathHelper.Clamp(MathHelper.Lerp(teleScale, 1.9f, 0.05f), 0f, 1.75f);
            }

            if (timer == 50) //50
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = 1.5f, PitchVariance = .47f, MaxInstances = 1, Volume = 0.25f };
                SoundEngine.PlaySound(style, Projectile.Center);

                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .08f, Pitch = .4f, PitchVariance = .2f, MaxInstances = 1 };
                SoundEngine.PlaySound(style2, Projectile.Center);

                Vector2 offset = new Vector2(24f, 0f).RotatedBy(Projectile.rotation);

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + offset,
                    Projectile.rotation.ToRotationVector2() * -0.1f, ModContent.ProjectileType<CyverBeam>(),
                    Projectile.damage, Projectile.knockBack);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + offset * -1,
                    Projectile.rotation.ToRotationVector2() * 0.1f, ModContent.ProjectileType<CyverBeam>(),
                    Projectile.damage, Projectile.knockBack);

                //whiteIntensity = 1;

                /*
                int a = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HollowPulse>(), 0, 0, Main.myPlayer);
                if (Main.projectile[a].ModProjectile is HollowPulse pulse)
                {
                    pulse.color = Color.Pink;
                    pulse.oval = false;
                    pulse.size = 0.5f;
                    
                }
                */
            }

            if (timer >= 65)
            {
                drawAlpha -= 0.08f;
                Projectile.ai[0] += 0.03f;
                ///drawAlpha = MathHelper.Clamp(MathHelper.Lerp(drawAlpha, -0.2f, 0.1f), 0f, 1f);
                //overallScale = Math.Clamp(MathHelper.Lerp(overallScale, 5f, 0.2f), 0f, 1f);

                if (drawAlpha == 0)
                    Projectile.active = false;
            }
            else if (timer > 50)
            {
                drawAlpha -= 0.075f;
                Projectile.ai[0] = Math.Clamp(MathHelper.Lerp(0f, 1f, Easings.easeOutCirc((timer - 50f) / 20f)), 0f, 1f);
                overallScale -= 0.02f;


                if (drawAlpha == 0)
                    Projectile.active = false;
            }
            else
            {
                overallScale = Math.Clamp(MathHelper.Lerp(overallScale, 1.15f, 0.175f), 0f, 1f);
                Projectile.ai[1] = Math.Clamp(MathHelper.Lerp(Projectile.ai[1], 1.15f, 0.2f), 0f, 1f);
                drawAlpha = Math.Clamp(MathHelper.Lerp(drawAlpha, 1.15f, 0.175f), 0f, 1f);
            }

            whiteIntensity = Math.Clamp(MathHelper.Lerp(whiteIntensity, -0.25f, 0.04f), 0, 1);
            timer++;
        }

        float teleScale = 0f;
        float drawAlpha = 0f;
        float colorIntensity = 2f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/PinkL").Value;
            Texture2D White = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/WhiteL").Value;
            Texture2D RayTex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/Medusa_Gray").Value;

            float extraAngle = MathHelper.Lerp(MathF.PI * 0.25f, 2f * MathF.PI, Projectile.ai[1]);

            float L1rot = (Projectile.rotation + extraAngle) - MathHelper.ToRadians(45);
            float L2rot = (Projectile.rotation + extraAngle) - MathHelper.ToRadians(225);

            Vector2 extraLPos1 = new Vector2((50f * Projectile.ai[0]), 20 * (1 - overallScale) + (1f * Projectile.ai[0]));
            Vector2 extraLPos2 = new Vector2((-50f * Projectile.ai[0]), -20 * (1 - overallScale) - (1f * Projectile.ai[0]));
            Vector2 L1pos = new Vector2(extraLPos1.X, 10 + extraLPos1.Y).RotatedBy(Projectile.rotation + extraAngle) + Projectile.Center;
            Vector2 L2pos = new Vector2(extraLPos2.X, -10 + extraLPos2.Y).RotatedBy(Projectile.rotation + extraAngle) + Projectile.Center;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * 3);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.4f); //0.8
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();
            if (timer < 50)
            {
                Vector2 vec2Scale = new Vector2(teleScale, teleScale * 0.5f);

                if (longTelegraph)
                    vec2Scale = new Vector2(teleScale * 2.65f, teleScale * 0.6f); //2.5 0.6

                Vector2 origin = new Vector2(0f, RayTex.Height / 2);

                Main.spriteBatch.Draw(RayTex, Projectile.Center - Main.screenPosition, null, Color.DeepPink * drawAlpha * 1f, Projectile.rotation + MathHelper.Pi, origin, vec2Scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(RayTex, Projectile.Center - Main.screenPosition, null, Color.DeepPink * drawAlpha * 1f, Projectile.rotation, origin, vec2Scale, SpriteEffects.None, 0);

            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            if (telegraphLong)
            {
                Vector2 vec2Scale = new Vector2(1.35f, 0.15f) * (2 + (teleScale * 0.6f));
                Vector2 offsetAdd = Projectile.rotation.ToRotationVector2() * 10;

                Main.spriteBatch.Draw(RayTex, Projectile.Center - Main.screenPosition + offsetAdd, null, Color.DeepPink with { A = 0 } * drawAlpha * 0.3f, Projectile.rotation, new Vector2(0, RayTex.Height / 2), vec2Scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(RayTex, Projectile.Center - Main.screenPosition + offsetAdd, null, Color.DeepPink with { A = 0 } * drawAlpha * 0.3f, Projectile.rotation + MathHelper.Pi, new Vector2(0, RayTex.Height / 2), vec2Scale, SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(RayTex, Projectile.Center - Main.screenPosition + offsetAdd, null, Color.White with { A = 0 } * 0.45f * drawAlpha, Projectile.rotation, new Vector2(0, RayTex.Height / 2), vec2Scale * 0.75f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(RayTex, Projectile.Center - Main.screenPosition + offsetAdd, null, Color.White with { A = 0 } * 0.45f * drawAlpha, Projectile.rotation + MathHelper.Pi, new Vector2(0, RayTex.Height / 2), vec2Scale * 0.75f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(Tex, L1pos - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White with { A = 0 } * 0.75f * drawAlpha, L1rot, Tex.Size() / 2, overallScale * 0.9f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, L2pos - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White with { A = 0 } * 0.75f * drawAlpha, L2rot, Tex.Size() / 2, overallScale * 0.9f, SpriteEffects.None, 0f);
            
            Main.spriteBatch.Draw(White, L1pos - Main.screenPosition, White.Frame(1, 1, 0, 0), Color.White * 0.75f * drawAlpha, L1rot, White.Size() / 2, overallScale * 0.9f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(White, L2pos - Main.screenPosition, White.Frame(1, 1, 0, 0), Color.White * 0.75f * drawAlpha, L2rot, White.Size() / 2, overallScale * 0.9f, SpriteEffects.None, 0f);

            return false;
        }
    }
    public class CyverBeam : ModProjectile
    {
        public float luminos = 0.8f;
        public Vector2 endPoint;
        public float LaserRotation = (float)Math.PI / 2f;
        int timer = 0;
        int secondTimer = 0;

        float extraAngle = 0;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cyver Beam");
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.scale = 2.5f;
            Projectile.timeLeft = 50;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (timer == 0)
            {
                LaserRotation = Projectile.velocity.ToRotation();
            }
            Projectile.velocity = Vector2.Zero;
            additional = Math.Clamp(MathHelper.Lerp(additional, 120 * Projectile.scale, 0.04f), 0, 50 * Projectile.scale);

            timer++;
        }

        float additional = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            if (timer > 0)
            {
                endPoint = LaserRotation.ToRotationVector2() * 2000f;

                //Star
                Texture2D star = Mod.Assets.Request<Texture2D>("Assets/TrailImages/CrispStarPMA").Value;
                Main.spriteBatch.Draw(star, Projectile.Center + Vector2.UnitX.RotatedBy(LaserRotation) * 24 - Main.screenPosition, null, Color.HotPink with { A = 0 }, LaserRotation, star.Size() / 2f, (50f * Projectile.scale - additional) * 0.01f, 0, 0);
                Main.spriteBatch.Draw(star, Projectile.Center + Vector2.UnitX.RotatedBy(LaserRotation) * 24 - Main.screenPosition, null, Color.White with { A = 0 }, LaserRotation, star.Size() / 2f, (50f * Projectile.scale - additional) * 0.005f, 0, 0);

                //Beam
                Texture2D texBeam = Mod.Assets.Request<Texture2D>("Assets/GlowTrailMoreRes").Value;
                Vector2 origin2 = new Vector2(0, texBeam.Height / 2);
                float height = (50f * Projectile.scale) - additional; //15
                //height *= 4f;
                if (height == 0)
                    Projectile.active = false;

                int width = (int)(Projectile.Center - endPoint).Length() - 24;

                var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
                var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));


                Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/CheapScroll", AssetRequestMode.ImmediateLoad).Value;
                #region Shader Params
                myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/EnergyTex").Value);
                myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);

                Color c1 = new Color(255, 20, 125);//Color.DeepPink;
                Color c2 = new Color(255, 20, 125);//Color.DeepPink;

                myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
                myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
                myEffect.Parameters["Color1Mult"].SetValue(1f);
                myEffect.Parameters["Color2Mult"].SetValue(1f);
                myEffect.Parameters["totalMult"].SetValue(0.75f);

                myEffect.Parameters["tex1reps"].SetValue(10f);
                myEffect.Parameters["tex2reps"].SetValue(10f);
                myEffect.Parameters["satPower"].SetValue(0.8f);
                myEffect.Parameters["time1Mult"].SetValue(1f);
                myEffect.Parameters["time2Mult"].SetValue(1f);
                myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.018f);
                #endregion

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
                myEffect.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(texBeam, target, null, Color.HotPink, LaserRotation, origin2, 0, 0);
                Main.spriteBatch.Draw(texBeam, target, null, Color.HotPink, LaserRotation, origin2, 0, 0);
                Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);

            }
            secondTimer++;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (timer > 7) return false;

            Vector2 unit = LaserRotation.ToRotationVector2();
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + unit * 1000, 22, ref point);
        }

        public void setExtraAngle(float input)
        {
            extraAngle = input;
        }
    }
    public class NewCyverBombBeam : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }

        Vector2 center = Vector2.Zero;
        Vector2 end1 = Vector2.Zero;
        Vector2 end2 = Vector2.Zero;


        public float luminos = 0.8f;
        public Vector2 endPoint;
        public float LaserRotation = (float)Math.PI / 2f;
        int timer = 0;

        public float width = 1f;
        public float scale = 1f;
        float progress = 0f;

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 50;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (timer == 0)
            {
                LaserRotation = Projectile.velocity.ToRotation();
            }
            Projectile.velocity = Vector2.Zero;
            additional = Math.Clamp(MathHelper.Lerp(additional, 120 * Projectile.scale, 0.04f), 0, 50 * Projectile.scale);

            timer++;
        }

        float additional = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            if (timer > 0)
            {
                endPoint = LaserRotation.ToRotationVector2() * 2000f;
                var texBeam = Mod.Assets.Request<Texture2D>("Assets/GlowTrailMoreRes").Value;

                Vector2 origin2 = new Vector2(0, texBeam.Height / 2);

                float height = (50f * Projectile.scale) - additional; //15

                if (height == 0)
                    Projectile.active = false;

                int width = (int)(Projectile.Center - endPoint).Length() - 24;

                var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
                var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));

                Main.spriteBatch.Draw(texBeam, target, null, Color.HotPink, LaserRotation, origin2, 0, 0);
                Main.spriteBatch.Draw(texBeam, target, null, Color.HotPink, LaserRotation, origin2, 0, 0);
                Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);

            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (timer > 7) return false;

            Vector2 unit = LaserRotation.ToRotationVector2();
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + unit * 1000, 22, ref point);
        }
    }
} 