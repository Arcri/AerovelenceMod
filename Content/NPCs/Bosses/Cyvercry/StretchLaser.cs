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
using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Content.Projectiles;
using System.Collections.Generic;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{   
    public class StretchLaser : ModProjectile
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 9000;
        }
        public int enemiesHit = 0;
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.scale = 1f;
            Projectile.timeLeft = 200;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = -1;

            Projectile.friendly = false;
            Projectile.hostile = true;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

        }
        public int accelerateTime = 65;
        public float accelerateStrength = 1.038f;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            if (timer < accelerateTime)
                Projectile.velocity *= accelerateStrength;

            Projectile.ai[0] = Math.Clamp(MathHelper.Lerp(Projectile.ai[0], Projectile.scale + 0.2f, 0.12f), 0, Projectile.scale); 
            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer == 0)
                return false;

            Color newPink = new Color(255, 50, 140);
            Color pinkToUse = new Color(255, 25, 155);

            Vector2 vscale = new Vector2(0.5f, Projectile.velocity.Length() * 0.15f) * Projectile.ai[0] * 0.95f;
            Vector2 vscale2 = new Vector2(0.2f, Projectile.velocity.Length() * 0.15f) * Projectile.ai[0] * 0.95f;
            Vector2 vscale3 = new Vector2(0.4f, Projectile.velocity.Length() * 0.3f) * Projectile.ai[0] * 0.95f;

            //Texture2D softGlow = Mod.Assets.Request<Texture2D>("Assets/DiamondGlow").Value;
            Texture2D softGlow = Mod.Assets.Request<Texture2D>("Assets/TrailImages/DiamondGlowPMA").Value;
            //Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulseShot").Value;
            Texture2D Tex = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/GlowDartBlack").Value;


            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition, softGlow.Frame(1, 1, 0, 0), Color.Black * 0.25f, Projectile.rotation, softGlow.Size() / 2, vscale3 * 0.85f, SpriteEffects.None, 0f);

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX)), null, newPink with { A = 0 } * 0.8f, Projectile.rotation, Tex.Size() / 2, vscale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition, null, pinkToUse with { A = 0 } * 0.75f, Projectile.rotation, softGlow.Size() / 2, vscale3, SpriteEffects.None, 0f);

            //Set up glowy shader 
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * 2.5f);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.28f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

            //Activate Shader
            myEffect.CurrentTechnique.Passes["Glow"].Apply();

            

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX), null, Color.LightPink with { A = 0 } * 0.85f, Projectile.rotation, Tex.Size() / 2, vscale2, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX), null, Color.LightPink with { A = 0 } * 0.35f, Projectile.rotation, Tex.Size() / 2, vscale2, SpriteEffects.None, 0f);

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            /*
            //Draw the Circlular Glow
            var softGlow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulseShot").Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX)), Tex.Frame(1, 1, 0, 0), Color.DeepPink, Projectile.rotation, Tex.Size() / 2, new Vector2(0.5f, Projectile.velocity.Length() * 0.15f), SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition , softGlow.Frame(1, 1, 0, 0), Color.Pink, Projectile.rotation, softGlow.Size() / 2, new Vector2(0.5f, Projectile.velocity.Length() * 0.15f), SpriteEffects.None, 0f);

            //Set up Shader
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * 2.2f); //2.5f makes it more spear like
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.28f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            //0.2f
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX), Tex.Frame(1, 1, 0, 0), Color.Crimson, Projectile.rotation, Tex.Size() / 2, new Vector2(0.25f, Projectile.velocity.Length() * 0.15f), SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            */

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //return base.Colliding(projHitbox, targetHitbox);
            Player Player = Main.player[Projectile.owner];
            Vector2 unit = Projectile.rotation.ToRotationVector2();
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + Projectile.velocity * 30, 10, ref point);
        }
    }

    public class FocusedLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cyver Laser");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 78;
        }
        public float LaserRotation = 0;
        public float laserWidth = 70;
        int timer = 0;
        public float direction = 0;
        public Vector2 endPoint;
        public int parentIndex = 0;

        public bool spinDir = false;

        public override void AI()
        {
            NPC parent = Main.npc[parentIndex];

            if (!parent.active)
            {
                Projectile.active = false;
            }

            direction = parent.rotation;
            Projectile.Center = parent.Center + (parent.rotation.ToRotationVector2() * -96);
            endPoint = Projectile.Center + (parent.rotation.ToRotationVector2() * -1200);
            LaserRotation = direction + MathHelper.Pi;
            
            if (timer > 20)
            {
                for (int i = 0; i < 1; i++)
                {
                    //float randomDustPercent = Main.rand.NextFloat();
                    //Vector2 dustPercentPoint = (endPoint - Projectile.Center) * randomDustPercent;
                    //Vector2 dustVel = (LaserRotation + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.5f) * Main.rand.NextFloat(2f, 6f) * 2f;

                    //Dust.NewDustPerfect(Projectile.Center + dustPercentPoint, ModContent.DustType<GlowPixelCross>(), dustVel, 0, Color.DeepPink, 0.2f);

                    float randomDustPercent = Main.rand.NextFloat();
                    Vector2 dustPercentPoint = (endPoint - Projectile.Center) * randomDustPercent;
                    Vector2 dustVel = (LaserRotation + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.1f) * Main.rand.NextFloat(1.1f, 9.1f);

                    Dust.NewDustPerfect(Projectile.Center + dustPercentPoint, ModContent.DustType<MuraLineBasic>(), dustVel, 20, Color.DeepPink, Main.rand.NextFloat(0.18f, 0.3f));

                }
            }


            if (Projectile.timeLeft == 1)
            {
                for (int i = 0; i < 50; i++)
                {
                    float randomDustPercent = Main.rand.NextFloat();
                    Vector2 dustPercentPoint = (endPoint - Projectile.Center) * randomDustPercent;
                    Vector2 dustVel = (LaserRotation + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.15f) * Main.rand.NextFloat(1.1f, 6.1f) * 1.25f;
                    //new Color(255,155,190)
                    Dust.NewDustPerfect(Projectile.Center + dustPercentPoint, ModContent.DustType<GlowStrong>(), dustVel, 2, Color.DeepPink, Main.rand.NextFloat(0.45f, 0.65f) * 0.75f);

                }
            }

            //Dust.NewDustPerfect(Projectile.Center, DustID.AmberBolt);
            //Dust.NewDustPerfect(endPoint, DustID.AmberBolt);

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D LaserTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/FlashLightBeam").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/RimeLaser", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3() * 0.4f);
            myEffect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail5Loop").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value);
            myEffect.Parameters["uTime"].SetValue(timer * -0.01f); //0.006
            myEffect.Parameters["uSaturation"].SetValue(2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);

            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            Vector2 origin2 = new Vector2(0, LaserTexture.Height / 2);

            float height = (laserWidth * 1.8f); //25

            int width = (int)(Projectile.Center - endPoint).Length();

            var pos = Projectile.Center - Main.screenPosition;
            var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.7f));

            Main.spriteBatch.Draw(LaserTexture, target, null, Color.White, LaserRotation, origin2, 0, 0);
            Main.spriteBatch.Draw(LaserTexture, target, null, Color.White, LaserRotation, origin2, 0, 0);

            //Main.spriteBatch.Draw(texture, target, null, Color.White, LaserRotation, origin2, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            var target2 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.8f));
            var target3 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));

            Main.spriteBatch.Draw(LaserTexture, target2, null, Color.DeepPink * 0.5f, LaserRotation, origin2, 0, 0);
            Main.spriteBatch.Draw(LaserTexture, target3, null, Color.DeepPink * 0.25f, LaserRotation, origin2, 0, 0);

            //Flares
            Texture2D flare1 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_1").Value;
            Texture2D flare12 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_16").Value;

            Main.spriteBatch.Draw(flare12, Projectile.Center - Main.screenPosition, flare12.Frame(1, 1, 0, 0), Color.HotPink, timer * 0.08f, flare12.Size() / 2, 0.4f * laserWidth * 0.02f, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(flare12, Projectile.Center - Main.screenPosition, flare12.Frame(1, 1, 0, 0), Color.Pink, timer * -0.05f, flare12.Size() / 2, 0.3f * laserWidth * 0.02f, SpriteEffects.None, 0.0f);

            Main.spriteBatch.Draw(flare1, endPoint - Main.screenPosition, flare1.Frame(1, 1, 0, 0), Color.HotPink, timer * 0.07f, flare1.Size() / 2, 0.01f * laserWidth, SpriteEffects.None, 0.0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = LaserRotation.ToRotationVector2();
            float point = 0f;

            if (laserWidth > 5)
            {
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                    endPoint, 30, ref point);
            }

            return false;
        }
    }

    public class SplittingLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cyver Laser");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 2;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 90; //125
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

        }
        int timer = 0;
        public override void AI()
        {
            
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            if (timer < 20) //20
                Projectile.velocity *= 1.088f;

            //FTW | GFB timer % 5
            if (timer > 9 && timer % 9 == 0) // > 10 % 10
            {
                int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CyverLaserBomb>(), 20, 0);
                Main.projectile[a].rotation = Projectile.rotation;

                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                for (int i = 0; i < 360; i += 20)
                {
                    Vector2 circular = new Vector2(40, 0).RotatedBy(MathHelper.ToRadians(i));
                    Dust b = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + circular, ModContent.DustType<GlowCircleDust>(), Vector2.Zero, Color.DeepPink, 0.3f, 0.6f, 0f, dustShader);
                }
            }


            if (timer % 2 == 0 && Main.rand.NextBool())
            {
                //My old shader dust system is yucky and stupid so replace this when i redo it

                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                int gd = GlowDustHelper.DrawGlowDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowCircleFlare>(),
                    Color.DeepPink, 0.4f + Main.rand.NextFloat(0.15f), 0.55f, 0f, dustShader2);

                Main.dust[gd].velocity += Projectile.velocity.RotateRandom(0.05f);

            }

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer == 0)
                return false;

            Color pinkToUse = new Color(255, 25, 155);

            var softGlow = Mod.Assets.Request<Texture2D>("Assets/DiamondGlow").Value;
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulseShot").Value;

            Vector2 scale1 = new Vector2(0.56f, Projectile.velocity.Length() * 0.1f) * 0.85f;
            Vector2 scale2 = new Vector2(0.3f, Projectile.velocity.Length() * 0.1f) * 0.85f;
            Vector2 scale3 = new Vector2(0.66f, Projectile.velocity.Length() * 0.3f) * 0.85f;

            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition, softGlow.Frame(1, 1, 0, 0), Color.Black * 0.25f, Projectile.rotation, softGlow.Size() / 2, scale3 * 0.85f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX)), Tex.Frame(1, 1, 0, 0), Color.DeepPink, Projectile.rotation, Tex.Size() / 2, scale1, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition, softGlow.Frame(1, 1, 0, 0), pinkToUse * 0.75f, Projectile.rotation, softGlow.Size() / 2, scale3, SpriteEffects.None, 0f);

            //Set up Shader
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * 2.4f); //2.5f makes it more spear like
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.28f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            //0.2f
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX), Tex.Frame(1, 1, 0, 0), Color.Crimson, Projectile.rotation, Tex.Size() / 2, scale2, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);



            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //return base.Colliding(projHitbox, targetHitbox);
            Player Player = Main.player[Projectile.owner];
            Vector2 unit = Projectile.rotation.ToRotationVector2();
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + Projectile.velocity * 30, 10, ref point);
        }
    }

    public class EyeSword : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1000;
            Projectile.extraUpdates = 0;
        }
        public float LaserRotation = 0;
        public float laserWidth = 70;
        int timer = 0;
        public float direction = 0;
        public Vector2 endPoint;
        public int parentIndex = 0;

        Vector2 startCenter = Vector2.Zero;

        public float initialBurst = 1f;


        public float alpha = 1f;
        public float progress = 0f;

        public bool dash = false;
        public bool spinDir = false;
        public bool fade = false;
        public bool start = false;

        public List<float> previousRotations = new List<float>();

        public float easeProg = 0f;
        public float currentAngle = 0f;
        public float centerAngle = 0f;
        public bool drawSlash = false;

        int fadeTimeLeft = 0;

        BaseTrailInfo Trail = new BaseTrailInfo();
        public override void AI()
        {
            if (timer == 0)
            {
                easeProg = 0.03f;
                startCenter = Projectile.Center;
                Projectile.ai[2] = 0;
                fadeTimeLeft = 5;
            }

            if (fade)
            {
                alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.25f, 0.2f), 0f, 1f);
                if (fadeTimeLeft == 0)
                    Projectile.active = false;
                fadeTimeLeft--;
            }
            else
                alpha = Math.Clamp(MathHelper.Lerp(alpha, 0f, 0.08f), 1f, 1f);

            previousRotations.Add(Projectile.rotation);
            if (previousRotations.Count > 10)
            {
                previousRotations.RemoveAt(0);
            }


            timer++;
        }


        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            if (timer <= 1)
                return false;

            Trail.TrailDrawing(Main.spriteBatch);

            Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/MuzzleFlashes/circle_053").Value;
            Texture2D glow2 = Mod.Assets.Request<Texture2D>("Assets/MuzzleFlashes/muzzle_flash_12").Value;
            Texture2D glow3 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/spotlight_8").Value;
            Texture2D glow4 = Mod.Assets.Request<Texture2D>("Assets/MuzzleFlashes/EasyLightray").Value;

            float ySinVal = (float)Math.Sin(Main.timeForVisualEffects * 0.22f) * 0.15f;
            float xSinVal = (float)Math.Sin(Main.timeForVisualEffects * 0.22f) * 0.05f;

            //re-name these 
            Vector2 newScale = new Vector2(1.5f, 1f) * 0.5f; //sword
            Vector2 newScale2 = new Vector2(0.75f, 1.3f + ySinVal) * (0.5f + xSinVal); //spiky
            Vector2 newScale3 = new Vector2(0.25f, 0.25f); //Hilt
            Vector2 newScale4 = new Vector2(1f, 0.5f) * 1f; //After Image

            newScale *= 0.75f;// + (0.5f * MathF.Sin(MathF.PI * progress));
            newScale2 *= 0.75f;// + (0.5f * MathF.Sin(MathF.PI * progress));
            newScale3 *= 0.75f;
            newScale4 *= 0.75f;

            Vector2 origin1 = new Vector2(0f, glow.Height / 2f);
            Vector2 origin2 = new Vector2(0f, glow2.Height / 2f);
            Vector2 origin4 = new Vector2(0f, glow4.Height / 2f);

            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/ComboLaser", AssetRequestMode.ImmediateLoad).Value;

            //Black Base
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition - new Vector2(0f, 0f), null, Color.Black * 0.35f, Projectile.rotation, origin1, newScale, SpriteEffects.None, 0f);

            ShaderParams();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            //MainBlade
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + new Vector2(-60f, 0f).RotatedBy(Projectile.rotation), null, Color.White, Projectile.rotation, origin1, newScale, SpriteEffects.None, 0f);

            //Spiky part near guard
            Main.spriteBatch.Draw(glow2, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), null, Color.White, Projectile.rotation, origin2, newScale2, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow2, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), null, Color.White, Projectile.rotation, origin2, newScale2 * 0.5f, SpriteEffects.FlipVertically, 0f);

            //"Hilt"
            Vector2 off = Projectile.rotation.ToRotationVector2() * 8f;
            Main.spriteBatch.Draw(glow3, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(4f, 4f) + off, null, Color.White, Projectile.rotation + MathHelper.PiOver2, glow3.Size() / 2, newScale3, SpriteEffects.FlipVertically, 0f);
            Main.spriteBatch.Draw(glow3, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(4f, 4f) + off, null, Color.White, Projectile.rotation - MathHelper.PiOver2, glow3.Size() / 2, newScale3, SpriteEffects.FlipVertically, 0f);

            Texture2D slash = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/KennySlashHalfBig").Value;
            float slashOpacity = MathF.Pow(MathF.Sin(MathF.PI * progress), 2);
            Vector2 slashScale = new Vector2(1f, 2f) * 1.5f * slashOpacity * 0.75f;
            Main.spriteBatch.Draw(slash, Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * (130f * 0.75f), null, Color.Lerp(Color.DeepPink, Color.HotPink, 0.35f) * slashOpacity, Projectile.rotation, slash.Size() / 2f, slashScale, SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(slash, Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * (130f * 0.75f), null, Color.Lerp(Color.DeepPink, Color.HotPink, 0.35f) * slashOpacity, Projectile.rotation, slash.Size() / 2f, slashScale, SpriteEffects.FlipHorizontally, 0f);


            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            for (int i = 0; i < 0; i++)
            {
                float listProgress = (float)i / previousRotations.Count;
                Main.spriteBatch.Draw(glow4, Projectile.Center - Main.screenPosition + new Vector2(-60f, 0f).RotatedBy(previousRotations[i]), null, Color.HotPink * listProgress * MathF.Sin(MathF.PI * progress) * 2f, previousRotations[i], origin4, newScale4, SpriteEffects.None, 0f);

            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            
            


            //pixelKennySlashBlack

            //draw


            return false;
        }

        public void ShaderParams()
        {
            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail5Loop").Value);
            myEffect.Parameters["sampleTexture3"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value);
            myEffect.Parameters["sampleTexture4"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/ThinGlowLine").Value);

            Color c1 = Color.DeepPink;
            Color c2 = Color.DeepPink;
            Color c3 = Color.DeepPink;
            Color c4 = Color.DeepPink;

            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color3"].SetValue(c3.ToVector4());
            myEffect.Parameters["Color4"].SetValue(c4.ToVector4());

            myEffect.Parameters["Color1Mult"].SetValue(1.5f);
            myEffect.Parameters["Color2Mult"].SetValue(1.5f);
            myEffect.Parameters["Color3Mult"].SetValue(1.5f);
            myEffect.Parameters["Color4Mult"].SetValue(1.1f);
            myEffect.Parameters["totalMult"].SetValue(1f * alpha);

            myEffect.Parameters["tex1reps"].SetValue(1f);
            myEffect.Parameters["tex2reps"].SetValue(1f);
            myEffect.Parameters["tex3reps"].SetValue(1f);
            myEffect.Parameters["tex4reps"].SetValue(1f);

            myEffect.Parameters["satPower"].SetValue(1f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.03f);
            
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float range = 325f; //345
            
            Vector2 unit = Projectile.rotation.ToRotationVector2();
            float point = 0f;
            float bonusWidth = MathF.Pow(MathF.Sin(MathF.PI * progress), 3) * 250;

            if (laserWidth > 5)
            {
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                    Projectile.Center + unit * range, 80 + bonusWidth , ref point);
            }

            return false;
        }
    }

}