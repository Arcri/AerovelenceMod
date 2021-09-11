#region Using directives

using Terraria;
using Terraria.ModLoader;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
	public sealed class GrowthFlower : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 3;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 20;

			projectile.penetrate = -1;
			projectile.timeLeft = 180;

			projectile.magic = true;
			projectile.melee = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			projectile.velocity *= 0;
			projectile.scale += 0.005f;

			if (++projectile.ai[0] == 40)
			{
				projectile.frame++;
				projectile.damage /= 2;
				projectile.width = projectile.height = 24;
			}
			else if (projectile.ai[0] == 80)
			{
				projectile.frame++;
				projectile.damage /= 2;
				projectile.width = projectile.height = 28;
			}

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 15; ++i)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.RedPetal>(), 0, 0, 100);
			}
		}
	}
}
