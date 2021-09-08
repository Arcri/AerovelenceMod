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

		public override bool Autoload(ref string name)
			=> mod.Properties.Autoload;

		protected override void ShootAt(NPC target)
		{
			Vector2 shootDirection = Vector2.Normalize(target.Center - projectile.Center) * 16f;

			Projectile.NewProjectile(projectile.Center, shootDirection, ModContent.ProjectileType<CharmingBushGreenFlowerBeam>(),
				(int)(projectile.damage * 0.33f), projectile.knockBack, projectile.owner, projectile.whoAmI);
		}
	}
}
