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
	
	public class SunParticle : ModDust
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/SunParticle";

		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.fadeIn = 1;
			dust.frame = new Rectangle(0, 0, 60, 60);
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool Update(Dust dust)
		{

			if (dust.customData != null)
			{
				if (dust.customData is SunParticleBehavior behavior)
				{
					int rotDir = dust.velocity.X > 0 ? 1 : -1;

					dust.rotation += dust.velocity.Length() * behavior.rotAmount * rotDir;
					dust.position += dust.velocity;

					if (behavior.slow)
						dust.velocity *= behavior.slowAmount;

					if (!dust.noGravity)
						dust.velocity.Y += behavior.gravityIntensity;

					if (behavior.fadeColor)
						dust.color *= behavior.colorFadeSpeed; 

					if (dust.alpha >= behavior.lifeTime)
						dust.active = false;

					if (behavior.shrink)
						dust.scale *= behavior.shrinkAmount;

					if (dust.scale <= 0.02f)
						dust.active = false;

					dust.alpha++;
				}
			}
			else
			{
				//Default behavior 
				int rotDir = dust.velocity.X > 0 ? 1 : -1;

				dust.rotation += dust.velocity.Length() * 0.02f * rotDir;
				dust.position += dust.velocity;

				if (dust.alpha > 15)
					dust.velocity *= 0.915f;
				else
					dust.velocity *= 0.96f;

				if (dust.alpha > 10)
                {
					dust.fadeIn *= 0.97f;
					dust.color *= 0.97f;
				}

				if (dust.alpha > 120)
					dust.active = false;

				if (dust.alpha > 40)
					dust.scale *= 0.92f;
				else
					dust.scale *= 0.98f;

				if (dust.scale <= 0.05f)
					dust.active = false;

				dust.alpha++;
			}

			return false;
		}


		public override bool PreDraw(Dust dust)
		{
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 }, dust.rotation, new Vector2(30, 30), dust.scale, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, Color.White with { A = 0 } * dust.fadeIn, dust.rotation, new Vector2(30, 30), dust.scale * 0.5f, SpriteEffects.None, 0f);

			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * 0.35f, dust.rotation, new Vector2(30, 30), dust.scale * 1.5f, SpriteEffects.None, 0f);


			return false;
		}
	}

	public class SunParticleBehavior
	{
		public float gravityIntensity = 0.15f;
		public bool slow = true;
		public float slowAmount = 0.95f;

		public bool shrink = false;
		public float shrinkAmount = 0.98f;

		public bool fadeColor = false;
		public float colorFadeSpeed = 0.93f;

		public int lifeTime = 80;

		//TBD
		public int timeToStartSlow = 0;
		public int timeToStartShrink = 0;
		public int timeToStartFade = 0;

		public float rotAmount = 0.02f;

	}
}