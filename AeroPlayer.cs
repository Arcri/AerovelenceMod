using Terraria.ID;
using Terraria;
using Terraria.GameInput;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using AerovelenceMod.Common.Systems;
using ReLogic.Content;
using AerovelenceMod.Content.Items.Weapons.Misc.Magic;
using static Humanizer.In;
using System;

namespace AerovelenceMod
{
	public class AeroPlayer : ModPlayer
	{

		public int PlatformTimer = 0;

		public float ScreenShakePower;

		public override void ModifyScreenPosition()
		{
			if (ScreenShakePower > 0.1f) //Kackbrise#5454 <3 <3 Thank
			{
				//This runs less often at lower frame rates (and vice versa) so this normalizes that 
				float adjustedValue = ScreenShakePower * (Main.frameRate / 144f);

                float totalIntensity = adjustedValue * ModContent.GetInstance<AeroClientConfig>().ScreenshakeIntensity;

				if (totalIntensity > 0)
					Main.screenPosition += new Vector2(Main.rand.NextFloat(totalIntensity), Main.rand.NextFloat(totalIntensity));
				ScreenShakePower *= 0.9f;
			}
		}

		public override void PreUpdate()
		{
			PlatformTimer--;
		}

		#region Usestyle Code
		public object useStyleData;
		public int useStyleInt;

		public int fireballFrame;
		public float glowPercent;
		public float fireBallAlpha;

		public float KWRot;

		public float[] orbGlowAmount = new float[5];

		//Draw Layer

		//TODO move this with the weapon
		private class CthulhusWrathDrawLayer : PlayerDrawLayer
		{
			private Asset<Texture2D> fireBallTex;
            private Asset<Texture2D> fireBallGlowTex;

            private Asset<Texture2D> KW;
            private Asset<Texture2D> KWWhite;
            private Asset<Texture2D> KWGlow;

            private Asset<Texture2D> Line;
            private Asset<Texture2D> Glow;
            private Asset<Texture2D> Glow2;


            public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
			{
				return drawInfo.drawPlayer.HeldItem?.type == ModContent.ItemType<CthulhusWrath>() &&
					drawInfo.drawPlayer.GetModPlayer<AeroPlayer>().useStyleData is (Vector2[]) &&
					drawInfo.drawPlayer.controlUseItem &&
					drawInfo.drawPlayer.CheckMana(drawInfo.drawPlayer.HeldItem);
			}
			public override Position GetDefaultPosition()
			{
				return new BeforeParent(PlayerDrawLayers.HeldItem);
			}
			protected override void Draw(ref PlayerDrawSet drawInfo)
			{

				//textures
				if (fireBallTex == null)
                    fireBallTex = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/KWFireball");

                if (fireBallGlowTex == null)
                    fireBallGlowTex = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/KWFireballGlow");


                if (KW == null)
                    KW = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/CthulhusWrath");

                if (KWWhite == null)
                    KWWhite = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/KWWhite");

                if (KWGlow == null)
                    KWGlow = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/KWWhiteGlow");

                if (Line == null)
                    Line = ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Medusa_Gray");

                if (Glow == null)
                    Glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/impact_2fade2");

                if (Glow2 == null)
                    Glow2 = ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/RainbowRod");

				if (drawInfo.shadow != 0f)
					return;

                Vector2 itemOrigin = new Vector2(0, KW.Height());
				float rotation = drawInfo.drawPlayer.GetModPlayer<AeroPlayer>().KWRot + MathHelper.PiOver4;
				//Vector2 armPosition = drawInfo.drawPlayer.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation); // get position of hand

				//armPosition.Y += drawInfo.drawPlayer.gfxOffY;

				Vector2 drawPos = drawInfo.drawPlayer.MountedCenter - Main.screenPosition;
                drawPos.Y += drawInfo.drawPlayer.gfxOffY;


                drawInfo.DrawDataCache.Add(new DrawData(KW.Value, drawPos, null, Color.White * 1f, rotation, itemOrigin, 1f, SpriteEffects.None, 0));

                float intensity = drawInfo.drawPlayer.GetModPlayer<AeroPlayer>().glowPercent;


                int numberOfFrames = 6;
                int frameHeight = fireBallTex.Height() / numberOfFrames;
                int startY = frameHeight * drawInfo.drawPlayer.GetModPlayer<AeroPlayer>().fireballFrame;

                // Get this frame on texture
                Rectangle sourceRectangle = new Rectangle(0, startY, fireBallTex.Width(), frameHeight);
                Vector2 origin = sourceRectangle.Size() / 2f;

                Vector2[] orbs = (Vector2[])drawInfo.drawPlayer.GetModPlayer<AeroPlayer>().useStyleData;
				for (int i = 0; i < orbs.Length; i++)
				{

                    Vector2 pos = drawInfo.drawPlayer.MountedCenter + (orbs[i] * 4) - Main.screenPosition;

					float orbAlpha = drawInfo.drawPlayer.GetModPlayer<AeroPlayer>().fireBallAlpha;


                    float glowOrbAmount = drawInfo.drawPlayer.GetModPlayer<AeroPlayer>().orbGlowAmount[i];

                    //drawInfo.DrawDataCache.Add(new DrawData(Glow2.Value, pos, null, Color.Orange with { A = 0 } * 1f, (float)Main.timeForVisualEffects * 0.02f + orbs[i].ToRotation(), Glow2.Size() / 2, glowOrbAmount * orbAlpha, SpriteEffects.None, 0));
                    //drawInfo.DrawDataCache.Add(new DrawData(Glow2.Value, pos, null, Color.White with { A = 0 } * 1f, (float)Main.timeForVisualEffects * 0.02f + orbs[i].ToRotation(), Glow2.Size() / 2, glowOrbAmount * 0.75f * orbAlpha, SpriteEffects.None, 0));

                    drawInfo.DrawDataCache.Add(new DrawData(fireBallTex.Value, pos, sourceRectangle, Color.Yellow * 0.2f * orbAlpha, orbs[i].ToRotation(), origin, 1.4f, SpriteEffects.None, 0));
					drawInfo.DrawDataCache.Add(new DrawData(fireBallTex.Value, pos + new Vector2(Main.rand.NextFloat(0,2), Main.rand.NextFloat(0, 2)), sourceRectangle, Color.White * 1f * orbAlpha, orbs[i].ToRotation(), origin, 1f, SpriteEffects.None, 0));

                    //drawInfo.DrawDataCache.Add(new DrawData(fireBallGlowTex.Value, pos, sourceRectangle, Color.White * 1f * orbAlpha * glowOrbAmount, orbs[i].ToRotation(), origin, 1f, SpriteEffects.None, 0));

                    drawInfo.DrawDataCache.Add(new DrawData(KWWhite.Value, drawPos, null, Color.White * intensity, rotation, itemOrigin, 1f, SpriteEffects.None, 0));
                    drawInfo.DrawDataCache.Add(new DrawData(KWGlow.Value, drawPos + Main.rand.NextVector2Circular(2.5f, 2.5f), null, Color.Orange with { A = 0 } * intensity * 0.8f, rotation, itemOrigin, 1f, SpriteEffects.None, 0));


				}
			}
			public override void Unload()
			{
                fireBallTex = null;
				fireBallGlowTex = null;

                KW = null;
                KWGlow = null;
                KWWhite = null;

				Line = null;
                Glow = null;
                Glow2 = null;

            }
        }
		#endregion
	}
}
