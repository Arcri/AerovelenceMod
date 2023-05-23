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

                Vector2 offset = new Vector2(25, 0).RotatedBy(Projectile.rotation);

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + offset,
                    Projectile.rotation.ToRotationVector2() * -0.1f, ModContent.ProjectileType<CyverBeam>(),
                    Projectile.damage, Projectile.knockBack);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + offset * -1,
                    Projectile.rotation.ToRotationVector2() * 0.1f, ModContent.ProjectileType<CyverBeam>(),
                    Projectile.damage, Projectile.knockBack);

                whiteIntensity = 1;

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

            if (timer >= 60)
            {
                drawAlpha = MathHelper.Clamp(MathHelper.Lerp(drawAlpha, -0.2f, 0.1f), 0f, 1f);
                overallScale = Math.Clamp(MathHelper.Lerp(overallScale, 5f, 0.2f), 0f, 1f);

                if (drawAlpha == 0)
                    Projectile.active = false;
            }
            else
            {
                overallScale = Math.Clamp(MathHelper.Lerp(overallScale, 1.5f, 0.2f), 0f, 1f);
            }

            whiteIntensity = Math.Clamp(MathHelper.Lerp(whiteIntensity, -0.25f, 0.04f), 0, 1);
            timer++;
        }

        float teleScale = 0f;
        float drawAlpha = 1f;
        float colorIntensity = 2f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/PinkL").Value;
            Texture2D White = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/WhiteL").Value;

            float L1rot = Projectile.rotation - MathHelper.ToRadians(45);
            float L2rot = Projectile.rotation - MathHelper.ToRadians(225);

            Vector2 L1pos = new Vector2(0, 10).RotatedBy(Projectile.rotation) + Projectile.Center;
            Vector2 L2pos = new Vector2(0, -10).RotatedBy(Projectile.rotation) + Projectile.Center;

            //Vector2 L1pos = (L1rot.ToRotationVector2() * 30) + Projectile.Center;
            //Vector2 L2pos = (L2rot.ToRotationVector2() * 30) + Projectile.Center;

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
                Vector2 vec2Scale = new Vector2(teleScale, teleScale * 0.75f);

                Texture2D RayTex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/Medusa_Gray").Value;
                Main.spriteBatch.Draw(RayTex, Projectile.Center - Main.screenPosition + new Vector2(-50 * teleScale, 0).RotatedBy(Projectile.rotation), RayTex.Frame(1, 1, 0, 0), Color.DeepPink * drawAlpha * 1f, Projectile.rotation + MathHelper.Pi, RayTex.Size() / 2, vec2Scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(RayTex, Projectile.Center - Main.screenPosition + new Vector2(50 * teleScale, 0).RotatedBy(Projectile.rotation), RayTex.Frame(1, 1, 0, 0), Color.DeepPink * drawAlpha * 1f, Projectile.rotation, RayTex.Size() / 2, vec2Scale, SpriteEffects.None, 0);

            }
            //Texture2D spotTex = Mod.Assets.Request<Texture2D>("Assets/Glorb").Value;
            //Main.spriteBatch.Draw(spotTex, Projectile.Center - Main.screenPosition, spotTex.Frame(1, 1, 0, 0), Color.DeepPink * drawAlpha * 0.7f, Projectile.rotation, spotTex.Size() / 2, drawAlpha * 0.2f, SpriteEffects.None, 0);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Tex, L1pos - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White * 1f * drawAlpha, L1rot, Tex.Size() / 2, overallScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, L2pos - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White * 1f * drawAlpha, L2rot, Tex.Size() / 2, overallScale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(White, L1pos - Main.screenPosition, White.Frame(1, 1, 0, 0), Color.White * 0.8f * drawAlpha * whiteIntensity, L1rot, White.Size() / 2, overallScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(White, L2pos - Main.screenPosition, White.Frame(1, 1, 0, 0), Color.White * 0.8f * drawAlpha * whiteIntensity, L2rot, White.Size() / 2, overallScale, SpriteEffects.None, 0f);




            //Main.spriteBatch.Draw(spotTex, Projectile.Center - Main.screenPosition, spotTex.Frame(1, 1, 0, 0), Color.DeepPink * drawAlpha * 2f, Projectile.rotation, spotTex.Size() / 2, drawAlpha * 0.4f, SpriteEffects.None, 0);
            //Main.spriteBatch.Draw(spotTex, Projectile.Center - Main.screenPosition, spotTex.Frame(1, 1, 0, 0), Color.DeepPink * drawAlpha * 2f, Projectile.rotation, spotTex.Size() / 2, drawAlpha * 0.4f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


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
            //int sin = (int)(Math.Sin(secondTimer * 0.05) * 40f);
            //var color = new Color(255, 160 + sin, 40 + sin / 2);

            Color col = timer < 2 ? Color.White * 0.5f : Color.DeepPink;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(col.ToVector3() * 3f);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(luminos); //0.8
            myEffect.Parameters["uSaturation"].SetValue(1.2f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            if (timer > 0)
            {
                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

                endPoint = LaserRotation.ToRotationVector2() * 2000f;
                var texBeam = Mod.Assets.Request<Texture2D>("Assets/GlowTrailMoreRes").Value;

                Vector2 origin2 = new Vector2(0, texBeam.Height / 2);

                float height = (50f * Projectile.scale) - additional; //15

                if (height == 0)
                    Projectile.active = false;

                int width = (int)(Projectile.Center - endPoint).Length() - 24;

                var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
                var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));

                Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);
                Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);


                //for (int i = 0; i < width; i += 6)
                    //Lighting.AddLight(pos + Vector2.UnitX.RotatedBy(LaserRotation) * i + Main.screenPosition, Color.DeepPink.ToVector3() * height * 0.020f); //0.030

                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

                float progress = additional / (50f * Projectile.scale);
                Texture2D spotTex = Mod.Assets.Request<Texture2D>("Assets/Glorb").Value;
                Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.DeepPink, Projectile.rotation, spotTex.Size() / 2, 1f - progress, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.DeepPink, Projectile.rotation, spotTex.Size() / 2, 1f - progress, SpriteEffects.None, 0);


                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
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
} 