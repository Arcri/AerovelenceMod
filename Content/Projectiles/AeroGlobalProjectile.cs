using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles
{
	public class AeroGlobalProjectile : GlobalProjectile
	{
		public override void AI(Projectile projectile)
		{
			/*
			if (projectile.type >= 230 && projectile.type <= 235 && Main.player[projectile.owner].GetModPlayer<AeroPlayer>().UpgradedHooks)
			{
				if (projectile.type == 230)
				{
					Lighting.AddLight(projectile.Center, 0.5f, 0.1f, 0.3f);
				}
				else if (projectile.type == 231)
				{
					Lighting.AddLight(projectile.Center, 0.5f, 0.3f, 0f);
				}
				else if (projectile.type == 232)
				{
					Lighting.AddLight(projectile.Center, 0.2f, 0.3f, 0.5f);
				}
				else if (projectile.type == 233)
				{
					Lighting.AddLight(projectile.Center, 0.2f, 0.5f, 0.05f);
				}
				else if (projectile.type == 234)
				{
					Lighting.AddLight(projectile.Center, 0.5f, 0.1f, 0.1f);
				}
				else if (projectile.type == 235)
				{
					Lighting.AddLight(projectile.Center, 1, 1, 1);
				}
			}
			*/
		}
	}
}