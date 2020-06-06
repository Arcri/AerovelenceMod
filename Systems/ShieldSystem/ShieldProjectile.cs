using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Systems.ShieldSystem
{
	public class ShieldProjectile : GlobalProjectile
	{
		public override void PostAI(Projectile projectile)
		{
			Player player = Main.player[Main.myPlayer];
			Shield shield = player.GetModPlayer<ShieldPlayer>().shield;

			if (shield == null)
				return;

			if (projectile.Hitbox.Distance(player.Center) >= 50) // Todo: Get the correct number to sync with the sprite
				shield.OnProjCollision(projectile);
		}
	}
}