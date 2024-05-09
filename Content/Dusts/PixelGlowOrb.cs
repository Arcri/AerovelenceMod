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
	public class PixelGlowOrb : ModDust
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/Textures/PixelGlowOrb";
		private Texture2D core;
        private Texture2D glow;

		public override void Load()
		{
			core = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Dusts/Textures/PixelGlowOrbInner");
            glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow64");
        }


        public override void Unload()
        {
			core = null;
			glow = null;
        } 

        public override void OnSpawn(Dust dust)
		{
			//Alpha is used as a timer in this dust
			dust.alpha = 0;

			//FadeIn is used as the opacity
			dust.fadeIn = 1f;

			dust.noGravity = true;
			dust.noLight = true;
			dust.frame = new Rectangle(0, 0, 16, 16);
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool Update(Dust dust)
		{
			if (dust.customData != null)
			{
				if (dust.customData is PixelGlowOrbBehavior behavior)
				{
                    dust.velocity *= dust.alpha < behavior.base_timeBeforeSlow ? behavior.base_preSlowPower : behavior.base_postSlowPower;
                    if (dust.velocity.Length() < behavior.base_velToBeginShrink)
                    {
                        dust.scale *= behavior.base_fadePower;
                        dust.fadeIn *= behavior.base_colorFadePower;
                    }

                    if (dust.scale < 0.1f)
                    {
                        dust.active = false;
                    }

					if (dust.alpha == behavior.base_killEarlyTime)
						dust.active = false;

                    float averageVel = (Math.Abs(dust.velocity.X) + Math.Abs(dust.velocity.Y)) / 2f;
                    dust.rotation += averageVel * behavior.base_rotPower;

                    dust.position += dust.velocity;



                    dust.alpha++;
                }
			}
            else
            {
				
				dust.velocity *= dust.alpha < 3 ? 0.99f : 0.92f;
				if (dust.velocity.Length() < 1f)
				{
                    dust.fadeIn *= 0.95f;
                    dust.scale *= 0.9f;
				}

				if (dust.scale < 0.1f)
				{
					dust.active = false;
				}


				float averageVel = (Math.Abs(dust.velocity.X) + Math.Abs(dust.velocity.Y)) / 2f;
                dust.rotation += averageVel * 0.05f;

                dust.position += dust.velocity;


                dust.alpha++;

            }

            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.5f * dust.scale);

            return false;
		}


		public override bool PreDraw(Dust dust)
		{
			Texture2D Core = (Texture2D)Request<Texture2D>("AerovelenceMod/Content/Dusts/Textures/PixelGlowOrbInner");
            Texture2D Outer = (Texture2D)Request<Texture2D>("AerovelenceMod/Content/Dusts/Textures/PixelGlowOrbOuter");
            Texture2D Glow = (Texture2D)Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow64");

			//Vector2 offset = new Vector2(-8f, -8f).RotatedBy(dust.rotation) * dust.scale;
			Vector2 drawPos = dust.position - Main.screenPosition;// + offset;

			if (dust.customData != null)
			{
				if (dust.customData is PixelGlowOrbBehavior behavior)
				{
					if (!behavior.base_dontDrawOrb)
						Main.EntitySpriteDraw(Glow, drawPos, null, dust.color with { A = 0 } * behavior.base_glowIntensity * dust.fadeIn, dust.rotation, Glow.Size() / 2f, dust.scale * 0.5f, SpriteEffects.None);

                    Main.EntitySpriteDraw(Outer, drawPos, null, dust.color with { A = 255 } * 0.25f * dust.fadeIn, dust.rotation, Outer.Size() / 2f, dust.scale, SpriteEffects.None);

                    Main.EntitySpriteDraw(Texture2D.Value, drawPos, null, dust.color with { A = 0 } * dust.fadeIn, dust.rotation, Core.Size() / 2f, dust.scale, SpriteEffects.None);

                    Main.EntitySpriteDraw(Core, drawPos, null, Color.White with { A = 0 } * dust.fadeIn, dust.rotation, Core.Size() / 2f, dust.scale * 0.9f, SpriteEffects.None);
                }

            }
			else
			{
                Main.EntitySpriteDraw(Glow, drawPos, null, dust.color with { A = 0 } * 0.3f * dust.fadeIn, dust.rotation, Glow.Size() / 2f, dust.scale * 0.5f, SpriteEffects.None);

                Main.EntitySpriteDraw(Outer, drawPos, null, dust.color with { A = 255 } * 0.25f * dust.fadeIn, dust.rotation, Outer.Size() / 2f, dust.scale, SpriteEffects.None);

                Main.EntitySpriteDraw(Texture2D.Value, drawPos, null, dust.color with { A = 0 } * dust.fadeIn, dust.rotation, Core.Size() / 2f, dust.scale, SpriteEffects.None);

                Main.EntitySpriteDraw(Core, drawPos, null, Color.White with { A = 0 } * dust.fadeIn, dust.rotation, Core.Size() / 2f, dust.scale * 0.9f, SpriteEffects.None);
            }

			

            //Main.spriteBatch.Draw(glow, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * 0.3f, dust.rotation, glow.Size() / 2f, dust.scale, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 }, dust.rotation, core.Size() / 2f, dust.scale, SpriteEffects.None, 0f);
			//Main.spriteBatch.Draw(core, dust.position - Main.screenPosition, null, Color.White with { A = 0 }, dust.rotation, core.Size() / 2f, dust.scale * 0.5f, SpriteEffects.None, 0f);

			return false;
		}

	}

	public class PixelGlowOrbBehavior
	{
		//Using this format so when you type in "base_" it will show you all of the options for that behavior, lets see if I end up regreting this

		//How much the dust rotates based on its velocity
		public float base_rotPower = 0.05f;

		public int base_timeBeforeSlow = 3;
		public float base_preSlowPower = 0.99f;
		public float base_postSlowPower = 0.92f;
		public float base_velToBeginShrink = 1f;
		public float base_fadePower = 0.95f;

		//Kills dust dust once it reaches this frame (-1 for never kill early)
		public int base_killEarlyTime = -1;

		//Drawing
		public float base_glowIntensity = 0.2f;
		public bool base_dontDrawOrb = false;

		public float base_colorFadePower = 0.95f;

	}
}