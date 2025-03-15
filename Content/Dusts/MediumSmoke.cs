using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
using Steamworks;
using Terraria.Map;

namespace AerovelenceMod.Content.Dusts
{
    public class MediumSmoke : ModDust
    {
        public override string Texture => "AerovelenceMod/Content/Dusts/Textures/MediumWhiteSmoke";

        public override void OnSpawn(Dust dust)
        {
            //alpha is used as a timer 
            dust.alpha = 0;

            dust.customData = null;
        }
        
        public override bool Update(Dust dust)
        {
            if (dust.customData == null)
            {
                dust.customData = new MediumSmokeBehavior(22, 0.98f, 0.01f, 1f);
            }

            if (dust.customData is MediumSmokeBehavior msb)
            {
                dust.velocity *= msb.velShrinkAmount;
                dust.scale += msb.scaleGrowthAmount;

                float halftime = (msb.maxTime / 2f);

                msb.totalOpacity = (float)Math.Pow((halftime - Math.Abs(halftime - dust.alpha)) / halftime, 0.75);

                if (dust.alpha >= msb.maxTime)
                    dust.active = false;
            }

            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X * 0.05f;


            dust.alpha++;
            return false;
        }


        public override bool PreDraw(Dust dust)
        {
            Texture2D TexMain = Mod.Assets.Request<Texture2D>("Content/Dusts/Textures/MediumWhiteSmoke").Value;

            if (dust.customData is MediumSmokeBehavior msb)
            {
                if (msb.currentFrame == -1)
                    Main.NewText("How did this happen | MediumSmoke frame -1");
                
                Vector2 drawPos = dust.position - Main.screenPosition;

                int frameHeight = TexMain.Height / 3;
                int startY = frameHeight * (int)msb.currentFrame;
                Rectangle sourceRectangle = new Rectangle(0, startY, TexMain.Width, frameHeight);
                Vector2 origin = sourceRectangle.Size() / 2f;

                Color lightColor = Lighting.GetColor((int)dust.position.X / 16, (int)dust.position.Y / 16);

                Color col = (lightColor.MultiplyRGBA(dust.color) * msb.totalOpacity);

                Main.EntitySpriteDraw(TexMain, drawPos, sourceRectangle, col * msb.colorIntensity, dust.rotation, origin, dust.scale * 1f, SpriteEffects.None);
            }

            return false;
        }

    }

    public class MediumSmokeBehavior
    {
        public int currentFrame = -1;
        public float totalOpacity = 1f;

        public float scaleGrowthAmount = 0.01f;
        public float velShrinkAmount = 0.98f;
        public float colorIntensity = 1f;

        public int maxTime = 22;

        public MediumSmokeBehavior(int MaxTime = 22, float VelShrinkAmount = 0.98f, float ScaleGrowthAmount = 0.01f, float ColorIntensity = 1f)
        {
            maxTime = MaxTime;
            velShrinkAmount = VelShrinkAmount;
            scaleGrowthAmount = ScaleGrowthAmount;
            colorIntensity = ColorIntensity;

            currentFrame = Main.rand.Next(0, 3);
        }

        public MediumSmokeBehavior(int MaxTime = 22, float ColorIntensity = 1f)
        {
            maxTime = MaxTime;
            colorIntensity = ColorIntensity;

            currentFrame = Main.rand.Next(0, 3);
        }
    }

}