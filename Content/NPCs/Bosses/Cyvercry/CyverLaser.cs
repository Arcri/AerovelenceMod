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

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{   
    public class CyverLaser : ModProjectile
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyver Laser");

        }
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 2;
            Projectile.width = 5;
            Projectile.height = 5;
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
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() * 0.5f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {

            //Draw the Circlular Glow
            var softGlow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            var Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/CyverLaser").Value;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition , softGlow.Frame(1, 1, 0, 0), Color.OrangeRed, Projectile.rotation, softGlow.Size() / 2, 1f, SpriteEffects.None, 0f);

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
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -30), Tex.Frame(1, 1, 0, 0), Color.Cyan, Projectile.rotation, Tex.Size() / 2, 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -30), Tex.Frame(1, 1, 0, 0), Color.Cyan, Projectile.rotation, Tex.Size() / 2, 0.2f, SpriteEffects.None, 0f);

             Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);



            return false;
        }


        public override void Kill(int timeLeft)
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


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > 0)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_10") with { Pitch = .92f, PitchVariance = .28f, MaxInstances = -1, Volume = 0.5f };
                SoundEngine.PlaySound(style, Projectile.Center);
            }
            return true;
        }

        public override bool? CanDamage()
        {
            return true;
        }
    }
} 