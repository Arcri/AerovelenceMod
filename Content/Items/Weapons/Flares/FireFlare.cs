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
using AerovelenceMod.Content.Dusts;

using Terraria.GameContent;
using Terraria.Audio;
using AerovelenceMod.Content.Buffs.FlareDebuffs;

namespace AerovelenceMod.Content.Items.Weapons.Flares
{
    public class FireFlare : ModProjectile
    {
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
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

        }
        public override void AI()
        {

            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() * 1.5f);
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
                        new Vector2(0, -2) + Projectile.velocity * -0.2f + Main.rand.NextVector2Circular(1, 1), new Color(255, 75, 50), Main.rand.NextFloat(0.3f, 0.6f), 0.7f, 1.2f, dustShader);
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

            var softGlow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition, softGlow.Frame(1, 1, 0, 0), Color.Red, Projectile.rotation, softGlow.Size() / 2, 3.3f, SpriteEffects.None, 0f);
            var star = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;
            var star2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_05").Value;

            if (timer > 0 && timer < 50)
            {
                //Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), Color.Red, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * 3 + 45), star.Size() / 2, FlareLerp, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(star2, Projectile.Center - Main.screenPosition, star2.Frame(1, 1, 0, 0), Color.Red * 0.7f, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * -2), star2.Size() / 2, 0.20f, SpriteEffects.None, 0f);




            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.Red.ToVector3() * 2);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.6f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            if (timer > 1 && timer < 50)
            {
                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), Color.Red, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * 3 + 45), star.Size() / 2, FlareLerp, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), Color.Red, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * 3 + 45), star.Size() / 2, FlareLerp, SpriteEffects.None, 0f);
            }

            myEffect.CurrentTechnique.Passes[0].Apply();

            //Draw Flare Center
            var FlareFlare = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/flare_01").Value;
            //Main.spriteBatch.Draw(FlareFlare, Projectile.Center - Main.screenPosition, FlareFlare.Frame(1, 1, 0, 0), Color.Red, 0f, FlareFlare.Size() / 2, 0.35f * 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(FlareFlare, Projectile.Center - Main.screenPosition, FlareFlare.Frame(1, 1, 0, 0), Color.Red, (float)Math.PI, FlareFlare.Size() / 2, 0.35f * 0.5f, SpriteEffects.None, 0f);


            //Draw Swirls
            var swirl = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/twirl_02").Value;
            var swirl2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/twirl_03").Value;

            //Main.spriteBatch.Draw(swirl, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.Red, vortexRot + MathHelper.Pi, swirl.Size() / 2, 0.10f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(swirl, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.Red, vortexRot, swirl.Size() / 2, 0.10f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(swirl2, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.Red, MathHelper.ToRadians(vortexRotsmall * 8), swirl.Size() / 2, 0.06f, SpriteEffects.None, 0f);

            //DrawStar
            //var star = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;
            //if (timer > 5 && timer < 40)
           // {
                //Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), Color.Red * 0.75f, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall), star.Size() / 2, FlareLerp, SpriteEffects.None, 0f);
                //Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), Color.Red * 0.75f, randomRotation[1] + vortexRotsmall, star.Size() / 2, FlareLerp, SpriteEffects.None, 0f);

            //}

            Main.spriteBatch.Draw(swirl, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.Red, vortexRot, swirl.Size() / 2, 0.10f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


            //Main.spriteBatch.Draw(FlareFlare, Projectile.Center - Main.screenPosition, FlareFlare.Frame(1, 1, 0, 0), Color.Red, 0f, FlareFlare.Size() / 2, 0.35f * 0.5f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(FlareFlare, Projectile.Center - Main.screenPosition, FlareFlare.Frame(1, 1, 0, 0), Color.Red, (float)Math.PI, FlareFlare.Size() / 2, 0.35f * 0.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(swirl, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.Red, vortexRot + MathHelper.Pi, swirl.Size() / 2, 0.10f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(swirl2, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.Red, MathHelper.ToRadians(vortexRotsmall * 8), swirl.Size() / 2, 0.06f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/FireFlare2").Value;
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Tex.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None, 0f);

            //var glowmask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/GlowMask1").Value;
            //Main.spriteBatch.Draw(glowmask, Projectile.Center - Main.screenPosition, glowmask.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, glowmask.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None, 0f);


            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override void Kill(int timeLeft)
        {
            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_2") with { Pitch = -.53f, };
            SoundEngine.PlaySound(style, Projectile.Center);

            //SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/FlareImpact") with { Volume = 0.5f, MaxInstances = 1, PitchVariance };
            //SoundEngine.PlaySound(style2, Projectile.Center);


            for (int i = 0; i < 5; i++) //2
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(0, 0), ModContent.DustType<GlowCircleRise>(),
                    Main.rand.NextVector2Circular(5, 5), Color.OrangeRed, Main.rand.NextFloat(0.4f, 0.7f), 0.7f, 0f, dustShader);
                p.alpha = 0;
                //p.rotation = Main.rand.NextFloat(6.28f);
            }
            
            /*
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(60, 60), ModContent.DustType<Dusts.Glow>(),
                    Main.rand.NextVector2Circular(5, 5), 0, new Color(255, 150, 50), 0.95f,);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(60, 60), ModContent.DustType<Dusts.CoachGunDust>(),
                    Main.rand.NextVector2Circular(10, 10), 70 + Main.rand.Next(60), default, Main.rand.NextFloat(1.5f, 1.9f)).rotation = Main.rand.NextFloat(6.28f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(60, 60), ModContent.DustType<Dusts.CoachGunDustTwo>(),
                    Main.rand.NextVector2Circular(10, 10), 80 + Main.rand.Next(40), default, Main.rand.NextFloat(1.5f, 1.9f)).rotation = Main.rand.NextFloat(6.28f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(60, 60), ModContent.DustType<Dusts.CoachGunDustFour>()).scale = 0.9f;
            }
            */

        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {

            SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/FlareImpact") with { Volume = 0.5f, PitchVariance = 0.1f};
            SoundEngine.PlaySound(style2, Projectile.Center);

            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_45") with { Pitch = .75f, PitchVariance = 0.2f };
            SoundEngine.PlaySound(style);

            //SoundStyle style = new SoundStyle("Terraria/Sounds/Item_105") with { Pitch = .55f, };
            //SoundEngine.PlaySound(style);

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;

            int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FireFlareExplosion>(), 0, 0, Main.myPlayer);
            Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);
            for (int i = 0; i < 3; i++) 
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowCircleRise>(),
                    Main.rand.NextVector2Circular(5, 5), Color.OrangeRed, Main.rand.NextFloat(0.4f, 0.7f), 0.4f, 0f, dustShader);
                p.alpha = 0;

                //p.rotation = Main.rand.NextFloat(6.28f);

                
                /*
                for (int j = 0; j < 3; j++)
                {

                    Vector2 randomVecout = Main.rand.NextVector2Circular(5, 5);
                    int a = Dust.NewDust(target.Center, 4, 4, ModContent.DustType<ColorSmoke>(), randomVecout.X, randomVecout.Y);

                    Main.dust[a].scale *= 0.5f;

                    Main.dust[a].rotation = Main.rand.NextFloat(6.28f);

                    if (Main.dust[a].velocity.Y > 0)
                    {
                        Main.dust[a].velocity.Y *= -1f;
                    }
                }
                */

            }

            //for (float i = 0f; i < 6.28f; i += 6.28f / 24f)
            //{
                //Dust p = GlowDustHelper.DrawGlowDustPerfect(target.Center + Vector2.One.RotatedBy(i) * 10, ModContent.DustType<GlowCircleFlare>(),
                    //Vector2.One.RotatedBy(i) * -4.5f, Color.Red, 0.6f, 0.4f, 0f, dustShader);
                //p.velocity *= 0.3f;
                //int pindex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(0, 150 * 1.6f), Vector2.One.RotatedBy(i) * 4.5f, ModContent.ProjectileType<StraightSaw>(), 3, 0);
                //Main.projectile[pindex].scale = 0.1f;
            //}

            target.AddBuff(ModContent.BuffType<FlareFire>(), 200);
        }
    }

    public class FireFlareExplosion : ModProjectile
    {
        int timer = 0;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 0.3f; //0.3f
            Projectile.timeLeft = 100;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

        }

        public override void AI()
        {

            scale = Math.Clamp(MathHelper.Lerp(scale, 0f, 0.12f), 0, 0.3f);
            //float scaleBonus = Math.Clamp(Projectile.scale + (timer * 0.009f), 0, 0.3f);

            if (scale == 0f)
                Projectile.active = false;

            Projectile.rotation = Projectile.rotation + MathHelper.ToRadians(timer * 0.05f);
            timer++;
        }
        float scale = 0.3f;
        public override bool PreDraw(ref Color lightColor)
        {
            var Fire = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/scorch_01").Value;




            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(new Color(255, 75, 50).ToVector3() * 2.5f);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.8f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Fire, Projectile.Center - Main.screenPosition, Fire.Frame(1, 1, 0, 0), Color.OrangeRed, Projectile.rotation, Fire.Size() / 2, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Fire, Projectile.Center - Main.screenPosition, Fire.Frame(1, 1, 0, 0), Color.OrangeRed, Projectile.rotation + 2, Fire.Size() / 2, scale * 0.2f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }
    }

} 