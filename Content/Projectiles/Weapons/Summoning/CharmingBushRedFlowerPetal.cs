#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
	internal sealed class CharmingBushRedFlowerPetal : ModProjectile
	{
		public override string Texture => AerovelenceMod.ProjectileAssets + "RedPetal";

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.MinionShot[Projectile.type] = true;
		}
		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 6;

			Projectile.penetrate = 2;

			Projectile.friendly = true;
		}

		public override bool PreAI()
		{
			if (Projectile.localAI[0] == 0)
			{
				DustEffect();
				Projectile.localAI[0] = 1;
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			return (false);
		}

		public override void Kill(int timeLeft)
			=> DustEffect();

		private void DustEffect()
		{
			for (int i = 0; i < 5; ++i)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.RedPetal>(), Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 0.75f);
			}
		}
	}
}
