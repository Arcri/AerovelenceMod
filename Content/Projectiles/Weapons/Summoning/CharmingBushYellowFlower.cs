#region Using directives

using Terraria;

using Terraria.ModLoader;

using Microsoft.Xna.Framework;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
	internal sealed class CharmingBushYellowFlower : CharmingBushFlower
	{
		public override bool Autoload(ref string name)
			=> mod.Properties.Autoload;

		protected override void ShootAt(NPC target)
		{
			Vector2 shootDirection = Vector2.Normalize(target.Center - projectile.Center) * 8f;

			for (int i = 0; i < 2; ++i)
			{
				Projectile.NewProjectile(projectile.Center, shootDirection.RotatedByRandom(MathHelper.PiOver2 / 3), ModContent.ProjectileType<CharmingBushYellowFlowerPetal>(),
					(int)(projectile.damage * 0.66f), projectile.knockBack, projectile.owner);
			}
		}
	}
}
