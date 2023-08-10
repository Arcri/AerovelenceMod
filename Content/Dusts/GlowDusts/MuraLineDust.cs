using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
using AerovelenceMod.Effects.Dyes;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace AerovelenceMod.Content.Dusts.GlowDusts
{
	
	public class MuraLineDust : ModDust
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/MuraLine120x120";

		public override void OnSpawn(Dust dust)
		{
			dust.customData = false;
			dust.noGravity = true;
			//dust.frame = new Rectangle(0, 0, 120, 120);
			dust.fadeIn = 1f;
			dust.scale = 0;
            dust.frame = new Rectangle(0, 0, 38, 14);
        }

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool Update(Dust dust)
		{
			
			if (dust.customData is false)
			{
                dust.position -= new Vector2(19, 7) * dust.scale;
				dust.customData = true;
			}


            dust.rotation = dust.velocity.ToRotation();
            dust.position += dust.velocity;

            dust.velocity *= 0.95f;


            dust.color.A = 0;

            dust.scale = MathHelper.Clamp(MathHelper.Lerp(dust.scale, 1.2f, 0.05f), 0f, 0.5f);


			if (dust.alpha > 15)
			{
				dust.fadeIn = Math.Clamp(MathHelper.Lerp(dust.fadeIn, -0.5f, 0.05f), 0, 1);
			}

			if (dust.fadeIn <= 0)
				dust.active = false;

			dust.alpha++;

            return false;
		}


		public override bool PreDraw(Dust dust)
		{

			Vector2 vec2Scale = new Vector2(0.5f, 1f) * dust.scale;

            Vector2 offset = new Vector2(38, 14f) * dust.scale * 0f;
            //Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition + offset, dust.frame, Color.Black * 0.2f * dust.fadeIn, dust.rotation, new Vector2(120f, 120f), dust.scale, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition + offset, dust.frame, dust.color with { A = 0 } * dust.fadeIn, dust.rotation, new Vector2(120f, 120f), dust.scale, SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition + offset, null, Color.Black * 0.2f * dust.fadeIn, dust.rotation, new Vector2(60f, 60f), dust.scale, SpriteEffects.None, 0f);

			//I dont know why the FUCK the origin has to be (60,60) for the dust to line up I hate it here

            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition + offset, null, dust.color * dust.fadeIn, dust.rotation, new Vector2(60f, 60f), dust.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition + offset, null, Color.White with { A = 0 } * dust.fadeIn, dust.rotation, new Vector2(60, 60f), dust.scale * 0.5f, SpriteEffects.None, 0f);


            return false;
		}
	}

	public class MuraLineBasic : ModDust
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/MuraLine120x120";

		public override void OnSpawn(Dust dust)
		{
			dust.customData = false;
			dust.noGravity = true;
			dust.fadeIn = 1f;
			dust.frame = new Rectangle(0, 0, 38, 14);
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool Update(Dust dust)
		{

			if (dust.customData is false)
			{
				//dust.position -= new Vector2(19, 7) * dust.scale;
				dust.customData = true;
			}


			dust.rotation = dust.velocity.ToRotation();
			dust.position += dust.velocity;

			dust.velocity *= 0.97f;


			dust.color.A = 0;


			if (dust.alpha > 15)
			{
				dust.fadeIn = Math.Clamp(MathHelper.Lerp(dust.fadeIn, -0.5f, 0.08f), 0, 1);
			}

			if (dust.fadeIn <= 0)
				dust.active = false;

			dust.alpha++;

			return false;
		}


		public override bool PreDraw(Dust dust)
		{
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, dust.color * dust.fadeIn, dust.rotation, new Vector2(60f, 60f), dust.scale, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, Color.White with { A = 0 } * dust.fadeIn, dust.rotation, new Vector2(60, 60f), dust.scale * 0.5f, SpriteEffects.None, 0f);


			return false;
		}
	}

	public class ColorSpark : ModDust
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/ColorSpark";

		public override void OnSpawn(Dust dust)
		{
			dust.fadeIn = 0f;
			dust.frame = new Rectangle(0, 0, 30, 18);
		}

		public override bool Update(Dust dust)
		{
			if (dust.customData != null)
            {
				if (dust.customData is ColorSparkBehavior behavior)
                {
					dust.rotation = dust.velocity.ToRotation();
					dust.position += dust.velocity;

					if (behavior.slow)
						dust.velocity *= behavior.slowAmount;

					if (!dust.noGravity)
						dust.velocity.Y += behavior.gravityIntensity;

					if (dust.fadeIn > 1f)
						dust.color *= 0.93f;
					else
						dust.color *= 0.98f;

					dust.fadeIn += behavior.fadeInSpeed;

					if (dust.fadeIn >= 5)
						dust.active = false;

					dust.alpha++;

					if (dust.alpha > 60)
						dust.active = false;

					if (behavior.shrink)
						dust.scale *= behavior.shrinkAmount;
				}
            }
            else
            {
				//Default behavior 
				dust.rotation = dust.velocity.ToRotation();
				dust.position += dust.velocity;

				dust.velocity *= 0.95f;
				dust.velocity.Y += 0.24f;

				if (dust.fadeIn > 1f)
					dust.color *= 0.93f;
				else
					dust.color *= 0.98f;

				dust.fadeIn += 0.06f;

				if (dust.fadeIn >= 5)
					dust.active = false;

				dust.alpha++;

				if (dust.alpha > 60)
					dust.active = false;
			}


			return false;
		}


		public override bool PreDraw(Dust dust)
		{


			Color color = Color.Lerp(Color.White, dust.color, dust.fadeIn);

			//Main.spriteBatch.End();
			//Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, color with { A = 0 }, dust.rotation, new Vector2(15, 9), new Vector2(dust.scale * 0.75f, dust.scale * 0.35f), SpriteEffects.None, 0f);
			
			//Main.spriteBatch.End();
			//Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


			return false;
		}
	}

	public class ColorSparkBehavior
    {
		public float gravityIntensity = 0.15f;
		public bool slow = true;
		public float slowAmount = 0.95f;

		public bool shrink = false;
		public float shrinkAmount = 0.98f;

		public float fadeInSpeed = 0.06f;
	}
}