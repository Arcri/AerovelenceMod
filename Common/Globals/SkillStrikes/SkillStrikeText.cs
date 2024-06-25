using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using ReLogic.Graphics;
using Microsoft.Extensions.DependencyInjection;
using AerovelenceMod.Common.Utilities;

namespace AerovelenceMod.Content.Dusts.GlowDusts
{
    public class SkillStrikeText : ModDust
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 0, 0);

            //FADEIN IS USED AS THE SMOKE'S ALPHA
            dust.fadeIn = 0f;

            //ALPHA IS USED AS A TIMER
            dust.alpha = 0;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData != null)
            {
                if (dust.customData is SkillStrikeTextBehavior behavior)
                {
                    if (dust.alpha > 15)
                        behavior.secondOpacity = Math.Clamp(behavior.secondOpacity - 0.035f, 0, 1);

                    //if (dust.scale > 0.5f)
                    dust.scale = Math.Clamp(MathHelper.Lerp(dust.scale, 0.25f, 0.15f), 0.5f, 0.8f);

                    behavior.colorLerpValue = Math.Clamp(MathHelper.Lerp(behavior.colorLerpValue, -0.25f, 0.1f), 0f, 1f);

                    if (dust.alpha < 55)
                        dust.fadeIn = Math.Clamp(dust.fadeIn + 0.15f, 0.1f, 1f);

                    if (behavior.isCrit && dust.alpha > 75)
                        dust.fadeIn = Math.Clamp(dust.fadeIn - 0.005f, 0, 1);
                    else if (dust.alpha > 55)
                        dust.fadeIn = Math.Clamp(dust.fadeIn - 0.04f, 0, 1);
                }
            }

            dust.velocity.Y *= 0.85f;

            if (dust.fadeIn == 0)
                dust.active = false;

            dust.position += dust.velocity;

            dust.alpha++;
            return false;
        }


        public override bool PreDraw(Dust dust)
        {
            if (dust.customData != null)
            {
                if (dust.customData is SkillStrikeTextBehavior behavior)
                {
                    //Texture2D ExtraGlow = Mod.Assets.Request<Texture2D>("Assets/Orbs/SoftGlow").Value;

                    Color innerColor = Color.BlanchedAlmond;

                    float alpha = dust.fadeIn;

                    DynamicSpriteFont myFont = FontAssets.MouseText.Value;
                    Vector2 origin = (myFont.MeasureString(behavior.damageNumber) / 2f) * 0.5f;

                    Vector2 posOffset = myFont.MeasureString(behavior.damageNumber) / 2f;
                    Vector2 drawPos = dust.position - Main.screenPosition - posOffset * 1f;

                    ///Color col = behavior.isCrit ? new Color(255, 90, 170) : Color.Gold * 1f;
                    Color col = Color.Lerp(Color.Gold, Color.White, behavior.colorLerpValue);

                    //Based off SLR starsight ability thing
                    for (int k = 0; k < 5; k++)
                    {
                        float rotOff = (k / 5f) * MathHelper.TwoPi;

                        Vector2 off = Vector2.One.RotatedBy((Main.timeForVisualEffects * 0.08f) + rotOff) * 4f * dust.scale;
                        Main.spriteBatch.DrawString(FontAssets.DeathText.Value, behavior.damageNumber, drawPos + off, col * 0.8f * alpha, 0, origin, dust.scale * 0.9f, 0, 0);
                        
                        //if (behavior.isCrit)
                            //Main.spriteBatch.DrawString(FontAssets.DeathText.Value, behavior.damageNumber, drawPos + off, Color.Black * 0.15f * alpha, 0, origin, dust.scale, 0, 0);

                    }

                    Color outerCol = Color.Lerp(new Color(255, 160, 0), Color.White, Easings.easeInQuad(behavior.colorLerpValue)) * 0.9f;

                    Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.DeathText.Value, behavior.damageNumber, drawPos.X, drawPos.Y, innerColor * alpha, outerCol * alpha, origin, dust.scale * 0.9f);

                }
            }
            return false;
        }

    }

    public class SkillStrikeTextBehavior
    {
        public string damageNumber = "";
        public bool isCrit = false;

        public float secondOpacity = 1f;

        public float colorLerpValue = 1f;
    }

}