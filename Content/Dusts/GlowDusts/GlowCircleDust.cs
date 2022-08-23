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
}