using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Graphics;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Utilities;
using System.Linq;

namespace AerovelenceMod.Content.Projectiles.TempVFX
{
    public class SmokeTest : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.ignoreWater = true;
            Projectile.hostile = false;
            Projectile.friendly = false;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 370;

        }

        public Color color = Color.White;
        public float overallAlpha = 1f;
        public int frameToStartFade = 5;
        public int fadeDuration = 25;

        float rotMult = 1f;
        float alpha = 1f;
        float scale = 1f;

        int randomSmoke = 1;

        public override void AI()
        {
            
            if (timer == 0)
            {
                randomSmoke = Main.rand.NextBool() ? 1 : 2;

                Projectile.rotation = Main.rand.NextFloat(6.28f);
                rotMult = Main.rand.NextFloat(-1f, 1f);
            }

            if (timer >= 5)
            {
                float prog = Math.Clamp((timer - 5f) / fadeDuration, 0f, 1f);
                alpha = MathHelper.Lerp(1f, 0f, prog);
            }

            Projectile.rotation += (0.04f * rotMult);

            scale *= 0.99f;

            timer++;
        }



        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Smoke = Mod.Assets.Request<Texture2D>("Assets/Smoke/smoke_0" + randomSmoke).Value;
            Texture2D ExtraGlow = Mod.Assets.Request<Texture2D>("Assets/Orbs/SoftGlow").Value;

            float myscale = scale * 0.25f;

            //Color col = Color.Lerp(new Color(255, 255, 255), color, Easings.easeInQuart(alpha)) * alpha;
            Color col = color * alpha;

            Main.EntitySpriteDraw(ExtraGlow, Projectile.Center - Main.screenPosition, null, col with { A = 0 } * overallAlpha * 0.1f * alpha, Projectile.rotation, ExtraGlow.Size() / 2f, myscale * 0.5f, SpriteEffects.None);
            Main.EntitySpriteDraw(Smoke, Projectile.Center - Main.screenPosition, null, col with { A = 0 } * overallAlpha * 0.2f, Projectile.rotation, Smoke.Size() / 2f, myscale, SpriteEffects.None);

            return false;
        }
    }

}