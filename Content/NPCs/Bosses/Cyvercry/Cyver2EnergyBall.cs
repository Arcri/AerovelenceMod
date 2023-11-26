using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using System;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.GameContent;
using Terraria.Audio;
using AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry;
using AerovelenceMod.Content.Projectiles;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{
    public class Cyver2EnergyBall : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;
        public float curveAmount = 1f;

        public override void SetStaticDefaults()
        {
           
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.damage = 80;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool? CanDamage()
        {
            if (timer < 25)
                return false;
            return true;
        }

        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();
        public override void AI()
        {
            if (timer == 0)
                Projectile.rotation = Projectile.velocity.ToRotation();


            Player player = Main.player[(int)Projectile.ai[0]];

            if (player.active)
            {
                float curvePower = (timer < 60 ? 0.012f : 0.009f) * 0.75f;

                Projectile.rotation = Projectile.rotation.AngleTowards((player.Center - Projectile.Center).ToRotation(), curvePower); //(0.001f * timer) + 0.01f
                Projectile.velocity = Projectile.velocity.Length() * Projectile.rotation.ToRotationVector2();
                Projectile.velocity *= 1.008f;
            }

            if (timer < 0)
                Projectile.velocity *= 0.2f;

            //if (timer == 220)
                //Projectile.active = false;

            float scale = 2f;

            //Trail1 Info Dump
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/RealLightningBloom").Value;
            trail1.trailColor = Color.HotPink;
            trail1.trailPointLimit = (int)(125 * scale);
            trail1.trailWidth = (int)(20 * scale);
            trail1.trailMaxLength = (int)(225 * scale); //225
            trail1.timesToDraw = 3;
            trail1.pinch = false;
            trail1.pinchAmount = 0.2f;

            trail1.gradient = false;
            trail1.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/CyverGrad").Value;
            trail1.shouldScrollColor = true;
            trail1.gradientTime = 0.0f;

            trail1.trailTime = (float)timer * 0.025f;
            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center + Projectile.velocity;
            if (timer > 0) trail1.TrailLogic();

            //Trail2 Info Dump
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value;
            trail2.trailColor = Color.DeepPink;
            trail2.trailPointLimit = (int)(125 * scale);
            trail2.trailWidth = (int)(10 * scale);
            trail2.trailMaxLength = (int)(225 * scale); //255
            trail2.timesToDraw = 2;
            trail2.pinch = false;
            trail2.pinchAmount = 0.2f;

            trail2.gradient = false;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/CyverGrad").Value;
            trail2.shouldScrollColor = true;
            trail2.gradientTime = 0.0f;

            trail2.trailTime = (float)timer * 0.015f;
            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center + Projectile.velocity;
            if (timer > 0) trail2.TrailLogic();

            #region trail
            /*
            //Trail1 Info Dump
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/RealLightning").Value;
            trail1.trailColor = Color.White;
            trail1.trailPointLimit = (int)(125 * scale);
            trail1.trailWidth = (int)(15 * scale);
            trail1.trailMaxLength = (int)(125 * scale);
            trail1.timesToDraw = 2;
            trail1.pinch = true;
            trail1.pinchAmount = 0.01f;

            trail1.gradient = true;
            trail1.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/CyverGrad2").Value;
            trail1.shouldScrollColor = true;
            ///trail1.gradientTime = timer * 0.01f;

            trail1.trailTime = timer * 0.05f;
            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center + Projectile.velocity;
            if (timer > 0) trail1.TrailLogic();

            //Trail2 Info Dump
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/FireEdge").Value;
            trail2.trailColor = Color.White;
            trail2.trailPointLimit = (int)(250 * scale);
            trail2.trailWidth = (int)(8 * scale);
            trail2.trailMaxLength = (int)(250 * scale);
            trail2.timesToDraw = 3;
            trail2.pinch = true;
            trail2.pinchAmount = 0.01f;

            trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/CyverGrad2").Value;
            trail2.shouldScrollColor = true;
            ///trail2.gradientTime = timer * 0.01f;

            //trail2.trailTime = timer * 0.04f;
            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center + Projectile.velocity;
            if (timer > 0) trail2.TrailLogic();
            */
            #endregion

            timer++;
        }
        public override void Kill(int timeLeft)
        {
            //SoundEngine.PlaySound(SoundID.Item94 with { Pitch = 0.4f, Volume = 0.45f, PitchVariance = 0.2f }, Projectile.Center);
            //Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PinkExplosion>(), 0, 0, Main.myPlayer);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);

            Texture2D spike = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/Spike").Value;
            Texture2D star = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/FireSpike").Value;

            float spikeRot = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Vector2 spikeScale = new Vector2(0.85f, 1f) * 0.35f;
            Main.spriteBatch.Draw(spike, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f), null, Color.HotPink with { A = 0 } * 1f, spikeRot, spike.Size() / 2, spikeScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(spike, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), null, Color.HotPink with { A = 0 } * 1f, spikeRot, spike.Size() / 2, spikeScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(spike, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 1f, spikeRot, spike.Size() / 2, spikeScale * 0.5f, SpriteEffects.None, 0f);
            
            //Main.spriteBatch.Draw(glow4, Projectile.Center - Main.screenPosition, null, Color.HotPink with { A = 0 } * 0.65f, Projectile.velocity.ToRotation() + rot - MathHelper.ToRadians(110), glow4.Size() / 2, 0.2f * scale, SpriteEffects.None, 0f);


            return false;

            float scale = 2f;
            float rot = (float)Main.timeForVisualEffects * 0.2f;

            Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_1").Value;
            Texture2D glow2 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/FireSpike").Value;

            //Texture2D glow3 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/FireSpike").Value;

            Texture2D glow3 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_12").Value;
            Texture2D glow4 = Mod.Assets.Request<Texture2D>("Assets/TrailImages/TerraOrbC").Value;

            //Main.spriteBatch.Draw(glow4, Projectile.Center - Main.screenPosition, null, Color.DeepPink with { A = 0 } * 0.35f, Projectile.velocity.ToRotation() + rot, glow4.Size() / 2, 0.3f * scale, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(glow4, Projectile.Center - Main.screenPosition, null, Color.DeepPink with { A = 0 } * 0.35f, Projectile.velocity.ToRotation() + rot + MathHelper.ToRadians(120), glow4.Size() / 2, 0.3f * scale, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(glow4, Projectile.Center - Main.screenPosition, null, Color.DeepPink with { A = 0 } * 0.35f, Projectile.velocity.ToRotation() + rot - MathHelper.ToRadians(120), glow4.Size() / 2, 0.3f * scale, SpriteEffects.None, 0f);


            Main.spriteBatch.Draw(glow4, Projectile.Center - Main.screenPosition, null, Color.HotPink with { A = 0 } * 0.65f, Projectile.velocity.ToRotation() + rot, glow4.Size() / 2, 0.2f * scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow4, Projectile.Center - Main.screenPosition, null, Color.HotPink with { A = 0 } * 0.65f, Projectile.velocity.ToRotation() + rot + MathHelper.ToRadians(110), glow4.Size() / 2, 0.2f * scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow4, Projectile.Center - Main.screenPosition, null, Color.HotPink with { A = 0 } * 0.65f, Projectile.velocity.ToRotation() + rot - MathHelper.ToRadians(110), glow4.Size() / 2, 0.2f * scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(glow3, Projectile.Center - Main.screenPosition, null, Color.DeepPink  * 0.25f, Projectile.velocity.ToRotation(), glow3.Size() / 2, 0.5f * scale * 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow3, Projectile.Center - Main.screenPosition, null, Color.DeepPink , Projectile.velocity.ToRotation() + (MathF.PI / 2f), glow3.Size() / 2, 0.25f * scale * 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow3, Projectile.Center - Main.screenPosition, null, Color.White , Projectile.velocity.ToRotation() + (MathF.PI / 2f), glow3.Size() / 2, 0.25f * scale * 0.5f, SpriteEffects.None, 0f);
            
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            //Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(2f, 2f), null, Color.HotPink with { A = 0 }, Projectile.rotation - MathHelper.PiOver2, glow.Size() / 2, 0.2f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), null, Color.White with { A = 0 }, Projectile.rotation - MathHelper.PiOver2, glow.Size() / 2, 0.1f, SpriteEffects.None, 0f);



            return false;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(glow2, Projectile.Center - Main.screenPosition, null, Color.HotPink  * 0.25f, Projectile.velocity.ToRotation() + rot, glow2.Size() / 2, 0.4f * scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White , Projectile.velocity.ToRotation() + MathHelper.PiOver2, glow.Size() / 2, 0.2f * scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(glow2, Projectile.Center - Main.screenPosition, null, Color.HotPink  * 0.25f, Projectile.velocity.ToRotation() - rot + MathHelper.PiOver2, glow2.Size() / 2, 0.4f * scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White , Projectile.velocity.ToRotation() + MathHelper.PiOver2, glow.Size() / 2, 0.2f * scale , SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
            /*
            Vector2 bonus = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 5f;
            //Draw the Circlular Glow
            var Tex = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + bonus, Tex.Frame(1, 1, 0, 0), Color.DeepPink * 0.5f, Projectile.rotation, Tex.Size() / 2, 0.91f, SpriteEffects.None, 0f);


            var BallTexture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/EnergyBall").Value;

            int frameHeight = BallTexture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, BallTexture.Width, frameHeight);


            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.spriteBatch.Draw(BallTexture, Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, sourceRectangle, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
            return false;
            */
        }

    }

    public class CurveDashShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.damage = 80;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 3;
        }

        public override bool? CanDamage()
        {
            return true;
        }

        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();

        float direction = 0f;
        public float curvePower = 1f;
        public override void AI()
        {
            Player player = Main.player[(int)Projectile.ai[0]];

            if (timer == 0)
            {
                direction = (player.Center - Projectile.Center).ToRotation();
                Projectile.rotation = Projectile.velocity.ToRotation();
            }


            if (player.active)
            {
                Projectile.rotation = Projectile.rotation.AngleTowards((player.Center - Projectile.Center).ToRotation(), 0.02f); //(0.001f * timer) + 0.01f
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * Projectile.velocity.Length();// * 2f;
            }

            float scale = 2f;

            #region trail
            //Trail1 Info Dump
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/EnergyTex").Value;
            trail1.trailColor = Color.White * 1f;
            trail1.trailPointLimit = 800;
            trail1.trailWidth = 60;
            trail1.trailMaxLength = 800;
            trail1.timesToDraw = 1;
            trail1.pinch = true;
            trail1.pinchAmount = 0.55f;


            trail1.trailTime = timer * 0.01f;
            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center;
            trail1.TrailLogic();

            //Trail2 Info Dump
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value;
            trail2.trailColor = Color.Wheat;
            trail2.trailPointLimit = 800;
            trail2.trailWidth = 120;
            trail2.trailMaxLength = 800;
            trail2.timesToDraw = 2;
            trail2.pinch = true;
            trail2.pinchAmount = 0.55f;

            trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/CyverGrad2").Value;
            trail2.shouldScrollColor = true;
            trail2.gradientTime = timer * 0.01f;

            trail2.trailTime = timer * 0.02f;
            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center;
            trail2.TrailLogic();
            #endregion

            timer++;
        }
        public override void Kill(int timeLeft)
        {
            //SoundEngine.PlaySound(SoundID.Item94 with { Pitch = 0.4f, Volume = 0.45f, PitchVariance = 0.2f }, Projectile.Center);
            //Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PinkExplosion>(), 0, 0, Main.myPlayer);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);

            
            return false;

        }

    }
} 