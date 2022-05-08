#region Using directives

using Terraria;

using Terraria.ModLoader;

using Microsoft.Xna.Framework;

#endregion

namespace AerovelenceMod.Content.Projectiles.Weapons.Summoning
{
	internal sealed class CharmingBushGreenFlower : CharmingBushFlower
	{
		protected override float ShootRange => 440;

		protected override int ShootCooldown => 50;


		protected override void ShootAt(NPC target)
		{
			Vector2 shootDirection = Vector2.Normalize(target.Center - Projectile.Center) * 16f;

			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootDirection, ModContent.ProjectileType<CharmingBushGreenFlowerBeam>(),
				(int)(Projectile.damage * 0.33f), Projectile.knockBack, Projectile.owner, Projectile.whoAmI);
		}
	}
}
