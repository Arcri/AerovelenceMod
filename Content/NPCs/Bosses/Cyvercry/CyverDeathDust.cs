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
	
	public class CyverDeathDust : ModDust
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

}