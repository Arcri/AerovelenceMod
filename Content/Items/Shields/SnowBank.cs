using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.ShieldSystem;

namespace AerovelenceMod.Content.Items.Shields
{
	public class SnowBank : ShieldItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("[WIP] Snow Bank");
			Tooltip.SetDefault("50 shield capacity\n7.5 recharge rate\n15s recharge delay\nBubble shield\nPrevents damage\nPrevents damage from high heat");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			item.accessory = true;
			item.rare = ItemRarityID.Orange;
			shield.capacity = 50;
			shield.rechargeRate = 7.5f;
			shield.rechargeDelay = 15;
			shield.type = ShieldTypes.Bubble;


			//this.shield;
			// This shield will prevent damage from lethal liquids such as lava, and a few "hot" projectiles and tiles like Hellstone, until it's broken.
			// shield.durability = 100; (Low, similar to player HP)
			// shield.rechargeRate = 7.5; (15% of the total shield capacity)
			// shield.rechargeDelay = 10; (Measured in seconds)
			// shield.Impact = true;
		}
	}
}
