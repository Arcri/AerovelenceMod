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
using AerovelenceMod.Content.Dusts;
using static Terraria.ModLoader.PlayerDrawLayer;
using AerovelenceMod.Content.Items.Weapons.Frost.DeepFreeze;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;
using AerovelenceMod.Content.Buffs.FlareDebuffs;

namespace AerovelenceMod.Content.Items.Weapons.Flares
{   
    public class FrostFlare : BaseFlare
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        
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
            flareCol = Color.DeepSkyBlue;
            flareColIntensity = 1.5f; //Color intensity for the shader
            dustCol = Color.DeepSkyBlue;
            lightCol = Color.SkyBlue.ToVector3() * 1f; //Color of light

            baseAILogic();

            int modulo = timer < 60 ? 18 : 24;
            if (timer % modulo == 0 && timer != 0)
            {
                int a = Projectile.NewProjectile(null, Projectile.Center, new Vector2(0, 2).RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f)), ModContent.ProjectileType<FrostFlareIcicle>(), Projectile.damage / 2, 0, Main.myPlayer);

                int b = Projectile.NewProjectile(null, Projectile.Center, new Vector2(0, 0.5f).RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f)), ModContent.ProjectileType<DeepFreezeProj>(), 0, 0, Main.myPlayer);
                if (Main.projectile[b].ModProjectile is DeepFreezeProj p)
                {
                    p.multiplier = 1.5f;
                    p.size = 0.25f;
                }

            }
            #region old
            /*
            Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3() * 1f);
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

            int modulo = timer < 60 ? 18 : 24;
            if (timer % modulo == 0 && timer != 0)
            {
                int a = Projectile.NewProjectile(null, Projectile.Center, new Vector2(0,2).RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f)), ModContent.ProjectileType<FrostFlareIcicle>(), Projectile.damage / 2, 0, Main.myPlayer);
                
                int b = Projectile.NewProjectile(null, Projectile.Center, new Vector2(0, 0.5f).RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f)), ModContent.ProjectileType<DeepFreezeProj>(), 0, 0, Main.myPlayer);
                if (Main.projectile[b].ModProjectile is DeepFreezeProj p)
                {
                    p.multiplier = 1.5f;
                    p.size = 0.25f;
                }
                
            }

            if (timer % 7 == 0)
            {
                for (int i = 0; i < 1 + Main.rand.NextFloat(0, 1); i++)
                {
                    Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleRise>(),
                        new Vector2(0, -2) + Projectile.velocity * -0.2f + Main.rand.NextVector2Circular(1, 1), Color.DeepSkyBlue, Main.rand.NextFloat(0.3f, 0.6f), 0.7f, 1.2f, dustShader);
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
            */
            #endregion
        }

        public override bool PreDraw(ref Color lightColor)
        {
            textureLocation = "Content/Items/Weapons/Flares/FlareCores/FrostFlare";
            baseDrawing();

            /*
            //Draw the Circlular Glow
            var softGlow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition, softGlow.Frame(1, 1, 0, 0), Color.DeepSkyBlue, Projectile.rotation, softGlow.Size() / 2, 3.3f, SpriteEffects.None, 0f);
            
            var star = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;
            var star2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_05").Value;

            //Draw the 9 point star glow
            Main.spriteBatch.Draw(star2, Projectile.Center - Main.screenPosition, star2.Frame(1, 1, 0, 0), Color.DeepSkyBlue * 0.7f, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * -2), star2.Size() / 2, 0.20f, SpriteEffects.None, 0f);


            //Set up Shader
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.DeepSkyBlue.ToVector3() * 1.5f);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.8f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

            //Draw the "lens flare"
            if (timer > 1 && timer < 50)
            {
                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), Color.SkyBlue * 2, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * 3 + 45), star.Size() / 2, FlareLerp, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), Color.SkyBlue * 2, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * 3 + 45), star.Size() / 2, FlareLerp, SpriteEffects.None, 0f);
            }

            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            //Draw Flare Center
            var FlareFlare = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/flare_01").Value;
            Main.spriteBatch.Draw(FlareFlare, Projectile.Center - Main.screenPosition, FlareFlare.Frame(1, 1, 0, 0), Color.DeepSkyBlue, (float)Math.PI, FlareFlare.Size() / 2, 0.35f * 0.5f, SpriteEffects.None, 0f);


            //Draw Swirls
            var swirl = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/twirl_02").Value;
            var swirl2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/twirl_03").Value;

            Main.spriteBatch.Draw(swirl, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.DeepSkyBlue, vortexRot, swirl.Size() / 2, 0.10f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(swirl, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.DeepSkyBlue, vortexRot + MathHelper.Pi, swirl.Size() / 2, 0.10f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(swirl2, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.DeepSkyBlue, MathHelper.ToRadians(vortexRotsmall * 8), swirl.Size() / 2, 0.06f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);



            //Draw Proj
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/FlareCores/FrostFlare").Value;
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Tex.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None, 0f);

            */
            return false;
        }

        public override void Kill(int timeLeft)
        {
            noSound = true;
                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_1") with { Pitch = -.53f, PitchVariance = 0.2f };
            SoundEngine.PlaySound(style, Projectile.Center);
            KillDust();
            /*
            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_1") with { Pitch = -.53f, };
            SoundEngine.PlaySound(style, Projectile.Center);

            for (int i = 0; i < 5; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(0, 0), ModContent.DustType<GlowCircleRise>(),
                    Main.rand.NextVector2Circular(5, 5), Color.DeepSkyBlue, Main.rand.NextFloat(0.4f, 0.7f), 0.7f, 0f, dustShader);
                p.alpha = 0;
            }
            */
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            noSound = true;
            HitDust();

            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;;

            SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/FlareImpact") with { Volume = 0.3f, PitchVariance = 0.1f };
            SoundEngine.PlaySound(style2, Projectile.Center);

            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_45") with { Pitch = .75f, PitchVariance = 0.2f };
            SoundEngine.PlaySound(style, Projectile.Center);

            SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/deerclops_ice_attack_0") with { Volume = .1f, Pitch = .81f, PitchVariance = 0.34f };
            SoundEngine.PlaySound(style3, Projectile.Center);

            int b = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DeepFreezeProj>(), 0, 0, Main.myPlayer);
            if (Main.projectile[b].ModProjectile is DeepFreezeProj p2)
            {
                p2.multiplier = 1f;
                p2.size = 0.35f;
            }
            Main.projectile[b].scale = 0.1f;

            int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FrostFlareExplosion>(), 0, 0, Main.myPlayer);
            Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);


            target.AddBuff(ModContent.BuffType<FlareFrostburn>(), 200);


            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            for (int i = 0; i < 3; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowCircleRise>(),
                    Main.rand.NextVector2Circular(5, 5), Color.DeepSkyBlue, Main.rand.NextFloat(0.4f, 0.7f), 0.8f, 0f, dustShader);
                p.alpha = 0;
            }
        }
    }

    public class FrostFlareExplosion : BaseFlareExplosion
    {
        public override void AI()
        {
            col = Color.DeepSkyBlue;
            colMultipliter = 2f;
            aiLogic();
        }
    }

    public class FrostFlareIcicle : ModProjectile
    {
        int timer = 0;
        int frame = Main.rand.Next(3);

        float alpha = 0;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Frost Spike");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;

        }

        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.timeLeft = 320;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.scale = 0.7f;
        }


        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.frame = frame;
            }

            if (timer % 12 == 0)
            {
                int penis = GlowDustHelper.DrawGlowDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowCircleDust>(), Color.DeepSkyBlue, 0.5f * Main.rand.NextFloat(0.7f, 1.3f), 0.55f, 0f, new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic"));
                Main.dust[penis].noLight = true;
            }
            if (timer % 8 == 0)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 255, Scale: 0.85f);
                dust.noGravity = true;
                dust.color = Color.AliceBlue;

            }

            alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.35f, 0.1f), 0, 1);

            //terminal velocity is 23
            Projectile.velocity.Y += 0.03f;
            Projectile.velocity.Y = Math.Abs(Math.Clamp(Projectile.velocity.Y * 1.03f, -23, 23));
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/FrostFlareIcicle").Value;



            //Anim
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, lightColor * alpha, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0.0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            for (int j = 0; j < 10; j++)
            {
                float intensity = j == 0 ? 0.7f : 0.2f;

                Main.spriteBatch.Draw(texture, Projectile.oldPos[j] - Main.screenPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), sourceRectangle, Color.SkyBlue * alpha * intensity, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            

            return false;
        }

        public override void Kill(int timeLeft)
        {

            SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/deerclops_ice_attack_0") with { Volume = .05f, Pitch = .81f, PitchVariance = 0.34f };
            SoundEngine.PlaySound(style, Projectile.Center);
            SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_107") with { Volume = .29f, Pitch = .81f, PitchVariance = 0.2f };
            SoundEngine.PlaySound(style2, Projectile.Center);

            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f, 255, Scale: 0.8f);
                dust.noGravity = true;
                dust.velocity *= 2;
                dust.color = Color.SkyBlue;
            }
            
        }
    }
} 