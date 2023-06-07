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
				dust.color *= 0.95f;

			if (dust.scale < 0.05f)
			{
				dust.active = false;
			}

			//dust.rotation += dust.velocity.X * 0.01f;

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

}