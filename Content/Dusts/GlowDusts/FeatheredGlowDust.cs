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
using Steamworks;

namespace AerovelenceMod.Content.Dusts.GlowDusts
{
	public class FeatheredGlowDust : ModDust
	{
		public override string Texture => "AerovelenceMod/Assets/Orbs/feather_circle128PMA";

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
				if (dust.customData is FeatheredGlowBehavior behavior)
				{
                    if (dust.fadeIn > behavior.base_timeToChangeAlpha)
                        dust.alpha = (int)(dust.alpha * behavior.base_alphaChangeSpeed);

					if (dust.fadeIn > behavior.base_timeToChangeScale)
						dust.scale *= behavior.base_scaleChangeSpeed;

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
				if (dust.customData is FeatheredGlowBehavior behavior)
				{
					Vector2 scale = behavior.Vector2DrawScale * dust.scale;

                    //Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * behavior.overallAlpha * (dust.alpha / 255f), dust.rotation, tex.Size() / 2f, scale * 1f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * behavior.overallAlpha * (dust.alpha / 255f), dust.rotation, tex.Size() / 2f, scale * 1f, SpriteEffects.None, 0f);

                    if (behavior.DrawWhiteCore)
						Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, White with { A = 0 } * behavior.overallAlpha, dust.rotation, tex.Size() / 2f, scale * 0.5f, SpriteEffects.None, 0f);
				}
			}
            else
            {
				Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, dust.color with { A = 0 }, dust.rotation, tex.Size() / 2f, dust.scale * 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, White with { A = 0 }, dust.rotation, tex.Size() / 2f, dust.scale * 0.5f, SpriteEffects.None, 0f);
            }
            return false;
		}

	}

	public class FeatheredGlowBehavior
	{
		public bool DrawWhiteCore = true;
		
		
		public Vector2 Vector2DrawScale = new Vector2(1f, 1f);
        public float overallAlpha = 1f;


        public float base_alphaChangeSpeed = 0.95f;
        public float base_timeToChangeAlpha = 4;

        //Over 1 for grow, under 1 for shrink
        public float base_scaleChangeSpeed = 0.95f;
        public float base_timeToChangeScale = 2;

		public int base_timeToKill = 30;



        public FeatheredGlowBehavior(float AlphaChangeSpeed = 0.95f, int timeToChangeAlpha = 4, float ScaleChangeSpeed = 0.95f, int timeToChangeScale = 2, 
			int timeToKill = 30, float OverallAlpha = 1f, float XScale = 1f, float YScale = 1f)
        {
            base_alphaChangeSpeed = AlphaChangeSpeed;
			base_timeToChangeAlpha = timeToChangeAlpha;

			base_scaleChangeSpeed = ScaleChangeSpeed;
			base_timeToChangeScale = timeToChangeScale;

			base_timeToKill = timeToKill;

			overallAlpha = OverallAlpha;
            Vector2DrawScale = new Vector2(XScale, YScale);
        }

    }

}