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
	public static class DustBehaviorUtil
	{
		//Probably a better way to do this...

		//Add params later to repalce
		/// <summary>
		/// Dust rotates based on X velocity. Dust slows at one speed before a time, then a different after. 
		/// Once dust velocity length is under a threshold, it will shrink.
		/// </summary>
		/// 
		public static GlowPixelCrossBehavior AssignBehavior_GPCBase(
			float rotPower = 0.15f, int timeBeforeSlow = 3, float preSlowPower = 0.99f, float postSlowPower = 0.92f, float velToBeginShrink = 1f, float fadePower = 0.95f,
			bool shouldFadeColor = true, float colorFadePower = 0.95f)
		{
			GlowPixelCrossBehavior b = new GlowPixelCrossBehavior();

			b.behaviorToUse = GlowPixelCrossBehavior.Behavior.Base;

			b.base_rotPower = rotPower;
			b.base_timeBeforeSlow = timeBeforeSlow;
			b.base_preSlowPower = preSlowPower;
			b.base_postSlowPower = postSlowPower;
			b.base_velToBeginShrink = velToBeginShrink;
			b.base_fadePower = fadePower;

			b.base_shouldFadeColor = shouldFadeColor;
			b.base_colorFadePower = colorFadePower;

			return b;
		}
	}
}
