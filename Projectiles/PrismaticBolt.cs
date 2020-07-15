using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using AerovelenceMod.Dusts;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Projectiles
{
    public class PrismaticBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismatic Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 7;
            projectile.height = 7;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
			projectile.alpha = 255;
            projectile.timeLeft = 300;

        }

        private int TimeLeft = 0;

        public override void AI()
        {
            TimeLeft++;

            projectile.ai[1] += 0.1f;
            projectile.rotation = projectile.velocity.ToRotation() + 1.57f;

            float cos = 1 + (float)Math.Cos(projectile.ai[1]);
            float sin = 1 + (float)Math.Sin(projectile.ai[1]);
            Color color = new Color(0.5f + cos * 0.2f, 0.8f, 0.5f + sin * 0.25f);
            Lighting.AddLight(projectile.Center, color.ToVector3() * 0.6f);

            Dust d = Dust.NewDustPerfect(projectile.Center, 264, -projectile.velocity * 0.5f, 0, color, 1.4f);
            d.noGravity = true;
            d.rotation = Main.rand.NextFloat(6.50f);

            if (TimeLeft % 100 == 0)
            {
                projectile.Kill();
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.GetTexture(Texture);

            float sin = 1 + (float)Math.Sin(projectile.ai[1]);
            float cos = 1 + (float)Math.Cos(projectile.ai[1]);
            Color color = new Color(0.5f + cos * 0.2f, 0.8f, 0.5f + sin * 0.2f);

            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, tex.Frame(), color, projectile.rotation, tex.Size() / 2, projectile.scale, 0, 0);
            return false;
        }
    }
}