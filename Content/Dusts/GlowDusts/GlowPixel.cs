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
			dust.scale *= 0.97f;
			dust.position += dust.velocity; 

			//Makes shit more accurate to velocity, idfk why the value is pi/2
			dust.position += new Vector2(1.57f, 1.57f) * dust.scale;
			dust.velocity *= 0.99f;

			if (!dust.noLight && dust.scale > 0.2f)
				Lighting.AddLight(dust.position, dust.color.R * dust.scale * 0.002f, dust.color.G * dust.scale * 0.002f, dust.color.B * dust.scale * 0.002f);

			/*
			if (dust.alpha != 0)
				dust.color *= 0.99f;
			*/

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
}