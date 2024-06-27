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
using Steamworks;

namespace AerovelenceMod.Content.Dusts.GlowDusts
{
	
	public class SoftGlowDust : ModDust
	{
		public override string Texture => "AerovelenceMod/Assets/Orbs/SoftGlow";

		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.alpha = 255;
			dust.frame = new Rectangle(0, 0, 512, 512);
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool Update(Dust dust)
		{

			if (dust.customData != null)
			{
				if (dust.customData is SoftGlowDustBehavior behavior)
				{
                    if (dust.fadeIn > behavior.base_timeToStartFade)
                        dust.alpha = (int)(dust.alpha * behavior.base_fadeSpeed);

					if (dust.fadeIn > behavior.base_timeToChangeScale)
						dust.scale *= behavior.base_sizeChangeSpeed;


                    if (dust.scale <= 0.03f || dust.alpha <= 30)
                        dust.active = false;

                    if (dust.fadeIn >= behavior.base_timeToKill)
                        dust.active = false;

                    dust.fadeIn++;
                }
			}
			else
			{
                if (dust.fadeIn > 5)
				{
					dust.alpha = (int)(dust.alpha * 0.95f);
					dust.scale *= 0.95f;
				}

				if (dust.scale <= 0.03f || dust.alpha <= 30)
					dust.active = false;

				if (dust.fadeIn >= 60)
					dust.active = false;

                dust.fadeIn++;
            }

			return false;
		}


		public override bool PreDraw(Dust dust)
		{
			Color White = Color.White with { A = 0 } * (dust.alpha / 255f);
			Texture2D tex = Texture2D.Value;

            if (dust.customData != null)
			{
				if (dust.customData is SoftGlowDustBehavior behavior)
				{
					Vector2 scale = behavior.Vector2DrawScale * dust.scale;

                    Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * behavior.overallAlpha * (dust.alpha / 255f), dust.rotation, tex.Size() / 2f, scale * 1f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * behavior.overallAlpha * (dust.alpha / 255f), dust.rotation, tex.Size() / 2f, scale * 1f, SpriteEffects.None, 0f);

                    if (behavior.DrawWhiteCore)
						Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, White with { A = 0 } * behavior.overallAlpha * 0.75f, dust.rotation, tex.Size() / 2f, scale * 0.5f, SpriteEffects.None, 0f);
				}
			}
            else
            {
				Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, dust.color with { A = 0 }, dust.rotation, tex.Size() / 2f, dust.scale * 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, White with { A = 0 }, dust.rotation, tex.Size() / 2f, dust.scale * 0.65f, SpriteEffects.None, 0f);
            }
            return false;
		}

	}

	public class SoftGlowDustBehavior
	{
        //Default behavoir is Shrink with preset values
		/*
        public Behavior behaviorToUse = Behavior.Shrink;
		public enum Behavior
		{
			Shrink = 0,
			Grow = 1,
			JustFade = 2,
			PlaceHolder3 = 3,
		}
		*/

		public bool DrawWhiteCore = false;
		public Vector2 Vector2DrawScale = new Vector2(1f, 1f);
        public float overallAlpha = 1f;

        //Using this format so when you type in "base_" it will show you all of the options for that behavior, lets see if I end up regreting this
        //Base

        public float base_timeToStartFade = 5;
		public float base_timeToChangeScale = 5;
		public float base_fadeSpeed = 0.95f;
		public int base_timeToKill = 60;

		//Over 1 for grow, under 1 for shrink
		public float base_sizeChangeSpeed = 0.95f;

		/////////////////////

	}

}