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

namespace AerovelenceMod.Content.Dusts.GlowDusts
{
	
	public class GlowStrong : ModDust
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/GlorbStrong";

		public override void OnSpawn(Dust dust)
		{
			dust.fadeIn = 1f;
			dust.customData = false;
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 120, 120);
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool Update(Dust dust)
		{
			
			if (dust.customData is false)
			{
				dust.position = dust.position + (new Vector2(-60, -60) * dust.scale); 
				dust.customData = true;
			}


			dust.color.A = 0;
			dust.scale *= 0.94f;
			dust.position += dust.velocity; 

			//Makes shit more accurate to velocity, idfk why the value is pi
			dust.position += new Vector2(MathHelper.Pi) * dust.scale;
			dust.velocity *= 0.95f;

			if (!dust.noLight && dust.scale > 0.2f)
				Lighting.AddLight(dust.position, dust.color.R * dust.scale * 0.002f, dust.color.G * dust.scale * 0.002f, dust.color.B * dust.scale * 0.002f);


			if (dust.alpha != 0)
            {
				dust.fadeIn *= 0.95f;				
				dust.color *= 0.95f;
            }

			if (dust.scale < 0.05f)
			{
				dust.active = false;
			}

			//dust.rotation += dust.velocity.X * 0.01f;

			dust.fadeIn *= 0.93f;
			dust.color *= 0.93f;

			return false;
		}


		public override bool PreDraw(Dust dust)
		{
			Vector2 offset = new Vector2(116, 116) * dust.scale;
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition + offset, dust.frame, Color.Black * 0.2f, dust.rotation, new Vector2(120f, 120f), dust.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition + offset, dust.frame, dust.color with { A = 0 }, dust.rotation, new Vector2(120f, 120f), dust.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition + offset * 0.5f + new Vector2(-2,-2) * dust.scale, dust.frame, Color.White with { A = 0 } * 1f, dust.rotation, new Vector2(120f, 120f) * 0.5f, dust.scale * 0.5f, SpriteEffects.None, 0f);
            
