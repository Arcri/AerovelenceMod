using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
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
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;

namespace AerovelenceMod.Content.Dusts.GlowDusts
{
	public class HighResSmoke : ModDust
	{
		public override string Texture => "AerovelenceMod/Assets/Orbs/SoftGlow";

		public override void OnSpawn(Dust dust)
		{
			dust.frame = new Rectangle(0, 0, 512, 512);

			//FADEIN IS USED AS THE SMOKE'S ALPHA
			dust.fadeIn = 1f;

			//ALPHA IS USED AS A TIMER
			dust.alpha = 0;
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool Update(Dust dust)
		{
			if (dust.customData != null)
			{
				if (dust.customData is HighResSmokeBehavior behavior)
				{
                    if (dust.alpha == 0)
                    {
                        behavior.randomSmokeNumber = Main.rand.NextBool() ? 1 : 2;

                        dust.rotation = Main.rand.NextFloat(6.28f);
                        behavior.rotMult = Main.rand.NextFloat(-1f, 1f);
                    }

                    if (dust.alpha >= behavior.frameToStartFade)
                    {
                        //This float cast is STRICTLY necessary 
                        float prog = Math.Clamp((dust.alpha - (float)behavior.frameToStartFade) / behavior.fadeDuration, 0f, 1f);
                        dust.fadeIn = MathHelper.Lerp(1f, 0f, prog);
                    }

                    dust.rotation += (0.04f * behavior.rotMult);

                    dust.scale *= 0.99f;

                    dust.velocity *= behavior.velSlowAmount;

                    dust.position += dust.velocity;
                    dust.alpha++;
                }
			}
			else
			{
                if (dust.alpha == 0)
                {
                    dust.rotation = Main.rand.NextFloat(6.28f);
                    //rotMult = Main.rand.NextFloat(-1f, 1f);
                }

                if (dust.alpha >= 5)
                {
                    float prog = Math.Clamp((dust.alpha - 5f) / 25, 0f, 1f);
                    dust.fadeIn = MathHelper.Lerp(1f, 0f, prog);
                }

                dust.rotation += (0.02f);

                dust.scale *= 0.99f;

                dust.position += dust.velocity;

                dust.alpha++;
            }

            if (dust.fadeIn == 0)
                dust.active = false;

			return false;
		}


		public override bool PreDraw(Dust dust)
		{
			if (dust.customData != null)
			{
				if (dust.customData is HighResSmokeBehavior behavior)
				{
                    Texture2D Smoke = Mod.Assets.Request<Texture2D>("Assets/Smoke/smoke_0" + behavior.randomSmokeNumber).Value;
                    Texture2D ExtraGlow = Mod.Assets.Request<Texture2D>("Assets/Orbs/SoftGlow").Value;

                    float myscale = dust.scale * 0.25f;
                    Color col = dust.color * dust.fadeIn * behavior.overallAlpha;

                    //Yes the extra '* dust.fadeIn' is intentional
                    if (behavior.drawSoftGlowUnder) 
						Main.EntitySpriteDraw(ExtraGlow, dust.position - Main.screenPosition, null, col with { A = 0 } * 0.1f * behavior.softGlowIntensity * dust.fadeIn, dust.rotation, ExtraGlow.Size() / 2f, myscale * 0.5f, SpriteEffects.None);
                    
					Main.EntitySpriteDraw(Smoke, dust.position - Main.screenPosition, null, col with { A = 0 } * 0.2f, dust.rotation, Smoke.Size() / 2f, myscale, SpriteEffects.None);
                }
            }
			else
			{
                Texture2D Smoke = Mod.Assets.Request<Texture2D>("Assets/Smoke/smoke_0" + 1).Value;
                Texture2D ExtraGlow = Mod.Assets.Request<Texture2D>("Assets/Orbs/SoftGlow").Value;

                float myscale = dust.scale * 0.25f;

                Color col = dust.color * dust.fadeIn;

                //Yes the extra '* dust.fadeIn' is intentional
                Main.EntitySpriteDraw(ExtraGlow, dust.position - Main.screenPosition, null, col with { A = 0 } * 0.1f * dust.fadeIn, dust.rotation, ExtraGlow.Size() / 2f, myscale * 0.5f, SpriteEffects.None);
                
				Main.EntitySpriteDraw(Smoke, dust.position - Main.screenPosition, null, col with { A = 0 } * 0.2f, dust.rotation, Smoke.Size() / 2f, myscale, SpriteEffects.None);
            }
			return false;
        }

	}

	public class HighResSmokeBehavior
	{
        public float overallAlpha = 1f;
        public bool drawSoftGlowUnder = true;
        public float softGlowIntensity = 1f;

        public int randomSmokeNumber = 0;

        public int frameToStartFade = 5;
        public int fadeDuration = 25;
		public float velSlowAmount = 1f;

        public float rotMult = 0f;
    }

}