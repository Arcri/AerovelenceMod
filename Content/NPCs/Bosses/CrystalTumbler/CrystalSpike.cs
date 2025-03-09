using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class CrystalSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 166;
            Projectile.height = 42;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            int totalFrames = 6;
            int frameDuration;

            if (Projectile.frame == 0 || Projectile.frame == 6 || Projectile.frame == 12) 
            {
                frameDuration = 60;
            }
            else if (Projectile.frame == 1 || Projectile.frame == 7)
            {
                frameDuration = 20;
            }
            else
            {
                frameDuration = 5;
            }

            if (++Projectile.frameCounter >= frameDuration)
            {
                Projectile.frameCounter = 0;

                Projectile.frame++;
                if (Projectile.frame == totalFrames || Projectile.frame == totalFrames * 2)
                {

                    Projectile.Kill();
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2 dustVel = Main.rand.NextVector2Circular(2.25f, 2.25f) * Main.rand.Next(1, 3);
                dustVel += Projectile.velocity * 0.3f;

                Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.SkyBlue, Scale: Main.rand.NextFloat(0.2f, 0.35f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.3f, timeBeforeSlow: 5,
                    preSlowPower: 0.94f, postSlowPower: 0.90f, velToBeginShrink: 1f, fadePower: 0.92f, shouldFadeColor: false);
            }

            for (float i = 0; i < 6.28f; i += 0.3f)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Granite, new Vector2(MathF.Sin(i) * 1.3f, MathF.Cos(i)) * 2.4f);
                dust.velocity *= Main.rand.NextFloat(0.8f, 1.3f);
                dust.noGravity = true;
            }
        }
    }
}