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
	public class GlowFlare : ModDust
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/GlowCircleFlare";

		public override void OnSpawn(Dust dust)
		{
            //noLightEmittence is used as a first-frame check in this dust
            dust.noLightEmittence = false;

            dust.frame = new Rectangle(0, 0, 64, 64);
            dust.fadeIn = 1f;
            dust.noGravity = true;
            dust.shader = new ArmorShaderData(new Ref<Effect>(AerovelenceMod.Instance.Assets.Request<Effect>("Effects/NewGlowDustShader").Value), "ArmorBasic");
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color * dust.fadeIn;
        }

        public override bool Update(Dust dust)
		{            
            if (!dust.noLightEmittence)
            {
                dust.position -= Vector2.One * 32 * dust.scale;
                dust.noLightEmittence = true;
            }

            Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 32 * dust.scale;

            if (dust.noGravity)
                dust.scale *= 1 - (0.04f + (.04f * dust.fadeIn));
            else
                dust.scale *= 0.96f;

            Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * 32 * dust.scale;

            dust.rotation += 0.06f;
            dust.position += currentCenter - nextCenter;


            dust.position += dust.velocity; 

            dust.velocity *= 0.94f;

            if (!dust.noLight)
                Lighting.AddLight(currentCenter, dust.color.R * dust.scale * 0.005f, dust.color.G * dust.scale * 0.005f, dust.color.B * dust.scale * 0.005f);

            if (dust.scale < 0.05f)
            {
                dust.active = false;
            }


            dust.shader.UseColor(dust.color * dust.fadeIn);


            if (dust.customData is GlowFlareBehavior gfb)
            {
                dust.shader.UseOpacity(gfb.glowThreshold);
                dust.shader.UseSaturation(gfb.glowPower);
                dust.shader.UseTargetPosition(new Vector2(gfb.totalBoost, 1f));

            }
            else
            {
                dust.shader.UseOpacity(0.4f);
                dust.shader.UseSaturation(2.5f);
                dust.shader.UseTargetPosition(new Vector2(1f, 1f));
            }

            return false;
        }

    }

    public class GlowFlareBehavior
    {
        public float glowThreshold = 0.4f;
        public float glowPower = 2.5f;
        public float totalBoost = 1f;

        public GlowFlareBehavior(float GlowThreshold = 0.4f, float GlowPower = 2.5f)
        {
            glowThreshold = GlowThreshold;
            glowPower = GlowPower;
        }

        public GlowFlareBehavior(float GlowThreshold = 0.4f, float GlowPower = 2.5f, float TotalBoost = 1f)
        {
            glowThreshold = GlowThreshold;
            glowPower = GlowPower;
            totalBoost = TotalBoost;
        }
    }
}