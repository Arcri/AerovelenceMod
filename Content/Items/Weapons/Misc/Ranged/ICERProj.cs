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

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged
{   
    public class ICERProj : ModProjectile
    {
        public int timer = 0;
        public int hitstopTimer = 0;
        public Vector2 storedVel = Vector2.Zero;
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 3;
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 100;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 2;

        }
        public override void AI()
        {

            Projectile.velocity *= 1.03f;
            Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3() * 0.3f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;

            timer++;

            if (hitstopTimer > 0)
            {
                hitstopTimer--;
                Projectile.velocity = Vector2.Zero;
                if (hitstopTimer == 0)
                    Projectile.velocity = storedVel;


                Projectile.rotation = storedVel.ToRotation();
                Projectile.spriteDirection = Projectile.direction;

            }

        }

        public override bool PreDraw(ref Color lightColor)
        {

            Vector2 vel = Vector2.Zero;

            if (hitstopTimer > 0)
            {
                vel = storedVel;
            }
            else
            {
                vel = Projectile.velocity;
            }

            //Draw the Circlular Glow
            var softGlow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/ICERProj").Value;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition , softGlow.Frame(1, 1, 0, 0), Color.Cyan, Projectile.rotation, softGlow.Size() / 2, 1f, SpriteEffects.None, 0f);

            //Set up Shader
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.Cyan.ToVector3() * 2f); //2.5f makes it more spear like
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.2f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();


            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (vel.SafeNormalize(Vector2.UnitX) * -30), Tex.Frame(1, 1, 0, 0), Color.Cyan, Projectile.rotation, Tex.Size() / 2, 0.2f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.Cyan, Projectile.rotation, Tex.Size() / 2, 0.2f, SpriteEffects.None, 0f);


            //Set up Shader
            //Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            //myEffect.Parameters["uColor"].SetValue(Color.LawnGreen.ToVector3() * 1.5f);
            //myEffect.Parameters["uTime"].SetValue(2);
            //myEffect.Parameters["uOpacity"].SetValue(0.6f); //0.6
            //myEffect.Parameters["uSaturation"].SetValue(1.2f);

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


            //Activate Shader
            //myEffect.CurrentTechnique.Passes[0].Apply();




            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);



            return false;
        }


        public override void OnKill(int timeLeft)
        {

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            for (int i = 0; i < 3; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleDust>(),
                    (Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.Pi + Main.rand.NextFloat(-1, 1)) * Main.rand.Next(1, 3)),
                    Color.Cyan, Main.rand.NextFloat(0.2f, 0.4f), 0.6f, 0f, dustShader);
                p.alpha = 0;
                //p.rotation = Main.rand.NextFloat(6.28f);
            }

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            Projectile.timeLeft += 40;

            Player myPlayer = Main.player[Projectile.owner];

            //myPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = 0;

            SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = 1f, PitchVariance = .14f, Volume = 0.3f, MaxInstances = -1 };
            SoundEngine.PlaySound(style, Projectile.Center);

            hitstopTimer = 20;
            storedVel = Projectile.velocity;
            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            for (int i = 0; i < 3; i++) //4
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleDust>(),
                    (Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-0.5f,0.5f)) * Main.rand.Next(1,3)), 
                    Main.rand.NextBool() ? Color.Cyan : Color.Cyan, Main.rand.NextFloat(0.2f, 0.3f), 0.6f, 0f, dustShader);
                p.alpha = 0;
                //p.rotation = Main.rand.NextFloat(6.28f);
            }
            for (int i = 0; i < 2; i++) //4
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + new Vector2(10,0), ModContent.DustType<LineGlow>(),
                    (Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.Pi + Main.rand.NextFloat(-0.35f, 0.35f)) * Main.rand.Next(2, 5)),
                    Main.rand.NextBool() ? Color.Cyan : Color.Cyan, Main.rand.NextFloat(0.2f, 0.4f), 0.8f, 0f, dustShader);
                p.alpha = 0;
                p.velocity *= 1.5f;
                p.fadeIn = Main.rand.NextFloat(20, 40);
                //p.rotation = Main.rand.NextFloat(6.28f);
            
            }

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > 0)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_10") with { Pitch = .92f, PitchVariance = .28f, MaxInstances = -1 };
                SoundEngine.PlaySound(style, Projectile.Center);
            }
            return true;
        }

        public override bool? CanDamage()
        {
            if (hitstopTimer > 0)
                return false;
            return true;
        }
    }
} 