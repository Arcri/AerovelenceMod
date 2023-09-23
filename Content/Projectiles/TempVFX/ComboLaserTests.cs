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

namespace AerovelenceMod.Content.Projectiles.TempVFX
{

    public class ComboLaserTest : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        int mainTimer = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 400;
        }

        float startAngle = 0f;
        float endAngle = 0f;

        int timer = 0;
        public override void AI()
        {

            timer++;
        }

        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/ThinLineGlowClear").Value;
            Texture2D black = Mod.Assets.Request<Texture2D>("Assets/ThinGlowLine").Value;

            Vector2 newScale = new Vector2(11f, 3.3f) * 0.3f;
            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/ComboLaser", AssetRequestMode.ImmediateLoad).Value;

            //Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition - new Vector2(0f, 0f), null, Color.Black, Projectile.rotation, glow.Size() / 2, newScale, SpriteEffects.None, 0f);

            ShaderParams();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition - new Vector2(0f, 0f), null, Color.White, Projectile.rotation, glow.Size() / 2, newScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition - new Vector2(0f, 0f), null, Color.White, Projectile.rotation, glow.Size() / 2, newScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition - new Vector2(0f, 0f), null, Color.White, Projectile.rotation, glow.Size() / 2, newScale, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition - new Vector2(0f, 0f), null, Color.White, Projectile.rotation, glow.Size() / 2, newScale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public void ShaderParams()
        {
            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail5").Value);
            myEffect.Parameters["sampleTexture3"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value);
            myEffect.Parameters["sampleTexture4"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/ThinGlowLine").Value);

            Color c1 = new Color(255, 96, 40, 255);
            Color c2 = new Color(240, 79, 40, 255);
            Color c3 = new Color(255, 173, 40, 255);
            Color c4 = new Color(255, 149, 40, 255);

            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color3"].SetValue(c3.ToVector4());
            myEffect.Parameters["Color4"].SetValue(c4.ToVector4());

            myEffect.Parameters["Color1Mult"].SetValue(1.5f);
            myEffect.Parameters["Color2Mult"].SetValue(1.5f);
            myEffect.Parameters["Color3Mult"].SetValue(1.5f);
            myEffect.Parameters["Color4Mult"].SetValue(1.5f);
            myEffect.Parameters["totalMult"].SetValue(1.15f);

            myEffect.Parameters["tex1reps"].SetValue(1f);
            myEffect.Parameters["tex2reps"].SetValue(1f);
            myEffect.Parameters["tex3reps"].SetValue(1f);
            myEffect.Parameters["tex4reps"].SetValue(1f);

            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.03f);

        }
    }

    public class ComboVertexTest : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 7500;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
        }


        float startAngle = 0f;
        float endAngle = 0f;
        int timer = 0;
        float width = 1f;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            timer++;
        }

        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/MuzzleFlashes/circle_053").Value;
            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/ComboLaserVertex", AssetRequestMode.ImmediateLoad).Value;

            int val = timer * 6;

            Vector2[] positions = new Vector2[val];
            float[] rotations = new float[val];
            for (int i = 0; i < val; i++)
            {
                positions[i] = Projectile.Center + new Vector2((float)val * (i / (float)val), 0).RotatedBy(Projectile.rotation + (i * 0.0025f));
                rotations[i] = Projectile.rotation + (i * 0.0025f);
            }

            VertexStrip strip = new VertexStrip();
            strip.PrepareStripWithProceduralPadding(positions, rotations, StripColor, StripWidth, -Main.screenPosition, true);

            ShaderParams();
            myEffect.CurrentTechnique.Passes["DefaultPass"].Apply();
            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            
            strip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            return false;
        }

        public void ShaderParams()
        {
            myEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);

            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail5").Value);
            myEffect.Parameters["sampleTexture3"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value);
            myEffect.Parameters["sampleTexture4"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/ThinGlowLine").Value);

            Color c1 = new Color(255f, 96f, 0f);
            Color c2 = new Color(240f, 79f, 0f);
            Color c3 = new Color(255f, 173f, 0f);
            Color c4 = new Color(255, 149f, 0f);

            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color3"].SetValue(c3.ToVector4());
            myEffect.Parameters["Color4"].SetValue(c4.ToVector4());

            myEffect.Parameters["Color1Mult"].SetValue(1f);
            myEffect.Parameters["Color2Mult"].SetValue(1f);
            myEffect.Parameters["Color3Mult"].SetValue(1f);
            myEffect.Parameters["Color4Mult"].SetValue(2f);
            myEffect.Parameters["totalMult"].SetValue(1.15f);

            myEffect.Parameters["tex1reps"].SetValue(1f);
            myEffect.Parameters["tex2reps"].SetValue(1f);
            myEffect.Parameters["tex3reps"].SetValue(1f);
            myEffect.Parameters["tex4reps"].SetValue(1f);

            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.03f);

        }

        public Color StripColor(float progress)
        {
            Color color = Color.DarkOrange;
            //color.A = 0;
            return color;
        }
        public float StripWidth(float progress)
        {
            float size = Utils.GetLerpValue(160, 140, Projectile.timeLeft, true) * Utils.GetLerpValue(0, 32, Projectile.timeLeft, true);
            float start = Math.Clamp(1.5f * (float)Math.Pow(progress, 0.5f), 0f, 1f);
            float cap = (float)Math.Cbrt(Utils.GetLerpValue(1f, 0.9f, progress, true));
            return start * size * 100 * cap;// * (1.1f + (float)Math.Cos(timer) * (0.08f - progress * 0.06f));
            
        }
    }

}