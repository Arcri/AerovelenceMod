using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;

namespace AerovelenceMod.Content.Projectiles
{
	public abstract class MuzzleFlashBase : ModProjectile
	{
        public Vector2 distFromTarget = Vector2.Zero;
        public int timer = 0;
        public float fade = 1f;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.damage = 0;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
        }

        public override void AI()
        {
            if (timer == 0)
            {
                distFromTarget = Projectile.Center - Main.player[Projectile.owner].Center;
            }

            if (timer > 2)
            {
                fade = Math.Clamp(MathHelper.Lerp(fade, -0.20f, 0.2f), 0, 1);

                if (fade == 0)
                    Projectile.active = false;
            }
            Projectile.Center = Main.player[Projectile.owner].Center + distFromTarget;

            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 0.35f * (1 - fade)); 

            timer++;
        }
    }
}