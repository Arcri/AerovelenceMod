using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.ShieldSystem;

namespace AerovelenceMod.Content.Items.Shields
{
	public class Polarizer : ShieldItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("[WIP] Polarizer");
			Tooltip.SetDefault("140 shield capacity\n12 recharge rate\n6s recharge delay\nImpact shield\nReduces damage\nWorks sometimes, but won't fully protect you");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			item.accessory = true;
			item.rare = ItemRarityID.Orange;
			shield.capacity = 140;
			shield.rechargeRate = 12f;
			shield.rechargeDelay = 6;
			shield.type = ShieldTypes.Impact;

			// This shield will sometimes prevent damage, but unlike other shields, will only protect the player from attacks sometimes. This will still count towards shield durability. Slightly resistant to all electric-type attacks.
			// shield.capacity = 100; (Low, attacks do more damage to shield capacity unlike HP)
			// shield.rechargeRate = 15; (15% of the total shield capacity)
			// shield.rechargeDelay = 20; (Measured in seconds)
			// shield.Impact = true;
		}
	}
}
