#region Using directives

using Terraria;

using Terraria.ModLoader;

using Microsoft.Xna.Framework;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
	internal sealed class CharmingBushRedFlower : CharmingBushFlower
	{
		public override bool Autoload(ref string name)
			=> mod.Properties.Autoload;

		protected override void ShootAt(NPC target)
		{
			Vector2 shootDirection = Vector2.Normalize(target.Center - projectile.Center) * 6f;

			Projectile.NewProjectile(projectile.Center, shootDirection, ModContent.ProjectileType<CharmingBushRedFlowerPetal>(),
				projectile.damage, projectile.knockBack, projectile.owner);
		}
	}
}
