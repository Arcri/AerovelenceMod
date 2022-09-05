using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

using static Terraria.ModLoader.ModContent;


using System.Collections.Generic;

using Terraria.ModLoader.IO;
using ReLogic.Content;
using AerovelenceMod.Effects.Dyes;

namespace AerovelenceMod.Content.Dusts.GlowDusts
{
	public class LineGlow : ModDust
    {
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/LineGlow";

		Vector3 initialColor = Color.White.ToVector3();
		Vector3 colorToUse = Color.White.ToVector3();
		public override void OnSpawn(Dust dust)
        {
			initialColor = dust.color.ToVector3();
			colorToUse = dust.color.ToVector3();
			dust.fadeIn = 0;
			dust.customData = dust.scale;

			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 128, 28);
			dust.shader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
        }

        public override bool Update(Dust dust)
        {
			if ((float)dust.customData != 0f)
			{
				dust.position -= new Vector2(64, 14) * dust.scale;
				dust.scale = (float)dust.customData;
				dust.customData = 0f;
			}

			dust.rotation = dust.velocity.ToRotation();


			dust.velocity *= 0.97f;
			dust.position += dust.velocity;

			colorToUse.X = Math.Clamp(colorToUse.X * 0.95f, initialColor.X * 1f, 2f);
			colorToUse.Y = Math.Clamp(colorToUse.Y * 0.95f, initialColor.Y * 1f, 2f);
			colorToUse.Z = Math.Clamp(colorToUse.Z * 0.95f, initialColor.Z * 1f, 2f);

			dust.color = new Color(colorToUse.X, colorToUse.Y, colorToUse.Z);

			Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.6f);
			
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

			dust.fadeIn++;
			//dust.color.R = Math.Clamp((float)dust.color.R, (float)initialColor.R * 0.5f, 2f);
			//dust.color *= 0.99f;

			/*
			if (dust.fadeIn <= 0)
				dust.shader.UseColor(Color.Transparent);
			else
				dust.shader.UseColor(dust.color);

			dust.fadeIn++;

			Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.6f);

			if (dust.fadeIn > 60)
				dust.active = false;
			*/
			/*
			 * 			Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.6f);
			dust.scale *= 0.96f;

			if (dust.scale < 0.03f)
			{
				dust.active = false;
			}
			 * */
			return false;
		}

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
			if (dust.fadeIn <= 2)
				return Color.Transparent;

			return dust.color * MathHelper.Min(1, dust.fadeIn / 20f);
		}
    }
}