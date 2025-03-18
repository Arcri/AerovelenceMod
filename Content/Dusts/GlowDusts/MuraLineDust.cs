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
            //dust.customData = false;
            dust.noGravity = true;

            dust.fadeIn = 1f;
            //dust.scale = 0;
            dust.frame = new Rectangle(0, 0, 38, 14);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) { return dust.color; }

        public override bool Update(Dust dust)
        {

            dust.rotation = dust.velocity.ToRotation();
            dust.position += dust.velocity;

            if (dust.customData is MuraLineBehavior mlb)
            {
                dust.scale *= mlb.sizeChangeSpeed;
                dust.velocity *= mlb.velFadeSpeed;
                if (!mlb.NoAlphaZero)
                    dust.color.A = 0;
            }
            else
            {
                dust.velocity *= 0.95f;
                dust.scale *= 0.98f;
                dust.color.A = 0;
            }

            //dust.scale = MathHelper.Clamp(MathHelper.Lerp(dust.scale, 1f, 0.025f), 0f, 0.5f);


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
            Vector2 vec2scale = new Vector2(1f, 1f) * dust.scale;
            float whiteI = 1f;

            if (dust.customData is MuraLineBehavior mlb)
            {
                vec2scale = mlb.XYscale * dust.scale;
                whiteI = mlb.whiteIntensity;
            }

            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, dust.color * dust.fadeIn, dust.rotation, new Vector2(60f, 60f), vec2scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, Color.White with { A = 0 } * dust.fadeIn * whiteI, dust.rotation, new Vector2(60, 60f), vec2scale * 0.5f, SpriteEffects.None, 0f);
            return false;
        }
    }

	public class MuraLineBasic : ModDust
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/MuraLine120x120";

        public override void OnSpawn(Dust dust)
        {
            //dust.customData = false;
            dust.noGravity = true;
            dust.fadeIn = 1f;
            dust.frame = new Rectangle(0, 0, 38, 14);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) { return dust.color; }

        public override bool Update(Dust dust)
        {

            dust.rotation = dust.velocity.ToRotation();
            dust.position += dust.velocity;

            if (dust.customData is MuraLineBehavior mlb)
            {
                dust.velocity *= mlb.velFadeSpeed;
                dust.scale *= mlb.sizeChangeSpeed;
            }
            else
            {
                dust.velocity *= 0.97f;
                dust.scale *= 1f;
            }

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
            Vector2 vec2scale = new Vector2(1f, 1f) * dust.scale;
            float whiteI = 1f;

            if (dust.customData is MuraLineBehavior mlb)
            {
                vec2scale = mlb.XYscale * dust.scale;
                whiteI = mlb.whiteIntensity;
            }


            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, dust.color * dust.fadeIn, dust.rotation, new Vector2(60f, 60f), vec2scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, Color.White with { A = 0 } * dust.fadeIn * whiteI, dust.rotation, new Vector2(60, 60f), vec2scale * 0.5f, SpriteEffects.None, 0f);

            return false;
        }
    }

    public class MuraLineBehavior
    {
        public Vector2 XYscale;

        public float velFadeSpeed = 0.97f;

        public float sizeChangeSpeed = 1f;

        public float whiteIntensity = 1f;

        public bool NoAlphaZero = false;
        public MuraLineBehavior(Vector2 xyscale)
        {
            XYscale = xyscale;
        }

        public MuraLineBehavior(Vector2 xyscale, float VelFadeSpeed = 0.97f, float SizeChangeSpeed = 1f, float WhiteIntensity = 1f)
        {
            XYscale = xyscale;
            velFadeSpeed = VelFadeSpeed;
            sizeChangeSpeed = SizeChangeSpeed;
            whiteIntensity = WhiteIntensity;
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