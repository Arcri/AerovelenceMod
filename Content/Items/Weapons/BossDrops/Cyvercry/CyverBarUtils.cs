using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System.Linq;
using System;
using Terraria.DataStructures;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry.Oblivion;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry
{
	internal static class CyverBarUtils
	{
		public static void DrawCyverBar(PlayerDrawSet pds, float barProgress, float barVisualProgress, float justShotPower, float barFadeIn)
		{
            Player Player = pds.drawPlayer;

            bool drainingBar = Player.GetModPlayer<OblivionBarPlayer>().decreaseBar;

            //BTW we cast positions to ints because otherwise its really shaky for some reason

            float barRot = 0f;
            float barScale = 1f;

            //Ensures that this isn't drawn multiple times if the player has an afterimage
            if (pds.shadow == 0f)
            {
                Vector2 drawPos = Player.MountedCenter - Main.screenPosition - new Vector2(0, -32f - Player.gfxOffY) + new Vector2(0f, (int)(-15f * Easings.easeInCirc(1f - barFadeIn)));

                Texture2D BarTex = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/CyverCannonBar").Value;
                Texture2D BarBorder = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/CyverCannonBarBorderGlow").Value;
                Texture2D BarFill = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/CyverCannonBarFill").Value;

                float fillPercent = barProgress;

                if (barVisualProgress > 0f)
                {
                    fillPercent = barVisualProgress;
                    justShotPower = 1f;
                }

                float bonusScale = 1f + (Easings.easeInQuad(justShotPower) * 0.12f) * (drainingBar ? 0f : 1f);
                bonusScale *= barScale;

                //Border
                for (int i = 220; i < 3; i++)
                {
                    Color col = Color.HotPink;

                    //float offsetRot = ((float)Main.timeForVisualEffects * 0.25f) + ((MathHelper.TwoPi / 4f) * i);
                    //Vector2 offsetPos = drawPos + new Vector2(1.5f, 0f).RotatedBy(offsetRot);

                    Vector2 randPos = drawPos + Main.rand.NextVector2Circular(1f, 1f);

                    DrawData Border = new DrawData(BarBorder, new Vector2((int)randPos.X, (int)randPos.Y), null,
                        col with { A = 0 } * 0.2f, barRot, BarBorder.Size() / 2f, 1f * bonusScale, SpriteEffects.None);
                    pds.DrawDataCache.Add(Border);
                }

                Vector2 barVec2Scale = new Vector2(1f * Easings.easeInOutBack(barFadeIn, 0f, 1.5f), 1f) * bonusScale;

                //Border
                if (fillPercent >= 1f)
                {
                    float sineAlpha = (float)(Math.Sin(Main.timeForVisualEffects * 0.1f)) * 0.15f;

                    DrawData Border = new DrawData(BarBorder, new Vector2((int)drawPos.X, (int)drawPos.Y), null,
                        Color.HotPink with { A = 0 } * (0.2f + sineAlpha) * barFadeIn, barRot, BarBorder.Size() / 2f, barVec2Scale, SpriteEffects.None);
                    pds.DrawDataCache.Add(Border);
                }

                //BaseBar
                DrawData Bar = new DrawData(BarTex, new Vector2((int)drawPos.X, (int)drawPos.Y), null,
                    Color.White * barFadeIn, barRot, BarTex.Size() / 2f, barVec2Scale, SpriteEffects.None);
                pds.DrawDataCache.Add(Bar);



                int fillAmount = (fillPercent > 0.99f) ? BarFill.Width : (int)(BarFill.Width * fillPercent);
                Rectangle fillFrame = new Rectangle(0, 0, fillAmount, BarFill.Height);

                //Fill
                Color betweenPink = new Color(240, 12, 73);// Color.Lerp(Color.DeepPink, Color.HotPink, 0.05f);

                if (fillPercent >= 1f)
                {
                    float sinVal = (float)(Math.Sin(Main.timeForVisualEffects * 0.1f)) * 0.07f;

                    betweenPink = Color.Lerp(betweenPink, Color.Pink, 0.02f + (0.07f + sinVal));
                }

                Color fillColor = Color.Lerp(betweenPink, Color.White, justShotPower);

                if (drainingBar)
                    fillColor = Color.Lerp(betweenPink, Color.White, 0.2f + (float)Math.Sin(Main.timeForVisualEffects * 0.2f) * 0.05f);
                    //fillColor = Color.Lerp(betweenPink, Color.HotPink, 0.55f + (float)Math.Sin(Main.timeForVisualEffects * 0.2f) * 0.08f);

                Vector2 fillScale = new Vector2(1f, 1f + justShotPower * 0.2f);
                fillScale.X *= Easings.easeInOutBack(barFadeIn, 0f, 1.5f);

                for (int i = 0; i < 4; i++)
                {
                    Vector2 randPos = drawPos + Main.rand.NextVector2Circular(1.5f, 1.5f);

                    DrawData FillUnder = new DrawData(BarFill, new Vector2((int)randPos.X, (int)randPos.Y), fillFrame,
                        fillColor with { A = 0 } * 0.25f * barFadeIn, barRot, BarFill.Size() / 2f, fillScale * bonusScale, SpriteEffects.None);
                    pds.DrawDataCache.Add(FillUnder);
                }

                DrawData Fill = new DrawData(BarFill, new Vector2((int)drawPos.X, (int)drawPos.Y), fillFrame,
                    fillColor with { A = 0 } * 1f * barFadeIn, barRot, BarFill.Size() / 2f, fillScale * bonusScale, SpriteEffects.None);
                pds.DrawDataCache.Add(Fill);

            }
        }
    }
}
