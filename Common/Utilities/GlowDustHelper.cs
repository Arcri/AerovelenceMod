using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Graphics.Shaders;

namespace AerovelenceMod.Common.Utilities
{
	public static class GlowDustHelper
	{

		//DrawDustPerfect | 0.4 uOp, 1.2 uSat
		public static Dust DrawGlowDustPerfect(Vector2 position, int type, Vector2 velocity, Color color, float scale, float threshold, float strength, ArmorShaderData effecty)
        {

			Dust p = Dust.NewDustPerfect(position, type, velocity, newColor: color, Scale: scale);
			p.shader = effecty.UseColor(color).UseOpacity(threshold).UseSaturation(strength);
			//Position, type, velocity, Color, scale, threshold, strength
			return p;
        }

		public static Dust DrawGlowDustPerfect(Vector2 position, int type, Vector2 velocity, Color color, float scale, ArmorShaderData effecty)
        {
			Dust p = Dust.NewDustPerfect(position, type, velocity, newColor: color, Scale: scale);
			p.shader = effecty.UseColor(color).UseOpacity(0.4f).UseSaturation(1.2f);
			return p;
		}
	}
}
