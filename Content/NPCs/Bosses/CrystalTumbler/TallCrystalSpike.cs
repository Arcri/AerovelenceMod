using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TallCrystalSpike : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 200;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;

            Projectile.frame = 0;
            Main.projFrames[Projectile.type] = 9;
        }

        public override void AI()
        {
            AnimateProjectile();
            if (Main.rand.NextBool(3))
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, Color.White, 1f);
        }

        private void AnimateProjectile()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.Kill();

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