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
	public class GlowDustStrong : ModDust
    {

		public float fadeSpeed = 0f;
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/DotTell";

		public override void OnSpawn(Dust dust)
        {
			dust.fadeIn = 0;
			dust.customData = dust.scale;

			dust.frame = new Rectangle(0, 0, 64, 64);
			dust.shader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
        }

        public override bool Update(Dust dust)
        {

			//dust.rotation = dust.velocity.ToRotation();

			if (dust.customData is null)
			{
				dust.position += new Vector2(64,64) * dust.scale;
				dust.customData = true;
			}


			dust.velocity *= 0.98f;
			dust.position += dust.velocity;
			dust.scale *= (0.96f);

			if (dust.scale < 0.05f)
				dust.active = false;


			if (dust.noGravity)
            {
				if (dust.fadeIn > 60)
					dust.active = false;
				dust.fadeIn++;
			}

			if (!dust.noLight)
				Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.6f * dust.scale);

			return false;
		}

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
			return dust.color;
		}
    }

}