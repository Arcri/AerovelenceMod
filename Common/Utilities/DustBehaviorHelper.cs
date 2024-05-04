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

        public static LineSparkBehavior AssignBehavior_LSBase(
            float velFadePower = 0.97f, float preShrinkPower = 0.99f, float postShrinkPower = 0.97f, int timeToStartShrink = 40, int killEarlyTime = 60,
			float XScale = 1f, float YScale = 1f, bool shouldFadeColor = true, float colorFadePower = 0.95f)
        {
            LineSparkBehavior b = new LineSparkBehavior();

            b.base_velFadePower = velFadePower;
            b.base_preShrinkPower = preShrinkPower;
            b.base_postShrinkPower = postShrinkPower;
            b.base_timeToStartShrink = timeToStartShrink;
            b.base_killEarlyTime = killEarlyTime;
			b.Vector2DrawScale = new Vector2(XScale, YScale);

            b.base_shouldFadeColor = shouldFadeColor;
            b.base_colorFadePower = colorFadePower;

            return b;
        }

		public static SoftGlowDustBehavior AssignBehavior_SGDBase(float timeToStartFade = 5, float timeToChangeScale = 5, float fadeSpeed = 0.95f, float sizeChangeSpeed = 0.9f, int timeToKill = 60,
			float overallAlpha = 1f, bool DrawWhiteCore = false, float XScale = 1f, float YScale = 1f)
        {
            SoftGlowDustBehavior b = new SoftGlowDustBehavior();
			b.base_timeToStartFade = timeToStartFade;
			b.base_timeToChangeScale = timeToChangeScale;
			b.base_fadeSpeed = fadeSpeed;
			b.base_sizeChangeSpeed = sizeChangeSpeed;
			b.base_timeToKill = timeToKill;

			b.DrawWhiteCore = DrawWhiteCore;
			b.Vector2DrawScale = new Vector2(XScale, YScale);
			b.overallAlpha = overallAlpha;

            return b;
        }

		//Didn't use my 'base_' nomenclature for this and I don't know why
		public static HighResSmokeBehavior AssignBehavior_HRSBase(int frameToStartFade = 5, int fadeDuration = 25, float velSlowAmount = 1f, 
			float overallAlpha = 1f, bool drawSoftGlowUnder = true, float softGlowIntensity = 1f)
		{
            HighResSmokeBehavior b = new HighResSmokeBehavior();

			b.frameToStartFade = frameToStartFade;
			b.fadeDuration = fadeDuration;
			b.velSlowAmount = velSlowAmount;
			b.overallAlpha = overallAlpha;
			b.drawSoftGlowUnder = drawSoftGlowUnder;
            b.softGlowIntensity = softGlowIntensity;

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
