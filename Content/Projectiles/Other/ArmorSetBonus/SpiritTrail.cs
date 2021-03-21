using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Other.ArmorSetBonus
{
    public class SpiritTrail : ModProjectile
	{
		int i;
		public override void SetDefaults()
		{
			projectile.width = 16;
			projectile.height = 16;
			projectile.friendly = true;
			projectile.aiStyle = 27;
			projectile.penetrate = -1;
			projectile.timeLeft = 600;
			projectile.alpha = 255;
			projectile.timeLeft = 30;
		}

		public override bool PreAI()
		{
			projectile.tileCollide = false;
			i++;
			if (i % 2 == 0)
			{
				int dust = Dust.NewDust(projectile.position + projectile.velocity, projectile.width * 2, projectile.height * 2, 59, 0f, 0f);
				Main.dust[dust].noGravity = true;
			}
			return false;
		}
	}
}