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
using Terraria.ModLoader.IO;
using AerovelenceMod.Effects.Dyes;

namespace AerovelenceMod.Content.Dusts
{
    public class ColorSmoke : ModDust
    {
        public override string Texture => "AerovelenceMod/Content/Dusts/ColorSmoke2";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;

            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Dusts/ColorSmoke2").Value;
            dust.frame = new Rectangle(0, texture.Height / 3 * Main.rand.Next(3), texture.Width, texture.Height / 3); ;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color black = Color.Black;
            Color gray = new Color(25, 25, 25);
            Color ret;
            if (dust.alpha < 80)
            {
                ret = Color.Lerp(Color.Yellow, Color.Orange, dust.alpha / 80f * 0.5f);
            }
            else if (dust.alpha < 140)
            {
                ret = Color.Lerp(Color.Orange, Color.Black, (dust.alpha - 80) / 80f * 0.5f);
            }
            else
                ret = gray;
            return ret * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            //Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 32 * dust.scale;
            //Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * 32 * dust.scale;
            //dust.position += currentCenter - nextCenter;


            dust.position += dust.velocity; //Idk why we have to do this ourselves

            dust.velocity.Y -= 0.05f;

            if (dust.velocity.Length() > 3)
                dust.velocity *= 0.85f;
            else
                dust.velocity *= 0.92f;

            if (dust.alpha > 100)
            {
                dust.scale *= 0.975f;
                dust.alpha += 2;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }
}