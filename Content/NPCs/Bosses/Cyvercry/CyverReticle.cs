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
using System.Collections.Generic;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{   
    public class CyverReticle : ModProjectile
    {
        float scaleVal = 0;
        public int timer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Target");

        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 400;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override void AI()
        {
            Player player = Main.player[(int)Projectile.ai[0]];
            Projectile.Center = player.Center;
            Projectile.rotation = MathHelper.ToRadians(timer);
            if (timer < 200)
            {
                scaleVal = Math.Clamp(MathHelper.Lerp(scaleVal, 1.5f, 0.03f), 0f, 1f);
            }
            else
            {
                scaleVal = Math.Clamp(MathHelper.Lerp(scaleVal, -0.5f, 0.03f), 0f, 1f);

            }

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[(int)Projectile.ai[0]];
            //Projectile.Center = player.Center;

            //Draw the Circlular Glow
            //var softGlow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            var Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/RetHalfSolid").Value;
            var Tex2 = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/RetHalfGhost").Value;

            //var Danger = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/Danger4").Value;


            var Danger = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/Danger4").Value;
            var DangerGlow = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/Danger4Glow").Value;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(Tex2, Projectile.Center - Main.screenPosition + new Vector2(0, 35).RotatedBy(Projectile.rotation), Tex2.Frame(1, 1, 0, 0), Color.White, Projectile.rotation + MathHelper.Pi, Tex2.Size() / 2, scaleVal * 2, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex2, Projectile.Center - Main.screenPosition + new Vector2(0,-35).RotatedBy(Projectile.rotation), Tex2.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Tex2.Size() / 2, scaleVal * 2, SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(DangerGlow, Projectile.Center - Main.screenPosition, DangerGlow.Frame(1, 1, 0, 0), Color.White, 0f, DangerGlow.Size() / 2, scaleVal * 3f, SpriteEffects.None, 0f);


            //Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition, softGlow.Frame(1, 1, 0, 0), Color.OrangeRed, Projectile.rotation, softGlow.Size() / 2, 20f, SpriteEffects.None, 0f);
            /*
            //Set up Shader
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3() * 2f); //2.5f makes it more spear like
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.2f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            //0.2f
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.Cyan, Projectile.rotation, Tex.Size() / 2, scaleVal * 0.2f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.Cyan, Projectile.rotation, Tex.Size() / 2, scaleVal * 0.2f, SpriteEffects.None, 0f);
            */

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            
            //Main.spriteBatch.Draw(Danger, Projectile.Center - Main.screenPosition, Danger.Frame(1, 1, 0, 0), Color.White, 0f, Danger.Size() / 2, scaleVal * 3f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + new Vector2(0, 35).RotatedBy(Projectile.rotation), Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation + MathHelper.Pi, Tex.Size() / 2, scaleVal * 2, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + new Vector2(0, -35).RotatedBy(Projectile.rotation), Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Tex.Size() / 2, scaleVal * 2, SpriteEffects.None, 0f);



            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            /*
            var Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/CyverReticle").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.Cyan.ToVector3() * 2f); //2.5f makes it more spear like
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.2f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            //0.2f
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.Cyan, Projectile.rotation, Tex.Size() / 2, scaleVal * 2.6f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            */
        }

        public override bool? CanDamage()
        {
            return false;
        }
    }
} 