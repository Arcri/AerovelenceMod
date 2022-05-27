using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;

namespace AerovelenceMod.Content.Projectiles.NPCs.CrystalCaverns
{
	public class LuminousShard : ModProjectile
	{
        public override void SetStaticDefaults()
		{
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}
		public override void SetDefaults()
		{
			Projectile.width = 14;
            Projectile.netUpdate = true;
            Projectile.height = 36;
			Projectile.maxPenetrate = 1;
			Projectile.alpha = 255;
			Projectile.hostile = false;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.friendly = false;
        }
        float rotation = 6400;
        bool runOnce = true;
        public override void AI()
		{
			if(Projectile.alpha > 0)
            {
				Projectile.alpha -= 5;
                Projectile.netUpdate = true;
            }
            Player player = Main.player[(int)Projectile.ai[0]];
            if (player.dead || !player.active)
            {
                Projectile.Kill();
            }
            Projectile.netUpdate = true;
            rotation -= 2;
            rotation *= 0.93f;
            if(rotation <= 0)
            {
                rotation = 0;
                Projectile.netUpdate = true;
            }
            Vector2 toPlayer = Projectile.Center - player.Center;
            if(rotation != 0)
                Projectile.rotation = MathHelper.ToRadians(90 + rotation) + (-toPlayer).ToRotation();
            if (rotation == 0)
            {
                Projectile.rotation = MathHelper.ToRadians(90 + rotation) + Projectile.velocity.ToRotation();
                if (runOnce)
                {
                    if (player.active)
                    {
                        toPlayer = toPlayer.SafeNormalize(Vector2.Zero) * 12;
                        Projectile.velocity = -toPlayer;
                        Projectile.netUpdate = true;
                    }
                    runOnce = false;
                    SoundEngine.PlaySound(SoundID.Item, Projectile.Center);
                    for (int i = 0; i < 360; i += 15)
                    {
                        Vector2 circular = new Vector2(8, 0).RotatedBy(MathHelper.ToRadians(i));
                        Dust dust2 = Dust.NewDustDirect(Projectile.Center - new Vector2(5) + circular, 0, 0, DustType<WispDust>(), 0, 0, Projectile.alpha);
                        dust2.velocity *= 0.35f;
                        dust2.velocity += -Projectile.velocity * 0.35f;
                        dust2.scale = 2.75f;
                        dust2.noGravity = true;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.hostile = true;
                int dust = Dust.NewDust(Projectile.Center + new Vector2(-4, -4), 0, 0, DustType<WispDust>(), 0, 0, Projectile.alpha, default, 1.25f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.1f;
                Projectile.velocity.Y += 0.09f;
                Projectile.netUpdate = true;
            }
            else
            {
                Projectile.velocity *= 0.9675f;
                Projectile.netUpdate = true;
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.2f / 255f, (255 - Projectile.alpha) * 0.3f / 255f, (255 - Projectile.alpha) * 0.8f / 255f);
            Projectile.netUpdate = true;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(5), 0, 0, DustType<WispDust>(), 0, 0, Projectile.alpha);
                dust.velocity *= 0.55f;
                dust.velocity += Projectile.velocity * 0.5f;
                dust.scale *= 1.75f;
                dust.noGravity = true;
                Projectile.netUpdate = true;
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			Projectile.Kill();
            Projectile.netUpdate = true;
        }
    }
}
