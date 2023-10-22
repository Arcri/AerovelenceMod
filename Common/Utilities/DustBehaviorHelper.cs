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

		public static GlowStarSharpBehavior AssignBehavior_GSSBase(
			float rotPower = 0.15f, int timeBeforeSlow = 3, float preSlowPower = 0.99f, float postSlowPower = 0.92f, float velToBeginShrink = 1f, float fadePower = 0.95f,
			bool shouldFadeColor = true, float colorFadePower = 0.95f, StarDustDrawInfo sdci = null)
		{
			GlowStarSharpBehavior b = new GlowStarSharpBehavior();

			b.behaviorToUse = GlowStarSharpBehavior.Behavior.Base;

			b.base_rotPower = rotPower;
			b.base_timeBeforeSlow = timeBeforeSlow;
			b.base_preSlowPower = preSlowPower;
			b.base_postSlowPower = postSlowPower;
			b.base_velToBeginShrink = velToBeginShrink;
			b.base_fadePower = fadePower;

			b.base_shouldFadeColor = shouldFadeColor;
			b.base_colorFadePower = colorFadePower;

			if (sdci != null)
            {
				b.DrawWhiteCore = sdci.DrawWhiteCore;
				b.DrawBlackCore = sdci.DrawBlackCore;
				b.DrawBlackUnder = sdci.DrawBlackUnder;

				b.DrawOrb = sdci.DrawOrb;
				b.OrbBlack = sdci.OrbBlack;
				b.OrbIntensity = sdci.OrbIntensity;
			}

			return b;
		}



		public class StarDustDrawInfo
		{
			public bool DrawWhiteCore = true;
			public bool DrawBlackCore = false;
			public bool DrawBlackUnder = false;

			public bool DrawOrb = false;
			public bool OrbBlack = false;
			public float OrbIntensity = 1f;

			public StarDustDrawInfo() { }

			public StarDustDrawInfo(bool DrawWhiteCore, bool DrawBlackCore, bool DrawBlackUnder)
			{
				this.DrawWhiteCore = DrawWhiteCore;
				this.DrawBlackCore = DrawBlackCore;
				this.DrawBlackUnder = DrawBlackUnder;
			}

			public StarDustDrawInfo(bool DrawOrb, float OrbIntensity)
			{
				this.DrawOrb = DrawOrb;
				this.OrbIntensity = OrbIntensity;
			}

			public StarDustDrawInfo(bool DrawOrb, bool OrbBlack, float OrbIntensity)
			{
				this.DrawOrb = DrawOrb;
				this.OrbBlack = OrbBlack;
				this.OrbIntensity = OrbIntensity;
			}

			public StarDustDrawInfo(bool DrawWhiteCore, bool DrawBlackCore, bool DrawBlackUnder, bool DrawOrb, bool OrbBlack, float OrbIntensity)
            {
				this.DrawWhiteCore = DrawWhiteCore;
				this.DrawBlackCore = DrawBlackCore;
				this.DrawBlackUnder = DrawBlackUnder;
				this.DrawOrb = DrawOrb;
				this.OrbBlack = OrbBlack;
				this.OrbIntensity = OrbIntensity;
            }
		}
	}
}
