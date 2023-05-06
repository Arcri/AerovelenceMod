using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common.Utilities;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Audio;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{
	public class TeleportFXCyver : ModProjectile
	{
		private int timer;
		public override string Texture => "Terraria/Images/Projectile_0";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("ShadowBlade");
		}

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;

			Projectile.scale = 1f;
			Projectile.timeLeft = 700;

			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;

			Projectile.width = 1;
			Projectile.height = 1;

		}

        public override bool? CanDamage()
        {
			return false;
        }

		float mainAlpha = 1f;

		float starAlpha = 1f;
		public List<StarParticle> stars = new List<StarParticle>();
		bool spawnedStars = false;
		public override void AI()
		{
			/*
			
			if (!spawnedStars)
			{
				for (int i = 0; i < 5; i++)
				{
					StarParticle newStar = new StarParticle(Projectile.Center, Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f)) * Main.rand.NextFloat(0.85f, 1.15f));
					stars.Add(newStar);
				}
				spawnedStars = true;
			}

			bool anyStarsActive = false; 
			foreach (StarParticle star in stars)
			{
				star.Update();
			}

			if (timer > 15)
			{
				starAlpha = Math.Clamp(MathHelper.Lerp(alpha, -0.5f, 0.05f * fadeMult), 0, 1);

				if (starAlpha == 0)
					Projectile.active = false;
			}
			*/

			alpha = MathHelper.Clamp(alpha - 0.01f, 0, 1);

			scale = getProgress(timer * 0.02f);

			if (getProgress(timer * 0.02f) >= 0.9f)
				Projectile.active = false;

			timer++;

		}

		float scale = 1f;
		float alpha = 1f;
		public override bool PreDraw(ref Color lightColor)
        {
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			var Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/CyvercryGlowy").Value;
			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White * alpha, Projectile.rotation, Tex.Size() / 2, 1f * (1f - scale), SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White * alpha, Projectile.rotation, Tex.Size() / 2, 1f * (1f - scale), SpriteEffects.None, 0f);

			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Square(3,3), Tex.Frame(1, 1, 0, 0), Color.HotPink * 0.5f * alpha, Projectile.rotation, Tex.Size() / 2, 1.2f * (1f - scale), SpriteEffects.None, 0f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
        }

		public float getProgress(float input)
        {
			float toReturn;

			float c1 = 1.70158f;
			float c3 = c1 + 1f;

			toReturn =  c3 * input * input * input - c1 * input * input;
			return toReturn;
        }
    }

	public class StarParticle
	{
		public Vector2 Velocity;
		public Vector2 Center;

		public float rotation;
		public Color color;
		public float scale;
		public float alpha;

		public int timer;
		public StarParticle(Vector2 pos, Vector2 vel)
		{
			Center = pos;
			Velocity = vel;
			scale = 1f;
			rotation = vel.ToRotation();
		}

		public void Update()
		{
			float size = 1f;
			scale = MathHelper.Clamp(MathHelper.Lerp(scale, size, 0.025f), 0f, size / 2);
			Center += Velocity;
			Velocity *= 0.95f;
			rotation += Velocity.X * 0.03f;
			timer++;
		}


		public void DrawStar(SpriteBatch sb, Texture2D tex)
		{
			sb.Draw(tex, Center - Main.screenPosition, null, color * alpha, rotation, tex.Size() / 2, scale, SpriteEffects.None, 0f);
			sb.Draw(tex, Center - Main.screenPosition, null, color * alpha, rotation, tex.Size() / 2, scale, SpriteEffects.None, 0f);

		}

	}

	public class EyeFlash : ModProjectile
    {
		private int timer;
		public override string Texture => "Terraria/Images/Projectile_0";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("ShadowBlade");
		}

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;

			Projectile.scale = 1f;
			Projectile.timeLeft = 700;

			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;

			Projectile.width = 1;
			Projectile.height = 1;

		}

		public override bool? CanDamage()
		{
			return false;
		}

		float mainAlpha = 1f;

		float starAlpha = 1f;
		public List<StarParticle> stars = new List<StarParticle>();
		bool spawnedStars = false;
		public override void AI()
		{
			/*
			
			if (!spawnedStars)
			{
				for (int i = 0; i < 5; i++)
				{
					StarParticle newStar = new StarParticle(Projectile.Center, Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f)) * Main.rand.NextFloat(0.85f, 1.15f));
					stars.Add(newStar);
				}
				spawnedStars = true;
			}

			bool anyStarsActive = false; 
			foreach (StarParticle star in stars)
			{
				star.Update();
			}

			if (timer > 15)
			{
				starAlpha = Math.Clamp(MathHelper.Lerp(alpha, -0.5f, 0.05f * fadeMult), 0, 1);

				if (starAlpha == 0)
					Projectile.active = false;
			}
			*/
			timer++;

		}


		public override bool PreDraw(ref Color lightColor)
		{
			var Tex = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/ShadowBlade").Value;
			Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Tex.Size() / 2, 1f, SpriteEffects.None, 0f);

			return false;
		}
	}
}


