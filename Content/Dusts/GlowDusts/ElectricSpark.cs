using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common.Systems;

namespace AerovelenceMod.Content.Dusts.GlowDusts
{
	public class ElectricSparkBasic : ModDust
	{
        public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/ElectricSparkTexture";


        public override void OnSpawn(Dust dust)
		{
			//Alpha is used as a timer in this dust
			dust.alpha = 0;

			//FadeIn is used as the opacity
			dust.fadeIn = 1f;

			//customData is used as the current dust frame
			dust.customData = 2;

			dust.noGravity = true;
			dust.noLight = true;
		}

		public override bool Update(Dust dust)
		{
			dust.velocity *= 0.92f;

            dust.rotation = dust.velocity.ToRotation();

			dust.fadeIn *= 0.89f;

            if (dust.fadeIn <= 0.02f)
                dust.active = false;

            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.5f * dust.scale);

			dust.position += dust.velocity;

            dust.alpha++;

            if (dust.alpha % 3 == 0)
                dust.customData = ((int)dust.customData + 1) % 6;

            return false;
		}


		public override bool PreDraw(Dust dust)
		{
            Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/Dusts/GlowDusts/DustTextures/ElectricSparkTexture").Value;

            Vector2 drawPos = dust.position - Main.screenPosition;// + offset;

            Color col = dust.color * dust.fadeIn;

            Vector2 vec2Scale = new Vector2(1f, 1f);


            int frameHeight = Tex.Height / 6;
            int startY = frameHeight * (int)dust.customData;
            Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.spriteBatch.Draw(Tex, drawPos, sourceRectangle, col, dust.rotation, origin, vec2Scale * dust.scale, SpriteEffects.None, 0f);

            return false;
		}
	}

    public class ElectricSparkGlow : ModDust
    {
        public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/ElectricSparkTexture";

        //Todo optimize by loading texture at start
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
                dust.customData = new ElectricSparkBehavior(FadeAlphaPower: 0.89f, FadeScalePower: 1f, FadeVelPower: 0.92f, Pixelize: false, XScale: 1f, YScale: 1f);
            }

            if (dust.alpha == 0)
                (dust.customData as ElectricSparkBehavior).initialVelLength = dust.velocity.Length();
            
            dust.rotation = dust.velocity.ToRotation();

            if (dust.customData is ElectricSparkBehavior esb)
            {

                if (dust.alpha >= esb.timeToStartAlphaFade)
                    dust.fadeIn *= esb.fadeAlphaPower;
                dust.scale *= esb.fadeScalePower;
                dust.velocity *= esb.fadeVelPower;

                if (dust.scale <= 0.02f || dust.fadeIn <= 0.02f)
                    dust.active = false;


                dust.alpha++;

                if (dust.alpha % esb.timeBetweenFrames == 0)
                    esb.sparkCurrentFrame = ((int)esb.sparkCurrentFrame + 1) % 6;

                if (dust.alpha == esb.killEarlyTime)
                    dust.active = false;

                if (esb.randomVelRotatePower > 0)
                {
                    //Ratio of current velocity over starting velocity
                    float dustVelPower = dust.velocity.Length() / esb.initialVelLength;
                    dust.velocity = dust.velocity.RotateRandom(esb.randomVelRotatePower * dustVelPower);
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

            if ((dust.customData as ElectricSparkBehavior).pixelize)
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
            Texture2D TexMain = Mod.Assets.Request<Texture2D>("Content/Dusts/GlowDusts/DustTextures/ElectricSparkTextureBigger").Value;
            Texture2D TexGlowUnder = Mod.Assets.Request<Texture2D>("Content/Dusts/GlowDusts/DustTextures/ElectricSparkTexture2Glow").Value;


            if (dust.customData is ElectricSparkBehavior esb)
            {
                Vector2 drawPos = dust.position - Main.screenPosition;
                Color col = dust.color * dust.fadeIn;

                int frameHeight = TexMain.Height / 6;
                int startY = frameHeight * (int)esb.sparkCurrentFrame;
                Rectangle sourceRectangle = new Rectangle(0, startY, TexMain.Width, frameHeight);
                Vector2 origin = sourceRectangle.Size() / 2f;

                Vector2 scale = dust.scale * esb.vec2Scale;

                float underGlowPower = esb.underGlowPower;
                float whiteLayerPower = esb.whiteLayerPower;

                Main.spriteBatch.Draw(TexGlowUnder, drawPos, sourceRectangle, col with { A = 0 } * underGlowPower, dust.rotation, origin, scale, SpriteEffects.None, 0f);

                if (esb.drawWhiteWithAlphaZero)
                    Main.spriteBatch.Draw(TexMain, drawPos, sourceRectangle, Color.White with { A = 0 } * dust.fadeIn * whiteLayerPower, dust.rotation, origin, scale, SpriteEffects.None, 0f);
                else
                    Main.spriteBatch.Draw(TexMain, drawPos, sourceRectangle, Color.White * dust.fadeIn * whiteLayerPower, dust.rotation, origin, scale, SpriteEffects.None, 0f);
            }
        }

    }

    public class ElectricSparkBehavior
	{
        public int sparkCurrentFrame = 0;

        public float initialVelLength = 0f;
        ///

        //Behavior
        public float fadeAlphaPower = 0.89f;

        public float fadeScalePower = 1f;

        public float fadeVelPower = 0.92f;

        public int timeBetweenFrames = 3;

        public float randomVelRotatePower = 0;
        
        //Defaults to 300 as a safety measure in case user doesn't give it scale or alpha fade
        public int killEarlyTime = 300;

        //How many frames before the alpha will start to fade (if applicable)
        public int timeToStartAlphaFade = 5;

        //Drawing
        public bool pixelize = false;

		public Vector2 vec2Scale = new Vector2(1f, 1f);

        public float underGlowPower = 2f;

        public float whiteLayerPower = 1f;

        public bool drawWhiteWithAlphaZero = true; 


        //Kitchen Sink constructor
        public ElectricSparkBehavior(float FadeAlphaPower = 0.89f, float FadeScalePower = 1f, float FadeVelPower = 0.92f, int TimeBetweenFrames = 3, int KillEarlyTime = 300, 
            bool Pixelize = false, float XScale = 1f, float YScale = 1f, float UnderGlowPower = 2f, float WhiteLayerPower = 1f, bool DrawWhiteWithAlphaZero = true)
        {
            fadeAlphaPower = FadeAlphaPower;
            fadeScalePower = FadeScalePower;
            fadeVelPower = FadeVelPower;
            timeBetweenFrames = TimeBetweenFrames;
            killEarlyTime = KillEarlyTime;
            pixelize = Pixelize;

            vec2Scale = new Vector2(XScale, YScale);
            underGlowPower = UnderGlowPower;
            whiteLayerPower = WhiteLayerPower;

            drawWhiteWithAlphaZero = DrawWhiteWithAlphaZero;
        }


        //Has fades, pixel and scale
        public ElectricSparkBehavior(float FadeAlphaPower = 0.89f, float FadeScalePower = 1f, float FadeVelPower = 0.92f, bool Pixelize = false, float XScale = 1f, float YScale = 1f)
        {
            fadeAlphaPower = FadeAlphaPower;
            fadeScalePower = FadeScalePower;
            fadeVelPower = FadeVelPower;
            pixelize = Pixelize;
            vec2Scale = new Vector2(XScale, YScale);
        }

    }
}