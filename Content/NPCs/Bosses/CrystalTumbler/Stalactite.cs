using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class Stalactite : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 900;
            Projectile.penetrate = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 0;
            Projectile.alpha = 255;
            Projectile.velocity = Vector2.Zero;
            Projectile.ai[0] = 0f;
        }

        public override void AI()
        {
            if (Projectile.ai[0] > 0)
            {
                Projectile.ai[0]--;
                return;
            }

            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = Main.rand.NextFloat(0.8f, 1.2f);
            }

            if (Projectile.timeLeft > 60)
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha = Math.Max(Projectile.alpha - 2, 0);
                }
                Projectile.velocity = Vector2.Zero;
                if (Projectile.localAI[0] >= 300)
                {
                    Projectile.tileCollide = true;
                }
            }
            else
            {
                Projectile.velocity.Y += 0.1f * Projectile.ai[1];
            }

        }
    }
}