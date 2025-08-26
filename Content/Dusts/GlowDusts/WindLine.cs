using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
using Terraria.GameContent;
using System.Security.Policy;
using Terraria.GameContent.UI.States;
using AerovelenceMod.Common.Systems;

namespace AerovelenceMod.Content.Dusts.GlowDusts
{
	public class WindLine : ModDust
	{
        public override string Texture => "AerovelenceMod/Assets/Pixel/Flare";


        public override void OnSpawn(Dust dust)
		{
			//Alpha is used as a timer in this dust
			dust.alpha = 0;

			//FadeIn is used as the opacity
			dust.fadeIn = 1f;

			dust.customData = null;

			dust.noGravity = true;
			dust.noLight = true;
		}


		public override bool Update(Dust dust)
		{
            if (dust.customData == null)
            {
                dust.customData = new WindLineBehavior();
            }

            if (dust.alpha == 0)
                (dust.customData as WindLineBehavior).initialVelLength = dust.velocity.Length();

            dust.rotation = dust.velocity.ToRotation();

            if (dust.customData is WindLineBehavior wlb)
            {
                if (dust.alpha >= wlb.timeToStartShrink)
                    wlb.vec2Scale.Y = wlb.vec2Scale.Y * wlb.shrinkYScalePower;

                dust.velocity *= wlb.velFadePower;

                if (wlb.vec2Scale.Y <= 0.07f || dust.fadeIn <= 0.02f)
                    dust.active = false;


                dust.alpha++;

                if (dust.alpha == wlb.killEarlyTime)
                    dust.active = false;


                if (wlb.randomVelRotatePower > 0)
                {
                    //Ratio of current velocity over starting velocity
                    float dustVelPower = dust.velocity.Length() / wlb.initialVelLength;
                    dust.velocity = dust.velocity.RotateRandom(wlb.randomVelRotatePower * dustVelPower);
                }

            }

            dust.position += dust.velocity;

            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.5f * dust.scale);


            return false;
		}


        public override bool PreDraw(Dust dust)
        {
            if (dust.customData == null)
                return false;

            if ((dust.customData as WindLineBehavior).pixelize)
            {
                ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.Dusts, () =>
                {
                    Draw(dust);
                });
            }
            else
            {
                Draw(dust);
            }

            return false;
        }


        //Doing this in a separate method for the sake of convenience (allows easier testing of pixelization) 
        public void Draw(Dust dust)
        {
            Texture2D Tex = Texture2D.Value;
            Vector2 drawPos = dust.position - Main.screenPosition;
            Color col = dust.color * dust.fadeIn;
            Vector2 origin = Tex.Size() / 2f;


            if (dust.customData is WindLineBehavior wlb)
            {
                Vector2 scale = dust.scale * new Vector2(wlb.vec2Scale.X, wlb.vec2Scale.Y * 0.5f);

                Main.spriteBatch.Draw(Tex, drawPos, null, col with { A = 0 } * 0.9f, dust.rotation, origin, scale, SpriteEffects.None, 0f);

                if (wlb.drawWhiteCore)
                    Main.spriteBatch.Draw(Tex, drawPos, null, Color.White with { A = 0 }, dust.rotation, origin, scale * 0.5f, SpriteEffects.None, 0f);
            }
        }
    }

    public class WindLineBehavior
	{
        public float initialVelLength = 0f;


        //Behavior - - - - - - - -

        //How much should the yScale shirnk (when timer > timeToStartShrink)
        public float shrinkYScalePower = 0.9f;

        //How much should the velocity fade
        public float velFadePower = 0.98f;

        //
        public float randomVelRotatePower = 0;
        
        //Defaults to 300 as a safety measure in case user doesn't give it scale or alpha fade
        public int killEarlyTime = 300;

        //How many frames before the yScale will start to shrink
        public int timeToStartShrink = 25;


        //Drawing - - - - - - - -
        public bool pixelize = false;

        public bool drawWhiteCore = true;

		public Vector2 vec2Scale = new Vector2(1f, 1f);


        //Basic constructor
        public WindLineBehavior(float VelFadePower = 0.95f, int TimeToStartShrink = 15, float ShrinkYScalePower = 0.5f, float XScale = 1f, float YScale = 1f, bool Pixelize = true) 
        {
            velFadePower = VelFadePower;
            timeToStartShrink = TimeToStartShrink;
            shrinkYScalePower = ShrinkYScalePower;
            vec2Scale = new Vector2(XScale, YScale);
            pixelize = Pixelize;
        }
    }
}