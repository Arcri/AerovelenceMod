using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using System.Linq;
using Terraria.Audio;
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Common.Utilities
{
	public static class GlowDustHelper
	{
		#region NewDustPerfect

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
        #endregion

        #region NewDust
        //base
        public static int DrawGlowDust(Vector2 position, int width, int height, int type, Color dustColor, float scale, ArmorShaderData effect) 
		{
			//position, width, height, type, speed x, speed y, color, scale, effect || threshold, strength
			int d = Dust.NewDust(position, width, height, type, newColor: dustColor, Scale: scale);
			Main.dust[d].shader = effect.UseColor(dustColor).UseOpacity(0.4f).UseSaturation(1.2f);
			return d;
        }

		//with shader params
		public static int DrawGlowDust(Vector2 position, int width, int height, int type, Color dustColor, float scale, float threshold, float strength, ArmorShaderData effect)
		{
			//position, width, height, type, speed x, speed y, color, scale, effect || threshold, strength
			int d = Dust.NewDust(position, width, height, type, newColor: dustColor, Scale: scale);
			Main.dust[d].shader = effect.UseColor(dustColor).UseOpacity(threshold).UseSaturation(strength);
			return d;
		}
		//with speedx speedy
		public static int DrawGlowDust(Vector2 position, int width, int height, int type, float velX, float velY, Color dustColor, float scale, ArmorShaderData effect)
		{
			//position, width, height, type, speed x, speed y, color, scale, effect || threshold, strength
			int d = Dust.NewDust(position, width, height, type, SpeedX: velX, SpeedY: velY, newColor: dustColor, Scale: scale);
			Main.dust[d].shader = effect.UseColor(dustColor).UseOpacity(0.4f).UseSaturation(1.2f);
			return d;
		}

		//with params and speedx speedy 
		public static int DrawGlowDust(Vector2 position, int width, int height, int type, float velX, float velY, Color dustColor, float scale, float threshold, float strength, ArmorShaderData effect)
		{
			//position, width, height, type, speed x, speed y, color, scale, effect || threshold, strength
			int d = Dust.NewDust(position, width, height, type, SpeedX: velX, SpeedY: velY, newColor: dustColor, Scale: scale);
			Main.dust[d].shader = effect.UseColor(dustColor).UseOpacity(threshold).UseSaturation(strength);
			return d;
		}

		#endregion
	}
}
