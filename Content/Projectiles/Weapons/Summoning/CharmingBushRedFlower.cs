#region Using directives

using Terraria;

using Terraria.ModLoader;

using Microsoft.Xna.Framework;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
	internal sealed class CharmingBushRedFlower : CharmingBushFlower
	{

		protected override void ShootAt(NPC target)
		{
			Vector2 shootDirection = Vector2.Normalize(target.Center - Projectile.Center) * 6f;

			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootDirection, ModContent.ProjectileType<CharmingBushRedFlowerPetal>(),
				Projectile.damage, Projectile.knockBack, Projectile.owner);
		}
	}
}
