﻿using System;
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
    public class GlowyPixelPulse : ModProjectile
    {
		int timer = 0;
		float opacity = 1f;
		public Color color = Color.White;
		public float size = 1f;
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 6;
			// DisplayName.SetDefault("RoAHit");
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

		public override void AI()
        {
			Player player = Main.player[Projectile.owner];
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (timer == 0)
				Projectile.frameCounter = -3;

			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 2)
			{
				if (Projectile.frame == 5)
					Projectile.active = false;

				Projectile.frameCounter = 0;
				Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
			}

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/Projectiles/Other/GlowyPixelPulse").Value;

			int frameHeight = Tex.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width, frameHeight);


            Vector2 origin = sourceRectangle.Size() / 2f;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, color * opacity, Projectile.rotation, origin, size, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, color * opacity * 0.5f, Projectile.rotation, origin, size * 1.1f, SpriteEffects.None, 0f);


            //Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, color * opacity * 0.5f, Projectile.rotation, origin, size * 1.25f, SpriteEffects.None, 0f);

            return false;
		}
		public override void PostDraw(Color lightColor)
		{
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

        }


    }
}
