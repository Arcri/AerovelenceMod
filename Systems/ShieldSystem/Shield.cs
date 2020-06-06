using Terraria;

namespace AerovelenceMod.Systems.ShieldSystem
{
	// Todo: Needs testing
	public abstract class Shield
	{
		protected Player player;

		public Shield(Player player)
		{
			this.player = player;
		}

		public void OnProjCollision(Projectile proj)
		{
			ShieldHelper.PlayShieldAnimation(player, 0f); // Todo: Rotation
			CollisionAI(proj);
		}

		public virtual void CollisionAI(Projectile proj) { }

		public virtual void OnEquip() { }

		public virtual void OnUnequip() { }
	}
}