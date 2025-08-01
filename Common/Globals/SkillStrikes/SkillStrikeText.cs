using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using ReLogic.Graphics;
using Microsoft.Extensions.DependencyInjection;
using AerovelenceMod.Common.Utilities;
using Terraria.UI.Chat;
using Terraria.ID;

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
                    //dust.scale = Math.Clamp(MathHelper.Lerp(dust.scale, 0.25f, 0.15f), 0.5f, 0.8f);

                    float timeForPopInAnim = 22;
                    float animProgress = Math.Clamp((dust.alpha + 7) / timeForPopInAnim, 0f, 1f);

                    dust.scale = MathHelper.Lerp(0f, 1f, Easings.easeInOutBack(animProgress, 0f, 1.75f)) * 0.5f;

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
                    float alpha = dust.fadeIn;

                    DynamicSpriteFont myFont = FontAssets.DeathText.Value;
                    Vector2 origin = (myFont.MeasureString(behavior.damageNumber) / 2f) * 0.5f;

                    Vector2 posOffset = myFont.MeasureString(behavior.damageNumber) / 2f;

                    posOffset.X -= 8;
                    Vector2 drawPos = dust.position - Main.screenPosition - posOffset * 0.475f;

                    //Color outerCol = Color.Lerp(new Color(255, 160, 0), Color.White, Easings.easeInQuad(behavior.colorLerpValue)) * 0.9f;

                    Color textCol = Color.White * 1f * alpha;
                    Color borderCol = Color.Lerp(Color.Orange, Color.White, behavior.colorLerpValue) * 1f * alpha;

                    Vector2 drawScale = new Vector2(1f, 1f) * dust.scale * 1f;
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, myFont, behavior.damageNumber, drawPos, textCol * 0.5f, 0f, origin, drawScale);
                    ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, myFont, behavior.damageNumber, drawPos, borderCol, 0f, origin, drawScale);
                    ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, myFont, behavior.damageNumber, drawPos + Main.rand.NextVector2Circular(1f, 1f), borderCol with { A = 0 } * 0.15f, 0f, origin, drawScale);
                    ChatManager.DrawColorCodedString(Main.spriteBatch, myFont, behavior.damageNumber, drawPos, textCol with { A = 0 }, 0f, origin, drawScale);


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