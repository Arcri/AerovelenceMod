using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;

namespace AerovelenceMod.Content.Projectiles.Other
{

    //TODO make less shitty/finish
	public class CirclePulse : ModProjectile
	{
        public override string Texture => "Terraria/Images/Projectile_0";

        public enum Behavoir
        {
            Base,
            JustFade,
            SlowDown,
        }

        public Behavoir behavoir = Behavoir.Base;

        int timer = 0;
        float opacity = 1f;
        public Color color = Color.White;
        public float size = 1f;

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.scale = 0f;

            Projectile.extraUpdates = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (behavoir == Behavoir.Base)
            {
                if (Projectile.scale < 0.5f * size)
                    Projectile.scale = MathHelper.Clamp(MathHelper.Lerp(Projectile.scale, 0.75f * size, 0.06f), 0f, 0.5f * size);
                else
                    Projectile.scale += 0.01f;


                if (Projectile.scale >= 0.5f * size)
                    opacity = MathHelper.Clamp(MathHelper.Lerp(opacity, -0.2f, 0.1f), 0, 2);

                if (timer > 3)
                    Projectile.velocity *= 0.96f;
            }


            Projectile.rotation = Projectile.velocity.ToRotation();

            if (opacity <= 0)
                Projectile.active = false;

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Tex = Mod.Assets.Request<Texture2D>("Assets/Orbs/impact_2newbetterfade").Value;
            //Texture2D Tex = Mod.Assets.Request<Texture2D>("Assets/Orbs/zFadeCircle").Value;

            Vector2 vec2Scale = new Vector2(0.4f, 0.8f) * Projectile.scale;

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, null, color with { A = 0 } * opacity * 0.2f, Projectile.rotation + MathF.PI, Tex.Size() / 2, vec2Scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, null, color * opacity, Projectile.rotation + MathF.PI, Tex.Size() / 2, vec2Scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, null, color * opacity * 0.6f, Projectile.rotation + MathF.PI, Tex.Size() / 2, vec2Scale * 1.1f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, null, color * opacity, Projectile.rotation + MathF.PI, Tex.Size() / 2, vec2Scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, null, color * opacity * 0.1f, Projectile.rotation + MathF.PI, Tex.Size() / 2, vec2Scale * 1.25f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

    }
}
