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
			Main.projFrames[Projectile.type] = 3;
		}
		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 20;

			Projectile.penetrate = -1;
			Projectile.timeLeft = 180;

			Projectile.DamageType = DamageClass.Magic;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Projectile.velocity *= 0;
			Projectile.scale += 0.005f;

			if (++Projectile.ai[0] == 40)
			{
				Projectile.frame++;
				Projectile.damage /= 2;
				Projectile.width = Projectile.height = 24;
			}
			else if (Projectile.ai[0] == 80)
			{
				Projectile.frame++;
				Projectile.damage /= 2;
				Projectile.width = Projectile.height = 28;
			}

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 15; ++i)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.RedPetal>(), 0, 0, 100);
			}
		}
	}
}
