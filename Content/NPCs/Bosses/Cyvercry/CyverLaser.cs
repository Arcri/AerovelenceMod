﻿using Microsoft.Xna.Framework;
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

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{   
    public class CyverLaser : ModProjectile
    {
        public int timer = 0;
        public int damageDelay = 0;
        public int tileCollideDelay = 45;

        public bool accelerate = false;
        public float accelerateAmount = 1.02f;
        public float accelerateTime = 65f;

        public bool teleAhead = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cyver Laser");

        }
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 2;
            Projectile.width = 7;
            Projectile.height = 7;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 400;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;

        }
        public override void AI()
        {
            //Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() * 0.5f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;

            if (timer <= tileCollideDelay)
                Projectile.tileCollide = false;
            else
                Projectile.tileCollide = true;

            if (accelerate && timer < accelerateTime)
                Projectile.velocity *= accelerateAmount;

            damageDelay--;
            timer++;

        }

        

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer == 0)
                return false;

            var Tex = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Starlight").Value;
            var Tex2 = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/CyverLaserPMA").Value;

            float colorIntensity = (damageDelay >= 0 ? 0.25f : 1f);

            Color pinkToUse = Color.Lerp(Color.HotPink, Color.DeepPink, 0.1f);


            Main.spriteBatch.Draw(Tex2, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -30), Tex2.Frame(1, 1, 0, 0), pinkToUse with { A = 0 } * colorIntensity * 0.5f, Projectile.rotation, Tex2.Size() / 2, Projectile.scale * 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex2, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -30), Tex2.Frame(1, 1, 0, 0), pinkToUse with { A = 0 } * colorIntensity, Projectile.rotation, Tex2.Size() / 2, Projectile.scale * 0.15f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex2, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -30), Tex2.Frame(1, 1, 0, 0), Color.White with { A = 0 } * colorIntensity * 0.7f, Projectile.rotation, Tex2.Size() / 2, Projectile.scale * 0.1f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -30), Tex.Frame(1, 1, 0, 0), Color.HotPink with { A = 0 } * colorIntensity * 0.7f, Projectile.rotation, Tex.Size() / 2, new Vector2(2f, 0.25f) * Projectile.scale, SpriteEffects.None, 0f);

            return false;

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            //Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition , softGlow.Frame(1, 1, 0, 0), Color.OrangeRed, Projectile.rotation, softGlow.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            float intensity = (damageDelay >= 0 ? 1f : 2f);

            //Set up Shader
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3() * intensity); //2.5f makes it more spear like
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.2f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            //0.2f
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -30), Tex.Frame(1, 1, 0, 0), Color.Cyan, Projectile.rotation, Tex.Size() / 2, Projectile.scale * 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -30), Tex.Frame(1, 1, 0, 0), Color.Cyan, Projectile.rotation, Tex.Size() / 2, Projectile.scale * 0.2f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }


        public override void OnKill(int timeLeft)
        {
            if (Projectile.tileCollide == true)
            {
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                for (int i = 0; i < 5; i++)
                {
                    Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleDust>(),
                        (Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.Pi + Main.rand.NextFloat(-1, 1)) * Main.rand.Next(1, 3)),
                        Color.DeepPink, Main.rand.NextFloat(0.2f, 0.4f), 0.5f, 0f, dustShader);
                    p.alpha = 0;
                    //p.rotation = Main.rand.NextFloat(6.28f);
                }
            }
            else
            {

                for (int i = 0; i < Main.rand.Next(2, 4); i++)
                {
                    Dust p2 = Dust.NewDustPerfect(Projectile.Center, DustID.FireworkFountain_Pink,
                        Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(0.5f, 1.5f), 0,
                        Color.DeepPink, Main.rand.NextFloat(0.4f, 0.8f));
                    p2.noLight = true;
                    //p.rotation = Main.rand.NextFloat(6.28f);
                }
            }
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (timer < 1) return false;

            if (Projectile.timeLeft > 0 && Projectile.scale >= 1)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_10") with { Pitch = .92f, PitchVariance = .28f, MaxInstances = -1, Volume = 0.2f };
                SoundEngine.PlaySound(style, Projectile.Center);
            }
            return true;
        }

        public override bool? CanDamage()
        {
            if (damageDelay > 0) return false;
            return true;
        }
    }

    public class CyverPulse : ModProjectile
    {

        int timer = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("CyverPulse");

        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 1000;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 0.5f;

        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(6.28f);
                //SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);
            }

            Projectile.scale = MathHelper.Lerp(Projectile.scale, 10f, 0.2f); //1.51
            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/ShadowCyvercry_Outline").Value;


            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/FireBallShader", AssetRequestMode.ImmediateLoad).Value;
            
            myEffect.Parameters["caustics"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/GaussianStar").Value);
            myEffect.Parameters["distort"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/noise").Value);
            myEffect.Parameters["gradient"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/EnergyBalls/energyball_9").Value);
            myEffect.Parameters["uTime"].SetValue(timer * 0.08f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            int height1 = texture.Height;
            Vector2 origin1 = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
} 