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


            //
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

}