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
    public class MuraLineHandler : ModProjectile
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

		float alpha = 1f;
		public List<MuraLine> lines = new List<MuraLine>();
		public override void AI()
        {
			/*
			if (!spawnedLines)
			{
				for (int i = 0; i < 5; i++)
				{
					MuraLine newWind = new MuraLine(Projectile.Center, Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f)) * Main.rand.NextFloat(0.85f, 1.15f));
					Wind.Add(newWind);
				}
				spawnedWind = true;
			}
			*/
			foreach (MuraLine line in lines)
			{
				line.Update();
			}

			if (timer > 15)
            {
				alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.5f, 0.05f * fadeMult), 0, 1);

				if (alpha == 0)
					Projectile.active = false;
            }
			timer++;

		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/Projectiles/Other/MuraLineAlt").Value;

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			foreach (MuraLine line in lines)
			{
				line.DrawIce(Main.spriteBatch, Tex, alpha);
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

			return false;
		}



    }

	public class MuraLine
	{
		public Vector2 Velocity;
		public Vector2 Center;

		public float rotation;
		public Color color;
		public float scale;
		public float xScale = 2f;

		public int timer;
		public MuraLine(Vector2 pos, Vector2 vel, float scaleX)
		{
			Center = pos;
			Velocity = vel;
			scale = 1f; //0f
			rotation = vel.ToRotation();
			xScale = scaleX;
		}

		public void Update()
		{
			float size = 1f;
			scale = MathHelper.Clamp(MathHelper.Lerp(scale, size, 0.025f), 0f, size / 2);
			Center += Velocity;
			Velocity *= 0.95f;
			timer++;
		}


		public void DrawIce(SpriteBatch sb, Texture2D tex, float alpha)
		{
			sb.Draw(tex, Center - Main.screenPosition, null, color * alpha, rotation, tex.Size() / 2, new Vector2(xScale, 2.2f) * scale, SpriteEffects.None, 0f);
			sb.Draw(tex, Center - Main.screenPosition, null, color * alpha, rotation, tex.Size() / 2, new Vector2(xScale, 2.2f) * scale, SpriteEffects.None, 0f);
			//sb.Draw(tex, Center - Main.screenPosition, null, Color.OrangeRed * alpha, rotation, tex.Size() / 2, scale * 3, SpriteEffects.None, 0f);

		}

	}
}
