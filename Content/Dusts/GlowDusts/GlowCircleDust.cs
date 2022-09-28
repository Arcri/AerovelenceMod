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
	public class GlowCircleDust : ModDust
	{
		
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/GlowOrb";
		public override void OnSpawn(Dust dust) 
		{
			
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 64, 64);
			dust.shader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

		}

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color; 
        }

        public override bool Update(Dust dust)
		{

			if (dust.customData is null)
			{
				dust.position -= Vector2.One * 32 * dust.scale;
				dust.customData = true;
			}
			//float plswowk = (float)Math.Atan(dust.velocity.Y / dust.velocity.X);
			//dust.rotation = plswowk;

			//The code for updating the dust is from SLR, but the shader and everything else is mine

			Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 32 * dust.scale;

			dust.scale *= 0.96f;
			Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * 32 * dust.scale;

			dust.rotation += 0.06f;
			dust.position += currentCenter - nextCenter;


			dust.position += dust.velocity; //Idk why we have to do this ourselves

			dust.velocity *= 0.94f;

			if (!dust.noLight)
				Lighting.AddLight(currentCenter, dust.color.R * dust.scale * 0.005f, dust.color.G * dust.scale * 0.005f, dust.color.B * dust.scale * 0.005f);


			if (dust.scale < 0.05f) 
			{
				dust.active = false;
			}

			return false; 

		}
	}
	public class GlowCircleRise : GlowCircleDust
	{
		public override bool Update(Dust dust)
		{
			dust.velocity.Y -= 0.04f;
			return base.Update(dust);
		}
	}

	//SoftGlow
	#region SoftGlow
	public class GlowCircleSoft : GlowCircleDust
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/SoftGlow";

	}

	public class GlowCircleRiseSoft : GlowCircleRise
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/SoftGlow";

	}
	#endregion

	#region Flare
	public class GlowCircleFlare : GlowCircleDust
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/Flare";

	}

	public class GlowCircleRiseFlare : GlowCircleRise
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/Flare";

	}
	#endregion

	#region Spinner
	public class GlowCircleSpinner : GlowCircleDust
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/Spinner";

	}

	public class GlowCircleRiseSpinner : GlowCircleRise
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/Spinner";

	}
    #endregion

    #region QuadStar
    public class GlowCircleQuadStar : GlowCircleDust
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/QuadStar";
	}
	public class GlowCircleRiseQuadStar : GlowCircleRise
	{
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/QuadStar";
	}
    #endregion

	//////////////////////////////////////
	public class GlowLine1 : ModDust
    {
		public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/GlowLine1";

		Vector3 initialColor = Color.White.ToVector3();
		Vector3 colorToUse = Color.White.ToVector3();
		public override void OnSpawn(Dust dust)
        {
			initialColor = dust.color.ToVector3();
			colorToUse = dust.color.ToVector3();
			dust.fadeIn = 0;
			dust.customData = dust.scale;

			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 128, 27);
			dust.shader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
        }

        public override bool Update(Dust dust)
        {
			if ((float)dust.customData != 0f)
			{
				//dust.position -= new Vector2(13, 64) * dust.scale;
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

	public class GlowLine1Fast : GlowLine1
    {
        public override bool Update(Dust dust)
        {
			dust.velocity *= 0.99f;
			if (Math.Abs(dust.velocity.Length()) < 0.5f)
				dust.active = false;
            return base.Update(dust);
        }
    }
}