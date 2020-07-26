using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using AerovelenceMod.Dusts;

namespace AerovelenceMod.Projectiles
{
    public class TumblerShockBlast : ModProjectile
    {
        public float WaveAmplitude = 175f;
        public float WaveFrequency = 0.5f;
        private int Timer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shock blast");
        }
        public override void SetDefaults()
        {
            projectile.width = 7;
            projectile.height = 7;
            projectile.aiStyle = -1;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 300;

        }
        private int TimeLeft = 0;
        public override void AI()
        {
            {
                TimeLeft++;
                projectile.rotation = projectile.velocity.ToRotation() + 1.57f;
                float cos = 1 + (float)Math.Cos(projectile.ai[1]);
                float sin = 1 + (float)Math.Sin(projectile.ai[1]);
                Lighting.AddLight(projectile.Center, 1f, 1f, 0.25f);
                Dust.NewDustPerfect(projectile.Center, DustID.Electric, (projectile.velocity * 0.5f).RotatedBy((float)Math.Sin(Timer++ * WaveFrequency) * WaveAmplitude), 0, default, 1.4f);
            }
        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<RainbowDust>());
                Main.dust[dust].noGravity = false;
                Main.dust[dust].velocity *= 2.5f;
            }
        }
    }
}