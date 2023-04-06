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

namespace AerovelenceMod.Content.Items.Weapons.Flares
{   
    public class ChloroFlare : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;
        public float vortexRot = 0;
        public float vortexRotsmall;
        public float FlareLerp = 0.3f;
        
        public float[] randomRotation = new float[5];
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

        }
        public override void AI()
        {

            Lighting.AddLight(Projectile.Center, Color.LawnGreen.ToVector3() * 1.5f);
            if (timer == 0)
            {
                for (int i = 0; i < randomRotation.Length; i++)
                {
                    randomRotation[i] = Main.rand.NextFloat(6.28f);
                }
            }

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            if (Projectile.velocity.X < 0)
            {
                vortexRot -= 0.06f;
                vortexRotsmall += 1;
                Projectile.rotation -= 0.08f;
            }
            else
            {
                vortexRot += 0.06f;
                vortexRotsmall -= 1;
                Projectile.rotation += 0.08f;

            }
            if (timer > 20) FlareLerp = Math.Clamp(FlareLerp - 0.015f, 0, 0.3f);//Math.Clamp(MathHelper.Lerp(FlareLerp, 0.1f, 0.02f), 0, 0.2f);

            
            if (timer % 7 == 0)
            {
                for (int i = 0; i < 1 + Main.rand.NextFloat(0, 1); i++)
                {
                    Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleRise>(),
                        new Vector2(0, -2) + Projectile.velocity * -0.2f + Main.rand.NextVector2Circular(1, 1), Color.LawnGreen, Main.rand.NextFloat(0.3f, 0.6f), 0.7f, 1.2f, dustShader);
                    //p.rotation = Main.rand.NextFloat(6.28f);
                }

            }
            

           

            if (timer % 4 == 0)
            {

                for (int i = 0; i < 1; i++)
                {
                    Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), ModContent.DustType<GlowCircleRise>(), 
                        new Vector2(0,-2) + Projectile.velocity * -0.2f + Main.rand.NextVector2Circular(3, 3), Color.Gray * 0.65f, Main.rand.NextFloat(0.5f, 0.9f), 1f, 0f, dustShader);
                }

            }
            
            Projectile.velocity.Y += 0.29f;

            //Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            //Projectile.spriteDirection = Projectile.direction;
            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {

            //Draw the Circlular Glow
            var softGlow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition, softGlow.Frame(1, 1, 0, 0), Color.LawnGreen, Projectile.rotation, softGlow.Size() / 2, 3.3f, SpriteEffects.None, 0f);
            
            var star = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;
            var star2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_05").Value;

            //Draw the 9 point star glow
            Main.spriteBatch.Draw(star2, Projectile.Center - Main.screenPosition, star2.Frame(1, 1, 0, 0), Color.LawnGreen * 0.7f, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * -2), star2.Size() / 2, 0.20f, SpriteEffects.None, 0f);


            //Set up Shader
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.LawnGreen.ToVector3() * 1.5f);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.6f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

            //Draw the "lense flare"
            if (timer > 1 && timer < 50)
            {
                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), Color.SkyBlue, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * 3 + 45), star.Size() / 2, FlareLerp, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), Color.SkyBlue, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * 3 + 45), star.Size() / 2, FlareLerp, SpriteEffects.None, 0f);
            }

            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            //Draw Flare Center
            var FlareFlare = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/flare_01").Value;
            Main.spriteBatch.Draw(FlareFlare, Projectile.Center - Main.screenPosition, FlareFlare.Frame(1, 1, 0, 0), Color.LawnGreen, (float)Math.PI, FlareFlare.Size() / 2, 0.35f * 0.5f, SpriteEffects.None, 0f);


            //Draw Swirls
            var swirl = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/twirl_02").Value;
            var swirl2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/twirl_03").Value;

            Main.spriteBatch.Draw(swirl, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.LawnGreen, vortexRot, swirl.Size() / 2, 0.10f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(swirl, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.LawnGreen, vortexRot + MathHelper.Pi, swirl.Size() / 2, 0.10f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(swirl2, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.LawnGreen, MathHelper.ToRadians(vortexRotsmall * 8), swirl.Size() / 2, 0.06f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);



            //Draw Proj
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/ChloroFlare").Value;
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Tex.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None, 0f);


            return false;
        }


        public override void Kill(int timeLeft)
        {
            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_1") with { Pitch = -.53f, };
            SoundEngine.PlaySound(style, Projectile.Center);

            for (int i = 0; i < 2; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(0, 0), ModContent.DustType<GlowCircleRise>(),
                    Main.rand.NextVector2Circular(5, 5), Color.Green, Main.rand.NextFloat(0.4f, 0.7f), 0.7f, 0f, dustShader);
                p.alpha = 0;
            }
            


        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            for (int i = 0; i < 5; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowCircleRise>(),
                    Main.rand.NextVector2Circular(5, 5), Color.LawnGreen, Main.rand.NextFloat(0.4f, 0.7f), 0.8f, 0f, dustShader);
                p.alpha = 0;
                //p.rotation = Main.rand.NextFloat(6.28f);
            }

            target.AddBuff(BuffID.Poisoned, 20);
        }
    }
} 