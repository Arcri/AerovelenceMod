using AerovelenceMod.Systems.ShieldSystem;
using Terraria;

namespace AerovelenceMod.Shields
{
	public class KillShield : Shield
	{
		public KillShield(Player player) : base(player) { }

		public override void CollisionAI(Projectile proj)
		{
			proj.Kill();
		}
	}
}