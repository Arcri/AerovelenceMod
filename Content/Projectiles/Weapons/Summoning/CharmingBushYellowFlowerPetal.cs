#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
	internal sealed class CharmingBushYellowFlowerPetal : ModProjectile
	{
		public override string Texture => AerovelenceMod.ProjectileAssets + "YellowPetal";

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.MinionShot[projectile.type] = true;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 4;

			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			if (projectile.localAI[0] == 0)
			{
				DustEffect();
				projectile.localAI[0] = 1;
			}

			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

			return (false);
		}

		public override void Kill(int timeLeft)
			=> DustEffect();

		private void DustEffect()
		{
			for (int i = 0; i < 5; ++i)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.YellowPetal>(), projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 0.5f);
			}
		}
	}
}
