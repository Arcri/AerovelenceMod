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
    public class H3Impact : ModProjectile
    {
		float timer = 0;
		public float opacity = 1f;
		public Color color = Color.White;
		public float size = 1f;
		public float speed = 0.2f;


		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("H3 Impact");

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
			Projectile.scale = 1f;

		}

		public override bool? CanDamage()
		{
			return false;
		}

		float xScale = 0.3f;
		float yScale = 1f;
		public override void AI()
        {
			Player player = Main.player[Projectile.owner];

			if (timer > 1)
            {
				xScale = Math.Clamp(MathHelper.Lerp(xScale, 4, speed * 0.3f), 0, 10);
				yScale = Math.Clamp(MathHelper.Lerp(yScale, -0.2f, speed), 0, 1);
			}


			if (yScale <= 0.09f)
				Projectile.active = false;

			timer++;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/Projectiles/Other/H3Impact").Value;
			int frameHeight = Tex.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

			Vector2 scale = new Vector2(xScale * size, yScale * size);

			Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
			myEffect.Parameters["uColor"].SetValue(color.ToVector3() * 3.2f); //2.5f makes it more spear like
			myEffect.Parameters["uTime"].SetValue(2);
			myEffect.Parameters["uOpacity"].SetValue(0.28f); //0.6
			myEffect.Parameters["uSaturation"].SetValue(1.2f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);


			//Activate Shader
			myEffect.CurrentTechnique.Passes[0].Apply();
			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, color * opacity, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, color * opacity, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
			
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}

    }
}
