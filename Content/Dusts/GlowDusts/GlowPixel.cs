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
	/*
	public class GlowPixel : ModDust
    {
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/PixelGlow2";

		private bool rotationDir;
		public override void OnSpawn(Dust dust)
        {
			dust.customData = false;
			rotationDir = Main.rand.NextBool();
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 64, 64);
			dust.shader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
        }

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool Update(Dust dust)
        {
			if (dust.customData is false)
			{
				dust.position = dust.position + new Vector2(-32, -32) * dust.scale; //Vector2.One * 64 * dust.scale;
				dust.alpha = dust.color.A;
				dust.customData = true;
			}

			Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 32 * dust.scale;

			if (dust.noGravity)
				dust.scale *= 1 - 0.04f;
			else
				dust.scale *= 0.98f;

			//Vector2 nextCenter = dust.position + dust.velocity; + Vector2.One.RotatedBy(dust.rotation + (rotationDir ? 0.03f : -0.03f))  * 32 * dust.scale; 

			//dust.rotation += rotationDir ? 0.03f : -0.03f; //.06
			//dust.position += currentCenter - nextCenter;

			dust.position += dust.velocity; //Idk why we have to do this ourselves
			dust.position += new Vector2(1, 1) * dust.scale;
			//dust.position = dust.position + new Vector2(-32, -32) * dust.scale;

			dust.velocity *= 0.94f;

			if (!dust.noLight && dust.scale > 0.2f)
				Lighting.AddLight(dust.position, dust.color.R * dust.scale * 0.005f, dust.color.G * dust.scale * 0.005f, dust.color.B * dust.scale * 0.005f);

			dust.shader.UseColor(dust.color);

			//255 is base, set to literally anything else to have dust fade
			if (dust.alpha != 255)
				dust.color *= 0.95f;


			if (dust.scale < 0.05f)
			{
				dust.active = false;
			}

			return false;
		}

    }
	*/
	public class GlowPixel : ModDust
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/PixelGlow";

		public override void OnSpawn(Dust dust)
		{
			dust.customData = false;
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 64, 64);
			//dust.rotation = Main.rand.NextFloat(6.28f);
			//dust.shader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool Update(Dust dust)
		{
			
			if (dust.customData is false)
			{
				dust.position = dust.position + (new Vector2(-32, -32) * dust.scale); //Vector2.One * 64 * dust.scale;
				//dust.alpha = dust.color.A;
				dust.customData = true;
				//dust.rotation = Main.rand.NextFloat(6.28f);
			}


			dust.color.A = 0;
			dust.scale *= 0.94f;
			dust.position += dust.velocity; //Idk why we have to do this ourselves

			//Makes shit more accurate to velocity, idfk why the value is pi/2
			dust.position += new Vector2(1.57f, 1.57f) * dust.scale;
			dust.velocity *= 0.95f;

			if (!dust.noLight && dust.scale > 0.2f)
				Lighting.AddLight(dust.position, dust.color.R * dust.scale * 0.002f, dust.color.G * dust.scale * 0.002f, dust.color.B * dust.scale * 0.002f);


			if (dust.alpha != 0)
				dust.color *= 0.95f;

			if (dust.scale < 0.05f)
			{
				dust.active = false;
			}

			dust.rotation += dust.velocity.X * 0.01f;

			return false;
		}

	}

	public class GlowPixelFast : GlowPixel
    {
		public override bool Update(Dust dust)
		{

			if (dust.customData is false)
			{
				dust.position = dust.position + (new Vector2(-32, -32) * dust.scale);
				dust.customData = true;
			}

			dust.color.A = 0;
			dust.scale *= 0.95f;
			dust.position += dust.velocity; 

			//Makes shit more accurate to velocity, idfk why the value is pi/2
			dust.position += new Vector2(1.57f, 1.57f) * dust.scale;
			dust.velocity *= 0.99f;

			if (!dust.noLight && dust.scale > 0.2f)
				Lighting.AddLight(dust.position, dust.color.R * dust.scale * 0.002f, dust.color.G * dust.scale * 0.002f, dust.color.B * dust.scale * 0.002f);

			
			if (dust.alpha != 0)
				dust.color *= 0.88f;
			

			if (dust.fadeIn >= 60)
				dust.active = false;
			dust.fadeIn++;

			dust.rotation += dust.velocity.X * 0.01f;

			return false;
		}
	}

	public class GlowPixelAlts : ModDust
    {
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/PixelGlowShapes";

		public override void OnSpawn(Dust dust)
		{
			Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Dusts/GlowDusts/DustTextures/PixelGlowShapes").Value;

			dust.customData = false;
			dust.noGravity = true;
			//dust.frame = new Rectangle(0, 0, 64, 64);
			dust.frame = new Rectangle(0, texture.Height / 5 * Main.rand.Next(5), texture.Width, texture.Height / 5);

		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool Update(Dust dust)
		{

			if (dust.customData is false)
			{
				dust.position = dust.position + (new Vector2(-34, -34) * dust.scale); 
				dust.customData = true;
			}


			dust.color.A = 0;
			dust.scale *= 0.94f;
			dust.position += dust.velocity; 

			//Makes shit more accurate to velocity, idfk why the value is pi/2
			dust.position += new Vector2(1.57f, 1.57f) * dust.scale;
			dust.velocity *= 0.95f;

			if (!dust.noLight && dust.scale > 0.2f)
				Lighting.AddLight(dust.position, dust.color.R * dust.scale * 0.002f, dust.color.G * dust.scale * 0.002f, dust.color.B * dust.scale * 0.002f);


			if (dust.alpha != 0)
				dust.color *= 0.95f;

			if (dust.scale < 0.05f)
			{
				dust.active = false;
			}

			dust.rotation += dust.velocity.X * 0.01f;

			return false;
		}

	}

	public class GlowPixelRise : GlowPixelAlts
    {
        public override bool Update(Dust dust)
        {
			if (dust.customData is false)
			{
				dust.position = dust.position + (new Vector2(-34, -34) * dust.scale);
				dust.customData = true;
			}

			dust.velocity.Y -= 0.05f;

			dust.color.A = 0;
			dust.scale *= 0.94f;
			dust.position += dust.velocity;

			//Makes shit more accurate to velocity, idfk why the value is pi/2
			dust.position += new Vector2(1.57f, 1.57f) * dust.scale;
			dust.velocity *= 0.95f;

			if (!dust.noLight && dust.scale > 0.2f)
				Lighting.AddLight(dust.position, dust.color.R * dust.scale * 0.002f, dust.color.G * dust.scale * 0.002f, dust.color.B * dust.scale * 0.002f);


			if (dust.alpha != 0)
				dust.color *= 0.95f;

			if (dust.scale < 0.05f)
			{
				dust.active = false;
			}

			dust.rotation += dust.velocity.X * 0.01f;

			if (dust.fadeIn == 200)
				dust.active = false;
			dust.fadeIn++;
			return false;
        }
    }


	public class GlowPixelCross : ModDust
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/PixelCrossMain";
		private Texture2D core;


		public override void Load() => core = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/PixelCrossInner");

		public override void Unload() => core = null;

        public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 68, 68);
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool Update(Dust dust)
		{
			
			if (dust.customData != null)
			{
				if (dust.customData is GlowPixelCrossBehavior behavior)
				{

					//if instead of switch for readability now, maybe change later
					if (behavior.behaviorToUse == GlowPixelCrossBehavior.Behavior.Base)
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
							dust.color *= behavior.base_colorFadePower;

						dust.fadeIn++;
					}

					else if (behavior.behaviorToUse == GlowPixelCrossBehavior.Behavior.PlaceHolder1)
					{

					}

					else if (behavior.behaviorToUse == GlowPixelCrossBehavior.Behavior.PlaceHolder2)
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
			Texture2D Core = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/PixelCrossInner");
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 }, dust.rotation, new Vector2(34f, 34f), dust.scale, SpriteEffects.None, 0f);
			
			Main.spriteBatch.Draw(Core, dust.position - Main.screenPosition, null, Color.White with { A = 0 }, dust.rotation, new Vector2(34, 34f), dust.scale * 0.5f, SpriteEffects.None, 0f);

			return false;
		}

	}

	public class GlowPixelCrossBehavior
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