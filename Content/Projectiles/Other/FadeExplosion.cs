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
    public class FadeExplosion : ModProjectile
    {
		int timer = 0;
		public float colorIntensity = 1f;
		public Color color = Color.White;
		public float size = 1f;
		public float multiplier = 3f;
		public bool rise = false;

		public bool rotDir = Main.rand.NextBool();

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Fade Explosion");

		}

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

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
        {
			Player player = Main.player[Projectile.owner];
			timer++;

			if (rotDir)
				Projectile.rotation += 0.02f;
			else
				Projectile.rotation -= 0.02f;

			colorIntensity -= 0.02f;

			Projectile.scale = MathHelper.Clamp(MathHelper.Lerp(Projectile.scale, size, 0.15f), 0f, size / 2);
			if (colorIntensity <= 0)
				Projectile.active = false;

			if (rise)
				Projectile.velocity.Y += -0.06f;

        }

		public override bool PreDraw(ref Color lightColor)
		{

            var Tex = Mod.Assets.Request<Texture2D>("Content/Projectiles/Other/FadeExplosion").Value;

            int frameHeight = Tex.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width, frameHeight);


            Vector2 origin = sourceRectangle.Size() / 2f;


			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
			myEffect.Parameters["uColor"].SetValue(color.ToVector3() * (multiplier * colorIntensity));
			myEffect.Parameters["uTime"].SetValue(2);
			myEffect.Parameters["uOpacity"].SetValue(0.5f); //0.6
			myEffect.Parameters["uSaturation"].SetValue(1.2f);


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
			myEffect.CurrentTechnique.Passes[0].Apply();

			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
			//Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

			return false;
		}
		public override void PostDraw(Color lightColor)
		{
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
		}


    }
}
