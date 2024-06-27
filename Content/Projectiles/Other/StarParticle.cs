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
using System.Collections.Generic;

namespace AerovelenceMod.Content.Projectiles.Other
{
    public class StarParticleHandler : ModProjectile
    {
		public override string Texture => "Terraria/Images/Projectile_0";

		int timer = 0;
		public float size = 1f;
		public float fadeMult = 1f;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Mura");
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
		public Color col = Color.HotPink;
		public float alpha = 1f;
		public float scale = 1f;
		public override void AI()
        {

			if (timer > 45)
            {
				scale = Math.Clamp(MathHelper.Lerp(scale, -0.75f, 0.04f * fadeMult), 0, 1);
				alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.2f, 0.03f * fadeMult), 0, 1);

				if (scale == 0)
					Projectile.active = false;
            }

			if (timer > 10)
				Projectile.velocity *= 0.95f;
			else
				Projectile.velocity *= 0.99f;
			timer++;

			Projectile.rotation += Projectile.velocity.X * 0.015f;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			
			Texture2D Glow = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/star_01").Value;
			Texture2D Pixel = Mod.Assets.Request<Texture2D>("Assets/TrailImages/GlowStarPMA").Value;

			Texture2D Line = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Starlight").Value;
			Texture2D GlowAlt = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;
			Texture2D GlowAltAlt = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_2").Value;

			//C
			/*
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

			Main.spriteBatch.Draw(GlowAltAlt, Projectile.Center - Main.screenPosition, null, col with { A = 255 } * alpha * 0.5f, Projectile.rotation, GlowAltAlt.Size() / 2, scale * 0.3f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(GlowAltAlt, Projectile.Center - Main.screenPosition, null, col with { A = 255 } * alpha, Projectile.rotation, GlowAltAlt.Size() / 2, scale * 0.2f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(GlowAltAlt, Projectile.Center - Main.screenPosition, null, Color.White with { A = 255 } * alpha * 0.7f, Projectile.rotation, GlowAltAlt.Size() / 2, scale * 0.15f, SpriteEffects.None, 0f);
			
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

			Main.spriteBatch.Draw(Line, Projectile.Center - Main.screenPosition, null, Color.Lerp(col, Color.White, 0.25f) with { A = 0 } * alpha * 0.7f, Projectile.rotation + MathHelper.ToRadians(-45), Line.Size() / 2, new Vector2(scale * 1.5f, scale * 0.5f), SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Line, Projectile.Center - Main.screenPosition, null, Color.Lerp(col, Color.White, 0.25f) with { A = 0 } * alpha * 0.7f, Projectile.rotation + MathHelper.ToRadians(45), Line.Size() / 2, new Vector2(scale * 1.5f, scale * 0.5f), SpriteEffects.None, 0f);
			

			return false;
			*/

			//B
			/*
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

			Main.spriteBatch.Draw(GlowAlt, Projectile.Center - Main.screenPosition, null, col with { A = 255 } * alpha * 0.5f, Projectile.rotation, GlowAlt.Size() / 2, scale * 0.25f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(GlowAlt, Projectile.Center - Main.screenPosition, null, col with { A = 255 } * alpha, Projectile.rotation, GlowAlt.Size() / 2, scale * 0.15f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(GlowAlt, Projectile.Center - Main.screenPosition, null, Color.White with { A = 255 } * alpha * 0.7f, Projectile.rotation, GlowAlt.Size() / 2, scale * 0.1f, SpriteEffects.None, 0f);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

			Main.spriteBatch.Draw(Line, Projectile.Center - Main.screenPosition, null, Color.Lerp(col, Color.White, 0.25f) with { A = 0 } * alpha * 0.7f, Projectile.rotation, Line.Size() / 2, new Vector2(scale * 1.5f, scale * 0.5f), SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Line, Projectile.Center - Main.screenPosition, null, Color.Lerp(col, Color.White, 0.25f) with { A = 0 } * alpha * 0.7f, Projectile.rotation + MathHelper.ToRadians(90), Line.Size() / 2, new Vector2(scale * 1.5f, scale * 0.5f), SpriteEffects.None, 0f);

			return false;
			*/

			//A
			Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition, null, col with { A = 0 } * alpha * 0.5f, Projectile.rotation, Glow.Size() / 2, scale * 0.25f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition, null, col with { A = 0 } * alpha, Projectile.rotation, Glow.Size() / 2, scale * 0.15f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * alpha * 0.7f, Projectile.rotation, Glow.Size() / 2, scale * 0.1f, SpriteEffects.None, 0f);

			Main.spriteBatch.Draw(Pixel, Projectile.Center - Main.screenPosition, null, Color.Lerp(col, Color.White, 0.25f) with { A = 0 } * alpha * 0.7f, Projectile.rotation, Pixel.Size() / 2, scale * 0.35f, SpriteEffects.None, 0f);


			return false;
		}



    }

}
