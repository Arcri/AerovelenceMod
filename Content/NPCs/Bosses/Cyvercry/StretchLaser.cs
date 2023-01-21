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
using AerovelenceMod.Common.Globals.SkillStrikes;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{   
    public class StretchLaser : ModProjectile
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
        }
        public int enemiesHit = 0;
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 2;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 200;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            if (timer < 60)
                Projectile.velocity *= 1.038f;

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            
            if (timer == 0)
                return false;

            //Draw the Circlular Glow
            var softGlow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulseShot").Value;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + (Projectile.velocity.SafeNormalize(Vector2.UnitX)), Tex.Frame(1, 1, 0, 0), Color.DeepPink, Projectile.rotation, Tex.Size() / 2, new Vector2(0.5f, Projectile.velocity.Length() * 0.15f), SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition , softGlow.Frame(1, 1, 0, 0), Color.Pink, Projectile.rotation, softGlow.Size() / 2, new Vector2(0.5f, Projectile.velocity.Length() * 0.15f), SpriteEffects.None, 0f);

            //Set up Shader
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * 2.2f); //2.5f makes it more spear like
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.28f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


            //Activate Shader
            myEffect.CurrentTechnique.Passes[0].Apply();

            //0.2f
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX), Tex.Frame(1, 1, 0, 0), Color.Crimson, Projectile.rotation, Tex.Size() / 2, new Vector2(0.25f, Projectile.velocity.Length() * 0.15f), SpriteEffects.None, 0f);





            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);



            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //return base.Colliding(projHitbox, targetHitbox);
            Player Player = Main.player[Projectile.owner];
            Vector2 unit = Projectile.rotation.ToRotationVector2();
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + Projectile.velocity * 30, 10, ref point);
        }
    }
} 