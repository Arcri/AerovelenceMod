using Terraria.ModLoader;

namespace AerovelenceMod.Systems.ShieldSystem
{
	public class ShieldPlayer : ModPlayer
	{
		public Shield shield = null;

		public void SetShield(Shield shield)
		{
			this.shield?.OnUnequip();
			this.shield = shield;
			this.shield.OnEquip();
		}

		public void RemoveShield(Shield shield)
		{
			this.shield.OnUnequip();
			this.shield = null;
		}
	}
}