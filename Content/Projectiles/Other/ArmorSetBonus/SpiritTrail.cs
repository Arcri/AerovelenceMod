using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Other.ArmorSetBonus
{
    public class SpiritTrail : ModProjectile
	{
		int i;
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.aiStyle = 27;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 600;
			Projectile.alpha = 255;
			Projectile.timeLeft = 30;
		}

		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			i++;
			if (i % 2 == 0)
			{
				int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width * 2, Projectile.height * 2, 59, 0f, 0f);
				Main.dust[dust].noGravity = true;
			}
			return false;
		}
	}
}