using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using System;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Graphics.Shaders;
using rail;
using Terraria.Audio;
using Terraria.ID;
using AerovelenceMod.Content.Buffs.FlareDebuffs;
using Steamworks;

namespace AerovelenceMod.Content.Items.Weapons.Flares
{   
    //This is for the little explosion effect that happens when you hit an NPC with a flare
    public abstract class BaseFlareExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        int timer = 0;
        public override bool? CanDamage() { return false; }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 0.3f;
            Projectile.timeLeft = 100;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public Color col = new Color(255, 75, 50); 
        public float colMultipliter = 2.5f; //How intense the color is for shader
        public float scale = 0.3f;

        public override void AI()
        {
            aiLogic();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawLogic();
            return false;
        }

        public void DrawLogic()
        {
            var Fire = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/scorch_01").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(col.ToVector3() * colMultipliter);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.8f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Fire, Projectile.Center - Main.screenPosition, Fire.Frame(1, 1, 0, 0), Color.OrangeRed, Projectile.rotation, Fire.Size() / 2, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Fire, Projectile.Center - Main.screenPosition, Fire.Frame(1, 1, 0, 0), Color.OrangeRed, Projectile.rotation + 2, Fire.Size() / 2, scale * 0.2f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

        }
        public void aiLogic()
        {
            //Shrink scale to zero and kill Projectile when it hits it
            scale = Math.Clamp(MathHelper.Lerp(scale, -0.1f, 0.12f), 0, 0.3f);

            if (scale <= 0f)
                Projectile.active = false;

            timer++;
        }
    }


    //This is for the actual flare projectile
    public abstract class BaseFlare : ModProjectile
    {
        public Color flareCol = Color.Red;
        public float flareColIntensity = 2; //Color intensity for the shader
        public Color dustCol = new Color(255, 75, 50);
        public Vector3 lightCol = Color.Red.ToVector3() * 1f; //Color of light
        public bool noSound = false;


        //Location of the little gungeon bullet texture
        public String textureLocation = "Content/Items/Weapons/Flares/FlareCores/FireFlare2";

        public int timer = 0;

        //for drawing
        float vortexRot = 0;
        float vortexRotsmall;
        float secondScale = 0.3f; //Scale used for drawing the quad-point star
        float randomRot = 0;

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true; //Debating whether to make them disapate on water
            Projectile.tileCollide = true;
        }

        float alpha = 0f;
        public override void AI()
        {
            baseAILogic();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            baseDrawing();
            return false;
        }

        public void baseAILogic()
        {
            //Add light based on set color
            Lighting.AddLight(Projectile.Center, lightCol);

            //Set randomRot to a random rotation (*shocking*)
            if (timer == 0)
            {
                randomRot = Main.rand.NextFloat(6.28f);
            }

            
            //Rotate based on direction 
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

            //Dust
            if (timer % 7 == 0)
            {
                ArmorShaderData dustShader1 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                for (int i = 0; i < 1 + Main.rand.NextFloat(0, 1); i++)
                {
                    Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleRise>(),
                        new Vector2(0, -2) + Projectile.velocity * -0.2f + Main.rand.NextVector2Circular(1, 1), dustCol, Main.rand.NextFloat(0.3f, 0.6f), 0.7f, 1.2f, dustShader1);
                }

            }

            if (timer % 4 == 0)
            {
                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                for (int i = 0; i < 1; i++)
                {
                    Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), ModContent.DustType<GlowCircleRise>(),
                        new Vector2(0, -2) + Projectile.velocity * -0.2f + Main.rand.NextVector2Circular(3, 3), Color.Gray * 0.65f, Main.rand.NextFloat(0.5f, 0.9f), 1f, 0f, dustShader2);
                }
            }

            //have the flare fade in quickly
            alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.5f, 0.1f), 0, 1);

            if (timer > 20) secondScale = Math.Clamp(secondScale - 0.015f, 0, 0.3f);


            //gravity
            Projectile.velocity.Y += 0.29f;

            timer++;
        }

        public void baseDrawing()
        {
            //load textures
            Texture2D softGlow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            Texture2D star = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;
            Texture2D star2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_05").Value;
            Texture2D FlareCircle = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/flare_01").Value;
            Texture2D swirl = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/twirl_02").Value;
            Texture2D swirl2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/twirl_03").Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            //Draw outer glow
            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition, softGlow.Frame(1, 1, 0, 0), flareCol * alpha, Projectile.rotation, softGlow.Size() / 2, 3.3f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(star2, Projectile.Center - Main.screenPosition, star2.Frame(1, 1, 0, 0), flareCol * 0.7f * alpha, randomRot + MathHelper.ToRadians(vortexRotsmall * -2), star2.Size() / 2, 0.20f, SpriteEffects.None, 0f);

            //Activate Shader
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(flareCol.ToVector3() * flareColIntensity * alpha);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.6f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            //draw four-point stars
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            if (timer > 1 && timer < 50)
            {
                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), flareCol, randomRot + MathHelper.ToRadians(vortexRotsmall * 3 + 45), star.Size() / 2, secondScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), flareCol, randomRot + MathHelper.ToRadians(vortexRotsmall * 3 + 45), star.Size() / 2, secondScale, SpriteEffects.None, 0f);
            }

            myEffect.CurrentTechnique.Passes[0].Apply();

            //Draw Flare Center
            Main.spriteBatch.Draw(FlareCircle, Projectile.Center - Main.screenPosition, FlareCircle.Frame(1, 1, 0, 0), flareCol * alpha, (float)Math.PI, FlareCircle.Size() / 2, 0.35f * 0.5f, SpriteEffects.None, 0f);


            //Draw Swirls
            //This one has shader
            Main.spriteBatch.Draw(swirl, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), flareCol * alpha, vortexRot, swirl.Size() / 2, 0.10f, SpriteEffects.None, 0f);

            //These dont
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(swirl, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), flareCol * alpha, vortexRot + MathHelper.Pi, swirl.Size() / 2, 0.10f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(swirl2, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), flareCol * alpha, MathHelper.ToRadians(vortexRotsmall * 8), swirl.Size() / 2, 0.06f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            Texture2D CenterTex = Mod.Assets.Request<Texture2D>(textureLocation).Value;
            Main.spriteBatch.Draw(CenterTex, Projectile.Center - Main.screenPosition, CenterTex.Frame(1, 1, 0, 0), Color.White * alpha, Projectile.rotation, CenterTex.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None, 0f);
        }

        public void KillDust() //Also includes SFX
        {
            if (!noSound)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_1") with { Pitch = -.53f, };
                SoundEngine.PlaySound(style, Projectile.Center);
            }

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            for (int i = 0; i < 5; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(0, 0), ModContent.DustType<GlowCircleRise>(),
                    Main.rand.NextVector2Circular(5, 5), flareCol, Main.rand.NextFloat(0.4f, 0.7f), 0.7f, 0f, dustShader);
                p.alpha = 0;
            }

        }

        public void HitDust() //Also includes SFX
        {
            if (!noSound)
            {
                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/FlareImpact") with { Volume = 0.5f, PitchVariance = 0.1f };
                SoundEngine.PlaySound(style2, Projectile.Center);

                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_45") with { Pitch = .75f, PitchVariance = 0.2f };
                SoundEngine.PlaySound(style);
            }

            //int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FrostFlareExplosion>(), 0, 0, Main.myPlayer);
            //Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            for (int i = 0; i < 3; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleRise>(),
                    Main.rand.NextVector2Circular(5, 5), flareCol, Main.rand.NextFloat(0.4f, 0.7f), 0.8f, 0f, dustShader);
                p.alpha = 0;
            }
        }

    }

    public abstract class BaseFlareGunItem : ModItem
    {

    }

    public abstract class BaseFlareDebuffNPC   : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool DebuffActive = false; 

        public float DebuffTime = 0f;

        public float timeBetweenHits = 30;

        public int tickDamage = 10;

        public float tagDamage = 3;

        public float tagCrit = 0;

        public SoundStyle sound = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_1") with { Pitch = -.53f, PitchVariance = 0.3f, Volume = 0.5f, MaxInstances = -1 };
        
        public Color colorA = new Color(255, 75, 50);

        public Color colorB = Color.OrangeRed;

        public int DebuffIndex = ModContent.BuffType<FlareFire>();

        public override void ResetEffects(NPC npc)
        {
            baseResetEffects(npc);
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            baseUpdateLifeRegen(npc, ref damage);
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            baseModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);
        }

        public void baseResetEffects(NPC npc)
        {
            if (!npc.HasBuff(DebuffIndex))
            {
                DebuffActive = false;
                DebuffTime = 0;
            }
        }

        public void baseUpdateLifeRegen(NPC npc, ref int damage)
        {
            if (DebuffActive)
            {
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                if (DebuffTime % 3 == 0)
                {
                    if (Main.rand.NextBool())
                    {
                        int p = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleRise>(), colorA, 0.4f, 0.65f, 0f, dustShader);
                        Main.dust[p].velocity *= 0.5f;
                    }
                    else
                    {
                        int d = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleRise>(), colorB, 0.4f, 0.65f, 0f, dustShader);
                        Main.dust[d].velocity *= 0.5f;
                    }
                }
                else if (DebuffTime % 7 == 0) //else if is intentional
                {

                    int p = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleRise>(), Color.Gray * 0.65f, Main.rand.NextFloat(0.5f, 0.9f), 1f, 0f, dustShader);
                    Main.dust[p].velocity.X *= 0.2f;
                    Main.dust[p].velocity.Y = Math.Abs(Main.dust[p].velocity.Y) * -1f;
                }

                //Very dumb, but should work
                if (DebuffTime % timeBetweenHits == 0)
                {

                    //Here we do something quite fuckywucky
                    //We store the NPC's hitsound, replace it with the desired sound effect, then strike it and set it back to normal
                    //This is to stop it playing the orginal hitsound

                    SoundStyle? storedHitsound = npc.HitSound;
                    npc.HitSound = sound;
                    npc.StrikeNPC(tickDamage, 0, 0, noEffect: true);
                    npc.HitSound = storedHitsound;


                }
                DebuffTime++;
            }
        }

        public void baseModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (DebuffActive && !projectile.npcProj && !projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]))
            {
                damage += (int)tagDamage;
                if (tagCrit >= Main.rand.Next(1, 101))
                    crit = true;
            }
        }
    }
} 