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
	
	public class LineSpark : ModDust
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/GlowLine1Black";


		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.alpha = 255;
			dust.frame = new Rectangle(0, 0, 128, 27);
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool Update(Dust dust)
		{

			if (dust.customData != null)
			{
				if (dust.customData is LineSparkBehavior behavior)
				{

                    dust.rotation = dust.velocity.ToRotation();

                    dust.velocity *= behavior.base_velFadePower;
                    dust.position += dust.velocity;

                    if (dust.fadeIn > behavior.base_killEarlyTime)
                        dust.active = false;

                    if (dust.fadeIn < behavior.base_timeToStartShrink)
                    {
                        dust.scale *= behavior.base_preShrinkPower;
                    }
                    else
                    {
                        dust.scale *= behavior.base_postShrinkPower;
                    }

                    if (dust.scale < 0.03f)
                    {
                        dust.active = false;
                    }

                    dust.fadeIn++;
                }
			}
			else
			{
                dust.rotation = dust.velocity.ToRotation();

                dust.velocity *= 0.97f;
                dust.position += dust.velocity;

                if (dust.noLight == false)
                {
                    if (dust.fadeIn > 60)
                        dust.active = false;
                }
                else
                {
                    if (dust.fadeIn < 40)
                    {
                        dust.scale *= 0.99f;
                    }
                    else
                    {
                        dust.scale *= 0.97f;
                    }

                    if (dust.scale < 0.03f)
                    {
                        dust.active = false;
                    }
                }

                //velFadeSpeed = 0.97f
                //time to start fade = 40
                //pre fade strength = 0.99
                //post fade strength = 0.97f
                //kill early time 60


                dust.fadeIn++;
            }

			return false;
		}


		public override bool PreDraw(Dust dust)
		{
			Color White = Color.White with { A = 0 } * (dust.alpha / 255f);
			Color Black = Color.Black * (dust.alpha / 255f);
			Texture2D tex = Texture2D.Value;

            if (dust.customData != null)
			{
				if (dust.customData is LineSparkBehavior behavior)
				{
					Vector2 scale = behavior.Vector2DrawScale * dust.scale;

                    Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, dust.color with { A = 0 }, dust.rotation, tex.Size() / 2f, scale * 1f, SpriteEffects.None, 0f);
                    
					if (behavior.DrawWhiteCore)
						Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, White with { A = 0 } * 1f, dust.rotation, tex.Size() / 2f, scale * 0.5f, SpriteEffects.None, 0f);
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

	public class LineSparkBehavior
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
		public Vector2 Vector2DrawScale = new Vector2(1f, 1f);
        //public bool DrawBlackCore = false;
        //public bool DrawBlackUnder = false;


        //Using this format so when you type in "base_" it will show you all of the options for that behavior, lets see if I end up regreting this

        //Base 
        public float base_velFadePower = 0.97f;
		public float base_preShrinkPower = 0.99f;
		public float base_postShrinkPower = 0.92f;
		public int  base_timeToStartShrink = 40;
		public int base_killEarlyTime = 60;

		public bool base_shouldFadeColor = false;
		public float base_colorFadePower = 0.93f;

		/////////////////////


	}

}