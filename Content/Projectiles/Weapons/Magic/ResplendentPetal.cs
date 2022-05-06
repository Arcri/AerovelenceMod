#region Using directives

using Terraria;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
	public class ResplendentPetal : ModProjectile
	{
		public override string Texture => AerovelenceMod.ProjectileAssets + "RedPetal";

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 10;

			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
			
			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
		}

		public override bool PreAI()
		{
			if (Projectile.localAI[0] == 0)
			{
				SpawnDust();

				Projectile.scale = 1.1f;
				Projectile.localAI[0] = 1;
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 15; ++i)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.RedPetal>(), 0, 0, 100);
			}
		}

		private void SpawnDust()
		{
			float amountOfDust = 16f;

			for (int i = 0; i < amountOfDust; i++)
			{
				Vector2 spinningpoint9 = Vector2.Zero;
				spinningpoint9 += -Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / amountOfDust)) * new Vector2(1f, 4f);
				spinningpoint9 = spinningpoint9.RotatedBy(Projectile.velocity.ToRotation());

				Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<Dusts.RedPetal>(), 0, 0, 50, default, 0.6f);

				dust.noGravity = true;
				dust.position = Projectile.Center + spinningpoint9;
				dust.velocity = Projectile.velocity * 0f + spinningpoint9.SafeNormalize(Vector2.UnitY) * 1f;
			}
		}
	}
}
