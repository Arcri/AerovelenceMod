using Terraria.ModLoader;

namespace AerovelenceMod.Projectiles.Weapons.Minions
{
	public abstract class Minion : ModProjectile
	{
		public override void AI()
		{
			CheckActive();
			Behavior();
		}

		public abstract void CheckActive();

		public abstract void Behavior();
	}
}