			// - (new Vector2(-60, -60) * dust.scale)
            return true;
		}
	}

	public class HitFlare : ModDust 
	{
        public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/flare_1tiny";

        public override void OnSpawn(Dust dust)
        {
            dust.customData = false;
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 128, 128);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {

            if (dust.customData is false)
            {
                dust.position = dust.position + (new Vector2(-64, -64) * dust.scale * 0.5f);
                dust.customData = true;
            }


            dust.color.A = 0;


            dust.scale = Math.Clamp(MathHelper.Lerp(dust.scale, -0.5f, 0.08f), 0, 10);

            //dust.rotation += 0.2f;

            dust.position += dust.velocity;

            //Makes shit more accurate to velocity, idfk why the value is pi
            dust.position += new Vector2(MathHelper.Pi) * dust.scale;
            dust.velocity *= 0.95f;

            if (!dust.noLight && dust.scale > 0.2f)
                Lighting.AddLight(dust.position, dust.color.R * dust.scale * 0.002f, dust.color.G * dust.scale * 0.002f, dust.color.B * dust.scale * 0.002f);



            if (dust.scale < 0.05f)
            {
                dust.active = false;
            }

            //dust.rotation += dust.velocity.X * 0.01f;

            return false;
        }


        public override bool PreDraw(Dust dust)
        {
            Vector2 offset = new Vector2(64, 64) * dust.scale * 0.5f;

            Vector2 newOffset = new Vector2(16, 16) * (1 - dust.scale);

            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition + offset + newOffset, dust.frame, Color.Black * 0.2f, dust.rotation, new Vector2(64f, 64f), dust.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition + offset + newOffset, dust.frame, dust.color with { A = 0 }, dust.rotation * -1, new Vector2(64f, 64f), dust.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition + offset * 0.5f + newOffset, dust.frame, Color.White with { A = 0 } * 1f, dust.rotation, new Vector2(64f, 64f) * 0.5f, dust.scale * 0.5f, SpriteEffects.None, 0f);

            // - (new Vector2(-60, -60) * dust.scale)
            return false;
        }
    }


	public class GlowStarSharp : ModDust
	{
		public override string Texture => "AerovelenceMod/Assets/TrailImages/CrispStarPMA";
		private Texture2D circleGlow;


		public override void Load() => circleGlow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/PartiGlowPMA");

		public override void Unload() => circleGlow = null;

		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.alpha = 255;
			dust.frame = new Rectangle(0, 0, 120, 120);
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool Update(Dust dust)
		{

			if (dust.customData != null)
			{
				if (dust.customData is GlowStarSharpBehavior behavior)
				{

					//if instead of switch for readability now, maybe change later
					if (behavior.behaviorToUse == GlowStarSharpBehavior.Behavior.Base)
					{
						dust.position += dust.velocity;
						dust.rotation += dust.velocity.X * behavior.base_rotPower;

						dust.velocity *= dust.fadeIn < behavior.base_timeBeforeSlow ? behavior.base_preSlowPower : behavior.base_postSlowPower;
						if (dust.velocity.Length() < behavior.base_velToBeginShrink)
						{
							dust.scale *= behavior.base_fadePower;
						}

						if (dust.scale < 0.1f)
						{
							dust.active = false;
						}

						if (behavior.base_shouldFadeColor)
                        {
							dust.alpha = (int)(dust.alpha * behavior.base_colorFadePower);
							dust.color *= behavior.base_colorFadePower;
						}

						dust.fadeIn++;
					}

					else if (behavior.behaviorToUse == GlowStarSharpBehavior.Behavior.PlaceHolder1)
					{

					}

					else if (behavior.behaviorToUse == GlowStarSharpBehavior.Behavior.PlaceHolder2)
					{

					}
				}
			}
			else
			{
				dust.position += dust.velocity;
				dust.rotation += dust.velocity.X * 0.15f;


				dust.velocity *= dust.fadeIn < 3 ? 0.99f : 0.92f;
				if (dust.velocity.Length() < 1f)
				{
					dust.scale *= 0.9f;
				}


				if (dust.scale < 0.1f)
				{
					dust.active = false;
				}

				dust.fadeIn++;
			}

			return false;
		}


		public override bool PreDraw(Dust dust)
		{
			Color White = Color.White with { A = 0 } * (dust.alpha / 255f);
			Color Black = Color.Black * (dust.alpha / 255f);

			if (dust.customData != null)
			{
				if (dust.customData is GlowStarSharpBehavior behavior)
				{
					if (behavior.DrawOrb)
                    {
						//todo not this
						Texture2D Core = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/PartiGlowPMA");

						Color orbCol = behavior.OrbBlack ? Black : dust.color with { A = 0 };
						Main.spriteBatch.Draw(Core, dust.position - Main.screenPosition, null, orbCol * 0.07f * behavior.OrbIntensity, dust.rotation, new Vector2(60f, 60f), dust.scale * 2f, SpriteEffects.None, 0f);
					}

					if (behavior.DrawBlackUnder)
					{
						Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, Black * 0.3f, dust.rotation, new Vector2(60f, 60f), dust.scale * 1f, SpriteEffects.None, 0f);
					}

					Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 }, dust.rotation, new Vector2(60f, 60f), dust.scale * 1f, SpriteEffects.None, 0f);

					if (behavior.DrawWhiteCore)
                    {
						Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, White, dust.rotation, new Vector2(60f, 60f), dust.scale * 0.5f, SpriteEffects.None, 0f);
					}
					if (behavior.DrawBlackCore)
                    {
						Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, Black, dust.rotation, new Vector2(60f, 60f), dust.scale * 0.5f, SpriteEffects.None, 0f);
					}

				}
			}
            else
            {
				Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 }, dust.rotation, new Vector2(60f, 60f), dust.scale * 1f, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, White, dust.rotation, new Vector2(60f, 60f), dust.scale * 0.5f, SpriteEffects.None, 0f);
			}
			return false;
		}

	}

	public class GlowStarSharpBehavior
	{
		public Behavior behaviorToUse = Behavior.Base;
		//Default behavoir is Base with preset values
		public enum Behavior
		{
			Base = 0,
			PlaceHolder1 = 1,
			PlaceHolder2 = 2,
			PlaceHolder3 = 3,
		}

		public enum DrawBehavior
		{
			Basic = 0,
			CircleGlow = 1,
			PlaceHolder2 = 2,
			PlaceHolder3 = 3,
		}

		public bool DrawWhiteCore = true;
		public bool DrawBlackCore = false;
		public bool DrawBlackUnder = false;

		public bool DrawOrb = false;
		public bool OrbBlack = false;
		public float OrbIntensity = 1f;

		//Using this format so when you type in "base_" it will show you all of the options for that behavior, lets see if I end up regreting this

		//Base 
		public float base_rotPower = 0.15f;
		public int base_timeBeforeSlow = 3;
		public float base_preSlowPower = 0.99f;
		public float base_postSlowPower = 0.92f;
		public float base_velToBeginShrink = 1f;
		public float base_fadePower = 0.95f;

		public bool base_shouldFadeColor = false;
		public float base_colorFadePower = 0.93f;

		/////////////////////


	}

}