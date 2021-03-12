using AerovelenceMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Projectiles
{
	public class LuminousShard : ModProjectile
	{
        public override void SetStaticDefaults()
		{
			ProjectileID.Sets.Homing[projectile.type] = true;
		}
		public override void SetDefaults()
		{
			projectile.width = 14;
            projectile.netUpdate = true;
            projectile.height = 36;
			projectile.maxPenetrate = 1;
			projectile.alpha = 255;
			projectile.hostile = false;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
			projectile.friendly = false;
        }
        float rotation = 6400;
        bool runOnce = true;
        public override void AI()
		{
			if(projectile.alpha > 0)
            {
				projectile.alpha -= 5;
                projectile.netUpdate = true;
            }
            Player player = Main.player[(int)projectile.ai[0]];
            if (player.dead || !player.active)
            {
                projectile.Kill();
            }
            projectile.netUpdate = true;
            rotation -= 2;
            rotation *= 0.93f;
            if(rotation <= 0)
            {
                rotation = 0;
                projectile.netUpdate = true;
            }
            Vector2 toPlayer = projectile.Center - player.Center;
            if(rotation != 0)
                projectile.rotation = MathHelper.ToRadians(90 + rotation) + (-toPlayer).ToRotation();
            if (rotation == 0)
            {
                projectile.rotation = MathHelper.ToRadians(90 + rotation) + projectile.velocity.ToRotation();
                if (runOnce)
                {
                    if (player.active)
                    {
                        toPlayer = toPlayer.SafeNormalize(Vector2.Zero) * 12;
                        projectile.velocity = -toPlayer;
                        projectile.netUpdate = true;
                    }
                    runOnce = false;
                    Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 8, 0.8f);
                    for (int i = 0; i < 360; i += 15)
                    {
                        Vector2 circular = new Vector2(8, 0).RotatedBy(MathHelper.ToRadians(i));
                        Dust dust2 = Dust.NewDustDirect(projectile.Center - new Vector2(5) + circular, 0, 0, DustType<WispDust>(), 0, 0, projectile.alpha);
                        dust2.velocity *= 0.35f;
                        dust2.velocity += -projectile.velocity * 0.35f;
                        dust2.scale = 2.75f;
                        dust2.noGravity = true;
                        projectile.netUpdate = true;
                    }
                }
                projectile.hostile = true;
                int dust = Dust.NewDust(projectile.Center + new Vector2(-4, -4), 0, 0, DustType<WispDust>(), 0, 0, projectile.alpha, default, 1.25f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.1f;
                projectile.velocity.Y += 0.09f;
                projectile.netUpdate = true;
            }
            else
            {
                projectile.velocity *= 0.9675f;
                projectile.netUpdate = true;
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.2f / 255f, (255 - projectile.alpha) * 0.3f / 255f, (255 - projectile.alpha) * 0.8f / 255f);
            projectile.netUpdate = true;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.Center - new Vector2(5), 0, 0, DustType<WispDust>(), 0, 0, projectile.alpha);
                dust.velocity *= 0.55f;
                dust.velocity += projectile.velocity * 0.5f;
                dust.scale *= 1.75f;
                dust.noGravity = true;
                projectile.netUpdate = true;
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			projectile.Kill();
            projectile.netUpdate = true;
        }
    }
}