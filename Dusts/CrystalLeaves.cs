using AerovelenceMod;
using AerovelenceMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Dusts
{
    public class CrystalLeaves : ModDust
    {
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return lightColor;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.fadeIn = Main.rand.NextFloat(4.5f);
            dust.scale *= 1.5f;
            dust.noLight = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position.Y += dust.velocity.Y;
            dust.velocity.Y += 0.01f;
            dust.position.X += (float)Math.Sin(AeroWorld.rotationTime + dust.fadeIn) * dust.scale * dust.velocity.X * 0.5f;
            dust.rotation = (float)Math.Sin(AeroWorld.rotationTime + dust.fadeIn) * 0.5f;
            dust.scale *= 0.99f;
            dust.color *= 0.90f;
            if (dust.scale <= 0.4)
            {
                dust.active = false;
            }
            return false;
        }
    }
}