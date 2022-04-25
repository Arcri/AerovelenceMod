using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.ShieldSystem;

namespace AerovelenceMod.Content.Items.Shields
{
	public class CrystallizedWall : ShieldItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("[WIP] Crystallized Wall");
			Tooltip.SetDefault("100 shield capacity\n20 recharge rate\n12s recharge delay\nImpact shield\nHas a small chance to reflect projectiles\nReduces contact damage, but doesn't protect you from it");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			item.accessory = true;
			item.rare = ItemRarityID.Orange;
			shield.capacity = 100;
			shield.rechargeRate = 20f;
			shield.rechargeDelay = 12;
			shield.type = ShieldTypes.Impact;

			// This shield will have a small chance to reflect projectiles and reduce contact damage from NPCs. Reflected projectiles will become friendly and harm the boss. Heavily resistant to all electric-type attacks.
			// shield.capacity = 100; (Low, attacks do more damage to shield capacity unlike HP)
			// shield.rechargeRate = 15; (15% of the total shield capacity)
			// shield.rechargeDelay = 7; (Measured in seconds)
			// shield.Impact = true;
		}
	}
}
