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
	public class CirclePulse : ModDust
	{
        public override string Texture => "AerovelenceMod/Assets/Orbs/zFadeCirclePMA";


        public override void OnSpawn(Dust dust)
		{
			//Alpha is used as a timer in this dust
			dust.alpha = 0;

			//FadeIn is used as the opacity
			dust.fadeIn = 1f;

			dust.noGravity = true;
			dust.noLight = true;

			//VERY IMPORTANT BTW
			dust.scale = 0f;
		}

		public override bool Update(Dust dust)
		{
			if (dust.customData != null)
			{
				if (dust.customData is CirclePulseBehavior b)
				{
                    if (dust.scale < 0.5f * b.size)
                        dust.scale = MathHelper.Clamp(MathHelper.Lerp(dust.scale, 0.75f * b.size, 0.06f), 0f, 0.5f * b.size);
                    else
                        dust.scale += 0.01f;


                    if (dust.scale >= 0.5f * b.size)
                        dust.fadeIn = MathHelper.Clamp(MathHelper.Lerp(dust.fadeIn, -0.2f, 0.1f), 0, 2);

                    if (dust.scale < 0.5f * b.size)
                        dust.scale = MathHelper.Clamp(MathHelper.Lerp(dust.scale, 0.75f * b.size, 0.06f), 0f, 0.5f * b.size);
                    else
                        dust.scale += 0.01f;


                    if (dust.scale >= 0.5f * b.size)
                        dust.fadeIn = MathHelper.Clamp(MathHelper.Lerp(dust.fadeIn, -0.2f, 0.1f), 0, 2);
                }
			}

            if (dust.alpha > 3)
			{
                dust.velocity *= 0.96f;
                dust.velocity *= 0.96f;
            }


            dust.rotation = dust.velocity.ToRotation();

            if (dust.fadeIn <= 0)
                dust.active = false;

            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.5f * dust.scale);

            dust.alpha += 2;

			dust.position += dust.velocity;
            dust.position += dust.velocity;
            return false;
		}


		//TODO clean this up
		public override bool PreDraw(Dust dust)
		{
            Texture2D Tex = (Texture2D)Request<Texture2D>("AerovelenceMod/Assets/Orbs/zFadeCircleBlack");

            Vector2 drawPos = dust.position - Main.screenPosition;// + offset;
            Vector2 vec2Scale = new Vector2(0.4f, 0.8f) * dust.scale;


            if (dust.customData != null)
			{
				if (dust.customData is CirclePulseBehavior behavior)
				{
					vec2Scale = behavior.vec2Scale * dust.scale;
					int timesToDraw = behavior.timesToDraw;


					if (behavior.pixelize)
					{

						//Doing UnderProjectiles instead of Dusts cuz I want it to draw under the player
						ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(behavior.drawLayer, () =>
						{
                            for (int i = 0; i < timesToDraw; i++)
                                Draw(dust, Tex, vec2Scale);
                        });
					}
					else
					{
						for (int i = 0; i < timesToDraw; i++)
							Draw(dust, Tex, vec2Scale);
					}
                }

            }
			else
			{
				Draw(dust, Tex, vec2Scale);
            }

			return false;
		}

		public void Draw(Dust dust, Texture2D Tex, Vector2 vec2Scale)
		{
            Vector2 drawPos = dust.position - Main.screenPosition;// + offset;

            Color col = dust.color with { A = 0 } * dust.fadeIn;

			if (dust.customData != null)
			{
				if (dust.customData is CirclePulseBehavior behavior)
				{
					if (behavior.drawBlackUnder)
					{
                        Texture2D Tex2 = (Texture2D)Request<Texture2D>(Texture);

                        float power = 0.6f * behavior.blackUnderPower * dust.fadeIn;

                        Main.spriteBatch.Draw(Tex2, drawPos, null, Color.Black * power, dust.rotation + MathF.PI, Tex2.Size() / 2, vec2Scale, SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(Tex2, drawPos, null, Color.Black * power * 0.6f, dust.rotation + MathF.PI, Tex2.Size() / 2, vec2Scale * 1.1f, SpriteEffects.None, 0f);
                    }
				}
			}


            Main.spriteBatch.Draw(Tex, drawPos, null, col, dust.rotation + MathF.PI, Tex.Size() / 2, vec2Scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, drawPos, null, col * 0.6f, dust.rotation + MathF.PI, Tex.Size() / 2, vec2Scale * 1.1f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Tex, drawPos, null, col, dust.rotation + MathF.PI, Tex.Size() / 2, vec2Scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, drawPos, null, col * 0.1f, dust.rotation + MathF.PI, Tex.Size() / 2, vec2Scale * 1.25f, SpriteEffects.None, 0f);
        }

	}

	public class CirclePulseBehavior
	{
		public CirclePulseBehavior(float Size = 1f, bool Pixelize = false, int TimesToDraw = 1)
		{
			size = Size;
			pixelize = Pixelize;
			timesToDraw = TimesToDraw;
		}

		public CirclePulseBehavior(float Size = 1f, bool Pixelize = false, int TimesToDraw = 1, bool DrawBlackUnder = false, float BlackUnderPower = 1f)
        {
            size = Size;
            pixelize = Pixelize;
            timesToDraw = TimesToDraw;
			drawBlackUnder = DrawBlackUnder;
			blackUnderPower = BlackUnderPower;
        }

        public CirclePulseBehavior(float Size = 1f, bool Pixelize = false, int TimesToDraw = 1, float XScale = 0.4f, float YScale = 0.8f)
        {
            size = Size;
            pixelize = Pixelize;
            timesToDraw = TimesToDraw;

			vec2Scale = new Vector2(XScale, YScale);
        }

		public RenderLayer drawLayer = RenderLayer.UnderProjectiles;

		public bool drawBlackUnder = false;
		public float blackUnderPower = 1f;


        public bool pixelize = false;
		public float size = 1f;

		public int timesToDraw = 1;

		public Vector2 vec2Scale = new Vector2(0.4f, 0.8f);
	}
}