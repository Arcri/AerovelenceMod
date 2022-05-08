#region Using directives

using Terraria;

using Terraria.ModLoader;

using Microsoft.Xna.Framework;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
	internal sealed class CharmingBushYellowFlower : CharmingBushFlower
	{

		protected override void ShootAt(NPC target)
		{
			Vector2 shootDirection = Vector2.Normalize(target.Center - Projectile.Center) * 8f;

			for (int i = 0; i < 2; ++i)
			{
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootDirection.RotatedByRandom(MathHelper.PiOver2 / 3), ModContent.ProjectileType<CharmingBushYellowFlowerPetal>(),
					(int)(Projectile.damage * 0.66f), Projectile.knockBack, Projectile.owner);
			}
		}
	}
}
