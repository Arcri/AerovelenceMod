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

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns
{   
    public class AdamantitePulseShot : ModProjectile
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
        }

        public const int MAX_PENETRATION = 5;
        public int enemiesHit = 0;
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 3;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 400;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;

        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            if (timer < 180)
                Projectile.velocity *= 1.03f;

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {

            //Draw the Circlular Glow
            var softGlow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulseShot").Value;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -140), Tex.Frame(1, 1, 0, 0), Color.Red * 0.85f, Projectile.rotation, Tex.Size() / 2, new Vector2(0.5f, Projectile.velocity.Length() * 0.15f), SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition , softGlow.Frame(1, 1, 0, 0), Color.Red, Projectile.rotation, softGlow.Size() / 2, new Vector2(1f, Projectile.velocity.Length() * 0.06f), SpriteEffects.None, 0f);

            //Set up Shader
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.Crimson.ToVector3() * 3.2f); //2.5f makes it more spear like
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.28f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            //0.2f
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * (-140), Tex.Frame(1, 1, 0, 0), Color.Crimson, Projectile.rotation, Tex.Size() / 2, new Vector2(0.25f, Projectile.velocity.Length() * 0.15f), SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -70), Tex.Frame(1, 1, 0, 0), Color.Crimson, Projectile.rotation, Tex.Size() / 2, new Vector2(0.1f, Projectile.velocity.Length() * 0.05f), SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * -30), Tex.Frame(1, 1, 0, 0), Color.Cyan, Projectile.rotation, Tex.Size() / 2, 0.2f, SpriteEffects.None, 0f);


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




        public override void Kill(int timeLeft)
        {    

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            for (int i = 0; i < 5; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleDust>(),
                    (Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.Pi + Main.rand.NextFloat(-1, 1)) * Main.rand.Next(1, 3)),
                    Color.Red, Main.rand.NextFloat(0.2f, 0.4f), 0.3f, 0f, dustShader);
                p.alpha = 0;
                //p.rotation = Main.rand.NextFloat(6.28f);
            }

        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            enemiesHit++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > 0)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_10") with { Pitch = .42f, PitchVariance = .38f, MaxInstances = -1, Volume = 0.5f };
                SoundEngine.PlaySound(style, Projectile.Center);
            }
            return true;
        }

        public override bool? CanDamage()
        {
            if (enemiesHit >= MAX_PENETRATION)
                return false;
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {


            return base.Colliding(projHitbox, targetHitbox);
        }
    }
} 