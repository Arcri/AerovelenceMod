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
    public class HollowPulse : ModProjectile
    {
		int timer = 0;
		float opacity = 1f;
		public Color color = Color.White;
		public float size = 1f;
		public bool oval = false;

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

		}

        public override bool? CanDamage() { return false; }
	
		public override void AI()
        {
			Player player = Main.player[Projectile.owner];
			timer++;

			Projectile.scale = MathHelper.Clamp(MathHelper.Lerp(Projectile.scale, 0.75f * size, 0.08f), 0f, 0.5f * size);

			if (Projectile.scale == 0.5f * size)
				opacity = MathHelper.Clamp(MathHelper.Lerp(opacity, -0.2f, 0.1f), 0, 2);

			if (opacity <= 0)
				Projectile.active = false;

        }

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D Tex;
			if (oval)
				Tex = Mod.Assets.Request<Texture2D>("Content/Projectiles/Other/HollowOvalPulse").Value;
			else
				Tex = Mod.Assets.Request<Texture2D>("Content/Projectiles/Other/HollowPulse").Value;

			int frameHeight = Tex.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width, frameHeight);


            Vector2 origin = sourceRectangle.Size() / 2f;


            Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, color * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, color * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

			return false;
		}
		public override void PostDraw(Color lightColor)
		{
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
		}


    }

	public class otherHollowPulseTestDearFutureMePleaseRewriteAndMoveThisInsteadOfUsingItInTheFutureDearGod : ModProjectile
	{
        public override string Texture => "Terraria/Images/Projectile_0";


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
            Player player = Main.player[Projectile.owner];
            timer++;

            if (Projectile.scale < 0.5f * size)
                Projectile.scale = MathHelper.Clamp(MathHelper.Lerp(Projectile.scale, 0.75f * size, 0.06f), 0f, 0.5f * size);
            else
                Projectile.scale += 0.01f;


            if (Projectile.scale >= 0.5f * size)
                opacity = MathHelper.Clamp(MathHelper.Lerp(opacity, -0.2f, 0.1f), 0, 2);

            if (opacity <= 0)
                Projectile.active = false;

            if (timer > 2)
                Projectile.velocity *= 0.96f;

            Projectile.rotation = Projectile.velocity.ToRotation();
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
